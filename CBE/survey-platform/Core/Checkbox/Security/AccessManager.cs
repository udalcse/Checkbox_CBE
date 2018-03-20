//===============================================================================
// Prezza Technologies Checkbox
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Data;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Users;
using Prezza.Framework.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Security;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.Caching;

namespace Checkbox.Security
{
    /// <summary>
    /// Provides interface for defining Roles and Policies used in controlling access to resources
    /// </summary>
    public static class AccessManager
    {
        private static readonly CacheManager _permissionMaskDisplayNamesCacheManager;
        private static readonly CacheManager _permissionMaskPermissionNamesCacheManager;
        private static readonly CacheManager _permissionNamesCacheManager;

        private static readonly object _synchObject = new object();

        /// <summary>
        /// Initialize internal caches
        /// </summary>
        static AccessManager()
        {
            lock (_synchObject)
            {
                _permissionMaskDisplayNamesCacheManager = CacheFactory.GetCacheManager("permissionMaskDisplayNames");
                _permissionMaskPermissionNamesCacheManager = CacheFactory.GetCacheManager("permissionMaskPermissionNames");
                _permissionNamesCacheManager = CacheFactory.GetCacheManager("permissionNames");
            }
        }

        /// <summary>
        /// Create the specified policy in the database.
        /// </summary>
        /// <param name="p">Policy to create.</param>
        /// <returns>Database identifier of the policy.</returns>
        public static int CreatePolicy(Policy p)
        {
            ArgumentValidation.CheckForNullReference(p, "p");

            try
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_CreatePolicy");
                        command.AddInParameter("PolicyType", DbType.String, p.GetType().ToString());
                        command.AddInParameter("PolicyAssemblyName", DbType.String, p.GetType().Assembly.GetName().Name);
                        command.AddOutParameter("PolicyID", DbType.Int32, 4);

                        db.ExecuteNonQuery(command, transaction);

                        AddPolicyPermissions(p, db, command, transaction);

                        transaction.Commit();

                        return (int)command.GetParameterValue("PolicyID");
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessProtected");

                        transaction.Rollback();
                        throw new Exception("Unable to save Policy.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return -1;
        }

        /// <summary>
        /// Delete specific user in all ACL
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void DeleteUserEntriesInAllAcl(string uniqueIdentifier)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_DeleteUserEntries");
                        command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);

