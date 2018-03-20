//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml;
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Caching.Configuration
{
	/// <summary>
	/// Data object for cache storage
	/// </summary>
	public class CacheStorageData : ProviderData, IXmlConfigurationBase
	{
        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="typeName"></param>
		public CacheStorageData(string providerName, string typeName) : base(providerName, typeName)
		{
		}
		#region IXmlConfigurationBase Members
		
        /// <summary>
        /// Load the data from xml
        /// </summary>
        /// <param name="node"></param>
		public virtual void LoadFromXml(XmlNode node)
		{
			
		}

		#endregion
	}
}
