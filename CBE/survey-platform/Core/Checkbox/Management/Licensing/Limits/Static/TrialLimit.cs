namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// 
    /// </summary>
    public class TrialLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public TrialLimit(){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public TrialLimit(string limitValue) : base(limitValue)
        {
        }
    }
}
