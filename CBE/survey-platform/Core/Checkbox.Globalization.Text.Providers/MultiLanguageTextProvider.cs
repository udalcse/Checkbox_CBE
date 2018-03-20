/****************************************************************************
 * MultiLanguageTextProvider.cs												*
 * Text provider that supports multiple languages.							*
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

using Prezza.Framework.Caching;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Data;

namespace Checkbox.Globalization.Text.Providers
{
    /// <summary>
    /// Multilanguage text provider
    /// </summary>
    public class MultiLanguageTextProvider : ITextProvider, IImportExportTextProvider
    {
        /// <summary>
        /// Provider configuration
        /// </summary>
        private MultiLanguageTextProviderData _config;

        /// <summary>
        /// Cache manager for texts
        /// </summary>
        private CacheManager _cacheManager;

        #region ITextProvider Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static string getCacheKey(string textIdentifier, string languageCode)
        {
            return languageCode.ToLower() + "|" + textIdentifier.ToLower();
        }


        /// <summary>
        /// Get the specified text using the specified language.
        /// </summary>
        /// <param name="textIdentifier">Id of text to retrieve.</param>
        /// <param name="languageCode">Language code to get data for.</param>
        /// <returns>Requested string</returns>
        public string GetText(string textIdentifier, string languageCode)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = _config.DefaultLanguage;
            }

            var key = getCacheKey(textIdentifier, languageCode);

            //If the object is in the cache, return it
            if (_cacheManager != null)
            {                
                if (_cacheManager.Contains(key))
                {
                    return (string)_cacheManager[key];
                }
            }

            //Otherwise, look up the object and put it in the cache, if necessary
            string textValue = null;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetText");
            command.AddInParameter("TextID", DbType.String, textIdentifier);
            command.AddInParameter("LanguageCode", DbType.String, languageCode);

            object scalar = db.ExecuteScalar(command);

            if (scalar != DBNull.Value)
            {
                textValue = (string)scalar;
            }

            textValue = textValue ?? string.Empty;

            if (_cacheManager != null)
            {
                _cacheManager.Add(key, textValue);
            }

            return textValue;
        }

        /// <summary>
        /// Read all texts for all languages and put them into the cache
        /// </summary>
        private void bulkInit()
        {
            _cacheManager = CacheFactory.GetCacheManager(_config.CacheManagerName);

            /*DataTable returnTable = new DataTable();
            returnTable.TableName = "MatchingTexts";
            returnTable.Columns.Add("TextId", typeof(string));
            returnTable.Columns.Add("LanguageCode", typeof(string));
            returnTable.Columns.Add("TextValue", typeof(string));
            */

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetAll");
            DataSet ds = db.ExecuteDataSet(command);

            //There should be 3 tables
            //  The first is all language codes
            //  The second is all text ids
            //  The third is the entire text table
            if (ds.Tables.Count == 3)
            {
                DataTable dtText = ds.Tables[2];

                foreach (DataRow dr in dtText.Rows)
                {
                    string textId = DbUtility.GetValueFromDataRow(dr, "TextId", string.Empty);

                    if (!string.IsNullOrWhiteSpace(textId))
                    {
                        string languageCode = DbUtility.GetValueFromDataRow(dr, "LanguageCode", string.Empty);
                        string textValue = DbUtility.GetValueFromDataRow(dr, "TextValue", string.Empty);

                        var key = getCacheKey(textId, languageCode);
                        _cacheManager.Add(key, textValue);                        
                    }
                }
            }
        }


        /// <summary>
        /// Get text data for the specified text id
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public DataTable GetTextData(string textIdentifier)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetByID");
            command.AddInParameter("TextID", DbType.String, textIdentifier);

            DataSet ds = db.ExecuteDataSet(command);

            if (ds.Tables.Count == 1)
                return ds.Tables[0];

            return null;
        }

        /// <summary>
        /// Get all texts for the specified text id
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllTexts(string textIdentifier)
        {
            var texts = new Dictionary<string, string>();

            DataTable dt = GetTextData(textIdentifier);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[1] != DBNull.Value && dr[2] != DBNull.Value)
                    {
                        texts.Add((string)dr[1], (string)dr[2]);
                    }
                }
            }

            return texts;
        }

        /// <summary>
        /// Get a data table containing matching texts
        /// </summary>
        /// <param name="matchExpressions"></param>
        /// <returns></returns>
        public DataTable GetMatchingTextData(params string[] matchExpressions)
        {
            var returnTable = new DataTable { TableName = "MatchingTexts" };
            returnTable.Columns.Add("TextId", typeof(string));
            returnTable.Columns.Add("LanguageCode", typeof(string));
            returnTable.Columns.Add("TextValue", typeof(string));

            foreach (string partialTextId in matchExpressions)
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetFiltered");
                command.AddInParameter("PartialTextID", DbType.String, partialTextId);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            string textId = DbUtility.GetValueFromDataReader(reader, "TextId", string.Empty);
                            string languageCode = DbUtility.GetValueFromDataReader(reader, "LanguageCode", string.Empty);
                            string textValue = DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty);

                            if (!string.IsNullOrEmpty(textId) && !string.IsNullOrEmpty(languageCode))
                            {
                                DataRow dr = returnTable.NewRow();
                                dr["TextId"] = textId;
                                dr["LanguageCode"] = languageCode;
                                dr["TextValue"] = textValue;

                                returnTable.Rows.Add(dr);
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return returnTable;
        }

        /// <summary>
        /// Set the text for the specified language.
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <param name="languageCode"></param>
        /// <param name="textValue"></param>
        public void SetText(string textIdentifier, string languageCode, string textValue)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = _config.DefaultLanguage;
            }

            if (textValue == null)
            {
                textValue = string.Empty;
            }

            //Set the item in the cache
            if (_cacheManager != null)
            {
                //TODO:  Handle expiration
                string key = getCacheKey(textIdentifier, languageCode);
                _cacheManager.Add(key, textValue);
            }

            //Update the database
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_Set");
            command.AddInParameter("TextID", DbType.String, textIdentifier);
            command.AddInParameter("LanguageCode", DbType.String, languageCode);
            command.AddInParameter("TextValue", DbType.String, textValue);

            db.ExecuteNonQuery(command);

        }

        #endregion


        #region IConfigurationProvider Members

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Initialize the authentication provider with the supplied configuration object.
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ConfigurationBase config)
        {
            _config = (MultiLanguageTextProviderData)config;

            _cacheManager = _config.CacheManagerName != string.Empty ? CacheFactory.GetCacheManager(_config.CacheManagerName) : null;
            
            //bulkInit();
        }

        #endregion

        #region IImportExportTextProvider Members

        /// <summary>
        /// Export the localized texts to the specified text writer
        /// The exported text is formatted as XML.
        /// </summary>
        /// <param name="writer">Writer to write text data to.</param>
        public void ExportAllTexts(TextWriter writer)
        {
            //Not implemented due to general uselessness since no screens are hooked to this method

            //Database db = DatabaseFactory.CreateDatabase();
            //DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetAll");
            //DataSet ds = db.ExecuteDataSet(command);

            ////Start the XML document
            //writer.WriteLine("<textExport>");

            ////There should be 3 tables
            ////  The first is all language codes
            ////  The second is all text ids
            ////  The third is the entire text table
            //if (ds.Tables.Count == 3)
            //{
            //    WriteTextToXml(writer, ds.Tables[2]);
            //}

            //writer.WriteLine("</textExport>");
        }

        /// <summary>
        /// Export texts which contain the specified partial text ids and languages.
        /// The exported text is formatted as XML.
        /// </summary>
        /// <param name="writer">Writer to write text data to.</param>
        /// <param name="languageCodes">Language codes to export texts for.</param>
        /// <param name="partialTextIds">Partial text ids to filter on.</param>
        public void ExportFilteredTexts(TextWriter writer, string[] languageCodes, params string[] partialTextIds)
        {
            ExportTextsSubSet(writer, languageCodes, partialTextIds, false);
        }

        /// <summary>
        /// Export texts which exactly match the specified text ids and languages.
        /// The exported text is formatted as XML.
        /// </summary>
        /// <param name="writer">Writer to write text data to.</param>
        /// <param name="languageCodes">Language codes to export texts for.</param>
        /// <param name="textIds">Text ids to filter on.</param>
        public void ExportTextsById(TextWriter writer, string[] languageCodes, string[] textIds)
        {
            ExportTextsSubSet(writer, languageCodes, textIds, true);
        }

        /// <summary>
        /// Exports a sub set of all text ids in XML format.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="languageCodes"></param>
        /// <param name="textIds"></param>
        /// <param name="getById"></param>
        private void ExportTextsSubSet(TextWriter writer,
                                              string[] languageCodes,
                                              IEnumerable<string> textIds,
                                              bool getById)
        {
            if (languageCodes == null || languageCodes.Length == 0)
            {
                languageCodes = TextManager.ApplicationLanguages;
            }

            writer.WriteLine("<textExport>");

            Database db = DatabaseFactory.CreateDatabase();

            foreach (string id in textIds)
            {
                if (getById)
                {
                    //Write text element and ID
                    writer.WriteLine("  <text>");
                    writer.WriteLine("    <textId>{0}</textId>", id);

                    foreach (string languageCode in languageCodes)
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetByID");
                        command.AddInParameter("LanguageCode", DbType.String, languageCode);
                        command.AddInParameter("TextID", DbType.String, id);

                        string textValue = string.Empty;

                        using (IDataReader reader = db.ExecuteReader(command))
                        {
                            try
                            {
                                if (reader.Read())
                                {
                                    textValue = DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty);
                                }
                            }
                            finally
                            {
                                reader.Close();
                            }
                        }

                        //Write text value
                        writer.WriteLine("    <{0}><![CDATA[{1}]]></{2}>", languageCode, textValue.Replace("<![CDATA[", "").Replace("]]>", ""), languageCode);

                    }

                    writer.WriteLine("  </text>");

                }
                else
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetFiltered");
                    command.AddInParameter("PartialTextID", DbType.String, id);
                    DataSet ds = db.ExecuteDataSet(command);

                    if (ds != null && ds.Tables.Count == 1)
                    {
                        WriteTextToXml(writer, ds.Tables[0], languageCodes);
                    }
                }
            }

            writer.WriteLine("</textExport>");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="dtText"></param>
        /// <param name="languageCodes"></param>
        private static void WriteTextToXml(TextWriter writer, DataTable dtText, string[] languageCodes)
        {
            //Build dictionary of text ids
            var textList = 
                new Dictionary<string,List<LocalizedText>>(StringComparer.InvariantCultureIgnoreCase);

            DataRow[] textRows = dtText.Select();

            foreach (DataRow textRow in textRows)
            {
                string textId = DbUtility.GetValueFromDataRow(textRow, "TextId", string.Empty);
                string languageCode = DbUtility.GetValueFromDataRow(textRow, "LanguageCode", string.Empty);
                string textValue = DbUtility.GetValueFromDataRow(textRow, "TextValue", string.Empty).Replace("<![CDATA[", "").Replace("]]>", "");                        

                if (string.IsNullOrEmpty(textId) || string.IsNullOrEmpty(languageCode))
                {
                    continue;
                }

                var localizedText = new LocalizedText(
                    textId,
                    languageCode,
                    textValue);

                if (!textList.ContainsKey(textId))
                {
                    textList[textId] = new List<LocalizedText>();
                }

                textList[textId].Add(localizedText);
            }

            //Now write rows
            foreach (string textId in textList.Keys)
            {
                List<LocalizedText> textValueList = textList[textId];

                writer.WriteLine("<text>");
                writer.WriteLine("  <textId>{0}</textId>", textId);

                foreach (string languageCode in languageCodes)
                {
                    writer.Write("<{0}><![CDATA[", languageCode);

                    LocalizedText localizedtext = textValueList.Find(
                        lt => lt.LanguageCode.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase));

                    if (localizedtext != null)
                    {
                        writer.Write(localizedtext.TextValue);
                    }

                    writer.Write("]]></{0}>", languageCode);
                }

                writer.WriteLine("</text>");
            }
        }

        /// <summary>
        /// Import texts from a text reader.
        /// </summary>
        /// <param name="reader">Reader with text information.</param>
        public void ImportTexts(TextReader reader)
        {
            ArgumentValidation.CheckForNullReference(reader, "reader");
            ImportTextsFromXml(reader);
        }
        #endregion

        /// <summary>
        /// Import texts in Xml format from a text reader
        /// </summary>
        /// <param name="reader">Reader to import texts from.</param>
        private void ImportTextsFromXml(TextReader reader)
        {
            var doc = new XmlDocument();

            doc.Load(reader);

            XmlNodeList textNodes = doc.SelectNodes("/textExport/text");

            var importedTexts = new ArrayList();

            foreach (XmlNode textNode in textNodes)
            {
                string textId = string.Empty;

                //Get the text Id
                foreach (XmlNode childNode in textNode.ChildNodes)
                {
                    if (childNode.Name == "textId")
                    {
                        textId = childNode.InnerText;
                    }
                }

                //Get the language codes and text values				
                foreach (XmlNode childNode in textNode.ChildNodes)
                {
                    if (childNode.Name != "textId")
                    {
                        string languageCode = childNode.Name;
                        string textValue = childNode.InnerText;

                        if (textId != string.Empty && languageCode != string.Empty)
                        {
                            importedTexts.Add(new LocalizedText(textId, languageCode, textValue));
                        }
                    }
                }
            }


            foreach (LocalizedText localizedText in importedTexts)
            {
                SetText(localizedText.TextId, localizedText.LanguageCode, localizedText.TextValue);
            }
        }

        /// <summary>
        /// Subclass for storing localized texts, used mainly for creating
        /// lists of text updates
        /// </summary>
        internal class LocalizedText
        {
            public LocalizedText(string textId, string languageCode, string textValue)
            {
                TextId = textId;
                LanguageCode = languageCode;
                TextValue = textValue;
            }

            public string TextId { get; set; }

            public string LanguageCode { get; set; }

            public string TextValue { get; set; }
        }
    }
}
