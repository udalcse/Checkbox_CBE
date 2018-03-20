//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling.Configuration;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Handle an exception by replacing it with another exception.
	/// </summary>
	public class ReplaceHandler : ExceptionHandler
	{
		/// <summary>
		/// Configuration for the exception handler.
		/// </summary>
		private ReplaceHandlerData _replaceHandlerData;

	    /// <summary>
		/// Initialize the replace exception handler with its configuration.
		/// </summary>
		/// <param name="config">Configuration for the replace exception handler.</param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(ReplaceHandlerData));

			_replaceHandlerData = (ReplaceHandlerData)config;
			
		}

		/// <summary>
		/// Return an instance of a new exception as determined by the configuration.
		/// </summary>
		/// <param name="exception">Exception to handle.</param>
		/// <param name="policyName">Name of the exception policy.</param>
		/// <param name="handlingInstanceId">Exception handling instance Id.</param>
		/// <returns></returns>
		public override Exception HandleException(Exception exception, string policyName, Guid handlingInstanceId)
		{
			return ReplaceException(exception, ReplaceExceptionType, ExceptionUtility.FormatExceptionMessage(ExceptionMessage, handlingInstanceId), handlingInstanceId);
		}

		/// <summary>
		/// Get the Type of the replacment exception to generate.
		/// </summary>
		public Type ReplaceExceptionType
		{
			get{ return Type.GetType(_replaceHandlerData.ReplaceExceptionTypeName);}
		}

		/// <summary>
		/// Get the message to set in the generated replacement exception.
		/// </summary>
		public string ExceptionMessage
		{
			get{return _replaceHandlerData.ExceptionMessage;}
		}

		/// <summary>
		/// Replace the specified exception with one of the specified type.
		/// </summary>
		/// <param name="originalException">Exception to replace.</param>
		/// <param name="replaceExceptionType">Type of exception to create.</param>
		/// <param name="replaceExceptionMessage">Exception message for the created exception.</param>
		/// <param name="handlingInstanceId">Exception handling instance id.</param>
		/// <returns>Created exception to replace the original exception.</returns>
		private static Exception ReplaceException(Exception originalException, Type replaceExceptionType, string replaceExceptionMessage, Guid handlingInstanceId)
		{
			if(replaceExceptionMessage == null)
			{
				replaceExceptionMessage = string.Empty;
			}

			try
			{
				var extraParameters = new object[] {replaceExceptionMessage};

				object exception = Activator.CreateInstance(replaceExceptionType, extraParameters);

				if(exception is BaseException)
					((BaseException)exception).ExceptionId = handlingInstanceId;

				return (Exception)exception;
			}
			catch(Exception ex)
			{
				throw new ExceptionHandlingException("Unable to replace exception: " + replaceExceptionType.Name, ex);
			}
		}
	}
}
