/****************************************************************************
 * Patch.cs												                    *
 * Container for information about available patches.					    *
 ****************************************************************************/
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;

using Prezza.Framework.Common;


namespace Checkbox.Configuration.Install
{
    /// <summary>
    /// Container class and getter for available patch information.
    /// </summary>
    [Serializable]
    public class PatchInfo : IComparable<PatchInfo>
    {
        private readonly string _patchfConfigurationXML;

        #region Static Methods

        /// <summary>
        /// Get a list of patches available in the specified path.
        /// </summary>
        /// <param name="rootSearchPath"></param>
        /// <returns></returns>
        public static List<PatchInfo> GetAvailablePatches(string rootSearchPath)
        {
            List<PatchInfo> availablePatches = new List<PatchInfo>();

            if (!string.IsNullOrEmpty(rootSearchPath))
            {
                string[] directories = Directory.GetDirectories(rootSearchPath  + Path.DirectorySeparatorChar + "Patches");

                foreach (string directory in directories)
                {
                    if (File.Exists(directory + Path.DirectorySeparatorChar + "PatchInfo.xml"))
                    {
                        XmlDocument doc = new XmlDocument();

                        try
                        {
                            doc.Load(directory + "\\PatchInfo.xml");
                            availablePatches.Add(new PatchInfo(doc));
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return availablePatches;
        }

        /// <summary>
        /// Get a patch info object with the specified name/description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal static PatchInfo GetPatchInfo(string name, string version)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<patchInfo><properties><name>" + name + "</name><version>" + version + "</version></properties></patchInfo>");

            return new PatchInfo(doc);
        }

        #endregion


        #region Instance Members

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="doc"></param>
        private PatchInfo(XmlDocument doc)
        {
            _patchfConfigurationXML = doc.OuterXml;
        }

        /// <summary>
        /// Get an XML document for patch info.
        /// </summary>
        private XmlDocument PatchInfoDoc
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_patchfConfigurationXML);

                return doc;
            }
        }

        #endregion

        #region Instance Properties

        //Get the name of the patch
        ///<summary>
        ///</summary>
        public string Name
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/properties/name")); }
        }

