//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Base class for log sink configuration information.
	/// </summary>
	public abstract class SinkData : ProviderData
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected SinkData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
		protected SinkData(string name) : this(name, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
		/// <param name="typeName">Type name of the log sink.</param>
		protected SinkData(string name, string typeName) : base(name, typeName)
		{
		}
	}
}
