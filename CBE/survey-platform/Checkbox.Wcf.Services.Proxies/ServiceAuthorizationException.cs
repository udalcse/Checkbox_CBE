using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Exception for web service authorization issues usually arising when the supplied user context has 
    /// insufficient permission to access web services or secured resources used by web services.
    /// </summary>
    [Serializable]
    public class ServiceAuthorizationException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public ServiceAuthorizationException()
            : base("The provided user context does not have the necessary authorization for the requested operation.")
        {
        }
    }
}
