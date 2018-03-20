using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;

namespace Checkbox.Web.Mail
{
    /// <summary>
    /// Configuration class for system smtp email provider data
    /// </summary>
    public class SystemSmtpEmailProviderData : ProviderData, IXmlConfigurationBase
    {
        #region IXmlConfigurationBase Members

        ///<summary>
        ///</summary>
        ///<param name="providerName"></param>
        public SystemSmtpEmailProviderData(string providerName)
            : base(providerName, typeof(SystemSmtpEmailProvider).AssemblyQualifiedName)
        {
            ArgumentValidation.CheckForEmptyString(providerName, "providerName");
        }

        /// <summary>
        /// Required method
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
        }

        #endregion
    }
}