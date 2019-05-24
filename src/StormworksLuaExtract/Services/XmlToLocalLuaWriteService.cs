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
			Console.WriteLine($"Extracting Lua scripts from vehicle '{luaScript.VehicleName}'");

			var vehicleXmlScript = ScriptExtractHelper.GetScriptFromXmlFile(luaScript.VehicleXmlPath, luaScript.ObjectId);
			var isMinifiedScript = vehicleXmlScript.EndsWith(Constants.MinifiedScriptSuffix);

			if (File.Exists(luaScript.LuaFilePath))
			{
				var currentLocalScript = FileHelper.NoTouchReadFile(isMinifiedScript ? luaScript.MinifiedLuaPath : luaScript.LuaFilePath);

				if (currentLocalScript == vehicleXmlScript)
				{
					Console.WriteLine($"Nothing changed for script {luaScript.ObjectId} from vehicle {luaScript.VehicleName}.");
					return false;
				}

				if (isMinifiedScript)
				{
					ConsoleHelper.WriteWarning($"Script {luaScript.ObjectId} in microcontroller {luaScript.MicrocontrollerName} in vehicle {luaScript.VehicleName} was changed, but it's a minified script. Not updating local file.");
					return false;
				}

				// Backup
				if (!BackupFileHelper.BackupFile(currentLocalScript, luaScript.MinifiedLuaFileName))
					return false;
			}
			
			FileHelper.TryWriteFile(luaScript.LuaFilePath, vehicleXmlScript);
			FileHelper.TryWriteFile(luaScript.MinifiedLuaPath, vehicleXmlScript);

			Console.WriteLine($"Wrote script {luaScript.ObjectId} from vehicle {luaScript.VehicleName} to {luaScript.LuaFileName}.");

			return true;
		}
	}
}
