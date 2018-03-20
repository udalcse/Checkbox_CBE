//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Remoting;

using Prezza.Framework.Common;

namespace Checkbox.Configuration
{
	/// <summary>
	/// Validator for checkbox framework and application configuration files.
	/// </summary>
	public class ConfigurationValidator
	{
		private ConfigurationValidator()
		{
		}

		/// <summary>
		/// Validate all checkbox XML configuration files.  Only files currently part of the active configuration will
		/// be validated, i.e. only files referenced by the web.config or other configuration files will be validated.
		/// </summary>
        /// <param name="appRoot">Path to the application web.config</param>
		/// <param name="status">String that will contain status messages.</param>
		/// <returns>True of the configuration is successfully validated, false otherwise.</returns>
		/// <remarks>The output status string will provide detail for any success or failures.</remarks>
		public static bool ValidateConfiguration(string appRoot, out string status)
		{
			StringBuilder sb = new StringBuilder();
            
			bool success = true;

			try
			{
                ArgumentValidation.CheckForNullReference(appRoot, "webconfigPath");
			
				//Step 1:  Load the web.config and search for the checkbox application configuration sets.
				XmlDocument webconfig = new XmlDocument();
                XmlNamespaceManager ns;


                string webconfigPath = appRoot + "/web.config";

				if(!File.Exists(webconfigPath))
				{
					status = "Specified web.config path [" + webconfigPath + "] does not exist.";
					return false;
				}

				//Load the XML
				try
				{
					webconfig.Load(webconfigPath);
                    ns = new XmlNamespaceManager(webconfig.NameTable);
                    ns.AddNamespace("ms", "http://schemas.microsoft.com/.NetConfiguration/v2.0");
				}
				catch(Exception ex)
				{
					status = "Unable to load web.config. Error was [" + ex.Message + "].";
					return false;
				}

                //Check connection strings
                if(ConfigurationManager.ConnectionStrings.Count == 0)
                {
                    status = "No connection strings found in web.config";
                    return false;
                }

                //Check default & workflow persistence strings
                if(ConfigurationManager.ConnectionStrings["DefaultConnectionString"] == null
                    || string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString))
                {
                    status = "DefaultConnectionString is not present or has no value.";
                    return false;
                }

				//Get the config sections
				XmlNodeList configSections = webconfig.SelectNodes("/ms:configuration/ms:configSections/ms:section", ns);

				foreach(XmlNode configSection in configSections)
				{
					bool sectionSuccess = true;

					//Verify the name and type nodes are populated
					string name = XmlUtility.GetAttributeText(configSection, "name");

					if(string.IsNullOrEmpty(name))
					{
						sb.Append(Environment.NewLine + "FAILED -- Found a configSection in the web.config with no name attribute.");
						sectionSuccess = false;
						name = string.Empty;
					}

					string type = XmlUtility.GetAttributeText(configSection, "type");

					if(string.IsNullOrEmpty(type))
					{
						sb.Append(Environment.NewLine + "FAILED -- Found a configSection in the web.config with no type attribute.  ConfigSection name was [" + name + "].");
						sectionSuccess = false;
					}
					//Verify the section handler can be loaded
					else
					{
						string loadStatus;

						if(!LoadClass(type, out loadStatus))
						{
							sb.Append(Environment.NewLine + "FAILED -- Unable to load section handler for configSection.  Name was [" + name + "] and type was [" + type + "].  Load status was [" + loadStatus + "].");
							sectionSuccess = false;
						}
					}

					//Now check the config sections themselves
					if(name != string.Empty)
					{
						XmlNode configData = webconfig.SelectSingleNode("/ms:configuration/ms:" + name, ns);

						if(configData == null)
						{
							sb.Append(Environment.NewLine + "FAILED -- Unable to find named configuration section in web.confg.  Name was [" + name + "].");
							sectionSuccess = false;
						}
						else
						{
							//Get the file name and the configDataType
							string filePath = XmlUtility.GetAttributeText(configData, "filePath");
							string configDataType = XmlUtility.GetAttributeText(configData, "configDataType");
							
							//Validate the path
							if(string.IsNullOrEmpty(filePath))
							{
								sb.Append(Environment.NewLine + "FAILED -- The configuration path for the section [" + name + "] was not specified.");
								sectionSuccess = false;
							}
							else
							{
								if(string.IsNullOrEmpty(configDataType))
								{
									sb.Append(Environment.NewLine + "FAILED -- The configuration data type for the section [" + name + "] was not specified.");
									sectionSuccess = false;
								}
								else
								{
									//Validate the configuration for this file
									string fileStatus;

									if(!ValidateFile(appRoot, filePath, ns, out fileStatus))
									{
										sb.Append(fileStatus);
										sectionSuccess = false;
									}
								}
							}
						}
					}

					if(!sectionSuccess)
					{
						sb.Append(Environment.NewLine + "FAILED -- Failed to validate configuration for section [" + name + "].");
					}

					success = success && sectionSuccess;
				}
			}
			catch(Exception ex)
			{
				sb.Append("An unhandled exception occurred while validating the application configuration. [" + ex.Message + "].");

				status = sb.ToString();

				return false;
			}

