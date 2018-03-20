using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for status of survey page post.
    /// </summary>
    [DataContract]
    [Serializable]
    public class PagePostResult
    {
        /// <summary>
        /// New response session state after post.
        /// </summary>
        [DataMember]
        public ResponseSessionState NewSessionState { get; private set; }

        /// <summary>
        /// ID of page
        /// </summary>
        [DataMember]
        public int PageId { get; private set; }

        /// <summary>
        /// Validation errors for page.
        /// </summary>
        [DataMember]
        public string[] ValidationErrors { get; private set; }

        /// <summary>
        /// Get/set whether page is valid or not.
        /// </summary>
        [DataMember]
        public bool IsValid { get; private set; }

        /// <summary>
        /// Has same page items' conditions
        /// </summary>
        [DataMember]
        public bool HasSPC { get; private set; }

        /// <summary>
        /// Constructor required for serialization.  Should not be used by client code.
        /// </summary>
        public PagePostResult()
        {
        }

        /// <summary>
        /// Construct a page post result.
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="isValid"></param>
        /// <param name="validationErrors"></param>
        /// <param name="hasSPC"></param>
        /// <param name="newSessionState"></param>
        public PagePostResult(int pageId, bool isValid, string[] validationErrors, bool hasSPC, ResponseSessionState newSessionState)
        {
            PageId = pageId;
            ValidationErrors = validationErrors;
            IsValid = isValid;
            HasSPC = hasSPC;
            NewSessionState = newSessionState;
        }
    }
}
