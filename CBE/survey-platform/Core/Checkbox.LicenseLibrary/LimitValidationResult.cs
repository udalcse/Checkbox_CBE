namespace Checkbox.LicenseLibrary
{
    /// <summary>
    /// Limit validation results
    /// </summary>
    public enum LimitValidationResult
    {
        /// <summary>
        /// The limit has not yet been reached.
        /// </summary>
        LimitNotReached = 1,

        /// <summary>
        /// The limit has been reached but not exceeded
        /// </summary>
        LimitReached,

        /// <summary>
        /// The limit has been exceeded
        /// </summary>
        LimitExceeded,

        /// <summary>
        /// The limit can't be evaluated
        /// </summary>
        UnableToEvaluate
    }
}
