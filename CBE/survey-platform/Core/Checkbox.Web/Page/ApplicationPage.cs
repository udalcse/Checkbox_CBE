using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Common;
using Checkbox.Web.Common;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base application functionality for Checkbox Web pages, including interaction with master page
    /// </summary>
    public abstract class ApplicationPage : LicenseProtectedPage
    {
        private bool _initErrorOccurred;
        protected delegate void SimpleEventHandler();

        /// <summary>
        /// Set the init error flag to true which will cause other
        /// overridable OnPage...() methods to not be called.
        /// </summary>
        protected virtual void RaiseInitializationErrorFlag()
        {
            _initErrorOccurred = true;
        }

        /// <summary>
        /// Get the initialization error flag value.
        /// </summary>
        /// <remarks>True indicates an error occurred.</remarks>
        protected virtual bool InitializationErrorFlag { get { return _initErrorOccurred; } }

        /// <summary>
        /// Overridable pre init method
        /// </summary>
        protected virtual void OnPagePreInit(){ }

        /// <summary>
        /// Overridable init method
        /// </summary>
        protected virtual void OnPageInit() { }

        /// <summary>
        /// Overridable pre load method
        /// </summary>
        protected virtual void OnPagePreLoad() { }

        /// <summary>
        /// Overridable load method
        /// </summary>
        protected virtual void OnPageLoad() { }

        /// <summary>
        /// Overridable pre render event
        /// </summary>
        protected virtual void OnPagePreRender() { }

        /// <summary>
        /// Overridable unload event
        /// </summary>
        protected virtual void OnPageUnLoad() { }

        /// <summary>
        /// Overridable flag controlling whether javascript localization data should be included in the page.
        /// </summary>
        public bool IncludeJsLocalization { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected virtual void ShowError(string message, Exception ex)
        {
            if (Master is BaseMasterPage)
            {
                ((BaseMasterPage)Master).ShowError(message, ex);
            }
        }

        /// <summary>
        /// On pre-init
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreInit(EventArgs e)
        {
            try
            {
                base.OnPreInit(e);
                
                WebParameterAttribute.SetValues(this, HttpContext.Current);

                OnPagePreInit();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                //Set a flag so that later processing doesn't occur in the event of an error
                RaiseInitializationErrorFlag();
            }
        }



        /// <summary>
        /// Handle init to bind events and wrap page initialization in try/catch
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            try
            {
                //Call base init
                base.OnInit(e);

                if (InitializationErrorFlag)
                {
                    return;
                }

                //Store user context
                SetUserContext();

                //Call overridable method
                OnPageInit();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                //Set a flag so that Page_Load processing doesn't occur in the event of an error
                // in OnInit
                RaiseInitializationErrorFlag();
            }
        }

        /// <summary>
        /// Set the user context for reporting on admin screens
        /// </summary>
        protected virtual void SetUserContext()
        {
            CheckboxPrincipal currentPrincipal = UserManager.GetCurrentPrincipal();

            string userName = currentPrincipal != null
                ? currentPrincipal.Identity.Name
                : "Anonymous";

            //Set the caller's context
            UserManager.SetUserContext(
                userName,
                HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"],
                HttpContext.Current.Request.ServerVariables["URL"]);
        }

        /// <summary>
        /// Handle page pre-load and wrap with a try/catch
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreLoad(EventArgs e)
        {
            try
            {
                base.OnPreLoad(e);

                if (InitializationErrorFlag)
                {
                    return;
                }

                OnPagePreLoad();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                RaiseInitializationErrorFlag();
            }
        }

        /// <summary>
        /// Handle page load and wrap in try/catch
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                //Call base
                base.OnLoad(e);

                if (InitializationErrorFlag)
                {
                    return;
                }

                //Add Js Localizatoin
                if (IncludeJsLocalization)
                {
                    RegisterClientScriptInclude(
                        GetType(),
                        "jsLocalize",
                        ResolveUrl("~/PageText.aspx?p=") + Server.UrlEncode(Request.AppRelativeCurrentExecutionFilePath));
                }

                //Call overridable on load
                OnPageLoad();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                //Raise the error flag
                RaiseInitializationErrorFlag();
            }
        }

        /// <summary>
        /// Wrap pre render logic in error handling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);

                if (InitializationErrorFlag)
                {
                    return;
                }

                //Call overridable pre render
                OnPagePreRender();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                //Raise the error flag
                RaiseInitializationErrorFlag();
            }
        }

        /// <summary>
        /// Wrap pre render logic in error handling
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            try
            {
                base.OnUnload(e);

                if (InitializationErrorFlag)
                {
                    return;
                }

                //Call overridable unload
                OnPageUnLoad();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);

                //Raise the error flag
                RaiseInitializationErrorFlag();
            }
        }

        /// <summary>
        /// Page state save
        /// </summary>
        /// <param name="state"></param>
        protected override void SavePageStateToPersistenceMedium(object state)
        {
            base.SavePageStateToPersistenceMedium(state);

            Memento.PersistMementos(this, HttpContext.Current);
        }

        /// <summary>
        /// Wrap event handling in a try-catch statement to catch errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="realHandler"></param>
        protected void EventHandlerWrapper(object sender, EventArgs e, EventHandler realHandler)
        {
            try
            {
                realHandler(sender, e);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);
            }
        }

        /// <summary>
        /// Wrap event handling in a try-catch statement to catch errors
        /// </summary>
        /// <param name="realHandler"></param>
        protected void EventHandlerWrapper(SimpleEventHandler realHandler)
        {
            try
            {
                realHandler();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
                
                ShowError(WebTextManager.GetText("/errorMessages/common/errorOccurred"), ex);
            }
        }


        ///// <summary>
        ///// Get a typed version of the base master page.
        ///// </summary>
        //protected BaseMasterPage TypedMaster
        //{
        //    get { return Master as BaseMasterPage; }
        //}

        /// <summary>
        /// Get a value with the specified name from the session.  If the value is null or cannot be cast to the
        /// specified type, a redirect to the default page occurs with the timeout message.
        /// </summary>
        /// <typeparam name="T">Type of object ot get</typeparam>
        /// <param name="name">Name of object.</param>
        /// <param name="defaultValue">Default value to return if the value is not found.</param>
        /// <returns>Typed object or null if not found.</returns>
        protected T GetSessionValue<T>(string name, T defaultValue)
        {
            return GetSessionValue(name, false, defaultValue);
        }


        /// <summary>
        /// Get a value from the session
        /// </summary>
        /// <typeparam name="T">Type of object ot get</typeparam>
        /// <param name="name">Name of object.</param>
        /// <param name="redirectIfNull">Specify whether to redirect to the timeout page if not found</param>
        /// <param name="defaultValue">Default value to return if the value is not found.</param>
        /// <returns>Typed object or default value if not found or is null and redirect is false.</returns>
        protected T GetSessionValue<T>(string name, bool redirectIfNull, T defaultValue)
        {
            if (Session[name] == null)
            {
                if (redirectIfNull)
                {
                    DoTimeoutRedirect();
                    return defaultValue;
                }

                return defaultValue;
            }

            return (T)Session[name];
        }

        /// <summary>
        /// Get a value from the query string.
        /// </summary>
        /// <param name="name">Query string parameter name.</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Query string value</returns>
        protected string GetQueryStringValue(string name, string defaultValue)
        {
            return GetQueryStringValue(name, false, defaultValue);
        }

        /// <summary>
        /// Get a value from the query string
        /// </summary>
        /// <param name="name">Name of object.</param>
        /// <param name="redirectIfNull">Specify whether to redirect to the timeout page if not found</param>
        /// <param name="defaultValue">Default value to return if the value is not found.</param>
        /// <returns>Typed object or default value if not found or is null and redirect is false.</returns>
        protected string GetQueryStringValue(string name, bool redirectIfNull, string defaultValue)
        {
            if (Utilities.IsNullOrEmpty(Request.QueryString[name]))
            {
                if (redirectIfNull)
                {
                    DoTimeoutRedirect();
                    return defaultValue;
                }

                return defaultValue;
            }

            return Request.QueryString[name];
        }

        /// <summary>
        /// Handle null session value exception
        /// </summary>
        /// <param name="ex"></param>
        protected override void HandleException(Exception ex)
        {
            if (ex is NullSessionValueException)
            {
                base.HandleException(ex, TimeoutRedirectUrl);
            }
        }

        /// <summary>
        /// Get the location to redirect to on time out or null session value
        /// </summary>
        protected virtual string TimeoutRedirectUrl
        {
            get
            {
                return UserDefaultRedirectUrl;
            }
        }

        /// <summary>
        /// Do the work of redirecting on timeout.
        /// </summary>
        /// <remarks>Popup dialogs will override this method to close themselves.</remarks>
        protected virtual void DoTimeoutRedirect()
        {
            try
            {
                Response.Redirect(TimeoutRedirectUrl, true);
            }
            catch (ThreadAbortException)
            {
                //Ignore thread aborts on redirect
            }
        }

        protected string UserDefaultRedirectUrl
        {
            get
            {
                // if there is no logged in user and the user is
                // being redirected, they should go to the login page
                if (!(User is CheckboxPrincipal))
                    return ResolveUrl("~/Login.aspx");

                // if user is logged in they should be redirected
                // to a page determined by their most important role
                if (User.IsInRole("System Administrator") || User.IsInRole("Survey Administrator") ||
                    User.IsInRole("Survey Editor") || User.IsInRole("Report Administrator"))
                    return ResolveUrl("~/Forms/Default.aspx");

                if (User.IsInRole("User Administrator"))
                    return ResolveUrl("~/Users/Default.aspx");

                 if (User.IsInRole("Group Administrator"))
                    return ResolveUrl("~/Users/Manage.aspx?m=g");

                if (User.IsInRole("Report Viewer") && !User.IsInRole("Respondent"))
                    return ResolveUrl("~/AvailableReports.aspx");

                return ResolveUrl("~/AvailableSurveys.aspx");
            }
        }

        //protected override string SystemAdministratorBanner()
        //{
        //    if (_principal != null && _principal.IsInRole("System Administrator"))
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("The following link is only displayed to System Administrators: To begin the activation process ");
        //        sb.Append("<a href=\"" + ApplicationManager.ApplicationPath + "/Install/Activation/ActivateStart.aspx\">Click Here</a>");
        //        sb.Append(".");
        //        return string.Format("<tr><td class=\"ErrorMessage\" align=\"center\">{0}</tr></td>", sb);
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// Get an instance of the default authorization provider
        /// </summary>
        protected virtual IAuthorizationProvider AuthorizationProvider
        {
            get { return AuthorizationFactory.GetAuthorizationProvider(); }
        }

        #region Site Map Providers

        /// <summary>
        /// Get the site map provider for the nav bar
        /// </summary>
        protected static SiteMapProvider NavBarProvider { get { return SiteMap.Providers["navMenuSiteMap"]; } }

        #endregion

    }
}
