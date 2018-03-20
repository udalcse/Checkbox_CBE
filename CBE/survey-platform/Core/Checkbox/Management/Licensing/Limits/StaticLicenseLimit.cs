using System;
using Checkbox.LicenseLibrary;
//using Xheo.Licensing;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Limit access based on a boolean on/off value.
    /// </summary>
    public abstract class StaticLicenseLimit : ValueLimit<bool?>
    {
		/// <summary>
		/// 
		/// </summary>
		public StaticLicenseLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public StaticLicenseLimit(string limitValue)
		{
			_licenseFileLimitValue = bool.Parse(limitValue);
		}

        private bool? _licenseFileLimitValue;

        /// <summary>
        /// Get the value of the limit as stored in the license file.
        /// </summary>
        public override bool? LicenseFileLimitValue
        {
            get { return _licenseFileLimitValue; }
        }

		/// <summary>
		/// 
		/// </summary>
		public override string LimitValue
		{
			get 
			{
				bool valid = _licenseFileLimitValue ?? ValidIfLimitNull;

				return valid.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="license"></param>
        public override void Initialize(CheckboxLicenseData license)
        {
            //Attempt to load the limit value from the license
            if (license != null && license.GetValue(LimitName) != null)
            {
                try
                {
                    _licenseFileLimitValue = Convert.ToBoolean(license.GetValue(LimitName));
                }
                catch
                {
                    _licenseFileLimitValue = null;
                }
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
        public override LimitValidationResult ProtectedValidate(out string message)
        {
            message = string.Empty;

            if (RuntimeLimitValue != null && (bool)RuntimeLimitValue)
            {
                return LimitValidationResult.LimitNotReached;
            }

            return LimitValidationResult.LimitExceeded;
        }
    }
}
