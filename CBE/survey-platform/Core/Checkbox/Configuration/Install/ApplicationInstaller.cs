/****************************************************************************
 * ApplicationInstaller.cs												    *
 * Support class for installing Checkbox Application.					    *
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

using Checkbox.Management;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

//using MySql.Data.MySqlClient;
//using Npgsql;
//using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using Checkbox.Progress;
using Prezza.Framework.Caching;

//using IBM.Data.DB2;


namespace Checkbox.Configuration.Install
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class ApplicationInstaller
    {
        private const string APP_INSTALLER_CACHE_KEY = "_cachedAppInstaller";

        /// <summary>
        /// Version to install information
        /// </summary>
        private readonly string _rootFolder;
        private readonly bool _newInstall;
        private string _version;
        private List<ModuleInfo> _allModules;
        private List<PatchInfo> _allPatches;
        private List<string> _dbInstallScripts;
        private List<InstallFileInfo> _installFiles;
        private string _installAdminScript;
        private List<string> _mailDBInstallScripts;
        private List<InstallFileInfo> _mailDBInstallFiles;

        private string _applicationUrl;

        /// <summary>
        /// Currently installed version information
        /// </summary>
        private string _installedVersion;

        #region Constructor
        /// <summary>
        /// Default constructor, automatically determines whether this is a new install or upgrade
        /// </summary>
        /// <param name="rootFolder"></param>
        public ApplicationInstaller(string rootFolder) : this(rootFolder, false, null) {}

        /// <summary>
        /// Construct a new application installer
        /// </summary>
        /// <param name="root"></param>
        /// <param name="forceNew"></param>
        /// <param name="provider"></param>
        public ApplicationInstaller(string root, bool forceNew, string provider)
        {
            if (string.IsNullOrEmpty(root))
            {
                throw new Exception("Application Root was not specified.");
            }

            InstallType = "install";
            _rootFolder = root;
            DatabaseProvider = provider;

            //Determine if this is a new install...check web.config for install success flag
            if (forceNew || !ApplicationManager.AppSettings.InstallSuccess)
                _newInstall = true;
            else
                _newInstall = false;

            LoadExistingInstallInformation();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string InstallType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string RootFolder
        {
            get { return _rootFolder; }
        }

        ///<summary>
        ///</summary>
        public string DatabaseProvider { get; set; }

        /// <summary>
        /// Load information about the current installation (if any)
        /// </summary>
        protected void LoadExistingInstallInformation()
        {
            //Get the list of all modules & patches
            _allModules = ModuleInfo.GetAvailableModules(_rootFolder + Path.DirectorySeparatorChar + "Install");
            _allPatches = PatchInfo.GetAvailablePatches(_rootFolder + Path.DirectorySeparatorChar + "Install");

            InstalledModules = new List<ModuleInfo>();
            InstalledPatches = new List<PatchInfo>();

            if (!_newInstall)
            {
                //If not a new install, attempt to get more information about the current installation
                try
                {
                    var db = DatabaseFactory.CreateDatabase();

                    var command = new SqlCommand()
                        {
                            Connection = (SqlConnection) db.GetConnection(),
                            CommandText = string.Format(
                                "SELECT TOP 1 ProductID, ProductName, Version, InstallDate FROM ckbx_Product_Info WHERE ProductName = '{0}' ORDER BY InstallDate DESC",
                                Name),
                            CommandType = CommandType.Text
                        };

                    command.Connection.Open();
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            if (reader.Read())
                            {
                                _installedVersion = (string) reader["Version"];
                                Convert.ToDateTime(reader["InstallDate"]);
                                InstalledProductID = Convert.ToInt32(reader["ProductID"]);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPolicy.HandleException(ex, "BusinessPublic");
                        }
                        finally
                        {
                            reader.Close();

                            command.Connection.Close();
                        }
                    }
                    //Get installed modules
                    command = new SqlCommand()
                        {
                            Connection = (SqlConnection) db.GetConnection(),
                            CommandText =
                                string.Format(
                                    "SELECT ProductID, ModuleID, ModuleName, Version, InstallDate FROM ckbx_Product_Modules WHERE ProductID = {0}",
                                    InstalledProductID),
                            CommandType = CommandType.Text
                        };
                    command.Connection.Open();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                InstalledModules.Add(ModuleInfo.GetModuleInfo((string) reader["ModuleName"],
                                                                              (string) reader["Version"], _rootFolder));
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPolicy.HandleException(ex, "BusinessPublic");
                        }
                        finally
                        {
                            reader.Close();
                            command.Connection.Close();
                        }
                    }

                    command = new SqlCommand()
                        {
                            Connection = (SqlConnection) db.GetConnection(),
                            CommandText =
                                string.Format(
                                    "SELECT p.ProductID, m.ModuleID, p.PatchName, p.Version, p.InstallDate FROM ckbx_Product_Patches p LEFT OUTER JOIN ckbx_Product_Modules m on m.ModuleID = p.ModuleID WHERE p.ProductID = {0} ORDER BY p.Version",
                                    InstalledProductID),
                            CommandType = CommandType.Text
                        };
                    command.Connection.Open();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                InstalledPatches.Add(PatchInfo.GetPatchInfo((string) reader["PatchName"],
                                                                            (string) reader["Version"]));
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPolicy.HandleException(ex, "BusinessPublic");
                        }
                        finally
                        {
                            reader.Close();
                            command.Connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    //If upgrading, don't throw an exception since it's possible that the data is not there
                    if (InstallType != "upgrade")
                    {
                        throw new Exception(
                            "An error occurred while attempting to get information about the current installation: " +
                            ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Load information from the install files
        /// </summary>
        public virtual void LoadInstallFilesAndScripts()
        {
            LoadFilesAndScriptsByType(InstallType, ref _dbInstallScripts, ref _installFiles);
        }

        /// <summary>
        /// Load files by type
        /// </summary>
        /// <param name="installType">Type of the files. Describes node name that contains a group of scripts</param>
        /// <param name="dbInstallScripts"></param>
        /// <param name="installFiles"></param>
        /// <returns></returns>
        private List<string> LoadFilesAndScriptsByType(string installType, ref List<string> dbInstallScripts, ref List<InstallFileInfo> installFiles)
        {
            //Attempt to get the information for this product
            var doc = new XmlDocument();
            doc.Load(_rootFolder + Path.DirectorySeparatorChar + "Install" + Path.DirectorySeparatorChar + "ProductInfo.xml");

            _version = XmlUtility.GetNodeText(doc.SelectSingleNode("/productInfo/properties/version"), true);

            //Get the list of files associated with this install
            installFiles = new List<InstallFileInfo>();

            var filesList = doc.SelectNodes("/productInfo/" + installType + "/files/file");

            foreach (XmlNode fileNode in filesList)
            {
                var source = XmlUtility.GetNodeText(fileNode.SelectSingleNode("source"));
                var destination = XmlUtility.GetNodeText(fileNode.SelectSingleNode("destination"));

                if (source.Trim() != string.Empty && destination.Trim() != string.Empty)
                {
                    var fileInfo = new InstallFileInfo(
                        _rootFolder + Path.DirectorySeparatorChar + source,
                        _rootFolder + Path.DirectorySeparatorChar + destination);

                    installFiles.Add(fileInfo);
                }
            }

            //Get the list of scripts associated with this install
            dbInstallScripts = new List<string>();

            var scriptsList = doc.SelectNodes("/productInfo/" + installType + "/dbScripts[@dbProvider='" + DatabaseProvider + "']/dbScript");

            foreach (XmlNode scriptNode in scriptsList)
            {
                var script = XmlUtility.GetNodeText(scriptNode);
                script = _rootFolder + Path.DirectorySeparatorChar + script.Replace('/', Path.DirectorySeparatorChar);

                if (XmlUtility.GetAttributeBool(scriptNode, "admin"))
                    _installAdminScript = script;
                else
                    dbInstallScripts.Add(script);
            }
            return dbInstallScripts;
        }

        #region Public Methods
        
        /// <summary>
        /// Return a boolean indicating if checkbox was upgraded from ultimate survey.
        /// </summary>
        /// <returns></returns>
        public static bool WasUpgradedFromUltimateSurvey3()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Install_WasUpgradedFromUS");
            command.AddOutParameter("WasUpgraded", DbType.Boolean, 4);

            db.ExecuteNonQuery(command);

            var outValue = command.GetParameterValue("WasUpgraded");

            if (outValue != DBNull.Value)
            {
                return (bool)outValue;
            }

            return false;
        }

        /// <summary>
        /// Get the information for the installed module information
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static bool IsModuleInstalled(string productName, string moduleName)
        {
            var returnValue = false;

            //If not a new install, attempt to get more information about the current installation
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Product_GetInfo");
            command.AddInParameter("ProductName", DbType.String, productName);

            var productID = -1;

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        productID = Convert.ToInt32(reader["ProductID"]);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
                finally
                {
                    reader.Close();
                }
            }

            //Get installed modules
            command = db.GetStoredProcCommandWrapper("ckbx_sp_Product_GetModules");
            command.AddInParameter("ProductID", DbType.Int32, productID);
            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (string.Compare((string)reader["ModuleName"], moduleName, true) == 0)
                        {
                            returnValue = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                }
                finally
                {
                    reader.Close();
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Perform the database portion of the product install
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SetupDatabase(out string status)
        {
            status = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(InstallConnectionString))
                {
                    status = "No connection string was specified.";
                    return false;
                }

                if (!ValidateAndRunScripts(InstallConnectionString, _dbInstallScripts, out status))
                {
                    return false;
                }

                if (_installAdminScript != null)
                {
                    //add admin user
                    if (!AddDefaultAdminUser(_installAdminScript, AdminUserName, AdminPassword, out status))
                    {
                        status = string.Format("[{0}]. {1}", _installAdminScript, status);

                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(EmailDBConnectionString))
                {
                    if (_mailDBInstallScripts != null)
                    {
                        if (!ValidateAndRunScripts(EmailDBConnectionString, _mailDBInstallScripts, out status))
                        {
                            return false;
                        }
                    }
                }

                //save predefined settings
                if (!SaveSettings(out status))
                {
                    status = string.Format("Failed to save settings. {1}", status);

                    return false;
                }
                
            }
            catch (Exception ex)
            {
                status = "An error occurred while setting up the database.  The error was:  " + ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates and runs all scripts using the given connection string
        /// </summary>
        /// <param name="dbInstallScripts"></param>
        /// <param name="status"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private bool ValidateAndRunScripts(string connectionString, List<string> dbInstallScripts, out string status)
        {
            status = "";

            ProgressProvider.SetProgress(
                ProgressKey,
                "Validating installation scripts.",
                string.Empty,
                ProgressStatus.Pending,
                10,
                100);

            //Validate that scripts exist before running them
            foreach (var sqlScript in dbInstallScripts)
            {
                if (!File.Exists(sqlScript))
                {
                    status = "A required database installation script was not found:  " + sqlScript;
                    return false;
                }
            }

            ProgressProvider.SetProgress(
                ProgressKey,
                "Validating installation scripts.",
                string.Empty,
                ProgressStatus.Pending,
                20,
                100);


            var data = ProgressProvider.GetProgress(ProgressKey);

            var itemsPerScript = data.TotalItemCount == 0 || dbInstallScripts.Count  == 0 ? 1 : ((data.TotalItemCount - data.CurrentItem) / (double)dbInstallScripts.Count);
            var scriptsDone = 0;
                
            //install scripts
            foreach (var sqlScript in dbInstallScripts)
            {
                var sqlRunStatus = RunDBScriptFile(connectionString, sqlScript, out status);

                ProgressProvider.SetProgress(
                    ProgressKey,
                    "Running database installation scripts.",
                    string.Empty,
                    ProgressStatus.Pending,
                    (int)(data.CurrentItem + itemsPerScript * scriptsDone++),
                    data.TotalItemCount);

                if (!sqlRunStatus)
                {
                    status = string.Format("[{0}]. {1}", sqlScript, status);

                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool SaveSettings(out string status)
        {
            status = "";
            try
            {
                foreach (var key in _dbSettings.Keys)
                {
                    var SQL = string.Format("exec ckbx_sp_Setting_Delete '{0}'; exec ckbx_sp_Setting_Insert '{0}', '{1}';",
                        key.Replace("'", ""), _dbSettings[key].Replace("'", "''"));
                    ExecuteSqlString(InstallConnectionString, SQL);
                }
            }
            catch (Exception ex)
            {
                status = ex.Message;

            }
            return true;
        }

        /// <summary>
        /// Dictionary with all settings that should be set to the database
        /// </summary>
        private Dictionary<string, string> _dbSettings = new Dictionary<string, string>();

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddSetting(string key, string value)
        {
            if (_dbSettings.ContainsKey(key))
            {
                _dbSettings[key] = value;
            }
            else
                _dbSettings.Add(key, value);
        }

        /// <summary>
        /// Read all blocks from sql file
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <returns></returns>
        private static IEnumerable<string> ReadScriptBlocks(string sqlScript)
        {
            var result = new List<string>();

            var sr = new StreamReader(sqlScript);
            var sb = new StringBuilder();
            try
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line != null)
                    {
                        if (string.Compare(line.Trim(), "GO", true) == 0)
                        {
                            result.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                }

                //If EOF reached, run the last line
                result.Add(sb.ToString());

                return result;
            }
            finally
            {
                sr.Close();
            }
        }

        /// <summary>
        /// Run the specified script file
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <param name="status"></param>
        public virtual bool RunDBScriptFile(string connectionString, string sqlScript, out string status)
        {
            try
            {
                var currentScript = string.Empty;

                if (!File.Exists(sqlScript))
                {
                    status = "A required database installation script was not found:  " + sqlScript;
                    return false;
                }

                try
                {
                    foreach (var scriptBlock in ReadScriptBlocks(sqlScript))
                    {
                        currentScript = scriptBlock;
                        ExecuteSqlString(connectionString, scriptBlock);
                    }

                    status = "Success.";
                    return true;
                }
                catch (SqlException ex)
                {
                    status = string.Format("An error occurred on line {0}, while setting up the database.  The error was:  {1} [[[ {2} ]]].", ex.LineNumber, ex.Message, currentScript);
                    return false;
                }
                catch (Exception ex)
                {
                    status = "An error occurred while setting up the database.  The error was:  " + ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                status = "An error occurred while setting up the database.  The error was:  " + ex.Message;
#if DEBUG
                Console.WriteLine(ex.ToString());
                Console.WriteLine(sqlScript);
#endif
                return false;
            }
        }

        /// <summary>
        /// Execute the specified SQL string
        /// </summary>
        /// <param name="sql"></param>
        private void ExecuteSqlString(string connectionString, string sql)
        {
            if (sql == null || sql.Trim() == string.Empty)
            {
                return;
            }

            // Set up the connection and sql command based on the database provider selected
            IDbConnection connection;
            IDbCommand command;
            switch (DatabaseProvider)
            {
                    /*
                case "mysql":
                    connection = new MySqlConnection(_connectionString);
                    command = new MySqlCommand(sql, (MySqlConnection)connection);
                    break;
                case "oracle":
                    connection = new OracleConnection(_connectionString);
                    command = new OracleCommand(sql, (OracleConnection)connection);
                    break;
                case "npgsql":
                    connection = new NpgsqlConnection(_connectionString);
                    command = new NpgsqlCommand(sql, (NpgsqlConnection)connection);
                    break;
                case "db2":
                    connection = new DB2Connection(_connectionString);
                    command = new DB2Command(sql, (DB2Connection)connection);
                    break; */
                default:
                    connection = new SqlConnection(connectionString);
                    command = new SqlCommand(sql, (SqlConnection)connection);
                    break;
            }

            // Execute the sql script
            try
            {
                connection.Open();
                command.CommandTimeout = 0;
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }


        /// <summary>
        /// Update the application configuration files.
        /// </summary>
        /// <param name="status">Status of the operation.</param>
        /// <returns>Boolean indicating if the operation was successful.</returns>
        public bool UpdateConfigurationFiles(out string status)
        {
            try
            {
                //Update config. files
                var errorStatus = ConfigurationWriter.UpdateConfigFiles(_rootFolder, _rootFolder + Path.DirectorySeparatorChar + "Config", AppConnectionString, DatabaseProvider, out status);

                //Only if the config files were updated without error update web.config
                //This is to prevent the user from getting stuck in a state where they can
                //no longer run the install but have partially configured files.
                if (errorStatus)
                {
                    //Update web.config settings
                    UpdateWebConfigSetting("ApplicationRoot", ApplicationRoot);
                    UpdateWebConfigSetting("ApplicationURL", _applicationUrl);
                    UpdateWebConfigConnectionStringSetting("DefaultConnectionString", AppConnectionString);
                    UpdateWebConfigConnectionStringSetting("MailDbConnectionString", EmailDBConnectionString);
                    UpdateWebConfigServiceHostUrls();
                    UpdateWebConfigServiceClientUrls();
                }
                return errorStatus;
            }
            catch (Exception ex)
            {
                status = "An error occurred while updating the configuration files:  " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Update the application configuration files.
        /// </summary>
        /// <param name="status">Status of the operation.</param>
        /// <returns>Boolean indicating if the operation was successful.</returns>
        public bool MarkInstallSuccess(out string status)
        {
            status = string.Empty;
            try
            {
                //Update web.config settings
                UpdateWebConfigSetting("InstallSuccess", "true");             
            }
            catch (Exception ex)
            {
                status = "An error occurred while setting installation success flag:  " + ex.Message;
                return false;
            }

            InstallSuccess = true;

            return true;
        }

        /// <summary>
        /// Get install success
        /// </summary>
        public bool InstallSuccess
        {
            get
            {
                bool? success = HttpContext.Current.Session["InstallSuccessKey"] as bool?;
                return success.HasValue && success.Value;
            }
            set { HttpContext.Current.Session["InstallSuccessKey"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateWebConfig(out string status)
        {
            status = String.Empty;
            try
            {
                UpdateWebConfigSetting("ApplicationRoot", ApplicationRoot);
                UpdateWebConfigSetting("ApplicationURL", _applicationUrl);
                UpdateWebConfigConnectionStringSetting("DefaultConnectionString", InstallConnectionString);
                if (!string.IsNullOrEmpty(EmailDBConnectionString))
                    UpdateWebConfigConnectionStringSetting("MailDbConnectionString", EmailDBConnectionString);

                UpdateWebConfigServiceHostUrls();
                //UpdateWebConfigServiceClientUrls();
                
                return true;
            }
            catch (Exception ex)
            {
                status = "An error occurred while updating the configuration files:  " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Update the web.config
        /// </summary>
        public void UpdateWebConfigSetting(string settingName, string settingValue)
        {
            if (settingName == null || settingName.Trim() == string.Empty)
            {
                throw new Exception("No setting name was specified to update.");
            }

            var webconfig = new XmlDocument();
            XmlNamespaceManager ns;

            var webconfigPath = _rootFolder + Path.DirectorySeparatorChar + "web.config";

            if (!File.Exists(webconfigPath))
            {
                throw new Exception("Unable to open web.config file: " + webconfigPath);
            }

            //Load the XML
            try
            {
                webconfig.Load(webconfigPath);
                ns = new XmlNamespaceManager(webconfig.NameTable);
                ns.AddNamespace("ms", "http://schemas.microsoft.com/.NetConfiguration/v2.0");

                var appSettingsNode = webconfig.SelectSingleNode("/ms:configuration/ms:appSettings", ns);

                if (appSettingsNode == null)
                {
                    throw new Exception("Unable to load appSettings node from web.config " + webconfigPath);
                }

                var settingNode = appSettingsNode.SelectSingleNode("ms:add[@key='" + settingName + "']", ns);

                //If the node exists, update it otherwise add it
                if (settingNode == null)
                {
                    throw new Exception("AppSetting key " + settingName + " was not found in the web.config " + webconfigPath);
                }
                
                var valueAttr = webconfig.CreateAttribute("value");
                valueAttr.Value = settingValue;
                settingNode.Attributes.SetNamedItem(valueAttr);

                webconfig.Save(webconfigPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update web.config at " + webconfigPath + ". Error was [" + ex.Message + "].");
            }
        }

        /// <summary>
        /// Update the web.config connection string with the specified name
        /// </summary>
        public void UpdateWebConfigConnectionStringSetting(string connectionStringName, string settingValue)
        {
            if (connectionStringName == null || connectionStringName.Trim() == string.Empty)
            {
                throw new Exception("No setting name was specified to update.");
            }

            var webconfig = new XmlDocument();

            var webconfigPath = _rootFolder + Path.DirectorySeparatorChar + "web.config";

            if (!File.Exists(webconfigPath))
            {
                throw new Exception("Unable to open web.config file: " + webconfigPath);
            }

            //Load the XML
            try
            {
                const string xmlns = "http://schemas.microsoft.com/.NetConfiguration/v2.0";

                webconfig.Load(webconfigPath);
                var ns = new XmlNamespaceManager(webconfig.NameTable);
                ns.AddNamespace("ms", xmlns);

                var configurationNode = webconfig.SelectSingleNode("/ms:configuration", ns);

                if (configurationNode == null)
                {
                    throw new Exception("Unable to find <configuration /> node in web.config " + webconfigPath);
                }

                var connectionStringsNode = webconfig.SelectSingleNode("/ms:configuration/ms:connectionStrings", ns);

                //If the connection strings node does not exist, create it
                if (connectionStringsNode == null)
                {
                    connectionStringsNode = webconfig.CreateElement("connectionStrings", configurationNode.NamespaceURI);
                    configurationNode.AppendChild(connectionStringsNode);
                }

                var settingNode = connectionStringsNode.SelectSingleNode("ms:add[@name='" + connectionStringName + "']", ns);

                //If the node exists, update it.
                if (settingNode == null)
                {
                    settingNode = webconfig.CreateElement("add", configurationNode.NamespaceURI);
                    var nameAttr = webconfig.CreateAttribute("name");
                    nameAttr.Value = connectionStringName;

                    settingNode.Attributes.Append(nameAttr);
                    connectionStringsNode.AppendChild(settingNode);
                }

                if (!settingValue.Contains("Application Name=") && DatabaseProvider.ToLower() == "sqlserver")
                {
                    settingValue +=  (settingValue.EndsWith(";") ? "" : ";") + "Application Name=Checkbox Survey Server;";
                }

                var valueAttr = webconfig.CreateAttribute("connectionString");
                valueAttr.Value = settingValue;
                settingNode.Attributes.SetNamedItem(valueAttr);

                webconfig.Save(webconfigPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update web.config at " + webconfigPath + ". Error was [" + ex.Message + "].");
            }
        }

        /// <summary>
        /// Add admin user
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="status"></param>
        /// <param name="scriptFile"></param>
        private bool AddDefaultAdminUser(string scriptFile, string name, string password, out string status)
        {
            try
            {
                if (!File.Exists(scriptFile))
                {
                    status = "A required database installation script was not found:  " + scriptFile;
                    return false;
                }

                string script = null;

                try
                {
                    foreach (var scriptBlock in ReadScriptBlocks(scriptFile))
                    {
                        script = scriptBlock.Replace("{adminName}", name).Replace("{adminPassword}", password);
                        ExecuteSqlString(InstallConnectionString, script);
                    }

                    status = "Success.";
                    return true;
                }
                catch (SqlException ex)
                {
                    status = string.Format("An error occurred on line {0}, while setting up the database.  The error was:  {1} [[[ {2} ]]].", ex.LineNumber, ex.Message, script);
                    return false;
                }
                catch (Exception ex)
                {
                    status = "An error occurred while setting up the database.  The error was:  " + ex.Message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                status = "An error occurred while setting up the database.  The error was:  " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Update the web.config connection string with the specified name
        /// </summary>
        private void UpdateWebConfigServiceHostUrls()
        {
            var webconfig = new XmlDocument();

            var webconfigPath = _rootFolder + Path.DirectorySeparatorChar + "web.config";

            if (!File.Exists(webconfigPath))
            {
                throw new Exception("Unable to open web.config file: " + webconfigPath);
            }

            //Load the XML
            try
            {
                const string xmlns = "http://schemas.microsoft.com/.NetConfiguration/v2.0";

                webconfig.Load(webconfigPath);
                var ns = new XmlNamespaceManager(webconfig.NameTable);
                ns.AddNamespace("ms", xmlns);

                var configurationNode = webconfig.SelectSingleNode("/ms:configuration", ns);

                if (configurationNode == null)
                {
                    throw new Exception("Unable to find <configuration /> node in web.config " + webconfigPath);
                }

                var serviceHostNodes = webconfig.SelectNodes("/ms:configuration/ms:system.serviceModel/ms:services/ms:service/ms:host/ms:baseAddresses/ms:add", ns);

                if (serviceHostNodes == null || serviceHostNodes.Count == 0)
                {
                    throw new Exception("Unable to locate service host elements in " + webconfigPath);
                }

                foreach (XmlNode serviceHostNode in serviceHostNodes)
                {
                    if (serviceHostNode.Attributes == null)
                    {
                        continue;
                    }

                    foreach (XmlAttribute attr in serviceHostNode.Attributes)
                    {
                        if (attr.Name.Equals("baseAddress"))
                        {
                            attr.Value = _applicationUrl + ApplicationRoot + "/Services";
                        }
                    }
                }

                webconfig.Save(webconfigPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update web.config at " + webconfigPath + ". Error was [" + ex.Message + "].");
            }
        }


        /// <summary>
        /// Update the web.config connection string with the specified name
        /// </summary>
        private void UpdateWebConfigServiceClientUrls()
        {
            var webconfig = new XmlDocument();

            var webconfigPath = _rootFolder + Path.DirectorySeparatorChar + "web.config";

            if (!File.Exists(webconfigPath))
            {
                throw new Exception("Unable to open web.config file: " + webconfigPath);
            }

            //Load the XML
            try
            {
                const string xmlns = "http://schemas.microsoft.com/.NetConfiguration/v2.0";

                webconfig.Load(webconfigPath);
                var ns = new XmlNamespaceManager(webconfig.NameTable);
                ns.AddNamespace("ms", xmlns);

                var configurationNode = webconfig.SelectSingleNode("/ms:configuration", ns);

                if (configurationNode == null)
                {
                    throw new Exception("Unable to find <configuration /> node in web.config " + webconfigPath);
                }

                var clientHostHodes = webconfig.SelectNodes("/ms:configuration/ms:system.serviceModel/ms:client/ms:endpoint", ns);

                if (clientHostHodes == null || clientHostHodes.Count == 0)
                {
                    throw new Exception("Unable to locate service client endpoint elements in " + webconfigPath);
                }

                foreach (XmlNode serviceHostNode in clientHostHodes)
                {
                    if (serviceHostNode.Attributes == null)
                    {
                        continue;
                    }

                    foreach (XmlAttribute attr in serviceHostNode.Attributes)
                    {
                        if (attr.Name.Equals("address"))
                        {
                            var attrValue = attr.Value;

                            //Find address after "/Services/Portion
                            var clientAddrPart = attrValue.Substring(
                                attrValue.IndexOf("/Services", StringComparison.InvariantCultureIgnoreCase) 
                                + "/Services/".Length);

                            
                            attr.Value = _applicationUrl + ApplicationRoot + "/Services/" + clientAddrPart;
                        }
                    }
                }

                webconfig.Save(webconfigPath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update web.config at " + webconfigPath + ". Error was [" + ex.Message + "].");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of the product.
        /// </summary>
        public string Name
        {
            get { return "CheckboxWeb Survey"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSlaAccepted { set; get; }

        /// <summary>
        /// Get the version of the product
        /// </summary>
        public string Version
        {
            get
            {
                return _installedVersion ?? _version;
            }
        }

        /// <summary>
        /// Get whether this is a new install
        /// </summary>
        public bool IsNewInstall
        {
            get { return _newInstall; }
        }

        /// <summary>
        /// Get the id of the installed product
        /// </summary>
        public Int32 InstalledProductID { get; private set; }

        /// <summary>
        /// Get a list of installed modules
        /// </summary>
        public List<ModuleInfo> InstalledModules { get; private set; }

        /// <summary>
        /// Get the information for the specified module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public ModuleInfo GetInstalledModuleInfo(string moduleName)
        {
            return InstalledModules.FirstOrDefault(info => info.CompareTo(moduleName) == 0);
        }

        /// <summary>
        /// Get the information for the specified module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public ModuleInfo GetAvailableModuleInfo(string moduleName)
        {
            foreach (ModuleInfo info in AvailableModules)
            {
                if (info.CompareTo(moduleName) == 0)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Get a list of modules available to install
        /// </summary>
        public List<ModuleInfo> AvailableModules
        {
            get
            {
                List<ModuleInfo> availableModules = new List<ModuleInfo>();

                foreach (ModuleInfo moduleInfo in _allModules)
                {
                    bool installed = false;

                    foreach (ModuleInfo moduleInfo2 in InstalledModules)
                    {
                        if (string.Compare(moduleInfo.Name, moduleInfo2.Name, true) == 0)
                        {
                            installed = true;
                            break;
                        }
                    }

                    bool preReqMet = CheckPrerequisite(moduleInfo.PreRequisiteProduct, moduleInfo.PreRequisiteProductVersions, ProductName, ProductVersion, false);

                    if (!installed && preReqMet)
                    {
                        availableModules.Add(moduleInfo);
                    }
                }

                return availableModules;
            }
        }

        /// <summary>
        /// Get a list of installed patches
        /// </summary>
        public List<PatchInfo> InstalledPatches { get; private set; }

        /// <summary>
        /// Get a list of patches available for this installation
        /// </summary>
        public List<PatchInfo> AvailablePatches
        {
            get
            {
                var maxPatches = new Dictionary<string, PatchInfo>();

                foreach (PatchInfo patchInfo in _allPatches)
                {
                    bool modulePreReqMet = false;

                    //Check if the patch is already installed
                    bool installed = InstalledPatches.Any(patchInfo2 => string.Compare(patchInfo.Name, patchInfo2.Name, true) == 0);

                    //Simplify for now and assume prerequisites met
                    /*
                    //Check if the pre-requisite product is installed
                    bool productPreReqMet = !Common.Utilities.IsNotNullOrEmpty(patchInfo.PreRequisiteProduct.Trim()) || patchInfo.PreRequisiteProductVersions.Contains(Version);

                    //Check if the prequisite module is installed
                    if (Common.Utilities.IsNullOrEmpty(patchInfo.PreRequisiteModule.Trim()))
                    {
                        modulePreReqMet = true;
                    }
                    else
                    {
                        foreach (ModuleInfo info in InstalledModules)
                        {
                            if (patchInfo.PreRequisiteModuleVersions.Contains(info.Version))
                            {
                                modulePreReqMet = true;
                                break;
                            }                            
                        }
                    }

                    if (productPreReqMet && modulePreReqMet) */
                    
                    if(true)
                    {
                        //If patch already installed, make sure that we haven't already added a patch for a lower version to
                        // the list of available patches.
                        if (installed)
                        {
                            //If the patch is already installed, remove any possible patches that are for a lower version
                            if (maxPatches.ContainsKey(patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule))
                            {
                                //Check if this patch has the highest version for the product/module combination
                                string[] savedVersion = maxPatches[patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule].Version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] compareVersion = patchInfo.Version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                if (savedVersion.Length == compareVersion.Length)
                                {
                                    int i = 0;
                                    foreach (string versionPiece in savedVersion)
                                    {
                                        //If version of patch is < version of installed patch, remove it from list of potential items
                                        if (int.Parse(compareVersion[i]) > int.Parse(versionPiece))
                                        {
                                            maxPatches.Remove(patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule);
                                            break;
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                        //If patch not alread installed
                        else
                        {
                            //Now check if this is the highest-version for the patch
                            if (!maxPatches.ContainsKey(patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule))
                            {
                                maxPatches[patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule] = patchInfo;
                            }
                            else
                            {
                                //Check if this patch has the highest version for the product/module combination
                                string[] savedVersion = maxPatches[patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule].Version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                string[] compareVersion = patchInfo.Version.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                if (savedVersion.Length == compareVersion.Length)
                                {
                                    int i = 0;
                                    foreach (string versionPiece in savedVersion)
                                    {
                                        if (int.Parse(compareVersion[i]) > int.Parse(versionPiece))
                                        {
                                            maxPatches[patchInfo.PreRequisiteProduct + "_" + patchInfo.PreRequisiteModule] = patchInfo;
                                            break;
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }

                return maxPatches.Values.ToList();
            }
        }


        /// <summary>
        /// Get/set the connection string for the install
        /// </summary>
        public string InstallConnectionString { get; set; }

        /// <summary>
        /// Get/set the connection string for the Messaging Service Database
        /// </summary>
        public string EmailDBConnectionString { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string AppConnectionString { get; set; }

        /// <summary>
        /// Get/set the URL of the application.
        /// </summary>
        public string ApplicationURL
        {
            get { return _applicationUrl; }
            set { _applicationUrl = value; }
        }

        /// <summary>
        /// Get/set the application virtual folder name
        /// </summary>
        public string ApplicationRoot { get; set; }

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName
        {
            get { return Name; }
        }

        /// <summary>
        /// Admin user name
        /// </summary>
        public string AdminUserName { get; set; }

        /// <summary>
        /// Admin password
        /// </summary>
        public string AdminPassword { get; set; }

        /// <summary>
        /// The product version
        /// </summary>
        public string ProductVersion
        {
            get
            {
                string maxVersion = Version;

                foreach (PatchInfo patchInfo in InstalledPatches)
                {
                    if (CompareVersions(maxVersion, patchInfo.Description, false))
                    {
                        maxVersion = patchInfo.Description;
                    }
                }

                return maxVersion;
            }
        }

        /// <summary>
        /// The application assembly short version
        /// </summary>
        public static decimal ApplicationAssemblyShortVersion
        {
            get
            {
                var productVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                var versionString = string.Format("{0}.{1}", productVersion.Major, productVersion.Minor);

                return Convert.ToDecimal(versionString, new CultureInfo("en-US"));
            }
        }

        /// <summary>
        /// The application assembly version
        /// </summary>
        public static string ApplicationAssemblyVersion
        {
            get
            {
                var productVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                var versionString = string.Format("{0}.{1}", productVersion.Major, productVersion.Minor);
                if (productVersion.Build != 0)
                    versionString += "." + productVersion.Build;

                return versionString;
            }
        }

        /// <summary>
        /// Installation behavior flags
        /// </summary>
        public Boolean InstallDatabase { get; set; }
        public Boolean InstallEMailDatabase { get; set; }
        public Boolean InstallSampleSurveys { get; set; }
        public Boolean InstallSampleStyles { get; set; }
        public Boolean InstallSampleLibraries { get; set; }


        /// <summary>
        /// SQL Server-specific install properties
        /// </summary>
        public string SQLServerName { get; set; }
        public string SQLDatabaseName { get; set; }
        public bool SQLAppUseTrustedConnection { get; set; }
        public string SQLAppUsername { get; set; }
        public string SQLAppPassword { get; set; }


        #endregion


        /// <summary>
        /// Check the specified prerequisite
        /// </summary>
        /// <param name="preReqName"></param>
        /// <param name="preReqVersions"></param>
        /// <param name="installedProductName"></param>
        /// <param name="installedProductVersion"></param>
        /// <param name="requireVersionMatch"></param>
        /// <returns></returns>
        private static bool CheckPrerequisite(string preReqName, List<string> preReqVersions, string installedProductName, string installedProductVersion, bool requireVersionMatch)
        {
            //Check if the pre-requisite product is installed
            if (string.IsNullOrEmpty(preReqName))
            {
                return true;
            }
            
            if (string.Compare(preReqName, installedProductName, true) == 0)
            {
                if (preReqName.Trim() == string.Empty)
                {
                    return true;
                }
                    
                foreach (var preReqVersion in preReqVersions)
                {
                    if (CompareVersions(preReqVersion, installedProductVersion, requireVersionMatch))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Compares the version number required to the currently installed version number
        /// </summary>
        /// <param name="preReqVersion"></param>
        /// <param name="installedProductVersion"></param>
        /// <param name="requireExactMatch"></param>
        /// <returns></returns>
        private static bool CompareVersions(string preReqVersion, string installedProductVersion, bool requireExactMatch)
        {
            var installedVersionParts = installedProductVersion.Split('.');
            var preReqVersionParts = preReqVersion.Split('.');

            if (installedVersionParts.Length != preReqVersionParts.Length)
            {
                return false;
            }

            for (var i = 0; i < preReqVersionParts.Length; i++)
            {
                Int32 preReqVersionPart;
                if (Int32.TryParse(preReqVersionParts[i], out preReqVersionPart))
                {
                    Int32 installedVersionPart;
                    if (Int32.TryParse(installedVersionParts[i], out installedVersionPart))
                    {
                        if ((requireExactMatch && (preReqVersionPart != installedVersionPart)) || (preReqVersionPart > installedVersionPart))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        
        /// <summary>
        /// Tests that database is available
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="statusMessage"></param>
        /// <returns></returns>
        public bool TestDBConnectivity(string connectionString, out String statusMessage)
        {
            IDbConnection connection;

            switch (DatabaseProvider)
            {
                /*
            case "mysql":
                connection = new MySqlConnection(connectionString);
                command = new MySqlCommand(query, (MySqlConnection)connection);
                v3command = new MySqlCommand(v3query, (MySqlConnection)connection);
                break;
            case "oracle":
                connection = new OracleConnection(connectionString);
                command = new OracleCommand(query, (OracleConnection)connection);
                v3command = new OracleCommand(v3query, (OracleConnection)connection);
                break;
            case "npgsql":
                connection = new NpgsqlConnection(connectionString);
                command = new NpgsqlCommand(query, (NpgsqlConnection)connection);
                v3command = new NpgsqlCommand(v3query, (NpgsqlConnection)connection);
                break;
            case "db2":
                connection = new DB2Connection(connectionString);
                command = new DB2Command(query, (DB2Connection)connection);
                v3command = new DB2Command(v3query, (DB2Connection)connection);
                break;
                 */
                default:
                    connection = new SqlConnection(connectionString);
                    break;
            }
            var result = true;
            statusMessage = String.Empty;
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                result = false;
                statusMessage = e.Message;
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        ///<summary>
        ///</summary>
        ///<param name="connectionString"></param>
        ///<returns></returns>
        public bool TestForExistingMailDatabase(string connectionString)
        {
            const string query = "select * from information_schema.tables where table_name = 'ckbx_DatabaseRelayEmail_Batches'";

            IDbConnection connection = new SqlConnection(connectionString);
            IDbCommand command = new SqlCommand(query, (SqlConnection)connection);

            try
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return true;
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }

            return false;
        }

        /// <summary>
        /// Test for the presence of an existing Checkbox database
        /// </summary>
        /// <param name="connectionString">Connection string to the database</param>
        /// <param name="previousVersion"> </param>
        public bool TestForExistingDatabase(string connectionString, out int? previousVersion)
        {
            IDbConnection connection;
            IDbCommand command, versionCommand;
            var dbExists = false;
            previousVersion = null;

            const string query = "SELECT * FROM ckbx_Group WHERE GroupID = 1;";
            const string versionQuery = "Select Version FROM ckbx_Product_Info WHERE ProductName = 'CheckboxWeb Survey'";

            switch (DatabaseProvider)
            {
                    /*
                case "mysql":
                    connection = new MySqlConnection(connectionString);
                    command = new MySqlCommand(query, (MySqlConnection)connection);
                    v3command = new MySqlCommand(v3query, (MySqlConnection)connection);
                    break;
                case "oracle":
                    connection = new OracleConnection(connectionString);
                    command = new OracleCommand(query, (OracleConnection)connection);
                    v3command = new OracleCommand(v3query, (OracleConnection)connection);
                    break;
                case "npgsql":
                    connection = new NpgsqlConnection(connectionString);
                    command = new NpgsqlCommand(query, (NpgsqlConnection)connection);
                    v3command = new NpgsqlCommand(v3query, (NpgsqlConnection)connection);
                    break;
                case "db2":
                    connection = new DB2Connection(connectionString);
                    command = new DB2Command(query, (DB2Connection)connection);
                    v3command = new DB2Command(v3query, (DB2Connection)connection);
                    break;
                     */
                default:
                    connection = new SqlConnection(connectionString);
                    command = new SqlCommand(query, (SqlConnection)connection);
                    versionCommand = new SqlCommand(versionQuery, (SqlConnection) connection);
                    break;
            }

            try
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        dbExists = true;
                }
            }
            catch { }
            finally
            {
                connection.Close();
            }

            try
            {
                connection.Open();

                using (var reader = versionCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ver = reader[0] as string;
                        int pointIndex;
                        if (!string.IsNullOrEmpty(ver) && (pointIndex = ver.IndexOf('.')) >= 0)
                            ver = ver.Remove(pointIndex);

                        int version;
                        if (int.TryParse(ver, out version) && (!previousVersion.HasValue || version > previousVersion))
                        {
                            previousVersion = version;                            
                        }
                    }
                }
            }
            catch {}
            finally
            {
                connection.Close();
            }

            return dbExists;
        }

        /// <summary>
        /// Create a List database provider names that contain the sql scripts to for installation
        /// </summary>
        /// <returns></returns>
        public List<string> CreateDbInstallSelector(string rootPath)
        {
            var availableDatabases = new List<string>();
            var path = rootPath + string.Format("{0}Install{0}InstallScripts{0}", Path.DirectorySeparatorChar);
            foreach (var directoryName in Directory.GetDirectories(path))
            {
                if (directoryName.Replace(path, "").ToLower() != "upgrade")
                {
                    var scriptFiles = Directory.GetFiles(directoryName, "*.sql");

                    if (scriptFiles.Length > 0)
                    {
                        availableDatabases.Add(directoryName.Replace(path, ""));
                    }
                }
            }

            return availableDatabases;
        }
        
        /// <summary>
        /// Loads files and scripts for email database setup
        /// </summary>
        /// <param name="installType"></param>
        public void LoadInstallEmailFilesAndScripts(string installType)
        {
            LoadFilesAndScriptsByType("emaildb" + installType, ref _mailDBInstallScripts, ref _mailDBInstallFiles);
        }

        /// <summary>
        /// Checks if the update is needed for email db
        /// </summary>
        /// <returns></returns>
        public bool UpgradeIsNeededForEmailDB()
        {
            try
            {
                IDbConnection connection = new SqlConnection(EmailDBConnectionString);
                connection.Open();
                IDbCommand command = new SqlCommand(
                    @" IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME='ckbx_DatabaseRelayEmail_Batches' AND COLUMN_NAME='WebServiceURL')	
                        AND EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME='ckbx_DatabaseRelayEmail_Batches' AND COLUMN_NAME='BatchId')	
                    BEGIN		
                        SELECT 1 as UpgradeIsNeededForEmailDB
	                END
                    ELSE
                    BEGIN		
                        SELECT 0 as UpgradeIsNeededForEmailDB
	                END",
                    (SqlConnection)connection);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0].ToString() == "0")
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the install is needed for email db
        /// </summary>
        /// <returns></returns>
        public bool InstallIsNeededForEmailDB()
        {
            try
            {
                IDbConnection connection = new SqlConnection(EmailDBConnectionString);
                connection.Open();
                IDbCommand command = new SqlCommand(
                    @" IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME='ckbx_DatabaseRelayEmail_Batches' AND COLUMN_NAME='BatchId')	
                    BEGIN		
                        SELECT 0 as InstallIsNeededForEmailDB
	                END
                    ELSE
                    BEGIN		
                        SELECT 1 as InstallIsNeededForEmailDB
	                END",
                    (SqlConnection)connection);
                IDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (reader[0].ToString() == "0")
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///<summary>
        /// The application assembly full version
        /// </summary>
        public static string ApplicationAssemblyFullVersion
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public bool DropEmailDatabaseData(out string status)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

            stringBuilder.AppendLine("select name into #tables from sys.objects where type = 'U'");
            stringBuilder.AppendLine("while (select count(1) from #tables) > 0");
            stringBuilder.AppendLine("begin");
            stringBuilder.AppendLine("declare @sql varchar(max)");
            stringBuilder.AppendLine("declare @tbl varchar(255)");
            stringBuilder.AppendLine("select top 1 @tbl = name from #tables");
            stringBuilder.AppendLine("set @sql = 'drop table ' + @tbl");
            stringBuilder.AppendLine("exec(@sql)");
            stringBuilder.AppendLine("delete from #tables where name = @tbl");
            stringBuilder.AppendLine("end");
            stringBuilder.AppendLine("drop table #tables;");

            stringBuilder.AppendLine("EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'");

            status = null;
            IDbConnection connection = new SqlConnection(EmailDBConnectionString);

            try
            {
                IDbCommand command = new SqlCommand(stringBuilder.ToString(), (SqlConnection)connection);

                connection.Open();
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                status = "An error occurred while overwriting the Messaging Service Database.  The error was:  " + e.Message;
            }
            finally
            {
                connection.Close();
            }

            return false;
        }

        /// <summary>
        /// Runs email db install scripts
        /// </summary>
        public bool RunEMailDatabaseScripts(out string status)
        {
            return ValidateAndRunScripts(EmailDBConnectionString, _mailDBInstallScripts, out status);
        }

        public string ProgressKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Version CurrentPatchVersion
        {
            get
            {
                if (InstalledPatches != null)
                {
                    var last = InstalledPatches.Max(p => new Version(p.Version));
                    return last;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static ApplicationInstaller CachedApplicationInstaller
        {
            get
            {
                var cacheManager = CacheFactory.GetCacheManager();
                var appInstaller = cacheManager.GetData(APP_INSTALLER_CACHE_KEY) as ApplicationInstaller;
                return appInstaller;
            }
            set
            {
                var cacheManager = CacheFactory.GetCacheManager();
                cacheManager.Add(APP_INSTALLER_CACHE_KEY, value);
            }
        }
    }
}