///****************************************************************************
// * UpgradeInstaller.cs												        *
// * Support class for upgrading Ultimate Survey to Checkbox.				    *
// ****************************************************************************/
//using System;
//using System.IO;
//using System.Xml;
//using System.Data;
//using System.Data.SqlClient;
//using System.Collections.Generic;

//using Checkbox.Styles;

//using Prezza.Framework.Common;
//using Prezza.Framework.Data;
//using Prezza.Framework.ExceptionHandling;


//namespace Checkbox.Configuration.Install
//{
//    ///<summary>
//    ///</summary>
//    [Serializable]
//    public class UpgradeInstaller : ApplicationInstaller
//    {
//        private string _upgradeProductName;
//        private List<string> _preProcessingScripts;
//        private List<string> _postProcessingScripts;
//        private Dictionary<string, string> _isoMappingTable;

//        /// <summary>
//        /// Default constructor
//        /// </summary>
//        /// <param name="rootFolder"></param>
//        public UpgradeInstaller(string rootFolder) : this(rootFolder, false)
//        {
//        }

//        /// <summary>
//        /// Construct a new upgrade installer
//        /// </summary>
//        public UpgradeInstaller(string rootFolder, bool forceNew) : base(rootFolder, forceNew, "sqlserver")
//        {
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        protected override string InstallType
//        {
//            get
//            {
//                return "upgrade";
//            }
//        }

//        /// <summary>
//        /// Get/set the previous version
//        /// </summary>
//        public string PreviousVersion { get; set; }

//        /// <summary>
//        /// Executes the entire Upgrade Procedure from Ultimate Survey 3.0 to Checkbox 4.0
//        /// </summary>
//        /// <param name="status">a string to report any status errors encountered during the operation</param>
//        /// <returns>true, if execution successfull; otherwise false</returns>
//        public bool DoUpgrade(out string status)
//        {
//            if (!ExecuteUpgradeScripts(out status))
//            {
//                return false;
//            }
            
//            ExecuteInCodeUpgradeOperations();

//            if (!ExecutePostUpgradeScripts(out status))
//            {
//                return false;
//            }

//            return true;
//        }

//        /// <summary>
//        /// Determines in the upgrade process has already been performed
//        /// </summary>
//        /// <remarks>
//        /// The Upgrade process places an entry in the ckbx_Product_Info table, acting as if it is an 'installed' product.  If 
//        /// the table contains this entry, then the Upgrade has been executed.
//        /// </remarks>
//        /// <param name="status">a string to report any status errors encountered during the operation</param>
//        /// <returns>true, if execution successfull; otherwise false</returns>
//        public bool TestForExistingUpgrade(out string status)
//        {
//            if (string.IsNullOrEmpty(InstallConnectionString))
//            {
//                status = "No connection string was specified.";
//                return true;
//            }

//            status = string.Empty;
//            bool result = false;

//            using (SqlConnection connection = new SqlConnection(InstallConnectionString))
//            {
//                try
//                {
//                    connection.Open();

//                    string testQuery = "select ProductID from ckbx_Product_Info where ProductName = N'" + _upgradeProductName + "' ";

//                    SqlCommand command = connection.CreateCommand();
//                    command.CommandText = testQuery;

//                    using (IDataReader reader = command.ExecuteReader())
//                    {
//                        if (reader != null)
//                        {
//                            try
//                            {
//                                if (reader.Read())
//                                {
//                                    status = "The upgrade procedure has already completed for this installation.";
//                                    result = true;
//                                }
//                            }
//                            finally
//                            {
//                                reader.Close();
//                            }
//                        }
//                    }
//                }
//                catch { }
//                finally
//                {
//                    connection.Close();
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        /// Runs the set of database scripts designated in the UpgradeInfo.xml file as preProcessing, i.e., before application-level 
//        /// processing is performed.
//        /// </summary>
//        /// <remarks>
//        /// The preProcessing scripts perform the bulk of Upgrade operations, including the creation and execution of data migration scripts 
//        /// and the transformation of most, but not all, of the data fro 3.0 to 4.0.
//        /// </remarks>
//        /// <param name="status">a string to report any status errors encountered during the operation</param>
//        /// <returns>true, if execution successfull; otherwise false</returns>
//        private bool ExecuteUpgradeScripts(out string status)
//        {
//            status = string.Empty;
//            try
//            {
//                if (string.IsNullOrEmpty(InstallConnectionString))
//                {
//                    status = "No connection string was specified.";
//                    return false;
//                }

