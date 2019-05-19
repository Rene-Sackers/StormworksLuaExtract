using System;
using System.IO;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Helpers
{
	public static class BackupFileHelper
	{
		public static bool BackupFile(string originalContent, string originalFileName)
		{
			var backupFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)} {DateTime.Now:yyyy-MM-dd HH-mm-ss}{Path.GetExtension(originalFileName)}";
			var backupFilePath = Path.Join(Statics.LocalBackupDirectory, backupFileName);
			var success = FileHelper.TryWriteFile(backupFilePath, originalContent);

			if (success)
				Console.WriteLine($"Wrote backup to {backupFilePath}");

			return success;
		}
	}
}
