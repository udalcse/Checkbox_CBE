//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Data.SqlClient;

namespace Prezza.Framework.Data.Sql
{
    /// <summary>
    /// <para>Represents a SQL statement or stored procedure to execute against a Sql Server database.</para>
    /// </summary>   
    public class SqlCommandWrapper : DBCommandWrapper
    {
        private readonly SqlCommand _command;
        private readonly object[] _parameterValues;
        private readonly bool _needsParameters = false;
        private char _parameterToken;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="SqlCommandWrapper"/> class with the text of a query and the _command type.</para>
        /// </summary>
        /// <param name="commandText"><para>The stored procedure name or SQL sting the _command represents.</para></param>
        /// <param name="commandType"><para>One of the <see crer="CommandType"/> values.</para></param>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        internal SqlCommandWrapper(string commandText, CommandType commandType, char parameterToken)
        {
            _parameterToken = parameterToken;
            _command = CreateCommand(commandText, commandType);
            _command.CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SQLCommandTimeout"]);
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="SqlCommandWrapper"/> class with the text of a query the _command type, and the parameter values.</para>
        /// </summary>        
        /// <param name="commandText"><para>The stored procedure name or SQL sting the _command represents.</para></param>
        /// <param name="commandType"><para>One of the <see crer="CommandType"/> values.</para></param>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        /// <param name="parameterValues"><para>The parameter values to assign in positional order.</para></param>
        internal SqlCommandWrapper(string commandText, CommandType commandType, char parameterToken, object[] parameterValues)
            : this(commandText, commandType, parameterToken)
        {
            //_command = CreateCommand(commandText, commandType);
            _parameterValues = parameterValues;
            if (commandType == CommandType.StoredProcedure)
            {
                _needsParameters = true;
            }
        }

        /// <summary>
        /// <para>Gets the underlying <see cref="IDbCommand"/>.</para>
        /// </summary>
        /// <value>
        /// <para>The underlying <see cref="IDbCommand"/>. The default is <see langword="null"/>.</para>
        /// </value>
        /// <remarks>
        /// <para>This _command is a <see cref="SqlCommand"/></para>        
        /// </remarks>        
        /// <seealso cref="SqlCommand"/>
        public override IDbCommand Command
        {
            get { return _command; }
        }

        /// <summary>
        /// <para>Gets or sets the rows affected by this _command.</para>
        /// </summary>
        /// <value>
        /// <para>The rows affected by this _command.</para>
        /// </value>
        public override int RowsAffected { get; set; }

        /// <summary>
        /// <para>Gets or sets the wait time before terminating the attempt to execute a _command and generating an error.</para>
        /// </summary>
        /// <value>
        /// <para>The wait time before terminating the attempt to execute a _command and generating an error.</para>
        /// </value>
        public override int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts null values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        public override void AddParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            _command.Parameters.Add(CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value));
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="sqlType"><para>One of the <see cref="SqlDbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts null values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddParameter(string name, SqlDbType sqlType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = CreateParameter(name, DbType.String, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            param.SqlDbType = sqlType;
            _command.Parameters.Add(param);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        public override void AddParameter(string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = CreateParameter(name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
            _command.Parameters.Add(param);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>       
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        /// <param name="value"><para>The value of the parameter.</para></param>    
        public override void AddParameter(string name, DbType dbType, ParameterDirection direction, int size, object value)
        {
            SqlParameter param = CreateParameter(name, dbType, 0, direction, false, 0, 0, string.Empty, DataRowVersion.Default, value);
            _command.Parameters.Add(param);
        }


        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command set as <see cref="ParameterDirection"/> value of Output.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public override void AddOutParameter(string name, DbType dbType, int size)
        {
            AddParameter(name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public override void AddInParameter(string name, DbType dbType)
        {
            AddParameter(name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public override void AddInParameter(string name, DbType dbType, object value)
        {
            AddParameter(name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// <para>Adds a new instance of an <see cref="SqlParameter"/> object to the _command set as <see cref="ParameterDirection"/> value of Input.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public override void AddInParameter(string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            AddParameter(name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// <para>Returns the value of the parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to get the value.</para></param>
        /// <returns><para>The value of the parameter.</para></returns>
        public override object GetParameterValue(string name)
        {
            return _command.Parameters[BuildParameterName(name)].Value;
        }

        /// <summary>
        /// <para>Sets the value of a parameter for the given <paramref name="name"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the parameter to set the value.</para></param>
        /// <param name="value"><para>The new value of the parameter.</para></param>
        public override void SetParameterValue(string name, object value)
        {
            _command.Parameters[BuildParameterName(name)].Value = value ?? DBNull.Value;
        }

        /// <summary>
        /// <para>Clean up resources.</para>
        /// </summary>
        public override void Dispose()
        {
            _command.Dispose();
        }

        /// <summary>
        /// <para>Dicover the parameters for a stored procedure using a separate connection and _command.</para>
        /// </summary>
        /// <param name="parameterToken"><para>The parameter delimeter for database commands.</para></param>
        protected override void DoDiscoverParameters(char parameterToken)
        {
            _parameterToken = parameterToken;
            using (SqlCommand newCommand = CreateNewCommandAndConnectionForDiscovery())
            {
                SqlCommandBuilder.DeriveParameters(newCommand);

                foreach (IDataParameter parameter in newCommand.Parameters)
                {
                    IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                    cloneParameter.ParameterName = BuildParameterName(cloneParameter.ParameterName);
                    _command.Parameters.Add(cloneParameter);
                }
                newCommand.Connection.Close();
            }
        }

        /// <summary>
        /// <para>Assign the values provided by a user to the _command parameters discovered in positional order.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The number of parameters does not match number of values for stored procedure.</para>
        /// </exception>
        protected override void DoAssignParameterValues()
        {
            if (SameNumberOfParametersAndValues() == false)
            {
                throw new InvalidOperationException("The number of parameters does not match number of values for stored procedure.");
            }

            const int returnParameter = 1;
            for (int i = 0; i < _parameterValues.Length; i++)
            {
                IDataParameter parameter = _command.Parameters[i + returnParameter];

                // There used to be code here that checked to see if the parameter was input or input/output
                // before assigning the value to it. We took it out because of an operational bug with
                // deriving parameters for a stored procedure. It turns out that output parameters are set
                // to input/output after discovery, so any direction checking was unneeded. Should it ever
                // be needed, it should go here, and check that a parameter is input or input/output before
                // assigning a value to it.
                SetParameterValue(parameter.ParameterName, _parameterValues[i]);
            }
        }

        /// <summary>
        /// <para>Determine if a stored procedure is using parameter discovery.</para>
        /// </summary>
        /// <returns>
        /// <para><see langword="true"/> if further preparation is needed.</para>
        /// </returns>
        protected override bool DoIsFurtherPreparationNeeded()
        {
            return _needsParameters;
        }

        /// <devdoc>
        /// Create a parameter.
        /// </devdoc>        
        private SqlParameter CreateParameter(string name, DbType type, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = _command.CreateParameter();
            param.ParameterName = BuildParameterName(name);

            if ((type.Equals(DbType.Object)) && (value is byte[]))
            {
                param.SqlDbType = SqlDbType.Image;
            }
            else
            {
                param.DbType = type;
            }

            param.Size = size;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.Precision = precision;
            param.Scale = scale;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            param.Value = value ?? DBNull.Value;

            return param;
        }

        private bool SameNumberOfParametersAndValues()
        {
            const int returnParameterCount = 1;
            int numberOfParametersToStoredProcedure = _command.Parameters.Count - returnParameterCount;
            int numberOfValuesProvidedForStoredProcedure = _parameterValues.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <devdoc>
        /// Discovery has to be done on its own connection to allow for the case of the
        /// connection being used being enrolled in a transaction. The SqlCommandBuilder.DeriveParameters
        /// method creates a new SqlCommand internally to communicate to the database, and it
        /// reuses the same connection that is passed in on the _command object. If this _command
        /// object has a connection that is enrolled in a transaction, the DeriveParameters method does not
        /// honor that transaction, and the call fails. To avoid this, create your own connection and
        /// _command, and use them. 
        /// 
        /// You then have to clone each of the IDataParameter objects before it can be transferred to 
        /// the original _command, or another exception is thrown.
        /// </devdoc>
        private SqlCommand CreateNewCommandAndConnectionForDiscovery()
        {
            SqlConnection clonedConnection = (SqlConnection)((ICloneable)_command.Connection).Clone();
            clonedConnection.Open();
            SqlCommand newCommand = CreateCommand(_command.CommandText, _command.CommandType);
            newCommand.Connection = clonedConnection;

            return newCommand;
        }

        private static SqlCommand CreateCommand(string commandText, CommandType commandType)
        {
            SqlCommand newCommand = new SqlCommand {CommandText = commandText, CommandType = commandType};

            return newCommand;
        }

        private string BuildParameterName(string name)
        {
            //System.Diagnostics.Debug.Assert(parameterToken != 0x0000);
            if (name[0] != _parameterToken)
            {
                return name.Insert(0, new string(_parameterToken, 1));
            }
            return name;
        }
    }
}