        /// <summary>
        /// Get the description of the patch
        /// </summary>
        public string Description
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/properties/description")); }
        }

        /// <summary>
        /// Get the setup URL for the patch
        /// </summary>
        public string SetupURL
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/properties/setupUrl")); }
        }

        /// <summary>
        /// Get the URL for the setup instructions
        /// </summary>
        public string SetupInstructionsUrl
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/properties/setupInstructionsUrl")); }
        }

        /// <summary>
        /// Get the list of items fixed for the patch
        /// </summary>
        public List<string> Fixes
        {
            get 
            { 
                List<string> fixes = new List<string>();

                foreach (XmlNode node in PatchInfoDoc.SelectNodes("/patchInfo/fixes/fix"))
                {
                    fixes.Add(node.InnerText);
                }
                
                return fixes; 
            }
        }

        /// <summary>
        /// Get the list of known issue for the patch
        /// </summary>
        public List<string> KnownIssues
        {
            get 
            { 
                List<string> issues = new List<string>();

                foreach (XmlNode node in PatchInfoDoc.SelectNodes("/patchInfo/knownIssues/knownIssue"))
                {
                    issues.Add(node.InnerText);
                }

                return issues; 
            }
        }

        /// <summary>
        /// Get the product prerequisite for this patch
        /// </summary>
        public string PreRequisiteProduct
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/preRequisites/product/name")); }
        }

        /// <summary>
        /// Get the version prerequisite for this patch
        /// </summary>
        public List<string> PreRequisiteProductVersions
        {
            get 
            {
                List<string> versions = new List<string>();

                XmlNodeList productVersionNodes = PatchInfoDoc.SelectNodes("/patchInfo/preRequisites/product/versions/version");

                foreach (XmlNode node in productVersionNodes)
                {
                    versions.Add(node.InnerText);
                }

                return versions;
            }
        }

        /// <summary>
        /// Get the product prerequisite for this patch
        /// </summary>
        public string PreRequisiteModule
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/preRequisites/module/name")); }
        }

        /// <summary>
        /// Get the version prerequisite for this patch
        /// </summary>
        public List<string> PreRequisiteModuleVersions
        {
             get 
            {
                List<string> versions = new List<string>();

                XmlNodeList productVersionNodes = PatchInfoDoc.SelectNodes("/patchInfo/preRequisites/module/versions/version");

                foreach (XmlNode node in productVersionNodes)
                {
                    versions.Add(node.InnerText);
                }

                return versions;
            }
        }

        /// <summary>
        /// Files associated with module installation
        /// </summary>
        public List<InstallFileInfo> GetFiles(string currentVersion)
        {
           
                List<InstallFileInfo> files = new List<InstallFileInfo>();

                XmlNodeList fileList = PatchInfoDoc.SelectNodes("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/files/file");

                foreach (XmlNode file in fileList)
                {
                    files.Add(new InstallFileInfo(XmlUtility.GetNodeText(file.SelectSingleNode("source")), XmlUtility.GetNodeText(file.SelectSingleNode("destination"))));
                }

                return files;
        }

        /// <summary>
        /// DB scripts to run
        /// </summary>
        public List<string> GetDatabaseScripts(string currentVersion)
        {
            //List scripts specific to current app version
            XmlNodeList scriptsList = PatchInfoDoc.SelectNodes("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/databaseScripts/databaseScript");

            var scripts = (from XmlNode script in scriptsList select script.InnerText).ToList();

            //List scripts for any app version
            scriptsList = PatchInfoDoc.SelectNodes("/patchInfo/patchDetails[@currentVersion='all']/databaseScripts/databaseScript");

            scripts.AddRange(from XmlNode script in scriptsList select script.InnerText);

            return scripts;
        }

        /// <summary>
        /// Version patch upgrades to
        /// </summary>
        public string Version
        {
            get { return XmlUtility.GetNodeText(PatchInfoDoc.SelectSingleNode("/patchInfo/properties/version")); }
        }

        /// <summary>
        /// Update WebChart graphs to Dundas charts
        /// </summary>
        public bool UpdateGraphs(string currentVersion)
        {
            return XmlUtility.GetNodeBool(PatchInfoDoc.SelectSingleNode("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/updateGraphs"));
        }

        /// <summary>
        /// Update Globalization configuration if necessary
        /// </summary>
        public bool UpdateGlobalizationConfig(string currentVersion)
        {
            return XmlUtility.GetNodeBool(PatchInfoDoc.SelectSingleNode("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/updateGlobalizationConfig"));
        }

        /// <summary>
        /// Install style templates if necessary
        /// </summary>
        public bool InstallStyles(string currentVersion)
        {
            return XmlUtility.GetNodeBool(PatchInfoDoc.SelectSingleNode("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/installStyles"));
        }

        /// <summary>
        /// Update Config/Security.xml file to remove DbProfileProvider
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public bool UpdateDbProfileSecurityConfig(string currentVersion)
        {
            return XmlUtility.GetNodeBool(PatchInfoDoc.SelectSingleNode("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/updateDbProfileSecurityConfig"));
        }

        /// <summary>
        /// Update Config/Security.xml file to include LDAPAuthenticationProvider
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public bool UpdateLdapSecurityConfig(string currentVersion)
        {
            return XmlUtility.GetNodeBool(PatchInfoDoc.SelectSingleNode("/patchInfo/patchDetails[@currentVersion='" + currentVersion + "']/updateLdapSecurityConfig"));
        }

        #endregion

        #region IComparable<PatchInfo> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PatchInfo other)
        {
            return string.Compare(other.Name, this.Name, true);
        }

        #endregion
    }
}
