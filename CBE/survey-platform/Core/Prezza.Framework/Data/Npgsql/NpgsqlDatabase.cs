﻿//===============================================================================
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
using Npgsql;

namespace Prezza.Framework.Data.Npgsql
{
    /// <summary>
    /// <para>Represents a PostgreSQL Database.</para>
    /// </summary>
    /// <remarks> 
    /// <para>
    /// Internally uses the .NET Managed Provider from PostgreSQL (Npgsql) to connect to the database.
    /// </para>  
    /// </remarks>
    public class NpgsqlDatabase : Database
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="NpgsqlDatabase"/> class.
        /// </summary>
        public NpgsqlDatabase()
            : base()
        {
        }

        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the PostgreSQL Database Checkbox © stored procedures .</para>
        /// </summary>
        protected override char ParameterToken
        {
            get { return ':'; }
        }

        /// <summary>
        /// <para>Get the connection for this database.</para>
        /// <seealso cref="IDbConnection"/>
        /// <seealso cref="NpgsqlConnection"/>
        /// </summary>
        /// <returns>
        /// <para>The <see cref="NpgsqlConnection"/> for this database.</para>
        /// </returns>
        public override IDbConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }

        /// <summary>
        /// <para>Create a <see cref="NpgsqlCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="NpgsqlCommandWrapper"/> for the stored procedure.</para></returns>
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

            return new NpgsqlCommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken);
        }

        /// <summary>
        /// <para>Create an <see cref="NpgsqlCommandWrapper"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="NpgsqlCommandWrapper"/> for the stored procedure.</para></returns>
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

            return new NpgsqlCommandWrapper(storedProcedureName, CommandType.StoredProcedure, ParameterToken, parameterValues);
        }

        /// <summary>
        /// <para>Create an <see cref="NpgsqlCommandWrapper"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="NpgsqlCommandWrapper"/> for the SQL query.</para></returns>
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

            return new NpgsqlCommandWrapper(query, CommandType.Text, ParameterToken);
        }

        /// <summary>
        /// <para>Create a <see cref="NpgsqlDataAdapter"/> with the given update behavior and connection.</para>
        /// </summary>
        /// <param name="updateBehavior">
        /// <para>One of the <see cref="UpdateBehavior"/> values.</para>
        /// </param>
        /// <param name="connection">
        /// <para>The open connection to the database.</para>
        /// </param>
        /// <returns>An <see cref="NpgsqlDataAdapter"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        protected override DbDataAdapter GetDataAdapter(UpdateBehavior updateBehavior, IDbConnection connection)
        {
            string queryStringToBeFilledInLater = String.Empty;
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(queryStringToBeFilledInLater, (NpgsqlConnection)connection);

            if (updateBehavior == UpdateBehavior.Continue)
            {
                adapter.RowUpdated += new NpgsqlRowUpdatedEventHandler(OnNpgsqlRowUpdated);
            }
            return adapter;
        }

        /// <summary>
        /// <para>Executes the <see cref="NpgsqlCommandWrapper"/> and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="NpgsqlCommandWrapper"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(NpgsqlCommandWrapper command)
        {
            IDbConnection connection = OpenConnection();
            PrepareCommand(command, connection);
            NpgsqlCommand mySqlCommand = command.Command as NpgsqlCommand;
            return DoExecuteXmlReader(mySqlCommand);
        }

        /// <summary>
        /// <para>Executes the <see cref="NpgsqlCommandWrapper"/> in a transaction and returns an <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="NpgsqlCommandWrapper"/> to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(NpgsqlCommandWrapper command, IDbTransaction transaction)
        {
            PrepareCommand(command, transaction);

            NpgsqlCommand mySqlCommand = command.Command as NpgsqlCommand;
            return DoExecuteXmlReader(mySqlCommand);
        }

        /// <devdoc>
        /// Execute the actual Xml Reader call.
        /// </devdoc>        
        private XmlReader DoExecuteXmlReader(NpgsqlCommand sqlCommand)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                throw;
            }
        }

        /// <devdoc>
        /// Listens for the RowUpdate event on a dataadapter to support UpdateBehavior.Continue
        /// </devdoc>
        private void OnNpgsqlRowUpdated(object sender, NpgsqlRowUpdatedEventArgs rowThatCouldNotBeWritten)
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