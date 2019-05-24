using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class LocalLuaToXmlWriteService
	{
		private readonly MinifyLuaService _minifyLuaService;

		public LocalLuaToXmlWriteService(MinifyLuaService minifyLuaService)
		{
			_minifyLuaService = minifyLuaService;
		}

		public void WriteScriptToMicrocontroller(LuaScript luaScript)
		{
			Console.WriteLine($"Local lua file '{luaScript.LuaFileName}' object ID {luaScript.ObjectId} script changed.");

			if (!File.Exists(luaScript.VehicleXmlPath))
			{
				Console.WriteLine($"Target vehicle file '{luaScript.VehicleXmlPath}' not found.");
				return;
			}

			var processedScript = ProcessScript(luaScript);

			if (processedScript == null)
				return;

			if (!FileHelper.TryWriteFile(luaScript.MinifiedLuaPath, processedScript))
				return;

			var currentScript = ScriptExtractHelper.GetScriptFromXmlFile(luaScript.VehicleXmlPath, luaScript.ObjectId);
			if (currentScript == null)
			{
				Console.WriteLine($"Failed to find the Lua object with ID {luaScript.ObjectId} in vehicle file '{luaScript.VehicleXmlPath}'.");
				return;
			}

			if (currentScript == processedScript)
			{
				Console.WriteLine($"Script {luaScript.ObjectId} hasn't changed compared to the vehicle XML.");
				return;
			}

			var currentXml = FileHelper.NoTouchReadFile(luaScript.VehicleXmlPath);

			// Backup
			if (!BackupFileHelper.BackupFile(currentXml, luaScript.VehicleXmlFileName))
				return;

			processedScript = WebUtility.HtmlEncode(processedScript);

			var pattern = Statics.ObjectMatchPattern(luaScript.ObjectId);
			var newXml = Regex.Replace(currentXml, pattern, "<object id=\"${id}\" script='" + processedScript + "'>");

			// Overwrite
			if (!FileHelper.TryWriteFile(luaScript.VehicleXmlPath, newXml))
				return;

			Console.WriteLine($"Updated vehicle {luaScript.VehicleName} XML with new script with ID {luaScript.ObjectId}.");
		}

		private string ProcessScript(LuaScript luaScript)
		{
			var newScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);
			if (newScript == null)
				return null;

			Console.WriteLine($"Script length: {newScript.Length}/{Constants.LuaMaximumLength}");

			if (newScript.Length > Constants.LuaMaximumLength)
				newScript = MinifyScript(newScript);

			if (newScript == null)
				return null;

			return newScript;
		}

		private string MinifyScript(string script)
		{
			ConsoleHelper.WriteWarning($"Script is too long for game: {script.Length}/{Constants.LuaMaximumLength} characters, minifying script first.");

			script = _minifyLuaService.Minify(script) + Constants.MinifiedScriptSuffix;

			Console.WriteLine($"Length after minification: {script.Length}");

			if (script.Length > Constants.LuaMaximumLength)
			{
				ConsoleHelper.WriteWarning("Minified script still too large for game! Not updating vehicle xml.");
				return null;
			}

			return script;
		}
	}
}
