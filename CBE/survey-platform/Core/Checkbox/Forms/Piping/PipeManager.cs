using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Checkbox.Common;
using Checkbox.Security;
using Checkbox.Management;
using Checkbox.Forms.Piping.Tokens;
using Checkbox.Forms.Piping.PipeHandlers;
using Checkbox.Invitations;

namespace Checkbox.Forms.Piping
{
    /// <summary>
    /// PipeManager handles functionality related to variable information using Pipes, or placeholders, for runtime data.
    /// </summary>
    public static class PipeManager
    {
        /// <summary>
        /// Get a list of possible profile pipe names
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<string> GetProfilePipeNames(List<string> customUserFieldNames)
        {
            if (customUserFieldNames == null)
            {
                customUserFieldNames = ProfileManager.ListPropertyNames();
            }

            if (!customUserFieldNames.Contains("UserName"))
            {
                customUserFieldNames.Add("UserName");
            }

            if (!customUserFieldNames.Contains("Password"))
            {
                customUserFieldNames.Add("Password");
            }

            if (!customUserFieldNames.Contains("GUID"))
            {
                customUserFieldNames.Add("GUID");
            }

            if (!customUserFieldNames.Contains("Email"))
            {
                customUserFieldNames.Add("Email");
            }

            return new ReadOnlyCollection<string>(customUserFieldNames);
        }

        /// <summary>
        /// Get a list of possible pipe names for response information
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<string> GetResponsePipeNames()
        {
            return ResponseProperties.PropertyNames;
        }

        /// <summary>
        /// Get pipe names for the response template
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<string> GetResponseTemplatePipeNames()
        {
            //TODO: Flesh out this list
            var names = new List<string> {"ResponseTemplateGUID"};

            if (ApplicationManager.AppSettings.AllowResponseTemplateIDLookup)
            {
                names.Add("ResponseTemplateID");
            }

            return new ReadOnlyCollection<string>(names);
        }


        /// <summary>
        /// Get the value of a particular token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context">Token context</param>
        /// <returns></returns>
        public static string GetTokenValue(Token token, object context)
        {
            if (token.Type == TokenType.Answer)
            {
                return new AnswerPipeHandler().GetTokenValue(token, context);
            }

            if (token.Type == TokenType.Profile)
            {
                return new ProfilePipeHandler().GetTokenValue(token, context);
            }

            if (token.Type == TokenType.Response)
            {
                return new ResponsePipeHandler().GetTokenValue(token, context);
            }

            if (token.Type == TokenType.ResponseTemplate)
            {
                return new ResponseTemplatePipeHandler().GetTokenValue(token, context);
            }

            return string.Empty;
        }


        /// <summary>
        /// Get a new token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="customUserFieldNames"></param>
        /// <returns></returns>
        public static Token GetToken(string token, List<string> customUserFieldNames)
        {
            TokenType tokenType = GetTokenType(token, customUserFieldNames);

            if (tokenType == TokenType.Profile)
            {
                return new ProfileToken(token);
            }

            if (tokenType == TokenType.Response)
            {
                return new ResponseToken(token);
            }

            if (tokenType == TokenType.ResponseTemplate)
            {
                return new ResponseTemplateToken(token);
            }

            return new Token(token);
        }

        /// <summary>
        /// Get the type of token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="customUserFieldNames"></param>
        /// <returns></returns>
        private static TokenType GetTokenType(string token, List<string> customUserFieldNames)
        {
            if (Utilities.IsNotNullOrEmpty(token))
            {
                string strippedToken = token.Replace(ApplicationManager.AppSettings.PipePrefix, string.Empty);

                foreach (string pipeName in GetProfilePipeNames(customUserFieldNames))
                {
                    if (string.Compare(pipeName, strippedToken, true) == 0
                        || string.Compare(pipeName, strippedToken.Replace("_", " "), true) == 0)
                    {
                        return TokenType.Profile;
                    }

                    if (string.Compare(pipeName, strippedToken.Replace("_", " "), true) == 0)
                    {
                        return TokenType.Profile;
                    }
                }

                if (GetResponsePipeNames().Any(pipeName => string.Compare(pipeName, strippedToken, true) == 0)
                    || strippedToken.StartsWith(ResponseProperties.PageScorePropertyName, true, CultureInfo.InvariantCulture)
                    || strippedToken.StartsWith(ResponseProperties.PagePossibleScorePropertyName, true, CultureInfo.InvariantCulture))
                {
                    return TokenType.Response;
                }

                if (GetResponseTemplatePipeNames().Any(pipeName => string.Compare(pipeName, strippedToken, true) == 0))
                {
                    return TokenType.ResponseTemplate;
                }

                //Otherwise, assume answer
                return TokenType.Answer;
            }

            throw new Exception("Unable to determine type for NULL token.");
        }

        /// <summary>
        /// Returns a list of company names
        /// </summary>
        /// <returns></returns>
        public static string[] ListCompanyProperties()
        {
            return CompanyProfileFacade.ListCompanyPropertyNames();
        }

        private static string[] _dateProperties = new string[] { "Year", "Month", "Day" };

        /// <summary>
        /// Returns a list of company names
        /// </summary>
        /// <returns></returns>
        public static string[] ListDateProperties()
        {
            return _dateProperties;
        }
    }
}
