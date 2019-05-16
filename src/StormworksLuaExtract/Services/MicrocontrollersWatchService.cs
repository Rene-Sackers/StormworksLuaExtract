using System;
using System.IO;
using System.Threading.Tasks;
using StormworksLuaExtract.Helpers;
using StormworksLuaExtract.Models;

namespace StormworksLuaExtract.Services
{
	public class MicrocontrollersWatchService
	{
		public delegate void MicrocontrollerChangedHandler(string xmlFilePath);

		public event MicrocontrollerChangedHandler MicrocontrollerAdded;
		public event MicrocontrollerChangedHandler MicrocontrollerDeleted;

		public void StartWatching()
		{
			var processorsXmlWatcher = new BufferedFileSystemWatcher(Statics.MicrocontrollerPath, "*.xml");
			processorsXmlWatcher.Created += MicrocontrollerXmlFileAdded;
			processorsXmlWatcher.Deleted += MicrocontrollerXmlFileDeleted;
		}

		private async void MicrocontrollerXmlFileAdded(object sender, FileSystemEventArgs e)
		{
			await Task.Delay(Constants.ReadWriteTimeoutInMilliseconds);

			Console.WriteLine($"Microcontroller XML file created: {e.Name}");

			MicrocontrollerAdded?.Invoke(e.FullPath);
		}

		private void MicrocontrollerXmlFileDeleted(object sender, FileSystemEventArgs e)
		{
			Console.WriteLine($"Microcontroller XML file deleted: {e.Name}");

			MicrocontrollerDeleted?.Invoke(e.FullPath);
		}
	}
}
