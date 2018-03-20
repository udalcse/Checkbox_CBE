using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ResponseSelectBase : Common.UserControlBase
    {
        private List<string> _reservedParams = new List<string>
                                               {
                                                   "i","s","u","r","iid","preview","forceNew"
                                               };

        /// <summary>
        /// 
        /// </summary>
        protected Guid? AuthenticatedUserGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="authenticatedUserGuid"></param>
        /// <param name="languageCode"></param>
        /// <param name="listResponsesCallback"></param>
        /// <param name="moreResponsesAllowedCallback"></param>
        public virtual void Initialize(ResponseSessionData sessionData, Guid? authenticatedUserGuid,
                                        string languageCode, Func<Guid, SurveyResponseData[]> listResponsesCallback,
                                        Func<Guid, bool> moreResponsesAllowedCallback)
        {
            AuthenticatedUserGuid = authenticatedUserGuid;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="authenticatedUserGuid"></param>
        protected string GetNewResponseLinkUrl(ResponseSessionData sessionData, Guid? authenticatedUserGuid)
        {
            var baseUrl = ResolveUrl("~/Survey.aspx?forceNew=true") + GetQueryStringParameters();

            //Attempt to preserve invitation guid, if any
            if (sessionData.InvitationRecipientGuid.HasValue)
            {
                return baseUrl + "&i=" + sessionData.InvitationRecipientGuid;
            }

            if (authenticatedUserGuid.HasValue)
            {
                baseUrl = baseUrl + "&u=" + authenticatedUserGuid;
            }

            if (sessionData.ResponseTemplateGuid.HasValue)
            {
                return baseUrl + "&s=" + sessionData.ResponseTemplateGuid;
            }

            return baseUrl + "&s=" + sessionData.ResponseTemplateId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetQueryStringParameters()
        {
            var param = string.Empty;
            foreach (var key in Page.Request.QueryString.AllKeys)
            {
                if(!_reservedParams.Contains(key))
                    param += string.Format("&{0}={1}", key, Request.QueryString[key]);
            }

            return param;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetResumeLink(IDataItemContainer itemContainer)
        {
            var responseGuid = DataBinder.Eval(itemContainer.DataItem, "ResponseGuid") as Guid?;
            var workflowId = DataBinder.Eval(itemContainer.DataItem, "WorkflowInstanceId") as Guid?;

            return GetResumeLink(responseGuid, workflowId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetResumeLink(Guid? responseGuid, Guid? workflowId)
        {
            if (workflowId != null)
            {
                return ApplicationManager.ApplicationRoot + "/Survey.aspx?preview=true&iid=" + workflowId + getInvitationParam();
            }

            if (responseGuid != null)
            {
                return ApplicationManager.ApplicationRoot + "/Survey.aspx?r=" + responseGuid + getInvitationParam();
            }

            return string.Empty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string getInvitationParam()
        {
            if (!string.IsNullOrEmpty(Page.Request["i"]))
            {
                return "&i=" + Page.Request["i"];
            }
            return "";
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetEditLink(IDataItemContainer itemContainer)
        {
            var responseGuid = DataBinder.Eval(itemContainer.DataItem, "ResponseGuid") as Guid?;

            return GetEditLink(responseGuid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetEditLink(Guid? responseGuid)
        {
            var surveyUrl = ApplicationManager.ApplicationRoot + "/Survey.aspx?edit=true" + getInvitationParam();

            if (AuthenticatedUserGuid.HasValue)
            {
                surveyUrl += "&u=" + AuthenticatedUserGuid;
            }

            if (responseGuid != null)
            {
                return surveyUrl + "&r=" + responseGuid;
            }

            return string.Empty;
        }
    }
}
