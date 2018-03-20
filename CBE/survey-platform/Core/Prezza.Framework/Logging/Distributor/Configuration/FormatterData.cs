//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Base class for log formatter configuration.
	/// </summary>
	public abstract class FormatterData : ProviderData
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the formatter.</param>
		/// <param name="typeName">Type name of the formatter.</param>
		protected FormatterData(string name, string typeName) : base(name, typeName)
		{
		}
	}
}
