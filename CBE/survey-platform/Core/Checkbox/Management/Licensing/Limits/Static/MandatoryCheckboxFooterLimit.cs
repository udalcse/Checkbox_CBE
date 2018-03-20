using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not the "powered by Checkbox®" footer appears in all surveys 
    /// </summary>
    public class MandatoryCheckboxFooterLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public MandatoryCheckboxFooterLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public MandatoryCheckboxFooterLimit(String limitValue) : base(limitValue) { }

        /// <summary>
        /// 
        /// </summary>
        public override string LimitName
        {
            get { return "MandatoryCheckboxFooter"; }
        }

        /// <summary>
        /// Override this property. If there is no information in the DB - don't use mandatory checkbox footer
        /// </summary>
        protected override bool ValidIfLimitNull
        {
            get { return false; }
        }
    }
}
