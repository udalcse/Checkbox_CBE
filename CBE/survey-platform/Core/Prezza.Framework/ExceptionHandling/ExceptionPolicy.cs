//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;

using Prezza.Framework.Common;
using Prezza.Framework.Logging;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling.Configuration;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Provide methods to handle exceptions according to specified policies.
	/// </summary>
	public sealed class ExceptionPolicy
	{
		/// <summary>
		/// Collection of exception handling policy configurations.
		/// </summary>
		private Hashtable policyEntries;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ExceptionPolicy()
		{
		}

		/// <summary>
		/// Handle the specified exception according to the configuration of the specified exception handling policy.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <param name="policyName">Policy to use.</param>
		/// <returns>Boolean indicating if the exception should be rethrown by the calling method.</returns>
		public static bool HandleException(Exception ex, string policyName)
		{
			return HandleException(ref ex, policyName);
		}

		/// <summary>
		/// Handle the specified exception according to the configuration of the specified exception handling policy.  Since the 
		/// exception is passed by reference, it may be modified according by the configured exception handlers.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <param name="policyName">Policy to use.</param>
		/// <returns>Boolean indicating if the exception should be rethrown by the calling method.</returns>
		public static bool HandleException(ref Exception ex, string policyName)
		{
			ExceptionHandlingConfiguration config = (ExceptionHandlingConfiguration)ConfigurationManager.GetConfiguration("checkboxExceptionHandlingConfiguration");
			return HandleException(ref ex, policyName, config);
		}

		/// <summary>
		/// Handle the specified exception according to the configuration of the specified exception handling policy, which is stored
		/// in the supplied configuration object.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <param name="policyName">Policy to use.</param>
		/// <param name="config">Configuration object containing exception handling configuration information.</param>
		/// <returns>Boolean indicating if the exception should be rethrown by the calling method.</returns>
		public static bool HandleException(Exception ex, string policyName, ConfigurationBase config)
		{
			return HandleException(ref ex, policyName, config);
		}

		/// <summary>
		/// Handle the specified exception according to the configuration of the specified exception handling policy, which is
		/// stored in the supplied configuration object.  The exception is passed by reference, so it may be modified by
		/// an exception handler.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <param name="policyName">Policy to use.</param>
		/// <param name="config">Configuration object containing exception handling information.</param>
		/// <returns>Boolean indicating if the exception should be rethrown by the calling method.</returns>
		public static bool HandleException(ref Exception ex, string policyName, ConfigurationBase config)
		{
			ArgumentValidation.CheckForNullReference(ex, "ex");
			ArgumentValidation.CheckForEmptyString(policyName, "policyName");
			ArgumentValidation.CheckForNullReference(config, "config");
			ArgumentValidation.CheckExpectedType(config, typeof(ExceptionHandlingConfiguration));
			
			ExceptionPolicy policy = GetExceptionPolicy(ex, policyName, config);
			policy.Initialize(config, policyName);
			return policy.HandleException(ref ex);
		}

		/// <summary>
		/// Handle the specified exception.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <returns>True if the exception should be rethrown.</returns>
		private bool HandleException(ref Exception ex)
		{
			Type exceptionType = ex.GetType();
			ExceptionPolicyEntry entry = this.FindExceptionPolicyEntry(exceptionType);

			bool recommendRethrow = false;

			if(entry == null)
			{
				recommendRethrow = true;
				Logger.Write("An exception of type " + exceptionType.ToString() + " was not handled.  Error message was: " + ex.Message);
			}
			else
			{
				try
				{
					recommendRethrow = entry.Handle(ref ex);
					//ExceptionHandledEvent.Fire();
				}
				catch(ExceptionHandlingException)
				{
					throw ex;
				}
			}

			return recommendRethrow;
		}

		/// <summary>
		/// Get an instance of the specified exception policy.
		/// </summary>
		/// <param name="exception">Exception to get the exception policy for.</param>
		/// <param name="policyName">Name of exception policy.</param>
		/// <param name="config">Object containing security configuration information.</param>
		/// <returns>Exception policy instance.</returns>
		private static ExceptionPolicy GetExceptionPolicy(Exception exception, string policyName, ConfigurationBase config)
		{
			try
			{
				ExceptionPolicyFactory factory = new ExceptionPolicyFactory(config);
				return factory.CreateExceptionPolicy(policyName, exception);
			}
			catch(InvalidOperationException ex)
			{
				Prezza.Framework.ExceptionHandling.ExceptionUtility.LogHandlingException(policyName, ex, null, exception);
				throw new ExceptionHandlingException(ex.Message, ex);
			}
            catch (Exception ex)
            {
                ExceptionUtility.LogHandlingException(policyName, ex, null, exception);
                throw new ExceptionHandlingException(ex.Message, ex);
            }
		}

		/// <summary>
		/// Initialize the exception policy instance with the specified configuration and policy name.
		/// </summary>
		/// <param name="config">Configuration information for exception handling.</param>
		/// <param name="policyName">Name of the policy.</param>
		private void Initialize(ConfigurationBase config, string policyName)
		{
			ExceptionHandlingConfiguration exceptionHandlingConfiguration = (ExceptionHandlingConfiguration)config;
			ExceptionPolicyData policyData = exceptionHandlingConfiguration.GetExceptionPolicyData(policyName);
			this.AddPoliciesToCache(policyData, exceptionHandlingConfiguration);
		}

		/// <summary>
		/// Add policy entries to a class cache.
		/// </summary>
		/// <param name="policyData">Policy configuration information.</param>
		/// <param name="config">Exception handling configuration information.</param>
		private void AddPoliciesToCache(ExceptionPolicyData policyData, ConfigurationBase config)
		{
			this.policyEntries = new Hashtable(policyData.ExceptionTypes.Count);

			foreach(ExceptionTypeData typeData in policyData.ExceptionTypes)
			{
				Type exceptionType = GetExceptionType(typeData, policyData.Name);
				ExceptionPolicyEntry exceptionEntry = new ExceptionPolicyEntry(policyData.Name, typeData, config);
				this.policyEntries.Add(exceptionType, exceptionEntry);
			}
		}

		/// <summary>
		/// Get the TypeName of an exception type.
		/// </summary>
		/// <param name="typeData">Exception type configuration information.</param>
		/// <param name="policyName">Name of the policy containing the exception type information.</param>
		/// <returns></returns>
		private Type GetExceptionType(ExceptionTypeData typeData, string policyName)
		{
			try
			{
				return Type.GetType(typeData.TypeName, true);
			}
			catch(TypeLoadException ex)
			{
				ExceptionUtility.LogHandlingException(policyName, null, null, ex);
				throw new ExceptionHandlingException("Unknown type in configuration for policy: " + policyName, ex);
			}
		}

		/// <summary>
		/// Get the configuration information for the specified exception type handled by the policy.
		/// </summary>
		/// <param name="exceptionType">Type of the exception.</param>
		/// <returns>Configuration information for specified exception type.</returns>
		private ExceptionPolicyEntry GetPolicyEntry(Type exceptionType)
		{
			return (ExceptionPolicyEntry)this.policyEntries[exceptionType];
		}

		/// <summary>
		/// Iterate up the class hierarchy to find a matching exception type.  Should work as long as all exceptions
		/// have <see cref="System.Exception"/> as the ultimate base class and the policy is configured to handle
		/// that type.
		/// </summary>
		/// <param name="exceptionType">Exception type to find a policy entry for.</param>
		/// <returns>Configuration information for the specified exception type.</returns>
		private ExceptionPolicyEntry FindExceptionPolicyEntry(Type exceptionType)
		{
			ExceptionPolicyEntry entry = null;

			while(exceptionType != typeof(Object))
			{
				entry = this.GetPolicyEntry(exceptionType);

				if(entry == null)
				{
					exceptionType = exceptionType.BaseType;
				}
				else
				{
					break;
				}
			}

			return entry;
		}
	}
}
