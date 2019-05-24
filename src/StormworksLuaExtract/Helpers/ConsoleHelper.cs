using System;

namespace StormworksLuaExtract.Helpers
{
	public static class ConsoleHelper
	{
		public static void WriteWarning(object value)
		{
			var previousForegroundColor = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(value);
			Console.ForegroundColor = previousForegroundColor;
		}
	}
}
