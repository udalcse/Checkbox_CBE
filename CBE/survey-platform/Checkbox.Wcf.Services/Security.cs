using System.Linq;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Static class for performing security-related operations by other web service classes.
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// Authorize a given user context for a given role permission.
        /// </summary>
        /// <param name="principal">User principal to authorize.</param>
        /// <param name="permissions">Role permissions to check.  Access to one permission is considered sufficient.</param>
        public static void AuthorizeUserContext(ExtendedPrincipal principal, params string[] permissions)
        {
            var authProvider = AuthorizationFactory.GetAuthorizationProvider();

            //Authorize
            if (permissions.Any(permission => authProvider.Authorize(principal, permission)))
            {
                return;
            }

            throw new ServiceAuthorizationException();
        }

        /// <summary>
        /// Authorize a given user context for a permissible resource.
        /// </summary>
        /// <param name="principal">User principal to authorize.</param>
        /// <param name="controllableResource">Access controllable resource to authorize against.</param>
        /// <param name="permissions">Role permissions to check.  Access to one permission is considered sufficient.</param>
        public static void AuthorizeUserContext(ExtendedPrincipal principal, IAccessControllable controllableResource, params string[] permissions)
        {
            var authProvider = AuthorizationFactory.GetAuthorizationProvider();

            //Authorize
            if (permissions.Any(permission => authProvider.Authorize(principal, controllableResource, permission)))
            {
                return;
            }

            throw new ServiceAuthorizationException();
        }
    }
}
