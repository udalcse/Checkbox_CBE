//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Data.Common;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
/*
using IBM.Data.DB2;

namespace Prezza.Framework.Data.DB2
{
    /// <summary>
    /// <para>Represents a DB2 Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses the .NET Managed Provider from DB2 AB (DB2.Data.DB2Client) to connect to the database.
    /// </para>  
    /// </remarks>
    public class DB2Database : Database
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="DB2Database"/> class.
        /// </summary>
        public DB2Database()
            : base()
        {
        }

        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the DB2 Database Checkbox © stored procedures .</para>
        /// </summary>
        protected override char ParameterToken
        {
            get { return '_'; }
        }

        /// <summary>
        /// <para>Get the connection for this database.</para>
        /// <seealso cref="IDbConnection"/>
        /// <seealso cref="DB2Connection"/>
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DB2Connection"/> for this database.</para>
        /// </returns>
        public override IDbConnection GetConnection()
        {
            return new DB2Connection(ConnectionString);
        }

        /// <summary>
        /// <para>Create a <see cref="DB2CommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DB2CommandWrapper"/> for the stored procedure.</para></returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="storedProcedureName"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="storedProcedureName"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName)
        {
            ArgumentValidation.CheckForNullReference(storedProcedureName, "storedProcedureName");
            ArgumentValidation.CheckForEmptyString(storedProcedureName, "storedProcedureName");

            return new DB2CommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken);
        }

        /// <summary>
        /// <para>Create an <see cref="DB2CommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DB2CommandWrapper"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="storedProcedureName"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// <para>- or -</para>
        /// <para><paramref name="parameterValues"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="storedProcedureName"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetStoredProcCommandWrapper(string storedProcedureName, params object[] parameterValues)
        {
            ArgumentValidation.CheckForNullReference(storedProcedureName, "storedProcedureName");
            ArgumentValidation.CheckForEmptyString(storedProcedureName, "storedProcedureName");
            ArgumentValidation.CheckForNullReference(parameterValues, "parameterValues");

            return new DB2CommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken, parameterValues);
        }

        /// <summary>
        /// <para>Create an <see cref="DB2CommandWrapper"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DB2CommandWrapper"/> for the SQL query.</para></returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="query"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><paramref name="query"/> hast not been initialized.</para>
        /// </exception>
        public override DBCommandWrapper GetSqlStringCommandWrapper(string query)
        {
            ArgumentValidation.CheckForNullReference(query, "query");
            ArgumentValidation.CheckForEmptyString(query, "query");

            return new DB2CommandWrapper(query, CommandType.Text, ParameterToken);
        }

        /// <summary>
        /// <para>Create a <see cref="DB2DataAdapter"/> with the given update behavior and connection.</para>
        /// </summary>
        /// <param name="updateBehavior">
        /// <para>One of the <see cref="UpdateBehavior"/> values.</para>
        /// </param>
        /// <param name="connection">
        /// <para>The open connection to the database.</para>
        /// </param>
        /// <returns>An <see cref="DB2DataAdapter"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        protected override DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior, IDbConnection connection)
        {
            string queryStringToBeFilledInLater = String.Empty;
            DB2DataAdapter adapter = new DB2DataAdapter(queryStringToBeFilledInLater, (DB2Connection)connection);

            if (updateBehavior == UpdateBehavior.Continue)
            {
                adapter.RowUpdated += new DB2RowUpdatedEventHandler(OnDB2RowUpdated);
            }
            return adapter;
        }

        /// <summary>
        /// <para>Executes the <see cref="DB2CommandWrapper"/> and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DB2CommandWrapper"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(DB2CommandWrapper command)
        {
            IDbConnection connection = OpenConnection();
            PrepareCommand(command, connection);
            DB2Command db2Command = command.Command as DB2Command;
            return DoExecuteXmlReader(db2Command);
        }

        /// <summary>
        /// <para>Executes the <see cref="DB2CommandWrapper"/> in a transaction and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DB2CommandWrapper"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(DB2CommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);

            DB2Command db2Command = command.Command as DB2Command;
            return DoExecuteXmlReader(db2Command);
        }

        /// <devdoc>
        /// Execute the actual Xml Reader call.
        /// </devdoc>        
        private XmlReader DoExecuteXmlReader(DB2Command sqlCommand)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                XmlReader reader = sqlCommand.ExecuteXmlReader();
                return reader;
            }
            catch
            {
                throw;
            }
        }

        /// <devdoc>
        /// Listens for the RowUpdate event on a dataadapter to support UpdateBehavior.Continue
        /// </devdoc>
        private void OnDB2RowUpdated(object sender, DB2RowUpdatedEventArgs rowThatCouldNotBeWritten)
        {
            if (rowThatCouldNotBeWritten.RecordsAffected == 0)
            {
                if (rowThatCouldNotBeWritten.Errors != null)
                {
                    //rowThatCouldNotBeWritten.Row.RowError = SR.ExceptionMessageUpdateDataSetRowFailure;
                    rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }
    }
} */