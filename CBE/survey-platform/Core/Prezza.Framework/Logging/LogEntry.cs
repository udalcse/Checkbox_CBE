//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Globalization;

namespace Prezza.Framework.Logging
{
	/// <summary>
	/// Container for log items to send/distribute.
	/// </summary>
	public class LogEntry : ICloneable
	{
	    /// <summary>
		/// Category of the entry.
		/// </summary>
		private string category = string.Empty;

	    /// <summary>
		/// Builder for generating error messages.
		/// </summary>
		private StringBuilder errorMessages;
        
		/// <summary>
		/// Constructor.  Initializes properties that can be determine automatically, such as thread name, process name, etc.
		/// </summary>
		public LogEntry()
		{
		    Win32ThreadId = string.Empty;
		    ManagedThreadName = string.Empty;
		    ProcessName = string.Empty;
		    ProcessId = string.Empty;
		    AppDomainName = string.Empty;
		    MachineName = string.Empty;
		    TimeStamp = DateTime.MaxValue;
		    Title = string.Empty;
		    Severity = Severity.Unspecified;
		    Priority = -1;
		    Message = string.Empty;
		    CollectIntrinsicProperties();
		}

	    /// <summary>
		/// Constructor.  Initializes properties that can be determine automatically, such as thread name, process name, etc.
		/// </summary>
		/// <param name="message">Message to send.</param>
		/// <param name="category">Category of the _message.</param>
		/// <param name="priority">Priority of the _message.</param>
		/// <param name="eventId">Event Id associated with the entry.</param>
		/// <param name="severity">Severity of the event.</param>
		/// <param name="title">Title of the entry.</param>
		/// <param name="properties">Additional properties to log.</param>
		public LogEntry(object message, string category, int priority, int eventId, Severity severity, string title, IDictionary properties)
		{
	        Win32ThreadId = string.Empty;
	        ManagedThreadName = string.Empty;
	        ProcessName = string.Empty;
	        ProcessId = string.Empty;
	        AppDomainName = string.Empty;
	        MachineName = string.Empty;
	        TimeStamp = DateTime.MaxValue;
	        Message = message.ToString();
			Category = category;
			Priority = priority;
			EventId = eventId;
			Severity = severity;
			Title = title;
			ExtendedProperties = properties;

			CollectIntrinsicProperties();
		}

        /// <summary>
        /// Get/set application context, if any, associated with the entry
        /// </summary>
        public string ApplicationContext { get; set; }

	    /// <summary>
	    /// Get/Set the log _message.
	    /// </summary>
	    public string Message { get; set; }

	    /// <summary>
		/// Get/Set the entry's category.
		/// </summary>
		public string Category
		{
            get { return category ?? string.Empty; }
	        set{category = value;}
		}

	    /// <summary>
	    /// Get/Set the entry's priority.
	    /// </summary>
	    public int Priority { get; set; }

	    /// <summary>
	    /// Get/Set the event Id associated with the entry.
	    /// </summary>
	    public int EventId { get; set; }

	    /// <summary>
	    /// Get/Set the severity of the event.
	    /// </summary>
	    public Severity Severity { get; set; }

	    /// <summary>
		/// Get the int that will be logged as the severity.
		/// </summary>
		public int LoggedSeverity
		{
			get{return (int)Severity;}
		}

	    /// <summary>
	    /// Get/Set the title of the entry.
	    /// </summary>
	    public string Title { get; set; }

	    /// <summary>
	    /// Get/Set the timestamp of the entry.
	    /// </summary>
	    public DateTime TimeStamp { get; set; }

	    /// <summary>
	    /// Get/Set the machine name.
	    /// </summary>
	    public string MachineName { get; set; }

	    /// <summary>
	    /// Get/Set the application domain name.
	    /// </summary>
	    public string AppDomainName { get; set; }

	    /// <summary>
	    /// Get/Set the id of the process.
	    /// </summary>
	    public string ProcessId { get; set; }

	    /// <summary>
	    /// Get/Set the name of the process.
	    /// </summary>
	    public string ProcessName { get; set; }

	    /// <summary>
	    /// Get/Set the name of the thread.
	    /// </summary>
	    public string ManagedThreadName { get; set; }

	    /// <summary>
	    /// Get/Set the id of the thread.
	    /// </summary>
	    public string Win32ThreadId { get; set; }

	    /// <summary>
	    /// Get/Set the extended properties associated with the log entry.
	    /// </summary>
	    public IDictionary ExtendedProperties { get; set; }

	    /// <summary>
		/// Get the timestamp as a string
		/// </summary>
		public string TimeStampString
		{
			get
			{
				return TimeStamp.ToString(CultureInfo.CurrentCulture);
			}
		}

		/// <summary>
		/// Clone the log entry.
		/// </summary>
		/// <returns>A LogEntry object that is a clone of the original.</returns>
		public object Clone()
		{
            LogEntry result = new LogEntry
            {
                Message = Message,
                Category = Category,
                EventId = EventId,
                Title = Title,
                Severity = Severity,
                Priority = Priority,
                TimeStamp = TimeStamp,
                MachineName = MachineName,
                AppDomainName = AppDomainName,
                ProcessId = ProcessId,
                ProcessName = ProcessName,
                ManagedThreadName = ManagedThreadName,
                ApplicationContext = ApplicationContext
            };

		    if(ExtendedProperties is ICloneable)
			{
				result.ExtendedProperties = (IDictionary)((ICloneable)ExtendedProperties).Clone();
			}

			return result;
		}

		/// <summary>
		/// Add an error _message to the error _message builder.
		/// </summary>
		/// <param name="message"></param>
		public virtual void AddErrorMessage(string message)
		{
			if(errorMessages == null)
			{
				errorMessages = new StringBuilder();
			}

			errorMessages.Insert(0, Environment.NewLine);
			errorMessages.Insert(0, Environment.NewLine);
			errorMessages.Insert(0, message);
		}

		/// <summary>
		/// Get the error messages builder object.
		/// </summary>
		public string ErrorMessages
		{
			get
			{
			    if(errorMessages == null)
				{
					return null;
				}
			    
                return errorMessages.ToString();
			}
		}

		/// <summary>
		/// Get the values of properties that are not set by calling methods.
		/// </summary>
		private void CollectIntrinsicProperties()
		{
			TimeStamp = DateTime.Now;

			try
			{
				MachineName = Environment.MachineName;
			}
			catch(Exception e)
			{
				MachineName = "Unable to get intrinsic property: " + e.Message;
			}

			try
			{
				AppDomainName = AppDomain.CurrentDomain.FriendlyName;
			}
			catch(Exception e)
			{
				MachineName = "Unable to get intrinsic property: " + e.Message;
			}
			
			try
			{
				MachineName = Environment.MachineName;
			}
			catch(Exception e)
			{
				MachineName = "Unable to get intrinsic property: " + e.Message;
			}
			

			//ProcessId
			//ProcessName
			try
			{
				ManagedThreadName = Thread.CurrentThread.Name;
			}
			catch(Exception e)
			{
				MachineName = "Unable to get intrinsic property: " + e.Message;
			}
			

			//win32ThreadId
		}
	}
}
