//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;

using Prezza.Framework.Common;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Interface for configuration objects that support loading configuration information from Xml.
	/// </summary>
	public interface IXmlConfigurationBase
	{
		/// <summary>
		/// Load configuration information from the specified XML node.
		/// </summary>
		/// <param name="node">XML Node containing the configuration information to load.</param>
		/// <remarks>
		/// Implementations of the IXmlConfigurationBase interface must implement this method.  The data contained in the XML node
		/// may come from any source, including one or more XML files, database(s), etc.
		/// </remarks>
		void LoadFromXml(XmlNode node);

	}
}
