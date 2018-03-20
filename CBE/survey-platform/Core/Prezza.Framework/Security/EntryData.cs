//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Prezza Technologies, Inc.  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

using Prezza.Framework.Common;

namespace Prezza.Framework.Security
{
	/// <summary>
	/// Summary description for EntryData.
	/// </summary>
	public class EntryData
	{
		private string entryType;
		private string entryIdentifier;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entryType">Entry type string.</param>
		/// <param name="entryIdentifier">Entry identifier.</param>
		public EntryData(string entryType, string entryIdentifier)
		{
			ArgumentValidation.CheckForEmptyString(entryIdentifier, "entryIdentifier");
			// allow null entryType
			this.entryIdentifier = entryIdentifier;
			this.entryType = entryType;
		}

		/// <summary>
		/// Get the entry type.
		/// </summary>
		public string EntryType
		{
			get { return entryType; }
		}

		/// <summary>
		/// Get the entry identifier.
		/// </summary>
		public string EntryIdentifier
		{
			get { return entryIdentifier; }
		}
	}
}
