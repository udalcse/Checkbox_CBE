using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Common;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.Caching;

namespace Checkbox.Users
{
    /// <summary>
    /// Static manager class for groups that encapsulates functionality previously implemented by
    /// static methods on the Group class itself.
    /// </summary>
    public static class GroupManager
    {
        /// <summary>
        /// Default ID for new groups before they are saved.
        /// </summary>
        public static int DEFAULT_ID = -1;

        private static readonly CacheManager _groupCacheManager;
        private static readonly CacheManager _membershipCacheManager;


          /// <summary>
        /// Initialize membership cache
        /// </summary>
        static GroupManager()
        {
            _groupCacheManager = CacheFactory.GetCacheManager("groups");
            _membershipCacheManager = CacheFactory.GetCacheManager("membership");
        }

         /// <summary>
        /// Get a list of ids of groups the specified user belongs to.
        /// </summary>
        /// <param name="userUniqueIdentifier">ID of user.</param>
        /// <returns></returns>
        public static List<int> ListGroupMembershipIds(string userUniqueIdentifier)
        {
            List<int> groupIdList = new List<int>();

            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                return groupIdList;
            }

            //Check cache first
            if (_membershipCacheManager.Contains(userUniqueIdentifier))
            {
                return _membershipCacheManager.GetData(userUniqueIdentifier) as List<int>;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_GetIdMemberships");
            command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int groupId = DbUtility.GetValueFromDataReader(reader, "GroupId", -1);

                        if (groupId > 0 && !groupIdList.Contains(groupId))
                        {
                            groupIdList.Add(groupId);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }


            //Cache setting
            if (ApplicationManager.AppSettings.CacheVolatileDataInApplication)
            {
                _membershipCacheManager.Add(userUniqueIdentifier, groupIdList);
            }

            return groupIdList;
        }

        /// <summary>
        /// Clears cache of the user memberships
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        public static void InvalidateUserMemberships(string userUniqueIdentifier)
        {
            _membershipCacheManager.Remove(userUniqueIdentifier);
        }

        /// <summary>
        ///  Get a list of groups the specified principal is a member of, sorted in ascending alphabetical order by name.
        /// </summary>
        /// <param name="userUniqueIdentifier">Identity of user to get group memberships for.</param>
        /// <returns>List of groups the specified principal belongs to.</returns>
        public static List<Group> GetGroupMemberships(string userUniqueIdentifier)
        {
            List<int> groupIdList = ListGroupMembershipIds(userUniqueIdentifier);

            //Get groups
            List<Group> groupList = new List<Group>();

            foreach (int groupId in groupIdList)
            {
                groupList.Add(GetGroup(groupId));
            }
            
            //Now sort use groups by performing name string comparison.  Use lamba
            // expression for simplier syntax instead of anonymous method
            groupList.Sort((g1, g2) => g1.Name.CompareTo(g2.Name));

            return groupList;
        }

