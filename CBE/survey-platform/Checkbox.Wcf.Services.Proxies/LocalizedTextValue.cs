using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Localized text value used for two-way fetching/updating between server and
    /// client code.
    /// </summary>
    [DataContract]
    public class LocalizedTextValue
    {
        /// <summary>
        /// Language code for text value
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Identifier for text value
        /// </summary>
        [DataMember]
        public string TextId { get; set; }

        /// <summary>
        /// Actual text value
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
