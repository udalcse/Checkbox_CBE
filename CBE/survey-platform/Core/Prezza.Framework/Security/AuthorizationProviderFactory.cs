//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using Prezza.Framework.Configuration;

using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Configuration;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Handles creation and initialization of instances of authorization provider objects.
	/// </summary>
	public class AuthorizationProviderFactory : ProviderFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
		public AuthorizationProviderFactory(string factoryName) : base(factoryName, typeof(IAuthorizationProvider)) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
		/// <param name="config">Security configuration information.</param>
		public AuthorizationProviderFactory(string factoryName, SecurityConfiguration config) : base(factoryName, typeof(IAuthorizationProvider), config) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
				ArgumentValidation.CheckForNullReference(config, "config");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}

		/// <summary>
		/// Get an instance of an authorization provider.
		/// </summary>
		/// <returns>Instance of the default <see cref="IAuthorizationProvider" />.</returns>
		public IAuthorizationProvider GetAuthorizationProvider()
		{
			try
			{
				return (IAuthorizationProvider)base.CreateDefaultInstance();
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
				else
					return null;
			}
		}

		/// <summary>
		/// Get an instance of an authorization provider with the specified name.
		/// </summary>
		/// <param name="providerName">Name of the provider to get an instance of.</param>
		/// <returns>Instance of the specified <see cref="IAuthorizationProvider" />.</returns>
		public IAuthorizationProvider GetAuthorizationProvider(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				return (IAuthorizationProvider)base.CreateInstance(providerName);
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
				else
					return null;
			}
		}

		/// <summary>
		/// Get the configuration object for the specified provider.
		/// </summary>
		/// <param name="providerName">Name of the provider to get configuration for.</param>
		/// <returns><see cref="ProviderData" /> object for the specified provider.</returns>
		protected override ConfigurationBase GetConfigurationObject(string providerName)
		{
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");

			SecurityConfiguration config = (SecurityConfiguration)base.Config;
			return config.GetAuthorizationProviderConfig(providerName);
		}

		/// <summary>
		/// Get the <see cref="System.Type" /> of the specified authorization provider.
		/// </summary>
		/// <param name="authorizationProviderName">Name of the authorization provider.</param>
		/// <returns><see cref="System.Type" /> of the specified authorization provider.</returns>
		protected override Type GetConfigurationType(string authorizationProviderName) 
		{
		
			ArgumentValidation.CheckForEmptyString(authorizationProviderName, "authorizationProviderName");

			ProviderData config = (ProviderData)GetConfigurationObject(authorizationProviderName);
			return GetType(config.TypeName);
		}

		/// <summary>
		/// Get the name of the default authorization provider (if specified in the configuration).
		/// </summary>
		/// <returns>Name of the default authorization provider.</returns>
		protected override string GetDefaultInstanceName()
		{
			SecurityConfiguration config = (SecurityConfiguration)base.Config;
			return config.DefaultAuthorizationProvider;
		}
	}	
}
