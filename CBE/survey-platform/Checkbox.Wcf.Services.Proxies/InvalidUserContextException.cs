using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Exception to indicate invalid user context.  Possible causes of this exception are an invalid context 
    /// guid string provided to a web service method or that the user session expired due to inactive session
    /// timeout on the web server.
    /// </summary>
    [Serializable]
    public class InvalidUserContextException : Exception
    {
        /// <summary>
        /// Exception to indicate invalid user context.
        /// </summary>
        public InvalidUserContextException()
            : base("The provided user context was not valid.")
        {
        }
    }
}
