using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Users;
using Checkbox.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Security;
using Checkbox.Pagination;

namespace Checkbox.Security
{
    /// <summary>
    /// Abstract base class for editor of access controllable persisted domain
    /// objects.
    /// </summary>
    public abstract class AccessControllablePDOSecurityEditor : SecurityEditor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resource"></param>
        protected AccessControllablePDOSecurityEditor(IAccessControllable resource)
            : base(resource)
        {
            GetPermissible = GetPermissbleEntity;
        }

        /// <summary>
        /// Get the name of the field containing the domain object's default policy
        /// </summary>
        protected virtual string DefaultPolicyIDFieldName { get { return "DefaultPolicy"; } }

        /// <summary>
        /// Get an access permissible entry
        /// </summary>
        /// <param name="entryData"></param>
        /// <returns></returns>
        protected static IAccessPermissible GetPermissbleEntity(IAccessControlEntry entryData)
        {
            if (entryData.AclEntryTypeIdentifier.Equals("Prezza.Framework.Security.ExtendedPrincipal", StringComparison.InvariantCultureIgnoreCase))
            {
                return UserManager.GetUserPrincipal(entryData.AclEntryIdentifier);
            }

            if (entryData.AclEntryTypeIdentifier.Equals("Checkbox.Users.Group", StringComparison.InvariantCultureIgnoreCase))
            {
                int groupId;

                if (Int32.TryParse(entryData.AclEntryIdentifier, out groupId))
                {
                    return GroupManager.GetGroup(groupId);
                }
            }

            return null;
        }

        /// <summary>
        /// Set the default policy
        /// </summary>
        /// <param name="p"></param>
        public override void SetDefaultPolicy(Policy p)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            ArgumentValidation.CheckForNullReference(p, "p");
            ArgumentValidation.CheckExpectedType(ControllableResource, typeof(AccessControllablePersistedDomainObject));

            AccessControllablePersistedDomainObject pdo = (AccessControllablePersistedDomainObject)ControllableResource;

            if (pdo.DefaultPolicyID > 0)
            {
                AccessManager.UpdatePolicy(pdo.DefaultPolicyID.Value, p);

                //Set the policy id to cause it to be reloaded by the object
                pdo.DefaultPolicyID = pdo.DefaultPolicyID;
            }
            else
            {
                //Policy will be reloaded by the object when it is set
                pdo.DefaultPolicyID = AccessManager.CreatePolicy(p);
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_UpdateDefaultPolicyId");
            command.AddInParameter("DataTableName", DbType.String, pdo.DomainDBTableName);
            command.AddInParameter("IdentityColumnName", DbType.String, pdo.DomainDBIdentityColumnName);
            command.AddInParameter("DefaultPolicyColumnName", DbType.String, DefaultPolicyIDFieldName);
            command.AddInParameter("ObjectIdentity", DbType.Int32, pdo.ID);
            command.AddInParameter("NewDefaultPolicyId", DbType.Int32, pdo.DefaultPolicyID);

            db.ExecuteNonQuery(command);

            pdo.ReloadDefaultPolicy();
        }

        /// <summary>
        /// Get access permissible entities that can be added to the object acl
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="permissions"></param>
        /// <param name="provider"></param>
        /// <param name="currentPage"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public override List<IAccessControlEntry> GetAccessPermissible(string provider, int currentPage, 
                                                                       int pageSize, string filterValue,
                                                                       params string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            var entries = new List<IAccessControlEntry>();

            var context = new PaginationContext
                              {
                                  CurrentPage = currentPage,
                                  PageSize = pageSize,
                                  FilterValue = filterValue,
                                  PermissionJoin = PermissionJoin.Any
                              };

            #region Groups

