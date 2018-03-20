using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for numbers of the page and items.
    /// </summary>
    [DataContract]
    [Serializable]
    public class PageNumberInfo
    {
        [DataMember]
        public int CurrentPageNumber { get; set; }

        [DataMember]
        public int TotalPageCount { get; set; }

        [DataMember]
        public int FirstItemNumber { get; set; }
    }
}
