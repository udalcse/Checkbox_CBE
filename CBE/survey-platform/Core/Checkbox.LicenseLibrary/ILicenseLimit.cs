using System;
using System.Collections.Generic;

namespace Checkbox.LicenseLibrary
{
    /// <summary>
    /// Limit on number of survey editor users
    /// </summary>
    public interface ILicenseLimit
    {
        /// <summary>
        /// Initialize the limit.
        /// </summary>
        /// <param name="license">License to initialize the limit with.</param>
        void Initialize(CheckboxLicenseData license);

        /// <summary>
        /// Get the limit name
        /// </summary>
        string LimitName { get; }

        /// <summary>
        /// Get the limit type name
        /// </summary>
        string LimitTypeName { get; }

        /// <summary>
        /// Get the limit value as string
        /// </summary>
        string LimitValue { get; }

        /// <summary>
        /// Validate the limit and set a string description of the error.
        /// </summary>
        /// <param name="messageTextId">TextID of the error message to display.</param>
        /// <returns>Boolean indicating if the limit is valid.</returns>
        LimitValidationResult Validate(out string messageTextId);
    }
}
