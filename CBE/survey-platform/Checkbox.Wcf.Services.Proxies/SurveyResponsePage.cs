using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for survey response page.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyResponsePage
    {
        [DataMember]
        public int PageId { get; set; }

        [DataMember]
        public int Position { get; set; }

        [DataMember]
        public int? LayoutTemplateId { get; set; }

        [DataMember]
        public string[] ValidationErrors { get; set; }

        [DataMember]
        public int[] ItemIds { get; set; }

        [DataMember]
        public int[] ItemNumbers { get; set; }

        [DataMember]
        public string PageType { get; set; }

        [DataMember]
        public bool IsValid { get; set; }

        [DataMember]
        public bool HasSPC { get; set; }

        [DataMember]
        public bool IsLastContentPage { get; set; }

        [DataMember]
        public bool IsFirstContentPage { get; set; }
    }
}
