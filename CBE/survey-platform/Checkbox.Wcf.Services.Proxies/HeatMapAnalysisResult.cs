using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Aggreagation result for heat map graph
    /// </summary>
    /// <seealso cref="Checkbox.Analytics.Data.AnalysisItemResult" />
    [DataContract]
    [Serializable]
    public class HeatMapAnalysisResult : ReportResult
    {
        /// <summary>
        /// Gets or sets the responses of respondents.
        /// </summary>
        /// <value>
        /// The responses.
        /// </value>
        public List<HeatMapResult> Responses { get; set; }

        /// <summary>
        /// Gets or sets the sections mean values.
        /// </summary>
        /// <value>
        /// The sections mean values.
        /// </value>
        public Dictionary<string, double> SectionsMeanValues { get; set; }

        /// <summary>
        /// Max value of raiting scale range
        /// </summary>
        /// <value>
        /// The maximum raiting range.
        /// </value>
        public int MaxRatingRange { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapAnalysisResult"/> class.
        /// </summary>
        public HeatMapAnalysisResult()
        {
            this.Responses = new List<HeatMapResult>();
            this.SectionsMeanValues = new Dictionary<string, double>();
        }
    }

    /// <summary>
    /// Represents one respondent response for heat map chart
    /// </summary>
    public class HeatMapResult
    {
        /// <summary>
        /// Gets or sets the response identifier.
        /// </summary>
        /// <value>
        /// The response identifier.
        /// </value>
        public int ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the respondent name.
        /// </summary>
        /// <value>
        /// The respondent.
        /// </value>
        public string Respondent { get; set; }

        /// <summary>
        /// Gets or sets the sections related to one response of the particular respondent.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public List<SurveySection> Sections { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatMapResult"/> class.
        /// </summary>
        public HeatMapResult ()
        {
            this.Sections = new List<SurveySection>();
        }
    }
}
