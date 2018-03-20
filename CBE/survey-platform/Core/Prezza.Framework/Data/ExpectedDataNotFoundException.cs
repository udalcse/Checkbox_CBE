//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.Data
{
	/// <summary>
	/// Describes an exception related to missing data when performing a query on a datasource
	/// </summary>
	public class ExpectedDataNotFoundException : System.Exception
	{
		/// <summary>
		/// Describes an exception related to missing data when performing a query on a datasource
		/// </summary>
		public ExpectedDataNotFoundException() : this("The requested data was not found in the datasource")
		{
		}

		/// <summary>
		/// Describes an exception related to missing data when performing a query on a datasource
		/// </summary>
		/// <param name="message">Message for exception.</param>
		public ExpectedDataNotFoundException(string message) : base(message)
		{
		}
	}
}
