using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class UploadItemAdditionalData
    {
        /// <summary>
        /// Data uploaded by respondent.
        /// </summary>
        [DataMember]
        public byte[] FileBytes { get; set; }
    }
}
