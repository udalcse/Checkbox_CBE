namespace Checkbox.Management.Licensing.Limits.Static
{
    /// <summary>
    /// Controls whether or not the invitation manager is enabled.
    /// </summary>
    public class InvitationLimit : StaticLicenseLimit
    {
		/// <summary>
		/// 
		/// </summary>
		public InvitationLimit() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="limitValue"></param>
		public InvitationLimit(string limitValue):base(limitValue) { }

        public override string LimitName
        {
            get { return "AllowInvitations"; }
        }
    }
}