//                //Run the 3.x to 3.0.10 upgrade
//                if (PreviousVersion != "3.0.10")
//                {
//                    string scriptName = RootFolder + Path.DirectorySeparatorChar + "Install" + Path.DirectorySeparatorChar + "InstallScripts" + Path.DirectorySeparatorChar + "Upgrade" + Path.DirectorySeparatorChar + "Patch-" + PreviousVersion + "-to-3.0.10.sql";

//                    if (!File.Exists(scriptName))
//                    {
//                        throw new Exception("Unable to find patch files: " + scriptName);
//                    }
                    
//                    bool runStatus = RunDBScriptFile(scriptName, out status);

//                    if (!runStatus)
//                    {
//                        return false;
//                    }
//                }

//                //Validate that scripts exist before running them
//                foreach (string sqlScript in _preProcessingScripts)
//                {
//                    if (sqlScript.Trim().StartsWith("EXEC"))
//                    {
//                        continue;
//                    }
                    
//                    if (!File.Exists(sqlScript))
//                    {
//                        status = "A required database installation script was not found:  " + sqlScript;
//                        return false;
//                    }
//                }

//                foreach (string sqlScript in _preProcessingScripts)
//                {
//                    bool sqlRunStatus = RunDBScriptFile(sqlScript, out status);

//                    if (!sqlRunStatus)
//                    {
//                        return false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                status = "An error occurred while upgrading the database.  The error was:  " + ex.Message;
//                return false;
//            }

//            return true;
//        }

//        /// <summary>
//        /// Runs the set of database scripts designated in the UpgradeInfo.xml file as postProcessing, 
//        /// i.e., after the main port of data from the 3.0 database
//        /// </summary>
//        /// <param name="status">a string to report any status errors encountered during the operation</param>
//        /// <returns>true, if execution successfull; otherwise false</returns>
//        private bool ExecutePostUpgradeScripts(out string status)
//        {
//            status = string.Empty;
//            try
//            {
//                if (string.IsNullOrEmpty(InstallConnectionString))
//                {
//                    status = "No connection string was specified.";
//                    return false;
//                }

//                //Validate that scripts exist before running them
//                foreach (string sqlScript in _postProcessingScripts)
//                {
//                    if (sqlScript.Trim().StartsWith("EXEC"))
//                    {
//                        continue;
//                    }
                    
//                    if (!File.Exists(sqlScript))
//                    {
//                        status = "A required database installation script was not found:  " + sqlScript;
//                        return false;
//                    }
//                }

//                foreach (string sqlScript in _postProcessingScripts)
//                {
//                    bool sqlRunStatus = RunDBScriptFile(sqlScript, out status);

//                    if (!sqlRunStatus)
//                    {
//                        return false;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                status = "An error occurred while upgrading the database.  The error was:  " + ex.Message;
//                return false;
//            }

//            return true;
//        }

//        /// <summary>
//        /// Overridden.  Adds load functionality to the ApplicationInstaller.LoadInstallFilesAndScripts method to retrieve 
//        /// upgrade specific configuration directives
//        /// </summary>
//        public override void LoadInstallFilesAndScripts()
//        {
//            base.LoadInstallFilesAndScripts();

//            XmlDocument doc = new XmlDocument();
//            doc.Load(RootFolder + Path.DirectorySeparatorChar + "Install" + Path.DirectorySeparatorChar + "UpgradeInfo.xml");

//            _upgradeProductName = XmlUtility.GetNodeText(doc.SelectSingleNode("/upgradeInfo/properties/name"), true);

//            // Upgrade scripts to be run prior to in-code processing
//            _preProcessingScripts = new List<string>();

//            XmlNodeList preProcessingScriptsList = doc.SelectNodes("/upgradeInfo/databaseScripts/preProcessing/databaseScript");

//            if (preProcessingScriptsList != null)
//            {
//                foreach (XmlNode script in preProcessingScriptsList)
//                {
//                    if (script.InnerText.Trim().StartsWith("EXEC"))
//                    {
//                        _preProcessingScripts.Add(script.InnerText.Trim());
//                    }
//                    else
//                    {
//                        _preProcessingScripts.Add(RootFolder + Path.DirectorySeparatorChar + script.InnerText.Trim());
//                    }
//                }
//            }

