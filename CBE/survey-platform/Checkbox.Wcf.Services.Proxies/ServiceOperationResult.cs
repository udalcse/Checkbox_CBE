using System;
using System.Runtime.Serialization;
using System.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for returning status of service operations.  Wraps all calls so that server-side
    /// errors can be reported to clients.
    /// </summary>
    /// <typeparam name="T">Type of result object for service.</typeparam>
    [Serializable]
    [DataContract]
    public class ServiceOperationResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public ServiceOperationResult()
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null)
                IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            else
                IsAuthenticated = false;
        }

        /// <summary>
        /// Indicates whether or not the service operation completed successfully.
        /// </summary>
        [DataMember]
        public bool CallSuccess { get; set; }

        /// <summary>
        /// Message indicating reason for failure
        /// </summary>
        [DataMember]
        public string FailureMessage { get; set; }

        /// <summary>
        /// Data associated with result.  Not all operations will have associated data, so this
        /// value may be null.
        /// </summary>
        [DataMember]
        public T ResultData { get; set; }

        /// <summary>
        /// Get type of exception, if any that resulted in the failure.
        /// </summary>
        [DataMember]
        public string FailureExceptionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsAuthenticated { set; get; }
    }
}
