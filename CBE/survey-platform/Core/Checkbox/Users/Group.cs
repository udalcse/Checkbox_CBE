//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;

using Checkbox.Common;
using Checkbox.Users.Security;

using Prezza.Framework.Data;
using Prezza.Framework.Security;

namespace Checkbox.Users
{
    /// <summary>
    /// Container for information about what users (principals) exist in a group. 
    /// </summary>
    [Serializable]
    public class Group : AccessControllablePersistedDomainObject, IAccessPermissible
    {
        //TODO: Handle case where multiple, iterative, accesses are made to groups to determine if a user
        // is in a group.

        // Security members
        private Dictionary<string, string> _userIdentities;

        private Dictionary<string, string> _pendingInserts = new Dictionary<string, string>();
        private Dictionary<string, string> _pendingDeletes = new Dictionary<string, string>();

        /// <summary>
        /// Get object type
        /// </summary>
        public override string ObjectTypeName { get { return "Group"; } }

        /// <summary>
        /// Get load procedure for group, currently none though this will change when groups start tracking
        /// last modified status information.
        /// </summary>
        protected override string LoadSprocName { get { return string.Empty; } }

        /// <summary>
        /// User id who modified the group
        /// </summary>
        public string Modifier
        {
            get;
            set;
        }

        /// <summary>
        /// Get load configuration container for a group, currently none though this will change when groups start tracking
        /// last modified status information.
        /// </summary>
        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            return null;
        }

        #region Constructors

        /// <summary>
        /// Constructor.  Initialize a group with a name, id, and list of members.
        /// </summary>
        /// <param name="name">Name of the group.</param>
        /// <param name="description">Description of the group.</param>
        /// <param name="id">ID of the group.</param>
        /// <param name="userIdentifiers">List of group members.</param>
        public Group(string name, string description, int id, string[] userIdentifiers)
            : base(new[] { "Group.Edit", "Group.View", "Group.ManageUsers" }, new[] { "Group.Edit", "Group.View", "Group.ManageUsers", "Group.Create", "Group.Delete" })
        {
            Name = name;
            Description = description;
            ID = id;

            _userIdentities = new Dictionary<string, string>(userIdentifiers.Length);
            foreach (string userIdentity in userIdentifiers)
            {
                _userIdentities[userIdentity] = userIdentity;
            }

            MemberCount = userIdentifiers.Length;
        }
        #endregion

        /// <summary>
        /// Get domain db table
        /// </summary>
        public override string DomainDBTableName
        {
            get { return "ckbx_Group"; }
        }

        /// <summary>
        /// Get domain identity column name
        /// </summary>
        public override string DomainDBIdentityColumnName
        {
            get { return "GroupID"; }
        }

        ///// <summary>
        ///// Get data table name
        ///// </summary>
        //public override string DataTableName { get { return "UserGroupData"; } }

        ///// <summary>
        ///// Get identity column name
        ///// </summary>
        //public override string IdentityColumnName { get { return "GroupId"; } }

        /// <summary>
        /// Get/set group member count.  Value is intended only to be used in cases where it is not desireable to load
        /// all group members, such as when listing groups.  In other cases, GetUserIdentities().Count should be used.
        /// </summary>
        public int MemberCount { get; set; }


        /// <summary>
        /// Add a user to the list of members for the particular group.  If the user is already in the group, 
        /// nothing will be done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="principal"><see cref="IPrincipal"/> object representing the user to add to the group.</param>
        public void AddUser(IPrincipal principal)
        {
            AddUser(principal.Identity);
        }

        /// <summary>
        /// Add a user to the list of members for the particular group.  If the user is already in the group, 
        /// nothing will be done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="identity"><see cref="IIdentity"/> object representing the user to add to the group.</param>
        public void AddUser(IIdentity identity)
        {
            AddUser(identity.Name);
        }

        /// <summary>
        /// Add a user to the list of members for the particular group.  If the user is already in the group, 
        /// nothing will be done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="uniqueName">Unique identifier of the user to add to this group.</param>
        public void AddUser(string uniqueName)
        {
            EnsureMembersLoaded();

            if (!_userIdentities.ContainsKey(uniqueName))
            {
                _userIdentities.Add(uniqueName, uniqueName);
                _pendingInserts.Add(uniqueName, uniqueName);
                MemberCount++;
            }
        }

