using System;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class DetailsItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Get appearance code for item
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_DETAILS"; }
        }

        //TODO: Update, if necessary
        ///// <summary>
        ///// Get/set the graph type
        ///// </summary>
        //public override GraphType GraphType
        //{
        //    get{ return GraphType.SummaryTable;}
        //    set{}
        //}
    }
}
