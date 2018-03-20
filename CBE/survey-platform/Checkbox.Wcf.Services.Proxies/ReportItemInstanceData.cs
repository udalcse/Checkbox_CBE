using System;
using System.Runtime.Serialization;
using Checkbox.Analytics.Data;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Generic data container for a survey item data.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportItemInstanceData : ItemProxyObject
    {
        [DataMember]
        public int SourceResponseTemplateId { get; set; }

        [DataMember]
        public ReportItemSourceItemData[] SourceItems { get; set; }

        [DataMember]
        public ReportItemSourcePageData[] SourcePages { get; set; }

        [DataMember]
        public bool UseAliases { get; set; }

        [DataMember]
        public bool IsPreview { get; set; }

        [DataMember]
        public AggregateResult[] AggregateResults { get; set; }

        [DataMember]
        public CalculateResult[] CalculateResult { get; set; }

        [DataMember]
        public DetailResult[] DetailResults { get; set; }

        [DataMember]
        public GradientColorDirectorMatrixResult GradientColorDirectorMatrixResult { get; set; }

        [DataMember]
        public HeatMapAnalysisResult HeatMapAnalysisResult { get; set; }

        [DataMember]
        public GroupedResult<DetailResult>[] GroupedDetailResults { get; set; }

        [DataMember]
        public GroupedResult<AggregateResult>[] GroupedAggregateResults { get; set; }

        [DataMember]
        public string[] AppliedFilterTexts { get; set; }
    }
}
