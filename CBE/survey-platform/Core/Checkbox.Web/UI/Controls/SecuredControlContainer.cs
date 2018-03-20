using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Common;

using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;


namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Container for controls that can show/hide controls based on authorization checks
    /// </summary>
    [ParseChildren(ChildrenAsProperties = false)]
    public class SecuredControlContainer : CompositeControl
    {
        private readonly List<Control> _controlsToSecure;

        /// <summary>
        /// Constructor, initialize list
        /// </summary>
        public SecuredControlContainer()
        {
            _controlsToSecure = new List<Control>();
        }

        /// <summary>
        /// Get/set the comma-separated list of roles a user must have at least one of for this control to be visible.  If the user
        /// is not logged-in, the control will be hidden.
        /// </summary>
        public string RequiredRoles { get; set; }

        /// <summary>
        /// Get/set the comma-separated list of required role permissions a user must have for this control to be visible.  If the user
        /// is not logged-in, the control will be hidden.
        /// </summary>
        public string RequiredRolePermissions { get; set; }

        /// <summary>
        /// Get/set the access controllable object the user must have access to 
        /// </summary>
        public IAccessControllable AccessControllable { get; set; }

        /// <summary>
        /// Get/set entity that permissions are
        /// </summary>
        public ExtendedPrincipal CurrentPrincipal { get; set; }

        /// <summary>
        /// Get/set the comma-separated required access controllable permissions
        /// </summary>
        public string RequiredAccessControllablePermissions { get; set; }

        /// <summary>
        /// Add a control to the list of non-child controls to secure
        /// </summary>
        /// <param name="c"></param>
        public void AddControlToSecure(Control c)
        {
            _controlsToSecure.Add(c);
        }

        /// <summary>
        /// Show/hide based on authorized access
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Handle case of child controls or referenced controls
            bool accessAuthorized = AuthorizeAccess();

            if (!accessAuthorized)
            {
                if (Controls.Count == 0)
                {
                    Visible = false;
                }
                else
                {
                    foreach (Control c in Controls)
                    {
                        c.Visible = false;
                    }
                }

                //Handle case where controls to secure aren't children
                foreach (Control c in _controlsToSecure)
                {
                    c.Visible = false;
                }
            }
        }


        /// <summary>
        /// Authorize access to the controls
        /// </summary>
        /// <returns></returns>
        protected bool AuthorizeAccess()
        {
            //Check roles

            if (Utilities.IsNotNullOrEmpty(RequiredRoles))
            {
                if (CurrentPrincipal == null)
                {
                    return false;
                }
                
                string[] roles = RequiredRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string role in roles)
                {
                    if (CurrentPrincipal.IsInRole(role))
                    {
                        return true;
                    }
                }

                //No roles matched, no access
                return false;
            }

            //Check role permissions
            if (Utilities.IsNotNullOrEmpty(RequiredRolePermissions))
            {
                IAuthorizationProvider authorizationProvider = AuthorizationFactory.GetAuthorizationProvider();

                string[] rolePermissions = RequiredRolePermissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string rolePermission in rolePermissions)
                {
                    if (authorizationProvider.Authorize(CurrentPrincipal, rolePermission))
                    {
                        return true;
                    }
                }

                //No role permissions matched, no access
                return false;

            }

            //Check access controllable
            if (AccessControllable != null && Utilities.IsNotNullOrEmpty(RequiredAccessControllablePermissions))
            {
                IAuthorizationProvider authorizationProvider = AuthorizationFactory.GetAuthorizationProvider();

                string[] permissions = RequiredAccessControllablePermissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string permission in permissions)
                {
                    if (authorizationProvider.Authorize(CurrentPrincipal, AccessControllable, permission))
                    {
                        return true;
                    }
                }

                //No permissions matched
                return false;
            }

            //No permissions or access controllable objects were specified...return true
            return true;
        }
    }
}
