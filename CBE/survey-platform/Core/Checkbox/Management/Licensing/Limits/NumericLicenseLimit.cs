using System;
using Checkbox.LicenseLibrary;

//using Xheo.Licensing;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Simple class to validate numeric licensing limits in the application
    /// </summary>
    public abstract class NumericLicenseLimit : ValueLimit<long?>
    {
        private long? _licenseFileLimitValue;

		/// <summary>
		/// 
		/// </summary>
		public NumericLicenseLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public NumericLicenseLimit(string limitValue)
		{
			_licenseFileLimitValue = long.Parse(limitValue);
		}

        
        /// <summary>
        /// Get the value of the limit as stored in the license file.
        /// </summary>
        public override long? LicenseFileLimitValue 
        {
            get
            {
                if (ApplicationManager.AppSettings.LimitDebugMode)
                {
                    return 1;
                }

                return _licenseFileLimitValue;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public override string LimitValue
		{
			get 
			{
				long limit = _licenseFileLimitValue ?? 1;

				return limit.ToString();
			}
		}


        /// <summary>
        /// Get the current count of limited entities
        /// </summary>
        public long CurrentCount 
        {
            get { return GetCurrentCount(); }
        }

        /// <summary>
        /// Overridable method to get the current count of limited entities.
        /// </summary>
        /// <returns></returns>
        protected abstract long GetCurrentCount();

        /// <summary>
        /// Initialize the limit from the license
        /// </summary>
        /// <param name="license"></param>
        public override void Initialize(CheckboxLicenseData license)
        {
            //Attempt to load the limit value from the license
            if (license != null && license.GetValue(LimitName) != null)
            {
                try
                {
                    _licenseFileLimitValue = Convert.ToInt64(license.GetValue(LimitName));
                }
                catch
                {
                    _licenseFileLimitValue = null;
                }
            }
        }
    }
}
