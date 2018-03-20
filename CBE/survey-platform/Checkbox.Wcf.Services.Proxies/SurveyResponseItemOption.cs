using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyResponseItemOption
    {
        [DataMember]
        public int OptionId { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public double Points { get; set; }

        [DataMember]
        public bool IsOther { get; set; }

        [DataMember]
        public bool IsNoneOfAbove { get; set; }

        [DataMember]
        public bool IsSelected { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public int? ContentId { get; set; }

    }
}
