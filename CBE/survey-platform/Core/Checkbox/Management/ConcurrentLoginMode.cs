using System;

namespace Checkbox.Management
{
    /// <summary>
    /// Support for concurrent logins by same user.
    /// </summary>
	public enum ConcurrentLoginMode
	{
        /// <summary>
        /// User can be logged-in multiple times concurrently.
        /// </summary>
		Allowed,

        /// <summary>
        /// Attempt to login as user already logged in will fail.
        /// </summary>
		NotAllowed,
        
        /// <summary>
        /// Attempt to login as user already logged in will result in first user being logged out
        /// and second user being logged in.
        /// </summary>
		LogoutCurrent
	}
}
