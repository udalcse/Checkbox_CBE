using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Piping;
using Checkbox.Forms.Validation;
using Checkbox.Globalization;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Newtonsoft.Json;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Static methods for survey editor service implementation
    /// </summary>
    public static class SurveyEditorServiceImplementation
    {
        /// <summary>
        /// List autocopmplete list items
        /// </summary>
        /// <returns></returns>
        public static string ListAutocompleteListData(CheckboxPrincipal userPrincipal, int listId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            return string.Join("\n", AutocompleteListManager.ListItems(listId));
        }

        /// <summary>
        /// List survey pipe sources
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="maxPageNumber"></param>
        /// <param name="languageCode"></param>
        /// <param name="customFieldTypeId"></param>
        /// <returns></returns>
        public static PipeSource[] ListPipeSources(CheckboxPrincipal userPrincipal, int? surveyId, int? maxPageNumber,
                                                   string languageCode, int customFieldTypeId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Form.Edit");

            var pipeSources = new List<PipeSource>();

            if (surveyId.HasValue && surveyId > 0)
            {
                if (!maxPageNumber.HasValue || maxPageNumber <= 0)
                {
                    throw new Exception("Max Page number required when listing survey item pipes.");
                }

                if (string.IsNullOrEmpty(languageCode))
                {
                    throw new Exception("Language code required when listing survey item pipes.");
                }

                pipeSources.AddRange(customFieldTypeId == 0 ? ListProfilePipes() : ListProfilePipes(customFieldTypeId));
                pipeSources.AddRange(ListSurveyItemPipes(surveyId.Value, maxPageNumber.Value, languageCode));
                pipeSources.AddRange(ListResponsePipes(surveyId.Value, maxPageNumber.Value));
                pipeSources.AddRange(ListResponseTemplatePipes());
            }
            else
            {
                pipeSources.AddRange(customFieldTypeId == 0 ? ListProfilePipes() : ListProfilePipes(customFieldTypeId));
                pipeSources.AddRange(ListCompanyPipes());
                pipeSources.AddRange(ListDatePipes());
                pipeSources.AddRange(ListResponseTemplatePipes());
            }

            return pipeSources.ToArray();
        }

        //public static Dictionary<int, string> GetProfilePropertiesNames(int customFieldTypeId)
        //{
        //    return ProfileManager.GetProfileKeysWithIds(customFieldTypeId);
        //}

        public static void SaveItemProfilePropertyMapping(int itemId, int profilePropertyId)
        {
            PropertyBindingManager.AddSurveyItemProfilePropertyMapping(itemId, profilePropertyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="maxPageNumber"></param>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListResponsePipes(int surveyId, int maxPageNumber)
        {
            var theSurvey = ResponseTemplateManager.GetResponseTemplate(surveyId);
            var includeScore = theSurvey != null && theSurvey.BehaviorSettings.EnableScoring;
            var userLanguage = WebTextManager.GetUserLanguage();

            List<PipeSource> res = new List<PipeSource>();
            foreach (var pn in PipeManager.GetResponsePipeNames())
            {
                if (includeScore || 
                    (!pn.Equals("currentscore", StringComparison.InvariantCultureIgnoreCase) && 
                    !pn.Equals("totalpossiblescore", StringComparison.InvariantCultureIgnoreCase)))
                {
                    res.Add(new PipeSource
                            {
                                SourceType = "Response",
                                PipeToken = ApplicationManager.AppSettings.PipePrefix + pn,
                                DisplayText = WebTextManager.GetText("/responseProperty/" + pn + "/text", userLanguage, pn)
                            });

                    if (includeScore && pn.Equals("currentscore", StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var p in ResponseProperties.GetPageScoreProperties(maxPageNumber, true))
                        {
                            res.Add(new PipeSource
                            {
                                SourceType = "Response",
                                PipeToken = ApplicationManager.AppSettings.PipePrefix + p.Key,
                                DisplayText = p.Value
                            });
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListProfilePipes()
        {
            return 
                ProfileManager.GetPropertiesList()
                    .Select(pn => new PipeSource
                    {
                        PipeToken = ApplicationManager.AppSettings.PipePrefix + pn.Name,
                        SourceType = "Profile",
                        DisplayText = pn.Name,
                        FieldType = pn.FieldType.ToString(),
                        BindedItemId = pn.BindedItemId
                     });
        }

        private static IEnumerable<PipeSource> ListProfilePipes(int customFieldTypeId)
        {
            return
                PipeManager
                    .GetProfilePipeNames(ProfileManager.ListPropertyNames(customFieldTypeId))
                    .OrderBy(pn => pn)
                    .Select(pn => new PipeSource
                    {
                        PipeToken = ApplicationManager.AppSettings.PipePrefix + pn,
                        SourceType = "Profile",
                        DisplayText = pn.Replace(" ", "_")
                    });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListCompanyPipes()
        {
            return
                PipeManager.ListCompanyProperties()
                    .OrderBy(pn => pn)
                    .Select(pn => new PipeSource
                    {
                        PipeToken = ApplicationManager.AppSettings.PipePrefix + pn,
                        SourceType = "Company",
                        DisplayText = pn.Replace(" ", "_")
                    });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListDatePipes()
        {
            return
                PipeManager.ListDateProperties()
                    .OrderBy(pn => pn)
                    .Select(pn => new PipeSource
                    {
                        PipeToken = ApplicationManager.AppSettings.PipePrefix + pn,
                        SourceType = "Date",
                        DisplayText = pn.Replace(" ", "_")
                    });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListResponseTemplatePipes()
        {
            var userLanguage = WebTextManager.GetUserLanguage();

            return
                PipeManager
                    .GetResponseTemplatePipeNames()
                    .Select(pn => new PipeSource
                                      {
                                          SourceType = "ResponseTemplate",
                                          PipeToken = ApplicationManager.AppSettings.PipePrefix + pn,
                                          DisplayText =
                                              WebTextManager.GetText("/responseTemplateProperty/" + pn + "/text",
                                                                     userLanguage, pn)
                                      });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<PipeSource> ListSurveyItemPipes(int surveyId, int maxPagePosition, string languageCode)
        {
            var surveyItems = ItemConfigurationManager.ListBasicItemsData(surveyId, maxPagePosition, languageCode);

            //Exclude RankOrder item. It isn't useful for piping.

            return
                surveyItems
                    .Select(si => new PipeSource
                                      {
                                          SourceType = "Question",
                                          PipeToken =
                                              "[I]" + ApplicationManager.AppSettings.PipePrefix +
                                              si.ItemId.ToString().PadLeft(6, '0'),
                                          //[I] value used by pipeHandler.js
                                          DisplayText = Utilities.DecodeAndStripHtml(si.ItemText, 61)
                                      });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public static bool ToggleSurveySetting(CheckboxPrincipal principal, int surveyId, string settingName)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Administer");

            var newValue = false;

            switch (settingName.ToLower())
            {
                    //Behavior Settings
                case "anonymizeresponses":
                    newValue = !responseTemplate.BehaviorSettings.AnonymizeResponses;
                    responseTemplate.BehaviorSettings.AnonymizeResponses = newValue;
                    break;

                case "enablescoring":
                    newValue = !responseTemplate.BehaviorSettings.EnableScoring;
                    responseTemplate.BehaviorSettings.EnableScoring = newValue;
                    break;

                case "allowsurveyeditwhileactive":
                    newValue = !responseTemplate.BehaviorSettings.AllowSurveyEditWhileActive;
                    responseTemplate.BehaviorSettings.AllowSurveyEditWhileActive = newValue;
                    break;
                case "displaypdfdownloadbutton":
                    newValue = !responseTemplate.BehaviorSettings.DisplayPDFDownloadButton;
                    responseTemplate.BehaviorSettings.DisplayPDFDownloadButton = newValue;
                    break;

                case "isactive":
                    newValue = !responseTemplate.BehaviorSettings.IsActive;
                    responseTemplate.BehaviorSettings.IsActive = newValue;
                    break;


                case "randomizeitems":
                    newValue = !responseTemplate.BehaviorSettings.RandomizeItemsInPages;
                    responseTemplate.BehaviorSettings.RandomizeItemsInPages = newValue;
                    break;

                case "allowedit":
                    newValue = !responseTemplate.BehaviorSettings.AllowEdit;
                    responseTemplate.BehaviorSettings.AllowEdit = newValue;
                    break;

                case "allowresume":
                    newValue = !responseTemplate.BehaviorSettings.AllowContinue;
                    responseTemplate.BehaviorSettings.AllowContinue = newValue;
                    break;

                case "showsaveandquit":
                    newValue = !responseTemplate.BehaviorSettings.ShowSaveAndQuit;
                    responseTemplate.BehaviorSettings.ShowSaveAndQuit = newValue;
                    break;

                case "disablebackbutton":
                    newValue = !responseTemplate.BehaviorSettings.DisableBackButton;
                    responseTemplate.BehaviorSettings.DisableBackButton = newValue;
                    break;

                case "allowformreset":
                    newValue = !responseTemplate.BehaviorSettings.AllowFormReset;
                    responseTemplate.BehaviorSettings.AllowFormReset = newValue;
                    break;

                    //Appearance settings
                case "showtitle":
                    newValue = !responseTemplate.StyleSettings.ShowTitle;
                    responseTemplate.StyleSettings.ShowTitle = newValue;
                    break;

                case "showprogressbar":
                    newValue = !responseTemplate.StyleSettings.ShowProgressBar;
                    responseTemplate.StyleSettings.ShowProgressBar = newValue;
                    break;

                case "showpagenumbers":
                    newValue = !responseTemplate.StyleSettings.ShowPageNumbers;
                    responseTemplate.StyleSettings.ShowPageNumbers = newValue;
                    break;

                case "showitemnumbers":
                    newValue = !responseTemplate.StyleSettings.ShowItemNumbers;
                    responseTemplate.StyleSettings.ShowItemNumbers = newValue;
                    break;

                case "showtopsurveybuttons":
                    newValue = !responseTemplate.StyleSettings.ShowTopSurveyButtons;
                    responseTemplate.StyleSettings.ShowTopSurveyButtons = newValue;
                    break;
                case "hidetopsurveybuttonsonfirstandlastpage":
                    newValue = !responseTemplate.StyleSettings.HideTopSurveyButtonsOnFirstAndLastPage;
                    responseTemplate.StyleSettings.HideTopSurveyButtonsOnFirstAndLastPage = newValue;
                    break;

                case "usedynamicpagenumbers":
                    newValue = !responseTemplate.StyleSettings.EnableDynamicPageNumbers;
                    responseTemplate.StyleSettings.EnableDynamicItemNumbers = newValue;
                        //Ensure both page/item numbering consistent
                    responseTemplate.StyleSettings.EnableDynamicPageNumbers = newValue;
                    break;

                case "showinputerroralert":
                    newValue = !responseTemplate.StyleSettings.ShowValidationErrorAlert;
                    responseTemplate.StyleSettings.ShowValidationErrorAlert = newValue;
                    break;

                case "showasterisks":
                    newValue = !responseTemplate.StyleSettings.ShowAsterisks;
                    responseTemplate.StyleSettings.ShowAsterisks = newValue;
                    break;

                case "hidefooterheader":
                    newValue = !responseTemplate.StyleSettings.HideFooterHeader;
                    responseTemplate.StyleSettings.HideFooterHeader = newValue;
                    break;

                default:
                    throw new Exception("Updating setting with name [" + settingName +
                                        "] not allowed by SurveyEditorService.");
            }

            responseTemplate.ModifiedBy = principal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);

            return newValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="value"> </param>
        /// <returns></returns>
        public static object UpdateConditionSource(CheckboxPrincipal principal, int surveyId, string value)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Administer");

            ServiceHelper.CurrentEditingConditionSource = value;

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValueAsString"></param>
        /// <returns></returns>
        public static string UpdateSurveySetting(CheckboxPrincipal principal, int surveyId, string settingName,
                                                 string settingValueAsString)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Administer");

            //encode the value
            settingValueAsString = Utilities.AdvancedHtmlDecode(settingValueAsString);

            string newValue = string.Empty;

            //Something clever could be done w/reflection to address this, but this will be used
            // (at least initially) for a limited number of properties, making such a solution
            // overkill.  Names used will generally correspond to Checkbox.Wcf.Proxies.SurveyMetaData
            // properties.
            switch (settingName.ToLower())
            {
                case "name":
                    //if keeping name same or just changing case, don't duplicate name check
                    newValue = settingValueAsString.Equals(responseTemplate.Name,
                                                           StringComparison.InvariantCultureIgnoreCase)
                                   ? settingValueAsString
                                   : ResponseTemplateManager.GetUniqueName(settingValueAsString,
                                                                           responseTemplate.LanguageSettings.
                                                                               DefaultLanguage);

                    responseTemplate.Name = newValue;
                    break;

                case "activationstartdate":
                    if (string.IsNullOrEmpty(settingValueAsString))
                    {
                        responseTemplate.BehaviorSettings.ActivationStartDate = null;
                        newValue = "";
                    }
                    else
                    {
                        
                        var startDate = Utilities.GetDate(settingValueAsString);

                        if (!startDate.HasValue)
                        {
                            throw new Exception("Unable to convert [" + settingValueAsString + "] to date.");
                        }

                        startDate = WebUtilities.ConvertFromClientToServerTimeZone(startDate);

                        //Check date value))
                        if (responseTemplate.BehaviorSettings.ActivationEndDate.HasValue &&
                            responseTemplate.BehaviorSettings.ActivationEndDate < startDate)
                        {
                            throw new Exception("Activation start date must be earlier than the activation end date.");
                        }

                        responseTemplate.BehaviorSettings.ActivationStartDate = startDate;

                        //Return to the client's time zone, because this value will be displayed for user.
                        startDate = WebUtilities.ConvertToClientTimeZone(startDate);

                        newValue = startDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                    }

                    break;

                case "activationenddate":
                    if (string.IsNullOrEmpty(settingValueAsString))
                    {
                        responseTemplate.BehaviorSettings.ActivationEndDate = null;
                        newValue = "";
                    }
                    else
                    {
                        var endDate = Utilities.GetDate(settingValueAsString);

                        if (!endDate.HasValue)
                        {
                            throw new Exception("Unable to convert [" + settingValueAsString + "] to date.");
                        }

                        endDate = WebUtilities.ConvertFromClientToServerTimeZone(endDate);

                        //Check date value
                        if (responseTemplate.BehaviorSettings.ActivationStartDate.HasValue &&
                            responseTemplate.BehaviorSettings.ActivationStartDate > endDate)
                        {
                            throw new Exception("Activation end date must be later than the activation start date.");
                        }

                        responseTemplate.BehaviorSettings.ActivationEndDate = endDate;

                        //Return to the client's time zone, because this value will be displayed for user.
                        endDate = WebUtilities.ConvertToClientTimeZone(endDate);

                        newValue = endDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                    }

                    break;

                case "maxtotalresponses":

                    var maxTotalValue = Utilities.AsInt(settingValueAsString);

                    if (!maxTotalValue.HasValue && settingValueAsString != "No Limit"
                        && !string.IsNullOrEmpty(settingValueAsString))
                    {
                        throw new Exception("Unable to convert [" + settingValueAsString + "] to number.");
                    }

                    if (maxTotalValue.HasValue && maxTotalValue <= 0)
                    {
                        throw new Exception("Max total responses must be greater than 0.");
                    }

                    newValue = maxTotalValue.HasValue ? maxTotalValue.ToString() : "No Limit";

                    responseTemplate.BehaviorSettings.MaxTotalResponses = maxTotalValue;

                    break;

                case "maxresponsesperuser":

                    var maxUserValue = Utilities.AsInt(settingValueAsString);

                    if (!maxUserValue.HasValue && settingValueAsString != "No Limit" 
                        && !string.IsNullOrEmpty(settingValueAsString))
                    {
                        throw new Exception("Unable to convert [" + settingValueAsString + "] to number.");
                    }

                    if (maxUserValue.HasValue && maxUserValue <= 0)
                    {
                        throw new Exception("Max total responses must be greater than 0.");
                    }

                    newValue = maxUserValue.HasValue ? maxUserValue.ToString() : "No Limit";

                    responseTemplate.BehaviorSettings.MaxResponsesPerUser = maxUserValue;

                    break;


                case "securitytype":
                    newValue = UpdateSecurityType(principal, responseTemplate, settingValueAsString);
                    break;

                case "password":
                    if (string.IsNullOrEmpty(settingValueAsString))
                    {
                        throw new Exception("No password provided");
                    }

                    responseTemplate.BehaviorSettings.Password = settingValueAsString.Trim();
                    newValue = responseTemplate.BehaviorSettings.Password;

                    break;
                
                case "styletemplateid" :

                    var styleTemplateId = Utilities.AsInt(settingValueAsString);

                    if (styleTemplateId.HasValue && styleTemplateId <= 0)
                        responseTemplate.StyleSettings.StyleTemplateId = null;
                    else if (!styleTemplateId.HasValue)
                        throw new Exception("Unable to convert [" + settingValueAsString + "] to number.");
                    else
                        responseTemplate.StyleSettings.StyleTemplateId = styleTemplateId;

                    newValue = styleTemplateId.ToString();
                    break;

                case "progressbarorientation":

                    int orientation;
                    if (int.TryParse(settingValueAsString, out orientation))
                    {
                        responseTemplate.StyleSettings.ProgressBarOrientation = (ProgressBarOrientation)orientation;
                        newValue = orientation.ToString();
                    }
                    else
                        newValue = ((int)responseTemplate.StyleSettings.ProgressBarOrientation).ToString();

                    break;

                case "mobilestyleid":

                    var mobileStyleId = Utilities.AsInt(settingValueAsString);

                    if (mobileStyleId.HasValue)
                    {
                        responseTemplate.StyleSettings.MobileStyleId = mobileStyleId <= 0 ?
                            default(int?) : mobileStyleId;
                    }
                    else  
                        throw new Exception("Unable to convert [" + settingValueAsString + "] to number.");

                    newValue = mobileStyleId.ToString();
                    break;

                case "languagesource":

                    responseTemplate.LanguageSettings.LanguageSource = settingValueAsString;
                    newValue = settingValueAsString;
                    break;

                case "languagesourcetoken":

                    responseTemplate.LanguageSettings.LanguageSourceToken = settingValueAsString;
                    newValue = settingValueAsString;
                    break;

                case "defaultlanguage":
                    responseTemplate.LanguageSettings.DefaultLanguage = settingValueAsString;
                    newValue = settingValueAsString;
                    break;

                case "googleanalyticstrackingid":
                    responseTemplate.BehaviorSettings.GoogleAnalyticsTrackingID = settingValueAsString;
                    newValue = settingValueAsString;
                    break;

                case "term":
                    var termDefinition = JsonConvert.DeserializeObject<SurveyTerm>(settingValueAsString);

                    if (termDefinition == null)
                    {
                        throw new Exception("Could not get parse term data");
                    }

                    if (termDefinition.Id == 0 || responseTemplate.SurveyTerms.FirstOrDefault(s => s.Id == termDefinition.Id) == null)
                    {
                        var id = responseTemplate.AddOrChangeSurveyTerm(termDefinition);
                        newValue = id.ToString();
                        termDefinition.Id = id;
                        responseTemplate.SurveyTerms.Add(termDefinition);
                    }
                    else
                    {
                        var indexInTerms = responseTemplate.SurveyTerms.FindIndex(s => s.Id == termDefinition.Id);
                        responseTemplate.SurveyTerms[indexInTerms] = termDefinition;
                        newValue = "0";
                    }

                    break;

                case "removeterm":
                    var termId = Utilities.AsInt(settingValueAsString);

                    if (termId == null)
                    {
                        throw new Exception("Could not get id of term");
                    }

                    responseTemplate.SurveyTerms.Remove(responseTemplate.SurveyTerms.First(s => s.Id == termId));

                    responseTemplate.DeleteSurveyTerm(termId.Value);

                    break;
                default:
                    throw new Exception("Updating setting with name [" + settingName +
                                        "] not allowed by SurveyEditorService.");
            }

            responseTemplate.ModifiedBy = principal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);

            return newValue;
        }

        /// <summary>
        /// Update survey security type, including making necessary changes to ACLs/Policies, etc.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="rt"></param>
        /// <param name="newSecurityType"></param>
        /// <returns></returns>
        private static string UpdateSecurityType(CheckboxPrincipal currentPrincipal, ResponseTemplate rt, string newSecurityType)
        {
            rt.BehaviorSettings.SecurityType = (SecurityType) Enum.Parse(typeof (SecurityType), newSecurityType);
            ResponseTemplateManager.InitializeResponseTemplateAclAndPolicy(rt, currentPrincipal);

            return rt.BehaviorSettings.SecurityType.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDateFormat(CheckboxPrincipal principal)
        {
            return GlobalizationManager.GetDateFormat();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetTimeFormat(CheckboxPrincipal principal)
        {
            return GlobalizationManager.GetTimeFormat();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="alternateUrl"></param>
        /// <returns></returns>
        public static string SetAlternateUrl(CheckboxPrincipal principal, int surveyId, string alternateUrl)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            if (!ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                throw new Exception("URL Rewriting not enabled.");
            }


            alternateUrl = alternateUrl.Trim();

            if (alternateUrl.Length > AlternateURLValidator.MaxLenght)
            {
                return "URL_TOO_LONG";
            }

            //Get URL for the survey.
            string surveyUrl = string.Format("{0}/Survey.aspx?s={1}",
                                             ApplicationManager.ApplicationRoot,
                                             responseTemplate.GUID.ToString().Replace("-", string.Empty));


            //Check to see if we're removing the URL
            if (string.IsNullOrEmpty(alternateUrl))
            {
                UrlMapper.RemoveMapping(surveyUrl);

                ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);

                return "URL_UPDATE_SUCCESS";
            }


            //Validate URL
            string shortUrl = string.Format("{0}/{1}",
                                            ApplicationManager.ApplicationRoot,
                                            alternateUrl);

            //See if a survey url is already mapped to this short url
            bool mappingExists = UrlMapper.SourceMappingExists(shortUrl);
            string mappedUrl = UrlMapper.GetMapping(shortUrl);

            //Make sure there are no special characters other than the underscore
            //Get url minus extension
            var urlWithoutExtension = alternateUrl.Replace(Path.GetExtension(alternateUrl) ?? string.Empty, string.Empty);
            var validator = new AlternateURLValidator();

            if (!validator.Validate(urlWithoutExtension))
            {
                return "URL_INVALID";
            }


            if (string.Compare(surveyUrl, mappedUrl, true) != 0 && mappingExists)
            {
                return "URL_MAPPING_IN_USE";
            }

            //Make sure the mapping does not point to an existing file
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + alternateUrl))
            {
                return "URL_FILE_EXISTS";
            }

            //Save mapping
            UrlMapper.AddMapping(shortUrl, surveyUrl);

            //Mark template updated
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);

            return "URL_UPDATE_SUCCESS";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="styleTemplateId"></param>
        /// <returns></returns>
        public static int SetStyleTemplate(CheckboxPrincipal principal, int surveyId, int styleTemplateId, string styleType)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Administer");

            StyleTemplateType type = (StyleTemplateType)Enum.Parse(typeof(StyleTemplateType), styleType);
            switch (type)
            {
                case StyleTemplateType.PC :
                    responseTemplate.StyleSettings.StyleTemplateId = styleTemplateId > 0 ? styleTemplateId : (int?)null;
                    break;
                case StyleTemplateType.Tablet:
                    responseTemplate.StyleSettings.TabletStyleTemplateId = styleTemplateId > 0 ? styleTemplateId : (int?)null;
                    break;
                case StyleTemplateType.SmartPhone:
                    responseTemplate.StyleSettings.SmartPhoneStyleTemplateId = styleTemplateId > 0 ? styleTemplateId : (int?)null;
                    break;
            }
            responseTemplate.ModifiedBy = principal.Identity.Name;
            responseTemplate.Save();
            ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);

            return styleTemplateId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <param name="alternateLanguages"></param>
        /// <returns></returns>
        public static string GetSurveyText(string textKey, int surveyId, string languageCode, params string[] alternateLanguages)
        {
            //No key or survey id = do nothing
            if (string.IsNullOrEmpty(textKey) || surveyId <= 0)
            {
                return string.Empty;
            }

            //Get text
            var textValue = TextManager.GetText(GetSurveySpecificTextId(textKey, surveyId), languageCode, string.Empty, new string[] { });

            //If no value found, check system-wide settings
            if (string.IsNullOrEmpty(textValue) && ResponseTemplateManager.SurveyTextKeyMap.ContainsKey(textKey))
            {
                textValue = TextManager.GetText(ResponseTemplateManager.SurveyTextKeyMap[textKey], languageCode, string.Empty, alternateLanguages);    
            }

            return textValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <param name="alternateLanguages"></param>
        /// <returns></returns>
        private static string GetSurveyTextDescription(string textKey, int surveyId, string languageCode, params string[] alternateLanguages)
        {
            //Special case for title which should have survey name by default
            var textId = textKey.Equals("title", StringComparison.InvariantCultureIgnoreCase) 
                ? "/pageText/responseTemplate.cs/titleDefaultText/description"
                : ResponseTemplateManager.SurveyTextKeyMap[textKey] + "/description";


            return TextManager.GetText(textId, languageCode, textKey, alternateLanguages);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        private static string GetSurveySpecificTextId(string textKey, int surveyId)
        {
            //Generate text id
            return string.Format("/templateData/{0}/{1}", surveyId, textKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode">s</param>
        /// <returns></returns>
        public static LocalizedTextValue[] GetSurveyTexts(CheckboxPrincipal principal, int surveyId, string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                throw new Exception("Language code not specified for call to GetSurveyTexts");    
            }

            if (!ValidateLanguageCode(languageCode))
            {
                throw new Exception("Language is not valid for call to GetSurveyTexts");
            }

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            var altLanguages = responseTemplate.LanguageSettings.SupportedLanguages.ToArray();

            return
                ResponseTemplateManager.SurveyTextKeyMap
                    .Keys
                    .Select(key => new LocalizedTextValue()
                                       {
                                           LanguageCode = languageCode,
                                           TextId = key,
                                           Value = GetSurveyText(key, surveyId, languageCode, altLanguages),
                                           Name = GetSurveyTextDescription(key, surveyId, WebTextManager.GetUserLanguage(), TextManager.ApplicationLanguages)
                                       }
                    ).ToArray();


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static QuestionResult WillRulesBeChangedIfPageDeletes(CheckboxPrincipal principal, int surveyId, int pageId)
        {
            var result = new QuestionResult();

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");            

            result.Yes = responseTemplate.WillRulesBeChangedIfPageDeletes(pageId);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="textKey"></param>
        /// <param name="textValue"></param>
        /// <param name="languageCode"></param>
        public static void UpdateSurveyText(CheckboxPrincipal principal, int surveyId, string textKey, string textValue, string languageCode)
        {
            if (string.IsNullOrEmpty(textKey))
            {
                throw new Exception("TextKey not specified for call to UpdateSurveyText");
            }

            if (!ResponseTemplateManager.SurveyTextKeyMap.ContainsKey(textKey))
            {
                throw new Exception("TextKey [" + textKey + "] not supported.");
            }

            if(string.IsNullOrEmpty(languageCode))
            {
                throw new Exception("Language code not specified for call to UpdateSurveyText");
            }

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Administer");


            TextManager.SetText(GetSurveySpecificTextId(textKey, surveyId), languageCode, textValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool SetDefaultLanguage(CheckboxPrincipal principal, int surveyId, string language)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            responseTemplate.LanguageSettings.DefaultLanguage = language;
            responseTemplate.ModifiedBy = principal.Identity.Name;
            responseTemplate.Save();

            return true;
        }

        /// <summary>
        /// Add specified language to the list of survey default languages
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool AddDefaultLanguage(CheckboxPrincipal principal, int surveyId, string language)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate != null)
            {
                responseTemplate.AddSupportedLanguage(language);
                responseTemplate.ModifiedBy = principal.Identity.Name;
                responseTemplate.Save();

                ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove specified language from the list of survey default languages
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static bool RemoveDefaultLanguage(CheckboxPrincipal principal, int surveyId, string language)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate != null)
            {
                responseTemplate.RemoveSupportedLanguage(language);
                responseTemplate.ModifiedBy = principal.Identity.Name;
                responseTemplate.Save();

                ResponseTemplateManager.MarkTemplateUpdated(responseTemplate.ID.Value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Toggle item 'active' status
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="principal"></param>
        public static void ToggleItemActiveStatus(CheckboxPrincipal principal, int surveyId, int itemId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            ItemData item = ItemConfigurationManager.GetConfigurationData(itemId);

            item.IsActive = !item.IsActive;

            item.Save();

            ItemConfigurationManager.InvalidateCachedItemData(itemId);

            //Update template
            responseTemplate.SetItem(item, true);

            //Cleanup caches of the item
            responseTemplate.CleanupItemCaches(itemId);
        }

        /// <summary>
        /// Returns item 'is active' status
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="principal"></param>
        public static bool GetItemIsActive(CheckboxPrincipal principal, int surveyId, int itemId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            ItemData item = ItemConfigurationManager.GetConfigurationData(itemId);

            return item.IsActive;
        }

        /// <summary>
        /// Check if 'copy action' is available for item
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool CopyActionIsAvailableForItem(CheckboxPrincipal principal, int surveyId, int itemId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            ItemData item = ItemConfigurationManager.GetConfigurationData(itemId);

            return item.ItemTypeName != "HiddenItem";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static string GetStatus(CheckboxPrincipal principal, int surveyId)
        {
            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to find response template with id [" + surveyId + "].");
            }

            Security.AuthorizeUserContext(principal, responseTemplate, "Form.Edit");

            bool isActive = responseTemplate.BehaviorSettings.IsActive;
            string status = string.Empty;
            string action = string.Empty;
            DateTime? date = null;

            DateTime now = DateTime.Now;

            if (responseTemplate.BehaviorSettings.ActivationStartDate.HasValue 
                    && now < responseTemplate.BehaviorSettings.ActivationStartDate)
            {
                action = TextManager.GetText("/pageText/forms/surveys/surveySettings.aspx/startingOn");
                date = WebUtilities.ConvertToClientTimeZone(responseTemplate.BehaviorSettings.ActivationStartDate);
            }
            else if (responseTemplate.BehaviorSettings.ActivationEndDate.HasValue 
                && now < responseTemplate.BehaviorSettings.ActivationEndDate)
            {
                action = TextManager.GetText("/pageText/forms/surveys/surveySettings.aspx/endingOn");
                date = WebUtilities.ConvertToClientTimeZone(responseTemplate.BehaviorSettings.ActivationEndDate);
            }

            if (isActive && !string.IsNullOrEmpty(action) && date.HasValue)
                status = string.Format("{0} {1}", action, GlobalizationManager.FormatTheDate(date.Value));
            else
                status = "";
            

            return status;
        }

        /// <summary>
        /// Update single style template setting
        /// </summary>
        /// <param name="checkboxPrincipal"></param>
        /// <param name="styleId"></param>
        /// <param name="settingName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string UpdateStyleTemplateSetting(CheckboxPrincipal checkboxPrincipal, int styleId, string settingName, string newValue)
        {
            
            StyleTemplate styleTemplate = StyleTemplateManager.GetStyleTemplate(styleId);

            if (styleTemplate == null)
            {
                throw new Exception("Unable to find style with id [" + styleId + "].");
            }

            switch (settingName)
            {
                case "Name":
                    if (!StyleTemplateManager.IsStyleNameInUse(newValue))
                    {
                        styleTemplate.Name = newValue;
                        break;
                    }
                    return styleTemplate.Name;
            }

            StyleTemplateManager.SaveTemplate(styleTemplate, checkboxPrincipal);
            return newValue;
        }

        private static bool ValidateLanguageCode(string language)
        {
            return (!string.IsNullOrEmpty(language) &&
                    TextManager.SurveyLanguages.Any(l => l.ToLower().Trim().Equals(language.ToLower().Trim())));
        }
    }
}
