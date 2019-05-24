using System.IO;

namespace StormworksLuaExtract.Models
{
	public class LuaScript
	{
		public string VehicleXmlPath { get; }

		public string VehicleXmlFileName => Path.GetFileName(VehicleXmlPath);

		public string VehicleName => Path.GetFileNameWithoutExtension(VehicleXmlPath);

		public string MicrocontrollerName { get; }

		public string LuaFilePath { get; }

		public string LuaFileName => Path.GetFileName(LuaFilePath);

		public string MinifiedLuaPath { get; }

		public string MinifiedLuaFileName => Path.GetFileName(MinifiedLuaPath);

		public string ObjectId { get; }

		public LuaScript(string microcontrollerName, string vehicleXmlPath, string luaFilePath, string objectId)
		{
			MicrocontrollerName = microcontrollerName;
			VehicleXmlPath = vehicleXmlPath;
			LuaFilePath = luaFilePath;
			ObjectId = objectId;
			MinifiedLuaPath = luaFilePath.Replace(".lua", ".min.lua");
		}
	}
}