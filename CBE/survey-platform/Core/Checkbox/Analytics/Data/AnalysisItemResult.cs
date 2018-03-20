using System;
using System.Collections.Generic;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Generated result data for a given analysis item.
    /// </summary>
    [Serializable]
    public class AnalysisItemResult
    {
        private Dictionary<int, int> _itemResponseCounts;
        private Dictionary<int, int> _itemAnswerCounts;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, int> ItemResponseCounts
        {
            get { return _itemResponseCounts ?? (_itemResponseCounts = new Dictionary<int, int>()); }
            set { _itemResponseCounts = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, int> ItemAnswerCounts
        {
            get { return _itemAnswerCounts ?? (_itemAnswerCounts = new Dictionary<int, int>()); }
            set { _itemAnswerCounts = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is preview.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is preview; otherwise, <c>false</c>.
        /// </value>
        public bool IsPreview { get; set; }


        /// <summary>
        /// Answer details
        /// </summary>
        public DetailResult[] DetailResults { get; set; }

        /// <summary>
        /// Aggregate results
        /// </summary>
        public AggregateResult[] AggregateResults { get; set; }

        /// <summary>
        /// Calculate results
        /// </summary>
        public CalculateResult[] CalculateResults { get; set; }

        /// <summary>
        /// Grouped details results
        /// </summary>
        public GroupedResult<DetailResult>[] GroupedDetailResults { get; set; }

        /// <summary>
        /// Grouped aggregate results
        /// </summary>
        public GroupedResult<AggregateResult>[] GroupedAggregateResults { get; set; }

        /// <summary>
        /// Gets or sets the heat map analysis result.
        /// </summary>
        /// <value>
        /// The heat map analysis result.
        /// </value>
        public HeatMapAnalysisResult HeatMapAnalysisResult { get; set; }


        /// <summary>
        /// Gets or sets the heat map analysis result.
        /// </summary>
        /// <value>
        /// The heat map analysis result.
        /// </value>
        public GradientColorDirectorMatrixResult GradientColorDirectorMatrixResult { get; set; }
    }
}
