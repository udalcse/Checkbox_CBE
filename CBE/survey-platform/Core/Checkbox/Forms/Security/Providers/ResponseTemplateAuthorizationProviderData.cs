using System.Xml;
using Prezza.Framework.Configuration;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Security.Providers
{
    /// <summary>
    /// Authorization provider specific to response templates.
    /// </summary>
    public class ResponseTemplateAuthorizationProviderData : ProviderData, IXmlConfigurationBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="providerName">Name of the authorization provider.</param>
        public ResponseTemplateAuthorizationProviderData(string providerName)
            : this(providerName, typeof(ResponseTemplateAuthorizationProvider).AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Another constructor.
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="typeName"></param>
        public ResponseTemplateAuthorizationProviderData(string providerName, string typeName)
            : base(providerName, typeName)
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
