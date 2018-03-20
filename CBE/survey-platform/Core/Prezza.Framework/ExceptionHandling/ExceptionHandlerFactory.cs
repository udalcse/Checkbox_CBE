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
	/// Factory to create instances of <see cref="ExceptionHandler"/> objects.
	/// </summary>
	public class ExceptionHandlerFactory : ProviderFactory
	{
		/// <summary>
		/// Policy the exception handler is part of.
		/// </summary>
		private string policyName;

		/// <summary>
		/// TypeName of the exception the handler is configured to handle.
		/// </summary>
		private string exceptionTypeName;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionHandlerFactory() : this((ConfigurationBase)ConfigurationManager.GetConfiguration("exceptionHandlerConfiguration"))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="config">Exception Handler configuration information.</param>
		public ExceptionHandlerFactory(ConfigurationBase config) : base("exceptionHandlerFactory", typeof(IExceptionHandler), config)
		{
			this.policyName = string.Empty;
			this.exceptionTypeName = string.Empty;
		}

		/// <summary>
		/// Create all exception handler instances for the specified policy and exception TypeName.
		/// </summary>
		/// <param name="policyName">Policy name for the handlers.</param>
		/// <param name="exceptionTypeName">Exception type the handlers are configured to handle.</param>
		/// <returns>Array of exception handler objects.</returns>
		public IExceptionHandler[] CreateExceptionHandlers(string policyName, string exceptionTypeName)
		{
			ValidatePolicyAndExceptionType(policyName, exceptionTypeName);

			this.policyName = policyName;
			this.exceptionTypeName = exceptionTypeName;

			ExceptionHandlingConfiguration exceptionHandlingConfig = (ExceptionHandlingConfiguration)base.Config;
			ExceptionHandlerDataCollection handlers = exceptionHandlingConfig.GetExceptionHandlerDataCollection(policyName, exceptionTypeName);

			IExceptionHandler[] exceptionHandlers = new IExceptionHandler[handlers.Count];

			int index = 0;

			foreach(ExceptionHandlerData handler in handlers)
			{
				exceptionHandlers[index++] = CreateExceptionHandler(policyName, exceptionTypeName, handler.Name);
			}

			return exceptionHandlers;
		}

		/// <summary>
		/// Create an instance of a specific exception handler.
		/// </summary>
		/// <param name="policyName">Policy the handler is defined for.</param>
		/// <param name="exceptionTypeName">TypeName of the exception the handler is configured to handle.</param>
		/// <param name="handlerName">Name of the exception handler to instantiate.</param>
		/// <returns>Instance of the specified exception handler.</returns>
		public IExceptionHandler CreateExceptionHandler(string policyName, string exceptionTypeName, string handlerName)
		{
			ValidatePolicyAndExceptionType(policyName, exceptionTypeName);

			this.policyName = policyName;
			this.exceptionTypeName = exceptionTypeName;

			return (IExceptionHandler)base.CreateInstance(handlerName);
		}

		/// <summary>
		/// Log an exception handling exception.
		/// </summary>
		/// <param name="configurationName">Name of the configuration object where the exception occurred.</param>
		/// <param name="e">Exception to handle.</param>
		protected override void PublishFailureEvent(string configurationName, Exception e)
		{
			Exception wrappedException = new ExceptionHandlingException("Unable to load exception handlers.  Configuration: " + configurationName + "  Policy: " + policyName, e);
			ExceptionUtility.LogHandlingException(policyName, wrappedException, null, e);
			throw new ExceptionHandlingException("Unable to load exception handlers.  Configuration: " + configurationName + "  Policy: " + policyName);
		}

		/// <summary>
		/// Get the configuration object for the specified exception handler.
		/// </summary>
		/// <param name="providerName">Name of the exception handler to get the configuration of.</param>
		/// <returns><see cref="ExceptionHandlingConfiguration"/> object for the specified handler.</returns>
		protected override ConfigurationBase GetConfigurationObject(string providerName)
		{
			ExceptionHandlingConfiguration exceptionHandlingConfig = (ExceptionHandlingConfiguration)base.Config;
			return exceptionHandlingConfig.GetExceptionHandlerData(policyName, exceptionTypeName, providerName);
		}

		/// <summary>
		/// Get the typeName of the specified exception handler.
		/// </summary>
		/// <param name="configurationName">Exception handler to get the TypeName of.</param>
		/// <returns>TypeName [CLASS],[ASSEMBLY] of the specified exception handler.</returns>
		protected override Type GetConfigurationType(string configurationName)
		{
			ExceptionHandlerData data = GetExceptionHandlerData(configurationName);
			return GetType(data.TypeName);
		}

		/// <summary>
		/// Initialize the specified exception handler with its configuration object.
		/// </summary>
		/// <param name="providerName">Name of exception handler to initialize.</param>
		/// <param name="provider">Exception handler to initialize.</param>
		protected override void InitializeConfigurationProvider(string providerName, IConfigurationProvider provider)
		{
			((IExceptionHandler)provider).CurrentPolicyName = policyName;
			((IExceptionHandler)provider).CurrentExceptionTypeName = exceptionTypeName;

			base.InitializeConfigurationProvider (providerName, provider);
		}

		/// <summary>
		/// Get the configuration information for the specified exception handler.
		/// </summary>
		/// <param name="handlerName">Name of the exception handler to get the configuration of.</param>
		/// <returns>Configuration object for the specified exception handler.</returns>
		private ExceptionHandlerData GetExceptionHandlerData(string handlerName)
		{
			ExceptionHandlingConfiguration exceptionHandlingConfig = (ExceptionHandlingConfiguration)base.Config;
			return exceptionHandlingConfig.GetExceptionHandlerData(policyName, exceptionTypeName, handlerName);
		}

		/// <summary>
		/// Verify that the provided policy name and exception typeName are valid.
		/// </summary>
		/// <param name="policyName">Policy name to validate.</param>
		/// <param name="exceptionTypeName">Exception typeName to validate.</param>
		private static void ValidatePolicyAndExceptionType(string policyName, string exceptionTypeName)
		{
			ArgumentValidation.CheckForEmptyString(policyName, "policyName");
			ArgumentValidation.CheckForEmptyString(exceptionTypeName, "exceptionTypeName");
		}
	}
}
