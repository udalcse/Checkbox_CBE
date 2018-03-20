using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Analytics.Computation;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScoreItemResult : ItemResult
    {
        /// <summary>
        /// Get/set total answers value for the result
        /// </summary>
        public int TotalAnswers { get; set; }

        /// <summary>
        /// Get/set detractors value for the result
        /// </summary>
        public int Detractors { get; set; }

        /// <summary>
        /// Get/set passive value for the result
        /// </summary>
        public int Passive { get; set; }

        /// <summary>
        /// Get/set promoters value for the result
        /// </summary>
        public int Promoters { get; set; }

        /// <summary>
        /// Get/set net prmoter score value for the result
        /// </summary>
        public double NetPromoterScore { get; set; }
    }
}
