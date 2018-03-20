//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Collections;

using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Data.Configuration;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Configuration for database access class
    /// </summary>
    public class DatabaseConfiguration : ConfigurationBase, IXmlConfigurationBase
    {
        private DatabaseSettings databaseSettings;

        /// <summary>
        /// Hashtable of configured database providers
        /// </summary>
        protected Hashtable providers;

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatabaseConfiguration() : base(string.Empty) { }
        
        /// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="sectionName"></param>
		public DatabaseConfiguration(string sectionName) : base(sectionName)
		{
			try
			{
				providers = new Hashtable();
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

				if(rethrow)
					throw;
			}
		}

        /// <summary>
        /// Get the settings configuration
        /// </summary>
        /// <returns></returns>
        public DatabaseSettings GetDatabaseSettings()
        {
            return databaseSettings;
        }

        /// <summary>
        /// Load the configuration for the specified provider
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public DatabaseProviderData GetDatabaseProviderConfig(string providerName)
        {
            try
            {
                
                InstanceData instance = databaseSettings.Instances[providerName];
                if (null == instance)
                {
                    throw new Exception(string.Format("No InstanceData found for provider: {0}.", providerName));
                }

                DatabaseTypeData databaseType = databaseSettings.DatabaseTypes[instance.DatabaseTypeName];
                if (null == databaseType)
                {
                    throw new Exception(string.Format("No DatabaseTypeData found for provider: {0} and database type name: {1}.", providerName, instance.DatabaseTypeName));
                }

                return new DatabaseProviderData(instance, databaseType);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
                
                return null;
            }	
        }

        /// <summary>
        /// Get the name of the default db provider
        /// </summary>
        public string DefaultDatabaseProvider
        {
            get { return databaseSettings.DefaultInstance; }
        }

        #region IXmlConfigurationBase Members

        /// <summary>
        /// Load the data from XML
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(node.OuterXml));
            XmlSerializer serializer = new XmlSerializer(typeof(DatabaseSettings), "");
            databaseSettings = serializer.Deserialize(reader) as DatabaseSettings;
        }
        #endregion
    }
}
