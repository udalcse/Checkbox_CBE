namespace Checkbox.Management.Licensing
{
    /// <summary>
    /// Placeholder for a null license, which indicates a valid checkbox license was not found. Placeholder
    /// is required so that a more useful error than the typical .NET licensing error can be shown.
    /// </summary>
    public class NullLicense : CheckboxLicense
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationError"></param>
        public NullLicense(string validationError)
        {
            ValidationError = validationError;
        }
    }
}
