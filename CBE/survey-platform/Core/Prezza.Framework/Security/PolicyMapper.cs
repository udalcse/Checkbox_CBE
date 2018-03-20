using System;
using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using Prezza.Framework.Caching;
using Prezza.Framework.Caching.Expirations;

namespace Prezza.Framework.Security
{
    /// <summary>
    /// Internal class for maintaining policy data.
    /// </summary>
    /// <remarks>Since policy data storage is application specific, this should eventually move to
    /// Checkbox namespace.</remarks>
    internal static class PolicyMapper
    {

        /// <summary>
        /// Cache
        /// </summary>
        private static readonly CacheManager _cache;

        /// <summary>
        /// Static contructor
        /// </summary>
        static PolicyMapper()
        {
            _cache = CacheFactory.GetCacheManager("policyCacheManager");
        }


        /// <summary>
        /// Find and load a policy with the given id.
        /// </summary>
        /// <param name="policyID"></param>
        /// <returns></returns>
        public static Policy Find(int policyID)
        {
            var res = _cache.GetData(policyID.ToString()) as Policy;
            if (res != null)
                return res;

            ArgumentValidation.CheckExpectedType(policyID, typeof(Int32));

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_GetPolicy");
            command.AddInParameter("PolicyID", DbType.Int32, policyID);

            var policy = Load(db.ExecuteDataSet(command));

            _cache.Add(policyID.ToString(), policy);

            return policy;
        }

        /// <summary>
        /// Remove policy from cache
        /// </summary>
        /// <param name="policyID"></param>
        /// <returns></returns>
        public static void CleanupPolicyCaches(int policyID)
        {
            _cache.Remove(policyID.ToString());
        }

        /// <summary>
        /// Create the specified access policy in the database.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static int Insert(Policy policy)
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    int policyID = Insert(policy, transaction);

                    transaction.Commit();

                    return policyID;
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
        /// Create the specified policy in the database.  If a transaction is provided, the database write will
        /// occur in the context of that transaction and any rollback and commit logic must be implemented by the
        /// caller.
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int Insert(Policy policy, IDbTransaction transaction)
        {
            ArgumentValidation.CheckForNullReference(policy, "policy");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper createPolicy = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_CreatePolicy");
            createPolicy.AddInParameter("PolicyType", DbType.String, policy.GetType().FullName);
            createPolicy.AddInParameter("PolicyAssemblyName", DbType.String, policy.GetType().Assembly.GetName().Name);
            createPolicy.AddOutParameter("PolicyID", DbType.Int32, 4);

            db.ExecuteNonQuery(createPolicy, transaction);
            if (createPolicy.GetParameterValue("PolicyID") == DBNull.Value)
                throw new DataException("Could not create Policy");

            int policyID = (int)createPolicy.GetParameterValue("PolicyID");

            foreach (string permission in policy.Permissions)
            {
                DBCommandWrapper addPolicyPermission = db.GetStoredProcCommandWrapper("ckbx_sp_ACL_AddPolicyPermission");
                addPolicyPermission.AddInParameter("PolicyID", DbType.Int32, policyID);
                addPolicyPermission.AddInParameter("PermissionName", DbType.String, permission);

                db.ExecuteNonQuery(addPolicyPermission, transaction);
            }

            return policyID;
        }

        /// <summary>
        /// Load a policy from a dataset.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Policy Load(DataSet data)
        {
            if (data == null
                || data.Tables.Count < 1
                || data.Tables[0].Rows.Count < 1)
            {
                return null;
            }

            List<string> policyPermissions = new List<string>();

            if (data.Tables.Count > 1)
            {
                policyPermissions = DbUtility.ListDataColumnValues<string>(data.Tables[1], "PermissionName", null, null, true);
            }

            string policyTypeName = DbUtility.GetValueFromDataRow(data.Tables[0].Rows[0], "PolicyType", string.Empty);
            string policyAssemblyName = DbUtility.GetValueFromDataRow(data.Tables[0].Rows[0], "PolicyAssemblyName", string.Empty);

            if (string.IsNullOrEmpty(policyTypeName))
            {
                throw new ExpectedDataNotFoundException("PolicyType not found in selected datasource");
            }

            if (string.IsNullOrEmpty(policyAssemblyName))
            {
                throw new ExpectedDataNotFoundException("PolicyAssemblyName not found in selected datasource");
            }

            //Load type
            Type type = Type.GetType(policyTypeName + "," + policyAssemblyName, true, false);

            return (Policy)Activator.CreateInstance(type, new object[] { policyPermissions.ToArray() });
        }
    }
}

