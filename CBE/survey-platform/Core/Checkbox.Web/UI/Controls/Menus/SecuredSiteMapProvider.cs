using System;
using System.Linq;
using System.Web;
using Checkbox.Common;
using Checkbox.LicenseLibrary;
using Checkbox.Management;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Security.Principal;
using Prezza.Framework.Security;

namespace Checkbox.Web.UI.Controls.Menus
{
    //TODO: Additional Pruning, setting custom texts, URL customization
    /// <summary>
    /// Secured site map provider class that makes use of the current user context
    /// </summary>
    public class SecuredSiteMapProvider : XmlSiteMapProvider
    {
        ///<summary>
        ///Key representing access controllable resource in the specified http context.
        ///</summary>
        public const string HTTP_CONTEXT_ACCESS_CONTROLLABLE_CACHE_KEY = "SecuredProviderAccessControllableResource";

        private IAuthorizationProvider _authorizationProvider;


        /// <summary>
        /// Get an authorization provider
        /// </summary>
        protected IAuthorizationProvider AuthorizationProvider
        {
            get {
                return _authorizationProvider ??
                       (_authorizationProvider = AuthorizationFactory.GetAuthorizationProvider());
            }
        }

        /// <summary>
        /// Determine if a node is accessible to the given user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            if (!AuthorizeSiteMapNode(
                node, 
                context.User as CheckboxPrincipal, 
                context.Cache.Get(HTTP_CONTEXT_ACCESS_CONTROLLABLE_CACHE_KEY) as IAccessControllable))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Authorize a site map node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentPrincipal"></param>
        /// <param name="accessControllableEntity"></param>
        protected virtual bool AuthorizeSiteMapNode(
            SiteMapNode node, 
            CheckboxPrincipal currentPrincipal, 
            IAccessControllable accessControllableEntity)
        {
            if (node == null)
            {
                return false;
            }
            //Allow anonymous
            if (Utilities.IsNotNullOrEmpty(node["AllowAnonymous"]))
            {
                bool allowAnonymous;
                bool.TryParse(node["AllowAnonymous"], out allowAnonymous);

                //User is anonymous and anonymous access is explicitly denied, return false, otherwise fall
                // through to other rules
                if (currentPrincipal == null && !allowAnonymous)
                {
                    return false;
                }
            }

            //Check available reports
            if (node.Url.EndsWith("/AvailableReports.aspx")
                && !ApplicationManager.AppSettings.DisplayAvailableReportList)
            {
                return false;
            }

            //Check available surveys
            if (node.Url.EndsWith("/AvailableSurveys.aspx")
                && !node.Url.EndsWith("/Libraries/AvailableSurveys.aspx")
                && !ApplicationManager.AppSettings.DisplayAvailableSurveyList)
            {
                return false;
            }

            //Check invitaitons
            if (node.Url.Contains("/ManageInvitations.aspx")
                && (!ApplicationManager.AppSettings.EmailEnabled
                || !ApplicationManager.AppSettings.AllowInvitations))
            {
                return false;
            }

            //Allow logged-in
            if (Utilities.IsNotNullOrEmpty(node["AllowAuthenticated"]))
            {
                bool allowAuthenticated;
                bool.TryParse(node["AllowAuthenticated"], out allowAuthenticated);

                //Authenticated users expliclity disallowed, return false
                if (currentPrincipal != null && !allowAuthenticated)
                {
                    return false;
                }
            }

            //Check excluded role permissions
            if (Utilities.IsNotNullOrEmpty(node["ExcludedRolePermissions"]))
            {
                string[] rolePermissions = node["ExcludedRolePermissions"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (rolePermissions.Any(rolePermission => AuthorizationProvider.Authorize(currentPrincipal, rolePermission)))
                {
                    return false;
                }
            }

            //Check license limits
            if (ApplicationManager.AppSettings.EnableMultiDatabase && Utilities.IsNotNullOrEmpty(node["LicienseLimit"]))
            {
                string[] limits = node["LicienseLimit"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var limitName in limits)
                {
                    try
                    {
                        Type type = LicenseLimit.GetLimit(limitName);
                        LicenseLimit limit = Activator.CreateInstance(type) as LicenseLimit;

                        string message;
                        var result = limit.Validate(out message);
                        if (result == LimitValidationResult.LimitExceeded)
                            return false;
                    }
                    catch(Exception) {}
                }
            }

            //Next, check excluded roles
            if (currentPrincipal != null && Utilities.IsNotNullOrEmpty(node["ExcludedRoles"]))
            {
                string[] roles = node["ExcludedRoles"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (roles.Any(currentPrincipal.IsInRole))
                {
                    return false;
                }
            }

            //Next check to see if SimpleSecurity is enabled, if it is check to see whether the node should be disabled
            if (ApplicationManager.UseSimpleSecurity)
            {
                if (Utilities.IsNotNullOrEmpty(node["SimpleSecurity"]))
                {
                    return String.Equals(node["SimpleSecurity"], "Enabled", StringComparison.InvariantCultureIgnoreCase);
                }
            }

            //Next check included role permissions            
            if (Utilities.IsNotNullOrEmpty(node["RequiredRolePermissions"]))
            {
                //If there is no principal, then there is no role by definition
                if (currentPrincipal == null)
                {
                    return false;
                }

                string[] rolePermissions = node["RequiredRolePermissions"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                return rolePermissions.Any(rolePermission => AuthorizationProvider.Authorize(currentPrincipal, rolePermission));
            }

            //Check included roles
            if (Utilities.IsNotNullOrEmpty(node["RequiredRoles"]))
            {
                //If there is no principal, then there is no role by definition
                if (currentPrincipal == null)
                {
                    return false;
                }

                string[] roles = node["RequiredRoles"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                return roles.Any(currentPrincipal.IsInRole);
            }

            //Check access permissible permissions
            //Check if permissions on the current access controllable resource are required
            if (accessControllableEntity != null
                && Utilities.IsNotNullOrEmpty(node["RequiredAccessControllablePermissions"]))
            {
                string permissions = node["RequiredAccessControllablePermissions"];

                string[] permissionsArray = permissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //Only require one match for success
                return permissionsArray.Any(permission => AuthorizationProvider.Authorize(currentPrincipal, accessControllableEntity, permission));
            }

            return true;
        }
    }
}
