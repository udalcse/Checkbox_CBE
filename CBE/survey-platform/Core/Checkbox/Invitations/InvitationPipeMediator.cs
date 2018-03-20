using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Linq;

using Checkbox.Forms;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Forms.Piping;
using Checkbox.Forms.Piping.Tokens;
using Checkbox.Users;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Pipe mediator for invitations
    /// </summary>
    public class InvitationPipeMediator : PipeMediator
    {
        private Invitation _invitation;
        private Recipient _recipient;
        private List<string> _customUserFieldNames;
        private string _baseSurveyUrl;
        private Guid? _surveyGuid;
        private CompanyProfile _companyProfile;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="invitation"></param>
        /// <param name="recipient"></param>
        /// <param name="customUserFieldNames"></param>
        /// <param name="baseSurveyUrl"></param>
        public void Initialize(Invitation invitation, Recipient recipient, List<string> customUserFieldNames, string baseSurveyUrl, Guid? surveyGuid)
        {
            _invitation = invitation;
            _recipient = recipient;
            _baseSurveyUrl = baseSurveyUrl;
            _customUserFieldNames = customUserFieldNames;
            _surveyGuid = surveyGuid;
            _companyProfile = invitation.GetCompanyProfile();
        }

        /// <summary>
        /// 
        /// </summary>
        private List<string> CustomUserFieldNames
        {
            get { return _customUserFieldNames ?? (_customUserFieldNames = new List<string>()); }
        }

        /// <summary>
        /// Override token processing
        /// </summary>
        /// <param name="tokenValues"></param>
        /// <param name="removeEmpty"></param>
        protected override void ProcessTokens(ReadOnlyCollection<TokenValue> tokenValues, bool removeEmpty)
        {
            foreach (TokenValue tokenValue in tokenValues)
            {
                if (tokenValue.IsDirty)
                {
                    if (_recipient == null)
                    {
                        tokenValue.Value = string.Empty;
                    }
                    else
                    {
                        //Special handling
                        if (tokenValue.Token.CleanName.ToLower() == "password")
                        {
                            if (!string.IsNullOrEmpty(_recipient.UniqueIdentifier))
                            {
                                tokenValue.Value = UserManager.GetUserPassword(_recipient.UniqueIdentifier);
                            }
                        }
                        else if (_companyProfile != null && CompanyProfileFacade.ListCompanyPropertyNames().Contains(tokenValue.Token.CleanName, CompanyProfileFacade.PipeComparer))
                        {
                            tokenValue.Value = _companyProfile.GetProperty(tokenValue.Token.CleanName);
                        }
                        else if (tokenValue.Token.Type == TokenType.Profile
                            || tokenValue.Token.CleanName.ToLower() == "userguid"
                            || tokenValue.Token.CleanName.ToLower() == "guid"
                            || tokenValue.Token.CleanName.ToLower() == "email"
                            || tokenValue.Token.CleanName.ToLower() == "username")
                        {
                            tokenValue.Value = _recipient[tokenValue.Token.TokenName.Replace(ApplicationManager.AppSettings.PipePrefix, string.Empty)];
                        }
                        // Use the recipient GUID now but the OptOut.aspx page and the
                        // Invitation class must still handle the deprecated invitation id so include it
                        // for backwards compatibility.
                        else if (tokenValue.Token.CleanName.ToLower() == "invitationid")
                        {
                            tokenValue.Value = _recipient.ID.ToString().Replace("-", string.Empty);
                        }
                        else if (tokenValue.Token.CleanName.ToLower() == "recipientguid")
                        {
                            tokenValue.Value = _recipient.GUID.ToString().Replace("-", string.Empty);
                        }
                        else if (tokenValue.Token.TokenName.Equals(InvitationManager.SURVEY_URL_PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase))
                        {
                            tokenValue.Value = GetRecipientSurveyUrl();
                        }
                        else if (tokenValue.Token.TokenName.Equals(InvitationManager.OPT_OUT_URL_PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase))
                        {
                            tokenValue.Value = GetOptOutUrl(false);
                        }
                        else if (tokenValue.Token.CleanName.ToLower().Equals("year"))
                        {
                            tokenValue.Value = DateTime.Now.Year.ToString();
                        }
                        else if (tokenValue.Token.CleanName.ToLower().Equals("month"))
                        {
                            tokenValue.Value = DateTime.Now.Month.ToString();
                        }
                        else if (tokenValue.Token.CleanName.ToLower().Equals("day"))
                        {
                            tokenValue.Value = DateTime.Now.Day.ToString();
                        }
                        else
                        {
                            tokenValue.Value = string.Empty;
                        }
                    }
                }

                if (!removeEmpty)
                {
                    if (Utilities.IsNullOrEmpty(tokenValue.Value))
                    {
                        tokenValue.Value = tokenValue.Token.TokenName;
                    }
                }
            }
        }

        /// <summary>
        /// Override text processing
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        public void RegisterText(string key, string text)
        {
            if (Utilities.IsNotNullOrEmpty(key) && Utilities.IsNotNullOrEmpty(text))
            {
                string cacheKey = key;

                if (TextCacheContainsKey(cacheKey))
                {
                    ProcessedText processedText = GetTextFromCache(cacheKey);
                    processedText.OriginalText = text;
                }
                else
                {
                    ProcessedText processedText = new ProcessedText();
                    processedText.OriginalText = text;

                    CacheText(cacheKey, processedText);

                    //Parse the text for tokens
                    Match m = RegExp.Match(text);

                    while (m.Success)
                    {
                        if (ValueCacheContainsKey(m.Value))
                        {
                            processedText.AddTokenValue(GetValueFromCache(m.Value));
                        }
                        else
                        {
                            Token token = PipeManager.GetToken(m.Value, CustomUserFieldNames);

                            if (token != null)
                            {
                                TokenValues[m.Value] = new TokenValue(token);
                                processedText.AddTokenValue(TokenValues[m.Value]);
                            }
                        }

                        m = m.NextMatch();
                    }
                }
            }
        }

        /// <summary>
        /// Get custom text
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetText(string key)
        {
            if (key == null || key.Trim() == string.Empty)
            {
                return string.Empty;
            }

            string cacheKey = key;

            ProcessedText text = GetTextFromCache(cacheKey);

            if (text != null)
            {
                ProcessTokens(text.TokenValues, true);
                return text.Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get a custom survey url for the recipient
        /// </summary>
        /// <returns></returns>
        public string GetRecipientSurveyUrl()
        {
            //
            if (_recipient == null || _invitation == null || _invitation.ParentID <= 0)
            {
                return string.Empty;
            }

            //For sake of efficiency, attempt to use base url passed-in
            
            if (Utilities.IsNullOrEmpty(_baseSurveyUrl))
            {
                _baseSurveyUrl = GetBaseSurveyUrl(_invitation.ParentID);
            }

            string prefix = _baseSurveyUrl.Contains("?") ? "&" : "?";

            string recipientGuidString = prefix + "i=" + _recipient.GUID.ToString().Replace("-", string.Empty);

            return _baseSurveyUrl + recipientGuidString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyGuid"></param>
        /// <returns></returns>
        public static string GetBaseSurveyUrl(int surveyGuid)
        {
            return GetBaseSurveyUrl(ResponseTemplateManager.GetResponseTemplateGUID(surveyGuid));
        }

        /// <summary>
        /// Get base url for surveys
        /// </summary>
        public static string GetBaseSurveyUrl(Guid? surveyGuid)
        {
            if (!surveyGuid.HasValue)
            {
                return string.Empty;
            }

            string applicationPath = ApplicationManager.ApplicationPath;
            string surveyBaseUrl = "/Survey.aspx?s=" + surveyGuid.ToString().Replace("-", string.Empty);

            if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                string shortUrl = UrlMapper.GetSource(surveyBaseUrl);

                if (Utilities.IsNotNullOrEmpty(shortUrl))
                {
                    return applicationPath + shortUrl;
                }
            }

            return applicationPath + surveyBaseUrl;
        }

        /// <summary>
        /// Get base url for surveys
        /// </summary>
        public string GetOptOutUrl(bool addLinkTag)
        {
            if (!_surveyGuid.HasValue || _recipient == null)
                return string.Empty;

            string applicationPath = ApplicationManager.ApplicationPath;
            string baseUrl = "/OptOut.aspx?s=" + _surveyGuid.ToString().Replace("-", string.Empty) + "&i=" + _recipient.GUID.ToString().Replace("-", string.Empty);

            var href = applicationPath + baseUrl;

            if (!addLinkTag)
                return href;

            return string.Format("<a href=\"{0}\" >{1}</a>", href, "Unsubscribe from this list");
        }
    }
}
