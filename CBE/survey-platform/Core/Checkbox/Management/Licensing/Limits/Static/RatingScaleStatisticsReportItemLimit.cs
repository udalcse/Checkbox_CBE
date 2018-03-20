using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether ratingScaleStatisticsReportItem can be added to report
    /// </summary>
    public class RatingScaleStatisticsReportItemLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public RatingScaleStatisticsReportItemLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public RatingScaleStatisticsReportItemLimit(String limitValue) : base(limitValue) { }

        /// <summary>
        /// 
        /// </summary>
        public override string LimitName
        {
            get { return "AllowRatingScaleStatisticsReportItem"; }
        }
    }
}
