using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Helpers
{
	public static class ScriptExtractHelper
	{
		public static IEnumerable<LuaScript> ExtractScriptsFromMicrocontrollerXml(string microcontrollerXmlFilePath)
		{
			string xml;
			try
			{
				xml = FileHelper.NoTouchReadFile(microcontrollerXmlFilePath);
			}
			catch
			{
				yield break;
			}

			var matches = Regex.Matches(xml, Statics.ObjectMatchPattern());
			foreach (Match match in matches)
			{
				var element = match.Groups["element"].Value;

				// The script is printed twice in the XML, once under a <object item, and once under a <c33 item
				if (element != "object")
					continue;

				var objectId = match.Groups["id"].Value;
				var script = match.Groups["script"].Value;
				script = System.Net.WebUtility.HtmlDecode(script);

				yield return new LuaScript(microcontrollerXmlFilePath, GetLuaFilePath(microcontrollerXmlFilePath, objectId), objectId, script);
			}
		}

		private static string GetLuaFilePath(string xmlFilePath, string scriptObjectId)
		{
			var fileName = xmlFilePath.Split(Path.DirectorySeparatorChar).Last();
			return Path.Combine(Statics.LocalEditDirectory, fileName.Replace(".xml", $"_{scriptObjectId}.lua"));
		}
	}
}
