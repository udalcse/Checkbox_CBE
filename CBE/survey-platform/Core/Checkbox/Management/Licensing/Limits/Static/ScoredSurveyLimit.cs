using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether "Scored Survey" is available when editing survey properties.
    /// </summary>
    public class ScoredSurveyLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public ScoredSurveyLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public ScoredSurveyLimit(String limitValue) : base(limitValue) { }

        /// <summary>
        /// 
        /// </summary>
        public override string LimitName
        {
            get { return "AllowScoredSurvey"; }
        }
    }
}
