using System;
using System.Collections.Generic;
using System.Data;
using Prezza.Framework.Data;
using Prezza.Framework.Logging;

namespace Checkbox.SystemMode
{
    /// <summary>
    /// 
    /// </summary>
    public static class SystemModeManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static void LogEvent(SystemModeEvent systemEvent)
        {
            if (!string.IsNullOrEmpty(systemEvent.EventType.ToString()))
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_SystemMode_LogEvent");
                        command.AddInParameter("EventName", DbType.String, systemEvent.EventType.ToString());
                        command.AddInParameter("CreatedBy", DbType.String, systemEvent.User);

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
            else
            {
                Logger.Write("Unable to Log event on system mode change - no event type passed", "Error", 3, -1, Severity.Error);
            }
        }

        /// <summary>
        /// Adds the prep mode user to ivitation list.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        public static void AddPrepModeUser(Guid guid, bool isChecked)
        {
            if (!Guid.Empty.Equals(guid))
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_SystemMode_AddPrepModeUserToIvitationList");
                        command.AddInParameter("UserGuid", DbType.Guid, guid);
                        command.AddInParameter("Checked", DbType.Boolean, isChecked);

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
            else
            {
                Logger.Write("Unable to add a user to the invite list", "Error", 3, -1, Severity.Error);
            }
        }

        /// <summary>
        /// Determines whether [is prep mode user] [the specified unique identifier].
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is prep mode user] [the specified unique identifier]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPrepModeUser(Guid guid)
        {
            bool result = false;

            if (!Guid.Empty.Equals(guid))
            {
                Database db = DatabaseFactory.CreateDatabase();
              

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_SystemMode_IsPrepModeUser");
                        command.AddInParameter("UserGuid", DbType.Guid, guid);

                        result = (bool)db.ExecuteScalar(command);

                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                Logger.Write("Unable to add a user to the invite list", "Error", 3, -1, Severity.Error);
            }

            return result;
        }

        /// <summary>
        /// Gets the prep mode users.
        /// </summary>
        /// <returns></returns>
        public static List<Guid> GetPrepModeUserGuids()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_SystemMode_GetPrepModeUsers");

            List<Guid> userGuids = new List<Guid>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        Guid guid = DbUtility.GetValueFromDataReader(reader, "UserGuid", Guid.Empty);
                        if (!Guid.Empty.Equals(guid))
                            userGuids.Add(guid);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userGuids;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearLogHistory()
        {
            throw new NotImplementedException();
        }
    }
}