            if (provider == "CheckboxMembershipProvider")
            {
                //Get groups that the logged-in principal can view. 
                context.Permissions.Add("Group.View");
                context.SortField = "GroupName";
                context.SortAscending = true;
                context.FilterField = "GroupName";

                List<Int32> rawGroupList = GroupManager.ListAccessibleGroups(CurrentPrincipal, context, true);

                //Add identities to the list of available entries if they are not already on the ACL
                var groupEntries = (from groupId in rawGroupList
                                    select new AccessControlEntry("Checkbox.Users.Group", groupId.ToString(), -1)).
                    ToList();

                groupEntries.Where(ge =>
                                   ControllableResource.ACL.IsInList(
                                       ControllableResource.ACL.CreateAclIdentifier("Checkbox.Users.Group",
                                                                                    ge.AclEntryIdentifier))).ToList().
                    ForEach(ge => ge.IsInList = true);

                entries.AddRange(groupEntries);
            }

            #endregion

            #region Users

            if (entries.Count < pageSize)
            {
                context.SortField = "UniqueIdentifier";
                context.FilterField = "UniqueIdentifier";

                int totalGroups = context.ItemCount;
                int groupPages = totalGroups / pageSize;
                int groupsRest = totalGroups % pageSize;
                context.CurrentPage -= groupPages;

                List<string> rawUserList;

                if (groupsRest > 0)
                {
                    if (entries.Count > 0)
                        rawUserList = UserManager.ListUsers(CurrentPrincipal, context, provider).Take(pageSize - groupsRest).ToList();
                    else
                    {
                        context.CurrentPage--;
                        rawUserList = UserManager.ListUsers(CurrentPrincipal, context, provider).Skip(pageSize - groupsRest).ToList();

                        context.CurrentPage++;
                        rawUserList.AddRange(UserManager.ListUsers(CurrentPrincipal, context, provider));
                        rawUserList = rawUserList.Take(context.PageSize).ToList();
                    }
                }
                else
                    rawUserList = UserManager.ListUsers(CurrentPrincipal, context, provider);

                //Add identities to the list of available entries if they are not already on the ACL
                var usersEntries = (from userUniqueIdentifier in rawUserList
                                   select new AccessControlEntry("Prezza.Framework.Security.ExtendedPrincipal", userUniqueIdentifier, -1)).ToList();

                usersEntries.Where(ue => ControllableResource.ACL.IsInList(ControllableResource.ACL.CreateAclIdentifier("Prezza.Framework.Security.ExtendedPrincipal",
                    ue.AclEntryIdentifier))).ToList().ForEach(ue => ue.IsInList = true);

                entries.AddRange(usersEntries);
            }

            #endregion

            return entries;
        }


        /// <summary>
        /// Get access permissible entities that can be added to the object acl
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override List<IAccessControlEntry> GetAccessPermissible(params string[] permissions)
        {
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, ControllableResource, RequiredEditorPermission))
            {
                throw new AuthorizationException();
            }

            //Get identities that the logged-in principal can view, and are in roles that support at least one of the ControllableResources supported permissions.
            List<string> rawUserList = UserManager.ListUsers(CurrentPrincipal);

            //Get groups that the logged-in principal can view. 
            var context = new PaginationContext {PermissionJoin = PermissionJoin.Any};
            context.Permissions.Add("Group.View");

            List<Int32> rawGroupList = GroupManager.ListAccessibleGroups(CurrentPrincipal, context, true);

            var entries = new List<IAccessControlEntry>();

             //Add identities to the list of available entries if they are not already on the ACL
            entries.AddRange((from groupId in rawGroupList
                              where !ControllableResource.ACL.IsInList(ControllableResource.ACL.CreateAclIdentifier("Checkbox.Users.Group", groupId.ToString()))
                              select new AccessControlEntry("Checkbox.Users.Group", groupId.ToString(), -1)));

            //Add identities to the list of available entries if they are not already on the ACL
            entries.AddRange((from userUniqueIdentifier in rawUserList
                              where !ControllableResource.ACL.IsInList(ControllableResource.ACL.CreateAclIdentifier("Prezza.Framework.Security.ExtendedPrincipal", userUniqueIdentifier))
                              select new AccessControlEntry("Prezza.Framework.Security.ExtendedPrincipal", userUniqueIdentifier, -1)));

            return entries;
        }
    }
}
