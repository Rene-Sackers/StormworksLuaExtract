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

			var matches = Regex.Matches(xml, Statics.ObjectMatchPattern());
			foreach (Match match in matches)
			{
				var objectId = match.Groups["id"].Value;

				yield return new LuaScript(microcontrollerXmlFilePath, GetLuaFilePath(microcontrollerXmlFilePath, objectId), objectId);
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

		private static string GetLuaFilePath(string xmlFilePath, string scriptObjectId)
		{
			var fileName = xmlFilePath.Split(Path.DirectorySeparatorChar).Last();
			return Path.Combine(Statics.LocalEditDirectory, fileName.Replace(".xml", $"_{scriptObjectId}.lua"));
		}
	}
}
