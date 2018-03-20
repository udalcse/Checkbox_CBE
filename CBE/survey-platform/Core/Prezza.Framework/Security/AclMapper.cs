using System;
using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Data;
using Prezza.Framework.Caching;
using Prezza.Framework.Caching.Expirations;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Internal class for handling persistant storage and retrieval of acl information.  
    /// </summary>
    /// <remarks>
    /// Since this 
    /// storage is specific to an application, this class will eventually move to the Checkbox 
    /// namespace.
    /// </remarks>
    internal static class AclMapper
    {
        /// <summary>
        /// Create an ACL
        /// </summary>
        /// <returns></returns>
        internal static int Insert()
        {
            // create new record in database and set return value as id
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_Create");
            command.AddOutParameter("AclID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object aclID = command.GetParameterValue("AclID");

            if (aclID != null && aclID != DBNull.Value)
            {
                return (int)aclID;
            }

            throw new Exception("Unable to create ACL.");
        }

        /// <summary>
        /// Find an ACL in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static IAccessControlList Find(int id)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_GetAcl");
            command.AddInParameter("AclID", DbType.Int32, id);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new AccessControlList(id);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Update an access control list
        /// </summary>
        /// <param name="acl"></param>
        internal static void Update(AbstractAccessControlList acl)
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DeleteAclEntries(acl.ID, acl.GetEntriesToDelete(), transaction);
                            AddAclEntries(acl.ID, acl.GetEntriesToAdd(), transaction);

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }

                    }
                }
                finally
                {
                    connection.Close();
                }
            }

            _cache.Remove(acl.ID.ToString());
        }

        /// <summary>
        /// Delete acl entries as part of a database transaction.
        /// </summary>
        /// <param name="aclId"></param>
        /// <param name="entriesToDelete"></param>
        /// <param name="transaction"></param>
        private static void DeleteAclEntries(int aclId, List<IAccessControlEntry> entriesToDelete, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (IAccessControlEntry entry in entriesToDelete)
            {
                DBCommandWrapper deleteCommand = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_DeleteEntry");
                deleteCommand.AddInParameter("AclID", DbType.Int32, aclId);
                deleteCommand.AddInParameter("EntryType", DbType.String, entry.AclEntryTypeIdentifier);
                deleteCommand.AddInParameter("EntryIdentifier", DbType.String, entry.AclEntryIdentifier);

                db.ExecuteNonQuery(deleteCommand, transaction);
            }
        }

        /// <summary>
        /// Add acl entries to database.
        /// </summary>
        /// <param name="aclId"></param>
        /// <param name="entriesToAdd"></param>
        /// <param name="transaction"></param>
        private static void AddAclEntries(int aclId, List<IAccessControlEntry> entriesToAdd, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (IAccessControlEntry entry in entriesToAdd)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_AddEntry");
                command.AddInParameter("AclID", DbType.Int32, aclId);
                command.AddInParameter("EntryType", DbType.String, entry.AclEntryTypeIdentifier);
                command.AddInParameter("EntryIdentifier", DbType.String, entry.AclEntryIdentifier);
                command.AddInParameter("PolicyID", DbType.Int32, PolicyMapper.Insert(entry.Policy, transaction));

                db.ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// Cache
        /// </summary>
        private static readonly CacheManager _cache;

        /// <summary>
        /// Static contructor
        /// </summary>
        static AclMapper()
        {
            _cache = CacheFactory.GetCacheManager("aclCacheManager");
        }

        /// <summary>
        /// ist
        /// </summary>
        /// <param name="aclId"></param>
        /// <returns></returns>
        internal static List<IAccessControlEntry> ListEntries(int aclId)
        {
            var res = _cache.GetData(aclId.ToString()) as List<IAccessControlEntry>;
            if (res != null)
                return res;

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_GetEntries");
            command.AddInParameter("AclID", DbType.Int32, aclId);
            
            List<IAccessControlEntry> entries = new List<IAccessControlEntry>();

            //Load entries
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        entries.Add(new AccessControlEntry(
                            DbUtility.GetValueFromDataReader(reader, "EntryType", string.Empty),
                            DbUtility.GetValueFromDataReader(reader, "EntryIdentifier", string.Empty),
                            DbUtility.GetValueFromDataReader(reader, "PolicyId", -1)));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            _cache.Add(aclId.ToString(), entries);

            return entries;
        }
    }
}
