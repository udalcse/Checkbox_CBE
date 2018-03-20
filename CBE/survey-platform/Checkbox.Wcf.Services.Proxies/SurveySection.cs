using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// SurveySection
    /// </summary>
    [Serializable]
    public class SurveySection
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public double Score { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        /// <value>
        /// The responses.
        /// </value>
        public List<ItemAnswer> Answers { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveySection"/> class.
        /// </summary>
        public SurveySection()
        {
            this.Answers = new List<ItemAnswer>();
        }
    }
}
