namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// 
    /// </summary>
    public class AutocompleteRemoteSourceLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public AutocompleteRemoteSourceLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public AutocompleteRemoteSourceLimit(string limitValue) : base(limitValue) { }

        /// <summary>
        /// 
        /// </summary>
        public override string LimitName
        {
            get { return "AllowAutocompleteRemoteSource"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string ValueColumnName
        {
            get { return "ContextLimitValue"; }
        }
    }
}
