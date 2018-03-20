//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Collections;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Configuration;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Creates and initializes authorization provider objects.
	/// </summary>
	/// <remarks>
	/// The authentication factory creates authorization provider objects that
	/// implement the <see cref="IAuthorizationProvider" /> interface.
	/// </remarks>
	public sealed class AuthorizationFactory
	{
		private static readonly Hashtable _providers;

		/// <summary>
		/// Constructor.  Since this class only exposes static methods, there
		/// should be no need to call the constructor.
		/// </summary>
		static AuthorizationFactory()
		{
			lock(typeof(AuthorizationFactory))
			{
				_providers = new Hashtable();
			}
			ConfigurationManager.ConfigurationChanged += ConfigurationManager_ConfigurationChanged;
		}

		/// <summary>
		/// Create and initialize an instance of the default authorization 
		/// provider. 
		/// </summary>
		/// <returns>Initialized instance of an authorization provider.</returns>
		/// <remarks>
		/// The returned authorization provider implements the 
		/// <see cref="IAuthorizationProvider" /> interface.</remarks>
        /// <exception cref="Exception">Unable to create default IAuthorizationProvider</exception>
		public static IAuthorizationProvider GetAuthorizationProvider() 
		{
			try
			{
				lock(_providers.SyncRoot)
				{
					if(_providers.Contains("[DEFAULT]"))
					{
						return (IAuthorizationProvider)_providers["[DEFAULT]"];
					}
				}

                AuthorizationProviderFactory factory = new AuthorizationProviderFactory("authorizationProviderFactory", (SecurityConfiguration)ConfigurationManager.GetConfiguration("checkboxSecurityConfiguration"));
				IAuthorizationProvider provider = factory.GetAuthorizationProvider();

				lock(_providers.SyncRoot)
				{
					if(!_providers.Contains("[DEFAULT]"))
					{
						_providers.Add("[DEFAULT]", provider);
					}
				}

				return provider;
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			    
                return null;
			}				
		}

		/// <summary>
		/// Create and initialize an instance of the specified authorization 
		/// provider. 
		/// </summary>
		/// <param name="authorizationProvider">Name of the authorization provider to instantiate and initialize.</param>
		/// <returns>Initialized instance of an authorization provider.</returns>
		/// <remarks>
		/// The returned authorization provider implements the 
		/// <see cref="IAuthorizationProvider" /> interface.</remarks>
		/// <exception cref="ArgumentNullException">authorizationProvider is null</exception>
		/// <exception cref="ArgumentException">authorizationProvider is empty</exception>
        /// <exception cref="Exception">Could not find instance specified in authorizationProvider</exception>
		/// <exception cref="InvalidOperationException">Error processing configuration information defined in application configuration file.</exception>
		public static IAuthorizationProvider GetAuthorizationProvider(string authorizationProvider) 
		{
			try
			{
				lock(_providers.SyncRoot)
				{
					if(_providers.Contains(authorizationProvider))
					{
						return (IAuthorizationProvider)_providers[authorizationProvider];
					}
				}

                AuthorizationProviderFactory factory = new AuthorizationProviderFactory("authorizationProviderFactory", (SecurityConfiguration)ConfigurationManager.GetConfiguration("checkboxSecurityConfiguration"));
				IAuthorizationProvider provider = factory.GetAuthorizationProvider(authorizationProvider);
				
				lock(_providers.SyncRoot)
				{
					if(!_providers.Contains(authorizationProvider))
					{
						_providers.Add(authorizationProvider, provider);
					}
				}

				return provider;
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			    
                return null;
			}				
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private static void ConfigurationManager_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
		{
			if(_providers != null)
				_providers.Clear();
		}
	}
}
