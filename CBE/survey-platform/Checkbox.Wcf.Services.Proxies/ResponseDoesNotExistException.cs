using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Indicates that an attempt to retrieve specified response.
    /// The issue occurs when the response does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class ResponseDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public ResponseDoesNotExistException(long responseId)
            : base(string.Format("Unable to locate any responses with id {0}. Either the response does not exist or you do not have sufficient permissions to manage the response.", responseId))
        {
        }
    }
}
