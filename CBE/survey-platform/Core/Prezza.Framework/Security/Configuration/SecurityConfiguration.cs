//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Security.Configuration
{
	/// <summary>
	///	Configuration object for framework security configuration information, including configuration information
	///	for the various security providers.
	/// </summary>
	public class SecurityConfiguration : ConfigurationBase, IXmlConfigurationBase
	{
		/// <summary>
		/// Collection of authentication provider configuration objects.
		/// </summary>
		protected Hashtable authenticationProviders;

		/// <summary>
		/// Collection of authorization provider configuration objects.
		/// </summary>
		protected Hashtable authorizationProviders;

		/// <summary>
		/// Collection of profile provider configuration objects
		/// </summary>
		protected Hashtable profileProviders;

        /// <summary>
        /// Collection of sessionToken provider configuration objects
        /// </summary>
        protected Hashtable sessionTokenProviders;

		/// <summary>
		/// Name of the default authentication provider.
		/// </summary>
		protected string defaultAuthenticationProvider;

		/// <summary>
		/// Name of the default authorization provider.
		/// </summary>
		protected string defaultAuthorizationProvider;

		/// <summary>
		/// Name of the default profile provider.
		/// </summary>
		protected string defaultProfileProvider;

        /// <summary>
        /// Name of the default session token provider
        /// </summary>
        protected string defaultSessionTokenProvider;
		/// <summary>
		/// Configuration for the security cache
		/// </summary>
		protected ProviderData cacheConfiguration;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SecurityConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="sectionName"></param>
		public SecurityConfiguration(string sectionName) : base(sectionName)
		{
			try
			{
				authenticationProviders = new Hashtable();
				authorizationProviders = new Hashtable();
				profileProviders = new Hashtable();

				defaultAuthenticationProvider = string.Empty;
				defaultAuthorizationProvider = string.Empty;
				defaultProfileProvider = string.Empty;
                defaultSessionTokenProvider = string.Empty;
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}

		/// <summary>
		/// Get the custom configuration object for the specified authentication provider.
		/// </summary>
		/// <param name="providerName">Name of the authentication provider.</param>
		/// <returns>Custom configuration object that extends the <see cref="Prezza.Framework.Configuration.ProviderData" /> class.</returns>
		public ProviderData GetAuthenticationProviderConfig(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				if(authenticationProviders.Contains(providerName))
					return (ProviderData)authenticationProviders[providerName];
				else
					throw new Exception("No configuration for specified authentication provider exists.  Specified provider was: " + providerName);
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
		/// Get the custom configuration object for the specified authorization provider.
		/// </summary>
		/// <param name="providerName">Name of the authorization provider.</param>
		/// <returns>Custom configuration object that extends the <see cref="ProviderData" /> class.</returns>
		public ProviderData GetAuthorizationProviderConfig(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				if(authorizationProviders.Contains(providerName))
					return (ProviderData)authorizationProviders[providerName];
				else
					throw new Exception("No configuration for specified authorization provider exists.  Specified provider was: " + providerName);
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
		/// Get the custom configuration object for the specified profile provider.
		/// </summary>
		/// <param name="providerName">Name of the profile provider.</param>
		/// <returns>Custom configuration object that extends the <see cref="ProviderData" /> class.</returns>
		public ProviderData GetProfileProviderConfig(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				if(profileProviders.Contains(providerName))
					return (ProviderData)profileProviders[providerName];
				else
					throw new Exception("No configuration for specified profile provider exists.  Specified provider was: " + providerName);
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
        /// Get the configuration for the specified session security token provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public ProviderData GetSessionTokenProviderConfig(string providerName)
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(providerName, "providerName");

                if(sessionTokenProviders.Contains(providerName))
                    return (ProviderData)sessionTokenProviders[providerName];
                else
                    throw new Exception("No configuration for specified sessiontoken provider exists.  Specified provider was: " + providerName);
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
		/// Get the configuration object for the security cache provider.
		/// </summary>
		/// <returns><see cref="ProviderData"/> configuration for security cache provider.</returns>
		public ProviderData GetCacheProviderConfig()
		{
			try
			{
				return cacheConfiguration;
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw ex;
				else
					return null;
			}
		}

		/// <summary>
		/// Load framework security configuration from Xml.
		/// </summary>
		/// <param name="node">Xml node containing framework security information.</param>
		/// <remarks>
		/// In addition to loading framework level security information, this method will cause the configurations
		/// for custom providers to be loaded through calls to ConfigurationManager.GetConfiguration (<see cref="ConfigurationManager"/>).  The GetConfiguration
		/// method will be called for each child node in the providers sub groups.
		/// </remarks>
		public void LoadFromXml(XmlNode node)
		{
			try
			{
				ArgumentValidation.CheckForNullReference(node, "node");

				//Authentication Providers
				authenticationProviders = new Hashtable();
				
				XmlNodeList authenticationProvidersList = node.SelectNodes("/securityConfiguration/providers/provider[@type='authentication']");

				foreach(XmlNode provider in authenticationProvidersList)
				{
					string providerName = XmlUtility.GetAttributeText(provider, "name");
					string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
					string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
					bool isDefault = XmlUtility.GetAttributeBool(provider, "default");
				
					if(providerName == string.Empty)
						throw new Exception("An authentication provider was defined in the security configuration file, but the name was not specified.");

					if(providerConfigPath == string.Empty)
						throw new Exception("An authentication provider was defined in the security configuration file, but the provider's configuration file path was not specified.");

					if(providerDataTypeName == string.Empty)
						throw new Exception("An authentication provider was defined in the security configuration file, but the provider's configuration object type name was not specified.");

					object[] extraParams = {providerName};

					ProviderData config = (ProviderData)ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

					if(config == null)
						throw new Exception("Authentication provider [" + providerName + "] was specified but it's configuration could not be loaded.");

					authenticationProviders.Add(config.Name, config);

					//See if this is the default
					if(isDefault)
					{
						defaultAuthenticationProvider = config.Name;
					}
				}

				//Authorization Providers
				authorizationProviders = new Hashtable();
				
				XmlNodeList authorizationProvidersList = node.SelectNodes("/securityConfiguration/providers/provider[@type='authorization']");

				foreach(XmlNode provider in authorizationProvidersList)
				{
					string providerName = XmlUtility.GetAttributeText(provider, "name");
					string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
					string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
					bool isDefault = XmlUtility.GetAttributeBool(provider, "default");
				
					if(providerName == string.Empty)
						throw new Exception("An authorization provider was defined in the security configuration file, but the name was not specified.");

					if(providerConfigPath == string.Empty)
						throw new Exception("An authorization provider was defined in the security configuration file, but the provider's configuration file path was not specified.");

					if(providerDataTypeName == string.Empty)
						throw new Exception("An authorization provider was defined in the security configuration file, but the provider's configuration object type name was not specified.");

					object[] extraParams = {providerName};

					ProviderData config = (ProviderData)ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

					if(config == null)
						throw new Exception("Authorization provider [" + providerName + "] was specified but it's configuration could not be loaded.");

					authorizationProviders.Add(config.Name, config);

					//See if this is the default
					if(isDefault)
					{
						defaultAuthorizationProvider = config.Name;
					}
				}

				//Profile Providers
				profileProviders = new Hashtable();
				
				XmlNodeList profileProvidersList = node.SelectNodes("/securityConfiguration/providers/provider[@type='profile']");

				foreach(XmlNode provider in profileProvidersList)
				{
					string providerName = XmlUtility.GetAttributeText(provider, "name");
					string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
					string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
					bool isDefault = XmlUtility.GetAttributeBool(provider, "default");
				
					if(providerName == string.Empty)
						throw new Exception("An profile provider was defined in the security configuration file, but the name was not specified.");

					if(providerConfigPath == string.Empty)
						throw new Exception("An profile provider was defined in the security configuration file, but the provider's configuration file path was not specified.");

					if(providerDataTypeName == string.Empty)
						throw new Exception("An profile provider was defined in the security configuration file, but the provider's configuration object type name was not specified.");

					object[] extraParams = {providerName};

					ProviderData config = (ProviderData)ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

					if(config == null)
						throw new Exception("Profile provider [" + providerName + "] was specified but it's configuration could not be loaded.");

					profileProviders.Add(config.Name, config);

					//See if this is the default
					if(isDefault)
					{
						defaultProfileProvider = config.Name;
					}
				}

                // Session Token Providers
                sessionTokenProviders = new Hashtable();

                XmlNodeList sessionProvidersList = node.SelectNodes("/securityConfiguration/providers/provider[@type='session']");

                foreach(XmlNode provider in sessionProvidersList)
                {
                    string providerName = XmlUtility.GetAttributeText(provider, "name");
                    string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
                    string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
                    bool isDefault = XmlUtility.GetAttributeBool(provider, "default");
                
                    if(providerName == string.Empty)
                        throw new Exception("An profile provider was defined in the security configuration file, but the name was not specified.");

                    if(providerConfigPath == string.Empty)
                        throw new Exception("An profile provider was defined in the security configuration file, but the provider's configuration file path was not specified.");

                    if(providerDataTypeName == string.Empty)
                        throw new Exception("An profile provider was defined in the security configuration file, but the provider's configuration object type name was not specified.");

                    object[] extraParams = {providerName};

                    ProviderData config = (ProviderData)ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

                    if(config == null)
                        throw new Exception("SessionToken provider [" + providerName + "] was specified but it's configuration could not be loaded.");

                    sessionTokenProviders.Add(config.Name, config);

                    //See if this is the default
                    if(isDefault)
                    {
                        defaultSessionTokenProvider = config.Name;
                    }
                }

				//Load the security cache configuration
				XmlNode securityCacheNode = node.SelectSingleNode("/securityConfiguration/cache/securityCacheProvider");

				if(securityCacheNode != null)
				{
					string cacheProviderName = XmlUtility.GetAttributeText(securityCacheNode, "name");
					string cacheProviderConfigPath = XmlUtility.GetAttributeText(securityCacheNode, "filePath");
					string cacheProviderDataType = XmlUtility.GetAttributeText(securityCacheNode, "configDataType");

					if(cacheProviderName == string.Empty)
						throw new Exception("A security cache provider was specified in the security configuration file, but a name was not.");

					if(cacheProviderConfigPath == string.Empty)
						throw new Exception("A security cache provider was specified in the security configuration file, but a configuration file path was not.");

					if(cacheProviderDataType == string.Empty)
						throw new Exception("A security cache provider was specified in the security configuration file, but a configuration data type was not.");

					object[] extraParams = {cacheProviderName};

					cacheConfiguration = (ProviderData)ConfigurationManager.GetConfiguration(cacheProviderConfigPath, cacheProviderDataType, extraParams);
				}
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}


        /// <summary>
        /// Get a collection of available profile providers
        /// </summary>
        public ReadOnlyCollection<string> AvailableProfileProviders
        {
            get
            {
                List<string> providers  = new List<string>();
                foreach (object key in profileProviders.Keys)
                {
                    providers.Add(key.ToString());
                }

                return new ReadOnlyCollection<string>(providers);
            }
        }

        /// <summary>
        /// Get a collection of available authorization providers
        /// </summary>
        public ReadOnlyCollection<string> AvailableAuthorizationProviders
        {
            get
            {
                List<string> providers = new List<string>();
                foreach (object key in authorizationProviders.Keys)
                {
                    providers.Add(key.ToString());
                }

                return new ReadOnlyCollection<string>(providers);
            }

        }

        /// <summary>
        /// Get a collection of available authentication providers
        /// </summary>
        public ReadOnlyCollection<string> AvailableAuthenticationProviders
        {
            get
            {
                List<string> providers = new List<string>();
                foreach (object key in authenticationProviders.Keys)
                {
                    providers.Add(key.ToString());
                }

                return new ReadOnlyCollection<string>(providers);
            }
        }

		/// <summary>
		/// Name of the default authentication provider (if specified in the configuration file)
		/// </summary>
		public string DefaultAuthenticationProvider
		{
			get{return defaultAuthenticationProvider;}
			set{defaultAuthenticationProvider = value;}
		}
		
		/// <summary>
		/// Name of the default authorization provider (if specified in the configuration file)
		/// </summary>
		public string DefaultAuthorizationProvider
		{
			get{return defaultAuthorizationProvider;}
			set{defaultAuthorizationProvider = value;}
		}

		/// <summary>
		/// Name of the default profile provider (if specified in the configuration file)
		/// </summary>
		public string DefaultProfileProvider
		{
			get{return defaultProfileProvider;}
			set{defaultProfileProvider = value;}
		}

        /// <summary>
        /// Get the name of the default session security token provider
        /// </summary>
        public string DefaultSessionTokenProvider
        {
            get{return defaultSessionTokenProvider;}
            set{defaultSessionTokenProvider = value;}
        }
	}
}