//            // Upgrade scripts to be run post in-code processing
//            _postProcessingScripts = new List<string>();

//            XmlNodeList postProcessingScriptsList = doc.SelectNodes("/upgradeInfo/databaseScripts/postProcessing/databaseScript");

//            if (postProcessingScriptsList != null)
//            {
//                foreach (XmlNode script in postProcessingScriptsList)
//                {
//                    if (script.InnerText.Trim().StartsWith("EXEC"))
//                    {
//                        _postProcessingScripts.Add(script.InnerText.Trim());
//                    }
//                    else
//                    {
//                        _postProcessingScripts.Add(RootFolder + Path.DirectorySeparatorChar + script.InnerText.Trim());
//                    }
//                }
//            }

//            LoadIsoMappingTable();
//        }

//        /// <summary>
//        /// Overridden.  Executes a database script contained in a file.  Adds functionality to support inline scripts placed in the 
//        /// xml config file under a databaseScript node
//        /// </summary>
//        /// <param name="sqlScript">the script to be executed, usually a file path</param>
//        /// <param name="status">a string to report any status errors encountered during the operation</param>
//        /// <returns>true, if execution successfull; otherwise false</returns>
//        public override bool RunDBScriptFile(string sqlScript, out string status)
//        {
//            if (sqlScript.Trim().StartsWith("EXEC"))    // intercept sql commands
//            {
//                Database db = DatabaseFactory.CreateDatabase();
//                using (IDbConnection connection = db.GetConnection())
//                {
//                    try
//                    {
//                        sqlScript = sqlScript.Replace("[[PREVIOUS_VERSION]]", PreviousVersion);
                        
//                        sqlScript = sqlScript.Replace("[[PRODUCTID]]", InstalledProductID.ToString());
                        
//                        connection.Open();
//                        DBCommandWrapper command = db.GetSqlStringCommandWrapper(sqlScript);
//                        db.ExecuteNonQuery(command);
//                        status = "Success.";
//                        return true;
//                    }
//                    catch
//                    {
//                        status = "An error occurred executing a required script: " + sqlScript;
//                        return false;
//                    }
//                }
//            }
            
//            // all other calls can be handled by the base class
//            return base.RunDBScriptFile(sqlScript, out status);
//        }

//        /// <summary>
//        /// Creates a dictionary to map ISO code 639 values from 3.0 to ISO Code 639-3166 codes, valid for 4.0
//        /// </summary>
//        /// <remarks>
//        /// This operation uses the xml config for Upgrade and can be amended prior to upgrade by ML administrators to account for idiosyncracies 
//        /// in the ML ISO code designations
//        /// </remarks>
//        private void LoadIsoMappingTable()
//        {
//            //Attempt to get the information for this product
//            XmlDocument doc = new XmlDocument();
//            doc.Load(RootFolder + Path.DirectorySeparatorChar + "Install" + Path.DirectorySeparatorChar + "UpgradeInfo.xml");

//            _isoMappingTable = new Dictionary<string, string>();

//            XmlNodeList filesList = doc.SelectNodes("/upgradeInfo/SupportedLanguages/Language");

//            if (filesList != null)
//            {
//                foreach (XmlNode fileNode in filesList)
//                {
//                    string iso639 = XmlUtility.GetNodeText(fileNode.SelectSingleNode("ISOCode639"));
//                    string iso639_3166 = XmlUtility.GetNodeText(fileNode.SelectSingleNode("ISOCode639-3166"));

//                    if (iso639.Trim() != string.Empty && iso639_3166.Trim() != string.Empty)
//                    {
//                        _isoMappingTable.Add(iso639, iso639_3166);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Performs operations that rely upon application code, rather than database scripts, for their execution
//        /// </summary>
//        private void ExecuteInCodeUpgradeOperations()
//        {
//            UpgradeStyleTemplateCss();
//            InternationalizeUpgradedInstallation();
//        }

//        /// <summary>
//        /// Performs operations that bring the upgraded database into compliance with the new Checkbox internationalization support
//        /// </summary>
//        private void InternationalizeUpgradedInstallation()
//        {
//            if (IsMLInstallation())
//            {
//                UpgradeMultiLanguageSurveyText();
//            }

