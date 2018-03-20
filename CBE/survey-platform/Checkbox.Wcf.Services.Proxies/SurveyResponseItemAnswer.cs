using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    [DataContract]
    [Serializable]
    public class SurveyResponseItemAnswer
    {
        [DataMember]
        public Int64 AnswerId { get; set; }

        [DataMember]
        public Int32? OptionId { get; set; }

        [DataMember]
        public string AnswerText { get; set; }

        [DataMember]
        public double? Points { get; set; }

        [DataMember]
        public string Alias { get; set; }
    }
}
