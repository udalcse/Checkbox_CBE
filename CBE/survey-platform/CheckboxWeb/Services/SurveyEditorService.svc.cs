using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// Service implementation for survey editor service.  Essentially proxies calls to implementation class
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SurveyEditorService : ISurveyEditorService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="maxPagePosition"></param>
        /// <param name="languageCode"></param>
        /// <param name="customFieldTypeId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PipeSource[]> ListPipeSources(string authToken, int surveyId, int maxPagePosition, string languageCode, int customFieldTypeId)
        {
            try
            {
                return new ServiceOperationResult<PipeSource[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.ListPipeSources(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, maxPagePosition, languageCode, customFieldTypeId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PipeSource[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PipeSource[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Returns profile property keys along with their ids
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="customFieldTypeId"></param>
        /// <returns></returns>
        //public ServiceOperationResult<Dictionary<int, string>> ListProfileKeys(int customFieldTypeId)
        //{
        //    try
        //    {
        //        return new ServiceOperationResult<Dictionary<int, string>>
        //        {
        //            CallSuccess = true,
        //            ResultData = SurveyEditorServiceImplementation.GetProfilePropertiesNames(customFieldTypeId)
        //        };
        //    }
        //    catch (NoUserException ex)
        //    {
        //        return new ServiceOperationResult<Dictionary<int, string>>
        //        {
        //            CallSuccess = false,
        //            FailureMessage = AuthenticationService.NoUserMessage
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionPolicy.HandleException(ex, "Service");

        //        return new ServiceOperationResult<Dictionary<int, string>>
        //        {
        //            CallSuccess = false,
        //            FailureMessage = ex.Message
        //        };
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="listId"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> ListAutocompleteListData(string authToken, int listId)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.ListAutocompleteListData(AuthenticationService.GetCurrentPrincipal(authToken), listId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> ToggleSetting(string authToken, int surveyId, string settingName)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.ToggleSurveySetting(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, settingName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValueAsString"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> UpdateSetting(string authToken, int surveyId, string settingName, string settingValueAsString)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.UpdateSurveySetting(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, settingName, settingValueAsString)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="value"> </param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateConditionSource(string authToken, int surveyId, string value)
        {
            try
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.UpdateConditionSource(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, value)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetDateFormat(string authToken)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.GetDateFormat(AuthenticationService.GetCurrentPrincipal(authToken))
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetTimeFormat(string authToken)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.GetTimeFormat(AuthenticationService.GetCurrentPrincipal(authToken))
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> ToggleItemActiveStatus(string authToken, int surveyId, int itemId)
        {
            try
            {
                SurveyEditorServiceImplementation.ToggleItemActiveStatus(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId);
                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetStatus(string authToken, int surveyId)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.GetStatus(AuthenticationService.GetCurrentPrincipal(authToken), surveyId)
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Check if 'copy action' is available for item
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> CopyActionIsAvailableForItem(string authToken, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.CopyActionIsAvailableForItem(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId)
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> GetItemIsActive(string authToken, int surveyId, int itemId)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.GetItemIsActive(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, itemId)
                };
            }
            catch (NoUserException)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="altUrl"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> SetAlternateUrl(string authToken, int surveyId, string altUrl)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.SetAlternateUrl(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, altUrl)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<string>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="styleTemplateId"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> SetStyleTemplate(string authToken, int surveyId, int styleTemplateId, string styleType)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.SetStyleTemplate(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, styleTemplateId, styleType)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<LocalizedTextValue[]> GetLocalizableTexts(string authToken, int surveyId, string languageCode)
        {
            try
            {
                return new ServiceOperationResult<LocalizedTextValue[]>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.GetSurveyTexts(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, languageCode)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<LocalizedTextValue[]>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<LocalizedTextValue[]>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<QuestionResult> WillRulesBeChangedIfPageDeletes(string authToken, int surveyId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<QuestionResult>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.WillRulesBeChangedIfPageDeletes(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, pageId)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<QuestionResult>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<QuestionResult>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="textKey"></param>
        /// <param name="textValue"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> UpdateSurveyText(string authToken, int surveyId, string textKey, string textValue, string languageCode)
        {
            try
            {
                SurveyEditorServiceImplementation.UpdateSurveyText(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, textKey, textValue, languageCode);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true,
                    ResultData = null
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<SurveyItemMetaData> GetItemMetadata(int itemId)
        {
            throw new NotImplementedException();
        }

        public ServiceOperationResult<bool> UpdateItemMetadata(int itemId, ItemMetaData itemMetaData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> SetDefaultLanguage(string authToken, int surveyId, string language)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.SetDefaultLanguage(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, language)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Add specified language to the list of survey default languages
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> AddDefaultLanguage(string authToken, int surveyId, string language)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.AddDefaultLanguage(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, language)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Remove specified language from the list of survey default languages
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="surveyId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> RemoveDefaultLanguage(string authToken, int surveyId, string language)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.RemoveDefaultLanguage(AuthenticationService.GetCurrentPrincipal(authToken), surveyId, language)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }
    }
}
