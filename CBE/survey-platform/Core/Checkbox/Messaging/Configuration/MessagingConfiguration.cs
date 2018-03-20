using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Messaging.Configuration
{
    /// <summary>
    /// Configuration data for messaging services.
    /// </summary>
    public class MessagingConfiguration : ConfigurationBase, IXmlConfigurationBase
    {
        private readonly Dictionary<string, ProviderData> _emailProviders;

        /// <summary>
        /// All registered providers
        /// </summary>
        public List<ProviderData> EMailProviders
        {
            get
            {
                return _emailProviders == null ? null : _emailProviders.Values.ToList();
            }
        }

        /// <summary>
		/// Constructor.
		/// </summary>
		public MessagingConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="sectionName"></param>
		public MessagingConfiguration(string sectionName) : base(sectionName)
		{
			try
			{
                _emailProviders = new Dictionary<string, ProviderData>();
				DefaultEmailProvider = string.Empty;
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			}
		}

        /// <summary>
        /// Name of the default email provider (if specified in the configuration file)
        /// </summary>
        public string DefaultEmailProvider { get; private set; }

        /// <summary>
        /// Get the custom configuration object for the specified email provider.
        /// </summary>
        /// <param name="providerName">Name of the email provider.</param>
        /// <returns>Custom configuration object that extends the <see cref="Prezza.Framework.Configuration.ProviderData" /> class.</returns>
        public ProviderData GetEmailProviderConfig(string providerName)
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(providerName, "providerName");

                if (_emailProviders.ContainsKey(providerName))
                    return _emailProviders[providerName];
                
                throw new Exception("No configuration for specified email provider exists.  Specified provider was: " + providerName);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                    throw;
                
                return null;
            }
        }

        #region IXmlConfigurationBase Members

        /// <summary>
        /// Load messaging configuration from XML.
        /// </summary>
        /// <param name="node">Root node of messaging configuration file/section.</param>
        public void LoadFromXml(XmlNode node)
        {
            try
            {
                ArgumentValidation.CheckForNullReference(node, "node");
                

                XmlNodeList emailProvidersList = node.SelectNodes("/messagingConfiguration/emailProviders/providers/provider");

                foreach (XmlNode provider in emailProvidersList)
                {
                    string providerName = XmlUtility.GetAttributeText(provider, "name");
                    string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
                    string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
                    bool isDefault = XmlUtility.GetAttributeBool(provider, "default");

                    if (providerName == string.Empty)
                        throw new Exception("An email provider was defined in the message configuration file, but the name was not specified: " + providerName);

                    if (providerConfigPath == string.Empty)
                        throw new Exception("An email provider was defined in the message configuration file, but the provider's configuration file path was not specified: " + providerName);

                    if (providerDataTypeName == string.Empty)
                        throw new Exception("An email provider was defined in the message configuration file, but the provider's configuration object type name was not specified: " + providerName);

                    object[] extraParams = { providerName };

                    ProviderData config = (ProviderData)ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

                    _emailProviders.Add(config.Name, config);

                    //See if this is the default
                    if (isDefault)
                    {
                        DefaultEmailProvider = config.Name;
                    }
                }

            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                    throw;
            }

        }

        #endregion
    }
}