			status = sb.ToString();
			return success;
		}

		/// <summary>
		/// Attempt to create an instance of the specified type.
		/// </summary>
		/// <param name="typeName">Fully qualified type name for the type.</param>
		/// <param name="loadStatus">Status message populated on error.</param>
		/// <returns>True on success, false on failure.</returns>
		private static bool LoadClass(string typeName, out string loadStatus)
		{
			if(typeName.IndexOf(",") < 0)
			{
				loadStatus = "Invalid type name.";
				return false;
			}
			
			string className = typeName.Split(',')[0];
			string assemblyName = typeName.Split(',')[1];

			if(string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
			{
				loadStatus = "Invalid type name.";
				return false;
			}

			ObjectHandle handle;
			
			try
			{
				handle = Activator.CreateInstance(assemblyName, className);
			}
			catch(MissingMethodException)
			{
				//This ok, assembly was found, just a default constructor was not. 
				loadStatus = string.Empty;
				return true;
			}
			catch(Exception ex)
			{
				loadStatus = string.Format("{0}  {1}", ex.GetType(), ex.Message);
				return false;
			}

			if(handle == null)
			{
				loadStatus = "CreateInstance failed.";
				return false;
			}

			loadStatus = string.Empty;
			return true;
		}


		/// <summary>
		/// Validte the configuration of a specific file.
		/// </summary>
		/// <param name="appRoot"></param>
		/// <param name="filePath">Path to the configuration file.</param>
		/// <param name="fileStatus">Status of the operation.</param>
        /// <param name="ns">XmlNamespace Manager</param>
		/// <returns>True on success, false on failure.</returns>
		private static bool ValidateFile(string appRoot, string filePath, XmlNamespaceManager ns, out string fileStatus)
		{
			fileStatus = string.Empty;

            if (!filePath.Contains("\\") && !filePath.Contains("/"))
            {
                filePath = appRoot + "/config/" + filePath;
            }

			if(!File.Exists(filePath))
			{
				fileStatus = Environment.NewLine + "FAILED -- Specified file path [" + filePath + "] does not exist.";
				return false;
			}

			//Load the file as xml
			XmlDocument configFile = new XmlDocument();

			try
			{
				configFile.Load(filePath);
			}
			catch(Exception ex)
			{
				fileStatus = Environment.NewLine + "FAILED -- Unable to load [" + filePath + "].  Exception was [" + ex.Message + "]";
				return false;
			}

			//Validate that no nodes have filePath or configDataType with incorrect case
			XmlNodeList allNodes = configFile.SelectNodes("//ms:*", ns);

			bool success = true;

			foreach(XmlNode node in allNodes)
			{
			    bool hasConfigDataType = false;

				//Check each attribute
				foreach(XmlAttribute attr in node.Attributes)
				{
                    bool hasFilePath = false;

				    if(string.Compare(attr.Name, "filePath", true) == 0)
					{
						hasFilePath = true;

						if(string.Compare(attr.Name, "filePath", false) != 0)
						{
							fileStatus += Environment.NewLine + "FAILED -- filePath attribute in node [" + node.Name + "] is the wrong case.  It is [" + attr.Name + "] when it should be [filePath]";
							success = false;
						}
					} 
					
					if(string.Compare(attr.Name, "configDataType", true) == 0)
					{
						if(string.Compare(attr.Name, "configDataType", false) != 0)
						{
							fileStatus += Environment.NewLine + "FAILED -- configDataType attribute in node [" + node.Name + "] is the wrong case.  It is [" + attr.Name + "] when it should be [configDataType]";
							success = false;
						}
					}
					else
					{
						hasConfigDataType = true;
					}

					//Make sure that if a node has one attribute, it has both
					if(hasFilePath && !hasConfigDataType)
					{
						fileStatus += Environment.NewLine + "FAILED -- Node [" + node.Name + "] has a filePath attribute but no configDataType attribute.";
						success = false;
					}
					else if(!hasFilePath && hasConfigDataType)
					{
						fileStatus += Environment.NewLine + "FAILED -- Node [" + node.Name + "] has a configDataType attribute but no filePath attribute.";
						success = false;
					}
				}
			}

			//Validate assemblies and files
			XmlNodeList filePathList = configFile.SelectNodes("//ms:*[@filePath]", ns);

			foreach(XmlNode node in filePathList)
			{
				string configDataType = XmlUtility.GetAttributeText(node, "configDataType");

				//Ignore if there is no configdatatype, since it will have been caught above.
				if(!string.IsNullOrEmpty(configDataType))
				{
					string classStatus;
					
					if(!LoadClass(configDataType, out classStatus))
					{
						fileStatus += Environment.NewLine + "FAILED -- [" + filePath + "] Unable to load class.  Load status was [" + classStatus + "].";
						success = false;
					}
				}

				string file = XmlUtility.GetAttributeText(node, "filePath");

				if(!string.IsNullOrEmpty(file))
				{
					string subFileStatus;

					if(!ValidateFile(appRoot, file, ns, out subFileStatus))
					{
						fileStatus += Environment.NewLine + subFileStatus;
						success = false;
					}
				}
			}

			return success;
		}
	}
}
