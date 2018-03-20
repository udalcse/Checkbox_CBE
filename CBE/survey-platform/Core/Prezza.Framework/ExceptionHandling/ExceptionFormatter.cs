//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Security;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Security.Principal;
using System.Collections.Specialized;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Base class for exception formatters.
	/// </summary>
	public abstract class ExceptionFormatter
	{
		/// <summary>
		/// Properties to ignore when formatting error messages.
		/// </summary>
		private static readonly ArrayList ignoredProperties = new ArrayList(new string[]{"Source", "Message", "HelpLink", "InnerException", "StackTrace"});

		/// <summary>
		/// Exception to format.
		/// </summary>
		private Exception exception;

	    /// <summary>
	    /// Additional information to write as part of handling the exception.
	    /// </summary>
	    private NameValueCollection additionalInfo = null;

	    /// <summary>
	    /// Constructor.
	    /// </summary>
	    /// <param name="exception">Exception to format.</param>
        protected ExceptionFormatter(Exception exception)
	    {
	        this.exception = exception;
	    }

	    /// <summary>
	    /// Get the exception to format.
	    /// </summary>
	    public Exception Exception
	    {
	        get{return exception;}
	    }

	    /// <summary>
	    /// Get the additional properties to format the exception.
	    /// </summary>
	    public NameValueCollection AdditionalInfo
	    {
	        get
	        {
	            if(additionalInfo == null)
	            {
	                additionalInfo = new NameValueCollection();
	                additionalInfo.Add("MachineName", GetMachineName());
	                additionalInfo.Add("TimeStamp", DateTime.UtcNow.ToString(CultureInfo.CurrentCulture));
	                additionalInfo.Add("FullName", GetExecutingAssembly());
	                additionalInfo.Add("AppDomainName", AppDomain.CurrentDomain.FriendlyName);
	                additionalInfo.Add("ThreadIdentity", GetThreadIdentity());
	                additionalInfo.Add("WindowsIdentity", GetWindowsIdentity());
	            }

	            return additionalInfo;
	        }
	    }

	    /// <summary>
	    /// Format the exception message.
	    /// </summary>
	    public virtual void Format()
	    {
	        WriteDescription();
	        WriteDateTime(DateTime.UtcNow);
	        WriteException(exception, null);
	    }

	    /// <summary>
	    /// Write the exception message.
	    /// </summary>
	    /// <param name="e">Exception to write.</param>
	    /// <param name="outerException">Outer exception.</param>
	    protected virtual void WriteException(Exception e, Exception outerException)
	    {
	        WriteExceptionType(e.GetType());
	        WriteMessage(e.Message);
	        WriteSource(e.Source);
	        WriteHelpLink(e.HelpLink);
	        WriteReflectionInfo(e);
	        WriteStackTrace(e.StackTrace);

	        if(outerException == null)
	        {
	            this.WriteAdditionalInfo(AdditionalInfo);
	        }

	        Exception inner = e.InnerException;

	        if(inner != null)
	        {
	            WriteException(inner, e);
	        }
	    }

	    /// <summary>
	    /// Write information gleaned from reflection about the exception.
	    /// </summary>
	    /// <param name="e">Exception to write information about.</param>
	    protected void WriteReflectionInfo(Exception e)
	    {
	        Type type = e.GetType();
	        PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
	        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
	        object value;

	        foreach(PropertyInfo property in properties)
	        {
	            if(property.CanRead && ignoredProperties.IndexOf(property) == -1)
	            {
	                try
	                {
	                    value = property.GetValue(e, null);
	                    WritePropertyInfo(property, value);
	                }
	                catch
	                {
	                }
	            }
	        }

	        foreach(FieldInfo field in fields)
	        {
	            try
	            {
	                value = field.GetValue(e);
	                WriteFieldInfo(field, value);
	            }
	            catch
	            {
	            }
	        }
	    }

	    /// <summary>
	    /// Write the exception description.
	    /// </summary>
	    protected abstract void WriteDescription();

	    /// <summary>
	    /// Write the time of the exception.
	    /// </summary>
	    /// <param name="utcNow">Time exception occurred.</param>
	    protected abstract void WriteDateTime(DateTime utcNow);

	    /// <summary>
	    /// Write the type of the exception.
	    /// </summary>
	    /// <param name="exceptionType"></param>
	    protected abstract void WriteExceptionType(Type exceptionType);

	    /// <summary>
	    /// Write the exception message.
	    /// </summary>
	    /// <param name="message"></param>
	    protected abstract void WriteMessage(string message);

	    /// <summary>
	    /// Write the exception source.
	    /// </summary>
	    /// <param name="source"></param>
	    protected abstract void WriteSource(string source);

	    /// <summary>
	    /// Write the exception help link.
	    /// </summary>
	    /// <param name="helpLink"></param>
	    protected abstract void WriteHelpLink(string helpLink); 

	    /// <summary>
	    /// Write the stack trace.
	    /// </summary>
	    /// <param name="stackTrace"></param>
	    protected abstract void WriteStackTrace(string stackTrace);

	    /// <summary>
	    /// Write out a property name and value.
	    /// </summary>
	    /// <param name="propertyInfo"></param>
	    /// <param name="propertyValue"></param>
	    protected abstract void WritePropertyInfo(PropertyInfo propertyInfo, object propertyValue);

	    /// <summary>
	    /// Write out a field name and value.
	    /// </summary>
	    /// <param name="field"></param>
	    /// <param name="fieldValue"></param>
	    protected abstract void WriteFieldInfo(FieldInfo field, object fieldValue);

	    /// <summary>
	    /// Write a collection of properties and values.
	    /// </summary>
	    /// <param name="additionalInfo"></param>
	    protected abstract void WriteAdditionalInfo(NameValueCollection additionalInfo);

	    /// <summary>
	    /// Get the machine name for the exception.
	    /// </summary>
	    /// <returns>Name of the machine.</returns>
	    private string GetMachineName()
	    {
	        string machineName = string.Empty;

	        try
	        {
	            machineName = Environment.MachineName;
	        }
	        catch(InvalidOperationException)
	        {
	            machineName = "PermissionDenied";
	        }
	        catch
	        {
	            machineName = "UnknownReadError";
	        }

	        return machineName;
	    }

	    /// <summary>
	    /// Get the thread identity.
	    /// </summary>
	    /// <returns>Name of the thread.</returns>
	    private string GetThreadIdentity()
	    {
	        string threadIdentity = string.Empty;

	        try
	        {
	            threadIdentity = Thread.CurrentPrincipal.Identity.Name;
	        }
	        catch(SecurityException)
	        {
	            threadIdentity = "PermissionDenied";
	        }
	        catch
	        {
	            threadIdentity = "UnknownReadError";
	        }

	        return threadIdentity;
	    }

	    /// <summary>
	    /// Get windows identity.
	    /// </summary>
	    /// <returns>ASPNET User name</returns>
	    private string GetWindowsIdentity()
	    {
	        string windowsIdentity = string.Empty;

	        try
	        {
	            windowsIdentity = WindowsIdentity.GetCurrent().Name;
	        }
	        catch(SecurityException)
	        {
	            windowsIdentity = "PermissionDenied";
	        }
	        catch
	        {
	            windowsIdentity = "UnknownReadError";
	        }

	        return windowsIdentity;
	    }

	    /// <summary>
	    /// Get the name of the executing assembly.
	    /// </summary>
	    /// <returns>Name of the executing assembly.</returns>
	    private string GetExecutingAssembly()
	    {
	        string executingAssembly = string.Empty;

	        try
	        {
	            executingAssembly = Assembly.GetExecutingAssembly().FullName;
	        }
	        catch(SecurityException)
	        {
	            executingAssembly = "PermissionDenied";
	        }
	        catch
	        {
	            executingAssembly = "UnknownReadError";
	        }

	        return executingAssembly;
	    }		
	}
}
