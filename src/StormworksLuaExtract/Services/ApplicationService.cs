using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class ApplicationService
	{
		private readonly VehiclesWatchService _vehiclesWatchService;
		private readonly LocalLuaToXmlWriteService _localLuaToXmlWriteService;
		private readonly XmlToLocalLuaWriteService _xmlToLocalLuaWriteService;
		private readonly List<LuaScript> _luaScripts = new List<LuaScript>();

		private string _ignoreNextVehicleUpdatePath;

		public ApplicationService(
			VehiclesWatchService vehiclesWatchService,
			LocalLuaToXmlWriteService localLuaToXmlWriteService,
			XmlToLocalLuaWriteService xmlToLocalLuaWriteService)
		{
			_vehiclesWatchService = vehiclesWatchService;
			_localLuaToXmlWriteService = localLuaToXmlWriteService;
			_xmlToLocalLuaWriteService = xmlToLocalLuaWriteService;
			_vehiclesWatchService.MicrocontrollerAdded += VehicleAdded;
			_vehiclesWatchService.MicrocontrollerDeleted += VehicleDeleted;
			_vehiclesWatchService.MicrocontrollerChanged += VehicleChanged;
		}

		public void Run()
		{
			_vehiclesWatchService.StartWatching();

			if (!Directory.Exists(Statics.MicrocontrollerPath))
			{
				Console.WriteLine($"Could not find microcontroller folder: {Statics.MicrocontrollerPath}");
				Console.ReadKey();
				return;
			}

			if (!Directory.Exists(Statics.LocalEditDirectory))
				Directory.CreateDirectory(Statics.LocalEditDirectory);

			if (!Directory.Exists(Statics.LocalBackupDirectory))
				Directory.CreateDirectory(Statics.LocalBackupDirectory);
			
			WriteExistingMicrocontrollerScripts();

			var workspaceLuaWatcher = new BufferedFileSystemWatcher(Statics.LocalEditDirectory, "*.lua");
			workspaceLuaWatcher.Changed += LocalLuaFileChanged;

			Console.WriteLine($"Watching directory: {Statics.MicrocontrollerPath}");

			Console.ReadKey();
		}

		private void WriteExistingMicrocontrollerScripts()
		{
			foreach (var xmlFilePath in Directory.GetFiles(Statics.MicrocontrollerPath, "*.xml"))
				AddMicrocontrollerXmlFile(xmlFilePath);
		}

		private void VehicleAdded(string xmlfilepath) =>
			AddMicrocontrollerXmlFile(xmlfilepath);

		private void VehicleChanged(string xmlfilepath)
		{
			if (xmlfilepath == _ignoreNextVehicleUpdatePath)
			{
				_ignoreNextVehicleUpdatePath = null;
				return;
			}

			_luaScripts.Where(s => s.VehicleXmlPath == xmlfilepath).ToList().ForEach(s => _xmlToLocalLuaWriteService.WriteVehicleLuaScriptToFile(s));
		}

		private void VehicleDeleted(string xmlfilepath) =>
			_luaScripts.Where(s => s.VehicleXmlPath == xmlfilepath).ToList().ForEach(s => _luaScripts.Remove(s));
		
		private void AddMicrocontrollerXmlFile(string xmlFilePath)
		{
			var xmlFileScripts = ScriptExtractHelper.ExtractScriptsFromMicrocontrollerXml(xmlFilePath);
			foreach (var script in xmlFileScripts)
			{
				_luaScripts.Add(script);
				_xmlToLocalLuaWriteService.WriteVehicleLuaScriptToFile(script);
			}
		}

		private async void LocalLuaFileChanged(object sender, FileSystemEventArgs e)
		{
			var luaScript = _luaScripts.FirstOrDefault(ls => ls.LuaFilePath == e.FullPath);
			if (luaScript == null)
				return;

			await Task.Delay(Constants.ReadWriteTimeoutInMilliseconds);

			Console.WriteLine($"Lua file '{e.Name}' changed.");

			_ignoreNextVehicleUpdatePath = luaScript.VehicleXmlPath;

			_localLuaToXmlWriteService.WriteScriptToMicrocontroller(luaScript);
		}
	}
}