        /// <summary>
        /// Get a user group based on name
        /// </summary>
        /// <param name="groupName">Name of user group.</param>
        /// <returns>User group or null if no group with specified name found.</returns>
        public static Group GetGroup(string groupName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_GetIdFromName");
            command.AddInParameter("GroupName", DbType.String, groupName);
            command.AddOutParameter("GroupId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object groupIdObj = command.GetParameterValue("GroupID");

            if (groupIdObj != null && groupIdObj != DBNull.Value)
            {
                return GetGroup((int)groupIdObj);
            }

            return null;
        }

        /// <summary>
        /// Load the group with the specified ID and return a <see cref="Group"/> object representing that group.
        /// </summary>
        /// <param name="id">Database ID of the group to load.</param>
        /// <returns>Loaded group object or null if a group with the specified ID is not found or an exception occurs while
        /// loading the group.</returns>
        public static Group GetGroup(int id)
        {
            Group theGroup = null;

            if (ApplicationManager.AppSettings.CacheVolatileDataInApplication)
            {
                theGroup = _groupCacheManager.GetData(id.ToString()) as Group;

                if (theGroup != null)
                {
                    return theGroup;
                }
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_GetGroup");
            command.AddInParameter("GroupID", DbType.Int32, id);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        theGroup = new Group(
                            DbUtility.GetValueFromDataReader(reader, "GroupName", string.Empty),
                            DbUtility.GetValueFromDataReader(reader, "Description", string.Empty),
                            id,
                            new string[] { });
                        
                        theGroup.CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty);
                        theGroup.AclID = DbUtility.GetValueFromDataReader(reader, "AclID", 0);
                        theGroup.DefaultPolicyID = DbUtility.GetValueFromDataReader(reader, "DefaultPolicy", 0);
                        theGroup.MemberCount = GetMembersCount(id);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (theGroup != null
                && ApplicationManager.AppSettings.CacheVolatileDataInApplication)
            {
                _groupCacheManager.Add(id.ToString(), theGroup);
            }

            return theGroup;
        }

        /// <summary>
        /// Get an <see cref="Array"/> of group objects whether principal has
        /// the specified permissions.
        /// </summary>
        /// <param name="currentPrincipal">Principal accessing list</param>
        /// <param name="paginationContext"></param>
        /// the caller to specify whether all or any of the permissions must be met.</param>
        /// <returns><see cref="Array"/> of user groups.</returns>
        public static List<Group> GetAccessibleGroups(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext)
        {
            List<int> groupIds = ListAccessibleGroups(currentPrincipal, paginationContext, true);
            
            List<Group> groupList = new List<Group>();

            foreach (int groupId in groupIds)
            {
                groupList.Add(GetGroup(groupId));
            }

            
            return groupList;
        }

        /// <summary>
        /// List groups accessible to the specified principal.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="includeEveryoneGroup"></param>
        /// <returns></returns>
        public static List<int> ListAccessibleGroups(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext, bool includeEveryoneGroup)
        {
            List<int> groupList = new List<int>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            //prevent sql injections. here is only one possible value
            paginationContext.SortField = "GroupName";

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAllGroups");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand("ckbx_sp_Security_ListAccessibleGroups", currentPrincipal, paginationContext);
            }

            command.AddInParameter("IncludeEveryoneGroup", DbType.Boolean, includeEveryoneGroup);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int groupId = DbUtility.GetValueFromDataReader(reader, "GroupId", -1);

                        if (groupId > 0 && !groupList.Contains(groupId))
                        {
                            groupList.Add(groupId);
                        }
                    }

                    if (reader.NextResult()
                        && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "RecordCount", groupList.Count);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return groupList;
        }

        /// <summary>
        /// List members of groups user can view/edit.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<string> ListMembersOfAccessibleGroups(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext)
        {
            var memberList = new List<string>();

            if (paginationContext == null)
            {
                paginationContext = new PaginationContext();
            }

            //Ensure filter & sort field is correct.  If this is called as part of listing of users in general, non-existant sort fields may be specified
            var oldSortField = paginationContext.SortField;
            var oldFilterField = paginationContext.FilterField;

            if ("UniqueIdentifier".Equals(oldSortField, StringComparison.InvariantCultureIgnoreCase) 
                || !"Email".Equals(oldSortField, StringComparison.InvariantCultureIgnoreCase))
            {
                paginationContext.SortField = "MemberUniqueIdentifier";
            }

            if ("UniqueIdentifier".Equals(oldFilterField, StringComparison.InvariantCultureIgnoreCase))
            {
                paginationContext.FilterField = "MemberUniqueIdentifier";
            }

            //those parameters are not in the current stored procedure
            paginationContext.DateFieldName = string.Empty;
            paginationContext.StartDate = null;
            paginationContext.EndDate = null;

            //
            paginationContext.Permissions = new List<string>(new[]{"Group.View", "Group.Edit"});
            paginationContext.PermissionJoin = PermissionJoin.Any;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = QueryHelper.CreateListAccessibleCommand(
                "ckbx_sp_Security_ListAccessibleGroupMembers",
                currentPrincipal,
                paginationContext);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //added for performance optimization
                    var members = new Dictionary<string, bool>();

                    while (reader.Read())
                    {
                        string memberName = DbUtility.GetValueFromDataReader(reader, "MemberUniqueIdentifier", string.Empty);

                        if(Utilities.IsNotNullOrEmpty(memberName)
                            && !members.ContainsKey(memberName.ToLower()))
                        {
                            memberList.Add(memberName);
                            members.Add(memberName.ToLower(), true);
                        }
                    }

                    if (reader.NextResult()
                        && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "RecordCount", memberList.Count);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            paginationContext.SortField = oldSortField;
            paginationContext.FilterField = oldFilterField;

            return memberList;
        }



