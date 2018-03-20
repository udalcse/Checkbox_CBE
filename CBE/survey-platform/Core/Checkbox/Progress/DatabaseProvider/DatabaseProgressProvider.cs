using System;
using System.Data;
using Prezza.Framework.Data;

namespace Checkbox.Progress.DatabaseProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class DatabaseProgressProvider : ProgressProviderBase
    {
        private readonly string _appContextName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appContextName"></param>
        public DatabaseProgressProvider(string appContextName = null)
        {
            _appContextName = appContextName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override ProgressData GetProgress(string key)
        {
            var db = CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Progress_Get");
            command.AddInParameter("Key", DbType.String, key);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        //parse status
                        string statusValue = DbUtility.GetValueFromDataReader(reader, "Status", string.Empty);
                        ProgressStatus status;
                        if (!Enum.TryParse(statusValue, out status))
                            status = ProgressStatus.Pending;

                        return new ProgressData
                                   {
                                       Message = DbUtility.GetValueFromDataReader(reader, "Message", string.Empty),
                                       ErrorMessage = DbUtility.GetValueFromDataReader(reader, "ErrorMessage", string.Empty),
                                       CurrentItem = DbUtility.GetValueFromDataReader(reader, "CurrentItem", 0),
                                       TotalItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", 1),
                                       Status = status,
                                       AdditionalData = DbUtility.GetValueFromDataReader(reader, "DownloadLink", string.Empty)
                        };
                    }
                }
                catch { }

                finally
                {
                    reader.Close();
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public override void ClearProgress(string key)
        {
            var db = CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Progress_Clear");
            command.AddInParameter("Key", DbType.String, key);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <param name="errorMessage"></param>
        /// <param name="status"></param>
        /// <param name="currentItem"></param>
        /// <param name="itemCount"></param>
        public override void SetProgress(string key, string message, string errorMessage, ProgressStatus status, int currentItem, int itemCount)
        {
            SetProgress(key, new ProgressData
                                 {
                                     Message = message,
                                     ErrorMessage = errorMessage,
                                     CurrentItem = currentItem,
                                     TotalItemCount = itemCount,
                                     Status = status
                                 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="progressData"></param>
        public override void SetProgress(string key, ProgressData progressData)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var db = CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Progress_Add");
            command.AddInParameter("Key", DbType.String, key);
            command.AddInParameter("Message", DbType.String, progressData.Message);
            command.AddInParameter("ErrorMessage", DbType.String, progressData.ErrorMessage);
            command.AddInParameter("CurrentItem", DbType.String, progressData.CurrentItem);
            command.AddInParameter("TotalItemCount", DbType.String, progressData.TotalItemCount);
            command.AddInParameter("Status", DbType.String, progressData.Status);
            command.AddInParameter("DownloadLink", DbType.String, progressData.AdditionalData);
            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="message"></param>
        /// <param name="currentItem"></param>
        public override void SetProgress(string progressKey, string message, int currentItem)
        {
            SetProgress(progressKey, new ProgressData
            {
                Message = message,
                ErrorMessage = string.Empty,
                CurrentItem = currentItem,
                TotalItemCount = 100,
                Status = ProgressStatus.Running,
            });
        }

        private Database CreateDatabase()
        {
            return string.IsNullOrEmpty(_appContextName) ? DatabaseFactory.CreateDatabase() 
            : DatabaseFactory.CreateDatabase(_appContextName, DatabaseFactory.DEFAULT_PROVIDER_NAME);
        }
    }
}
