using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Common;
using Checkbox.Globalization;
using Prezza.Framework.Security;
using Checkbox.Security.Principal;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for pages that are secured
    /// </summary>
    public abstract class SecuredPage : ProgressReportingEnabledPage
    {
        /// <summary>
        /// Simple struct for storing page controls to authorize
        /// </summary>
        public class PageControl
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="label"></param>
            /// <param name="input"></param>
            /// <param name="permission"></param>
            /// <param name="controllableResource"></param>
            public PageControl(Label label, Control input, string permission, IAccessControllable controllableResource)
            {
                Label = label;
                Input = input;
                Permission = permission;
                ControllableResource = controllableResource;
            }

            /// <summary>
            /// Default constructor
            /// </summary>
            public PageControl()
            {
            }

            /// <summary>
            /// Get/set the label associated with the input to authorize
            /// </summary>
            public Label Label;

            /// <summary>
            /// Get/set the input to authorize
            /// </summary>
            public Control Input;

            /// <summary>
            /// Get/set the permission to check
            /// </summary>
            public string Permission;

            /// <summary>
            /// Get/set the controllable resource
            /// </summary>
            public IAccessControllable ControllableResource;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual CheckboxPrincipal CurrentPrincipal
        {
            get
            {
                if(HttpContext.Current == null)
                {
                    return null;
                }

                return HttpContext.Current.User as CheckboxPrincipal;
            }
        }

        /// <summary>
        /// Get the role required to access this page
        /// </summary>
        protected virtual string PageRequiredRole { get { return null; } }

        /// <summary>
        /// Get the required role permission required to access this page
        /// </summary>
        protected virtual string PageRequiredRolePermission { get { return null; } }

        /// <summary>
        /// Determines if the page contains functionality that is not available
        /// in all product tiers.
        /// </summary>
        protected virtual bool LicenseControlledFeature {get { return false; } }

        /// <summary>
        /// Indicates if license controlled features is accessible.
        /// </summary>
        protected virtual bool FeatureCriterion { get { return true; } }

        /// <summary>
        /// Overridable method to get the current controllable entity.  This method is called        
        /// in the OnInit(..) page event.
        /// </summary>
        /// <returns>NULL unless overridden.</returns>
        protected virtual IAccessControllable GetControllableEntity() { return null; }

        /// <summary>
        /// Get the required permission for the specified access controllable entity
        /// </summary>
        protected virtual string ControllableEntityRequiredPermission { get { return null; } }

        /// <summary>
        /// If access is not authorized redirect to default page
        /// </summary>
        protected override void OnPagePreInit()
        {
            base.OnPagePreInit();

            if (LicenseControlledFeature && !FeatureCriterion)
            {
                DoTransfer("~/ErrorPages/FeatureNotEnabled.aspx", true);
            }

            try
            {
                if (!AuthorizePage())
                {
                    DoTransfer("~/ErrorPages/PermissionError.aspx", true);
                }
            }
            catch (ThreadAbortException)
            {
                //Ignore thread aborts on redirect
            }
        }

        /// <summary>
        /// Get the controllable entity and authorize it and page-level controls
        /// </summary>
        protected override void OnPagePreLoad()
        {
            base.OnPagePreLoad();

            try
            {
                if (!AuthorizeControllable())
                {
                    DoTransfer("~/ErrorPages/PermissionError.aspx", true);
                }
            }
            catch (ThreadAbortException)
            {
                //Ignore thread aborts on redirect
            }

            //Authorize page controls
            AuthorizePageControls();
        }

        /// <summary>
        /// Transfer to url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="adjustForDialog"></param>
        protected virtual void DoTransfer(string url, bool adjustForDialog)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            //Adjust URL for dialog, if necessary
            if (!adjustForDialog)
            {
                Server.Transfer(url);
            }

            if (Master != null
                && Master is BaseMasterPage)
            {
                if (((BaseMasterPage)Master).IsDialog)
                {
                    url = url.Replace(".aspx", "Dialog.aspx");
                }

                if (((BaseMasterPage)Master).IsEmbedded)
                {
                    url = url.Replace(".aspx", "Embedded.aspx");
                }
            }

            Server.Transfer(url);
        }


        /// <summary>
        /// Check page required roles.  If the page specifies no role permission, then the check always returns true
        /// </summary>
        /// <returns></returns>
        protected virtual bool AuthorizePage()
        {
            var currentPrincipal = Context.User as CheckboxPrincipal;
            bool authorizedRolePermission = string.IsNullOrEmpty(PageRequiredRolePermission) || AuthorizationProvider.Authorize(currentPrincipal, PageRequiredRolePermission);
            bool authorizedRole = (currentPrincipal != null && currentPrincipal.IsInRole(PageRequiredRole)) || string.IsNullOrEmpty(PageRequiredRole);

            return authorizedRole && authorizedRolePermission;
        }

        /// <summary>
        /// Authorize the controllable resource
        /// </summary>
        /// <returns></returns>
        protected virtual bool AuthorizeControllable()
        {
            var controllableEntity = GetControllableEntity();

            if (string.IsNullOrEmpty(ControllableEntityRequiredPermission) || controllableEntity == null)
            {
                return true;
            }


            return AuthorizationProvider.Authorize(CurrentPrincipal, controllableEntity, ControllableEntityRequiredPermission);
        }

        /// <summary>
        /// Authorize controls on the page
        /// </summary>
        /// <returns></returns>
        protected virtual void AuthorizePageControls()
        {
            List<PageControl> controlsToAuthorize = GetControlsToAuthorize();

            foreach (PageControl pageControl in controlsToAuthorize)
            {
                bool controlAuthorized;

                if (Utilities.IsNullOrEmpty(pageControl.Permission))
                {
                    controlAuthorized = true;
                }
                else if (pageControl.ControllableResource != null)
                {
                    controlAuthorized = AuthorizationProvider.Authorize(CurrentPrincipal, pageControl.ControllableResource, pageControl.Permission);
                }
                else
                {
                    controlAuthorized = AuthorizationProvider.Authorize(CurrentPrincipal, pageControl.Permission);
                }

                if (pageControl.Label != null)
                {
                    pageControl.Label.Visible = controlAuthorized;
                }

                if (pageControl.Input != null)
                {
                    pageControl.Input.Visible = controlAuthorized;
                }
            }
        }

        /// <summary>
        /// Get controls to authorize
        /// </summary>
        /// <returns>List of page control structs</returns>
        protected virtual List<PageControl> GetControlsToAuthorize()
        {
            return new List<PageControl>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LoadDatePickerLocalized()
        {
            string file = GlobalizationManager.GetDatePickerLocalizationFile();
            if (!string.IsNullOrEmpty(file))
            {
                RegisterClientScriptInclude(
                    file,
                    ResolveUrl("~/Resources/" + file));
            }

            //Localization for datepicker
            RegisterClientScriptInclude(
                "jquery.localize.js",
                ResolveUrl("~/Resources/jquery.localize.js"));
        }
    }
}
