using System;

namespace Checkbox.Forms
{
    /// <summary>
    /// Survey security type enumeration.
    /// </summary>
	[Serializable]
	public enum SecurityType
	{
		/// <summary>
		/// Allows anyone who accesses the survey Url to complete the survey.
		/// </summary>
		Public = 1,

		/// <summary>
		/// Allows anyone who accesses the survey Url and has the global survey password 
		/// to complete the survey.
		/// </summary>
		PasswordProtected,

		/// <summary>
		/// Allows only registered users who are specifically added to a survey's access control 
		/// list to access and complete the survey
		/// </summary>
		AccessControlList,

		/// <summary>
		/// Allows all registered users on the system to access and complete the survey 
		/// </summary>
		AllRegisteredUsers,

        /// <summary>
        /// Only users with a valid invitation can take the survey
        /// </summary>
        InvitationOnly
	}	
}
