namespace Prezza.Framework.Data
{
    /// <summary>
    /// Base implementation of a data context provider
    /// </summary>
    public class DataContextProvider : IDataContextProvider
    {
        public const string APPLICATION_CONTEXT_KEY = "ApplicationContext";
        public const string REQUEST_SECURED_KEY = "RequestIsSecured";

        /// <summary>
        /// Get/set application context.
        /// </summary>
        public virtual string ApplicationContext { get; set; }

        /// <summary>
        /// Get/set whether application context is "secured"
        /// </summary>
        public virtual bool Secured { get; set; }
    }
}
