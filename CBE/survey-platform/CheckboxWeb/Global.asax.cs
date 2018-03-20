using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Optimization;
using System.Web.Security;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Web;
using Checkbox.Web.Providers;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Configuration=System.Configuration.Configuration;
using Checkbox.Messaging.Email;

namespace CheckboxWeb
{
    public class Global : HttpApplication
    {
        private const string CHAINING_ROLE_PROVIDER_NAME = "ChainingRoleProvider";
        private const string CHECKBOX_ROLE_PROVIDER_NAME = "CheckboxRoleProvider";

        private const string CHAINING_PROFILE_PROVIDER_NAME = "ChainingProfileProvider";
        private const string CHECKBOX_PROFILE_PROVIDER_NAME = "CheckboxProfileProvider";


        void Application_Start(object sender, EventArgs e)
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Initialize manager classes with web providers
        /// </summary>
        protected void InitializeApplication()
        {
            Checkbox.Management.Licensing.CheckboxLicenseProvider.Path = HttpContext.Current.Request.PhysicalApplicationPath;

            //Initialize context providers if multi-db enabled
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                var contextProvider = new SessionDataContextProvider();

                //Set the context provider for application manager for use in Checkbox components
                ApplicationManager.SetApplicationDataContextProvider(contextProvider);
                DatabaseLoggingExceptionHandler.Initialize(contextProvider);

                //Set for use by framework components
                DatabaseFactory.Initialize(contextProvider);
                CacheFactory.Initialize(contextProvider);
            }

            //Attempt to read forms authentication timeout
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            var authentication = (AuthenticationSection)config.GetSection("system.web/authentication");

            UserManager.SetAuthTimeout((int)authentication.Forms.Timeout.TotalMinutes);

            //initialize membership manager
            //Check for either chaining provider or checkbox provider for membership, roles, and profiles
            MembershipProviderManager.Initialize(Membership.Providers);
            UserManager.Initialize(MembershipProviderManager.DefaultProvider);

            //Roles);
            if (Roles.Providers[CHAINING_ROLE_PROVIDER_NAME] != null)
            {
                RoleManager.Initialize((ICheckboxRoleProvider)Roles.Providers[CHAINING_ROLE_PROVIDER_NAME]);
            }
            else if (Roles.Providers[CHECKBOX_ROLE_PROVIDER_NAME] != null)
            {
                RoleManager.Initialize((ICheckboxRoleProvider)Roles.Providers[CHECKBOX_ROLE_PROVIDER_NAME]);
            }
            else
            {
                throw new Exception(string.Format("At least one of {0} or {1} must be defined in roles section of web.config", CHAINING_ROLE_PROVIDER_NAME, CHECKBOX_ROLE_PROVIDER_NAME));
            }

            //Profile
            if (System.Web.Profile.ProfileManager.Providers[CHAINING_PROFILE_PROVIDER_NAME] != null)
            {
                ProfileManager.Initialize((ICheckboxProfileProvider)System.Web.Profile.ProfileManager.Providers[CHAINING_PROFILE_PROVIDER_NAME]);
            }
            else if (System.Web.Profile.ProfileManager.Providers[CHECKBOX_PROFILE_PROVIDER_NAME] != null)
            {

                ProfileManager.Initialize((ICheckboxProfileProvider)System.Web.Profile.ProfileManager.Providers[CHECKBOX_PROFILE_PROVIDER_NAME]);
            }
            else
            {
                throw new Exception(string.Format("At least one of {0} or {1} must be defined in profiles section of web.config", CHAINING_PROFILE_PROVIDER_NAME, CHECKBOX_PROFILE_PROVIDER_NAME));
            }

            if (ApplicationManager.AppSettings.InstallSuccess) //avoid exception on setup
            {
                //EMail Providers
                InitializeEMailProvider();
            }
        }

        /// <summary>
        /// Changes the default email provider overriding Default from attribute from Messaging.xml
        /// </summary>
        public static void InitializeEMailProvider()
        {
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
                return;

            //that works for server version only
            if (!string.IsNullOrEmpty(ApplicationManager.AppSettings.MSSMode))
            {
                if (ApplicationManager.AppSettings.MSSMode == "SES")
                {
                    if (!EmailGateway.ProviderSupportsBatches)
                        EmailGateway.ChangeEmailProvider("databaseRelayEmailProvider");
                }
                else
                {
                    if (EmailGateway.ProviderSupportsBatches)
                        EmailGateway.ChangeEmailProvider("SystemSmtpEmailProvider");
                }
            }
        }


        /// <summary>
        /// Process an application request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            Checkbox.Management.Licensing.CheckboxLicenseProvider.Path = HttpContext.Current.Request.PhysicalApplicationPath;

            //Do no processing for error pages
            if (Request.Url.LocalPath.ToLower().Contains("/errorpages/"))
                return;

            //Set context for multi db mode
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                string appContext = Request.Headers["Host"];

                HttpContext.Current.Items[DataContextProvider.APPLICATION_CONTEXT_KEY] = appContext;
                Thread.SetData(Thread.GetNamedDataSlot(DataContextProvider.APPLICATION_CONTEXT_KEY), appContext);