//            // perform other operations for internationalization
//            UpgradeSurveySupportedLanguages();
//        }

//        /// <summary>
//        /// Parses the SupportedLanguage and DefaultLanguage settings of ML Surveys and translates to 4.0 supported ISO codes
//        /// </summary>
//        private static void UpgradeSurveySupportedLanguages()
//        {
//        }

//        /// <summary>
//        /// Upgrade process method.  After upgrade scripts have run, called for post-script processing.
//        /// </summary>
//        /// <remarks>
//        /// This method uses the application components to parse out the Ultimate Survey style template's original CSS string from 
//        /// xml into the Checkbox 4.0 database persisted format.
//        /// </remarks>
//        private static void UpgradeStyleTemplateCss()
//        {
//            try
//            {
//                Database db = DatabaseFactory.CreateDatabase();

//                using (IDbConnection connection = db.GetConnection())
//                {
//                    connection.Open();

//                    DBCommandWrapper command = db.GetSqlStringCommandWrapper("SELECT TemplateID, Css FROM rz_StyleTemplate");

//                    using (IDataReader reader = db.ExecuteReader(command))
//                    {
//                        while (reader.Read())
//                        {
//                            if (!reader.IsDBNull(1) && (string)reader[1] != string.Empty)
//                            {
//                                // get the template from the database
//                                StyleTemplate template = StyleTemplateManager.GetStyleTemplate((int)reader[0]);

//                                // create an xml document from the old css-describing xml
//                                XmlDocument cssDoc = new XmlDocument();
//                                cssDoc.LoadXml((string)reader[1]);

//                                // load in the old css xml
//                                template.LoadCssClasses(cssDoc);

//                                // save it to the database, the css will be parsed and saved correctly
//                                template.Save();

//                            }
//                        }
//                    }

//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                bool rethrow = ExceptionPolicy.HandleException(ex, "UIProcess");

//                if (rethrow)
//                {
//                    throw;
//                }
//            }
//        }

//        /// <summary>
//        /// Determines whether the installation supports Multi-Language surveys
//        /// </summary>
//        /// <returns>true, if supported; otherwise, false</returns>
//        private static bool IsMLInstallation()
//        {
//            /****************************************
//             * because one consequence of using the license file to determine ML support 
//             * is that those who perhaps had trouble using ML in the past and/or are undecided 
//             * about upgrading their ML installation won't be able to upgrade their data
//             * at a later date  Rather than complicating the upgrade with a license check
//             * we simply check for any ML surveys and treat that as sufficient evidence for
//             * upgrading ML.  Data will be there even if the ML module isn't so at lease 
//             * one language will be supported.
//             * **************************************/

//            Database db = DatabaseFactory.CreateDatabase();
//            DBCommandWrapper command = db.GetSqlStringCommandWrapper("SELECT COUNT(FormID) FROM rz_Form WHERE EnableMultipleLanguages = 1");
            
//            return (int)db.ExecuteScalar(command) > 0;
//        }

     
//        /// <summary>
//        /// Upgrade multi-language texts and langauage codes for default text, etc.
//        /// </summary>
//        private void UpgradeMultiLanguageSurveyText()
//        {
//            UpdateTexts();
//            UpdateResponseTemplateLanguages();
//            UpdateResponseLanguages();
//        }

//        /// <summary>
//        /// Update supported languages and default languages for ML response templates
//        /// </summary>
//        private void UpdateResponseTemplateLanguages()
//        {
//            Database db = DatabaseFactory.CreateDatabase();
//            DBCommandWrapper selectCommand = db.GetSqlStringCommandWrapper("SELECT ResponseTemplateID, DefaultLanguage, SupportedLanguages FROM ckbx_ResponseTemplate");

//            DataSet ds = db.ExecuteDataSet(selectCommand);

//            if (ds.Tables.Count > 0)
//            {
//                DataRow[] textRows = ds.Tables[0].Select(null, null, DataViewRowState.CurrentRows);

//                foreach (DataRow textRow in textRows)
//                {
//                    int rtID = DbUtility.GetValueFromDataRow(textRow, "ResponseTemplateID", -1);
//                    string defaultLanguage = DbUtility.GetValueFromDataRow(textRow, "DefaultLanguage", string.Empty);
//                    string supportedLanguages = DbUtility.GetValueFromDataRow<string>(textRow, "SupportedLanguages", null);

