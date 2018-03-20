using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Generic container for data necessary for survey item renderers to operate.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyResponseItem : ItemProxyObject
    {
        [DataMember]
        public int QuestionNumber { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public bool IsAnswerable { get; set; }

        [DataMember]
        public SurveyResponseItemOption[] Options { get; set; }

        [DataMember]
        public SurveyResponseItemAnswer[] Answers { get; set; }

        [DataMember]
        public bool Excluded { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public bool IsSPCArgument { get; set; }

        [DataMember]
        public string[] ValidationErrors { get; set; }

        [DataMember]
        public bool AnswerRequired { get; set; }

        [DataMember]
        public string LanguageCode { get; set; }

        [DataMember]
        public int? Width { get; set; }
    }
}
