using System;
using System.Globalization;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class VersionLimit : LicenseLimit
    {
        protected decimal? _versionLimitValue;

        protected VersionLimit()
        {
        }

        protected VersionLimit(string limitValue)
        {
            _versionLimitValue = Convert.ToDecimal(limitValue, new CultureInfo("en-US"));
        }

        public override string LimitValue
        {
            get { return _versionLimitValue.HasValue ? _versionLimitValue.Value.ToString(new CultureInfo("en-US")) : string.Empty; }
        }

        public override void Initialize(CheckboxLicenseData license)
        {
            //Attempt to load the limit value from the license
            if (license != null && license.GetValue(LimitName) != null)
            {
                try
                {
                    _versionLimitValue = Convert.ToDecimal(license.GetValue(LimitName), new CultureInfo("en-US"));
                }
                catch
                {
                    _versionLimitValue = null;
                }
            }
        }
    }
}
