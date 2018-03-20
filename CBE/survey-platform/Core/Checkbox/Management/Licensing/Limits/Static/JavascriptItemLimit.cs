namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// 
    /// </summary>
    public class JavascriptItemLimit : StaticLicenseLimit
    {
        /// <summary>
        /// 
        /// </summary>
        public JavascriptItemLimit() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public JavascriptItemLimit(string limitValue) : base(limitValue) { }

        /// <summary>
        /// 
        /// </summary>
        public override string LimitName
        {
            get { return "AllowJavascriptItem"; }
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
