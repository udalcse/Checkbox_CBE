using System;

using Checkbox.Forms.Piping.Tokens;
using Checkbox.Management;

namespace Checkbox.Forms.Piping.PipeHandlers
{
    /// <summary>
    /// Handler for getting pipe values for tokens representing response or response template values.
    /// </summary>
    public class ResponseTemplatePipeHandler : PipeHandler
    {
        /// <summary>
        /// Get the value of a token.
        /// </summary>
        /// <param name="token">Token values.</param>
        /// <param name="context">Context for response template.</param>
        /// <returns></returns>
        public override string GetTokenValue(Token token, object context)
        {
            string pipe = ApplicationManager.AppSettings.PipePrefix;

            if (token.TokenName.ToLower() == pipe + "responsetemplateguid")
            {
                if (context is ResponseTemplate)
                {
                    return ((ResponseTemplate)context).GUID.ToString();
                }
                
                if (context is ResponseProperties)
                {
                    object val = ((ResponseProperties)context).GetObjectValue("ResponseTemplateID");

                    if (val != null && val is Int32)
                    {
                        Guid? guid = ResponseTemplateManager.GetResponseTemplateGUID((Int32)val);

                        if (guid != null)
                        {
                            return guid.ToString();
                        }
                    }
                }
            }
            else if (token.TokenName.ToLower() == pipe + "responsetemplateid")
            {
                if (context is ResponseProperties)
                {
                    return ((ResponseProperties)context).GetStringValue("ResponseTemplateID");
                }
                if (context is ResponseTemplate)
                {
                    return ((ResponseTemplate)context).ID.ToString();
                }
            }

            return string.Empty;
        }
    }
}