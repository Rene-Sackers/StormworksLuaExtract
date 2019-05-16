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
		private readonly MicrocontrollersWatchService _microcontrollersWatchService;
		private readonly LocalLuaToXmlWriteService _localLuaToXmlWriteService;
		private readonly XmlToLocalLuaWriteService _xmlToLocalLuaWriteService;
		private readonly List<LuaScript> _luaScripts = new List<LuaScript>();

		public ApplicationService(
			MicrocontrollersWatchService microcontrollersWatchService,
			LocalLuaToXmlWriteService localLuaToXmlWriteService,
			XmlToLocalLuaWriteService xmlToLocalLuaWriteService)
		{
			_microcontrollersWatchService = microcontrollersWatchService;
			_localLuaToXmlWriteService = localLuaToXmlWriteService;
			_xmlToLocalLuaWriteService = xmlToLocalLuaWriteService;
			_microcontrollersWatchService.MicrocontrollerAdded += MicrocontrollerAdded;
			_microcontrollersWatchService.MicrocontrollerDeleted += MicrocontrollerDeleted;
		}

		public void Run()
		{
			_microcontrollersWatchService.StartWatching();

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

		private void MicrocontrollerAdded(string xmlfilepath) =>
			AddMicrocontrollerXmlFile(xmlfilepath);

		private void MicrocontrollerDeleted(string xmlfilepath) =>
			_luaScripts.Where(s => s.MicrocontrollerXmlPath == xmlfilepath).ToList().ForEach(s => _luaScripts.Remove(s));
		
		private void AddMicrocontrollerXmlFile(string xmlFilePath)
		{
			var xmlFileScripts = ScriptExtractHelper.ExtractScriptsFromMicrocontrollerXml(xmlFilePath);
			foreach (var script in xmlFileScripts)
			{
				_luaScripts.Add(script);
				_xmlToLocalLuaWriteService.WriteMicrocontrollerLuaScriptsToFiles(script);
			}
		}

		private async void LocalLuaFileChanged(object sender, FileSystemEventArgs e)
		{
			var luaScript = _luaScripts.FirstOrDefault(ls => ls.LuaFilePath == e.FullPath);
			if (luaScript == null)
				return;

			await Task.Delay(Constants.ReadWriteTimeoutInMilliseconds);

			Console.WriteLine($"Lua file '{e.Name}' changed.");

			_localLuaToXmlWriteService.WriteScriptToMicrocontroller(luaScript);
		}
	}
}
