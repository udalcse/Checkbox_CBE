//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Configuration;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Configuration information for an exception policy.
	/// </summary>
	public class ExceptionPolicyData : ProviderData
	{
		/// <summary>
		/// Collection of exception types that the exception policy defines handlers for.
		/// </summary>
		private ExceptionTypeDataCollection exceptionTypes;

		/// <summary>
		/// Constructor.  Sets the policy name to an empty string and initializes the exception type
		/// configuration collection.
		/// </summary>
		public ExceptionPolicyData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.  Initializes the exception type collection.
		/// </summary>
		/// <param name="name">Name of the exception policy.</param>
		public ExceptionPolicyData(string name) : base(name, typeof(ExceptionPolicy).AssemblyQualifiedName)
		{
			exceptionTypes = new ExceptionTypeDataCollection();
		}

        /// <summary>
        /// Get the collection of exception type configurations for exception types handled by the policy.
        /// </summary>
		public ExceptionTypeDataCollection ExceptionTypes
		{
			get{return exceptionTypes;}
		}
	}
}
