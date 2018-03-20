using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Checkbox.Analytics;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Newtonsoft.Json;
using Prezza.Framework.Caching;
using Newtonsoft.Json.Linq;

namespace Checkbox.Forms.Workflow.RSM
{
    /// <summary>
    /// Response State Machine operates for one response only
    /// </summary>
    [Serializable]
    public class ResponseStateMachine : StateMachine<RSMState>
    {
        /// <summary>
        /// Constructor initializes basic properties
        /// </summary>
        /// <param name="template"></param>
        /// <param name="session"></param>
        /// <param name="respondent"></param>
        public ResponseStateMachine(ResponseTemplate template, ResponseSessionData session, CheckboxPrincipal respondent)
        {
            ResponseTemplate = template;
            ResponseSessionData = session;
            Respondent = respondent;
        }

        private ResponseTemplate _responseTemplate;

        private int? _responseTemplateID;
        
        /// <summary>
        /// Survey to run
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                return _responseTemplate;
            }
            private set
            {
                _responseTemplate = value;
                _responseTemplateID = _responseTemplate.ID.Value;
            }
        }

        /// <summary>
        /// Session data for the current session
        /// </summary>
        public ResponseSessionData ResponseSessionData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Current response
        /// </summary>
        public Response Response
        {
            get;
            internal set;
        }

        /// <summary>
        /// Respondent data
        /// </summary>
        public CheckboxPrincipal Respondent 
        { 
            get; 
            internal set; 
        }


        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public override void CleanupBeforeCaching()
        {
            _responseTemplate = null;
        }

        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public override void RestoreAfterCaching(CacheContext cacheContext)
        {
            if (cacheContext != null)
            {
                _responseTemplate = cacheContext.ResponseTemplate;
                _responseTemplateID = cacheContext.ResponseTemplate.ID.Value;
            }

            if (_responseTemplate == null && _responseTemplateID.HasValue)
            {
                _responseTemplate = ResponseTemplateManager.GetResponseTemplate(_responseTemplateID.Value);
            }
        }

        /// <summary>
        /// Initialize state machine by creating a new response
        /// </summary>
        public void InitializeAsNew()
        {
            Response = ResponseTemplate.CreateResponse(ResponseSessionData.SelectedLanguage, null, true);

            Response.Initialize(
                ResponseSessionData.RespondentIpAddress,
                ResponseSessionData.NetworkUser,
                ResponseSessionData.SelectedLanguage,
                ResponseSessionData.IsTest,
                ResponseSessionData.Invitee,
                Respondent,
                ResponseSessionData.SessionGuid);

            Response.InitializeItemsDefaults();

            ResponseSessionData.ResponseGuid = Response.GUID;
            Initialize(RSMState.InProgress);
        }

        public void OnResponseSaved(object sender, ResponseStateEventArgs e)
        {
            Response response = (Response)sender;
            if (null == response || null == response.Respondent) { return; }

            UpdatePartnerLinkStatus(response, "STARTED");
        }

        public void OnResponseCompleted(object sender, ResponseStateEventArgs e)
        {
            Response response = (Response)sender;
            if (null == response || null == response.Respondent) { return; }

            UpdatePartnerLinkStatus(response, "COMPLETED");
        }

        private void UpdatePartnerLinkStatus(Response response, string status)
        {
            string partnerLinkApiRoot = ConfigurationManager.AppSettings["PartnerLinkApiRoot"];
            if (String.IsNullOrEmpty(partnerLinkApiRoot) || partnerLinkApiRoot.Equals("UNSET"))
            {
                partnerLinkApiRoot = "http://localhost:5000";
            }
            HttpWebRequest surveyLookupRequest = (HttpWebRequest)HttpWebRequest.Create(partnerLinkApiRoot + "/api/surveys?engaugeSurveyId=" + response.ResponseTemplateID + "_" + ConfigurationManager.AppSettings["EnvironmentName"]);
            surveyLookupRequest.Method = "GET";
            surveyLookupRequest.Accept = "application/json";
            HttpWebResponse surveyLookupResponse = null;
            Char[] readBuffer = new Char[256];
            try
            {
                surveyLookupResponse = (HttpWebResponse)surveyLookupRequest.GetResponse();
                if (surveyLookupResponse.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                Stream stream = surveyLookupResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                int count = streamReader.Read(readBuffer, 0, 256);
                while (count > 0)
                {
                    string outputData = new String(readBuffer, 0, count);
                    count = streamReader.Read(readBuffer, 0, 256);
                }
                // Release the response object resources.
                streamReader.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                return;
            }
            finally
            {
                if (surveyLookupResponse != null)
                {
                    surveyLookupResponse.Close();
                }
            }

            string surveyResponse = new string(readBuffer);
            string surveyResponseTrimmed = surveyResponse.Trim('\0');
            var parsed = JObject.Parse(surveyResponseTrimmed);
            var token = parsed.SelectToken("$.id");
            string surveyId = JObject.Parse(surveyResponseTrimmed).SelectToken("$.id").ToString();

            object userResponseStatus = new
            {
                EngaugeSurveyId = response.ResponseTemplateID.ToString() + "_" + ConfigurationManager.AppSettings["EnvironmentName"],
                EngaugeUsername = response.Respondent.Identity.Name,
                Status = status,
                ResponseDate = DateTime.UtcNow
            };

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(partnerLinkApiRoot + "/api/surveys/" + surveyId + "/status");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.SendChunked = true;
            string jsonPayload = JsonConvert.SerializeObject(userResponseStatus);
            byte[] postData = System.Text.ASCIIEncoding.UTF8.GetBytes(jsonPayload);
            webRequest.ContentLength = postData.Length;
            Stream postDataStream = webRequest.GetRequestStream();
            postDataStream.Write(postData, 0, postData.Length);
            postDataStream.Close();

            HttpWebResponse webResponse = null;

            try {
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                Stream stream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                readBuffer = new Char[256];
                int count = streamReader.Read(readBuffer, 0, 256);
                while (count > 0)
                {
                    string outputData = new String(readBuffer, 0, count);
                    count = streamReader.Read(readBuffer, 0, 256);
                }
                // Release the response object resources.
                streamReader.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                // do nothing
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        /// <summary>
        /// Initialize state machine by loading an existing response
        /// </summary>
        /// <param name="resume"></param>
        /// <param name="isStarted">use to split situations when the response is resuming or editing (true) 
        ///  and when it's not been started yet and just restoring after language selection or similar</param>
        public void InitializeByResponseGuid(bool resume, bool isStarted)
        {
            //Otherwise, load response and reset to first content page
            //If resuming/editing via response guid, load that response directly.
            LoadResponseData(ResponseTemplate, ResponseSessionData.ResponseGuid.Value, isStarted);

            if (!resume)
            {
                //Go to first page
                Response.MoveToStart();
            }

            Response.InitializeItemsDefaults();

            Initialize(RSMState.InProgress);
        }

        /// <summary>
        /// Sets language for the started survey
        /// </summary>
        /// <param name="languageCode"></param>
        public void SetLanguage(string languageCode)
        {
            ResponseSessionData.SelectedLanguage = languageCode;
            if (Response != null && Response.GUID != null && ResponseTemplate != null)
            {
                LoadResponseData(ResponseTemplate, Response.GUID.Value, true);
            }
        }

        public void UpdateLanguage(string languageCode)
        {
            if (Response != null)
                Response.LanguageCode = languageCode;
        }

        /// <summary>
        /// Load response data from the database
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="responseGuid"></param>
        /// <param name="isStarted"></param>
        private void LoadResponseData(ResponseTemplate rt, Guid responseGuid, bool isStarted)
        {
            if (string.IsNullOrEmpty(ResponseSessionData.SelectedLanguage))
            {
                var responseData = ResponseManager.GetResponseData(responseGuid);
                if (responseData != null)
                    ResponseSessionData.SelectedLanguage = responseData.ResponseLanguage;
            }

            var language = string.IsNullOrEmpty(ResponseSessionData.SelectedLanguage)
                ? ResponseTemplate.LanguageSettings.DefaultLanguage
                : ResponseSessionData.SelectedLanguage;

            Response = rt.CreateResponse(language, responseGuid, true);

            try
            {
                Response.Restore(ResponseStateManager.GetResponseState(responseGuid), isStarted);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Survey [{0}] does not have a response {1}", rt.Name, responseGuid.ToString()), ex);
            }

            if (Response.WorkflowSessionId.HasValue)
                ResponseSessionData.ResumeInstanceId = Response.WorkflowSessionId.Value;

            ResponseSessionData.SelectedLanguage = string.IsNullOrEmpty(Response.LanguageCode) ? language : Response.LanguageCode;
            ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
        }

        /// <summary>
        /// Get page info.
        /// </summary>
        /// <param name="pageId">If NULL the method returns the current page</param>
        /// <returns></returns>
        public SurveyResponsePage GetPage(int? pageId)
        {
            //force reload conditions and rules
            if (!pageId.HasValue)
                Response.LoadCurrentPage();

            SurveyResponsePage result = null;
            var requestedPage = pageId.HasValue ? Response.GetPage(pageId.Value) : Response.CurrentPage;

            if (requestedPage != null)
            {
                result = requestedPage.GetDataTransferObject();
                result.ItemNumbers = Response.GetItemNumbers(requestedPage);                
                result.IsLastContentPage = Response.CompleteOnNext && requestedPage.PageType == TemplatePageType.ContentPage;
                result.IsFirstContentPage = Response.CurrentPage.Position <= 2;
            }

            return result;
        }


        public void SetPage(int? pageId)
        {
            Response.SetPage(pageId);
        }

        /// <summary>
        /// Get page info.
        /// </summary>
        /// <param name="pageId">If NULL the method returns the current page</param>
        /// <returns></returns>
        public List<SurveyResponsePage> GetPages()
        {

            List<SurveyResponsePage> results = new List<SurveyResponsePage>();
   
            var requestedPages = Response.GetPages();

            foreach (var item in requestedPages)
            {
                var result = new SurveyResponsePage();
                result = item.GetDataTransferObject();
                result.ItemNumbers = Response.GetItemNumbers(item);
                result.IsLastContentPage = Response.CompleteOnNext && item.PageType == TemplatePageType.ContentPage;
                result.IsFirstContentPage = Response.CurrentPage.Position <= 2;
                results.Add(result);
              
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, bool> GetVisitedPageVisibilities()
        {
            var result = new Dictionary<int, bool>();

            if (Response != null && Response.VistedPageStack != null)
            {
                var visited = Response.VistedPageStack.ToArray();
                foreach (var page in visited)
                {
                    result.Add(
                        page.PageId, 
                        page.PageType != TemplatePageType.HiddenItems &&
                        !page.Excluded &&
                        !page.ExcludedNoItems);
                }
            }

            return result;
        }


        /// <summary>
        /// Get items' data for all the Ids passed in the collection
        /// </summary>
        /// <param name="itemIds"></param>
        /// <returns></returns>
        public ItemProxyObject[] GetItemDatas(IEnumerable<int> itemIds)
        {
            var resultList = (from itemID in itemIds select Response.GetItem(itemID) into item where item != null select item.GetDataTransferObject() as ItemProxyObject).ToList();

            return resultList.Where(item => item != null).ToArray();
        }

        /// <summary>
        /// Get a current page number
        /// </summary>
        /// <returns></returns>
        public PageNumberInfo GetPageNumber()
        {
            var PageNumberInfo = new PageNumberInfo
                                     {
                                         CurrentPageNumber = Response.GetCurrentPageDisplayNumber(),
                                         TotalPageCount = Response.GetPageCountDisplayNumber(),
                                         FirstItemNumber = Response.ShowItemNumbers ? Response.GetFirstItemNumber() : 0
                                     };
            return PageNumberInfo;
        }


        #region Caching
        public void Cache(CacheManager cacheManager)
        {
            var response = Response.GetCachable();
            cacheManager.Add("Response_" + ResponseSessionData.SessionGuid, response);
            cacheManager.Add("RSMState_" + ResponseSessionData.SessionGuid, _currentState);
        }

        public static ResponseStateMachine GetFromCache(CacheManager cacheManager, Guid key, ResponseTemplate rt, ResponseSessionData rsd, CheckboxPrincipal respondent)
        {
            var rsm = new ResponseStateMachine(rt, rsd, respondent);
            rsm._currentState = (State<RSMState>)cacheManager.GetData("RSMState_" + key);
            rsm.ResponseSessionData = rsd;
            rsm.Respondent = respondent;

            rsm.Update(cacheManager, key);

            return rsm;
        }

        public void Update(CacheManager cacheManager, Guid key)
        {
            var cachableResponse = cacheManager.GetData("Response_" + key) as CachableResponse;
            Response = ResponseTemplate.InitializeResponseFromCached(cachableResponse);
        }

        #endregion Caching

    }
}
