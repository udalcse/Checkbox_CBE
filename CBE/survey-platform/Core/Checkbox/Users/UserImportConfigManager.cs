using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Data;

namespace Checkbox.Users
{
    /// <summary>
    /// Represents the user import configs manager
    /// </summary>
    public static class UserImportConfigManager
    {
        /// <summary>
        /// Removes the user import configs.
        /// </summary>
        public static void RemoveUserImportConfigs()
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_RemoveUserImportConfig");
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Saves CSV configuration to the database
        /// </summary>
        /// <param name="userImportConfigs">The user import configs.</param>
        public static void SaveUserImportConfigs(List<string> userImportConfigs)
        {
            var db = DatabaseFactory.CreateDatabase();
            var importConfigsStr = string.Join(",", userImportConfigs);

            var command = db.GetStoredProcCommandWrapper("ckbx_sp_SaveUserImportConfig");
            command.AddInParameter("@ImportConfigsStr", DbType.String, importConfigsStr);
                db.ExecuteNonQuery(command);
        }
    }
}
