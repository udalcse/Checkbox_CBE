using System;
using System.Collections.Generic;

namespace Checkbox.Analytics.Data
{
    /// <summary>
    /// Extension of analysis item result to get matrix summary specific data.
    /// </summary>
    [Serializable]
    public class MatrixSummaryAnalysisItemResult : AnalysisItemResult
    {
        private Dictionary<int, double> _sumTotalAverages;
        private Dictionary<int, double> _ratingScaleAverages;
        private Dictionary<int, double> _sliderAverages;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, double> SumTotalAverages
        {
            get { return _sumTotalAverages ?? (_sumTotalAverages = new Dictionary<int, double>()); }
            set { _sumTotalAverages = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, double> RatingScaleAverages
        {
            get { return _ratingScaleAverages ?? (_ratingScaleAverages = new Dictionary<int, double>()); }
            set { _ratingScaleAverages = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, double> SliderAverages
        {
            get { return _sliderAverages ?? (_sliderAverages = new Dictionary<int, double>()); }
            set { _sliderAverages = value; }
        }
    }
}
