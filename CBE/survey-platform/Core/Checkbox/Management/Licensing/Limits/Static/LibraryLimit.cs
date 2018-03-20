using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not MLS can be enabled.
    /// </summary>
    public class LibraryLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public LibraryLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public LibraryLimit(string limitValue) : base(limitValue) { }

        public override string LimitName
        {
            get { return "RestrictLibraries"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override LimitValidationResult Validate(out string message)
        {
            //Save a little work if the limit has no value and that indicates a valid
            //limit.
            message = string.Empty;
            
            if (RuntimeLimitValue == null)
            {
                return ValidIfLimitNull ? 
                    LimitValidationResult.LimitNotReached :
                    LimitValidationResult.LimitExceeded;
            }

            return (bool)RuntimeLimitValue ? 
                LimitValidationResult.LimitExceeded :
                LimitValidationResult.LimitNotReached;
        }
    }
}
