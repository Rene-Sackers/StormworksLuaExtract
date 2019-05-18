using System;
using System.IO;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class XmlToLocalLuaWriteService
	{
		public void WriteMicrocontrollerLuaScriptsToFiles(LuaScript luaScript)
		{
			Console.WriteLine($"Extracting Lua scripts from microcontroller '{luaScript.MicrocontrollerXmlPath}'");
			
			if (File.Exists(luaScript.LuaFilePath))
			{
				var currentScript = FileHelper.NoTouchReadFile(luaScript.LuaFilePath);

				if (currentScript == luaScript.Script)
				{
					Console.WriteLine($"Nothing changed for script {luaScript.ObjectId} from microcontroller {luaScript.MicrocontrollerName}.");
					return;
				}

				// Backup
				var backupFilePath = Path.Join(Statics.LocalBackupDirectory, $"{luaScript.MicrocontrollerName}_{luaScript.ObjectId} {DateTime.Now:yyyy-MM-dd HH-mm-ss}.lua");
				if (!FileHelper.TryWriteFile(backupFilePath, currentScript))
					return;

				Console.WriteLine($"Wrote backup to {backupFilePath}");
			}


			FileHelper.TryWriteFile(luaScript.LuaFilePath, luaScript.Script);

			Console.WriteLine($"Wrote script {luaScript.ObjectId} from microcontroller {luaScript.MicrocontrollerName} to {luaScript.LuaFilePath}.");
		}
	}
}
