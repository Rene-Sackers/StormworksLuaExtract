using System;

namespace StormworksLuaExtract.Helpers
{
	public static class ConsoleHelper
	{
		private static readonly ConsoleColor DefaultConsoleColor = Console.ForegroundColor;

		public static void WriteWarning(object value)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(value);
			Console.ForegroundColor = DefaultConsoleColor;
		}
	}
}
