using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for text data
    /// </summary>
    [DataContract]
    public class TextData
    {
        /// <summary>
        /// Language code for text data
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Name value collection of TextIds / Text Values
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection TextValues { get; set; }
    }
}