                        db.ExecuteNonQuery(command, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessProtected");

                        transaction.Rollback();
                        throw new Exception("Unable to delete user ACL entries.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Update the specified policy.
        /// </summary>
        /// <param name="policyID">ID of the policy to update.</param>
        /// <param name="p">Policy data.</param>
        public static void UpdatePolicy(int policyID, Policy p)
        {
            ArgumentValidation.CheckForNullReference(p, "p");

            try
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_DelPolicyPermissions");
                        command.AddInParameter("PolicyID", DbType.Int32, policyID);

                        db.ExecuteNonQuery(command, transaction);

                        AddPolicyPermissions(p, db, command, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessProtected");

                        transaction.Rollback();
                        throw new Exception("Unable to save Policy.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="db"></param>
        /// <param name="command"></param>
        /// <param name="transaction"></param>
        public static void AddPolicyPermissions(Policy policy, Database db, DBCommandWrapper command, IDbTransaction transaction)
        {
            if (policy.Permissions != null)
            {
                foreach (string permission in policy.Permissions)
                {
                    DBCommandWrapper permissionCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_AddPolicyPermission");
                    permissionCommand.AddInParameter("PolicyID", DbType.Int32, (int)command.GetParameterValue("PolicyID"));
                    permissionCommand.AddInParameter("PermissionName", DbType.String, permission);

                    db.ExecuteNonQuery(permissionCommand, transaction);
                }
            }
        }

        /// <summary>
        /// Get the display name for a permission mask.  Return the mask's name if the display
        /// name can't be found.
        /// </summary>
        /// <param name="maskName"></param>
        public static string GetPermissionMaskDisplayName(string maskName)
        {
            string displayName = maskName;

            //Check cache
            if (_permissionMaskDisplayNamesCacheManager.Contains(maskName))
            {
                return _permissionMaskDisplayNamesCacheManager.GetData(maskName) as string;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_GetPermissionMaskDisplayName");
            command.AddInParameter("MaskName", DbType.String, maskName);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        if (reader["MaskDisplayName"] != DBNull.Value)
                        {
                            displayName = (string)reader["MaskDisplayName"];
                        }
                    }
                }
                catch
                {
                }
            }

            //Add to cache
            lock (_synchObject)
            {
                _permissionMaskDisplayNamesCacheManager.Add(maskName, displayName);
            }

            return displayName;
        }

        /// <summary>
        /// Get the list of permissions associated with a permission mask
        /// </summary>
        /// <param name="maskName"></param>
        /// <returns></returns>
        public static List<string> GetPermissionMaskPermissions(string maskName)
        {
            //Check cache
            if (_permissionMaskPermissionNamesCacheManager.Contains(maskName))
            {
                return _permissionMaskPermissionNamesCacheManager.GetData(maskName) as List<string>;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListPermissionMaskPermissions");
            command.AddInParameter("MaskName", DbType.String, maskName);

            DataSet ds = db.ExecuteDataSet(command);

            List<string> permissions = new List<string>();

            if (ds.Tables.Count > 0)
            {
                lock (_synchObject)
                {
                    _permissionMaskPermissionNamesCacheManager.Add(maskName, permissions = DbUtility.ListDataColumnValues<string>(ds.Tables[0], "PermissionName", null, null, true));
                }
            }

            return permissions;
        }

        /// <summary>
        /// Get the list of all permission names
        /// </summary>
        /// <returns></returns>
        public static List<string> ListPermissions()
        {
            const string key = "KEY"; 

            //Check cache
            if (_permissionNamesCacheManager.Contains(key))
            {
                return _permissionNamesCacheManager.GetData(key) as List<string>;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListPermissions");

            DataSet ds = db.ExecuteDataSet(command);

            List<string> permissions = new List<string>();

            if (ds.Tables.Count > 0)
            {
                lock (_synchObject)
                {
                    _permissionNamesCacheManager.Add(key, permissions = DbUtility.ListDataColumnValues<string>(ds.Tables[0], "PermissionName", null, null, true));
                }
            }

            return permissions;
        }

        #region Security Editing Shortcuts

        /// <summary>
        /// Add the everyone group to the survey ACL with the specified permissions.
        /// </summary>
        /// <param name="accessControllable">Controllable resource to modify access to.</param>
        /// <param name="principal">Principal modifying the resource.</param>
        /// <param name="permissions">Permissions to add to everyone group policy for the controllable resource.</param>
        public static void AddEveryoneGroupWithPermissions(IAccessControllable accessControllable, ExtendedPrincipal principal, params string[] permissions)
        {
            if (permissions != null && permissions.Length > 0)
            {
                EveryoneGroup everyone = GroupManager.GetEveryoneGroup();

                //Get the current permissions for the everyone group
                Policy policy = accessControllable.ACL.GetPolicy(everyone);

                List<string> newPermissions;

                if (policy != null)
                {
                    newPermissions = new List<string>(policy.Permissions);

                    foreach (string permission in permissions)
                    {
                        if (!newPermissions.Contains(permission))
                        {
                            newPermissions.Add(permission);
                        }
                    }
                }
                else
                {
                    newPermissions = new List<string>(permissions);
                }

                SecurityEditor editor = accessControllable.GetEditor();
                editor.Initialize(principal);
                editor.ReplaceAccess(everyone, newPermissions.ToArray());

                editor.SaveAcl();
            }
        }

        /// <summary>
        /// Add a set of permissions to the survey's default policy
        /// </summary>
        /// <param name="accessControllable">Controllable resource to modify permissions of.</param>
        /// <param name="principal">Principal modifying the resource.</param>
        /// <param name="permissions">Permissions to add to the default policy.</param>
        public static void AddPermissionsToDefaultPolicy(IAccessControllable accessControllable, ExtendedPrincipal principal, params string[] permissions)
        {
            if (permissions != null && permissions.Length > 0)
            {
                //Get the default policy of the survey
                List<string> defaultPolicyPermissions = accessControllable.DefaultPolicy != null 
                                                    ? new List<string>(accessControllable.DefaultPolicy.Permissions) 
                                                    : new List<string>();

                foreach (string permission in permissions)
                {
                    if (!defaultPolicyPermissions.Contains(permission))
                    {
                        defaultPolicyPermissions.Add(permission);
                    }
                }

                //Now set the default policy
                Policy newPolicy = accessControllable.CreatePolicy(defaultPolicyPermissions.ToArray());

                SecurityEditor editor = accessControllable.GetEditor();
                editor.Initialize(principal);
                editor.SetDefaultPolicy(newPolicy);
            }
        }

        /// <summary>
        /// Remove a set of permissions from the survey's default policy
        /// </summary>
        /// <param name="accessControllable">Controllable resource to update.</param>
        /// <param name="principal">Principal modifying the resource.</param>
        /// <param name="permissions">Permissions to remove from the default policy.</param>
        public static void RemovePermissionsFromDefaultPolicy(IAccessControllable accessControllable, ExtendedPrincipal principal, params string[] permissions)
        {
            if (permissions != null && permissions.Length > 0)
            {
                if (accessControllable.DefaultPolicy != null && accessControllable.DefaultPolicy.Permissions.Count > 0)
                {
                    //Get the current list
                    List<string> defaultPolicyPermissions = new List<string>(accessControllable.DefaultPolicy.Permissions);

                    foreach (string permission in permissions)
                    {
                        if (defaultPolicyPermissions.Contains(permission))
                        {
                            defaultPolicyPermissions.Remove(permission);
                        }
                    }

                    //Create a new accessControllable
                    Policy policy = accessControllable.CreatePolicy(defaultPolicyPermissions.ToArray());

                    SecurityEditor editor = accessControllable.GetEditor();
                    editor.Initialize(principal);
                    editor.SetDefaultPolicy(policy);
                }
            }
        }

        /// <summary>
        /// Add permissions to all entries in the survey ACL
        /// </summary>
        /// <param name="accessControllable">Controllable resource to modify permissions of.</param>
        /// <param name="principal">Principal modifying the resource.</param>
        /// <param name="permissions">Permissions to add</param>
        public static void AddPermissionsToAllAclEntries(IAccessControllable accessControllable, ExtendedPrincipal principal, params string[] permissions)
        {
            if (permissions != null && permissions.Length > 0)
            {
                //Get all acl entries
                SecurityEditor editor = accessControllable.GetEditor();
                editor.Initialize(principal);

                List<IAccessControlEntry> listEntries = editor.List();

                if (listEntries != null)
                {
                    foreach (IAccessControlEntry listEntry in listEntries)
                    {
                        string entryType = listEntry.AclEntryTypeIdentifier;
                        string entryID = listEntry.AclEntryIdentifier;

                        IAccessPermissible permissible = null;

                        if (string.Compare(entryType, "Checkbox.Users.Group", true) == 0)
                        {
                            permissible = GroupManager.GetGroup(Convert.ToInt32(entryID));
                        }
                        else if (string.Compare(entryType, "Prezza.Framework.Security.ExtendedPrincipal", true) == 0)
                        {
                            permissible = UserManager.GetUserPrincipal(entryID);
                        }

                        //Check the policies and add the permissions, if necessary
                        if (permissible != null)
                        {
                            Policy policy = accessControllable.ACL.GetPolicy(permissible);
                            List<string> policyPermissions;

                            if (policy != null)
                            {
                                policyPermissions = new List<string>(policy.Permissions);

                                foreach (string permission in permissions)
                                {
                                    if (!policyPermissions.Contains(permission))
                                    {
                                        policyPermissions.Add(permission);
                                    }
                                }
                            }
                            else
                            {
                                policyPermissions = new List<string>(permissions);
                            }

                            //Set the policy for the user
                            editor.ReplaceAccess(permissible, policyPermissions.ToArray());
                        }
                    }

                    //Now that all the entries have been added, commit the changes
                    editor.SaveAcl();
                }
            }
        }

        /// <summary>
        /// Remove permissions from all ACL entries for the CurrentSurvey.
        /// </summary>
        /// <param name="accessControllable">Access controllable entity to update permissions for.</param>
        /// <param name="entryIdToIgnore">ID of entry to ignore when removing permissions.  This is useful when one entry represents an "owner" 
        /// entry that shouldn't be modified.</param>
        /// <param name="principal">Principal modifying the resource.</param>
        /// <param name="permissions">Permissions to remove.</param>
        public static void RemovePermissionsFromAllAclEntries(IAccessControllable accessControllable, ExtendedPrincipal principal, string entryIdToIgnore, params string[] permissions)
        {
            if (permissions != null && permissions.Length > 0)
            {
                //Get current user
                ExtendedPrincipal currentPrincipal = UserManager.GetCurrentPrincipal();

                if (currentPrincipal == null)
                {
                    return;
                }

                //Get all acl entries
                SecurityEditor editor = accessControllable.GetEditor();
                editor.Initialize(principal);

                List<IAccessControlEntry> listEntries = editor.List();

                if (listEntries != null)
                {
                    foreach (IAccessControlEntry listEntry in listEntries)
                    {
                        string entryType = listEntry.AclEntryTypeIdentifier;
                        string entryID = listEntry.AclEntryIdentifier;

                        //Find the permissible entry to modify permissions for, but DO NOT remove permissions for the survey
                        // creator or the current user.
                        IAccessPermissible permissible = null;

                        if (string.Compare(entryType, "Checkbox.Users.Group", true) == 0)
                        {
                            permissible = GroupManager.GetGroup(Convert.ToInt32(entryID));
                        }
                        //If the specified entry is a user entry, AND that entry is NOT to be ignored, AND the entry is not the user modifying
                        // the permissions, get the permissible entity to update access for.
                        else if (entryType.Equals("Prezza.Framework.Security.ExtendedPrincipal", StringComparison.InvariantCultureIgnoreCase)
                            && (Utilities.IsNotNullOrEmpty(entryIdToIgnore) && !entryID.Equals(entryIdToIgnore, StringComparison.InvariantCultureIgnoreCase))
                            && !entryID.Equals(currentPrincipal.Identity.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            permissible = UserManager.GetUserPrincipal(entryID);
                        }

                        //Check the policies and add the permissions, if necessary
                        if (permissible != null)
                        {
                            Policy policy = accessControllable.ACL.GetPolicy(permissible);
                            List<string> policyPermissions;

                            if (policy != null && policy.Permissions.Count > 0)
                            {
                                policyPermissions = new List<string>(policy.Permissions);

                                foreach (string permission in permissions)
                                {
                                    if (policyPermissions.Contains(permission))
                                    {
                                        policyPermissions.Remove(permission);
                                    }
                                }

                                //Set the policy for the user
                                editor.ReplaceAccess(permissible, policyPermissions.ToArray());
                            }
                        }
                    }

                    //Now that all the entries have been added, commit the changes
                    editor.SaveAcl();
                }
            }
        }

        #endregion
    }
}
