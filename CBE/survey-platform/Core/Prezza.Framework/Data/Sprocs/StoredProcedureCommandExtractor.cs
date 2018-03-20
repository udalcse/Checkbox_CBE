using System;
using System.Data;
using System.Reflection;
using System.Collections.Generic;

using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Procecure type
    /// </summary>
    public enum ProcedureType
    {
        /// <summary>
        /// Select/Fetch Procedure
        /// </summary>
        Select,

        /// <summary>
        /// Insert/Create procedure
        /// </summary>
        Insert,

        /// <summary>
        /// Update procedure
        /// </summary>
        Update,

        /// <summary>
        /// Delete procedure
        /// </summary>
        Delete
    }

    /// <summary>
    /// Static extractor to create a db command wrapper to extract
    /// stored procedure command objects from objects by reflection
    /// and use of attributes.
    /// </summary>
    public static class StoredProcedureCommandExtractor
    {
        private static readonly Dictionary<string, ProcedureReflectionInfo> _procedureCache = new Dictionary<string, ProcedureReflectionInfo>();

        /// <summary>
        /// Execute a procedure on the object.  Throws an error if the procedure can't be determined or an
        /// error occurs while running it.
        /// </summary>
        /// <param name="procedureType">Procedure type</param>
        /// <param name="theObject">Object value.</param>
        public static void ExecuteProcedure(ProcedureType procedureType, object theObject)
        {
            ExecuteProcedure(null, procedureType, theObject);
        }


        /// <summary>
        /// Execute a procedure on the object.  Throws an error if the procedure can't be determined or an
        /// error occurs while running it.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="procedureType">Procedure type</param>
        /// <param name="theObject">Object value.</param>
        public static void ExecuteProcedure(string databaseName, ProcedureType procedureType, object theObject)
        {
            ExecuteProcedure(string.Empty, databaseName, procedureType, theObject);
        }

        /// <summary>
        /// Populate object properties with values from a data reader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="theObject"></param>
        /// <remarks>This method does not handle any exceptions, it is up to the caller to 
        /// handle exceptions and properly dispose the reaer.</remarks>
        public static void PopulateObjectFromReaderReturnValues(IDataReader reader, object theObject)
        {
            PopulateObjectFromReaderReturnValues(reader, theObject, GetProcedureInformation(ProcedureType.Select, theObject));
        }

        /// <summary>
        /// Execute a procedure on the object.  Throws an error if the procedure can't be determined or an
        /// error occurs while running it.
        /// </summary>
        /// <param name="instanceName">Name of application instance.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="procedureType">Procedure type</param>
        /// <param name="theObject">Object value.</param>
        public static void ExecuteProcedure(string instanceName, string databaseName, ProcedureType procedureType, object theObject)
        {
            //Get the procedure info and the command wrapper
            DBCommandWrapper command = GetCommandWrapper(instanceName, databaseName, procedureType, theObject, true);
            ProcedureReflectionInfo procedureInfo = GetProcedureInformation(procedureType, theObject);

            //Both values shouldn't be null, since that would have caused an exception to be thrown by getcommandwrapper
            Database db = DatabaseFactory.CreateDatabase(instanceName, databaseName);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //For each output parameter, assign values
                    foreach (ParameterReflectionInfo parameterInfo in procedureInfo.Parameters.Values)
                    {
                        //Get output parameters
                        if (parameterInfo.Direction == ParameterDirection.Output || parameterInfo.Direction == ParameterDirection.InputOutput)
                        {
                            object paramValue = command.GetParameterValue(parameterInfo.Name);

                            if (parameterInfo.ConvertDBNullToNull && paramValue == DBNull.Value)
                            {
                                paramValue = null;
                            }

                            //Attempt to set the value
                            parameterInfo.Property.SetValue(theObject, paramValue, null);
                        }
                    }

                    if (reader.Read())
                    {
                        PopulateObjectFromReaderReturnValues(reader, theObject, procedureInfo);
                    }
                }
                catch
                {
                    reader.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Populate an objects properties using values from the specified reader and by reading
        /// fetch parameter attributes with a ReturnValue direction.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="theObject"></param>
        /// <param name="procedureInfo"></param>
        /// <remarks>This method does not handle any exceptions, it is up to the caller to 
        /// handle exceptions and properly dispose the reaer.</remarks>
        private static void PopulateObjectFromReaderReturnValues(IDataReader reader, object theObject, ProcedureReflectionInfo procedureInfo)
        {
            foreach (ParameterReflectionInfo parameterInfo in procedureInfo.Parameters.Values)
            {
                if (parameterInfo.Direction == ParameterDirection.ReturnValue)
                {
                    object paramValue = DbUtility.GetValueFromDataReader<object>(reader, parameterInfo.Name, DBNull.Value);

                    if (parameterInfo.ConvertDBNullToNull && paramValue == DBNull.Value)
                    {
                        paramValue = null;
                    }

                    if (paramValue != null || parameterInfo.AllowNull)
                    {
                        parameterInfo.Property.SetValue(theObject, paramValue, null);
                    }
                }
            }
        }

        /// <summary>
        /// Extract a command wrapper for the object.  Null is returned if the wrapper
        /// could not be extracted.  An exception will be thrown if errorOnFailure is true
        /// and a command could not be extracted.  The command will be created using
        /// the default database object.
        /// </summary>
        /// <param name="procedureType">Type of procedure to get</param>
        /// <param name="theObject">Object to extract command from.</param>
        /// <param name="errorOnFailure">Throw an error on failure or not.</param>
        /// <returns></returns>
        public static DBCommandWrapper GetCommandWrapper(ProcedureType procedureType, object theObject, bool errorOnFailure)
        {
            return GetCommandWrapper(null, null, procedureType, theObject, errorOnFailure);
        }

        /// <summary>
        /// Extract a command wrapper for the object.  Null is returned if the wrapper
        /// could not be extracted.  An exception will be thrown if errorOnFailure is true
        /// and a command could not be extracted.  The command will be 
        /// </summary>
        /// <param name="instanceName">Name of application instance.</param>
        /// <param name="databaseName">Name of the database to use.</param>
        /// <param name="procedureType">Type of procedure to get</param>
        /// <param name="theObject">Object to extract command from.</param>
        /// <param name="errorOnFailure">Throw an error on failure or not.</param>
        /// <returns></returns>
        public static DBCommandWrapper GetCommandWrapper(string instanceName, string databaseName, ProcedureType procedureType, object theObject, bool errorOnFailure)
        {
            DBCommandWrapper commandWrapper = null;

            try
            {
                //Use reflection to get the query information
                ProcedureReflectionInfo procedureInfo = GetProcedureInformation(procedureType, theObject);

                //Get the command wrapper
                commandWrapper = GetCommandWrapper(instanceName, databaseName, procedureInfo, theObject);

            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (errorOnFailure)
                {
                    throw new StoredProcedureExtractionFailure();
                }
            }

            //No method found, throw an error
            if (commandWrapper == null && errorOnFailure)
            {
                throw new StoredProcedureExtractionFailure();
            }

            return commandWrapper;
        }

        /// <summary>
        /// Get a command wrapper based on the provided procedure reflection information and
        /// the object.
        /// </summary>
        /// <param name="instanceName">Application instance name</param>
        /// <param name="databaseName">Name of database config to use.</param>
        /// <param name="procedureInfo">Procedure information.</param>
        /// <param name="theObject">Object</param>
        /// <returns>Return value</returns>
        private static DBCommandWrapper GetCommandWrapper(string instanceName, string databaseName, ProcedureReflectionInfo procedureInfo, object theObject)
        {
            Database db = DatabaseFactory.CreateDatabase(instanceName, databaseName);

            //Get the command wrapper
            DBCommandWrapper command = db.GetStoredProcCommandWrapper(procedureInfo.Name);

            //Set parameters, except for return values, which are used to assign values from "Get"
            // statements.
            foreach (ParameterReflectionInfo parameterInfo in procedureInfo.Parameters.Values)
            {
                if (parameterInfo.Direction != ParameterDirection.ReturnValue)
                {
                    object value = parameterInfo.Property.GetValue(theObject, null) ?? DBNull.Value;

                    //Ensure db null is passed instead of null

                    command.AddParameter(parameterInfo.Name, parameterInfo.DbType, parameterInfo.Direction, parameterInfo.Size, value);
                }
            }

            return command;
        }

        /// <summary>
        /// Get procedure information for the object
        /// </summary>
        /// <param name="procedureType">Procedure type</param>
        /// <param name="theObject">Object</param>
        /// <returns>Return value</returns>
        private static ProcedureReflectionInfo GetProcedureInformation(ProcedureType procedureType, object theObject)
        {
            //Check the cache
            string cacheKey = procedureType + "_" + theObject.GetType().AssemblyQualifiedName;

            if (_procedureCache.ContainsKey(cacheKey))
            {
                return _procedureCache[cacheKey];
            }

            Type procedureAttributeType;
            Type parameterAttributeType;

            if (procedureType == ProcedureType.Delete)
            {
                procedureAttributeType = typeof(DeleteProcedure);
                parameterAttributeType = typeof(DeleteParameter);
            }
            else if (procedureType == ProcedureType.Insert)
            {
                procedureAttributeType = typeof(InsertProcedure);
                parameterAttributeType = typeof(InsertParameter);
            }
            else if (procedureType == ProcedureType.Select)
            {
                procedureAttributeType = typeof(FetchProcedure);
                parameterAttributeType = typeof(FetchParameter);
            }
            else
            {
                procedureAttributeType = typeof(UpdateProcedure);
                parameterAttributeType = typeof(UpdateParameter);
            }

            //Attempt to extract the information
            ProcedureReflectionInfo procedureInfo = ExtractInformation(procedureAttributeType, parameterAttributeType, theObject);

            if (procedureInfo != null)
            {
                _procedureCache[cacheKey] = procedureInfo;
            }

            return procedureInfo;
        }

        /// <summary>
        /// Extract query 
        /// </summary>
        /// <param name="queryAttributeType">Type of query attribute to check.</param>
        /// <param name="parameterType">Type of parameter to check for query params</param>
        /// <param name="theObject">Object to extract parameters from.</param>
        private static ProcedureReflectionInfo ExtractInformation(Type queryAttributeType, Type parameterType, object theObject)
        {
            ProcedureReflectionInfo procedureInfo = null;

            //Attempt to get the procedure information
            Type objectType = theObject.GetType();
            object[] objectAttrs = objectType.GetCustomAttributes(queryAttributeType, true);

            if (objectAttrs.Length > 0)
            {
                procedureInfo = new ProcedureReflectionInfo { Name = ((StoredProcedure)objectAttrs[0]).Name };

                //Now get the parameters
                PropertyInfo[] properties = objectType.GetProperties();

                foreach (PropertyInfo propertyInfo in properties)
                {
                    object[] propertyAttrs = propertyInfo.GetCustomAttributes(parameterType, true);

                    if (propertyAttrs.Length > 0)
                    {
                        //If a name is not explicitly specified, use the property name.
                        string paramName = ((SprocParameter)propertyAttrs[0]).Name;

                        if (paramName == null || paramName.Trim() == string.Empty)
                        {
                            paramName = "" + propertyInfo.Name;
                        }

                        ParameterReflectionInfo parameterInfo = new ParameterReflectionInfo(
                            ((SprocParameter)propertyAttrs[0]).Size,
                            paramName,
                            ((SprocParameter)propertyAttrs[0]).DbType,
                            ((SprocParameter)propertyAttrs[0]).Direction,
                            ((SprocParameter)propertyAttrs[0]).ConvertDBNullToNull,
                            ((SprocParameter)propertyAttrs[0]).AllowNull,
                            propertyInfo);

                        procedureInfo.AddParameter(parameterInfo.Name, parameterInfo);
                    }
                }
            }

            return procedureInfo;
        }
    }
}
