using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// GradientColorDirectorMatrixResult
    /// </summary>
    /// <seealso cref="Checkbox.Wcf.Services.Proxies.ReportResult" />
    [DataContract]
    [Serializable]
    public class GradientColorDirectorMatrixResult : ReportResult
    {
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public List<GradientColorMatrixOption> Options { get; set; }

        /// <summary>
        /// Gets or sets the respondents.
        /// </summary>
        /// <value>
        /// The respondents.
        /// </value>
        public List<GradientColorMatrixRespondent> Respondents { get; set; }

        /// <summary>
        /// Gets or sets the OptionAverage.
        /// </summary>
        /// <value>
        /// The OptionAverage.
        /// </value>
        public double OptionAverage { get; set; }

        /// <summary>
        /// Gets or sets the maximum scale rating range.
        /// </summary>
        /// <value>
        /// The maximum scale rating range.
        /// </value>
        public double MaxScaleRatingRange { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientColorDirectorMatrixResult"/> class.
        /// </summary>
        public GradientColorDirectorMatrixResult()
        {
            this.Options = new List<GradientColorMatrixOption>();
            this.Respondents = new List<GradientColorMatrixRespondent>();
        }
    }

    /// <summary>   
    /// GradientColorMatrixOption
    /// </summary>
    public class GradientColorMatrixOption
    {
        /// <summary>
        /// Gets or sets the option title.
        /// </summary>
        /// <value>
        /// The option title.
        /// </value>
        public string OptionTitle { get; set; }

        /// <summary>
        /// Gets or sets the avarage score.
        /// </summary>
        /// <value>
        /// The avarage score.
        /// </value>
        public double AverageScore { get; set; }
    }

    /// <summary>
    /// GradientColorMatrixRespondent
    /// </summary>
    public class GradientColorMatrixRespondent
    {
        /// <summary>
        /// Gets or sets the name of the respondent.
        /// </summary>
        /// <value>
        /// The name of the respondent.
        /// </value>
        public string RespondentName { get; set; }

        /// <summary>
        /// Gets or sets the response identifier.
        /// </summary>
        /// <value>
        /// The response identifier.
        /// </value>
        public int ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the answer score.
        /// </summary>
        /// <value>
        /// The answer score.
        /// </value>
        public List<double> AnswerScore { get; set; }

        /// <summary>
        /// Gets or sets the avarage by director.
        /// </summary>
        /// <value>
        /// The avarage by director.
        /// </value>
        public double AverageByDirector { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientColorMatrixRespondent"/> class.
        /// </summary>
        public GradientColorMatrixRespondent()
        {
            this.AnswerScore = new List<double>();
        }
    }
}
