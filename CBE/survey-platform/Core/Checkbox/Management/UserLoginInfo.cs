using System;

namespace Checkbox.Management
{
	/// <summary>
	/// Simple container class for information about a currently logged-in user.
	/// </summary>
    [Serializable]
	public class UserLoginInfo : UserContext
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="userName">User name of the logged-in user.</param>
		/// <param name="userHostName">Users host name.</param>
		/// <param name="userHostIp">Users host IP.</param>
		/// <param name="userAgent">User Agent.</param>
        public UserLoginInfo(string userName, string userHostName, string userHostIp, string userAgent)
            : base(userHostName, userHostIp, userAgent)
        {
            UserName = userName;
        }
	
	    /// <summary>
	    /// Get the name of the logged-in user.
	    /// </summary>
	    public string UserName { get; private set; }
	}
}
