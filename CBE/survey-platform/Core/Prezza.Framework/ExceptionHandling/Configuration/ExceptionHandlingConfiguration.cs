//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Xml;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Configuration information for framework exception handling.
	/// </summary>
	public class ExceptionHandlingConfiguration : ConfigurationBase, IXmlConfigurationBase
	{
		/// <summary>
		/// Collection of exception policies, which define how to handle an exception in a given context.
		/// </summary>
		private readonly ExceptionPolicyDataCollection exceptionPolicies;

		/// <summary>
		/// Default constructor.  Sets the configuration name to be an empty string and
		/// initializes the exception policy collection.
		/// </summary>
		public ExceptionHandlingConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.  Initializes the exception policy configuration.
		/// </summary>
		/// <param name="name">Name of the exception handling configuration.</param>
		public ExceptionHandlingConfiguration(string name) : base(name)
		{
			exceptionPolicies = new ExceptionPolicyDataCollection();
		}

		/// <summary>
		/// Get the configuration information for the exception policy with the specified name.
		/// </summary>
		/// <param name="policyName">Name of the exception policy to get the configuration for.</param>
		/// <returns>An <see cref="ExceptionPolicyData"/> object containing the configuration for the specified exception policy.</returns>
		public ExceptionPolicyData GetExceptionPolicyData(string policyName)
		{
			ValidatePolicyName(policyName);
			ExceptionPolicyData data = exceptionPolicies[policyName];

			if(data == null)
			{
				throw new Exception("Unable to find configuration for request policy: " + policyName);
			}

			return data;
		}

		/// <summary>
		/// Get the configuration information for a given Exception handler, which is defined by the context consisting
		/// of the exception policy, exception type, and handler name.
		/// </summary>
		/// <param name="policyName">Name of the exception policy the handler exists in.</param>
		/// <param name="exceptionTypeName">Type of exception the handler is configured to handler.</param>
		/// <param name="handlerName">Name of the handler.</param>
		/// <returns>Configuration object for the exception handler.</returns>
		public ExceptionHandlerData GetExceptionHandlerData(string policyName, string exceptionTypeName, string handlerName)
		{
			ValidatePolicyName(policyName);
			ValidateExceptionTypeName(exceptionTypeName);
			ValidateHandlerName(handlerName);

			ExceptionHandlerDataCollection exceptionHandlers = GetExceptionHandlerDataCollection(policyName, exceptionTypeName);

			ExceptionHandlerData exceptionHandler = exceptionHandlers[handlerName];

			if(exceptionHandler == null)
			{
                throw new Exception("Unable to get configuration for exception handler: " + handlerName + " for exception type: " + exceptionTypeName + " and policy: " + policyName);
			}
			
			return exceptionHandler;
		}

		/// <summary>
		/// Get the collection of exception handler configuration objects for a given exception policy and exception type.
		/// </summary>
		/// <param name="policyName">Name of the exception policy.</param>
		/// <param name="exceptionTypeName">Type of exception.</param>
		/// <returns></returns>
		public ExceptionHandlerDataCollection GetExceptionHandlerDataCollection(string policyName, string exceptionTypeName)
		{
			ValidatePolicyName(policyName);
			ValidateExceptionTypeName(exceptionTypeName);

			ExceptionTypeData exceptionTypeData = GetExceptionTypeData(policyName, exceptionTypeName);

			return exceptionTypeData.ExceptionHandlers;
		}

		/// <summary>
		/// Get a collection of exception type configurations that a specified exception policy handles.
		/// </summary>
		/// <param name="policyName"></param>
		/// <returns></returns>
		public ExceptionTypeDataCollection GetExceptionTypeDataCollection(string policyName)
		{
			ValidatePolicyName(policyName);

			ExceptionPolicyData policyData = GetExceptionPolicyData(policyName);
			return policyData.ExceptionTypes;
		}

		/// <summary>
		/// Get configuration of an exception type with the specified policy name and exception type name.
		/// </summary>
		/// <param name="policyName">Name of the exception policy.</param>
		/// <param name="exceptionTypeName">Exception type name.</param>
		/// <returns></returns>
		public ExceptionTypeData GetExceptionTypeData(string policyName, string exceptionTypeName)
		{
			ValidatePolicyName(policyName);
			ValidateExceptionTypeName(exceptionTypeName);

			ExceptionTypeDataCollection exceptionTypes = GetExceptionTypeDataCollection(policyName);
			ExceptionTypeData exceptionType = exceptionTypes[exceptionTypeName];

			if(exceptionType == null)
			{
                throw new Exception("Unable to find the exception type: " + exceptionTypeName + "  for policy: " + policyName);
			}
			return exceptionType;
		}

		/// <summary>
		/// Verify that the policy name is valid.
		/// </summary>
		/// <param name="policyName">Name of policy to validate.</param>
		private static void ValidatePolicyName(string policyName)
		{
			ArgumentValidation.CheckForEmptyString(policyName, "policyName");
		}

		/// <summary>
		/// Verify that the exception handler name is valid.
		/// </summary>
		/// <param name="handlerName">Handler name to validate.</param>
		private static void ValidateHandlerName(string handlerName)
		{
			ArgumentValidation.CheckForEmptyString(handlerName, "handlerName");
		}

		/// <summary>
		/// Verify that the specified exception type name is valid.
		/// </summary>
		/// <param name="exceptionTypeName">Exception type name to validate.</param>
		private static void ValidateExceptionTypeName(string exceptionTypeName)
		{
			ArgumentValidation.CheckForEmptyString(exceptionTypeName, "exceptionTypeName");
		}

		/// <summary>
		/// Load the exception handling configuration from the specified Xml node.
		/// </summary>
		/// <param name="node">Xml node containing exception handling configuration.</param>
		public void LoadFromXml(XmlNode node)
		{
			//Load policies
			XmlNodeList policiesList = node.SelectNodes("/exceptionHandlingConfiguration/policies/policy");

			foreach(XmlNode policyNode in policiesList)
			{
				var policyData = new ExceptionPolicyData(XmlUtility.GetAttributeText(policyNode, "name", true));

				//Exception types
				XmlNodeList exceptionTypes = policyNode.SelectNodes("exceptionTypes/exceptionType");

				foreach(XmlNode exceptionTypeNode in exceptionTypes)
				{
					var exceptionTypeData = new ExceptionTypeData();

					exceptionTypeData.Name = XmlUtility.GetAttributeText(exceptionTypeNode, "name", true);
					exceptionTypeData.TypeName = XmlUtility.GetAttributeText(exceptionTypeNode, "type", true);
					exceptionTypeData.PostHandlingAction = (PostHandlingAction)XmlUtility.GetAttributeEnum(exceptionTypeNode, "postHandlingAction", typeof(PostHandlingAction), true);

					//Exception Handlers
					XmlNodeList exceptionHandlers = exceptionTypeNode.SelectNodes("exceptionHandlers/exceptionHandler");

					foreach(XmlNode exceptionHandlerNode in exceptionHandlers)
					{
						string handlerName = XmlUtility.GetAttributeText(exceptionHandlerNode, "name", true);

						XmlNode exceptionHandlerConfigNode = node.SelectSingleNode("/exceptionHandlingConfiguration/exceptionHandlers/exceptionHandler[@name='" + handlerName + "']");
						string configDataType = XmlUtility.GetAttributeText(exceptionHandlerConfigNode, "configDataType", true);
						string filePath = XmlUtility.GetAttributeText(exceptionHandlerConfigNode, "filePath", true);

						object[] extraParams = {handlerName};
						var handlerData = (ExceptionHandlerData)ConfigurationManager.GetConfiguration(filePath, configDataType, extraParams);

						exceptionTypeData.ExceptionHandlers.Add(handlerData);
					}
					policyData.ExceptionTypes.Add(exceptionTypeData);
				}

				exceptionPolicies.Add(policyData);
			}
		}
	}
}
