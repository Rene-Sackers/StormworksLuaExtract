using System.IO;
using System.Linq;

namespace StormworksLuaExtract.Models
{
	public class LuaScript
	{
		public string MicrocontrollerXmlPath { get; }

		public string LuaFilePath { get; }

		public string ObjectId { get; }

		public string Script { get; }

		public string MicrocontrollerName { get; }

		public LuaScript(string microcontrollerXmlPath, string luaFilePath, string objectId, string script)
		{
			MicrocontrollerXmlPath = microcontrollerXmlPath;
			LuaFilePath = luaFilePath;
			ObjectId = objectId;
			Script = script;

			MicrocontrollerName = MicrocontrollerXmlPath.Split(Path.DirectorySeparatorChar).Last().Replace(".xml", string.Empty);
		}
	}
}