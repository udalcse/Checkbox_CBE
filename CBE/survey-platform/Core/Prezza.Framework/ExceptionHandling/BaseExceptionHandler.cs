using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling.Configuration;
using Prezza.Framework.Logging;

namespace Prezza.Framework.ExceptionHandling
{
    public class BaseExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// Get/Set the default category for entries written by the handler.
        /// </summary>
        protected string DefaultLogCategory { get; set; }

        /// <summary>
        /// Get/Set the default event Id for entries written by the handler.
        /// </summary>
        protected int DefaultEventId { get; set; }

        /// <summary>
        /// Get/Set the default severity for entries written by the handler.
        /// </summary>
        protected Severity DefaultSeverity { get; set; }

        /// <summary>
        /// Get/Set the default title for entries written by the handler.
        /// </summary>
        protected string DefaultTitle { get; set; }

        /// <summary>
        /// Get/Set the TypeName of the log formatter to use when writing entries.
        /// </summary>
        protected string FormatterTypeName { get; set; }

        /// <summary>
        /// The minimum priority to set when writing entries.
        /// </summary>
        protected int MinimumPriority { get; set; }

        /// <summary>
        ///  ConstructorInfo used to instantiate the formatter.
        /// </summary>
        private ConstructorInfo _constructor;

        /// <summary>
        /// Initialize the exception handler with its configuration information.
        /// </summary>
        /// <param name="config">Configuration information for the logging exception handler.</param>
        public override void Initialize(ConfigurationBase config)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckExpectedType(config, typeof(ExceptionHandlerData));

            DefaultLogCategory = ((ExceptionHandlerData)config).DefaultLogCategory;
            DefaultEventId = ((ExceptionHandlerData)config).DefaultEventId;
            DefaultSeverity = ((ExceptionHandlerData)config).DefaultSeverity;
            DefaultTitle = ((ExceptionHandlerData)config).DefaultTitle;
            MinimumPriority = ((ExceptionHandlerData)config).MinimumPriority;
            FormatterTypeName = ((ExceptionHandlerData)config).FormatterTypeName;
        }

        /// <summary>
        /// Handle the specified exception.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        /// <param name="policyName">Exception policy used.</param>
        /// <param name="handlingInstanceId">Exception instance Id.</param>
        /// <returns></returns>
        public override Exception HandleException(Exception exception, string policyName, Guid handlingInstanceId)
        {
            EnsureEnvironment();
            WriteToLog(exception.Message, CreateMessage(exception, handlingInstanceId));
            return exception;
        }

        /// <summary>
        /// Setup the the configuration and items for the exception handler.
        /// </summary>
        private void EnsureEnvironment()
        {
            if (_constructor == null)
            {
                Type formatterType = Type.GetType(FormatterTypeName, true);
                var types = new[] { typeof(TextWriter), typeof(Exception), typeof(string) };
                _constructor = formatterType.GetConstructor(types);

                if (_constructor == null)
                {
                    throw new ExceptionHandlingException("Unable to get _constructor for: " + formatterType.AssemblyQualifiedName);
                }
            }
        }

        /// <summary>
        /// Create the message to log for the specified exception.
        /// </summary>
        /// <param name="exception">Exception to generate the message for.</param>
        /// <param name="handlingInstanceId">Instace Id of the exception.</param>
        /// <returns></returns>
        private string CreateMessage(Exception exception, Guid handlingInstanceId)
        {
            StringWriter writer = null;
            StringBuilder stringBuilder;

            try
            {
                writer = CreateStringWriter();
                ExceptionFormatter formatter = CreateFormatter(writer, exception, handlingInstanceId.ToString());
                formatter.Format();
                stringBuilder = writer.GetStringBuilder();
            }
            finally
            {
                if (writer != null)
                {
                    try
                    {
                        writer.Close();
                    }
                    catch
                    {
                    }
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Create the message formatter.
        /// </summary>
        /// <param name="writer">StringWriter for the formatter to use.</param>
        /// <param name="exception">Exceptin to format the message for.</param>
        /// <param name="handlingInstanceIdString">Exception instance Id in string format.</param>
        /// <returns></returns>
        protected virtual ExceptionFormatter CreateFormatter(StringWriter writer, Exception exception, string handlingInstanceIdString)
        {
            return (ExceptionFormatter)_constructor.Invoke(new object[] { writer, exception, handlingInstanceIdString });
        }

        /// <summary>
        /// Create a string writer to use to format the exception message.
        /// </summary>
        /// <returns></returns>
        protected virtual StringWriter CreateStringWriter()
        {
            return new StringWriter(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Write a message to the log.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="logMessage">Message to write.</param>
        protected virtual void WriteToLog(string title, string logMessage)
        {
            Logger.Write(
                new LogEntry(
                    logMessage, 
                    DefaultLogCategory, 
                    MinimumPriority, 
                    DefaultEventId, 
                    DefaultSeverity, 
                    title, 
                    null)
            );
        }
    }
}
