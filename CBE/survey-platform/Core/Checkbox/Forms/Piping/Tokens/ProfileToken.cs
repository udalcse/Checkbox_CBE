using System;

namespace Checkbox.Forms.Piping.Tokens
{
    /// <summary>
    /// Token representing a property of a respondent taking a survey.
    /// </summary>
    [Serializable]
    public class ProfileToken : Token
    {
        /// <summary>
        /// Construct the token.
        /// </summary>
        /// <param name="token">Name of token (typically a user profile property)</param>
        public ProfileToken(string token)
            : base(token, TokenType.Profile)
        {
        }
    }
}