        /// <summary>
        /// Delete the user group with the specified ID.  Note that this does not delete users.
        /// </summary>
        /// <param name="id">Database id of the user group to delete.</param>
        /// <param name="userContext">User attempting to delete the group.</param>
        /// <exception cref="AuthorizationException">When the principal object does not have permission
        /// to delete the specified group.</exception>
        public static void DeleteGroup(int id, ExtendedPrincipal userContext)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(userContext, GetGroup(id), "Group.Delete"))
            {
                throw new AuthorizationException();
            }

            if (id == 1)
            {
                throw new Exception("Deleting the Everyone Group is not supported.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_Delete");
                    command.AddInParameter("GroupID", DbType.Int32, id);

                    db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Create a new user group with the specified name and description.
        /// </summary>
        /// <param name="name">Name of the new user group.</param>
        /// <param name="description">Description of the new user group.</param>
        /// <returns>Newly created <see cref="Group"/> object that has not yet been persisted to the database.</returns>
        public static Group CreateGroup(string name, string description)
        {
            return new Group(name, description, DEFAULT_ID, new string[] { });
        }

        /// <summary>
        /// Copy the specified user group.
        /// </summary>
        /// <param name="group">User group to copy.</param>
        /// <param name="languageCode">Language code to use when storing the name and description of the copy.</param>
        /// <param name="currentPrincipal">User copying the group.</param>
        /// <returns>Newly created and persisted user group.</returns>
        public static Group CopyGroup(Group group, string languageCode, ExtendedPrincipal currentPrincipal)
        {
            //Determine the new name
            Int32 copyVersion = 1;
            string proposedName = group.Name + " " + TextManager.GetText("/common/duplicate", languageCode) + " " + copyVersion;
            while (IsDuplicateName(null, proposedName))
            {
                copyVersion++;
                proposedName = group.Name + " " + TextManager.GetText("/common/duplicate", languageCode) + " " + copyVersion;
            }

            Group newGroup = new Group(proposedName, group.Description, DEFAULT_ID, group.GetUserIdentifiers());
            newGroup.CreatedBy = currentPrincipal.Identity.Name;
            newGroup.Save();

            return newGroup;
        }

        /// <summary>
        /// Invalidate a group's cache
        /// </summary>
        /// <param name="group">Group containing changes to persist to the database.</param>
        /// <exception cref="AuthorizationException">When the principal object does not have permission
        /// to modify the group.</exception>
        public static void InvalidateGroupCache(Group group)
        {
            //Update cache
            if (ApplicationManager.AppSettings.CacheVolatileDataInApplication)
            {
                //Replace group in cache with updated version
                _groupCacheManager.Remove(group.ID.ToString());
            }
        }

        /// <summary>
        /// Determines if a proposed name is already used by an existing user group.
        /// </summary>
        /// <param name="groupId">ID of a group to ignore when checking for duplicates.</param>
        /// <param name="groupName">Name to check.</param>
        /// <returns>True if a group, other than the one specified by groupId, exists with the specified name; 
        /// False in all other cases. 
        /// </returns>
        public static bool IsDuplicateName(int? groupId, string groupName)
        {
            //white space is ignored when comparing the names of entities
            if (groupName != null)
            {
                groupName = groupName.Trim();
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_IsNameUnique");
            command.AddInParameter("GroupName", DbType.String, groupName);
            command.AddInParameter("GroupId", DbType.Int32, groupId.GetValueOrDefault());

            //convert evaluates to True if value is not zero; otherwise, false.
            return Convert.ToBoolean(Convert.ToInt32(db.ExecuteScalar(command)));
        }

       
        /// <summary>
        /// Get an instance of the <see cref="EveryoneGroup"/>, which implicitly contains all users
        /// in the system.  The group does not support listing members or modifying membership, but
        /// can be used to check permissions and is an Access Permissible <seealso cref="IAccessPermissible"/> 
        /// which can be granted/denied access to secured objects.
        /// </summary>
        public static EveryoneGroup GetEveryoneGroup()
        {
                Group dbGroup = GetGroup(1);

                return new EveryoneGroup(
                    dbGroup.Name,
                    dbGroup.Description,
                    dbGroup.AclID.Value,
                    dbGroup.DefaultPolicyID.Value);
        }

        /// <summary>
        /// Get a count of users in the specified user group.
        /// </summary>
        /// <param name="groupID">ID of the user group to get the membership count for.</param>
        /// <returns>Number of users in the specified user group.</returns>
        public static int GetMembersCount(int groupID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_GetMemberCount");
            command.AddInParameter("GroupId", DbType.Int32, groupID);

            object result = db.ExecuteScalar(command);

            if (result != null && result != DBNull.Value)
            {
                return (int)result;
            }

            return 0;
        }

        /// <summary>
        /// Determines if a user is not already a member of a user group.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="groupID">The user group's unique identifier.</param>
        /// <param name="member">The user's unique identifier.</param>
        /// <param name="transaction">The Transaction that database queries are being executed under. </param>
        /// <returns>True if the user is not a member. False if the user is a member.</returns>
        public static bool IsUserInGroup(Database db, int groupID, string member, IDbTransaction transaction)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_IsAMember");
            command.AddInParameter("GroupID", DbType.Int32, groupID);
            command.AddInParameter("MemberID", DbType.String, member);

            object results;

            results = transaction == null 
                ? db.ExecuteScalar(command) 
                : db.ExecuteScalar(command, transaction);
                
            return "0".Equals(results.ToString());
        }

        /// <summary>
        /// Determine whether the provided user can create other users by having access to at least one group 
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static bool UserCanCreateUsers(ExtendedPrincipal currentPrincipal)
        {
            PaginationContext context = new PaginationContext
            {
                CurrentPage = 1,
                PageSize = 25,
                Permissions = new List<string> { "Group.ManageUsers" },
                FilterField = string.Empty,
                FilterValue = string.Empty,
                SortField = string.Empty,
                SortAscending = true
            };

            return ListAccessibleGroups(currentPrincipal, context, true).Count > 0;
        }

        /// <summary>
        /// Get the list of current users for new group
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public static List<string> ListCurrentUsersForNewGroup(CheckboxPrincipal callingPrincipal, string filterValue)
        {
            if (HttpContext.Current.Session["CurrentGroupMembers"] != null)
            {
                if (!string.IsNullOrEmpty(filterValue))
                {
                    return (HttpContext.Current.Session["CurrentGroupMembers"] as List<string>).Where(x=> x.Contains(filterValue)).ToList();
                }
                return HttpContext.Current.Session["CurrentGroupMembers"] as List<string>;
            }
            return new List<string>();
        }

        /// <summary>
        /// Remove user from list of current users of new group
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ListCurrentUsersForNewGroupRemoveUser(CheckboxPrincipal callingPrincipal, string userId)
        {
            if (HttpContext.Current.Session["CurrentGroupMembers"] != null)
            {
                var currentUserList = HttpContext.Current.Session["CurrentGroupMembers"] as List<string>;
                if (currentUserList.Contains(userId))
                {
                    currentUserList.Remove(userId);
                    HttpContext.Current.Session["CurrentGroupMembers"] = currentUserList;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove user to the list of current users of new group
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ListCurrentUsersForNewGroupAddUser(CheckboxPrincipal callingPrincipal, string userId)
        {
            if (HttpContext.Current.Session["CurrentGroupMembers"] != null)
            {
                var currentUserList = HttpContext.Current.Session["CurrentGroupMembers"] as List<string>;
                if (!currentUserList.Contains(userId))
                {
                    currentUserList.Add(userId);
                    HttpContext.Current.Session["CurrentGroupMembers"] = currentUserList;
                }
            }
            else
            {
                var currentUserList = new List<string>();
                currentUserList.Add(userId);
                HttpContext.Current.Session["CurrentGroupMembers"] = currentUserList;
            }
            return true;
        }

        /// <summary>
        /// Clean the list of current user for new group
        /// </summary>
        public static void CleanCurrentUsersForNewGroup()
        {
            HttpContext.Current.Session["CurrentGroupMembers"] = null;
        }
    }
}
