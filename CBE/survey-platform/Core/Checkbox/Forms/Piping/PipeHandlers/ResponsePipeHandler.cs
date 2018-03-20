using System;
using Checkbox.Forms.Piping.Tokens;

namespace Checkbox.Forms.Piping.PipeHandlers
{
    /// <summary>
    /// Pipe handler for retrieving values from a response.
    /// </summary>
    public class ResponsePipeHandler : PipeHandler
    {
        /// <summary>
        /// Get value of response token.
        /// </summary>
        /// <param name="token">Token to get value of.</param>
        /// <param name="context">Response context.</param>
        /// <returns>String value for token.</returns>
        public override string GetTokenValue(Token token, object context)
        {
            if (context is ResponseProperties && context != null)
            {
                ResponseProperties responseProperties = (ResponseProperties)context;
                string strippedToken = token.CleanName;

                switch (strippedToken.ToLower())
                {
                    case "started":
                    case "lastedit":
                        
                        object val = responseProperties.GetObjectValue(strippedToken);
                        
                        if (val != null && val is DateTime)
                        {
                            return ((DateTime)val).ToShortDateString();
                        }
                        
                        return string.Empty;

                    default:
                        return responseProperties.GetStringValue(strippedToken);
                }
            }

            return string.Empty;
        }
    }
}
