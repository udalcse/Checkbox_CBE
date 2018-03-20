namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not user can authenticate against active directory.
    /// </summary>
    public class NetworkUserLimit : StaticLicenseLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public NetworkUserLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public NetworkUserLimit(string limitValue) : base(limitValue) { }
		/// <summary>
		/// 
		/// </summary>
        public override string LimitName
        {
            get { return "AllowNetworkUsers"; }
        }
    }
}
