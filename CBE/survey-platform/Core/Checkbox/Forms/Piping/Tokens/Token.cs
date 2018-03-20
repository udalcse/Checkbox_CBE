using System;

using Checkbox.Management;

namespace Checkbox.Forms.Piping.Tokens
{
    /// <summary>
    /// Token for piping
    /// </summary>
    [Serializable]
    public class Token : IEquatable<string>, IEquatable<Token>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        public Token(string token)
        {
            TokenName = token;
            Type = TokenType.Other;
        }

        /// <summary>
        /// Create a token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        protected Token(string token, TokenType type)
        {
            TokenName = token;
            Type = type;
        }

        /// <summary>
        /// Get/set the name of the token
        /// </summary>
        public string TokenName { get; set; }

        /// <summary>
        /// Get the name of the token without the PipePrefix
        /// </summary>
        public string CleanName
        {
            get { return TokenName.Replace(ApplicationManager.AppSettings.PipePrefix, string.Empty); }
        }

        /// <summary>
        /// Token type
        /// </summary>
        public TokenType Type { get; set; }

        #region IEquatable<string> Members

        /// <summary>
        /// Compare token with/string
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            return (string.Compare(other, TokenName, true) == 0);
            
        }

        #endregion

        #region IEquatable<Token> Members

        /// <summary>
        /// Compare a token with itself
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Token other)
        {
            return (string.Compare(other.TokenName, TokenName, true) == 0);
        }

        #endregion
    }
}
