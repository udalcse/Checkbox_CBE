using System;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Data store for database relay email provider.
    /// </summary>
    public class DatabaseRelayEmailProviderData : ProviderData, IXmlConfigurationBase
    {
        /// <summary>
        /// Construct an instance of a new email provider data object.
        /// </summary>
        /// <param name="providerName"></param>
        public DatabaseRelayEmailProviderData(string providerName)
            : base(providerName, typeof(DatabaseRelayEmailProvider).AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Get name of database connection to use for queuing emails.
        /// </summary>
        public string MailDbInstanceName { get; private set; }


        #region IXmlConfigurationBase Members

        /// <summary>
        /// Load provider configuration data from Xml
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
            if (node == null)
            {
                throw new Exception("Unable to load DatabaseRelayEmailProviderData from null Xml node.");
            }

            MailDbInstanceName = XmlUtility.GetNodeText(node.SelectSingleNode("/databaseRelayEmailProviderData/mailDbInstanceName"), true);
        }

        #endregion
    }
}
