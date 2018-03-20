using System;
using System.Data;
using System.Web;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Data;
using Checkbox.Forms.Validation;
namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RequestSupport : SecuredPage
    {

        [QueryParameter("o")]
        public string OverView { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //protected override string PageRequiredRolePermission { get { return "Form.Edit"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (!ApplicationManager.AppSettings.EnableMultiDatabase && OverView == "t")
            {
                Response.Redirect("https://www.checkbox.com/support/overview");
                return;
            }

            if (!ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                Response.Redirect("https://www.checkbox.com/support/ticket");
                return;
            }
            
            Master.SetTitle("Request Support for " + Request.Headers["host"]);

            var isValid = new EmailValidator().Validate(CurrentPrincipal.Email);

            if(!isValid)
            {
                _warningPanel.Visible = true;
                return;
            }

            var zendeskLoginRequest = new JwtLogin {CurrentPrincipal = CurrentPrincipal};
            zendeskLoginRequest.ProcessRequest(HttpContext.Current);
        }

    }
}