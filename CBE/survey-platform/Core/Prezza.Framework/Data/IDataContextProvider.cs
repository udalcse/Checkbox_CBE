namespace Prezza.Framework.Data
{
    /// <summary>
    /// Definition of interface for object that provides contextual data for database access.
    /// </summary>
    public interface IDataContextProvider
    {
        /// <summary>
        /// Get the name of the application context to use.
        /// </summary>
        string ApplicationContext { get; }

        /// <summary>
        /// Get whether context is secure or not.  It is up to consumers of the 
        /// context provider and the context provider itself to agree upon what this means
        /// exactly.
        /// </summary>
        bool Secured { get; }
    }
}
