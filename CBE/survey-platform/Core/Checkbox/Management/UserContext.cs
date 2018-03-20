using System;

namespace Checkbox.Management
{
    /// <summary>
    /// Container for storing context associated with a user login
    /// </summary>
    [Serializable]
    public class UserContext
    {
        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="userHostName">Users host name.</param>
		/// <param name="userHostIp">Users host IP.</param>
		/// <param name="userAgent">User Agent.</param>
        public UserContext(string userHostName, string userHostIp, string userAgent)
		{
			UserHostName = userHostName;
			UserHostIp = userHostIp;
			UserAgent = userAgent;
			LoginTime = DateTime.Now.ToString("s");
			CurrentUrl = string.Empty;
		}

        /// <summary>
        /// Get the user's host name.
        /// </summary>
        public string UserHostName { get; private set; }

        /// <summary>
        /// Get the user's host IP
        /// </summary>
        public string UserHostIp { get; private set; }

        /// <summary>
        /// Get the users's browser agent.
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        /// Get the time the user logged-in
        /// </summary>
        public string LoginTime { get; private set; }

        /// <summary>
        /// Get/Set the user's current URL
        /// </summary>
        public string CurrentUrl { get; set; }
    }
}
