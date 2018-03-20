//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Data;
using Prezza.Framework.Logging;

namespace Prezza.Framework.ExceptionHandling
{
    /// <summary>
    /// Special exception handler that logs exception information to a log sink.
    /// </summary>
    public class DatabaseLoggingExceptionHandler : BaseExceptionHandler
    {
        private static IDataContextProvider _dataContextProvider;

        /// <summary>
        /// Initialize the provider with a data context provider
        /// </summary>
        /// <param name="dataContextProvider"></param>
        public static void Initialize(IDataContextProvider dataContextProvider)
        {
            _dataContextProvider = dataContextProvider;
        }

        /// <summary>
        /// Write a message to the log.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="logMessage">Message to write.</param>
        protected override void WriteToLog(string title, string logMessage)
        {
            var entry = new LogEntry(logMessage, DefaultLogCategory, MinimumPriority, DefaultEventId, DefaultSeverity, title, null);

            if (_dataContextProvider != null)
            {
                entry.ApplicationContext = _dataContextProvider.ApplicationContext;
            }

            Logger.Write(entry);
        }
    }
}