//                    //If the default language has been set and it's not a 5 letter code, update it
//                    if (defaultLanguage != null && defaultLanguage.Length == 2)
//                    {
//                        defaultLanguage = ConvertISOCode(defaultLanguage);

//                        //Update
//                        DBCommandWrapper updateDefaultTextCommand = db.GetSqlStringCommandWrapper("UPDATE ckbx_ResponseTemplate SET DefaultLanguage = @DefaultLanguage WHERE ResponseTemplateID = @ResponseTemplateID");
//                        updateDefaultTextCommand.AddInParameter("DefaultLanguage", DbType.String, defaultLanguage);
//                        updateDefaultTextCommand.AddInParameter("ResponseTemplateID", DbType.Int32, rtID);

//                        db.ExecuteNonQuery(updateDefaultTextCommand);
//                    }

//                    string newSupportedLanguageString = defaultLanguage;

//                    //Now update supported languages, if necessary
//                    if (supportedLanguages != null)
//                    {
//                        string[] supportedLanguageArray = supportedLanguages.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

//                        foreach(string supportedLanguage in supportedLanguageArray)
//                        {
//                            string convertedLanguage = ConvertISOCode(supportedLanguage);

//                            //try not to add the languages twice
//                            if (newSupportedLanguageString != null)
//                            {
//                                if (!newSupportedLanguageString.ToLower().Contains(convertedLanguage.ToLower()))
//                                {
//                                    newSupportedLanguageString = newSupportedLanguageString + ";" + convertedLanguage;
//                                }
//                            }
//                        }
//                    }

//                    //Update
//                    DBCommandWrapper updateSupportedLanguagesCommand = db.GetSqlStringCommandWrapper("UPDATE ckbx_ResponseTemplate SET SupportedLanguages = @SupportedLanguages WHERE ResponseTemplateID = @ResponseTemplateID");
//                    updateSupportedLanguagesCommand.AddInParameter("SupportedLanguages", DbType.String, newSupportedLanguageString);
//                    updateSupportedLanguagesCommand.AddInParameter("ResponseTemplateID", DbType.Int32, rtID);

//                    db.ExecuteNonQuery(updateSupportedLanguagesCommand);
//                }
//            }
//        }

//        /// <summary>
//        /// Update language code for responses
//        /// </summary>
//        private void UpdateResponseLanguages()
//        {
//            Database db = DatabaseFactory.CreateDatabase();
//            DBCommandWrapper selectCommand = db.GetSqlStringCommandWrapper("SELECT ResponseID, Language FROM ckbx_Response WHERE Language IS NOT NULL AND LEN(Language) = 2");

//            DataSet ds = db.ExecuteDataSet(selectCommand);

//            if (ds.Tables.Count > 0)
//            {
//                DataRow[] textRows = ds.Tables[0].Select(null, null, DataViewRowState.CurrentRows);

//                foreach (DataRow textRow in textRows)
//                {
//                    long responseID = DbUtility.GetValueFromDataRow<long>(textRow, "ResponseID", -1);
//                    string language = DbUtility.GetValueFromDataRow<string>(textRow, "Language", null);

//                    language = ConvertISOCode(language);

//                    //Update
//                    DBCommandWrapper updateLanguageCommand = db.GetSqlStringCommandWrapper("UPDATE ckbx_Response SET Language = @Language WHERE ResponseID = @ResponseID");
//                    updateLanguageCommand.AddInParameter("Language", DbType.String, language);
//                    updateLanguageCommand.AddInParameter("ResponseID", DbType.Int64, responseID);

//                    db.ExecuteNonQuery(updateLanguageCommand);
//                }
//            }
//        }


//        /// <summary>
//        /// Upgrade multi-language survey text by just checking for all entries in the
//        /// ckbx_text table that matching ML text sig. from Ultimate Survey
//        /// </summary>
//        private void UpdateTexts()
//        {
//            Database db = DatabaseFactory.CreateDatabase();
//            DBCommandWrapper command = db.GetSqlStringCommandWrapper("SELECT * FROM ckbx_Text WHERE TextValue LIKE '<MultiLanguageText>%</MultiLanguageText>'");

//            DataSet ds = db.ExecuteDataSet(command);

//            //For each text, load and parse the XML
//            if (ds.Tables.Count > 0)
//            {
//                DataRow[] textRows = ds.Tables[0].Select(null, null, DataViewRowState.CurrentRows);

