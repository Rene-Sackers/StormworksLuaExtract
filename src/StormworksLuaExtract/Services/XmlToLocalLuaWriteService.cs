using System;
using System.IO;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class XmlToLocalLuaWriteService
	{
		public void WriteVehicleLuaScriptToFile(LuaScript luaScript)
		{
			Console.WriteLine($"Extracting Lua scripts from vehicle '{luaScript.VehicleXmlPath}'");

			var vehicleXmlScript = ScriptExtractHelper.GetScriptFromXmlFile(luaScript.VehicleXmlPath, luaScript.ObjectId);

			if (File.Exists(luaScript.LuaFilePath))
			{
				var currentLocalScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);

				if (currentLocalScript == vehicleXmlScript)
				{
					Console.WriteLine($"Nothing changed for script {luaScript.ObjectId} from vehicle {luaScript.VehicleName}.");
					return;
				}

				// Backup
				var backupFilePath = Path.Join(Statics.LocalBackupDirectory, $"{luaScript.VehicleName}_{luaScript.ObjectId} {DateTime.Now:yyyy-MM-dd HH-mm-ss}.lua");
				if (!FileHelper.TryWriteFile(backupFilePath, currentLocalScript))
					return;

				Console.WriteLine($"Wrote backup to {backupFilePath}");
			}


			FileHelper.TryWriteFile(luaScript.LuaFilePath, vehicleXmlScript);

			Console.WriteLine($"Wrote script {luaScript.ObjectId} from vehicle {luaScript.VehicleName} to {luaScript.LuaFilePath}.");
		}
	}
}
