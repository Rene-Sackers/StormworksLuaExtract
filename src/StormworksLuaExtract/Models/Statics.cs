using System;
using System.IO;
using System.Text.RegularExpressions;

namespace StormworksLuaExtract.Models
{
	public static class Statics
	{
		public static readonly string MicrocontrollerPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Stormworks\data\vehicles");
		public static readonly string LocalEditDirectory = Path.GetFullPath(@".\Workspace");
		public static readonly string LocalBackupDirectory = Path.GetFullPath(@".\Backup");
		public static readonly Regex MicrocontrollerRegex = new Regex(@"<microprocessor_definition.*?name=(['\""])(?<name>.*?)\1[^>]*>.*?<\/microprocessor_definition>", RegexOptions.Compiled | RegexOptions.Singleline);

		public static string ObjectMatchPattern(string objectId = null)
		{
			var idMatch = objectId ?? "\\d+";
			return "<(?<element>(?:object)|(?:c\\d)) id=\"(?<id>" + idMatch + ")\" script=[\"|'](?<script>[^>]*)['|\"]>";
		}
	}
}
