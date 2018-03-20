using System;
using System.Diagnostics;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Distributor.Configuration;

namespace Prezza.Framework.Logging.Sinks
{
    ///<summary>
    ///</summary>
    public class SystemEventLogSink : LogSink
    {
        /// <summary>
        /// 
        /// </summary>
        private string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string Log { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(ConfigurationBase config)
        {
            ArgumentValidation.CheckExpectedType(config, typeof(SystemEventLogSinkData));

            Source = ((SystemEventLogSinkData)config).Source;
            Log = ((SystemEventLogSinkData) config).Log;
        }

        /// <summary>
        /// "Send" the message to the database.
        /// </summary>
        /// <param name="entry"></param>
        protected override void SendMessageCore(LogEntry entry)
        {
            try
            {
                WriteMessageToEventLog(entry, Source, Log);
            }
            catch (Exception ex)
            {
                entry.AddErrorMessage("Log sink failure: " + ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="source"></param>
        /// <param name="log"></param>
        private static void WriteMessageToEventLog(LogEntry entry, string source, string log)
        {
            try
            {
                //Create log, if necessary, and set overflow policy
                if (!EventLog.SourceExists(source))
                {
                    EventLog.CreateEventSource(source, log);

                    var eventLog = new EventLog(log);

                    eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, -1);
                }

                EventLogEntryType entryType;

                if(entry.Severity == Severity.Error)
                {
                    entryType = EventLogEntryType.Error;
                }
                else if(entry.Severity == Severity.Warning)
                {
                    entryType = EventLogEntryType.Warning;
                }
                else
                {
                    entryType = EventLogEntryType.Information;
                }

                EventLog.WriteEntry(
                    source,
                    entry.Message,
                    entryType,
                    entry.EventId);
            }
            catch
            {
            }
        }
    }
}
