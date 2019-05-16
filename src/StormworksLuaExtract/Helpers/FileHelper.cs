using System;
using System.IO;
using System.Text;

namespace StormworksLuaExtract.Helpers
{
	public static class FileHelper
	{
		public static string NoTouchReadFile(string path)
		{
			try
			{
				using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
					return streamReader.ReadToEnd();
			}
			catch
			{
				Console.WriteLine($"Failed to read file '{path}'");
			}

			return null;
		}

		public static bool TryWriteFile(string path, string content)
		{
			try
			{
				File.WriteAllText(path, content);
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to write file '{path}' - {e.Message}");
				return false;
			}
		}
	}
}
