/****************************************************************************
 * ModuleInfo.cs												            *
 * Container for information about available modules.					    *
 ****************************************************************************/
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Checkbox.Configuration.Install
{
    /// <summary>
    /// Container and getter for available modules
    /// </summary>
    [Serializable]
    public class ModuleInfo : IComparable<ModuleInfo>, IComparable<string>
    {
        private readonly string _moduleConfigurationXML;

        #region Static Methods
        /// <summary>
        /// Static Get of Available Modules
        /// </summary>
        /// <param name="rootSearchPath"></param>
        /// <returns></returns>
        internal static List<ModuleInfo> GetAvailableModules(string rootSearchPath)
        {
            List<ModuleInfo> availableModules = new List<ModuleInfo>();

            if (!string.IsNullOrEmpty(rootSearchPath))
            {
                if (Directory.Exists(rootSearchPath + Path.DirectorySeparatorChar + "Modules"))
                {
                    string[] directories = Directory.GetDirectories(rootSearchPath + Path.DirectorySeparatorChar + "Modules");

                    foreach (string directory in directories)
                    {
                        if (File.Exists(directory + Path.DirectorySeparatorChar + "ModuleInfo.xml"))
                        {
                            XmlDocument doc = new XmlDocument();

                            try
                            {
                                doc.Load(directory + Path.DirectorySeparatorChar + "ModuleInfo.xml");
                                availableModules.Add(new ModuleInfo(doc));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            return availableModules;
        }

        /// <summary>
        /// Get an instance of a module info based on the name/version passed
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="moduleVersion"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        internal static ModuleInfo GetModuleInfo(string moduleName, string moduleVersion, string searchPath)
        {
            List<ModuleInfo> modules = GetAvailableModules(searchPath + Path.DirectorySeparatorChar + "Install");

            foreach (ModuleInfo module in modules)
            {
                if (string.Compare(moduleName, module.Name, true) == 0 && string.Compare(moduleVersion, module.Version, true) == 0)
                {
                    return module;
                }
            }

            //If the module info can't be found, make a dummy object for the cases where the module was installed, but the files
            // were removed.
            StringBuilder sb = new StringBuilder();
            sb.Append("<moduleInfo><properties><name>");
            sb.Append(moduleName);
            sb.Append("</name><version>");
            sb.Append(moduleVersion);
            sb.Append("</version></properties></moduleInfo>");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            return new ModuleInfo(doc);
        }
        #endregion

        /// <summary>
        /// Constructor of a module info object
        /// </summary>
        /// <param name="doc"></param>
        private ModuleInfo(XmlDocument doc)
        {
            _moduleConfigurationXML = doc.OuterXml;
        }

        /// <summary>
        /// Get the module name
        /// </summary>
        public string Name
        {
            get
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);
                //return XmlUtility.GetNodeText(moduleConfiguration.SelectSingleNode("/moduleInfo/properties/name"));
                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/properties/name"));
            }
        }

        /// <summary>
        /// Get a short description of the module
        /// </summary>
        public string Description
        {
            get
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);
                //return XmlUtility.GetNodeText(moduleConfiguration.SelectSingleNode("/moduleInfo/properties/description"));
                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/properties/description"));
            }
        }

        /// <summary>
        /// Get the version of the module
        /// </summary>
        public string Version
        {
            get
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);
                //return XmlUtility.GetNodeText(moduleConfiguration.SelectSingleNode("/moduleInfo/properties/version"));
                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/properties/version"));
            }
        }

        /// <summary>
        /// Get a list of features of the installed module
        /// </summary>
        public List<string> Features
        {
            get
            {
                List<string> features = new List<string>();

                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                //XmlNodeList featuresList = moduleConfiguration.SelectNodes("/moduleInfo/features/feature");
                XmlNodeList featuresList = moduleDoc.SelectNodes("/moduleInfo/features/feature");

                foreach (XmlNode feature in featuresList)
                {
                    features.Add(feature.InnerText);
                }

                return features;
            }
        }

        /// <summary>
        /// Get the URL to use as the setup URL
        /// </summary>
        public string SetupURL
        {
            get 
            { 
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                //return XmlUtility.GetNodeText(moduleConfiguration.SelectSingleNode("/moduleInfo/properties/setupUrl")); 
                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/properties/setupUrl")); 
            }
        }

        /// <summary>
        /// Get the URL for the setup instructions
        /// </summary>
        public string SetupInstructionsUrl
        {
            get
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/properties/setupInstructionsUrl"));
            }
        }

        /// <summary>
        /// Files associated with module installation
        /// </summary>
        public List<InstallFileInfo> Files
        {
            get
            {
                List<InstallFileInfo> files = new List<InstallFileInfo>();
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                //XmlNodeList fileList = moduleConfiguration.SelectNodes("/moduleInfo/files/file");
                XmlNodeList fileList = moduleDoc.SelectNodes("/moduleInfo/files/file");

                foreach (XmlNode file in fileList)
                {
                    files.Add(new InstallFileInfo(XmlUtility.GetNodeText(file.SelectSingleNode("source")), XmlUtility.GetNodeText(file.SelectSingleNode("destination"))));
                }

                return files;
            }
        }

        /// <summary>
        /// DB scripts to run
        /// </summary>
        public List<string> DatabaseScripts
        {
            get
            {
                List<string> scripts = new List<string>();

                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);
                //XmlNodeList scriptsList = moduleConfiguration.SelectNodes("/moduleInfo/databaseScripts/databaseScript");
                XmlNodeList scriptsList = moduleDoc.SelectNodes("/moduleInfo/databaseScripts/databaseScript");

                foreach (XmlNode script in scriptsList)
                {
                    scripts.Add(script.InnerText);
                }

                return scripts;
            }
        }

        /// <summary>
        /// Get the product prerequisite for this module
        /// </summary>
        public string PreRequisiteProduct
        {
            get 
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                //return XmlUtility.GetNodeText(moduleConfiguration.SelectSingleNode("/moduleInfo/preRequisites/product/name")); 
                return XmlUtility.GetNodeText(moduleDoc.SelectSingleNode("/moduleInfo/preRequisites/product/name")); 
            }
        }

        /// <summary>
        /// Get the version prerequisite for this module
        /// </summary>
        public List<string> PreRequisiteProductVersions
        {
            get 
            {
                XmlDocument moduleDoc = new XmlDocument();
                moduleDoc.LoadXml(_moduleConfigurationXML);

                XmlNodeList moduleVersionNodes = moduleDoc.SelectNodes("/moduleInfo/preRequisites/product/versions/version");
                List<string> versions = new List<string>();

                foreach (XmlNode node in moduleVersionNodes)
                {
                    versions.Add(node.InnerText);
                }

                return versions;
            }
        }



        #region IComparable<ModuleInfo> Members

        /// <summary>
        /// Compare
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ModuleInfo other)
        {
            return CompareTo(other.Name);
        }

        #endregion

        #region IComparable<string> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(string other)
        {
            return string.Compare(other, Name, true);
        }

        #endregion
    }
}
