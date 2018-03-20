//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Collections;

using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Data
{
	/// <summary>
	///		The DbWrapper encapsulates ADO.NET code for data access w/ an OleDb compliant data source.
	/// </summary>
	/// <example>
	/// This examples show the use the DbWrapper class static methods
	/// <code>
	/// DbWrapper.ExecuteNonQuery("stored_Proc_Name", DbWrapper.GetParameter(DbType.Int32, ParameterDirection.Input, 1000));
	/// 
	/// IDataReader reader = DbWrapper.ExecuteDataReader("stored_Proc_Name", DbWrapper.GetParameter(DbType.Int32, ParameterDirection.Input, 1000));
	/// 
	/// DataSet ds = DbWrapper.ExecuteExecute("stored_Proc_Name", DbWrapper.GetParameter(DbType.Int32, ParameterDirection.Input, 1000));
	/// </code>
	/// </example>
	/// <remarks>	
	///		Date: 8.26.2003
	///		Copyright 2003 Prezza Software, Inc.
	///	</remarks>
    [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
	public class DbWrapper
	{
		/// <summary>
		///		The constructor of this class is private since all public functions are static
		/// </summary>
		private DbWrapper() { }

		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		#region ExecuteDataReader

		/// <summary>
		///		Executes a SQL command and returns a <see cref="IDataReader"/>
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="selectQueryText">The SQL command to execute</param>
		/// <returns><see cref="IDataReader"/> with results of query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataReader ExecuteDataReader(string selectQueryText)
		{

			//Set up the connection
			OleDbConnection connection = new OleDbConnection(GetConnectionString());

			//Pass through to the private implementation
			return ExecuteDataReader(CommandType.Text, selectQueryText, connection);
			
			
		}

		/// <summary>
		///		Executes a SQL command and returns a <see cref="IDataReader" />
		/// </summary>
		/// <param name="selectQueryText">The SQL command to execute</param>
		/// <param name="connectionString">Connection string for database connection</param>
		/// <returns><see cref="IDataReader"/> with results of the query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataReader ExecuteDataReader(string selectQueryText, string connectionString)
		{
			//Set up the connection
			OleDbConnection connection = new OleDbConnection(connectionString);

			//Pass through to the private implementation
			return ExecuteDataReader(CommandType.Text, selectQueryText, connection);
		}

		/// <summary>
		///		Executes a SQL command and returns a <see cref="IDataReader" />
		/// </summary>
		/// <param name="selectQueryText">The SQL command to execute</param>
		/// <param name="connectionString">Connection string for database connection</param>
		/// <param name="parameters">Query parameters</param>
		/// <returns><see cref="IDataReader"/> with results of the query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataReader ExecuteDataReader(string selectQueryText, string connectionString, params object[] parameters)
		{
			//Set up the connection
			OleDbConnection connection = new OleDbConnection(connectionString);

			//Pass through to the private implementation
			return ExecuteDataReader(CommandType.Text, selectQueryText, connection, parameters);
		}
		
		/// <summary>
		///		Executes a SQL command and returns a <see cref="IDataReader"/>.  
		///		This version accepts parameters for a stored proc
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="storedProcedureName">The Stored Procedure to execute</param>
		/// <param name="parameterValues">An array containing the parameters passed to this SP</param>
		/// <returns><see cref="IDataReader"/> with results of query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataReader ExecuteDataReader(string storedProcedureName, params object[] parameterValues)
		{
			
			//Set up the connection
			OleDbConnection connection = new OleDbConnection(GetConnectionString());

			//Pass through to the private implementation
			return ExecuteDataReader(CommandType.StoredProcedure, storedProcedureName, connection, parameterValues);
			
		}
		

		/// <summary>
		///		This is the private implementation of the public interfaces.  It executes the SQL
		///		command and returns a <see cref="IDataReader"/>
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="commandType">Specifies if this is a stored proc or plain SQL</param>
		/// <param name="commandText">The SQL command to execute</param>
		/// <param name="connection">The connection to the OleDb data source</param>
		/// <param name="parameterValues">An array containing the parameters passed to this SP</param>
		/// <returns><see cref="IDataReader"/> with results of query</returns>
		private static IDataReader ExecuteDataReader(CommandType commandType, string commandText, OleDbConnection connection, params object[] parameterValues)
		{
			//OleDb will take care of escaping, so don't do it here
			//if(commandType == CommandType.StoredProcedure)
			//{
			//	foreach(object o in parameterValues)
			//	{
			//		if (((IDataParameter)o).Value is string)
			//		{
			//			((IDataParameter)o).Value = ((string)((IDataParameter)o).Value).Replace("'","''");
			//		}
			//	}
			//}

			//Create the command object
			OleDbCommand command = new OleDbCommand();

			//Open the connection if we need to
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}

			//Associate the connection with the command
			command.Connection = connection;

			//Set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			//Set the command type
			command.CommandType = commandType;

			command.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);
	
			//Attach the command parameters if they are provided
			if (parameterValues != null)
			{
				if(commandType == CommandType.StoredProcedure)
				{
					AttachParameters(GetConnectionString(), command, commandText, false, parameterValues); 
				}
				else
				{
					foreach(object o in parameterValues)
					{
						command.Parameters.Add((IDataParameter)o);
					}
				}				
			}
			
			//create an output OleDbDataReader
			OleDbDataReader outputReader;
			

			outputReader = command.ExecuteReader(CommandBehavior.CloseConnection);
			//command.Connection.Close();
			//command.Connection.Dispose();
		
			// detach the OleDbParameters from the command object, so they can be used again.
			//command.Parameters.Clear();
			
			return outputReader;

			
		}

		#endregion

		#region ExecuteDataSet

		/// <summary>
		///		Executes a SQL command and returns a <see cref="DataSet"/>
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="selectQueryText">The SQL command to execute</param>
		/// <returns><see cref="DataSet"/> with results of query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static DataSet ExecuteDataSet(string selectQueryText)
		{

			//Set up the connection
			using(OleDbConnection connection = new OleDbConnection(GetConnectionString()))
			{

				//Pass through to the private implementation
				return ExecuteDataSet(CommandType.Text, selectQueryText, new DataSet(), connection);

			}//When the connection goes out of scope, Dispose(), and hence Close() will be called automatically
			
		}
		
		/// <summary>
		///		Executes a SQL command and returns a <see cref="DataSet"/>.  
		///		This version accepts parameters for a stored proc
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="storedProcedureName">The Stored Procedure to execute</param>
		/// <param name="parameterValues">An array containing the parameters passed to this SP</param>
		/// <returns><see cref="DataSet"/> with results of query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues)
		{
			return ExecuteDataSet(storedProcedureName, new DataSet(), parameterValues);
		}

		/// <summary>
		///		Executes a SQL command and returns a <see cref="DataSet"/>.  
		///		This version accepts parameters for a stored proc
		/// </summary>
		/// <param name="storedProcedureName"></param>
		/// <param name="dataObject"></param>
		/// <param name="parameterValues"></param>
		/// <returns></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static DataSet ExecuteDataSet(string storedProcedureName, DataSet dataObject, params object[] parameterValues)
		{
			//Set up the connection
			using(OleDbConnection connection = new OleDbConnection(GetConnectionString()))
			{
				//Pass through to the private implementation
				return ExecuteDataSet(CommandType.StoredProcedure, storedProcedureName, dataObject, connection, parameterValues);

			}//When the connection goes out of scope, Dispose(), and hence Close() will be called automatically
		}
		
		/// <summary>
		///		This is the private implementation of the public interfaces.  It executes the SQL
		///		command and returns a <see cref="DataSet"/>
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="commandType">Specifies if this is a stored proc or plain SQL</param>
		/// <param name="commandText">The SQL command to execute</param>
		/// <param name="dataObject">A DataSet to fill</param>
		/// <param name="connection">The connection to the SQL server DB</param>
		/// <param name="parameterValues">An array containing the parameters passed to this SP</param>
		/// <returns><see cref="DataSet"/> with results of query</returns>
		private static DataSet ExecuteDataSet(CommandType commandType, string commandText, DataSet dataObject, OleDbConnection connection, params object[] parameterValues)
		{
			
			//OleDb will take care of escaping, so don't do it here
			//foreach(object o in parameterValues)
			//{
			//	if (((IDataParameter)o).Value is string)
			//	{
			//		((IDataParameter)o).Value = ((string)((IDataParameter)o).Value).Replace("'","''");
			//	}
			//}
			
			//Create the command object
			OleDbCommand command = new OleDbCommand();

			//Open the connection if we need to
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}

			//Associate the connection with the command
			command.Connection = connection;

			//Set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			//Set the command type
			command.CommandType = commandType;

			command.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

			//Attach the command parameters if they are provided
			if ((parameterValues != null) && (commandType == CommandType.StoredProcedure))
			{
				AttachParameters(GetConnectionString(), command, commandText, false, parameterValues); 
				//False = no output params for these select stored procs
			}

			//Create a DataAdapter to connect the DataSet to the database
			OleDbDataAdapter adapter = new OleDbDataAdapter(command);

			adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
			//Fill the DataSet
			 adapter.Fill(dataObject);
				
			//Detach the OleDbParameters from the command object, so they can be used again.
			command.Parameters.Clear();
			
			//Return the filled DataSet
			return dataObject;
		}

		#endregion

		#region ExecuteNonQuery

		/// <summary>
		///		Executes a SQL command that returns no data (e.g. Insert)
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="commandText">The SQL command to execute</param>
		/// <returns>The number of rows affected by the query</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static int ExecuteNonQuery(string commandText)
		{
			
			//Set up the connection
			using(OleDbConnection connection = new OleDbConnection(GetConnectionString()))
			{

				//Pass through to the private implementation
				return ExecuteNonQuery(CommandType.Text, connection, commandText, null);
				//First null = no transaction, second null = no parameters

			}//When the connection goes out of scope, Dispose(), and hence Close() will be called automatically

		}

		/// <summary>
		/// This method allows the user to pass in a <see cref="IDbConnection"/> object
		/// </summary>
		/// <param name="commandText">An sql command to execute</param>
		/// <param name="connection"><see cref="IDbConnection"/></param>
		/// <returns></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static int ExecuteNonQuery(string commandText, IDbConnection connection)
		{
			return ExecuteNonQuery(CommandType.Text, (OleDbConnection)connection, commandText, null); 
		}
		
		/// <summary>
		///		Executes a SQL command that returns no data (e.g. Insert)
		///		This version can handle parameters (both input and output)
		///		If you use input/output params, default values 
		///		will be ignored.
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="storedProcedureName">The name of the stored procedure to execute</param>
		/// <param name="commandParameters">The parameters to pass to/from the stored proc</param>
		/// <returns>The number of rows affected by the query</returns>
		//public static int ExecuteNonQuery(string storedProcedureName, params IDataParameter[] commandParameters)
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static int ExecuteNonQuery(string storedProcedureName, params object[] commandParameters)
		{
			
			//Set up the connection
			using(OleDbConnection connection = new OleDbConnection(GetConnectionString()))
			{
				
				//Pass through to the private implementation
				return ExecuteNonQuery(CommandType.StoredProcedure, connection, storedProcedureName, commandParameters);

			}//When the connection goes out of scope, Dispose(), and hence Close() will be called automatically

		}

		/// <summary>
		/// Execute a non-query using the passed transaction wrapper.
		/// </summary>
		/// <param name="nonQueryTransaction">Wrapper for transaction consisting of non-query statements.</param>
		/// <returns>Rows affected by query.</returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static int ExecuteNonQuery(DbNonQueryTransactionWrapper nonQueryTransaction)
		{
			//Set up the return value
			int outputRowCount = 0;

			//Set up the connection
			using(OleDbConnection connection = new OleDbConnection(GetConnectionString()))
			{
				
				//Open the connection if we need to
				if (connection.State != ConnectionState.Open)
				{
					connection.Open();
				}

				// Transaction
				OleDbTransaction transaction = connection.BeginTransaction();

				
				try
				{
					foreach(DbNonQueryTransactionWrapper.NonQuery nonQuery in nonQueryTransaction.NonQueries)
					{
						//Create the command object
						OleDbCommand command = new OleDbCommand(nonQuery.CommandText, connection, transaction);
						command.CommandType = nonQuery.CommandType;

						command.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);



						//Attach the command parameters if they are provided
						if ((nonQuery.CommandParameters != null) && (nonQuery.CommandType == CommandType.StoredProcedure))
						{
							foreach (OleDbParameter parameter in nonQuery.CommandParameters)
							{
								if(parameter != null)
								{
									if ((parameter.Value != null) && (parameter.Value.GetType() == typeof(int)))
										parameter.DbType = DbType.Int32;

									//Check for derived output value with no value assigned
									if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
									{
										parameter.Value = DBNull.Value;
									}
								}

								command.Parameters.Add(parameter);

							} //end foreach
						}

						//Execute the command
						outputRowCount += command.ExecuteNonQuery();
	
						// detach the OleDbParameters from the command object, so they can be used again.
						command.Parameters.Clear();
					}
					// commit
					transaction.Commit();
				}
				catch(OleDbException ex)
				{
					transaction.Rollback();
					bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");
					if(rethrow)
						throw;
				}
				finally
				{
					connection.Close();
				}
				
			}
			//Outout the results of this query
			return outputRowCount;
		}

		/// <summary>
		///		This is the private implementation of the public interfaces.  It executes the SQL
		///		command and returns the number of rows affected
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="commandType">Indicates whether to execute a stored proc or raw TSQL</param>
		/// <param name="connection">The connection to use</param>
		/// <param name="commandText">The SQL or SP name to execute</param>
		/// <param name="commandParameters">The parameters to pass to the stored proc</param>
		/// <returns>The number of rows affected by the query</returns>
		private static int ExecuteNonQuery(CommandType commandType, OleDbConnection connection, string commandText, params object[] commandParameters)
		{
			#region AutoCreateParameter
			/*
			int i = 0;
			foreach(object o in commandParameters)
			{
				if(o != null)
				{
					// If these objects are not IDataParameters, create them as such with the object as their value
					if (o.GetType() != typeof(IDataParameter))
					{
						commandParameters[i++] = DbWrapper.GetParameter(o);
					}
				}
				else 
				{
					commandParameters[i++] = DbWrapper.GetParameter(null);
				}
			}
			*/
			#endregion


			

			//Create the command object
			OleDbCommand command = new OleDbCommand();

			//Open the connection if we need to
			if (connection.State != ConnectionState.Open)
			{
				connection.Open();
			}

			//Associate the connection with the command
			command.Connection = connection;

			//Set the command text (stored procedure name or SQL statement)
			command.CommandText = commandText;

			//Set the command type
			command.CommandType = commandType;

			command.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);


	
			//Attach the command parameters if they are provided
			if ((commandParameters != null) && (commandType == CommandType.StoredProcedure))
			{

				foreach (OleDbParameter parameter in commandParameters)
				{
					if(parameter != null)
					{
						if ((parameter.Value != null) && (parameter.Value.GetType() == typeof(int)))
							parameter.DbType = DbType.Int32;

						//Check for derived output value with no value assigned
						if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
						{
							parameter.Value = DBNull.Value;
						}
					}

					command.Parameters.Add(parameter);

				} //end foreach
			}
			
			//Set up the return value
			int outputRowCount;
			
			//Execute the command
			outputRowCount = command.ExecuteNonQuery();
	
			// detach the OleDbParameters from the command object, so they can be used again.
			command.Parameters.Clear();
			
			//Outout the results of this query
			return outputRowCount;

		}

		#endregion

		#region Utility methods

		/// <summary>
		///		This method attaches any parameters needed to a stored procedure being executed
		///		It derives the params needed by an SP from the DB, then caches the results for
		///		future calls
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="connectionString">Connection string to the database</param>
		/// <param name="command">The command object the parameters are being attached to</param>
		/// <param name="storedProcedureName">Name of the SP we will discover params for</param>
		/// <param name="includeReturnValueParameter">Indicates if there are output params</param>
		/// <param name="parameterValues">Array of parameter values</param>
		private static void AttachParameters(string connectionString, 
			OleDbCommand command, 
			string storedProcedureName, 
			bool includeReturnValueParameter, 
			params object[] parameterValues)
		{
			//First, attempt to get the parameter definitions for this stored procedure 
			//out of the cache
			string hashKey = connectionString + ":" + storedProcedureName;
			if (includeReturnValueParameter)
			{
				hashKey += ":include ReturnValue Parameter";
			}

			OleDbParameter[] cachedParameters;
			
			cachedParameters = (OleDbParameter[])paramCache[hashKey];

			if (cachedParameters == null)
			{			
				
				//If the parameter definition for this SP is not in the cache, look it up in the database

				OleDbConnection discoveryConnection = new OleDbConnection(connectionString); 
				OleDbCommand discoveryCommand = new OleDbCommand(storedProcedureName, discoveryConnection);
				
				discoveryConnection.Open();
				discoveryCommand.CommandType = CommandType.StoredProcedure;

				OleDbCommandBuilder.DeriveParameters(discoveryCommand);
						
				if (!includeReturnValueParameter) 
				{
					discoveryCommand.Parameters.RemoveAt(0);
				}
				
				OleDbParameter[] discoveredParameters = new OleDbParameter[discoveryCommand.Parameters.Count];
				
				discoveryCommand.Parameters.CopyTo(discoveredParameters, 0);

				

				//Store these parameters in the cache so we won't have to go to the database next time
				paramCache[hashKey] = discoveredParameters;
				//Now get them from the cache assign to OleDbParameter[] cachedParameters
				cachedParameters = (OleDbParameter[])paramCache[hashKey];

				discoveryConnection.Close();
				discoveryCommand.Dispose();
				discoveryConnection.Dispose();

			} //End cache check

			//Clone parameters
			OleDbParameter[] preparedParameters;

			preparedParameters = CloneParameters(cachedParameters);
			
			
			//Throw an error if the wrong number of arguments are specified.
			if(preparedParameters.Length != parameterValues.Length)
			{
				throw new System.ArgumentOutOfRangeException("Stored procedure [" + command.CommandText + "] was called with a different number of parameters than was discovered in the datbase.  Call was made with " + parameterValues.Length.ToString() + ", but " + preparedParameters.Length.ToString() + " were expected.");
			}

			//Iterate through the OleDbParameters, assigning the values from the corresponding position in the 
			//value array
			for (long count = 0; count < preparedParameters.Length; count++)
			{
				OleDbParameter p = (OleDbParameter)parameterValues[count];
				preparedParameters[count].Value = p.Value;
			}

			//Finally, add the parameters to the passed in command
			foreach (OleDbParameter parameter in preparedParameters)
			{
				//check for derived output value with no value assigned
				if ((parameter.Direction == ParameterDirection.InputOutput) && (parameter.Value == null))
				{
					parameter.Value = DBNull.Value;
				}
				if (parameter.Value.GetType() == typeof(int))
					parameter.DbType = DbType.Int32;

				command.Parameters.Add(parameter);

			} //end foreach

		}

		/// <summary>
		///		Clones the parameter object so we don't get the "parameter already
		///		exists" error
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="originalParameters">Array of OleDbParameter objects to be cloned</param>
		/// <returns>Copy of array of OleDbParameter objects</returns>
		private static OleDbParameter[] CloneParameters(OleDbParameter[] originalParameters)
		{
			OleDbParameter[] clonedParameters = new OleDbParameter[originalParameters.Length];

			for (long count = 0; count < originalParameters.Length;  count++)
			{
				clonedParameters[count] = (OleDbParameter)((ICloneable)originalParameters[count]).Clone();
				
			}

			return clonedParameters;
		}

		/// <summary>
		///		Gets the default connection string stored in the .config file
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <returns>Connection string</returns>
		private static string GetConnectionString()
		{
			return ConfigurationSettings.AppSettings["DefaultConnectionString"];
		}

		/// <summary>
		///		Gets a connection string stored in the .config file
		/// </summary>
		/// <remarks>
		///		Date: 8.26.2003
		///		Copyright 2003 Prezza Software, Inc.
		/// </remarks>
		/// <param name="connectionAlias">Alias of the connection string in the .config file</param>
		/// <returns>Connection string</returns>
		private static string GetConnectionString(string connectionAlias)
		{
			return ConfigurationSettings.AppSettings[connectionAlias];
		}

		#region GetParameter()

		/// <summary>
		/// Gets an object of type <see cref="IDataParameter"/>
		/// </summary>
		/// <returns></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataParameter GetParameter()
		{
			return new OleDbParameter();
		}

		/// <summary>
		/// Gets an object of type <see cref="IDataParameter"/> with its Value propery
		/// set to the argument
		/// </summary>
		/// <param name="parameterValue">The value of the parameter</param>
		/// <returns><see cref="IDataParameter"/></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataParameter GetParameter(object parameterValue)
		{
			IDataParameter parm = new OleDbParameter();
			if(parameterValue == null)
				parm.Value = DBNull.Value;
			else				
				parm.Value = parameterValue;
			
			return parm;
		}

		/// <summary>
		/// Creates an IDataParameter object with the given arguments as properties
		/// </summary>
		/// <param name="dbtype">The DbType for the parameter</param>
		/// <param name="parameterValue">The object to assign as the parameter.Value</param>
		/// <returns>An <see cref="IDataParameter"/></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataParameter GetParameter(DbType dbtype, object parameterValue)
		{
			IDataParameter parm = new OleDbParameter();
			parm.DbType = dbtype;
			parm.Value = parameterValue;
			
			return parm;
		}
		
		/// <summary>
		/// Gets an object of type <see cref="IDataParameter"/> with its Value, Direction and DbType
		/// properties set
		/// </summary>
		/// <param name="dbtype"><see cref="DbType"/></param>
		/// <param name="direction"><see cref="ParameterDirection"/></param>
		/// <param name="parameterValue">A value for the parameter</param>
		/// <returns><see cref="IDataParameter"/></returns>
        [Obsolete("The DbWrapper has been deprecated, but has been left in the application for backwards compatibility.  The Database and DBCommandWrapper objects should now be used.")]
		public static IDataParameter GetParameter(DbType dbtype, ParameterDirection direction, object parameterValue)
		{
			IDataParameter parm = new OleDbParameter();
			parm.DbType = dbtype;
			parm.Direction = direction;
			if(parameterValue == null)
				parm.Value = DBNull.Value;
			else				
				parm.Value = parameterValue;

			return parm;
		}
		#endregion

	}
}
#endregion
