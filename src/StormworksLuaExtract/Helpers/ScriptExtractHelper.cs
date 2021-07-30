using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

			foreach (Match microcontrollerMatch in Statics.MicrocontrollerRegex.Matches(xml))
			{
				var microcontrollerXml = microcontrollerMatch.Groups[0].Value;
				var microcontrollerName = WebUtility.HtmlDecode(microcontrollerMatch.Groups["name"].Value);
				
				foreach (Match scriptComponentMatch in Regex.Matches(microcontrollerXml, Statics.ObjectMatchPattern(), RegexOptions.Singleline))
				{
					var objectId = scriptComponentMatch.Groups["id"].Value;

					var luaFilePath = GetLuaFilePath(microcontrollerXmlFilePath, microcontrollerName, objectId);
					yield return new LuaScript(microcontrollerName, microcontrollerXmlFilePath, luaFilePath, objectId);
				}
			}
		}

		public static string GetScriptFromXmlFile(string xmlFile, string objectId)
		{
			string xml;
			try
			{
				xml = FileHelper.NoTouchReadFile(xmlFile);
			}
			catch
			{
				return null;
			}

			var match = Regex.Match(xml, Statics.ObjectMatchPattern(objectId));
			var script = match.Groups["script"].Value;

			return WebUtility.HtmlDecode(script);
		}

		private static string GetLuaFilePath(string vehicleXmlFilePath, string microcontrollerName, string scriptObjectId)
		{
			var vehicleName = Path.GetFileNameWithoutExtension(vehicleXmlFilePath);
			return Path.Combine(Statics.LocalEditDirectory, vehicleName, FileHelper.SanitizeFileName(microcontrollerName), scriptObjectId, "script.lua");
		}
	}
}