        /// <summary>
        /// Remove the specified user from the list of members for the particular group.  If the user is not in the group,
        /// nothing is done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="principal"><see cref="IPrincipal"/> representing the user to remove from the group.</param>
        public void RemoveUser(IPrincipal principal)
        {
            RemoveUser(principal.Identity);
        }

        /// <summary>
        /// Remove the specified user from the list of members for the particular group.  If the user is not in the group,
        /// nothing is done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="identity"><see cref="IIdentity"/> representing the user to remove from the group.</param>
        public void RemoveUser(IIdentity identity)
        {
            RemoveUser(identity.Name);
        }

        /// <summary>
        /// Remove the specified user from the list of members for the particular group.  If the user is not in the group,
        /// nothing is done. Changes are not persisted to the database until the Group.Commit() is called
        /// for the group.
        /// </summary>
        /// <param name="uniqueName">Unique identifier of the user to remove from the group.</param>
        public void RemoveUser(string uniqueName)
        {
            EnsureMembersLoaded();

            if (_userIdentities.ContainsKey(uniqueName))
            {
                _userIdentities.Remove(uniqueName);
                _pendingDeletes.Add(uniqueName, uniqueName);
                MemberCount--;
            }
        }

        /// <summary>
        /// Removes a record from the cache. We need to do it when user name is changed.
        /// </summary>
        /// <param name="uniqueName"></param>
        public void RemoveUserFromCache(string uniqueName)
        {
            _userIdentities.Remove(uniqueName);
            
            //reset also group cache
            GroupManager.InvalidateGroupCache(this);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void EnsureMembersLoaded()
        {
            if (_userIdentities == null || _userIdentities.Count == 0)
            {
                LoadMembers();
            }
        }

        /// <summary>
        /// Get an <see cref="Array"/> of unique identifiers for users that are members of the user group.
        /// </summary>
        /// <returns><see cref="Array"/> of unique identifiers.</returns>
        public string[] GetUserIdentifiers()
        {
            EnsureMembersLoaded();

            var users = new List<string>();
            
            if (_userIdentities != null)
            {
                users.AddRange(_userIdentities.Keys);
            }

            return users.ToArray();
        }


        //public List<> ListUsers(PaginationContext paginationContext)
        //{

        //}
        /// <summary>
        /// Lazy loader for the members of this group
        /// </summary>
        private void LoadMembers()
        {
            _userIdentities = new Dictionary<string, string>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_GetMembers");
            command.AddInParameter("GroupID", DbType.Int32, ID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (reader[0] != null && reader[0] != DBNull.Value && reader[0] is string)
                        {
                            var user = (string)reader[0];
                            _userIdentities[user] = user;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        #region Properties

        /// <summary>
        /// Get/set the description of the user group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get/set the creator of the user group.
        /// </summary>
        public string CreatedBy { get; set; }

        #endregion

        /// <summary>
        /// Set the ACL id
        /// </summary>
        /// <param name="aclId"></param>
        protected void SetAclID(int aclId)
        {
            AclID = aclId;
        }

        /// <summary>
        /// Set default policy id
        /// </summary>
        /// <param name="defaultPolicyId"></param>
        protected void SetDefaultPolicyID(int defaultPolicyId)
        {
            DefaultPolicyID = defaultPolicyId;
        }

        /// <summary>
        /// Get a <see cref="SecurityEditor"/> object that can be used to modify access control list and default policy information
        /// for the user group.
        /// </summary>
        /// <returns><see cref="SecurityEditor"/> object for this user group.</returns>
        public override SecurityEditor GetEditor()
        {
            return new GroupSecurityEditor(this);
        }

        /// <summary>
        /// Create an instance of the group.  Should not be called directly.
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            Commit();
        }

        /// <summary>
        /// Create an instance of the group.  Should not be called directly.  Static method Group.Commit
        /// should be called.  This will eventually change so that it fits the persisted domain object model.
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            Commit();
        }

        /// <summary>
        /// Commit changes to the group
        /// </summary>
        private void Commit()
        {
            if (ID > 0)
            {
                DoUpdate();

                _pendingInserts = new Dictionary<string, string>();
                _pendingDeletes = new Dictionary<string, string>();
            }
            else
            {
                DoInsert(string.IsNullOrEmpty(CreatedBy) ? System.Threading.Thread.CurrentPrincipal.Identity.Name : CreatedBy);

                _pendingInserts = new Dictionary<string, string>();
                _pendingDeletes = new Dictionary<string, string>();
            }

            //A little funky, but the group manager does some basic caching of groups and
            // group membership information for a single identity for more efficient processing
            // when multiple, iterative, is in group calls are made for an identity.
            GroupManager.InvalidateGroupCache(this);
        }

        /// <summary>
        /// Perform the actual update of the user group by making changes to ckbx_GroupMembers and the
        /// ckbx_Group table.
        /// </summary>
        private void DoUpdate()
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    AddMembersToGroup(db, _pendingInserts, transaction);

                    //Delete removed members
                    foreach (string member in _pendingDeletes.Keys)
                    {
                        DBCommandWrapper deleteCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Group_RemoveMember");
                        deleteCommand.AddInParameter("GroupID", DbType.Int32, ID);
                        deleteCommand.AddInParameter("MemberID", DbType.String, member);

                        db.ExecuteNonQuery(deleteCommand, transaction);
                    }

                    //Update the group name and description
                    DBCommandWrapper updateGroupCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Group_Update");
                    updateGroupCommand.AddInParameter("GroupId", DbType.Int32, ID);
                    updateGroupCommand.AddInParameter("GroupName", DbType.String, Name);
                    updateGroupCommand.AddInParameter("Description", DbType.String, Description);
                    updateGroupCommand.AddInParameter("Modifier", DbType.String, Modifier);

                    db.ExecuteNonQuery(updateGroupCommand, transaction);

                    //Commit the transaction
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
        /// Create an entry for a group in ckbx_Group table.
        /// </summary>
        /// <param name="creatorUniqueIdentifier">Unique identifier of the group creator.</param>
        /// <exception cref="DataException">When ckbx_sp_Group_Create does not return a valid Group ID or Group ACL ID.</exception>
        private void DoInsert(string creatorUniqueIdentifier)
        {
            Database db = DatabaseFactory.CreateDatabase();



            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    //Insert the entry into ckbx_Group
                    DBCommandWrapper insertGroupCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Group_Create");
                    insertGroupCommand.AddInParameter("GroupName", DbType.String, Name);
                    insertGroupCommand.AddInParameter("Description", DbType.String, Description);
                    insertGroupCommand.AddInParameter("CreatedBy", DbType.String, creatorUniqueIdentifier);
                    insertGroupCommand.AddOutParameter("GroupID", DbType.Int32, 4);
                    insertGroupCommand.AddOutParameter("GroupACLID", DbType.Int32, 4);
                    insertGroupCommand.AddOutParameter("GroupDefaultPolicyID", DbType.Int32, 4);

                    db.ExecuteNonQuery(insertGroupCommand, transaction);

                    object groupID = insertGroupCommand.GetParameterValue("GroupID");
                    object aclID = insertGroupCommand.GetParameterValue("GroupACLID");
                    object defaultPolicyID = insertGroupCommand.GetParameterValue("GroupDefaultPolicyID");

                    if (groupID == null || groupID == DBNull.Value)
                    {
                        throw new DataException("Error creating new Group");
                    }

                    if (aclID == null || aclID == DBNull.Value)
                    {
                        throw new DataException("Error creating ACL for Group");
                    }

                    if (defaultPolicyID == null || defaultPolicyID == DBNull.Value)
                    {
                        throw new DataException("Error creating Default Policy for Group");
                    }

                    ID = (int)groupID;
                    SetAclID((int)aclID);
                    SetDefaultPolicyID((int)defaultPolicyID);
                    CreatedBy = creatorUniqueIdentifier;

                    AddMembersToGroup(db, _userIdentities, transaction);

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
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="members"></param>
        /// <param name="transaction"></param>
        private void AddMembersToGroup(Database db, Dictionary<string, string> members, IDbTransaction transaction)
        {
            foreach (string member in members.Keys)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Group_AddMember");
                command.AddInParameter("GroupID", DbType.Int32, ID);
                command.AddInParameter("MemberID", DbType.String, member);

                db.ExecuteNonQuery(command, transaction);
            }
        }
       

        #region IAccessPermissible Members

        /// <summary>
        /// Get the identifier for this group when used in access control list entries.
        /// </summary>
        public virtual string AclEntryIdentifier
        {
            get
            {
                return ID.ToString();
            }
        }

        /// <summary>
        /// Get the string representation of the ACL type for this principal
        /// </summary>
        public const string GroupAclTypeIdentifier = "Checkbox.Users.Group";

        /// <summary>
        /// 
        /// </summary>
        public string AclTypeIdentifier { get { return GroupAclTypeIdentifier; } }


        #endregion
    
    }
}
