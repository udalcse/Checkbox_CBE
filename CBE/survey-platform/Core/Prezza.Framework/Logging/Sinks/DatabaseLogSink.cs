using System;
using System.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Data;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Sinks
{
    ///<summary>
    ///</summary>
    public class DatabaseLogSink : LogSink
    {
        /// <summary>
        /// Get/set name of database instance to log messages to
        /// </summary>
        protected string DbInstanceName { get; set; }

        /// <summary>
        /// Initialize the log sink with its configuration
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(ConfigurationBase config)
        {
            ArgumentValidation.CheckExpectedType(config, typeof(DatabaseSinkData));

            DbInstanceName = ((DatabaseSinkData)config).DbInstanceName;
        }

        /// <summary>
        /// "Send" the message to the database.
        /// </summary>
        /// <param name="entry"></param>
        protected override void SendMessageCore(LogEntry entry)
        {
            try
            {
                WriteMessageToDatabase(entry);
            }
            catch (Exception ex)
            {
                entry.AddErrorMessage("Log sink failure: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Write the log message to the database
        /// </summary>
        /// <param name="entry"></param>
        private void WriteMessageToDatabase(LogEntry entry)
        {
            try
            {
                //Create database and command wrapper
                Database db = DatabaseFactory.CreateDatabase(DbInstanceName);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ExceptionLog_InsertEntry");

                //Add parameters
                command.AddInParameter("AppDomainName", DbType.String, entry.AppDomainName ?? string.Empty);
                command.AddInParameter("ApplicationContext", DbType.String, entry.ApplicationContext ?? string.Empty);
                command.AddInParameter("Category", DbType.String, entry.Category ?? string.Empty);
                command.AddInParameter("ErrorMessages", DbType.String, entry.ErrorMessages ?? string.Empty);
                command.AddInParameter("EventId", DbType.Int32, entry.EventId);
                command.AddInParameter("MachineName", DbType.String, entry.MachineName ?? string.Empty);
                command.AddInParameter("ManagedThreadName", DbType.String, entry.ManagedThreadName ?? string.Empty);
                command.AddInParameter("Message", DbType.String, entry.Message ?? string.Empty);
                command.AddInParameter("Priority", DbType.Int32, entry.Priority);
                command.AddInParameter("ProcessId", DbType.String, entry.ProcessId ?? string.Empty);
                command.AddInParameter("ProcessName", DbType.String, entry.ProcessName ?? string.Empty);
                command.AddInParameter("Severity", DbType.String, entry.Severity.ToString());
                command.AddInParameter("TimeStamp", DbType.DateTime, entry.TimeStamp);
                command.AddInParameter("Title", DbType.String, entry.Title ?? string.Empty);
                command.AddInParameter("ThreadId", DbType.String, entry.Win32ThreadId ?? string.Empty);

                //Execute command
                db.ExecuteNonQuery(command);
            }
            catch
            {
            }
        }
    }
}
