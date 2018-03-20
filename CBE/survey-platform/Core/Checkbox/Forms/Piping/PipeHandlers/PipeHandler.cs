using Checkbox.Forms.Piping.Tokens;

namespace Checkbox.Forms.Piping.PipeHandlers
{
    /// <summary>
    /// Base class for object to handle performing token string replacements for piping.
    /// </summary>
    public abstract class PipeHandler
    {
        /// <summary>
        /// Get the string replacement for a specific piping token.
        /// </summary>
        /// <param name="token">Piping token to get string value for.</param>
        /// <param name="context">Context for pipe, such as a survey Response, respondent security IPrincipal, ResponseTemplate, etc. </param>
        /// <returns>String value for token replacement.</returns>
        public abstract string GetTokenValue(Token token, object context);
    }
}