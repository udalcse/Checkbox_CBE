//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.IO;
using System.Xml;
using Prezza.Framework.Common;

namespace Checkbox.Configuration
{
	/// <summary>
	/// Class to modify system configuration
	/// </summary>
	public static class ConfigurationWriter
	{
		/// <summary>
		/// Set the file paths to be in the applicationConfigFolder
		/// </summary>
		/// <param name="applicationFolder">Folder where the application is installed.</param>
		/// <param name="applicationConfigFolder">Folder where the appliation configuration files are placed.</param>
		/// <param name="connectionString">The database connection string.</param>
		/// <param name="dbProvider"></param>
		/// <param name="status">Status of the operation</param>
		/// <returns>True on success, false on failure.</returns>
		public static bool UpdateConfigFiles(string applicationFolder, string applicationConfigFolder, string connectionString, string dbProvider, out string status)
		{
		    string currentConfigFile = string.Empty;

			try
			{

				if(applicationFolder == null || applicationConfigFolder == null || applicationFolder == string.Empty || applicationConfigFolder == string.Empty)
				{
					status = "Application folder and application config folder must be specified.";
					return false;
				}
			
				//1: Verify file paths exist
				if(!Directory.Exists(applicationFolder))
				{
					status = "Specified application folder [" + applicationFolder + "] does not exist.";
					return false;
				}

				if(!Directory.Exists(applicationConfigFolder))
				{
                    status = "Specified application configuration folder [" + applicationConfigFolder + "] does not exist.";
					return false;
				}

                //2: Update database configuration
                if (!UpdateDatabaseConfiguration(applicationConfigFolder, dbProvider, out status))
                {
                    return false;
                }

                // NWC: 2010_05_20 - No longer necessary to overwrite configuration files since Checkbox will infer locations
                
                ////3: Update configuration files
                //foreach(string xmlFile in Directory.GetFiles(applicationConfigFolder, "*.xml"))
                //{
                //    XmlDocument doc = new XmlDocument();

                //    doc.Load(xmlFile);

                //    //Get all of the path elements
                //    XmlNodeList nodeList = doc.SelectNodes("//*[@filePath]");

                //    foreach(XmlNode node in nodeList)
                //    {
                //        if(!UpdateFilePath(node, applicationConfigFolder, out status))
                //        {
                //            return false;
                //        }
                //    }

                //    doc.Save(xmlFile);                    
                //}

                ////4: Update the web.config
                //if (!UpdateWebConfig(applicationFolder, applicationConfigFolder, out status))
                //{
                //    return false;
                //}

				return true;

			}
			catch(Exception ex)
			{
                if (currentConfigFile != String.Empty)
                    status = "An exception occurred while updating " + currentConfigFile + ".<br />  The exception was [" + ex.Message + "].";
                else
                    status = "An exception occurred while updating file paths.  The exception was [" + ex.Message + "].";
			    return false;
			}
		}

        /// <summary>
        /// Updates the database provider in the database configuration file
        /// </summary>
        /// <param name="applicationConfigFolder"></param>
        /// <param name="dbProvider"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static bool UpdateDatabaseConfiguration(string applicationConfigFolder, string dbProvider, out string status)
        {
            status = string.Empty;
            applicationConfigFolder += Path.DirectorySeparatorChar.ToString();

            if (!File.Exists(applicationConfigFolder + "DatabaseConfiguration.xml"))
            {
                status = "Unable to find DatabaseConfiguration.xml in specified configuration directory [" + applicationConfigFolder + "].";
                return false;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(applicationConfigFolder + "DatabaseConfiguration.xml");

            XmlNodeList instanceNodes = doc.SelectNodes("/databaseSettings/instances/instance");

            foreach (XmlNode node in instanceNodes)
            {
                node.Attributes["type"].InnerText = dbProvider;
            }

            doc.Save(applicationConfigFolder + "DatabaseConfiguration.xml");
            return true;
        }

        /// <summary>
        /// Updates references from the web.config file to external configuration files to reflect the local environment
        /// </summary>
        /// <param name="applicationFolder"></param>
        /// <param name="applicationConfigFolder"></param>
        /// <param name="status"></param>
        /// <returns></returns>
		private static bool UpdateWebConfig(string applicationFolder, string applicationConfigFolder, out string status)
		{
			status = string.Empty;

			if(!File.Exists(applicationFolder + "/web.config"))
			{
				status = "Unable to find web.config in specified application folder [" + applicationFolder + "].";
				return false;
			}

			XmlDocument webconfig = new XmlDocument();
            XmlNamespaceManager nsmgr;

			try
			{
				webconfig.Load(applicationFolder + "/web.config");
                nsmgr = new XmlNamespaceManager(webconfig.NameTable);
                nsmgr.AddNamespace("ms", "http://schemas.microsoft.com/.NetConfiguration/v2.0");
			}
			catch(Exception ex)
			{
				status = "Unable to load [" + applicationFolder + "/web.config].  Error was [" + ex.Message + "].";
				return false;
			}

			XmlNodeList configSections = webconfig.SelectNodes("/ms:configuration/ms:configSections/ms:section", nsmgr);

			foreach(XmlNode configSection in configSections)
			{
				string name = XmlUtility.GetAttributeText(configSection, "name");
				if(name != string.Empty)
				{
					XmlNode configNode = webconfig.SelectSingleNode("/ms:configuration/ms:" + name, nsmgr);

					if(configNode != null)
					{
						if(!UpdateFilePath(configNode, applicationConfigFolder, out status))
						{
							return false;
						}
					}
				}
			}

			webconfig.Save(applicationFolder + "/web.config");

			return true;
		}

        /// <summary>
        /// Update file path
        /// </summary>
        /// <param name="configNode"></param>
        /// <param name="applicationConfigFolder"></param>
        /// <param name="status"></param>
        /// <returns></returns>
		private static bool UpdateFilePath(XmlNode configNode, string applicationConfigFolder, out string status)
		{
			status = string.Empty;

			//Update the configNode
			XmlAttribute filePathAttribute = configNode.Attributes["filePath"];
			string filePath = filePathAttribute.Value;
			filePath = filePath.Replace('/', Path.DirectorySeparatorChar);

			if(filePath.LastIndexOf(Path.DirectorySeparatorChar) < (filePath.Length - 1))
			{
                string newFilePath = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                if (applicationConfigFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					filePathAttribute.Value = applicationConfigFolder + newFilePath;
				}
				else
				{
					filePathAttribute.Value = applicationConfigFolder + Path.DirectorySeparatorChar + newFilePath;
				}
			}
			return true;
		}
	}
}
