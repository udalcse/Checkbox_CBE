using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Checkbox.Globalization.Text;

using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Limit of number of users in roles with a specified permission
    /// </summary>
    public abstract class RolePermissionLimit : NumericLicenseLimit
    {
		static readonly string sqlQuery;

		static RolePermissionLimit()
		{
			StringBuilder query = new StringBuilder();
			query.Append("SELECT ");
			query.Append("DISTINCT UniqueIdentifier ");
			query.Append("FROM ");
			query.Append("ckbx_IdentityRoles ");
			query.Append("INNER JOIN ckbx_Role ");
			query.Append("ON ckbx_Role.RoleID = ckbx_IdentityRoles.RoleID ");
			query.Append("LEFT OUTER JOIN ckbx_RolePermissions ");
			query.Append("ON ckbx_RolePermissions.RoleID = ckbx_Role.RoleID ");
			query.Append("LEFT OUTER JOIN ckbx_Permission ");
			query.Append("ON ckbx_Permission.PermissionID = ckbx_RolePermissions.PermissionID ");
			query.Append("WHERE ");
			query.Append("ckbx_Permission.PermissionName IN (@Permission) ");
			query.Append("OR ckbx_Role.RoleName = 'System Administrator' ");

			sqlQuery = query.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public RolePermissionLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public RolePermissionLimit(string limitValue) : base(limitValue) { }

        /// <summary>
        /// Get the permission to limit
        /// </summary>
        public abstract IEnumerable<string> PermissionNames { get; }

        /// <summary>
        /// Aggregates the permissions into string
        /// </summary>
        protected string BuildPermissionsString(bool useApostrophe)
        {
            string result = PermissionNames.Aggregate<string, string>(null, (current, name) => current + ("'" + name + "', "));
                
            if(!string.IsNullOrEmpty(result))
                return result.Remove(result.Length - 2);
                
            return string.Empty;
        }

        /// <summary>
        /// Validate the limit
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <returns></returns>
        public override LimitValidationResult ProtectedValidate(out string message)
        {
            long identityCount = CurrentCount;

            //Something is wrong...deny access
            if (identityCount < 0)
            {
                //Use a default value if the text is not present
                message = TextManager.GetText("/rolePermissionLimit/unableToGetCount", TextManager.DefaultLanguage) ?? "Unable to determine the number of in roles that have {0} permission";

                message = string.Format(message, BuildPermissionsString(false));
                return LimitValidationResult.UnableToEvaluate;
            }
            
            if (!RuntimeLimitValue.HasValue)
            {
                message = string.Empty;
                return LimitValidationResult.LimitNotReached;
            }

            if (identityCount > RuntimeLimitValue.Value)
            {
                //Use a default value if the text is not present
                message = TextManager.GetText("/rolePermissionLimit/limitExceeded", TextManager.DefaultLanguage) ?? "{0} users were found in roles with the {1} permission, but your license only allows {2} users in this role.  To correct this, remove users from these roles in the User Manager.";

                message = string.Format(message, identityCount, BuildPermissionsString(false), RuntimeLimitValue);

                return LimitValidationResult.LimitExceeded;
            }

            if (identityCount == RuntimeLimitValue.Value)
            {
                message = string.Empty;
                return LimitValidationResult.LimitReached;
            }

            message = string.Empty;
            return LimitValidationResult.LimitNotReached;
        }

        /// <summary>
        /// Get the current count of limited entities
        /// </summary>
        /// <returns></returns>
        protected override long GetCurrentCount()
        {
            return GetIdentitiesWithRolePermissionCount();
        }

        /// <summary>
        /// Get the current list of unique identifiers in limited role
        /// </summary>
        /// <returns></returns>
        protected virtual List<string> GetCurrentUsersInRole()
        {
            return GetIdentitiesWithRolePermission();
        }

        /// <summary>
        /// Get the list of identities with the specified role permission
        /// </summary>
        /// <returns></returns>
        private List<string> GetIdentitiesWithRolePermission()
        {
            List<string> identities = new List<string>();
            string query = sqlQuery.Replace("@Permission", BuildPermissionsString(true));

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetSqlStringCommandWrapper(query);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        identities.Add(DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(new Exception("Unable to get user count for Survey Editor Limit."), "BusinessProtected");
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");
                }
            }

            return identities;
        }

		/// <summary>
		/// Get the number of identities with the specified role permission
		/// </summary>
		/// <returns></returns>
		private int GetIdentitiesWithRolePermissionCount()
		{
			int identities = 0;
            string query = sqlQuery.Replace("@Permission", BuildPermissionsString(true));

			Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetSqlStringCommandWrapper(query);

			using (IDataReader reader = db.ExecuteReader(command))
			{
				try
				{
					while (reader.Read())
					{
						string uid = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);

						if (!string.IsNullOrEmpty(uid))
							identities++;
					}
				}
				catch (Exception ex)
				{
					ExceptionPolicy.HandleException(new Exception("Unable to get user count for Survey Editor Limit."), "BusinessProtected");
					ExceptionPolicy.HandleException(ex, "BusinessProtected");
				}
			}

			return identities;
		}
	}
}
