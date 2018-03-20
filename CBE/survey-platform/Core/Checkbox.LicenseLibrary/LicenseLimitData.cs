using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//using Xheo.Licensing;

namespace Checkbox.LicenseLibrary
{
    /// <summary>
    /// Data object that stores limit data.
    /// </summary>
    public class LicenseLimitData : ILicenseLimit
    {
        /// <summary>
        /// Initialize the limit.
        /// </summary>
        /// <param name="license">License to initialize the limit with.</param>
        public void Initialize(CheckboxLicenseData license)
        {
        }
        
        /// <summary>
        /// Get the limit name
        /// </summary>
        public string LimitName { get; set; }

        /// <summary>
        /// Get the limit type name
        /// </summary>
        public virtual string LimitTypeName { get { return LimitName; } }

		/// <summary>
		/// Get the limit value as string
		/// </summary>
        public string LimitValue { get; set; }

        /// <summary>
        /// Validate the limit and set a string description of the error.
        /// </summary>
        /// <param name="messageTextId">TextID of the error message to display.</param>
        /// <returns>Boolean indicating if the limit is valid.</returns>
        public LimitValidationResult Validate(out string messageTextId)
        {
            messageTextId = "";
            return LimitValidationResult.LimitNotReached;
        }
    }
}
