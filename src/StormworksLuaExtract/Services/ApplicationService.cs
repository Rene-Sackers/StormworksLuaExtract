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
		private readonly List<string> _ignoreNextLuaUpdatePaths = new List<string>();

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
				AddVehicleXmlFile(xmlFilePath);
		}

		private void VehicleAdded(string xmlfilepath) =>
			AddVehicleXmlFile(xmlfilepath);

		private void VehicleDeleted(string xmlfilepath) =>
			_luaScripts.Where(s => s.VehicleXmlPath == xmlfilepath).ToList().ForEach(s => _luaScripts.Remove(s));

		private void VehicleChanged(string xmlfilepath)
		{
			if (xmlfilepath == _ignoreNextVehicleUpdatePath)
			{
				_ignoreNextVehicleUpdatePath = null;
				return;
			}

			var savedVehicleScripts = ScriptExtractHelper.ExtractScriptsFromMicrocontrollerXml(xmlfilepath).ToList();

			var previouslyExtractedScripts = _luaScripts.Where(ls => ls.VehicleXmlPath == xmlfilepath).ToList();
			var newScripts = savedVehicleScripts.Where(vs => previouslyExtractedScripts.All(nvs => nvs.ObjectId != vs.ObjectId));
			var deletedScripts = previouslyExtractedScripts.Where(vs => savedVehicleScripts.All(nvs => nvs.ObjectId != vs.ObjectId)).ToList();

			foreach (var newScript in newScripts)
			{
				_luaScripts.Add(newScript);
				_ignoreNextLuaUpdatePaths.Add(newScript.LuaFilePath);
				_xmlToLocalLuaWriteService.WriteVehicleLuaScriptToFile(newScript);
			}

			foreach (var deletedScript in deletedScripts)
			{
				_luaScripts.Remove(deletedScript);
				var deletedScriptContent = FileHelper.NoTouchReadFile(deletedScript.LuaFilePath);
				if (BackupFileHelper.BackupFile(deletedScriptContent, deletedScript.LuaFileName))
					File.Delete(deletedScript.LuaFilePath);
			}

			foreach (var existingScript in previouslyExtractedScripts.Except(deletedScripts))
			{
				_ignoreNextLuaUpdatePaths.Add(existingScript.LuaFilePath);
				if (!_xmlToLocalLuaWriteService.WriteVehicleLuaScriptToFile(existingScript))
					_ignoreNextLuaUpdatePaths.Remove(existingScript.LuaFilePath);
			}
		}
		
		private void AddVehicleXmlFile(string xmlFilePath)
		{
			var luaScripts = ScriptExtractHelper.ExtractScriptsFromMicrocontrollerXml(xmlFilePath);
			foreach (var script in luaScripts)
			{
				_luaScripts.Add(script);
				_xmlToLocalLuaWriteService.WriteVehicleLuaScriptToFile(script);
			}
		}

		private async void LocalLuaFileChanged(object sender, FileSystemEventArgs e)
		{
			if (_ignoreNextLuaUpdatePaths.Contains(e.FullPath))
			{
				_ignoreNextLuaUpdatePaths.Remove(e.FullPath);
				return;
			}

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
