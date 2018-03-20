using System;
using System.Collections.Generic;
using Checkbox.Security.Principal;
using JWT;
using System.Web;

namespace Checkbox.Management
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtLogin : IHttpHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckboxPrincipal CurrentPrincipal { get; set; }
        private readonly string _sharedKey = ApplicationManager.AppSettings.JwtAccessKey;
        private const string Subdomain = "checkbox";

        public void ProcessRequest(HttpContext context)
        {
            var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            var timestamp = (int) t.TotalSeconds;

            var organization = context.Request.Url.Host.ToLower()
                .Replace(".checkbox.com", string.Empty)
                .Replace(".ckbxeu.com", string.Empty)
                .Replace(".ckbxap.com", string.Empty)
                .Replace(".checkboxonline.com", string.Empty);

            var payload = new Dictionary<string, object>()
                {
                    {"iat", timestamp},
                    {"jti", System.Guid.NewGuid()},
                    {"name", CurrentPrincipal.Email},
                    {"email", CurrentPrincipal.Email},
                    {"external_id", CurrentPrincipal.UserGuid},
                    {"organization", organization},
                };

            var token = JsonWebToken.Encode(payload, _sharedKey, JwtHashAlgorithm.HS256);
            context.Response.Redirect(string.Format("https://{0}.zendesk.com/access/jwt?jwt={1}", Subdomain, token));
        }

        public bool IsReusable { get; set; }
    }
}
