//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Data.Common;
using System.Globalization;

using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Data.Configuration;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Represents an abstract database that commands can be run against. 
    /// </summary>
    public abstract class Database : ConfigurationProvider
    {
        private static readonly ParameterCache _parameterCache = new ParameterCache();
        private DatabaseProviderData _config;
        private string _connectionString;

        /// <summary>
        /// Initialize the database object with configuration data.
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(ConfigurationBase config)
        {
            _config = (DatabaseProviderData)config;
        }

        /// <summary>
        /// <para>Gets the string used to open a database.</para>
        /// <seealso cref="IDbConnection.ConnectionString"/>
        /// </summary>
        /// <value>
        /// <para>The string used to open a database.</para>
        /// </value>
        public string ConnectionString
        {
            get
            {
                //Use externally set connection string
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return _connectionString;
                }

                //Use configuration file string, but provide a better error if it can't be located.
                if (System.Configuration.ConfigurationManager.ConnectionStrings[_config.ConnectionStringName] == null)
                {
                    throw new Exception(string.Format("Unable to find connection string with name: {0} in the app config.", _config.ConnectionStringName));
                }

                return System.Configuration.ConfigurationManager.ConnectionStrings[_config.ConnectionStringName].ConnectionString;
            }
            
            set { _connectionString = value; }
        }

        /// <summary>
        /// <para>When implemented by a class, gets the parameter token used to delimit parameters for the database.</para>
        /// </summary>
        /// <value>
        /// <para>the parameter token used to delimit parameters for the database.</para>
        /// </value>
        protected abstract char ParameterToken { get; }

        /// <summary>
        /// <para>Gets the service key used to create the database instance.</para>
        /// </summary>
        /// <value>
        /// <para>The service key used to create the database instance.</para>
        /// </value>
        protected string ServiceKey
        {
            get { return DatabaseProviderData.Name; }
        }

        /// <summary>
        /// Gets the <see cref="DatabaseProviderData"/> from which
        /// this object was initialized.
        /// </summary>
        /// <value>A <see cref="DatabaseProviderData"/>.</value>
        protected DatabaseProviderData DatabaseProviderData
        {
            get { return _config; }
        }

        /// <summary>
        /// <para>When overridden in a derived class, gets the connection for this database.</para>
        /// <seealso cref="IDbConnection"/>        
        /// </summary>
        /// <returns>
        /// <para>The <see cref="IDbConnection"/> for this database.</para>
        /// </returns>
        public abstract IDbConnection GetConnection();

        /// <summary>
        /// <para>When overridden in a derived class, creates a <see cref="DBCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DBCommandWrapper"/> for the stored procedure.</para></returns>       
        public abstract DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName);

        /// <summary>
        /// <para>When overridden in a derived class, creates an <see cref="DBCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DBCommandWrapper"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        public abstract DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName, params object[] parameterValues);

        /// <summary>
        /// Wraps around a derived class's implementation of the GetStoredProcCommandWrapper method and adds functionality for
        /// using this method with UpdateDataSet.  The GetStoredProcCommandWrapper method (above) that takes a params array 
        /// expects the array to be filled with VALUES for the parameters. This method differs in that it allows a user to pass 
        /// in a string array and this method will dynamically discover the parameters for the stored procedure and set the 
        /// parameter's SourceColumns to the strings that are passed in.  It does this by mapping the parameters to the strings IN 
        /// ORDER.  Thus, order is very important.
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="sourceColumns"><para>The list of DataFields for the procedure.</para></param>
        /// <returns><para>The <see cref="DBCommandWrapper"/> for the stored procedure.</para></returns>
        public DBCommandWrapper GetStoredProcCommandWrapperWithSourceColumns(string storedProcedureName, params string[] sourceColumns)
        {
            ArgumentValidation.CheckForNullReference(storedProcedureName, "storedProcedureName");
            ArgumentValidation.CheckForNullReference(sourceColumns, "sourceColumns");

            DBCommandWrapper dbCommWrapper = GetStoredProcCommandWrapper(storedProcedureName);

            //we do not actually set the connection until the Fill or Update, so we need to temporarily do it here so we can set the sourcecolumns
            IDbCommand dbCommand = dbCommWrapper.Command;
            using (IDbConnection connection = GetConnection())
            {
                dbCommand.Connection = connection;
                dbCommWrapper.DiscoverParameters(ParameterToken);
            }

            int iSourceIndex = 0;
            foreach (IDataParameter dbParam in dbCommand.Parameters)
            {
                if ((dbParam.Direction == ParameterDirection.Input) | (dbParam.Direction == ParameterDirection.InputOutput))
                {
                    dbParam.SourceColumn = sourceColumns[iSourceIndex];
                    iSourceIndex++;
                }
            }

            return dbCommWrapper;
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates an <see cref="DBCommandWrapper"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DBCommandWrapper"/> for the SQL query.</para></returns>        
        public abstract DBCommandWrapper GetSqlStringCommandWrapper(string query);

        /// <summary>
        /// Gets the DbDataAdapter with the given update behavior and connection from the proper derived class.
        /// Created this new, public method instead of modifying the protected, abstract one so there will be no
        /// breaking changes for any currently derived Database class
        /// </summary>
        /// <returns>An <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        public DbDataAdapter GetDataAdapter()
        {
            return GetDataAdapter(UpdateBehavior.Standard);
        }

        /// <summary>
        /// Gets the DbDataAdapter with the given update behavior and connection from the proper derived class.
        /// Created this new, public method instead of modifying the protected, abstract one so there will be no
        /// breaking changes for any currently derived Database class
        /// </summary>
        /// <param name="behavior">
        /// <para>One of the <see cref="UpdateBehavior"/> values.</para>
        /// </param>
        /// <returns>An <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        public DbDataAdapter GetDataAdapter(UpdateBehavior behavior)
        {
            return GetDataAdapter(behavior, GetConnection());
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates a <see cref="DbDataAdapter"/> with the given update behavior and connection.</para>        
        /// </summary>
        /// <param name="behavior">
        /// <para>One of the <see cref="UpdateBehavior"/> values.</para>
        /// </param>
        /// <param name="connection">
        /// <para>The open connection to the database.</para>
        /// </param>
        /// <returns>An <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        protected abstract DbDataAdapter GetDataAdapter(UpdateBehavior behavior, IDbConnection connection);


        /// <summary>
        /// <para>Execute the <paramref name="command"/> and add a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see></para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DBCommandWrapper"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <seealso cref="DbDataAdapter.Fill(System.Data.DataSet)"/>
        /// <exception cref="System.ArgumentNullException">Any input parameter was null</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        public virtual void LoadDataSet(DBCommandWrapper command, DataSet dataSet, string tableName)
        {
            LoadDataSet(command, dataSet, new[] { tableName });
        }

        /// <summary>
        /// <para>Execute the <paramref name="command"/> within the given <paramref name="transaction" /> and add a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see></para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DBCommandWrapper"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="DbDataAdapter.Fill(System.Data.DataSet)"/>
        /// <exception cref="System.ArgumentNullException">Any input parameter was null</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        public virtual void LoadDataSet(DBCommandWrapper command, DataSet dataSet, string tableName, IDbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new[] { tableName }, transaction);
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from a <see cref="DBCommandWrapper"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(DBCommandWrapper command, DataSet dataSet, string[] tableNames)
        {
            try
            {
                using (IDbConnection connection = GetConnection())
                {
                    PrepareCommand(command, connection);
                    DoLoadDataSet(command, dataSet, tableNames);
                }
            }
            catch (ConstraintException ex)
            {
                if (dataSet == null)
                {
                    throw;
                }
                //Attempt to provide more useful error message context for contraint violations
                foreach (DataTable table in dataSet.Tables)
                {
                    // Test if the table has errors. If not, skip it.
                    if (table.HasErrors)
                    {
                        // Get an array of all rows with errors.
                        DataRow[] rowsInError = table.GetErrors();
                        
                        // Print the error of each column in each row.
                        for (int i = 0; i < rowsInError.Length; i++)
                        {
                            ex.Data[string.Format("{0}_{1}", table.TableName, i)] = rowsInError[i].RowError;
                        }
                    }
                }

                //Throw modified exception
                throw ex;
            }
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from a <see cref="DBCommandWrapper"/> in  a transaction.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        public virtual void LoadDataSet(DBCommandWrapper command, DataSet dataSet, string[] tableNames, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from a <see cref="DBCommandWrapper"/></para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        public virtual void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                LoadDataSet(wrapper, dataSet, tableNames);
            }
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from a stored procedure in  a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the stored procedure in.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        public virtual void LoadDataSet(IDbTransaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                LoadDataSet(wrapper, dataSet, tableNames, transaction);
            }
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from command text.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                LoadDataSet(wrapper, dataSet, tableNames);
            }
        }

        /// <summary>
        /// <para>Load a <see cref="DataSet"/> from command text in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(IDbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                LoadDataSet(wrapper, dataSet, tableNames, transaction);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="command"/> and return the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DBCommandWrapper"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>
        /// <seealso cref="DbDataAdapter.Fill(System.Data.DataSet)"/>
        public virtual DataSet ExecuteDataSet(DBCommandWrapper command)
        {
            DataSet dataSet = new DataSet {Locale = CultureInfo.InvariantCulture};
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        /// <summary>
        /// <para>Execute the <paramref name="command"/> as part of the <paramref name="transaction" /> and return the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DBCommandWrapper"/> to execute.</para></param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>
        /// <seealso cref="DbDataAdapter.Fill(System.Data.DataSet)"/>
        public virtual DataSet ExecuteDataSet(DBCommandWrapper command, IDbTransaction transaction)
        {
            DataSet dataSet = new DataSet {Locale = CultureInfo.InvariantCulture};
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }

        /// <summary>
        /// <para>Execute the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> and return the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        public virtual DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteDataSet(wrapper);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="storedProcedureName"/> ith <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and return the results in a new <see cref="DataSet"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        public virtual DataSet ExecuteDataSet(IDbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteDataSet(wrapper, transaction);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and return the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(wrapper);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and return the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataSet ExecuteDataSet(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(wrapper, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DBCommandWrapper command)
        {
            using (IDbConnection connection = OpenConnection())
            {
                PrepareCommand(command, connection);
                return DoExecuteScalar(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DBCommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(string storedProcedureName, params Object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(wrapper);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(IDbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(wrapper, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteScalar(wrapper);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the resultset returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the resultset.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteScalar(wrapper, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        /// <returns>Number of rows affected by the command.</returns>
        public virtual int ExecuteNonQuery(DBCommandWrapper command)
        {
            using (IDbConnection connection = OpenConnection())
            {
                PrepareCommand(command, connection);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        /// <returns>Number of rows affected by the command.</returns>
        public virtual int ExecuteNonQuery(DBCommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                ExecuteNonQuery(wrapper);
                return wrapper.RowsAffected;
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                ExecuteNonQuery(wrapper, transaction);

                return wrapper.RowsAffected;
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and return the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                ExecuteNonQuery(wrapper);

                return wrapper.RowsAffected;
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and return the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                ExecuteNonQuery(wrapper, transaction);

                return wrapper.RowsAffected;
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public virtual IDataReader ExecuteReader(DBCommandWrapper command)
        {
            IDbConnection connection = OpenConnection();
            PrepareCommand(command, connection);

            try
            {
                return DoExecuteReader(command.Command, CommandBehavior.CloseConnection);
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a transaction and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public virtual IDataReader ExecuteReader(DBCommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteReader(command.Command, CommandBehavior.Default);
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>        
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteReader(wrapper);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within the given <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public IDataReader ExecuteReader(IDbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DBCommandWrapper wrapper = GetStoredProcCommandWrapper(storedProcedureName, parameterValues))
            {
                return ExecuteReader(wrapper, transaction);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteReader(wrapper);
            }
        }

        /// <summary>
        /// <para>Execute the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> within the given 
        /// <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>
        public IDataReader ExecuteReader(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DBCommandWrapper wrapper = CreateCommandWrapperByCommandType(commandType, commandText))
            {
                return ExecuteReader(wrapper, transaction);
            }
        }
        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, UpdateBehavior updateBehavior)
        {
            return UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, 1, updateBehavior); 
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, IDbTransaction transaction)
        {
            return UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction, 1);
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="rowFilter">A DataViewRowStat to filter upon</param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, IDbTransaction transaction, DataViewRowState rowFilter)
        {
            return UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, transaction, 1, rowFilter);
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="updateBatchSize"><para>The BatchSize for the update</para></param>
        /// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, int updateBatchSize, UpdateBehavior updateBehavior)
        {
            using (IDbConnection connection = OpenConnection())
            {
                if (updateBehavior == UpdateBehavior.Transactional)
                {
                    IDbTransaction trans = BeginTransaction(connection);
                    try
                    {
                        int rowsAffected = UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, trans);
                        CommitTransaction(trans);
                        return rowsAffected;
                    }
                    catch
                    {
                        RollbackTransaction(trans);
                        throw;
                    }
                }
                
                if (insertCommand != null)
                {
                    PrepareCommand(insertCommand, connection);
                }
                if (updateCommand != null)
                {
                    PrepareCommand(updateCommand, connection);
                }
                if (deleteCommand != null)
                {
                    PrepareCommand(deleteCommand, connection);
                }

                return DoUpdateDataSet(updateBehavior, connection, dataSet, tableName,
                                       insertCommand, updateCommand, deleteCommand, updateBatchSize, DataViewRowState.CurrentRows | DataViewRowState.Deleted);
            }
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize"><para>The BatchSize for the update</para></param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, IDbTransaction transaction, int updateBatchSize)
        {
            if (insertCommand != null)
            {
                PrepareCommand(insertCommand, transaction);
            }
            if (updateCommand != null)
            {
                PrepareCommand(updateCommand, transaction);
            }
            if (deleteCommand != null)
            {
                PrepareCommand(deleteCommand, transaction);
            }

            return DoUpdateDataSet(UpdateBehavior.Transactional, transaction.Connection,
                                   dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize, DataViewRowState.CurrentRows | DataViewRowState.Deleted);
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
        /// <param name="updateCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DBCommandWrapper"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
        /// <param name="transaction"><para>The <see cref="IDbTransaction"/> to use.</para></param>
        /// <param name="updateBatchSize"><para>The BatchSize for the update</para></param>
        /// <param name="rowFilter">A DataViewRowStat to filter upon</param>
        /// <returns>number of records affected</returns>
        /// <seealso cref="DbDataAdapter.Update(System.Data.DataSet)"/>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName,
                                         DBCommandWrapper insertCommand, DBCommandWrapper updateCommand,
                                         DBCommandWrapper deleteCommand, IDbTransaction transaction, int updateBatchSize, DataViewRowState rowFilter)
        {
            if (insertCommand != null)
            {
                PrepareCommand(insertCommand, transaction);
            }
            if (updateCommand != null)
            {
                PrepareCommand(updateCommand, transaction);
            }
            if (deleteCommand != null)
            {
                PrepareCommand(deleteCommand, transaction);
            }

            return DoUpdateDataSet(UpdateBehavior.Transactional, transaction.Connection,
                                   dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize, rowFilter);
        }

        /// <summary>
        /// Clears the parameter cache. Since there is only one parameter cache that is shared by all instances
        /// of this class, this clears all parameters cached for all databases.
        /// </summary>
        public void ClearParameterCache()
        {
            _parameterCache.Clear();
        }

        /// <summary>
        /// <para>Assigns a <paramref name="connection"/> to the <paramref name="command"/> and discovers parameters if needed.</para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to prepare.</para></param>
        /// <param name="connection">The connection to assign to the command.</param>
        protected void PrepareCommand(DBCommandWrapper command, IDbConnection connection)
        {
            ArgumentValidation.CheckForNullReference(command, "command");
            ArgumentValidation.CheckForNullReference(connection, "connection");

            command.Command.Connection = connection;
            if (command.IsFurtherPreparationNeeded())
            {
                _parameterCache.FillParameters(command, ParameterToken);
            }
        }

        /// <summary>
        /// <para>Assigns a <paramref name="transaction"/> to the <paramref name="command"/> and discovers parameters if needed.</para>
        /// </summary>
        /// <param name="command"><para>The command that contains the query to prepare.</para></param>
        /// <param name="transaction">The transaction to assign to the command.</param>
        protected void PrepareCommand(DBCommandWrapper command, IDbTransaction transaction)
        {
            ArgumentValidation.CheckForNullReference(command, "command");
            ArgumentValidation.CheckForNullReference(transaction, "transaction");

            command.Command.Transaction = transaction;
            PrepareCommand(command, transaction.Connection);
        }

        /// <summary>
        /// <para>Open a connection.</para>
        /// </summary>
        /// <returns>The opened connection.</returns>
        protected IDbConnection OpenConnection()
        {
            IDbConnection connection = GetConnection();
            connection.Open();
            return connection;
        }

        private DBCommandWrapper CreateCommandWrapperByCommandType(CommandType commandType, string commandText)
        {
            DBCommandWrapper wrapper = null;
            switch (commandType)
            {
                case CommandType.StoredProcedure:
                    wrapper = GetStoredProcCommandWrapper(commandText);
                    break;

                case CommandType.Text:
                    wrapper = GetSqlStringCommandWrapper(commandText);
                    break;
            }

            if (wrapper == null)
            {
                throw new ArgumentException("The value of the parameter was not valid.", "commandType");
            }
            return wrapper;
        }

        private int DoUpdateDataSet(UpdateBehavior behavior, IDbConnection connection,
                                    DataSet dataSet, string tableName, DBCommandWrapper insertCommand,
                                    DBCommandWrapper updateCommand, DBCommandWrapper deleteCommand, int updateBatchSize, DataViewRowState rowFilter)
        {
            ArgumentValidation.CheckForNullReference(dataSet, "dataSet");
            ArgumentValidation.CheckForNullReference(tableName, "tableName");
            ArgumentValidation.CheckForEmptyString(tableName, "tableName");

            if (insertCommand == null && updateCommand == null && deleteCommand == null)
            {
                throw new ArgumentException("At least one command must be initialized.");
            }

            using (DbDataAdapter adapter = GetDataAdapter(behavior, connection))
            {
                IDbDataAdapter explicitAdapter = adapter;
                if (insertCommand != null)
                {
                    explicitAdapter.InsertCommand = insertCommand.Command;
                }
                if (updateCommand != null)
                {
                    explicitAdapter.UpdateCommand = updateCommand.Command;
                }
                if (deleteCommand != null)
                {
                    explicitAdapter.DeleteCommand = deleteCommand.Command;
                }

                adapter.UpdateBatchSize = updateBatchSize;

                if (updateBatchSize == 0)
                {
                    adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
                    adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
                    adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.OutputParameters;
                }
                int rows = adapter.Update(dataSet.Tables[tableName].Select(null, null, rowFilter));
                return rows;
            }
        }

        private void DoLoadDataSet(DBCommandWrapper command, DataSet dataSet, string[] tableNames)
        {
            ArgumentValidation.CheckForNullReference(tableNames, "tableNames");

            if (tableNames.Length == 0)
            {
                throw new ArgumentException("The table name array used to map results to user-specified table names cannot be empty.", "tableNames");
            }

            for (int i = 0; i < tableNames.Length; i++)
            {
                ArgumentValidation.CheckForNullReference(tableNames[i], string.Concat("tableNames[", i, "]"));
                ArgumentValidation.CheckForEmptyString(tableNames[i], string.Concat("tableNames[", i, "]"));
            }

            using (DbDataAdapter adapter = GetDataAdapter(UpdateBehavior.Standard, command.Command.Connection))
            {
                ((IDbDataAdapter)adapter).SelectCommand = command.Command;

                const string systemCreatedTableNameRoot = "Table";

                for (int i = 0; i < tableNames.Length; i++)
                {
                    string systemCreatedTableName = (i == 0)
                        ? systemCreatedTableNameRoot
                        : systemCreatedTableNameRoot + i;

                    adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                }

                adapter.Fill(dataSet);
            }
        }

        private static object DoExecuteScalar(DBCommandWrapper command)
        {
            object returnValue = command.Command.ExecuteScalar();
            return returnValue;
        }

        private static int DoExecuteNonQuery(DBCommandWrapper command)
        {
            command.RowsAffected = command.Command.ExecuteNonQuery();
            return command.RowsAffected;
        }

        private static IDataReader DoExecuteReader(IDbCommand command, CommandBehavior cmdBehavior)
        {
            IDataReader reader = command.ExecuteReader(cmdBehavior);
            return reader;
        }
        private static IDbTransaction BeginTransaction(IDbConnection connection)
        {
            IDbTransaction tran = connection.BeginTransaction();
            return tran;
        }

        private static void RollbackTransaction(IDbTransaction tran)
        {
            tran.Rollback();
        }

        private static void CommitTransaction(IDbTransaction tran)
        {
            tran.Commit();
        }

    }
}