//                foreach (DataRow row in textRows)
//                {
//                    ConvertText(
//                        DbUtility.GetValueFromDataRow<string>(row, "LanguageCode", null),
//                        DbUtility.GetValueFromDataRow<string>(row, "TextID", null),
//                        DbUtility.GetValueFromDataRow<string>(row, "TextValue", null));
//                }
//            }
//        }
        
//        /// <summary>
//        /// Convert the text in the USE 3.x style snippet and set the text in the database
//        /// </summary>
//        /// <param name="languageCode"></param>
//        /// <param name="xmlSnippet"></param>
//        /// <param name="textID"></param>
//        private void ConvertText(string languageCode, string textID, string xmlSnippet)
//        {
//            if (xmlSnippet != null && textID != null && textID.Trim() != string.Empty)
//            {
//                try
//                {
//                    XmlDocument doc = new XmlDocument();
//                    doc.LoadXml(xmlSnippet);

//                    XmlNodeList textNodes = doc.SelectNodes("/MultiLanguageText/Text");

//                    const string deleteTextCommand = "DELETE FROM ckbx_Text WHERE TextID = @TextID AND LanguageCode = @LanguageCode";

//                    //If there is no text for this item at all, clear the text so we don't have the ML
//                    // placeholder
//                    if (textNodes != null)
//                    {
//                        if (textNodes.Count == 0)
//                        {
//                            Database db = DatabaseFactory.CreateDatabase();
//                            DBCommandWrapper command = db.GetSqlStringCommandWrapper(deleteTextCommand);
//                            command.AddInParameter("TextID", DbType.String, textID);
//                            command.AddInParameter("LanguageCode", DbType.String, languageCode);

//                            db.ExecuteNonQuery(command);
//                        }
//                        else
//                        {
//                            //Now, parse the XML and set the texts.  If a text for the current language is found, the
//                            // XML in the DB will be overwritten with the text.  If the XML did not contain the 
//                            // language, then we'll note that and clear the text from the DB.
//                            bool languageCodeFound = false;

//                            foreach (XmlNode textNode in textNodes)
//                            {
//                                XmlAttribute languageAttr = textNode.Attributes["Language"];

//                                if (languageAttr != null)
//                                {
//                                    string language = ConvertISOCode(languageAttr.Value);
//                                    string text = textNode.InnerText;

//                                    if (language != null && language.Trim() != string.Empty)
//                                    {
//                                        if (string.Compare(language, languageCode, true) == 0)
//                                        {
//                                            languageCodeFound = true;
//                                        }

//                                        Database db = DatabaseFactory.CreateDatabase();
//                                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_Set");
//                                        command.AddInParameter("TextID", DbType.String, textID);
//                                        command.AddInParameter("LanguageCode", DbType.String, language);
//                                        command.AddInParameter("TextValue", DbType.String, text);

//                                        db.ExecuteNonQuery(command);
//                                    }
//                                }
//                            }

//                            //If the XML snippet contained no text for the language code in the DB,
//                            // clear the text.
//                            if (!languageCodeFound)
//                            {
//                                Database db = DatabaseFactory.CreateDatabase();
//                                DBCommandWrapper command = db.GetSqlStringCommandWrapper(deleteTextCommand);
//                                command.AddInParameter("TextID", DbType.String, textID);
//                                command.AddInParameter("LanguageCode", DbType.String, languageCode);

//                                db.ExecuteNonQuery(command);
//                            }
//                        }
//                    }
//                }
//                catch
//                {
//                }
//            }
//        }

//        /// <summary>
//        /// Matches the fully-valid ISO code (language-REGION) for a given language code
//        /// </summary>
//        /// <param name="languageCode">the fragment language part of the ISO code</param>
//        /// <returns>a valid ISO code</returns>
//        private string ConvertISOCode(string languageCode)
//        {
//            if (_isoMappingTable.ContainsKey(languageCode))
//            {
//                return _isoMappingTable[languageCode];
//            }
            
//            // if the list doesn't contain the language, make a best guess about what it might be if it
//            // appears to not be 5 characters
//            if (languageCode.Length != 5)
//            {
//                return languageCode + "-" + languageCode.ToUpper();
//            }
                
//            return languageCode;
//        }
//    }
//}
