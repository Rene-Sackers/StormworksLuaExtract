using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StormworksLuaExtract.Services
{
	public class LuaScript
	{
		public string ObjectId { get; }

		public string Script { get; }

		public LuaScript(string objectId, string script)
		{
			ObjectId = objectId;
			Script = script;
		}
	}

	public class ApplicationService
	{
		private static readonly string _microprocessorsPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Stormworks\data\microprocessors");
		private static readonly string _localEditDirectory = Path.GetFullPath(@".\Workspace");

		private FileSystemEventArgs _lastProcessorFileEvent;
		private DateTime _lastProcessorFileEventTime = DateTime.Now;

		private FileSystemEventArgs _lastLuaFileEvent;
		private DateTime _lastLuaFileEventTime = DateTime.Now;

		private DateTime _lastWriteTime = DateTime.Now;

		private const int EventGroupTimeInSeconds = 2;

		public async Task Run()
		{
			if (!Directory.Exists(_microprocessorsPath))
			{
				Console.WriteLine($"Could not find microprocessors folder: {_microprocessorsPath}");
				Console.ReadKey();
				return;
			}

			if (!Directory.Exists(_localEditDirectory))
				Directory.CreateDirectory(_localEditDirectory);

			WriteExistingMicroprocessorsScripts();

			var processorsXmlWatcher = new FileSystemWatcher(_microprocessorsPath, "*.xml");
			processorsXmlWatcher.Created += MicroprocessorXmlFileChanged;
			processorsXmlWatcher.Changed += MicroprocessorXmlFileChanged;
			processorsXmlWatcher.Renamed += MicroprocessorXmlFileChanged;
			processorsXmlWatcher.Deleted += MicroprocessorXmlFileChanged;
			processorsXmlWatcher.EnableRaisingEvents = true;

			var workspaceLuaWatcher = new FileSystemWatcher(_localEditDirectory, "*.lua");
			workspaceLuaWatcher.Changed += LocalLuaFileChanged;
			workspaceLuaWatcher.EnableRaisingEvents = true;

			Console.WriteLine($"Watching directory: {_microprocessorsPath}");

			Console.ReadKey();
		}

		private static string ObjectMatchPattern(string objectId = null)
		{
			var idMatch = objectId == null ? "\\d+" : objectId;

			return "<(?<element>(?:object)|(?:c\\d)) id=\"(?<id>" + idMatch + ")\" script=[\"|'](?<script>[^>]*)['|\"]>";
		}

		private void WriteExistingMicroprocessorsScripts()
		{
			foreach (var xmlFilePath in Directory.GetFiles(_microprocessorsPath, "*.xml"))
				WriteProcessorLuaScriptsToFiles(xmlFilePath);
		}

		private void MicroprocessorXmlFileChanged(object sender, FileSystemEventArgs e)
		{
			if (_lastProcessorFileEvent?.FullPath == e.FullPath && DateTime.Now.Subtract(_lastProcessorFileEventTime).TotalSeconds <= EventGroupTimeInSeconds)
				return;

			if (DateTime.Now.Subtract(_lastWriteTime).TotalSeconds <= EventGroupTimeInSeconds)
				return;

			_lastProcessorFileEvent = e;
			_lastProcessorFileEventTime = DateTime.Now;

			Console.WriteLine($"XML file changed: {e.Name} - {e.ChangeType}");

			if (e.ChangeType == WatcherChangeTypes.Deleted)
				return;

			WriteProcessorLuaScriptsToFiles(e.FullPath);
		}

		private void WriteProcessorLuaScriptsToFiles(string xmlFilePath)
		{
			var fileName = new FileInfo(xmlFilePath).Name;

			foreach (var script in ExtractLuaFromMicroprocessorXml(xmlFilePath))
			{
				var targetFile = Path.Combine(_localEditDirectory, fileName.Replace(".xml", $"_{script.ObjectId}.lua"));
				if (File.Exists(targetFile) && NoTouchReadFile(targetFile) == script.Script)
					continue;

				Console.WriteLine($"Processor '{fileName}' object {script.ObjectId} script changed.");

				_lastWriteTime = DateTime.Now;
				File.WriteAllText(targetFile, script.Script);
			}
		}

		private IEnumerable<LuaScript> ExtractLuaFromMicroprocessorXml(string xmlFilePath)
		{
			string xml;
			try
			{
				xml = NoTouchReadFile(xmlFilePath);
			}
			catch
			{
				yield break;
			}

			var matches = Regex.Matches(xml, ObjectMatchPattern());
			foreach (Match match in matches)
			{
				var element = match.Groups["element"].Value;

				// The script is printed twice in the XML, once under a <object item, and once under a <c33 item
				if (element != "object")
					continue;

				var objectId = match.Groups["id"].Value;
				var script = match.Groups["script"].Value;
				script = System.Net.WebUtility.HtmlDecode(script);

				yield return new LuaScript(objectId, script);
			}
		}

		private void LocalLuaFileChanged(object sender, FileSystemEventArgs e)
		{
			if (_lastLuaFileEvent?.FullPath == e.FullPath && DateTime.Now.Subtract(_lastLuaFileEventTime).TotalSeconds <= EventGroupTimeInSeconds)
				return;

			if (DateTime.Now.Subtract(_lastWriteTime).TotalSeconds <= EventGroupTimeInSeconds)
				return;

			_lastLuaFileEvent = e;
			_lastLuaFileEventTime = DateTime.Now;

			WriteUpdatedLua(e.FullPath);
		}

		private void WriteUpdatedLua(string luaFilePath)
		{
			var fileName = new FileInfo(luaFilePath).Name;
			var objectId = fileName.Split('_').Last().Replace(".lua", string.Empty);
			var processorXmlFileName = fileName.Replace($"_{objectId}.lua", ".xml");
			var targetFilePath = Path.Combine(_microprocessorsPath, processorXmlFileName);

			Thread.Sleep(1000);

			Console.WriteLine($"Local lua file '{fileName}' component ID {objectId} script changed.");

			if (!File.Exists(targetFilePath))
			{
				Console.WriteLine($"Target microprocessor file '{targetFilePath}' not found.");
				return;
			}

			var newScript = NoTouchReadFile(luaFilePath);
			newScript = System.Net.WebUtility.HtmlEncode(newScript);

			var currentScripts = ExtractLuaFromMicroprocessorXml(targetFilePath);
			var currentScript = currentScripts.FirstOrDefault(s => s.ObjectId == objectId);

			if (currentScript == null)
			{
				Console.WriteLine($"Failed to find the Lua object with ID {objectId} in microprocessor file '{targetFilePath}'. Couldn't update.");
				return;
			}

			var currentXml = NoTouchReadFile(targetFilePath);
			var pattern = ObjectMatchPattern(objectId);
			var newXml = Regex.Replace(currentXml, pattern, "<${element} id=\"${id}\" script='" + newScript + "'>");

			_lastWriteTime = DateTime.Now;

			// Backup
			File.WriteAllText(targetFilePath.Replace(".xml", $" - bu{Environment.TickCount}.xml"), currentXml);

			// Overwrite
			File.WriteAllText(targetFilePath, newXml);
		}

		private string NoTouchReadFile(string path)
		{
			using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}
	}
}