                Thread.SetData(
                    Thread.GetNamedDataSlot(DataContextProvider.REQUEST_SECURED_KEY + appContext), 
                    HttpContext.Current.Request.IsSecureConnection);
            }

            //Otherwise ensure configuration validated for non-install pages
            if(!Request.Url.LocalPath.ToLower().Contains("/install/"))
            {
                if (Application["ConfigurationValidated"] == null || (bool)Application["ConfigurationValidated"] != true)
                {
                    string status;
                    if (!Checkbox.Configuration.ConfigurationValidator.ValidateConfiguration(Server.MapPath(WebUtilities.ResolveUrl("~")), out status))
                    {
                        Application["ConfigurationValidated"] = false;
                        Application["ConfigurationErrors"] = status;
                    }
                    else
                    {
                        Application["ConfigurationValidated"] = true;
                        InitializeApplication(); // <-- all the heavy lifting occurs here 
                    }
                }
            }

            //If application was not installed

            if (!ApplicationManager.AppSettings.InstallSuccess && 
                (Request.AppRelativeCurrentExecutionFilePath.ToLower().Equals("~/") ||
                Request.AppRelativeCurrentExecutionFilePath.ToLower().Equals("~/login.aspx")))
            {
                Response.Redirect(WebUtilities.ResolveUrl("~/Install/Default.aspx"));
            }
            //If in multi-db mode, validate that the current context is active
            else if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                if (!ApplicationManager.IsDataContextActive) // if hosted account is inactive then redirect to error page
                    Response.Redirect(WebUtilities.ResolveUrl("~/ErrorPages/ApplicationNotActive.aspx"), true);

                if(ApplicationManager.AppSettings.RedirectHTTPtoHTTPS && !Request.IsSecureConnection) // check force https setting
                    Response.Redirect(Request.Url.AbsoluteUri.Replace(Uri.UriSchemeHttp, Uri.UriSchemeHttps));
            }
            // check the force https settings only if app url in web.config is configured to use ssl
            else if (ApplicationManager.ApplicationURL.Contains("https:") &&
                (ApplicationManager.AppSettings.RedirectHTTPtoHTTPS && !Request.IsSecureConnection))
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace(Uri.UriSchemeHttp, Uri.UriSchemeHttps));
            }
        }

        /// <summary>
        /// Provide automatic authentication of IIS users, if enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //Do nothing if config not valid
            if (Application["ConfigurationValidated"] == null || (bool) Application["ConfigurationValidated"] != true)
            {
                return;
            }

            //Do nothing for login or install pages
            if (Request.Url.LocalPath.ToLower().Contains("login.aspx")
                && Request.Url.LocalPath.Contains("/Install/"))
            {
                return;
            }

            var currentUserName = string.Empty;

            //If enabling automatic login for network users, check for auto-login
            if (ApplicationManager.AppSettings.InstallSuccess //avoid exception during install
                && ApplicationManager.AppSettings.NTAuthentication
                && !string.IsNullOrEmpty(ApplicationManager.AppSettings.NTAuthenticationVariableName)
                && !string.IsNullOrEmpty(
                        Request.ServerVariables[ApplicationManager.AppSettings.NTAuthenticationVariableName]))
            {
                //Otherwise, authenticate
                currentUserName =
                    Request.ServerVariables[ApplicationManager.AppSettings.NTAuthenticationVariableName].Replace("\\",
                                                                                                                 "/");

                //Attempt to authenticate
                CheckboxPrincipal userPrincipal = UserManager.AuthenticateUser(currentUserName, string.Empty);

                //User is not authenticated, so do nothing
                if (userPrincipal == null)
                    currentUserName = string.Empty;
                else // user is authenticated, set current HttpContext.User to the authenticated user
                    Context.User = userPrincipal;
            }

            //If no auto-login user, check current principal, if any
            if (string.IsNullOrEmpty(currentUserName)
                && Context.User != null
                && Context.User.Identity.IsAuthenticated)
            {
                currentUserName = Context.User.Identity.Name;
            }

            string path = Request.AppRelativeCurrentExecutionFilePath.ToLower();
            //if anonymus user tries to get a report or a survey preview, it could be pdf tool
            if ((path.Equals("~/runanalysis.aspx") || path.Equals("~/forms/surveys/preview.aspx"))
                && !string.IsNullOrEmpty(Request.Params["ticket"]) && string.IsNullOrEmpty(currentUserName))
            {
                var principal = UserManager.GetPrincipalByTicket(Request.Params["ticket"]);
                if (principal != null)
                    currentUserName = principal.Identity.Name;
            }

            //If no user, do nothing
            if (string.IsNullOrEmpty(currentUserName))
                return;

            //If user exists from the previous installation but this installation has not been finished, do nothing
            if (!ApplicationManager.AppSettings.InstallSuccess)
                return;

            // this checks the membership stores, Checkbox DB or Active Directory
            var user = Membership.GetUser(currentUserName);

            if (user == null)
                return;

            //If the user is locked, remove the user from logged user cache and clear cookies.
            if (user.IsLockedOut)
            {
                UserManager.ExpireLoggedInUser(currentUserName);
                
                //Logout forms authentication
                FormsAuthentication.SignOut();

                if (ApplicationManager.AppSettings.PreventSessionReuse)
                {
                    Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
                }

                Response.Redirect(FormsAuthentication.LoginUrl, false);
                return;
            }

            //Otherwise, flag user as authenticated
            FormsAuthentication.SetAuthCookie(currentUserName, false);
            Context.User = UserManager.GetUserPrincipal(currentUserName, true);

            //redirect from the root folder
            if (Request.AppRelativeCurrentExecutionFilePath.ToLower().Equals("~/") 
                || Request.AppRelativeCurrentExecutionFilePath.ToLower().Equals("~/default.aspx"))
            {
                // if there is no logged in user and the user is
                // being redirected, they should go to the login page
                if (!(User is CheckboxPrincipal))
                {
                    Response.Redirect(WebUtilities.ResolveUrl("~/Login.aspx"));
                    return;
                }

                // if user is logged in they should be redirected
                // to a page determined by their most important role
                if (User.IsInRole("System Administrator") || User.IsInRole("Survey Administrator") ||
                    User.IsInRole("Survey Editor") || User.IsInRole("Report Administrator"))
                {
                    Response.Redirect(WebUtilities.ResolveUrl("~/Forms/Default.aspx"));
                    return;
                }

                if (User.IsInRole("User Administrator"))
                {
                    Response.Redirect(WebUtilities.ResolveUrl("~/Users/Default.aspx"));
                    return;
                }

                if (User.IsInRole("Group Administrator"))
                {
                    Response.Redirect(WebUtilities.ResolveUrl("~/Users/Manage.aspx?m=g"));
                    return;
                }

                if (User.IsInRole("Report Viewer") && !User.IsInRole("Respondent"))
                {
                    Response.Redirect(WebUtilities.ResolveUrl("~/AvailableReports.aspx"));
                    return;
                }

                //by default redirect to available surveys
                Response.Redirect(WebUtilities.ResolveUrl("~/AvailableSurveys.aspx"));
            }
        }

        /// <summary>
        /// Implement post authenticate request to set the extended principal as the context user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            //Do nothing if config not valid
            if (Application["ConfigurationValidated"] == null || (bool) Application["ConfigurationValidated"] != true)
                return;

            if (!ApplicationManager.AppSettings.InstallSuccess)
                return;

            if (Request.IsAuthenticated)
            {
                //get the username which we previously set in   
                //forms authentication ticket in our login1_authenticate event   
                IPrincipal userPrincipal = UserManager.GetUserPrincipal(HttpContext.Current.User.Identity, true);

                //build a custom identity and custom principal object based on this username   
                HttpContext.Current.User = userPrincipal;
                Thread.CurrentPrincipal = userPrincipal;
            }   
        }


        /// <summary>
        /// Handle end request to encrypt session cookie when SSL used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Request.IsSecureConnection)
            {
                HttpCookie sessionCookie = Request.Cookies["ASP.NET_SessionId"];

                if (sessionCookie != null)
                    sessionCookie.Secure = true;
            }
        }

        /// <summary>
        /// Transfer to application error page when an error occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            //Attempt to log message
            try
            {
                ExceptionPolicy.HandleException(Server.GetLastError(), "UIProcess");
            }
            catch
            {
            }

            if(Request.Url.LocalPath.ToLower().Contains("/errorpages/"))
            {
                Response.Redirect("~/ErrorPages/ApplicationError.html", true);
                return;
            }
            //Attempt to prevent infinite redirect in case of error with application error
            // html, so only transfer for non-application error.html pages
            if (!Request.Url.LocalPath.ToLower().Contains("/errorpages/applicationerror.html"))
            {
                Server.Transfer("~/ErrorPages/ApplicationError.aspx");
            }
            
            //TODO: For XML and JSON services, write xml/json corresponding to service
            // operation result object w/appropriate error message.
            Exception ex = Server.GetLastError().GetBaseException();
            if (ex.Message == "Maximum request length exceeded.")
            {
                //because th 500th error cannot be handled using the javascript we have to 
                Server.ClearError();
                HttpContext.Current.Response.Write(ex.Message);
            }

            //Otherwise fall through and let web server handle, including any custom
            // handlers defined in web.config
        }

        /// <summary>
        /// Redirect if the session has not been validated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(Object sender, EventArgs e)
        {
            var url = Request.Url.ToString().ToLower();

            if (url.IndexOf("/install/") < 0)
            {
                if (Application["ConfigurationValidated"] != null && (bool)Application["ConfigurationValidated"] == false)
                {
                    if (url.IndexOf("ConfigurationError.aspx") < 0)
                    {
                        Response.Redirect(WebUtilities.ResolveUrl("~/ErrorPages/ConfigurationError.aspx"), true);
                    }

                    return;
                }

                //Ensure app setting is in line with session timeout
                ApplicationManager.AppSettings.SessionTimeOut = Session.Timeout;
            }
        }

        /// <summary>
        /// Attempt to shutdown workflows gracefully on application end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                SurveyResponseServiceImplementation.Finalize();
            }
            finally { }
        }
    }
}
