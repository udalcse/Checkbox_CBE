using System;
using System.Collections.Generic;
using System.Security.Principal;
using Checkbox.Security.Principal;


namespace Checkbox.Forms.Security.Principal
{
    /// <summary>
    /// Special principal used to represent anonymous survey respondents.
    /// </summary>
    [Serializable]
    public class AnonymousRespondent : CheckboxPrincipal
    {
        public const string IDENTITY_NAME = "AnonymousRespondent";

        /// <summary>
        /// Create an anonymous respondent
        /// </summary>
        public AnonymousRespondent(Guid respondentGuid)
            : base(new GenericIdentity(IDENTITY_NAME, "Anonymous Respondent"), null, DefaultRoles, null)
        {
            UserGuid = respondentGuid;
        }

        /// <summary>
        /// Get default roles for an anonymous respondent
        /// </summary>
        protected static string[] DefaultRoles
        {
            get { return new[] { "Respondent", "Report Viewer" }; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override Dictionary<string, string> LoadProfileProperties()
        {
            var profileProperties = new Dictionary<string, string>();

            profileProperties["GUID"] = UserGuid.ToString();
            profileProperties["UserName"] = Identity.Name;
            profileProperties["Email"] = Email ?? string.Empty;

            return profileProperties;
        }
    }
}
