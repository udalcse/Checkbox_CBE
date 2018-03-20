using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Representation of item option for item used as source for analysis item result
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportItemSourceOptionData
    {
        [DataMember]
        public int OptionId { get; set; }

        [DataMember]
        public string ReportingText { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public bool IsOther { get; set; }

        [DataMember]
        public double Points { get; set; }

        [DataMember]
        public int Position { get; set; }
    }
}
