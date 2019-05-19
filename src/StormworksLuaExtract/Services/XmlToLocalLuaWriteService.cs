using System;
using System.IO;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class XmlToLocalLuaWriteService
	{
		public bool WriteVehicleLuaScriptToFile(LuaScript luaScript)
		{
			Console.WriteLine($"Extracting Lua scripts from vehicle '{luaScript.VehicleXmlPath}'");

			var vehicleXmlScript = ScriptExtractHelper.GetScriptFromXmlFile(luaScript.VehicleXmlPath, luaScript.ObjectId);

			if (File.Exists(luaScript.LuaFilePath))
			{
				var currentLocalScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);

				if (currentLocalScript == vehicleXmlScript)
				{
					Console.WriteLine($"Nothing changed for script {luaScript.ObjectId} from vehicle {luaScript.VehicleName}.");
					return false;
				}

				// Backup
				if (!BackupFileHelper.BackupFile($"{luaScript.VehicleName}_{luaScript.ObjectId}", luaScript.VehicleName))
					return false;
			}
			
			FileHelper.TryWriteFile(luaScript.LuaFilePath, vehicleXmlScript);

			Console.WriteLine($"Wrote script {luaScript.ObjectId} from vehicle {luaScript.VehicleName} to {luaScript.LuaFilePath}.");

			return true;
		}
	}
}
