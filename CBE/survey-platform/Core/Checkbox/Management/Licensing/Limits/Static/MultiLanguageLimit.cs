namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not MLS can be enabled.
    /// </summary>
    public class MultiLanguageLimit : StaticLicenseLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public MultiLanguageLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public MultiLanguageLimit(string limitValue) : base(limitValue) { }

        public override string LimitName
        {
            get { return "AllowMultiLanguage"; }
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
