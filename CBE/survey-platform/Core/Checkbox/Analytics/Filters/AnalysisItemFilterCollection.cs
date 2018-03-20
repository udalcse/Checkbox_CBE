using System;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Collection of filters associated with individual report items rather than entire reports.
    /// </summary>
    [Serializable]
    public class AnalysisItemFilterCollection : FilterDataCollection
    {
        /// <summary>
        /// Always returns "AnalysisItem" as the value since this type of filter collection applies only to report items.
        /// </summary>
        public override string ParentType
        {
            get { return "AnalysisItem"; }
            set { ;}
        }
    }
}
