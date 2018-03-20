////===============================================================================
//// Prezza Technologies Application Framework
//// Copyright © Checkbox Survey Inc  All rights reserved.
//// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
////===============================================================================

//using System;

//using Prezza.Framework.Common;
//using Prezza.Framework.Configuration;
//using Prezza.Framework.ExceptionHandling;
//using Prezza.Framework.Security.Configuration;



//namespace Prezza.Framework.Security
//{
//    /// <summary>
//    /// Summary description for AuthenticationProviderFactory.
//    /// </summary>
//    /// <summary>
//    /// Handles creation and initialization of instances of authentication provider objects.
//    /// </summary>
//    public class AuthenticationProviderFactory : ProviderFactory
//    {
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="factoryName">Name of the provider factory instance.</param>
//        public AuthenticationProviderFactory(string factoryName) : base(factoryName, typeof(IAuthenticationProvider))  
//        {
//        }
		
//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        /// <param name="factoryName">Name of the provider factory instance.</param>
//        /// <param name="config">Security configuration information.</param>
//        public AuthenticationProviderFactory(string factoryName, SecurityConfiguration config) : base(factoryName, typeof(IAuthenticationProvider), config) 
//        {
//        }
		
//        /// <summary>
//        /// Get an instance of the default authentication provider.
//        /// </summary>
//        /// <returns>Instance of the default <see cref="IAuthenticationProvider" />.</returns>
//        /// <remarks>
//        /// Returns null if no default authentication provider is specified in the security configuration.
//        /// </remarks>
//        public IAuthenticationProvider GetAuthenticationProvider() 
//        {
//            try
//            {
//                return (IAuthenticationProvider)base.CreateDefaultInstance();
//            }
//            catch(Exception ex)
//            {
//                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

//                if(rethrow)
//                    throw;
//                else
//                    return null;
//            }				
//        }
//        /// <summary>
//        /// Get an instance of the authentication provider with the specified name.
//        /// </summary>
//        /// <param name="providerName">Name of the provider to get an instance of.</param>
//        /// <returns>Instance of the specified <see cref="IAuthenticationProvider" />.</returns>
//        public IAuthenticationProvider GetAuthenticationProvider(string providerName)
//        {
//            try
//            {
//                ArgumentValidation.CheckForEmptyString(providerName, "providerName");

//                return (IAuthenticationProvider)base.CreateInstance(providerName);	
//            }
//            catch(Exception ex)
//            {
//                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

//                if(rethrow)
//                    throw;
//                else
//                    return null;
//            }				

//        }
		
//        /// <summary>
//        /// Get the <see cref="ProviderData" /> of the specified provider.
//        /// </summary>
//        /// <param name="providerName">Name of the provider.</param>
//        /// <returns><see cref="System.Type" /></returns>
//        protected override ConfigurationBase GetConfigurationObject(string providerName) 
//        {
//            SecurityConfiguration config = (SecurityConfiguration)base.Config;
//            return (ProviderData)config.GetAuthenticationProviderConfig(providerName);
//        }

//        /// <summary>
//        /// Get the <see cref="System.Type" /> of the specified provider.
//        /// </summary>
//        /// <param name="authenticationProviderName">Name of the authentication provider</param>
//        /// <returns><see cref="System.Type" /> of the specified provider.</returns>
//        protected override Type GetConfigurationType(string authenticationProviderName)	
//        {
//            ArgumentValidation.CheckForEmptyString(authenticationProviderName, "authenticationProviderName");

//            ProviderData config = (ProviderData)GetConfigurationObject(authenticationProviderName);
//            return GetType(config.TypeName);
//        }

//        /// <summary>
//        /// Get the name of the default authentication provider (if specified in the configuration).
//        /// </summary>
//        /// <returns>Name of the default authentication provider.</returns>
//        protected override string GetDefaultInstanceName()
//        {
//            SecurityConfiguration config = (SecurityConfiguration)base.Config;
//            return config.DefaultAuthenticationProvider;
//        }
//    }
//}
