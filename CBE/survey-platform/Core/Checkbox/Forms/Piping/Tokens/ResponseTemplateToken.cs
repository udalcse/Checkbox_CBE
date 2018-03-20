using System;

namespace Checkbox.Forms.Piping.Tokens
{
    /// <summary>
    /// Token representing a value from a ResponseTemplate associated with a survey Response.
    /// </summary>
    [Serializable]
    public class ResponseTemplateToken : Token
    {
        /// <summary>
        /// Construct the token.
        /// </summary>
        /// <param name="token">Name of token.</param>
        public ResponseTemplateToken(string token)
            : base(token, TokenType.ResponseTemplate)
        {
        }
    }
}
