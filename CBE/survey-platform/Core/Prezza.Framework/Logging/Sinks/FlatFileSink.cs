//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Sinks
{
	/// <summary>
	/// Log sink for writing log messages to a flat file.
	/// </summary>
	public class FlatFileSink : LogSink
	{
		/// <summary>
		/// Sync object to control writing to the log file.
		/// </summary>
		private static object syncObject = new object();

		/// <summary>
		/// Configuration information for the sink.
		/// </summary>
		private FlatFileSinkData config;

		/// <summary>
		/// Constructor.
		/// </summary>
		public FlatFileSink()
		{
		}

		/// <summary>
		/// Initialize the file sink with it's configuration.
		/// </summary>
		/// <param name="config"></param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(FlatFileSinkData));

			this.config = (FlatFileSinkData)config;
		}

		/// <summary>
		/// Core send message routine which validates log entry parameters and writes the message to the file.
		/// </summary>
		/// <param name="logEntry">Log entry to write.</param>
		protected override void SendMessageCore(LogEntry logEntry)
		{
			if(ValidateParameters(logEntry))
			{
				try
				{
					WriteMessageToFile(logEntry);
				}
				catch(Exception ex)
				{
					logEntry.AddErrorMessage("Log sink failure: " + ex.ToString());
					throw;
				}
			}
		}

		/// <summary>
		/// Validate log entry by setting some default values and ensuring that sink configuration
		/// information exists.
		/// </summary>
		/// <param name="logEntry">Log entry to write.</param>
		/// <returns>True if the parameters were sucessfully validated.</returns>
		private bool ValidateParameters(LogEntry logEntry)
		{
			FlatFileSinkData data = GetFlatFileSinkDataFromCursor();

			bool valid = true;

			if(data.Header == null)
			{
				data.Header = "";
			}

			if(data.Footer == null)
			{
				data.Footer = "";
			}

			if(data.Name == null || data.Name.Length == 0)
			{
				valid = false;
				logEntry.AddErrorMessage("File sink was missing configuration information.");
			}

			return valid;
		}

		/// <summary>
		/// Get the configuration for the log sink.
		/// </summary>
		/// <returns></returns>
		private FlatFileSinkData GetFlatFileSinkDataFromCursor()
		{
			return config;
		}

		/// <summary>
		/// Write the log message to the file.  If the file or directory containing the file
		/// does not exist, this method will create it/them.
		/// </summary>
		/// <param name="logEntry">Log entry to write.</param>
		private void WriteMessageToFile(LogEntry logEntry)
		{
			FlatFileSinkData data = GetFlatFileSinkDataFromCursor();

			string directory = Path.GetDirectoryName(data.FileName);

			if(directory.Length == 0)
			{
				directory = AppDomain.CurrentDomain.BaseDirectory;
			}

			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using(FileStream fileStream = new FileStream(data.FileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(fileStream))
				{
					lock(syncObject)
					{
						if(data.Header.Length > 0)
						{
							writer.WriteLine(data.Header);
						}

						writer.WriteLine(FormatEntry(logEntry));

						if(data.Footer.Length > 0)
						{
							writer.WriteLine(data.Footer);
						}

						writer.Flush();
					}
				}
			}
		}
	}
}
