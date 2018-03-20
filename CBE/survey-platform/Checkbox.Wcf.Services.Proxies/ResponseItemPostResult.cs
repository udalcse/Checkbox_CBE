using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for status of item posted to response.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ResponseItemPostResult
    {
        /// <summary>
        /// ID of item
        /// </summary>
        [DataMember]
        public int ItemId { get; private set; }

        /// <summary>
        /// Any validation error messages
        /// </summary>
        [DataMember]
        public string[] ValidationErrors { get; private set; }

        /// <summary>
        /// Indicator of whether posted data is valid or not
        /// </summary>
        [DataMember]
        public bool IsValid { get; private set; }

        /// <summary>
        /// Constructor required for serialization.  Should not be used by client
        /// code.
        /// </summary>
        public ResponseItemPostResult()
        {
        }

        /// <summary>
        /// Construct item result
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="isValid"></param>
        /// <param name="validationErrors"></param>
        public ResponseItemPostResult(int itemId, bool isValid, string[] validationErrors)
        {
            ItemId = itemId;
            IsValid = isValid;
            ValidationErrors = validationErrors;
        }
    }
}
