//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Summary description for ConfigurationChangeFileWatcher.
	/// </summary>
	internal class ConfigurationChangeFileWatcher : IConfigurationChangeWatcher
	{
		//private static readonly string eventSourceName = SR.FileWatcherEventSource;
		private string configFilePath;
		private int pollDelayInMilliseconds = defaultPollDelayInMilliseconds;
		private static int defaultPollDelayInMilliseconds = 15000;
		private static readonly object configurationChangedKey = new object();
		private Thread pollingThread;
		private EventHandlerList eventHandlers = new EventHandlerList();
		private DateTime lastWriteTime;

		/// <summary>
		/// Watches a file for any changes.  ConfigurationChanged event is fired when file changes. 
		/// </summary>
		/// <param name="configFilePath">the path of the file the watch</param>
		public ConfigurationChangeFileWatcher(string configFilePath)
		{
			this.configFilePath = configFilePath;
		}

		~ConfigurationChangeFileWatcher()
		{
			Disposing(false);
		}

		#region IConfigurationChangeWatcher Members

		public event ConfigurationChangedEventHandler ConfigurationChanged
		{
			add { eventHandlers.AddHandler(configurationChangedKey, value); }
			remove { eventHandlers.RemoveHandler(configurationChangedKey, value); }
		}

		public void StartWatching()
		{
			if(pollingThread == null)
			{
				pollingThread = new Thread(new ThreadStart(Poller));
				pollingThread.IsBackground = true;
				pollingThread.Start();
			}
		}

		public void StopWatching()
		{
			if(pollingThread != null)
			{
				pollingThread.Interrupt();
				pollingThread = null;
			}
		}

		#endregion

		public void Dispose()
		{
			Disposing(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Disposing(bool isDisposing)
		{
			if (isDisposing)
			{
				eventHandlers.Dispose();
				StopWatching();
			}
		}

		/// <summary>
		/// Method run by background thread.  Monitors the file for changes.
		/// </summary>
		private void Poller()
		{
			lastWriteTime = DateTime.MinValue;
			while (true)
			{
				if (File.Exists(configFilePath) == true)
				{
					if (lastWriteTime.Equals(DateTime.MinValue))
					{
						lastWriteTime = File.GetLastWriteTime(configFilePath);
					}

					DateTime currentLastWriteTime = File.GetLastWriteTime(configFilePath);
					if (lastWriteTime.Equals(currentLastWriteTime) == false)
					{
						lastWriteTime = currentLastWriteTime;
						OnConfigurationChanged();
					}
				}

				try
				{
					Thread.Sleep(pollDelayInMilliseconds);
				}
				catch (ThreadInterruptedException)
				{
					return;
				}
			}

		}

		protected virtual void OnConfigurationChanged()
		{
			ConfigurationChangedEventHandler callbacks = (ConfigurationChangedEventHandler)eventHandlers[configurationChangedKey];
			ConfigurationChangedEventArgs eventData = new ConfigurationChangedEventArgs(Path.GetFullPath(configFilePath));

			try
			{
				if (callbacks != null)
				{
					callbacks(this, eventData);
				}
			}
			catch (Exception)
			{
				//EventLog.WriteEntry(eventSourceName, SR.ExceptionEventRaisingFailed + ":" + e.Message);
			}
		}
	}
}
