using System;
using System.IO;

namespace StormworksLuaExtract.Helpers
{
	public class BufferedFileSystemWatcher : IDisposable
	{
		private const int DefaultBUfferTime = 2000;

		private readonly int _bufferTimeInMilliseconds;
		private readonly FileSystemWatcher _watcher;

		private FileSystemEventArgs _lastFileChangePath;
		private DateTime _lastFileChangeTime = DateTime.Now;

		public event FileSystemEventHandler Changed;
		public event FileSystemEventHandler Created;
		public event FileSystemEventHandler Deleted;
		public event FileSystemEventHandler Renamed;

		public BufferedFileSystemWatcher(string path, string filter, int bufferTimeInMilliseconds = DefaultBUfferTime)
		{
			_watcher = new FileSystemWatcher(path, filter) {IncludeSubdirectories = true};
			_watcher.Changed += WatcherEvent;
			_watcher.Created += WatcherEvent;
			_watcher.Deleted += WatcherEvent;
			_watcher.Renamed += WatcherEvent;
			_bufferTimeInMilliseconds = bufferTimeInMilliseconds;
			_watcher.EnableRaisingEvents = true;
		}

		public BufferedFileSystemWatcher(string path, int bufferTimeInMilliseconds = DefaultBUfferTime)
		{
			_watcher = new FileSystemWatcher(path);
			_watcher.Changed += WatcherEvent;
			_watcher.Created += WatcherEvent;
			_watcher.Deleted += WatcherEvent;
			_watcher.Renamed += WatcherEvent;
			_bufferTimeInMilliseconds = bufferTimeInMilliseconds;
			_watcher.EnableRaisingEvents = true;
		}

		private void WatcherEvent(object sender, FileSystemEventArgs e)
		{
			if (_lastFileChangePath?.FullPath == e.FullPath && DateTime.Now.Subtract(_lastFileChangeTime).TotalMilliseconds <= _bufferTimeInMilliseconds)
				return;

			_lastFileChangePath = e;
			_lastFileChangeTime = DateTime.Now;

			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Changed:
					Changed?.Invoke(sender, e);
					break;
				case WatcherChangeTypes.Created:
					Created?.Invoke(sender, e);
					break;
				case WatcherChangeTypes.Deleted:
					Deleted?.Invoke(sender, e);
					break;
				case WatcherChangeTypes.Renamed:
					Renamed?.Invoke(sender, e);
					break;
				default:
					Changed?.Invoke(sender, e);
					Created?.Invoke(sender, e);
					Deleted?.Invoke(sender, e);
					Renamed?.Invoke(sender, e);
					break;
			}
		}

		public void Dispose()
		{
			_watcher?.Dispose();
		}
	}
}
