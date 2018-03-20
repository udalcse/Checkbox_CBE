using System.Collections.Generic;
using System.Data;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Interface definition for security access control list.
    /// </summary>
    public interface IAccessControlList
    {
        /// <summary>
        /// The unique ID of this AccessControlList
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Gets whether a given entity has an entry in this AccessControlList
        /// </summary>
        /// <param name="permissibleEntity">an <see cref="IAccessPermissible"/> entity, e.g., IPrincipal, Group, Role</param>
        /// <returns>true, if exists in AccessControlList; otherwise false</returns>
        bool IsInList(IAccessPermissible permissibleEntity);

        /// <summary>
        /// Gets whether a given entity has an entry in this AccessControlList
        /// </summary>
        /// <param name="aclIdentifier">an ACL identifier specific to type of access control list. Should be created
        ///  by CreateAclIdentifier method.  </param>   
        /// <returns>true, if exists in AccessControlList; otherwise false</returns>
        bool IsInList(string aclIdentifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aclTypeIdentifier"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        string CreateAclIdentifier(string aclTypeIdentifier, string aclEntryIdentifier);

        /// <summary>
        /// Gets the Policy stored for a given entry in this AccessControlList
        /// </summary>
        /// <param name="permissibleEntity">an <see cref="IAccessPermissible"/> entity, e.g., IPrincipal, Group, Role</param>
        /// <returns>the <see cref="Policy"/> for this entry, if exists; otherwise null</returns>
        Policy GetPolicy(IAccessPermissible permissibleEntity);

        /// <summary>
        /// Gets IAccessControlEntry associated with a policy ID.
        /// </summary>
        /// <param name="policyId"></param>
        /// <returns></returns>
        IAccessControlEntry GetPolicyEntry(int policyId);

        /// <summary>
        /// Get a datatable with ACL entries having all of the specified permissions.
        /// </summary>
        /// <param name="permissions">Permissions to check.</param>
        /// <returns><see cref="DataTable"/> containing ACL entry information.</returns>
        List<IAccessControlEntry> SelectAnd(string[] permissions);

        /// <summary>
        /// Get a datatable with ACL entries having any of the specified permissions.
        /// </summary>
        /// <param name="permissions">Permissions to check.</param>
        /// <returns><see cref="DataTable"/>Table containing ACL entries.</returns>
        List<IAccessControlEntry> SelectOr(string[] permissions);

        /// <summary>
        /// Get a list of all entries in the ACL, regardless of whether any permissions exist.
        /// </summary>
        /// <returns>DataTable with ACL entry information.</returns>
        List<IAccessControlEntry> SelectAll();

        /// <summary>
        /// Deletes an entry from the AccessControlList
        /// </summary>
        /// <param name="entry">the <see cref="IAccessPermissible"/> entry to delete</param>
        void Delete(IAccessPermissible entry);

        /// <summary>
        /// Adds an entry to the AccessControlList
        /// </summary>
        /// <param name="entry">the <see cref="IAccessPermissible"/> entitiy to add</param>
        /// <param name="policy">the policy to associate with this entry</param>
        void Add(IAccessPermissible entry, Policy policy);

        /// <summary>
        /// Deletes an entry from the AccessControlList
        /// </summary>
        /// <param name="entry">the <see cref="IAccessPermissible"/> entry to delete</param>
        void Delete(IAccessControlEntry entry);

        /// <summary>
        /// Adds an entry to the AccessControlList
        /// </summary>
        /// <param name="entry">the <see cref="IAccessPermissible"/> entitiy to add</param>
        void Add(IAccessControlEntry entry);

        /// <summary>
        /// Commit any pending changes to the ACL to the database.
        /// </summary>
        void Save();
    }
}