using System;
using System.Web;
using System.Web.SessionState;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Forms;
using Checkbox.Users;
using Checkbox.Analytics;
using Checkbox.Styles;
using Checkbox.Globalization.Text;

using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Analytics
{
    /// <summary>
    /// Handler for running analyses.
    /// </summary>
    public class AnalysisHandler
    {
        private string _languageCode;
        private AnalysisTemplate _analysisTemplate;
        private Analysis _analysis;
        private StyleTemplate _styleTemplate;

        readonly bool _isPostBack;

        /// <summary>
        /// Construct a new analysis handler.
        /// </summary>
        /// <param name="isPostBack">Specify whether page is being posted back or not.</param>
        public AnalysisHandler(bool isPostBack)
        {
            _isPostBack = isPostBack;
        }

        /// <summary>
        /// Authorize the current user to take the survey
        /// </summary>
        /// <returns></returns>
        public bool Authorize()
        {
            //0) A deleted survey is no longer available to anyone
            if (AnalysisTemplate.IsDeleted)
            {
                return false;
            }

            //1) An Editor will always be able to view the analysis
            if (AuthorizationFactory.GetAuthorizationProvider().Authorize(User, AnalysisTemplate, "Analysis.Edit"))
            {
                return true;
            }

            //2) A ticketed user using the View Analysis item will be able to view regardless of settings
            if (CheckTicket())
            {
                return true;
            }

            //3) Authorize the principal
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(User, AnalysisTemplate, "Analysis.Run"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a message localized for the current survey
        /// </summary>
        /// <param name="textID"></param>
        /// <param name="defaultText"></param>
        /// <returns></returns>
        public string GetMessage(string textID, string defaultText)
        {
            //Try to get a localized error message, otherwise default to US or an actual message.
            string message = TextManager.GetText(textID, LanguageCode);

            if (Utilities.IsNullOrEmpty(message) && LanguageCode != TextManager.DefaultLanguage && Utilities.IsNotNullOrEmpty(TextManager.DefaultLanguage))
            {
                message = TextManager.GetText(textID, TextManager.DefaultLanguage);
            }

            if (Utilities.IsNullOrEmpty(message) && LanguageCode != "en-US")
            {
                message = TextManager.GetText(textID, "en-US");
            }

            if (Utilities.IsNullOrEmpty(message))
            {
                message = defaultText;
            }

            return message;
        }

        /// <summary>
        /// Check if a security ticket is enabled
        /// </summary>
        /// <returns></returns>
        public bool CheckTicket()
        {
            bool valid = false;
            try
            {
                if (Request.QueryString["tg"] != null && Request.QueryString["tg"].Trim() != string.Empty)
                {
                    Guid ticketGuid = new Guid(Request.QueryString["tg"]);
                    valid = Ticketing.ValidateTicket(ticketGuid);                    
                }
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");
            }

            return valid;
        }

        /// <summary>
        /// Get the current response
        /// </summary>
        public Analysis Analysis
        {
            get
            {
                if (_analysis == null)
                {
                    //Cause response template to be loaded.  This is necessary due to way ResponseTemplateManager.ActiveSurveyLanguages is used
                    // in SurveyMetaDataProxy to limit number of languages to load item/option text for. (In hosting environment, there are
                    // 20+ available survey languages, and it is not efficient to load text for all languages.)  However, if the survey is not
                    // loaded first, the ActiveSurveyLanguages will not have the languages for that survey loaded, resulting in blank or default
                    // text in repors if the report is run before something else loads the survey after an application pool recycle.
                    ResponseTemplateManager.GetResponseTemplate(AnalysisTemplate.ResponseTemplateID);

                    _analysis = AnalysisTemplate.CreateAnalysis(LanguageCode, ApplicationManager.AppSettings.ReportIncompleteResponses, ApplicationManager.AppSettings.ReportTestResponses);

                    if (_analysis == null)
                    {
                        throw new Exception(GetMessage("/pageText/runAnalysis.aspx/unableToCreateAnalysis", "The analysis could not be created.  Please verify that your URL is correct and try again."));
                    }
                }

                return _analysis;
            }
        }

        /// <summary>
        /// Get the style template for the analysis
        /// </summary>
        public StyleTemplate StyleTemplate
        {
            get
            {
                if (_styleTemplate == null && AnalysisTemplate.StyleTemplateID != null)
                {
                    //Try to get the template from the session
                    _styleTemplate = GetSessionValue<StyleTemplate>("StyleTemplate", null);
                    
                    if (_styleTemplate == null)
                    {
                        _styleTemplate = StyleTemplateManager.GetStyleTemplate(AnalysisTemplate.StyleTemplateID.Value);

                        //Store the value
                        SetSessionValue("StyleTemplate", _styleTemplate);
                    }
                }

                return _styleTemplate;
            }
        }

        /// <summary>
        /// Get the analysis template
        /// </summary>
        public AnalysisTemplate AnalysisTemplate
        {
            get
            {
                if (_analysisTemplate == null)
                {
                    //Try to get the analysis template from the session
                    _analysisTemplate = GetSessionValue<AnalysisTemplate>("AnalysisTemplate", null);

                    //If the value is not in the session, try to load it
                    if (_analysisTemplate == null)
                    {
                        Guid? templateGuid = null;
                        
                        //Get guid
                        try
                        {
                            if (Request.QueryString["a"] != null && Request.QueryString["a"].Trim() != null)
                            {
                                templateGuid = new Guid(Request.QueryString["a"]);
                            }
                        }
                        catch
                        {
                        }

                        //Get ID
                        int? templateID = null;
                        try
                        {
                            if (Request.QueryString["AnalysisID"] != null && Request.QueryString["AnalysisID"].Trim() != null)
                            {
                                templateID = Convert.ToInt32(Request.QueryString["AnalysisID"]);
                            }
                        }
                        catch
                        {
                        }

                        if (templateGuid != null)
                        {
                            _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(templateGuid.Value);
                        }
                        else if (templateID != null && templateID > 0)
                        {
                            _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(templateID.Value);
                        }
                        else
                        {
                            throw new Exception(GetMessage("/pageText/runAnalysis.aspx/noAnalysisSpecified", "No analysis was specified.  Please verify that the URL is correct and try again."));
                        }

                        if (_analysisTemplate == null)
                        {
                            throw new Exception(GetMessage("/pageText/runAnalysis.aspx/unableToLoadAnalysisTemplate", "An analysis was specified, but could not be loaded."));
                        }

                        //Store the value for later
                        SetSessionValue("AnalysisTemplate", _analysisTemplate);
                    }
                }

                return _analysisTemplate;
            }
        }

        /// <summary>
        /// Get the language for the respondent
        /// </summary>
        public string LanguageCode
        {
            get
            {
                if (_languageCode == null || _languageCode.Trim() == string.Empty)
                {
                    //Attempt to get the language code from the session
                    _languageCode = GetSessionValue<string>("AnalysisLanguageCode", null);

                    //If the value is not in the session, try to determine what it should be
                    if (_languageCode == null || _languageCode.Trim() == string.Empty)
                    {
                        if (!string.IsNullOrEmpty(Request.QueryString["Language"]))
                        {
                            _languageCode = Request.QueryString["Language"];
                        }
                        else
                        {
                            _languageCode = WebTextManager.GetUserLanguage();

                            //Try the user language, but only if it's in the list of survey languages
                            if (Analysis.ResponseTemplateIDs.Count > 0)
                            {
                                LightweightResponseTemplate rt = ResponseTemplateManager.GetLightweightResponseTemplate(Analysis.ResponseTemplateIDs[0]);

                                if (rt != null)
                                {
                                    bool languageOk = false;

                                    foreach (string language in rt.SupportedLanguages)
                                    {
                                        if (string.Compare(language, _languageCode, true) == 0)
                                        {
                                            languageOk = true;
                                            break;
                                        }
                                    }

                                    if (!languageOk && rt.DefaultLanguage != null && rt.DefaultLanguage.Trim() != string.Empty)
                                    {
                                        _languageCode = rt.DefaultLanguage;
                                    }
                                }
                            }
                        }

                        //Store the value
                        SetSessionValue("AnalysisLanguageCode", _languageCode);
                    }
                }

                return _languageCode;
            }
        }

        /// <summary>
        /// Store a value in the session for the analysis handler. 
        /// </summary>
        /// <param name="name">Lookup key for value.</param>
        /// <param name="value">Value to store.</param>
        private static void SetSessionValue(string name, object value)
        {
            Session[name] = value;
        }

        /// <summary>
        /// Get a value from the session.  Returns null if value not found is session OR if
        /// postaback is false.
        /// </summary>
        /// <typeparam name="T">Type of value to return.</typeparam>
        /// <param name="name">Lookup key for value.</param>
        /// <param name="defaultValue">Default value to return if item with lookup key not value.</param>
        /// <returns>Value of found item or default value if item not found.</returns>
        private T GetSessionValue<T>(string name, T defaultValue)
        {
            if (!_isPostBack || Session[name] == null)
            {
                return defaultValue;
            }
            
            return (T)Session[name];
        }

        /// <summary>
        /// Get the current context Request object
        /// </summary>
        private static HttpRequest Request
        {
            get { return HttpContext.Current.Request; }
        }
        /// <summary>
        /// Get the current context Session object
        /// </summary>
        private static HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        /// <summary>
        /// Survey respondent
        /// </summary>
        private static ExtendedPrincipal User
        {
            get { return (ExtendedPrincipal)UserManager.GetCurrentPrincipal(); }
        }
    }
}
