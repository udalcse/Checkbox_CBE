/****************************************************************************
 * DefaultLanguageTextProvider.cs                                           *
 * Text provider that only supports the application's default language.     *
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

using Prezza.Framework.Caching;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Data;

namespace Checkbox.Globalization.Text.Providers
{
    /// <summary>
    /// Database authentication provider
    /// </summary>
    public class DefaultLanguageTextProvider : ITextProvider
    {
        /// <summary>
        /// Provider configuration
        /// </summary>
        private DefaultLanguageTextProviderData _config;

        /// <summary>
        /// Provider name.
        /// </summary>
        private string _configurationName;

        /// <summary>
        /// Cache manager for texts
        /// </summary>
        private CacheManager _cacheManager;

        #region ITextProvider Members

        /// <summary>
        /// Gets the specified text only using the default language
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllTexts(string textIdentifier)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string text = GetText(textIdentifier, _config.DefaultLanguage);

            if (text != null)
            {
                dictionary.Add(_config.DefaultLanguage, text);
            }

            return dictionary;
        }

        /// <summary>
        /// Get all text data for 
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <returns></returns>
        public DataTable GetTextData(string textIdentifier)
        {
            DataTable t = new DataTable();
            t.Columns.Add("TextID", typeof(string));
            t.Columns.Add("LanguageCode", typeof(string));
            t.Columns.Add("TextValue", typeof(string));

            string text = GetText(textIdentifier, _config.DefaultLanguage);

            if (text == null)
            {
                text = string.Empty;
            }

            t.Rows.Add(textIdentifier, _config.DefaultLanguage, text);

            t.AcceptChanges();

            return t;
        }

        /// <summary>
        /// Get the specified text, but use the default language instead of the specified language.
        /// </summary>
        /// <param name="textIdentifier">Id of text to retrieve.</param>
        /// <param name="languageCode">(ignored)</param>
        /// <returns>Requested string</returns>
        public string GetText(string textIdentifier, string languageCode)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

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
            string textValue = string.Empty;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetText");
            command.AddInParameter("TextID", DbType.String, textIdentifier);
            command.AddInParameter("LanguageCode", DbType.String, _config.DefaultLanguage);

            object scalar = db.ExecuteScalar(command);

            if (scalar != DBNull.Value)
            {
                textValue = (string)scalar;
            }

            textValue = textValue ?? string.Empty;

            if (_cacheManager != null)
            {
                //_cacheManager.Remove(key);
                _cacheManager.Add(key, textValue);
            }

            return textValue;
        }

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
        /// Store the localized text associated with a given text id
        /// </summary>
        /// <param name="textIdentifier"></param>
        /// <param name="languageCode"></param>
        /// <param name="textValue"></param>
        public void SetText(string textIdentifier, string languageCode, string textValue)
        {
            ArgumentValidation.CheckForEmptyString(textIdentifier, "textIdentifier");

            if (textValue == null)
            {
                textValue = string.Empty;
            }

            //Set the item in the cache
            if (_cacheManager != null)
            {
                //TODO:  Handle expiration
                _cacheManager.Add(textIdentifier.ToLower(), textValue);
            }

            //Update the database
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_Set");
            command.AddInParameter("TextID", DbType.String, textIdentifier);
            command.AddInParameter("LanguageCode", DbType.String, _config.DefaultLanguage);
            command.AddInParameter("TextValue", DbType.String, textValue);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Get a data table containing matching texts
        /// </summary>
        /// <param name="matchExpressions"></param>
        /// <returns></returns>
        public DataTable GetMatchingTextData(params string[] matchExpressions)
        {
            DataTable returnTable = new DataTable();
            returnTable.TableName = "MatchingTexts";
            returnTable.Columns.Add("TextId", typeof(string));
            returnTable.Columns.Add("LanguageCode", typeof(string));
            returnTable.Columns.Add("TextValue", typeof(string));

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Text_GetAll");
            DataSet ds = db.ExecuteDataSet(command);

            //There should be 3 tables
            //  The first is all language codes
            //  The second is all text ids
            //  The third is the entire text table
            if (ds.Tables.Count == 3)
            {
                DataTable dtLang = ds.Tables[0];
                DataTable dtId = ds.Tables[1];
                DataTable dtText = ds.Tables[2];

                foreach (DataRow drId in dtId.Rows)
                {
                    string textId = (string)drId["TextId"];

                    foreach (string matchExpression in matchExpressions)
                    {
                        Regex expr = new Regex(matchExpression);

                        Match m = expr.Match(textId);

                        if (m.Success)
                        {
                            string languageCode = _config.DefaultLanguage;
                            string textValue;

                            //Get the text value for this language & id
                            DataRow[] drTexts = dtText.Select("LanguageCode = '" + languageCode + "' AND TextId = '" + textId + "'");

                            if (drTexts.Length > 0)
                            {
                                textValue = (string)drTexts[0]["TextValue"];
                            }
                            else
                            {
                                textValue = string.Empty;
                            }

                            DataRow dr = returnTable.NewRow();
                            dr["TextId"] = textId;
                            dr["LanguageCode"] = languageCode;
                            dr["TextValue"] = textValue;

                            returnTable.Rows.Add(dr);
                        }
                    }
                }
            }

            return returnTable;
        }

        #endregion



        #region IConfigurationProvider Members

        /// <summary>
        /// Name of the provider.
        /// </summary>
        public string ConfigurationName
        {
            get { return _configurationName; }
            set { _configurationName = value; }
        }

        /// <summary>
        /// Initialize the authentication provider with the supplied configuration object.
        /// </summary>
        /// <param name="config"></param>
        public void Initialize(ConfigurationBase config)
        {
            _config = (DefaultLanguageTextProviderData)config;

            if (_config.CacheManagerName != string.Empty)
            {
                _cacheManager = CacheFactory.GetCacheManager(_config.CacheManagerName);
                //bulkInit();
            }
            else
                _cacheManager = null;
        }

        /// <summary>
        /// Read all texts for all languages and put them into the cache
        /// </summary>
        private void bulkInit()
        {
            _cacheManager = CacheFactory.GetCacheManager(_config.CacheManagerName);

            DataTable returnTable = new DataTable();
            returnTable.TableName = "MatchingTexts";
            returnTable.Columns.Add("TextId", typeof(string));
            returnTable.Columns.Add("LanguageCode", typeof(string));
            returnTable.Columns.Add("TextValue", typeof(string));

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
                    string textId = (string)dr["TextId"];
                    string languageCode = (string)dr["LanguageCode"];
                    string textValue = (string)dr["TextValue"];

                    var key = getCacheKey(textId, languageCode);
                    //_cacheManager.Remove(key);
                    _cacheManager.Add(key, textValue);
                }
            }
        }

        #endregion
    }
}
