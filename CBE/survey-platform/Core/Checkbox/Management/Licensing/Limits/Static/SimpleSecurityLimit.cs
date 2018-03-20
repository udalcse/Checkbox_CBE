using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not the system should use simple security.
    /// </summary>
    public class SimpleSecurityLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public SimpleSecurityLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public SimpleSecurityLimit(String limitValue) : base(limitValue) { }

        public override string LimitName
        {
            get { return "UseSimpleSecurity"; }
        }

        /// <summary>
        /// Override this property. If there is no information in the DB - don't use simple security.
        /// </summary>
        protected override bool ValidIfLimitNull
        {
            get { return false; }
        }



        public override bool? LicenseFileLimitValue
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get the currently in-effect limit value.  This is useful is certain
        /// limits need to override the license-specified limit such as when limit
        /// value should be stored in the database, etc.
        /// </summary>
        public override bool? RuntimeLimitValue
        {
            get
            {
                //Take the limit value from the database
                bool limit = GetLimitValueFromMasterDb<bool>(LimitName);

                return limit;
            }

        }
    }
}
