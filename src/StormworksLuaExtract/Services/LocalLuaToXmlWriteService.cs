using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class LocalLuaToXmlWriteService
	{
		public void WriteScriptToMicrocontroller(LuaScript luaScript)
		{
			Console.WriteLine($"Local lua file '{luaScript.LuaFilePath}' object ID {luaScript.ObjectId} script changed.");

			if (!File.Exists(luaScript.MicrocontrollerXmlPath))
			{
				Console.WriteLine($"Target microprocessor file '{luaScript.MicrocontrollerXmlPath}' not found.");
				return;
			}

			var newScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);
			if (newScript == null)
				return;

			newScript = WebUtility.HtmlEncode(newScript);

			var currentScripts = ScriptExtractHelper.ExtractScriptsFromMicrocontrollerXml(luaScript.MicrocontrollerXmlPath).ToList();
			var currentScript = currentScripts.FirstOrDefault(s => s.ObjectId == luaScript.ObjectId);

			if (currentScript == null)
			{
				Console.WriteLine($"Failed to find the Lua object with ID {luaScript.ObjectId} in microprocessor file '{luaScript.MicrocontrollerXmlPath}'.");
				return;
			}

			if (currentScript.Script == newScript)
			{
				Console.WriteLine($"Script {luaScript.ObjectId} hasn't changed compared to the microcontroller XML.");
				return;
			}

			var currentXml = FileHelper.NoTouchReadFile(luaScript.MicrocontrollerXmlPath);

			// Backup
			var backupFilePath = Path.Join(Statics.LocalBackupDirectory, luaScript.MicrocontrollerName + $"{DateTime.Now:G}.xml");
			FileHelper.TryWriteFile(backupFilePath, currentXml);
			Console.WriteLine($"Wrote backup to {backupFilePath}");

			var pattern = Statics.ObjectMatchPattern(luaScript.ObjectId);
			var newXml = Regex.Replace(currentXml, pattern, "<${element} id=\"${id}\" script='" + newScript + "'>");

			// Overwrite
			if (!FileHelper.TryWriteFile(luaScript.MicrocontrollerXmlPath, newXml))
				return;

			Console.WriteLine($"Updated microcontroller {luaScript.MicrocontrollerName} XML with new script with ID {luaScript.ObjectId}.");
		}
	}
}
