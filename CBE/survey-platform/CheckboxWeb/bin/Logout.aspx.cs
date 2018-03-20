using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Checkbox.Management;
using Checkbox.Users;

namespace CheckboxWeb
{
    /// <summary>
    /// Simple logout page to abandon sessions and clear up any cookies.  Eventually it could support redirecting
    /// to a different URL other than login.aspx
    /// </summary>
    public partial class Logout : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var currentPrincipal = UserManager.GetCurrentPrincipal();

            if (currentPrincipal != null)
            {
                UserManager.ExpireLoggedInUser(currentPrincipal.Identity.Name);
                UserManager.ExpireCachedPrincipal(currentPrincipal.Identity.Name);
            }

            //Logout forms authentication
            FormsAuthentication.SignOut();

            //Abandon session & clear session cookie
            Session.Abandon();

            if (ApplicationManager.AppSettings.PreventSessionReuse)
            {
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            }

            Response.Redirect("Login.aspx", false);
        }
    }
}
