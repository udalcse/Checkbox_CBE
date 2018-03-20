//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Utility routines for exception handling.
	/// </summary>
	public sealed class ExceptionUtility
	{
		/// <summary>
		/// Source string for event log entries (NOT CURRENTLY USED)
		/// </summary>
		private const string EventLogSource = "Prezza Framework Exception Handler";

		/// <summary>
		/// Token to replace with the handling instance Id.
		/// </summary>
		private const string HandlingInstanceToken = "{handlingInstanceId}";
		
		/// <summary>
		/// Constructor.
		/// </summary>
		private ExceptionUtility()
		{
		}

		/// <summary>
		/// Add the handling instance id to the message.
		/// </summary>
		/// <param name="message">Message to format.</param>
		/// <param name="handlingInstanceId">Handling instance Id for the exception message.</param>
		/// <returns>Formatted message.</returns>
		public static string FormatExceptionMessage(string message, Guid handlingInstanceId)
		{
			return message.Replace(HandlingInstanceToken, handlingInstanceId.ToString());
		}

		/// <summary>
		/// Log an error that occurs when handling an exception.
		/// </summary>
		/// <param name="policyName">Name of the exception policy in use when the error occurred.</param>
		/// <param name="offendingException">Exception that occurred during handling.</param>
		/// <param name="chainException">Chain exception that was being processed when the exception occurred.</param>
		/// <param name="originalException">Exception that was being handled when the exception occurred.</param>
		internal static void LogHandlingException(string policyName, Exception offendingException, Exception chainException, Exception originalException)
		{
			StringBuilder message = new StringBuilder();
			StringWriter writer = null;

			try
			{
				writer = new StringWriter(message, CultureInfo.CurrentCulture);

				if(policyName.Length > 0)
				{
					writer.WriteLine("Exception handling policy: " + policyName);
				}

				FormatHandlingException(writer, "OffendingException", offendingException);
				FormatHandlingException(writer, "ChainException", chainException);
				FormatHandlingException(writer, "OriginalException", originalException);
			}
			finally
			{
				if(writer != null)
				{
					writer.Close();
				}
			}

			//Try to write to the event log, but don't throw an exception
			try
			{
				EventLog.WriteEntry(EventLogSource, message.ToString(), EventLogEntryType.Error);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Format the exception handling exception method.
		/// </summary>
		/// <param name="writer">Writer to write the message to.</param>
		/// <param name="header">Header for message.</param>
		/// <param name="ex">Exception handling exception.</param>
		private static void FormatHandlingException(StringWriter writer, string header, Exception ex)
		{
			if(ex != null)
			{
				writer.WriteLine();
				writer.WriteLine(header);
				writer.Write(writer.NewLine);
				TextExceptionFormatter formatter = new TextExceptionFormatter(writer, ex, string.Empty);
				formatter.Format();
			}
		}
	}
}
