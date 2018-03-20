using System;

namespace Checkbox.Forms.Piping.Tokens
{
    /// <summary>
    /// Token representing a property of the current survey response.
    /// </summary>
    [Serializable]
    public class ResponseToken : Token
    {
        /// <summary>
        /// Construct the token.
        /// </summary>
        /// <param name="token">Name of token</param>
        public ResponseToken(string token)
            : base(token, TokenType.Response)
        {
        }
    }
}
