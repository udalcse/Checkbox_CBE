using System;
using System.Data;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Workflow
{
    /// <summary>
    /// Static class for manaing response session data.
    /// </summary>
    public static class ResponseSessionDataManager
    {
        /// <summary>
        /// Save session data for a response session.
        /// </summary>
        /// <param name="sessionData"></param>
        public static void SaveResponseSessionData(ResponseSessionData sessionData)
        {
            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    DeleteSessionData(sessionData.SessionGuid, db);
                    InsertSessionData(sessionData.SessionGuid, db);
                    UpdateResponseSessionData(sessionData, db);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Get session data for a given response key
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static ResponseSessionData GetResponseSessionData(Guid sessionKey)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetSession");
            command.AddInParameter("SessionGuid", DbType.Guid, sessionKey);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new ResponseSessionData
                        {
                            SessionGuid = sessionKey,
                            AnonymousRespondentGuid = DbUtility.GetValueFromDataReader<Guid?>(reader, "AnonymousRespondentUid", null),
                            AuthenticatedRespondentUid = DbUtility.GetValueFromDataReader(reader, "AuthenticatedRespondentUid", string.Empty),
                            EnteredPassword = DbUtility.GetValueFromDataReader(reader, "EnteredPassword", string.Empty),
                            ResponseId = DbUtility.GetValueFromDataReader<Int64?>(reader, "ResponseId", null),
                            ResponseTemplateId = DbUtility.GetValueFromDataReader<Int32?>(reader, "ResponseTemplateId", null),
                            SelectedLanguage = DbUtility.GetValueFromDataReader(reader, "SelectedLanguage", string.Empty)
                        };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            //Otherwise, create  new session key
            InsertSessionData(sessionKey, db);

            //Return value
            return new ResponseSessionData { SessionGuid = sessionKey };
        }

        /// <summary>
        /// Insert data for a response session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="db"></param>
        private static void InsertSessionData(Guid sessionKey, Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_InsertSession");
            command.AddInParameter("SessionGuid", DbType.Guid, sessionKey);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete data for a response session.
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="db"></param>
        private static void DeleteSessionData(Guid sessionKey, Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_DeleteSession");
            command.AddInParameter("SessionGuid", DbType.Guid, sessionKey);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Update data for a response session.
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="db"></param>
        private static void UpdateResponseSessionData(ResponseSessionData sessionData, Database db)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_UpdateSession");
            command.AddInParameter("SessionGuid", DbType.Guid, sessionData.SessionGuid);
            command.AddInParameter("AnonymousRespondentUid", DbType.Guid, sessionData.AnonymousRespondentGuid);
            command.AddInParameter("AuthenticatedRespondentUid", DbType.String, sessionData.AuthenticatedRespondentUid);
            command.AddInParameter("EnteredPassword", DbType.String, sessionData.EnteredPassword);
            command.AddInParameter("ResponseId", DbType.Int64, sessionData.ResponseId);
            command.AddInParameter("ResponseTemplateId", DbType.Int32, sessionData.ResponseTemplateId);
            command.AddInParameter("SelectedLanguage", DbType.String, sessionData.SelectedLanguage);

            db.ExecuteNonQuery(command);
        }
    }
}
