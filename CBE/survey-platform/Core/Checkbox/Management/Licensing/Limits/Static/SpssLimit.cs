namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not the native Spss export is enabled.
    /// </summary>
    public class SpssLimit : StaticLicenseLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public SpssLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public SpssLimit(string limitValue) : base(limitValue) { }

        public override string LimitName
        {
            get { return "AllowNativeSpssExport"; }
        }
    }
}
