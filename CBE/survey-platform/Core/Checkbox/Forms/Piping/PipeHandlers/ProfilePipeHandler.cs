using System.Security.Principal;
using Checkbox.Common;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Forms.Piping.Tokens;

namespace Checkbox.Forms.Piping.PipeHandlers
{
    /// <summary>
    /// Pipe handler for profile properties.
    /// </summary>
    public class ProfilePipeHandler : PipeHandler
    {
        /// <summary>
        /// Get value of profile pipe token.
        /// </summary>
        /// <param name="token">Token to get value of.</param>
        /// <param name="context">User principal context.</param>
        /// <returns>String value of token.</returns>
        public override string GetTokenValue(Token token, object context)
        {
            string profileName = token.CleanName;

            //Make sure token is a profile token and context is not null.
            if (!(token is ProfileToken)
                || context == null)
            {
                return string.Empty;
            }

            CheckboxPrincipal principal = null;

            if (context is CheckboxPrincipal)
            {
                principal = (CheckboxPrincipal)context;
            }

            if (context is IIdentity)
            {
                principal = UserManager.GetUserPrincipal((IIdentity)context, true);
            }

            return GetPipeValue(principal, profileName);
        }

        /// <summary>
        /// Get the value of a profile pipe.
        /// </summary>
        /// <param name="userPrincipal">User principal to get profile pipe for.</param>
        /// <param name="cleanPipeName">Pipe name stripped of any delimiting characters.</param>
        /// <returns></returns>
        public static string GetPipeValue(CheckboxPrincipal userPrincipal, string cleanPipeName)
        {
            //If no principal found, return empty string
            if (userPrincipal == null)
            {
                return string.Empty;
            }

            //Now try to get profile value.  In newer versions, profile names have underscores inserted
            // in place of spaces to address piping issues.  Attempt to handle these cases as well.
            string profileValue = userPrincipal[cleanPipeName];

            if (Utilities.IsNullOrEmpty(profileValue)
                && cleanPipeName.Contains("_"))
            {
                profileValue = userPrincipal[cleanPipeName.Replace("_", " ")];
            }

            return profileValue;
        }
    }
}
