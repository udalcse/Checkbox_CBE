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
	/// Exception handler to wrap an exception in another exception.
	/// </summary>
	public class WrapHandler : ExceptionHandler
	{
		/// <summary>
		/// Configuration information for the wrap exception handler.
		/// </summary>
		private WrapHandlerData _wrapHandlerData;

	    /// <summary>
		/// Initialize the exception handler with its configuration.
		/// </summary>
		/// <param name="config">Wrap exception handler configuration.</param>
		public override void Initialize(ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(WrapHandlerData));

			_wrapHandlerData = (WrapHandlerData)config;
		}

		/// <summary>
		/// Handle an exception.
		/// </summary>
		/// <param name="exception">Exception to handle.</param>
		/// <param name="policyName">Exception handling policy.</param>
		/// <param name="handlingInstanceId">Exception handling instance id.</param>
		/// <returns>Excpetion with the exception to be handled as its inner exception.</returns>
		public override Exception HandleException(Exception exception, string policyName, Guid handlingInstanceId)
		{
			return WrapException(exception, WrapExceptionType, ExceptionUtility.FormatExceptionMessage(WrapExceptionMessage, handlingInstanceId), handlingInstanceId);
		}

		/// <summary>
		/// Get the Type of the exception to create to wrap the handled exception with.
		/// </summary>
		public Type WrapExceptionType
		{
			get{return Type.GetType(_wrapHandlerData.WrapExceptionTypeName);}
		}

		/// <summary>
		/// Get the message for the exception to create to wrap the handled exception with.
		/// </summary>
		public string WrapExceptionMessage
		{
			get{return _wrapHandlerData.ExceptionMessage;}
		}

		/// <summary>
		/// Create an exception with the exception to handle as its inner exception.
		/// </summary>
		/// <param name="originalException">Exception to handle.</param>
		/// <param name="wrapExceptionType">Type of exception to create for the wrapper exception.</param>
		/// <param name="wrapExceptionMessage">Exception message for the wrapper exception.</param>
		/// <param name="handlingInstanceId">Exception handling instance Id.</param>
		/// <returns>Wrapper exception with the original exception set as its inner exception.</returns>
		private static Exception WrapException(Exception originalException, Type wrapExceptionType, string wrapExceptionMessage, Guid handlingInstanceId)
		{
			if(wrapExceptionMessage == null)
			{
				wrapExceptionMessage = string.Empty;
			}

			try
			{
				var extraParameters = new object[2];
				extraParameters[0] = wrapExceptionMessage;
				extraParameters[1] = originalException;

				object exception = Activator.CreateInstance(wrapExceptionType, extraParameters);

				if(exception is BaseException)
					((BaseException)exception).ExceptionId = handlingInstanceId;

				return (Exception)exception;
			}
			catch(Exception ex)
			{
				throw new ExceptionHandlingException("Unable to wrap exception: " + wrapExceptionType.Name, ex);
			}
		}
	}
}
