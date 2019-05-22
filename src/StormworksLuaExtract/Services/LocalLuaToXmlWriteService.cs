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
		private const int LuaMaximumLength = 4096;

		public void WriteScriptToMicrocontroller(LuaScript luaScript)
		{
			Console.WriteLine($"Local lua file '{luaScript.LuaFileName}' object ID {luaScript.ObjectId} script changed.");

			if (!File.Exists(luaScript.VehicleXmlPath))
			{
				Console.WriteLine($"Target vehicle file '{luaScript.VehicleXmlPath}' not found.");
				return;
			}

			var newScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);
			if (newScript == null)
				return;

			newScript = WebUtility.HtmlEncode(newScript);


			if (newScript.Length > LuaMaximumLength)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Warning! Script is too long: {newScript.Length}/{LuaMaximumLength}, not updating vehicle file.");
				Console.ForegroundColor = ConsoleColor.Gray;
				return;
			}

			var currentScript = ScriptExtractHelper.GetScriptFromXmlFile(luaScript.VehicleXmlPath, luaScript.ObjectId);
			if (currentScript == null)
			{
				Console.WriteLine($"Failed to find the Lua object with ID {luaScript.ObjectId} in vehicle file '{luaScript.VehicleXmlPath}'.");
				return;
			}

			if (currentScript == newScript)
			{
				Console.WriteLine($"Script {luaScript.ObjectId} hasn't changed compared to the vehicle XML.");
				return;
			}

			var currentXml = FileHelper.NoTouchReadFile(luaScript.VehicleXmlPath);

			// Backup
			if (!BackupFileHelper.BackupFile(currentXml, luaScript.VehicleName))
				return;

			var pattern = Statics.ObjectMatchPattern(luaScript.ObjectId);
			var newXml = Regex.Replace(currentXml, pattern, "<object id=\"${id}\" script='" + newScript + "'>");

			// Overwrite
			if (!FileHelper.TryWriteFile(luaScript.VehicleXmlPath, newXml))
				return;

			Console.WriteLine($"Updated vehicle {luaScript.VehicleName} XML with new script with ID {luaScript.ObjectId}.");
		}
	}
}
