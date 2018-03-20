using System;
using System.Configuration;
using System.Xml;

using Prezza.Framework.Common;
using Prezza.Framework.Security;
using Prezza.Framework.Configuration;

namespace Checkbox.Security.Providers
{
	/// <summary>
	/// Summary description for DbAuthorizationProviderData.
	/// </summary>
	internal class DbAuthorizationProviderData : ProviderData, IXmlConfigurationBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="providerName">Name of the authorization provider.</param>
		public DbAuthorizationProviderData(string providerName) : base(providerName, typeof(DbAuthorizationProvider).AssemblyQualifiedName)
		{
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
		}

		/// <summary>
		/// Load authorization data from XML.
		/// </summary>
		/// <param name="providerData">Node containing authorization provider configuration informaiton.</param>
		public void LoadFromXml(XmlNode providerData)
		{
		}
	}
}
