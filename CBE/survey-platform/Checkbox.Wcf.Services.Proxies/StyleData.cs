using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for style template data.
    /// </summary>
    [DataContract]
    public class StyleData
    {
        [DataMember]
        public string HeaderHtml { get; set; }

        [DataMember]
        public string FooterHtml { get; set; }

        [DataMember]
        public Dictionary<string, Dictionary<string, string>> Css { get; set; }
    }
}