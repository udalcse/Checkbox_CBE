using System;
using System.Security;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Panels;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Checkbox.Management;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Implementation of the authentication service
    /// </summary>
    public static class AuthorizationServiceImplementation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool UserHasRolePermission(string userUniqueIdentifier, string permission)
        {
            if (string.IsNullOrEmpty(userUniqueIdentifier))
            {
                return false;
            }

            return RoleManager.UserHasRoleWithPermission(userUniqueIdentifier, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionCsv"></param>
        /// <returns></returns>
        public static SimpleNameValueCollection BatchAuthorizeAccess(string userUniqueIdentifier, SecuredResourceType resourceType, string resourceId, string permissionCsv)
        {
            var nvCollection = new SimpleNameValueCollection();

            //Check inputs.  Null/Empty user is OK since we can check for anonymous access to things
            if (string.IsNullOrEmpty(permissionCsv))
            {
                return nvCollection;
            }

            var permissions = permissionCsv.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var permission in permissions)
            {
                nvCollection[permission] = AuthorizeAccess(userUniqueIdentifier, resourceType, resourceId, permission).ToString();
            }

            return nvCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool AuthorizeAccess(string userUniqueIdentifier, SecuredResourceType resourceType, string resourceId, string permission)
        {
            //Check inputs.  Null/Empty user is OK since we can check for anonymous access to things
            if (string.IsNullOrEmpty(permission))
            {
                return false;
            }

            if (!AccessManager.ListPermissions().Contains(permission.Trim()))
                throw new ArgumentException("\"permission\" parameter is passed in wrong format");

            var userPrincipal = UserManager.GetUserPrincipal(userUniqueIdentifier);

            //check the System Admin Role at first
            if (userPrincipal.IsInRole("System Administrator"))
                return true;

            int nResourceId;
            if (!int.TryParse(resourceId, out nResourceId) && resourceType != SecuredResourceType.User)
                return false;

            switch (resourceType)
            {
                case SecuredResourceType.Survey:
                    return AuthorizeSurveyAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.Folder:
                    return AuthorizeFolderAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.Report:
                    return AuthorizeReportAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.UserGroup:
                    return AuthorizeUserGroupAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.Library:
                    return AuthorizeLibraryAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.EmailList:
                    return AuthorizeEmailListAccess(userPrincipal, nResourceId, permission);
                case SecuredResourceType.User:
                    return AuthorizeUserAccess(userPrincipal, resourceId, permission);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="userName"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeUserAccess(ExtendedPrincipal userPrincipal, string userName, string permission)
        {
            if (permission == "Group.ManageUsers")
            {
                try
                {
                    Security.AuthorizeUserContext(userPrincipal, GroupManager.GetEveryoneGroup(), permission);
                    return true;
                }
                catch (ServiceAuthorizationException)
                {
                    return false;
                }
            }

            return UserManager.CheckPrincipalPermissionForUser(userPrincipal, userName, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeSurveyAccess(ExtendedPrincipal userPrincipal, int surveyId, string permission)
        {
            //Use full response template since template are cached, so it may actually be less work to use full template
            // instead of lightweight template
            if (ApplicationManager.AppSettings.CacheResponseTemplates)
            {
                var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

                if (responseTemplate == null)
                {
                    return false;
                }

                return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, responseTemplate, permission);
            }
            var lrt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lrt == null)
            {
                return false;
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, lrt, permission);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="folderId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeFolderAccess(ExtendedPrincipal userPrincipal, int folderId, string permission)
        {
            if (folderId == 1)
            {
                return true;
            }

            //Get folder to authorize
            var folder = FolderManager.GetFolder(folderId);

            if (folder == null)
            {
                return false;
            }

            if ((permission == "Form.Create" || permission == "Form.Delete"))
            {
                if (!UserHasRolePermission(userPrincipal.Identity.Name, permission))
                    return false;

                permission = "FormFolder.FullControl";
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, folder, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="groupId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeUserGroupAccess(ExtendedPrincipal userPrincipal, int groupId, string permission)
        {
            var userGroup = GroupManager.GetGroup(groupId);

            if (userGroup == null)
            {
                return false;
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, userGroup, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeReportAccess(ExtendedPrincipal userPrincipal, int reportId, string permission)
        {
            //Get resource to authorized
            var lightweightAt = AnalysisTemplateManager.GetLightweightAnalysisTemplate(reportId);

            if (lightweightAt == null)
            {
                return false;
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, lightweightAt, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="libraryId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeLibraryAccess(ExtendedPrincipal userPrincipal, int libraryId, string permission)
        {
            //Get resource to authorized
            var library = LibraryTemplateManager.GetLightweightLibraryTemplate(libraryId);

            if (library == null)
            {
                return false;
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, library, permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="emailListId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        private static bool AuthorizeEmailListAccess(ExtendedPrincipal userPrincipal, int emailListId, string permission)
        {
            //Get resource to authorized
            var panel = PanelManager.GetLightWeightPanel(emailListId);

            if (panel == null)
            {
                return false;
            }

            return AuthorizationFactory.GetAuthorizationProvider().Authorize(userPrincipal, panel, permission);
        }
    }
}
