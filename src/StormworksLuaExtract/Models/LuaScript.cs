using System.IO;
using System.Linq;

namespace StormworksLuaExtract.Models
{
	public class LuaScript
	{
		public string VehicleXmlPath { get; }

		public string LuaFilePath { get; }

		public string ObjectId { get; }

		public string VehicleName { get; }

		public LuaScript(string vehicleXmlPath, string luaFilePath, string objectId)
		{
			VehicleXmlPath = vehicleXmlPath;
			LuaFilePath = luaFilePath;
			ObjectId = objectId;

			VehicleName = VehicleXmlPath.Split(Path.DirectorySeparatorChar).Last().Replace(".xml", string.Empty);
		}
	}
}