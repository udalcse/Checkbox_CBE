using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Security.Providers;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Web.Providers;
using Newtonsoft.Json.Linq;
using Checkbox.Security;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Login : ApplicationPage
    {
        [QueryParameter("u")]
        public Guid? UserGuid { get; set; }

        private const string AdministratorRole = "Administrator";

        /// <summary>
        /// Handle page init
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Abandon session
            Session.Abandon();

            //Process automatic login
            if (UserGuid.HasValue)
            {
                var userPrincipal = UserManager.AuthenticateUser(UserGuid.Value);

                if (userPrincipal != null)
                {
                    if (!(ApplicationManager.AppSettings.PreventAdminAutoLogin && userPrincipal.IsInRole(AdministratorRole)))
                    {
                        SetAuthCookieAndRedirect(userPrincipal.Identity.Name);
                    }
                }
            }

            //Set links visibility
            _lnkNewUser.Visible = ApplicationManager.AppSettings.AllowPublicRegistration;
            _lnkLoginLookup.Visible = ApplicationManager.AppSettings.AllowPasswordReset &&
                                      ApplicationManager.AppSettings.EmailEnabled;

            //Set focus
            UserName.Focus();

            //Bind login
            LoginBtn.Click += LoginButton_Click;

            Page.ClientScript.RegisterStartupScript(GetType(), "AppRoot", "setApplicationRoot('" + ApplicationManager.ApplicationRoot + "');", true);
        }


        #region SpoofPrevention

        //Attempt to prevent session/login cookie spoofing using technique described at:
        // http://support.microsoft.com/kb/899918

        /// <summary>
        /// Override onload to try to ensure a new session id is generated when logging in and
        /// that a client-provided session id is not used.
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (ApplicationManager.AppSettings.TypeKitGuid != null)
                TypeKit.Text =
                    "<script type=\"text/javascript\" src=\"//use.typekit.net/" + ApplicationManager.AppSettings.TypeKitGuid +
                    ".js\"></script><script type=\"text/javascript\">try{Typekit.load();}catch(e){}</script>";

            if (ApplicationManager.AppSettings.PreventSessionReuse)
            {
                if (!IsPostBack
                    && (Request.Cookies["__LOGINCOOKIE__"] == null
                        || string.IsNullOrEmpty(Request.Cookies["__LOGINCOOKIE__"].Value)))
                {
                    //At this point, we do not know if the session ID that we have is a new
                    //session ID or if the session ID was passed by the client. 
                    //Update the session ID.

                    Session.Abandon();
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

                    //To make sure that the client clears the session ID cookie, respond to the client to tell 
                    //it that we have responded. To do this, set another cookie.
                    AddRedirCookie();
                    Response.Redirect(Request.Path, false);
                    return;
                }

                //Make sure that someone is not trying to spoof.
                try
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies["__LOGINCOOKIE__"].Value);

                    if (ticket == null || ticket.Expired)
                    {
                        throw new Exception();
                    }

                    RemoveRedirCookie();
                }
                catch
                {
                    //If someone is trying to spoof, do it again.
                    AddRedirCookie();
                    Response.Redirect(Request.Path, false);
                }
                //Debugging
                //Response.Write("Session.SessionID=" + Session.SessionID + "<br/>");
                //Response.Write("Cookie ASP.NET_SessionId=" + Request.Cookies["ASP.NET_SessionId"].Value + "<br/>");
            }
        }

        /// <summary>
        /// Remove cookie
        /// </summary>
        private void RemoveRedirCookie()
        {
            Response.Cookies.Add(new HttpCookie("__LOGINCOOKIE__", ""));
        }

        //Add short-lived cookie to indicate redirction should occur
        private void AddRedirCookie()
        {

            var ticket = new FormsAuthenticationTicket(1, "Test", DateTime.Now, DateTime.Now.AddMinutes(5), false, "");
            string encryptedText = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie("__LOGINCOOKIE__", encryptedText));
        }

        #endregion


        /// <summary>
        /// Creates an error text using fails count
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string buildLoginError(string userName)
        {
            Dictionary<string, object> userProperties = null;

            var provider = Membership.Provider as ICheckboxMembershipProvider;
            if (provider != null)
                userProperties = provider.GetUserIntrinsicProperties(userName);

            if (userProperties != null)
            {
                bool locked = userProperties["LockedOut"] != null && (bool)userProperties["LockedOut"];
                if (locked)
                {
                    return "The account is locked.";
                }

                int fails = userProperties["FailedLogins"] == null ? 0 : (int)userProperties["FailedLogins"];
                
                if (ApplicationManager.AppSettings.MaxFailedLoginAttempts - fails < 2)
                    return string.Format("Failed login attempts: {0}. Please note, after 1 more failed attempt, the account will be locked.", fails);

                return string.Format("Failed login attempts: {0}. Please note, after {1} more failed attempts, the account will be locked.", fails, ApplicationManager.AppSettings.MaxFailedLoginAttempts - fails);
            }

            return "Login failed, please try again";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        void showLoginError(string userName)
        {
            string errorText;

            if (!UserManager.IsDomainUser(userName) && UserManager.IsCheckboxUser(userName)
                && ApplicationManager.AppSettings.MaxFailedLoginAttempts > 0)
            {
                errorText = buildLoginError(userName);
            }
            else if (ApplicationManager.AppSettings.IsPrepMode && !UserManager.GetUserPrincipal(userName).IsRoleContains("Administrator"))
            {
                errorText = "System is in Prep Mode - contact an administrator for assistance";
            }
            else {
                errorText = "Login failed, please try again";
            }

            _loginError.Controls.Clear();
            _loginError.Controls.Add(new System.Web.UI.WebControls.Label());
            ((System.Web.UI.WebControls.Label)_loginError.Controls[0]).Text = errorText;
            _loginError.Style["display"] = "block";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginButton_Click(object sender, EventArgs e)
        {
            _loginError.Visible = false;

            if (!Page.IsValid)
            {
                return;
            }            

            var userName = ApplicationManager.AppSettings.AllowHTMLNames ? UserName.Text.Trim() : Server.HtmlEncode(UserName.Text.Trim());
            var password = Password.Text.Trim();
            if (ApplicationManager.AppSettings.IsPrepMode && !UserManager.GetUserPrincipal(userName).IsRoleContains(AdministratorRole))
            {
                showLoginError(userName);
                _loginError.Visible = true;
                return;
            }
            
            if ((!MembershipProviderManager.DisableForeignProviders ||
                Membership.Provider.Name == MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME)
                && Membership.ValidateUser(userName, password))
                SetAuthCookieAndRedirect(userName);

            showLoginError(userName);
            _loginError.Visible = true;
        }

        private void SetAuthCookieAndRedirect(string userName)
        {
            FormsAuthentication.SetAuthCookie(userName, false);
            HttpContext.Current.User = UserManager.GetUserPrincipal(userName);
            
            //the  Maintenance Notices must be visible for the Online version only
            if (ApplicationManager.AppSettings.EnableMultiDatabase && !RoleManager.IsJustARespondent(userName))
            {
                try
                {
                    // Check for Maintenance Notices
                    var data = new WebClient().DownloadString("https://www.checkbox.com/api/core/get_category_posts/?callback=?&slug=maintenance-notices&custom_fields=AppMessageEndDate&count=1");
                    data = data.Substring(0, 2) == "?(" ? data.Substring(2) : data;
                    data = data.Substring(data.Length - 1) == ")" ? data.Substring(0, data.Length - 1) : data;

                    // If notices exist, redirect the user to the roadblock screen to show them
                    // If posts exist, make sure one is active based on (potential) end date restriction
                    var noticePosts = JObject.Parse(data)["posts"];
                    var activeNotice =
                        noticePosts.Select(post => (JObject)post["custom_fields"])
                                   .Select(
                                       customFields =>
                                       (customFields["AppMessageEndDate"] != null &&
                                        ((JArray)customFields["AppMessageEndDate"])[0] != null)
                                           ? Convert.ToDateTime((string)(((JArray)customFields["AppMessageEndDate"])[0]))
                                           : (DateTime?)null)
                                   .Any(
                                       endDate =>
                                       endDate == null ||
                                       (endDate >=
                                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)));

                    // If there's an active notice, redirect the user to show it
                    if (activeNotice)
                    {
                        // Redirect the user to the notice page
                        var redirUrl = ResolveUrl("~/Notice.aspx");
                        var returnUrl = Request.QueryString["returnurl"] ?? "";

                        returnUrl = returnUrl.Split(',').LastOrDefault();

                        if (!string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/" || returnUrl.Equals("/Default.aspx", StringComparison.InvariantCultureIgnoreCase))
                        {
                            redirUrl += "?ReturnUrl=" + Utilities.RemoveScript(returnUrl);
                        }

                        Response.Redirect(redirUrl);
                    }
                }
                catch (Exception)
                {
                    //suppress exceptions
                }
            }

            // Redirect the user to the proper page
            var redirect = Utilities.RemoveScript(Request.QueryString["returnurl"] ?? "");
            var url = redirect.Split(',').LastOrDefault();

            if (string.IsNullOrWhiteSpace(url) || url == "/")
                Response.Redirect(UserDefaultRedirectUrl);

            Response.Redirect(url);
        }
    }
}
