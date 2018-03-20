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
	internal sealed class ExceptionPolicyFactory : ConfigurationFactory
	{
		private Exception currentException = null;

		public ExceptionPolicyFactory(ConfigurationBase config) : base("exceptionPolicyFactory", config)
		{
		}

		public ExceptionPolicy CreateExceptionPolicy(string policyName, Exception exception)
		{
			ArgumentValidation.CheckForNullReference(exception, "exception");
            
			currentException = exception;
			return (ExceptionPolicy)CreateInstance(policyName);
		}

		protected override Type GetConfigurationType(string policyName)
		{
			ExceptionHandlingConfiguration exceptionHandlingConfiguration = (ExceptionHandlingConfiguration)base.Config;
			ExceptionPolicyData policyData = exceptionHandlingConfiguration.GetExceptionPolicyData(policyName);
			return GetType(policyData.TypeName);
		}

		protected override void PublishFailureEvent(string name, Exception e)
		{
			ExceptionUtility.LogHandlingException(name, e, null, currentException);
		}

/*		private static string CreateProviderNotFoundExceptionMessage(Exception currentException, string policyName)
		{	
			//Used to publish exception handling error information to other sources		
		}  */
	}
}
