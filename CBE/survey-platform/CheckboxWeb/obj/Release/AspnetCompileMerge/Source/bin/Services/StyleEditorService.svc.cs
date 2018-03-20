using System;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// Service implementation for style editor service
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class StyleEditorService : IStyleEditorService
    {

        #region IStyleEditorService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public ServiceOperationResult<StyleMetaData> GetStyleData(string authTicket, int styleId)
        {
            try
            {
                return new ServiceOperationResult<StyleMetaData>
                {
                    CallSuccess = true,
                    ResultData = StyleEditorServiceImplementation.GetStyleData(AuthenticationService.GetCurrentPrincipal(authTicket), styleId)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<StyleMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<StyleMetaData>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="elementName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> GetStyleElementProperty(string authTicket, int styleId, string elementName, string property)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = StyleEditorServiceImplementation.GetStyleElementProperty(AuthenticationService.GetCurrentPrincipal(authTicket), styleId, elementName, property)
                };
            }
            catch(NoUserException ex)
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
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public ServiceOperationResult<SimpleNameValueCollection> GetStyleElementProperties(string authTicket, int styleId, string elementName)
        {
            try
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = true,
                    ResultData = StyleEditorServiceImplementation.GetStyleElementProperties(AuthenticationService.GetCurrentPrincipal(authTicket), styleId, elementName)
                };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<SimpleNameValueCollection>
                {
                    CallSuccess = false,
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="languageCode"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> SaveFormStyle(string authTicket, int styleId, string languageCode, StyleData style)
        {
            try
            {
                return new ServiceOperationResult<bool>
                           {
                               CallSuccess = true,
                               ResultData =
                                   StyleEditorServiceImplementation.SaveFormStyle(AuthenticationService.GetCurrentPrincipal(authTicket), styleId, languageCode, style)
                           };
            }
            catch(NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch(Exception ex)
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
        /// Update single style template setting
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="styleId"></param>
        /// <param name="settingName"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public ServiceOperationResult<string> UpdateStyleTemplateSetting(string authToken, int styleId, string settingName,  string newValue)
        {
            try
            {
                return new ServiceOperationResult<string>
                {
                    CallSuccess = true,
                    ResultData = SurveyEditorServiceImplementation.UpdateStyleTemplateSetting(AuthenticationService.GetCurrentPrincipal(authToken), styleId, settingName, newValue)
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
        #endregion
    }
}
