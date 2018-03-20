//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;
using System.Data;

namespace Prezza.Framework.Data
{
	/// <summary>
	/// Summary description for DbTransactionWrapper.
	/// </summary>
	public class DbNonQueryTransactionWrapper
	{
		/// <summary>
		/// An <see cref="ArrayList"/> collection of <see cref="DbNonQueryTransactionWrapper.NonQuery"/> objects
		/// </summary>
		public ArrayList NonQueries = new ArrayList();

		/// <summary>
		/// Adds a <see cref="DbNonQueryTransactionWrapper.NonQuery"/> to the transaction
		/// </summary>
		/// <param name="commandText">a sql command to execute</param>
		public void AddNonQuery(string commandText)
		{
			NonQueries.Add(new NonQuery(commandText, CommandType.Text, null));
		}

		/// <summary>
		/// Adds a <see cref="DbNonQueryTransactionWrapper.NonQuery"/> to the transaction
		/// </summary>
		/// <param name="storedProcedureName">a stored procedure to execute</param>
		/// <param name="commandParameters">a parameterized array of IDataParamater objects</param>
		public void AddNonQuery(string storedProcedureName, params object[] commandParameters)
		{
			NonQueries.Add(new NonQuery(storedProcedureName, CommandType.StoredProcedure, commandParameters));
		}

		/// <summary>
		/// A Parameter class to hold NonQuery info used in execution within a DbNonQueryTransactionWrapper
		/// </summary>
		public class NonQuery
		{
			/// <summary>
			/// The <see cref="CommandType"/> to execute
			/// </summary>
			public CommandType CommandType;
			/// <summary>
			/// The CommandText
			/// </summary>
			public string CommandText;
			/// <summary>
			/// A collection of IDataParameters
			/// </summary>
			public object[] CommandParameters;

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="commandText">the CommandText</param>
			/// <param name="type">the <see cref="CommandType"/></param>
			/// <param name="commandParameters">a collection of IDataParameters</param>
			public NonQuery(string commandText, CommandType type, params object[] commandParameters)
			{
				this.CommandType = type;
				this.CommandText = commandText;
				this.CommandParameters = commandParameters;
			}
		}
	}
}
