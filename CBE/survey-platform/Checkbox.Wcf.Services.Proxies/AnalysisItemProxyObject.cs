using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Generic data container for a survey item data.
    /// </summary>
    [DataContract]
    [Serializable]
    public class AnalysisItemProxyObject : ItemProxyObject
    {
        [DataMember]
        public string TitleTextWithResponseCounts { get; set; }

        [DataMember]
        public string TitleTextWithoutResponseCounts { get; set; }

        [DataMember]
        public bool Excluded { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public int SourceResponseTemplateId { get; set; }

        [DataMember]
        public int[] ResponseTemplateIds { get; set; }

        [DataMember]
        public int[] SourceItemIds { get; set; }

        [DataMember]
        public int[][] SourceItemOptions { get; set; }

        [DataMember]
        public SimpleNameValueCollection SourceItemTexts { get; set; }

        [DataMember]
        public SimpleNameValueCollection SourceOptionTexts { get; set; }

        [DataMember]
        public bool UseAliases { get; set; }

        [DataMember]
        public AggregateResult[] AggregateResults { get; set; }

        [DataMember]
        public DetailResult[] DetailResults { get; set; }

        [DataMember]
        public GroupedResult[] GroupedResults { get; set; }
    }
}
