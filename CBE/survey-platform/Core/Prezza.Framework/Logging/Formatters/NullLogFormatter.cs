using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Formatters
{
    /// <summary>
    /// Log formatter that does nothing.  Required for logging situations (such as database) where a formatter is not necessary.
    /// </summary>
    public class NullLogFormatter : ConfigurationProvider, ILogFormatter
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(ConfigurationBase config)
        {
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public string Format(LogEntry log)
        {
            return string.Empty;
        }
    }
}
