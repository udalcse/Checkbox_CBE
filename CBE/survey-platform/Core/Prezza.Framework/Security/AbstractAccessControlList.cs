using System;
using System.Linq;
using System.Collections.Generic;
using Prezza.Framework.Common;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Abstract base class
    /// </summary>
    [Serializable]
    public abstract class AbstractAccessControlList : IAccessControlList
    {
        /// <summary>
        /// In-memory store for loaded ACL Entries.
        /// </summary>
        private Dictionary<string, IAccessControlEntry> _entries;

        /// <summary>
        /// New entries to add to the acl
        /// </summary>
        private Dictionary<string, IAccessControlEntry> _entriesToAdd;

        /// <summary>
        /// Entries to delete from the acl
        /// </summary>
        private Dictionary<string, IAccessControlEntry> _entriesToDelete;

        /// <summary>
        /// The unique ID of this AccessControlList
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected AbstractAccessControlList()
        {
        }

        /// <summary>
        /// Abstract control list with acl id
        /// </summary>
        /// <param name="aclId"></param>
        protected AbstractAccessControlList(int aclId)
        {
            ID = aclId;
        }

        /// <summary>
        /// Get a reference to the entries dictionary
        /// </summary>
        protected Dictionary<string, IAccessControlEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    //Create the dictionary
                    _entries = new Dictionary<string, IAccessControlEntry>(StringComparer.InvariantCultureIgnoreCase);

                    //Populate it
                    LoadEntries();
                }

                return _entries;
            }
        }

        /// <summary>
        /// Get a reference to the dictionary of entries to add
        /// </summary>
        protected Dictionary<string, IAccessControlEntry> EntriesToAdd
        {
            get {
                return _entriesToAdd ??
                       (_entriesToAdd =
                        new Dictionary<string, IAccessControlEntry>(StringComparer.InvariantCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Get a reference to the dictionary of entries to delete
        /// </summary>
        protected Dictionary<string, IAccessControlEntry> EntriesToDelete
        {
            get {
                return _entriesToDelete ??
                       (_entriesToDelete =
                        new Dictionary<string, IAccessControlEntry>(StringComparer.InvariantCultureIgnoreCase));
            }
        }

        /// <summary>
        /// Get a list of all acl entries to delete
        /// </summary>
        /// <returns></returns>
        public List<IAccessControlEntry> GetEntriesToDelete()
        {
            return new List<IAccessControlEntry>(EntriesToDelete.Values);
        }

        /// <summary>
        /// Get a list of all acl entries to add.
        /// </summary>
        /// <returns></returns>
        public List<IAccessControlEntry> GetEntriesToAdd()
        {
            return new List<IAccessControlEntry>(EntriesToAdd.Values);
        }

        /// <summary>
        /// Commit changes to the acl
        /// </summary>
        public void Save()
        {
            if (ID <= 0)
            {
                ID = AclMapper.Insert();
            }

            //Insert entries
            AclMapper.Update(this);

            //Clear pending changes
            EntriesToAdd.Clear();
            EntriesToDelete.Clear();
        }

        /// <summary>
        /// Creates a string identifier for use in AccessControlLists
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string CreateAclIdentifier(IAccessControlEntry entry)
        {
            return CreateAclIdentifier(entry.AclEntryTypeIdentifier, entry.AclEntryIdentifier);
        }


        /// <summary>
        /// Creates a string identifier for use in AccessControlLists
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string CreateAclIdentifier(IAccessPermissible entry)
        {
            return CreateAclIdentifier(entry.AclTypeIdentifier, entry.AclEntryIdentifier);
        }

      
        /// <summary>
        /// Create a string identifier for use in Access Control List
        /// </summary>
        /// <param name="aclTypeIdentifier"></param>
        /// <param name="aclEntryIdentifier"></param>
        /// <returns></returns>
        public string CreateAclIdentifier(string aclTypeIdentifier, string aclEntryIdentifier)
        {
            return aclTypeIdentifier + "\\" + aclEntryIdentifier;
        }

        /// <summary>
        /// Load entries for the access control list
        /// </summary>
        protected void LoadEntries()
        {
            List<IAccessControlEntry> entries = AclMapper.ListEntries(ID);

            foreach (IAccessControlEntry entry in entries)
            {
                Entries[CreateAclIdentifier(entry)] = entry;
            }
        }

        /// <summary>
        /// Add an entry to the list
        /// </summary>
        /// <param name="entry"></param>
        public virtual void Add(IAccessControlEntry entry)
        {
            string entryKey = CreateAclIdentifier(entry);

            EntriesToAdd[entryKey] = entry;
            Entries[entryKey] = entry;

            //Unlike the Delete case, the entry to add is not removed from the 
            // EntriesToDelete queue.  This is because the delete happens at save time before
            // the add action and the only way to replace policy permissions for an entry is to call
            // Acl.Delete(...) then Acl.Add(...).   Removing the entry from the delete queue would
            // mean that that when the ACL is saved, the entry to replace would not be deleted from the 
            // database.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public IAccessControlEntry GetPolicyEntry(int policyId)
        {
            return Entries.Values.FirstOrDefault(entry => entry.PolicyId == policyId);
        }

        /// <summary>
        /// Add an entry to an acess control list
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="policy"></param>
        public virtual void Add(IAccessPermissible entry, Policy policy)
        {
            Add(new AccessControlEntry(entry, policy));
        }

        /// <summary>
        /// Remove an entry from an access control list
        /// </summary>
        /// <param name="entry"></param>
        /// <remarks>Entries aren't persisted until the ACL is committed.</remarks>
        public virtual void Delete(IAccessPermissible entry)
        {
            Delete(new AccessControlEntry(entry, null));
        }

        /// <summary>
        /// Delete an entry from the access list
        /// </summary>
        /// <param name="entry"></param>
        public virtual void Delete(IAccessControlEntry entry)
        {
            string entryKey = CreateAclIdentifier(entry);

            EntriesToDelete[entryKey] = entry;

            if (EntriesToAdd.ContainsKey(entryKey))
            {
                EntriesToAdd.Remove(entryKey);
            }

            if (Entries.ContainsKey(entryKey))
            {
                Entries.Remove(entryKey);
            }
        }

        /// <summary>
        /// Gets the Policy stored for a given entry in this AccessControlList
        /// </summary>
        /// <param name="permissibleEntity">an <see cref="IAccessPermissible"/> entity, e.g., IPrincipal, Group, Role</param>
        /// <returns>the <see cref="Policy"/> for this entry, if exists; otherwise null</returns>
        public virtual Policy GetPolicy(IAccessPermissible permissibleEntity)
        {
            if (!IsInList(permissibleEntity))
            {
                return null;
            }

            return Entries[CreateAclIdentifier(permissibleEntity)].Policy;
        }

        /// <summary>
        /// Gets whether a given entity has an entry in this AccessControlList
        /// </summary>
        /// <param name="permissibleEntity">an <see cref="IAccessPermissible"/> entity, e.g., IPrincipal, Group, Role</param>
        /// <returns>true, if exists in AccessControlList; otherwise false</returns>
        public virtual bool IsInList(IAccessPermissible permissibleEntity)
        {
            ArgumentValidation.CheckForNullReference(permissibleEntity, "permissibleEntity");

            return Entries.ContainsKey(CreateAclIdentifier(permissibleEntity));
        }

        /// <summary>
        /// Gets whether a given entity has an entry in this AccessControlList
        /// </summary>
        /// <param name="entryIdentifier">an <see cref="IAccessPermissible"/>Entry id generated by CreateAclIdentifier</param>
        /// <returns>true, if exists in AccessControlList; otherwise false</returns>
        public virtual bool IsInList(string entryIdentifier)
        {
            ArgumentValidation.CheckForNullReference(entryIdentifier, "entryIdentifier");

            return Entries.ContainsKey(entryIdentifier);
        }

        /// <summary>
        /// Select all entries on the access list
        /// </summary>
        /// <returns></returns>
        public virtual List<IAccessControlEntry> SelectAll()
        {
            return new List<IAccessControlEntry>(Entries.Values);
        }

        /// <summary>
        /// Select all entries on the access list that have all specified permissions.
        /// </summary>
        /// <returns></returns>
        public virtual List<IAccessControlEntry> SelectAnd(string[] permissions)
        {
            var allEntries = new List<IAccessControlEntry>(Entries.Values);

            //Now find all matching
            return allEntries.FindAll(entry => entry.Policy != null && entry.Policy.HasAllPermissions(permissions));
        }

        /// <summary>
        /// Select all entries on the access list where policy has at least one permission.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual List<IAccessControlEntry> SelectOr(string[] permissions)
        {
            var allEntries = new List<IAccessControlEntry>(Entries.Values);

            //Now find all matching
            return allEntries.FindAll(entry => entry.Policy != null && entry.Policy.HasAtLeastOnePermission(permissions));
        }
    }
}