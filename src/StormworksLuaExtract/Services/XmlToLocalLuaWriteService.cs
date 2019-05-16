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
			
			if (File.Exists(luaScript.LuaFilePath) && FileHelper.NoTouchReadFile(luaScript.LuaFilePath) == luaScript.Script)
			{
				Console.WriteLine($"Nothing changed for script {luaScript.ObjectId} from microcontroller {luaScript.MicrocontrollerName}.");
				return;
			}
				
			FileHelper.TryWriteFile(luaScript.LuaFilePath, luaScript.Script);

			Console.WriteLine($"Wrote script {luaScript.ObjectId} from microcontroller {luaScript.MicrocontrollerName} to {luaScript.LuaFilePath}.");
		}
	}
}
