//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;

using Prezza.Framework.Common;
using Prezza.Framework.Logging;
using Prezza.Framework.ExceptionHandling.Configuration;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Special exception handler that logs exception information to a log sink.
	/// </summary>
	public class LoggingExceptionHandler : ExceptionHandler
	{
		/// <summary>
		/// Default log category for the generated log entry.
		/// </summary>
		private string _defaultLogCategory;

		/// <summary>
		/// Default event id for the generated log entry.
		/// </summary>
		private int _defaultEventId;

		/// <summary>
		/// Default severity for the generated log entry.
		/// </summary>
		private Severity _defaultSeverity;

		/// <summary>
		/// Default title for the generated log entry.
		/// </summary>
		private string _defaultTitle;

		/// <summary>
		/// TypeName of the formatter to use for the generated entry.
		/// </summary>
		private string _formatterTypeName;

		/// <summary>
		/// Minimum priority to set for the generated entry.
		/// </summary>
		private int _minimumPriority;
		
		/// <summary>
		///  ConstructorInfo used to instantiate the formatter.
		/// </summary>
		private ConstructorInfo _constructor;
		
		/// <summary>
		/// Configuration information for the handler.
		/// </summary>
		private LoggingExceptionHandlerData _exceptionHandlerData;


	    /// <summary>
		/// Initialize the exception handler with its configuration information.
		/// </summary>
		/// <param name="config">Configuration information for the logging exception handler.</param>
		public override void Initialize(Framework.Configuration.ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(LoggingExceptionHandlerData));

			_exceptionHandlerData = (LoggingExceptionHandlerData)config;
		}

		/// <summary>
		/// Handle the specified exception.
		/// </summary>
		/// <param name="exception">Exception to handle.</param>
		/// <param name="policyName">Exception policy used.</param>
		/// <param name="handlingInstanceId">Exception instance Id.</param>
		/// <returns></returns>
		public override Exception HandleException(Exception exception, string policyName, Guid handlingInstanceId)
		{
			SetupEnvironment();
			WriteToLog(CreateMessage(exception, handlingInstanceId));
			return exception;
		}

		/// <summary>
		/// Write a message to the log.
		/// </summary>
		/// <param name="logMessage">Message to write.</param>
		protected virtual void WriteToLog(string logMessage)
		{
			LogEntry entry = new LogEntry(logMessage, _defaultLogCategory, _minimumPriority, _defaultEventId, _defaultSeverity, _defaultTitle, null);
			Logger.Write(entry);
		}

		/// <summary>
		/// Create a string writer to use to format the exception message.
		/// </summary>
		/// <returns></returns>
		protected virtual StringWriter CreateStringWriter()
		{
			return new StringWriter(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Create the message formatter.
		/// </summary>
		/// <param name="writer">StringWriter for the formatter to use.</param>
		/// <param name="exception">Exceptin to format the message for.</param>
		/// <param name="handlingInstanceIdString">Exception instance Id in string format.</param>
		/// <returns></returns>
		protected virtual ExceptionFormatter CreateFormatter(StringWriter writer, Exception exception, string handlingInstanceIdString)
		{
			return (ExceptionFormatter)_constructor.Invoke(new object[]{writer, exception, handlingInstanceIdString});
		}

		/// <summary>
		/// Setup the the configuration and items for the exception handler.
		/// </summary>
		private void SetupEnvironment()
		{
			InitProperties(_exceptionHandlerData);

			Type formatterType = Type.GetType(_formatterTypeName, true);
			Type[] types = new[] {typeof(TextWriter), typeof(Exception), typeof(string)};
			_constructor = formatterType.GetConstructor(types);
			
			if(_constructor == null)
			{
				throw new ExceptionHandlingException("Unable to get _constructor for: " + formatterType.AssemblyQualifiedName);
			}
		}

		/// <summary>
		/// Initialize the defaults for the exception handler based on the passed in configuration information.
		/// </summary>
		/// <param name="data">Logging exception handler configuration information.</param>
		private void InitProperties(LoggingExceptionHandlerData data)
		{
			_defaultLogCategory = data.DefaultLogCategory;
			_defaultEventId = data.DefaultEventId;
			_defaultSeverity = data.DefaultSeverity;
			_defaultTitle = data.DefaultTitle;
			_formatterTypeName = data.FormatterTypeName;
			_minimumPriority = data.MinimumPriority;
		}

		/// <summary>
		/// Create the message to log for the specified exception.
		/// </summary>
		/// <param name="exception">Exception to generate the message for.</param>
		/// <param name="handlingInstanceId">Instace Id of the exception.</param>
		/// <returns></returns>
		private string CreateMessage(Exception exception, Guid handlingInstanceId)
		{
			StringWriter writer = null;
			StringBuilder stringBuilder = null;

			try
			{
				writer = CreateStringWriter();
				ExceptionFormatter formatter = CreateFormatter(writer, exception, handlingInstanceId.ToString());
				formatter.Format();
				stringBuilder = writer.GetStringBuilder();
			}
			finally
			{
				if(writer != null)
				{
					try
					{
						writer.Close();
					}
					catch
					{
					}
				}
			}

			return stringBuilder.ToString();
		}
	}
}
