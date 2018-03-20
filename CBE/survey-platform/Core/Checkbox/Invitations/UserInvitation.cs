using System.Collections.Generic;

namespace Checkbox.Invitations
{
    /// <summary>
    /// UserInvitation
    /// </summary>
    public class UserInvitation
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public List<string> Emails { get; set; }

        /// <summary>
        /// Url.
        /// </summary>
        /// <value>
        /// Url.
        /// </value>
        public string Url { get; set; }
    }
}
