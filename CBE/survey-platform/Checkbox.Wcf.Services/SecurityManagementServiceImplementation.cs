using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Panels;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Security;
using Checkbox.Timeline;
using Prezza.Framework.ExceptionHandling;
using System.Security;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// 
    /// </summary>
    public static class SecurityManagementServiceImplementation
    {
        #region ACL

        /// <summary>
        /// Get ACL Entries for a specific resource type
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static PagedListResult<AclEntry[]> GetAclEntries(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            var result = new PagedListResult<AclEntry[]>();

            if(resourceId <= 0)
            {
                return result;
            }

            //Auth is performed in this method
            var securedResource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            return securedResource != null
            ? PageSortAndFilter(userPrincipal, securedResource.ACL.SelectAll(), pageNumber, pageSize, sortField, sortAscending, filterValue)
            : new PagedListResult<AclEntry[]>();
        }

        /// <summary>
        /// Get ACL Entries for a specific resource type
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionToGrant"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <param name="provider"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static PagedListResult<AclEntry[]> GetAvailableAclEntries(CheckboxPrincipal userPrincipal, string provider, SecuredResourceType resourceType, int resourceId, string permissionToGrant, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            if (resourceId <= 0)
            {
                return new PagedListResult<AclEntry[]>();
            }

            //Auth is performed in this method
            var securedResource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            if(securedResource == null)
            {
                return new PagedListResult<AclEntry[]>();
            }

            var securityEditor = securedResource.GetEditor();
            securityEditor.Initialize(userPrincipal);

            return string.IsNullOrEmpty(permissionToGrant)
                        ? PageSortAndFilter(securityEditor.GetAccessPermissible(provider, pageNumber, pageSize, filterValue))
                        : PageSortAndFilter(securityEditor.GetAccessPermissible(provider, pageNumber, pageSize, filterValue, permissionToGrant.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));
        }

        /// <summary>
        /// Get ACL Entries for a specific resource type
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceId"></param>
        /// <param name="permissionToCheck">not used</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static PagedListResult<AclEntry[]> GetCurrentAclEntries(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, string permissionToCheck, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            if (resourceId <= 0)
            {
                return new PagedListResult<AclEntry[]>();
            }

            //Auth is performed in this method
            var securedResource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            if (securedResource == null)
            {
                return new PagedListResult<AclEntry[]>();
            }

            var securityEditor = securedResource.GetEditor();
            securityEditor.Initialize(userPrincipal);

            return PageSortAndFilter(userPrincipal, securityEditor.List(), pageNumber, pageSize, sortField, sortAscending, filterValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"> </param>
        /// <param name="allEntries"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private static PagedListResult<AclEntry[]> PageSortAndFilter(CheckboxPrincipal userPrincipal, IEnumerable<IAccessControlEntry> allEntries, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterValue)
        {
            var result = new PagedListResult<AclEntry[]>();

            var aclEntries = allEntries.Select(GetAclEntry);

            if (!string.IsNullOrEmpty(filterValue))
            {
                filterValue = filterValue.ToLower();
                aclEntries = aclEntries.Where(entry => entry.FullEntryIdentifier.ToLower().Contains(filterValue));
            }

            aclEntries =
                aclEntries
                    .OrderBy(entry => entry.EntryType)
                    .ThenBy(entry => entry.ShortEntryIdentifier);

            //Count results before paging
            result.TotalItemCount = aclEntries.Count();

            if (pageNumber > 0 && pageSize > 0)
            {
                aclEntries = aclEntries.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            var aclArray = aclEntries.ToArray();

            //mark calling principal
            aclArray.Where(entry => entry.EntryType == "Prezza.Framework.Security.ExtendedPrincipal" 
                && entry.EntryIdentifier == userPrincipal.AclEntryIdentifier).ToList().ForEach(u => u.IsInList = true);

            result.ResultPage = aclArray;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allEntries"></param>
        /// <returns></returns>
        private static PagedListResult<AclEntry[]> PageSortAndFilter(IEnumerable<IAccessControlEntry> allEntries)
        {
            var result = new PagedListResult<AclEntry[]>
                             {
                                 ResultPage = allEntries.Select(GetAclEntry).ToArray()
                             };
            result.TotalItemCount = result.ResultPage.Count();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <param name="permissionToGrant"></param>
        /// <returns></returns>
        public static void AddEntryToAcl(CheckboxPrincipal userPrincipal,  SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier, string permissionToGrant)
        {
            if (resourceId <= 0)
            {
                return;
            }

            //Auth is performed in this method
            var securedResource = GetSecuredResource(userPrincipal, resourceType, resourceId);
            
            if (securedResource == null)
            {
                return;
            }

            var securityEditor = securedResource.GetEditor();
            securityEditor.Initialize(userPrincipal);

            if (string.IsNullOrEmpty(permissionToGrant))
            {
                securityEditor.GrantAccess(new[] {new AccessControlEntry(aclEntryType, aclEntryIdentifier, -1)});
            }
            else
            {
                //Grant string could be an comma-delimited array, so split it.
                securityEditor.GrantAccess(new[] {new AccessControlEntry(aclEntryType, aclEntryIdentifier, -1)}, permissionToGrant.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries));
            }

            securityEditor.SaveAcl();

            try
            {
                TimelineManager.ClearByACLEntry(aclEntryType, aclEntryIdentifier);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }

            //Force reload ACL
            if (securedResource is AccessControllablePersistedDomainObject)
            {
                (securedResource as AccessControllablePersistedDomainObject).ReloadACL();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="aclEntryType"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        public static void RemoveEntryFromAcl(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, string aclEntryType, string aclEntryIdentifier)
        {
            if (resourceId <= 0)
            {
                return;
            }

            //Auth is performed in this method
            var securedResource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            if (securedResource == null)
            {
                return;
            }

            //user tries to delete themselves from ACL
            if (aclEntryType.Contains("Principal") && userPrincipal.Identity.Name.Equals(aclEntryIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityException("You can not remove yourself from the access list.");
            }

            var securityEditor = securedResource.GetEditor();
            securityEditor.Initialize(userPrincipal);

            securityEditor.RemoveAccess(new[] { new AccessControlEntry(aclEntryType, aclEntryIdentifier, -1) });

            try
            {
                TimelineManager.ClearByACLEntry(aclEntryType, aclEntryIdentifier);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }


            securityEditor.SaveAcl();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceEntry"></param>
        /// <returns></returns>
        public static AclEntry GetAclEntry(IAccessControlEntry sourceEntry)
        {
            var aclEntry = new AclEntry
            {
                AclPolicyId = sourceEntry.PolicyId,
                EntryIdentifier = sourceEntry.AclEntryIdentifier,
                EntryType = sourceEntry.AclEntryTypeIdentifier,
                ShortEntryIdentifier = sourceEntry.AclEntryIdentifier,
                IsInList = sourceEntry.IsInList
            };

            if(aclEntry.EntryType.Equals("Checkbox.Users.Group", StringComparison.InvariantCultureIgnoreCase))
            {
                var groupIdAsInt = Utilities.AsInt(aclEntry.EntryIdentifier);

                if(groupIdAsInt.HasValue)
                {
                    var theGroup = GroupManager.GetGroup(groupIdAsInt.Value);

                    if(theGroup != null)
                    {
                        aclEntry.ShortEntryIdentifier = theGroup.Name;
                    }
                }
            }

            //Store full value, for filtering and such
            aclEntry.FullEntryIdentifier = aclEntry.ShortEntryIdentifier;
            aclEntry.ShortEntryIdentifier = Utilities.StripHtml(aclEntry.ShortEntryIdentifier, 40);

            return aclEntry;
        }

        

        #endregion


        #region Permission

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public static PermissionEntry[] GetPolicyPermissions(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, int policyId)
        {
            //Get access controllable resource to cause security check
            var resource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            if(resource == null)
            {
                return new PermissionEntry[] {};
            }

            var policy = Policy.GetPolicy(policyId);

            if(policy == null)
            {
                return new PermissionEntry[]{};
            }

            return 
                resource.SupportedPermissions.Select(
                    permissionName => 
                        new PermissionEntry
                            {
                                DisplayName =  AccessManager.GetPermissionMaskDisplayName(permissionName), 
                                PermissionName = permissionName, 
                                Selected = policy.HasPermission(permissionName),
                                Disabled = string.Equals(resource.ACL.GetPolicyEntry(policyId) != null ? resource.ACL.GetPolicyEntry(policyId).AclEntryIdentifier : string.Empty,
                                    userPrincipal.Identity.Name, StringComparison.InvariantCultureIgnoreCase)
                            })
                    .ToList().OrderBy(permissionEntry => permissionEntry.DisplayName).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public static PermissionMaskEntry[] GetMaskedPolicyPermissions(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, int policyId)
        {
            //Get access controllable resource to cause security check
            var resource = GetLightweightSecuredResource(userPrincipal, resourceType, resourceId);

            if (resource == null)
            {
                return new PermissionMaskEntry[] { };
            }
            
            var policy = Policy.GetPolicy(policyId);

            if(policy== null)
            {
                return new PermissionMaskEntry[] { };
            }

            return
                resource.SupportedPermissionMasks.Select(
                    maskName => 
                        new PermissionMaskEntry
                            {
                                DisplayName = AccessManager.GetPermissionMaskDisplayName(maskName),
                                MaskName = maskName,
                                MaskState = GetMaskState(maskName, policy),
                                Permissions = GetMaskPermissions(maskName, policy),
                                Disabled = string.Equals(resource.ACL.GetPolicyEntry(policyId) != null? resource.ACL.GetPolicyEntry(policyId).AclEntryIdentifier : string.Empty,
                                    userPrincipal.Identity.Name, StringComparison.InvariantCultureIgnoreCase)
                            })
                    .ToList().OrderBy(permissionEntry => permissionEntry.DisplayName).ToArray();
        }

        /// <summary>
        /// Replace specified policy permissions with selected permissions
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="permissions"></param>
        public static void UpdatePolicyPermissions(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, int policyId, string[] permissions)
        {
            //Get access controllable resource to cause security check
            var resource = GetSecuredResource(userPrincipal, resourceType, resourceId);

            if (resource == null)
            {
                return;
            }
            if (resourceType == SecuredResourceType.UserGroup)
            {
                GroupManager.InvalidateGroupCache((Group)resource);
            }
            var securityEditor = resource.GetEditor();
            securityEditor.Initialize(userPrincipal);

            //Figure out what to do, based on policy Id.  If editing default policy,
            // update item's default policy
            if (policyId == resource.DefaultPolicy.Id)
            {
                //Replace default policy
                var newDefaultPolicy = securityEditor.ControllableResource.CreatePolicy(permissions);
                securityEditor.SetDefaultPolicy(newDefaultPolicy);
            }
            else
            {
                //Otherwise, update access for user with policy id
                var p = Policy.GetPolicy(policyId);
                p.Permissions.Clear();
                p.Permissions.AddRange(permissions);
                AccessManager.UpdatePolicy(policyId, p);
            }
            
            Policy.UpdatePolicy(policyId);
            
            try
            {
                TimelineManager.ClearByPolicy(policyId);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }
        }

        /// <summary>
        /// Replace specified policy permissions with selected permissions
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <param name="policyId"></param>
        /// <param name="maskNames"></param>
        public static void UpdatePolicyMaskedPermissions(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId, int policyId, string[] maskNames)
        {
            var permissionNames = new List<string>();

            if (maskNames == null)
                return;

            foreach(var maskName in maskNames)
            {
                permissionNames.AddRange(AccessManager.GetPermissionMaskPermissions(maskName));
            }

            UpdatePolicyPermissions(userPrincipal, resourceType, resourceId, policyId, permissionNames.Distinct().ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskName"></param>
        /// <param name="policy"></param>
        private static PermissionEntry[] GetMaskPermissions(string maskName, Policy policy)
        {
            return 
                AccessManager
                    .GetPermissionMaskPermissions(maskName)
                    .OrderBy(maskPermission => maskPermission)
                    .Select(
                        permissionName => new PermissionEntry
                        {
                            DisplayName = permissionName, 
                            PermissionName = permissionName, 
                            Selected = policy.HasPermission(permissionName)
                        })
                    .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maskName"></param>
        /// <param name="policy"></param>
        private static PermissionMaskState GetMaskState(string maskName, Policy policy)
        {
            bool allPermissions = true;
            bool noPermissions = true;

            foreach (var permissionName in AccessManager.GetPermissionMaskPermissions(maskName).OrderBy(maskPermission => maskPermission))
            {
                if ( policy.HasPermission(permissionName))
                {
                    noPermissions = false;
                }
                else
                {
                    allPermissions = false;
                }
            }

            if (allPermissions)
            {
                return PermissionMaskState.Selected;
            }
            
            if (noPermissions)
            {
                return PermissionMaskState.NotSelected;
            }
            
            return PermissionMaskState.PartiallySelected;
        }


        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private static IAccessControllable GetLightweightSecuredResource(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId)
        {
            IAccessControllable securedResource = null;

            if(resourceType == SecuredResourceType.Folder)
            {
                //Folders are essentially "lightweight" anyway, and there is no folder-specfic lightweight security object, so
                // it's not possible to use the generic lightweightaccesscontrollable returned by getlightweightfolder to create
                // a security editor, so we need to return a full folder.
                securedResource = FolderManager.GetFolder(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource,  "FormFolder.FullControl");
            }

            if(resourceType == SecuredResourceType.Survey)
            {
                securedResource = ResponseTemplateManager.GetLightweightResponseTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Form.Administer");
            }

            if(resourceType == SecuredResourceType.Report)
            {
                securedResource = AnalysisTemplateManager.GetLightweightAnalysisTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Analysis.Administer");
            }

            if(resourceType == SecuredResourceType.Library)
            {
                securedResource = LibraryTemplateManager.GetLightweightLibraryTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Library.Edit");
            }

            if(resourceType == SecuredResourceType.EmailList)
            {
                securedResource = EmailListManager.GetEmailListPanel(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "EmailList.Edit");
            }


            if (resourceType == SecuredResourceType.UserGroup)
            {
                securedResource = GroupManager.GetGroup(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Group.Edit");
            }

            return securedResource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        private static IAccessControllable GetSecuredResource(CheckboxPrincipal userPrincipal, SecuredResourceType resourceType, int resourceId)
        {
            IAccessControllable securedResource = null;

            if (resourceType == SecuredResourceType.Folder)
            {
                securedResource = FolderManager.GetFolder(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "FormFolder.FullControl");
            }

            if (resourceType == SecuredResourceType.Survey)
            {
                securedResource = ResponseTemplateManager.GetResponseTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Form.Administer");
            }

            if (resourceType == SecuredResourceType.Report)
            {
                securedResource = AnalysisTemplateManager.GetAnalysisTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Analysis.Administer");
            }

            if (resourceType == SecuredResourceType.Library)
            {
                securedResource = LibraryTemplateManager.GetLibraryTemplate(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Library.Edit");
            }

            if (resourceType == SecuredResourceType.EmailList)
            {
                securedResource = EmailListManager.GetEmailListPanel(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "EmailList.Edit");
            }
            
            if (resourceType == SecuredResourceType.UserGroup)
            {
                securedResource = GroupManager.GetGroup(resourceId);

                //Authorize
                Security.AuthorizeUserContext(userPrincipal, securedResource, "Group.Edit");
            }

            return securedResource;
        }
    }
}
