//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Contains factory methods for creating Database objects
    /// </summary>
    public static class DatabaseFactory
    {
        /// <summary>
        /// Default name for master database when running in multi db mode
        /// </summary>
        public const string MASTER_DB_NAME = "MultiMasterDB";

        /// <summary>
        /// Default database provider name
        /// </summary>
        public const string DEFAULT_PROVIDER_NAME = "CheckboxSql";
        private static Dictionary<string, string> _connectionStrings;
        private static IDataContextProvider _dataContextProvider;

        /// <summary>
        /// Initialize the provider with a data context provider
        /// </summary>
        /// <param name="dataContextProvider"></param>
        public static void Initialize(IDataContextProvider dataContextProvider)
        {
            _dataContextProvider = dataContextProvider;
        }

        /// <summary>
        /// Get the current data context.
        /// </summary>
        public static string CurrentDataContext
        {
            get
            {
                if (_dataContextProvider != null)
                {
                    return _dataContextProvider.ApplicationContext;
                }

                return null;
            }
        }

        /// <summary>
        /// Get the connection strings collection
        /// </summary>
        private static Dictionary<string, string> ConnectionStrings
        {
            get
            {
                if (_connectionStrings == null)
                {
                    _connectionStrings = new Dictionary<string, string>();
                }

                return _connectionStrings;
            }
        }


        /// <summary>
        /// Load connection strings for a given context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="databaseName"></param>
        private static string LoadConnectionString(string context, string databaseName)
        {
            try
            {
                //Attempt to get an instance of the "MultiMaster" datbase
                Database masterDb = CreateDatabase(MASTER_DB_NAME);

                if (masterDb != null)
                {
                    DBCommandWrapper command = masterDb.GetStoredProcCommandWrapper("ckbx_ListApplicationContextConnections");
                    command.AddInParameter("ContextName", DbType.String, context);

                    using (IDataReader reader = masterDb.ExecuteReader(command))
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                string contextName = DbUtility.GetValueFromDataReader(reader, "ContextName", string.Empty);
                                string instance = DbUtility.GetValueFromDataReader(reader, "DbInstanceName", string.Empty);
                                string connectionString = DbUtility.GetValueFromDataReader(reader, "ConnectionString", string.Empty);

                                if (string.Compare(contextName, context, true) == 0
                                    && string.Compare(databaseName, instance, true) == 0)
                                {
                                    return connectionString;
                                }
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Unable to load connection string information from database for multi-database mode for context {0}.  Error was {1}.", context, ex.Message));
            }

            return string.Empty;
        }



        /// <summary>
        /// Method for invoking a default Database object.  Reads default settings
        /// from the ConnectionSettings.config file.
        /// </summary>
        /// <example>
        /// <code>
        /// Database dbSvc = DatabaseFactory.CreateDatabase();
        /// </code>
        /// </example>
        /// <returns>Database</returns>
        /// <exception cref="Exception">
        /// <para>A error occured while reading the configuration.</para>
        /// </exception>
        public static Database CreateDatabase()
        {
            //Logger.Write("Getting an instance of the default Database provider.", "Info", 1, -1, Severity.Information);
            DatabaseProviderFactory factory = new DatabaseProviderFactory("databaseProviderFactory", (DatabaseConfiguration)ConfigurationManager.GetConfiguration("checkboxDatabaseConfiguration"));
            Database provider = factory.CreateDefaultDatabase();

            //Initialize the connection strings for the provider
            InitializeProviderConnectionString(provider, string.Empty, DEFAULT_PROVIDER_NAME);

            return provider;
        }

        /// <summary>
        /// Method for invoking a specified Database service object.  Reads service settings
        /// from the ConnectionSettings.config file.
        /// </summary>
        /// <example>
        /// <code>
        /// Database dbSvc = DatabaseFactory.CreateDatabase("SQL_Customers");
        /// </code>
        /// </example>
        /// <param name="instanceName">configuration key for database service</param>
        /// <returns>Database</returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        /// <para><paramref name="instanceName"/> is not defined in configuration.</para>
        /// <para>- or -</para>
        /// <para>An error exists in the configuration.</para>
        /// <para>- or -</para>
        /// <para>A error occured while reading the configuration.</para>        
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        /// <para>The constructor being called throws an exception.</para>
        /// </exception>
        public static Database CreateDatabase(string instanceName)
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                return CreateDatabase();
            }

            //Logger.Write("Getting an instance of the default Database provider.", "Info", 1, -1, Severity.Information);
            DatabaseProviderFactory factory = new DatabaseProviderFactory("databaseProviderFactory", (DatabaseConfiguration)ConfigurationManager.GetConfiguration("checkboxDatabaseConfiguration"));
            Database provider = factory.CreateDatabase(instanceName);

            InitializeProviderConnectionString(provider, string.Empty, instanceName);

            return provider;
        }

        /// <summary>
        /// Get a database object for the specified application and instance.
        /// </summary>
        /// <param name="applicationContext"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static Database CreateDatabase(string applicationContext, string instanceName)
        {
            if (string.IsNullOrEmpty(applicationContext)
                && string.IsNullOrEmpty(instanceName))
            {
                return CreateDatabase();
            }

            if (string.IsNullOrEmpty(applicationContext))
            {
                return CreateDatabase(instanceName);
            }

            //Logger.Write("Getting an instance of the default Database provider.", "Info", 1, -1, Severity.Information);
            DatabaseProviderFactory factory = new DatabaseProviderFactory("databaseProviderFactory", (DatabaseConfiguration)ConfigurationManager.GetConfiguration("checkboxDatabaseConfiguration"));
            Database provider = factory.CreateDatabase(instanceName);

            InitializeProviderConnectionString(provider, applicationContext, instanceName);

            return provider;

        }

        /// <summary>
        /// Initialize database provider connection strings.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="applicationContext"></param>
        /// <param name="instanceName"></param>
        private static void InitializeProviderConnectionString(Database provider, string applicationContext, string instanceName)
        {
            //Load connection strings for the provider when NOT accessing the multi-master db
            if (string.Compare(instanceName, MASTER_DB_NAME, true) != 0)
            {
                //Figure out application context to use
                string contextToUse = string.IsNullOrEmpty(applicationContext) && _dataContextProvider != null ?
                    _dataContextProvider.ApplicationContext : applicationContext;

                if (!string.IsNullOrEmpty(contextToUse))
                {
                    string cacheKey = contextToUse + "_" + instanceName;

                    //Reload strings if:
                    //  a) Context connection strings dictionary is not present
                    //  b) Context connection strings dictionary is empty
                    //  c) Context connection strings dictionary has no key for the db instance
                    if (!ConnectionStrings.ContainsKey(cacheKey)
                        || string.IsNullOrEmpty(ConnectionStrings[cacheKey]))
                    {
                        ConnectionStrings[cacheKey] = LoadConnectionString(contextToUse, instanceName);
                    }

                    if (!ConnectionStrings.ContainsKey(cacheKey))
                    {
                        throw new Exception(string.Format("Unable to determine connection string for instance {0} and database {1}.", instanceName, contextToUse));
                    }

                    //Initialize the provider with the connection strings
                    provider.ConnectionString = ConnectionStrings[cacheKey];
                }
            }
        }
    }
}