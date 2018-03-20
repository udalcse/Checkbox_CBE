using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using Prezza.Framework.Data;
using Prezza.Framework.Caching;

namespace Checkbox.Management
{
    /// <summary>
    /// Handles operations relating to the writing and retrieval of application-wide settings
    /// </summary>
    public static class ApplicationManager
    {
        private static CacheManager _pageIdCacheManager;
        private static readonly Hashtable _settingsCache;
        private static readonly AppSettings _appSettings;
        private static IDataContextProvider _dataContextProvider;
        private static double? _serversTimeZone;

        //Store cache settings locally to prevent repeated access to check for setting
        private static bool? _cacheAppSettings;

        private class CacheObject<T>
        {
            public CacheObject(T value, DateTime expires)
            {
                Value = value;
                Expires = expires;
            }

            public T Value { get; private set; }
            public DateTime Expires { get; private set; } 
        }

        /// <summary>
        /// 
        /// </summary>
        static ApplicationManager()
        {
            lock (typeof(ApplicationManager))
            {
                _settingsCache = new Hashtable();
                _appSettings = new AppSettings();
                _pageIdCacheManager = CacheFactory.GetCacheManager("applicationPageIds");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void FlushSettingsCache()
        {
            lock (typeof(ApplicationManager))
            {
                if (_settingsCache != null)
                {
                    _settingsCache.Clear();
                }
            }
        }

        /// <summary>
        /// Initialize the application manager with a data context provider
        /// </summary>
        /// <param name="dataContextProvider"></param>
        public static void SetApplicationDataContextProvider(IDataContextProvider dataContextProvider)
        {
            _dataContextProvider = dataContextProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsDataContextTrial
        {
            get
            {
                //If not multi-db or no data context provider, trial = false
                if (!AppSettings.EnableMultiDatabase || _dataContextProvider == null)
                    return false;

                string cacheKey = "ContextTrial_" + _dataContextProvider.ApplicationContext;

                // First we check whether the cache item has expired, if expired we will check the database
                if (_settingsCache != null && _settingsCache.ContainsKey(cacheKey))
                {
                    var trialCacheObj = (CacheObject<bool>)_settingsCache[cacheKey];
                    
                    // if the cache object has not yet expired we should return the IsTrial value
                    // if IsTrial is false the cache object should have an expiration value of DateTime.MaxValue
                    if (DateTime.Now < trialCacheObj.Expires)
                        return trialCacheObj.Value;
                }

                var contextType = -1; // contextType of 1 is a trial

                //Otherwise, check the db
                try
                {
                    var db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                    var command = db.GetStoredProcCommandWrapper("ckbx_GetApplicationContextDetails");
                    command.AddInParameter("ContextName", DbType.String, _dataContextProvider.ApplicationContext);

                    using (var reader = db.ExecuteReader(command))
                    {
                        if (reader.Read())
                        {
                            contextType = DbUtility.GetValueFromDataReader(reader, "ContextType", -1);
                        }
                    }
                }
                catch
                {
                }

                if (_settingsCache != null)
                {
                    lock (_settingsCache.SyncRoot)
                    {
                        switch (contextType)
                        {
                            case 1:
                                _settingsCache[cacheKey] = new CacheObject<bool>(true, DateTime.Now.AddMinutes(5));
                                return true;
                            default:
                                _settingsCache[cacheKey] = new CacheObject<bool>(false, DateTime.MaxValue);
                                break;
                        }
                    }
                }

                return false;
            }
        }
        

        /// <summary>
        /// Get a boolean indicating if the data context is "active"
        /// </summary>
        public static bool IsDataContextActive
        {
            get
            {
                //If not multi-db or no data context provider, active = true
                if (!AppSettings.EnableMultiDatabase || _dataContextProvider == null)
                    return true;

                string cacheKey = "ContextActive_" + _dataContextProvider.ApplicationContext;

                //Check cache first, since app pool is periodically recycled, store "true" value.  Simply having the value in the cache
                // indicates application is active.
                if (_settingsCache != null && _settingsCache.ContainsKey(cacheKey))
                {
                    var isActiveCacheObj = (CacheObject<bool>)_settingsCache[cacheKey];
                    
                    // if the cache object has not yet expired we should return the IsActive value
                    // if IsTrial is false the cache object should have an expiration value of DateTime.MaxValue
                    if (DateTime.Now < isActiveCacheObj.Expires)
                        return isActiveCacheObj.Value;
                }

                var isActive = false;

                //Otherwise, check the db
                try
                {
                    var db = DatabaseFactory.CreateDatabase(DatabaseFactory.MASTER_DB_NAME);
                    var command = db.GetStoredProcCommandWrapper("ckbx_GetApplicationContext");
                    command.AddInParameter("ContextName", DbType.String, _dataContextProvider.ApplicationContext);

                    using (var reader = db.ExecuteReader(command))
                    {
                        if (reader.Read())
                        {
                            isActive = DbUtility.GetValueFromDataReader(reader, "Active", false);
                        }
                    } //try-finally block is redundant when IDataReader is in a using statement
                }
                catch
                {
                }

                //Only set flat if context is active
                if (isActive && _settingsCache != null)
                {
                    lock (_settingsCache.SyncRoot)
                    {
                        _settingsCache[cacheKey] = new CacheObject<bool>(true, DateTime.Now.AddDays(1));
                    }
                }

                //If we can't tell, return false
                return isActive;
            }
        }        

        /// <summary>
        /// Get Server's time zone
        /// </summary>
        public static double ServersTimeZone
        {
            get
            {
                if (!_serversTimeZone.HasValue)
                {
                    var timeZone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                    _serversTimeZone = timeZone.Hours + (timeZone.Minutes / 60.0);
                }

                return _serversTimeZone.Value;
            }
        }

        /// <summary>
        /// Get/set whether to use Simple Security or not
        /// </summary>
        public static bool UseSimpleSecurity 
        {
            get { return AppSettings.UseSimpleSecurity || AppSettings.SimpleSecurityDebugMode; }
        }

        /// <summary>
        /// Get a concatentated version of application url + application root
        /// </summary>
        public static string ApplicationPath
        {
            get { return ApplicationURL + ApplicationRoot; }
        }

        /// <summary>
        /// Gets and sets the URL of Application
        /// </summary>
        public static string ApplicationURL
        {
            get
            {
                if (_dataContextProvider != null)
                {
                    return string.Format(_dataContextProvider.Secured ? "https://{0}" : "http://{0}", _dataContextProvider.ApplicationContext);
                }

                var appUrl = ConfigurationManager.AppSettings["ApplicationURL"];

                if (!string.IsNullOrWhiteSpace(appUrl) && appUrl.Last().Equals('/'))
                    appUrl = appUrl.Substring(0, appUrl.Length - 1);

                return appUrl;
            }
        }


        /// <summary>
        /// Gets the application host.
        /// </summary>
        /// <value>
        /// The application host.
        /// </value>
        public static string ApplicationHost
        {
            get
            {
                //Ignore web.config setting if data context provider set
                return $"http{((HttpContext.Current.Request.IsSecureConnection) ? "s" : "")}://{HttpContext.Current.Request.Url.Host}";
            }
        }
        /// <summary>
        /// Gets and sets the ApplicationRoot property
        /// </summary>
        public static string ApplicationRoot
        {
            get
            {
                //Ignore web.config setting if data context provider set
                return _dataContextProvider != null 
                    ? string.Empty 
                    : ConfigurationManager.AppSettings["ApplicationRoot"];
            }
        }

        /// <summary>
        /// Get the current application data context.
        /// </summary>
        public static string ApplicationDataContext
        {
            get
            {
                if (_dataContextProvider != null)
                {
                    return _dataContextProvider.ApplicationContext ?? string.Empty;
                }

                return string.Empty;
            }
        }


        /// <summary>
        /// Gets the <see cref="AppSettings"/> for this application
        /// </summary>
        public static AppSettings AppSettings
        {
            get
            {
                return _appSettings;
            }
        }

        /// <summary>
        /// Get boolean indicating if app settings should be cached
        /// </summary>
        public static bool CacheAppSettings
        {
            get
            {
                if (!_cacheAppSettings.HasValue)
                {
                    _cacheAppSettings = AppSettings.CacheAppSettings;
                }

                return _cacheAppSettings.Value;
            }
        }

        /// <summary>
        /// Overloaded. Creates a new key/value pairing in the appsettings of the web.config file for this application
        /// </summary>
        /// <param name="key">the name of the key</param>
        /// <param name="property">the value of the key</param>
        public static void AddNewAppSetting(string key, string property)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Setting_Insert");
            command.AddInParameter("SettingName", DbType.String, key);
            command.AddInParameter("SettingValue", DbType.String, property);

            db.ExecuteNonQuery(command);

            //Update the cache
            if (CacheAppSettings && !string.IsNullOrEmpty(key))
            {
                lock (_settingsCache.SyncRoot)
                {
                    _settingsCache[GetSettingsCacheKey(key)] = property;
                }
            }
        }

        /// <summary>
        /// Overloaded. Updates a setting in the web.config file for a given key
        /// </summary>
        /// <param name="key">the key to update</param>
        /// <param name="property">the new value</param>
        internal static void UpdateAppSetting(string key, string property)
        {
            var db = DatabaseFactory.CreateDatabase();

            var command = new SqlCommand()
                {
                    Connection = (SqlConnection) db.GetConnection(),
                    CommandText =
                        string.Format("UPDATE ckbx_Settings SET SettingValue = '{0}' WHERE SettingName = '{1}'", property,
                                      key),
                    CommandType = CommandType.Text,
                };

            command.Connection.Open();
            command.ExecuteScalar();
            command.Connection.Close();

            //Update the cache
            if (CacheAppSettings && !string.IsNullOrEmpty(key))
            {
                lock (_settingsCache.SyncRoot)
                {
                    _settingsCache[GetSettingsCacheKey(key)] = property;
                }
            }
        }

        /// <summary>
        /// Overloaded. Updates a setting in the web.config file for a given key
        /// </summary>
        /// <param name="key">the key to update</param>
        /// <param name="property">the new value</param>
        /// <param name="path">the path of the web.config file</param>
        public static void UpdateAppSetting(string key, string property, string path)
        {
            var config = new XmlDocument();
            config.Load(path);

            var ns = new XmlNamespaceManager(config.NameTable);
            ns.AddNamespace("ms", "http://schemas.microsoft.com/.NetConfiguration/v2.0");

            var appSettings = config.SelectSingleNode("//ms:configuration/ms:appSettings", ns);

            if (appSettings != null)
            {
                var setting = (XmlElement)appSettings.SelectSingleNode("//ms:add[@key='" + key + "']", ns);
                if (setting != null)
                    setting.SetAttribute("value", property);
            }

            config.Save(path);

        }

        /// <summary>
        /// Get the specified application setting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string GetAppSetting(string key)
        {
            //First, check the cache
            if (CacheAppSettings && !string.IsNullOrEmpty(key) && _settingsCache.Contains(GetSettingsCacheKey(key)))
            {
                var returnValue = _settingsCache[GetSettingsCacheKey(key)];

                if (returnValue == null || returnValue == DBNull.Value)
                {
                    return null;
                }

                return returnValue.ToString();
            }
            
            //Get the value from the database
            var db = DatabaseFactory.CreateDatabase();

            var command = new SqlCommand()
                {
                    Connection =  (SqlConnection)db.GetConnection(),
                    CommandText = string.Format("SELECT SettingValue FROM ckbx_Settings WHERE SettingName = '{0}'", key),
                    CommandType = CommandType.Text
                };
            
            string settingValue = null;

            try
            {
                command.Connection.Open();
                var res = command.ExecuteScalar();
                if (res != null && res != DBNull.Value)
                    settingValue = res.ToString();
                command.Connection.Close();
            }
            catch (Exception)
            {
                return null;
            }

            //Store the value in the cache
            if (CacheAppSettings && !string.IsNullOrEmpty(key))
            {
                lock (_settingsCache.SyncRoot)
                {
                    _settingsCache[GetSettingsCacheKey(key)] = settingValue;
                }
            }

            return settingValue;
        }

        /// <summary>
        /// Get the cache key for a setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        private static string GetSettingsCacheKey(string settingName)
        {
            return (ApplicationDataContext + "_" + settingName).ToLower();
        }

        /// <summary>
        /// Get id of application page from its path
        /// </summary>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public static int GetPageId(string pagePath)
        {
            if (_pageIdCacheManager.Contains(pagePath))
            {
                return (int)_pageIdCacheManager.GetData(pagePath);
            }

            var pageId = 0;

            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_PageId_Get");
            command.AddInParameter("PagePath", DbType.String, pagePath);

            using(var reader = db.ExecuteReader(command))
            {
                if (reader.Read())
                {
                    pageId = DbUtility.GetValueFromDataReader(reader, "PageId", -1);
                }
            }

            if (pageId > 0)
            {
                _pageIdCacheManager.Add(pagePath, pageId);
            }

            return pageId;
        }
    }
}
