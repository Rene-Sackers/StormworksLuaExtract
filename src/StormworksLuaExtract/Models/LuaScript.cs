using System.IO;
using System.Linq;

namespace StormworksLuaExtract.Models
{
	public class LuaScript
	{
		public string VehicleXmlPath { get; }

		public string VehicleName => Path.GetFileNameWithoutExtension(VehicleXmlPath);

		public string LuaFilePath { get; }

		public string LuaFileName => Path.GetFileName(LuaFilePath);

		public string ObjectId { get; }

		public LuaScript(string vehicleXmlPath, string luaFilePath, string objectId)
		{
			VehicleXmlPath = vehicleXmlPath;
			LuaFilePath = luaFilePath;
			ObjectId = objectId;
		}
	}
}