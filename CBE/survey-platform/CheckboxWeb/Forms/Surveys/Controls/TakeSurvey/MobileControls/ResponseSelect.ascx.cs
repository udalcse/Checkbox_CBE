using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Globalization;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.TakeSurvey;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls
{
    public partial class ResponseSelect : ResponseSelectBase
    {
        private IEnumerable<SurveyResponseData> _resumeResponses;
        private IEnumerable<SurveyResponseData> _editResponses;

        public override void Initialize(ResponseSessionData sessionData, Guid? authenticatedUserGuid, string languageCode, Func<Guid, SurveyResponseData[]> listResponsesCallback, Func<Guid, bool> moreResponsesAllowedCallback)
        {
            base.Initialize(sessionData, authenticatedUserGuid, languageCode, listResponsesCallback, moreResponsesAllowedCallback);

            if (listResponsesCallback == null || moreResponsesAllowedCallback == null)
                return;

            //fill resume/edit response links
            var responseList = listResponsesCallback(sessionData.SessionGuid);

            ResumeResponses = responseList.Where(responseData => !responseData.DateCompleted.HasValue)
                .OrderByDescending(responseData => responseData.DateLastEdited).Take(20).OrderBy(responseData => responseData.DateLastEdited);

            EditResponses = responseList.Where(responseData => responseData.DateCompleted.HasValue);

            FillResume();
            FillEdit();

            //start new response button
            var startNew = new MultiLanguageHyperLink
                               {
                                   Text = StartNewText,
                                   CssClass = "workflowAjaxGetAction",
                                   NavigateUrl = GetNewResponseLinkUrl(sessionData, authenticatedUserGuid)
                               };
            startNew.Attributes["data-role"] = "button";

            if (ResumeResponses.Count() + EditResponses.Count() > 5)
                _startNewBottomPlaceholder.Controls.Add(startNew);
            else
                _startNewTopPlaceholder.Controls.Add(startNew);
        }

        protected void FillResume()
        {
            foreach (var responseData in ResumeResponses)
            {
                if (responseData.DateStarted.HasValue)
                {
                    var link = new MultiLanguageHyperLink
                                   {
                                       Text = GetResumeText(responseData.DateStarted.Value),
                                       CssClass = "workflowAjaxGetAction",
                                       NavigateUrl = GetResumeLink(responseData.ResponseGuid,
                                                                   responseData.WorkflowInstanceId)
                                   };
                    link.Attributes["data-role"] = "button";
                    
                    _resumePanel.Controls.Add(link);
                }
            }
        }

        protected void FillEdit()
        {
            foreach (var responseData in EditResponses)
            {
                if (responseData.DateStarted.HasValue)
                {
                    var link = new MultiLanguageHyperLink
                                   {
                                       Text = GetEditText(responseData.DateStarted.Value),
                                       CssClass = "workflowAjaxGetAction",
                                       NavigateUrl = GetEditLink(responseData.ResponseGuid)
                                   };
                    link.Attributes["data-role"] = "button";

                    _editPanel.Controls.Add(link);
                }
            }
        }

        protected IEnumerable<SurveyResponseData> ResumeResponses
        {
            set { _resumeResponses = value; }
            get
            {
                return _resumeResponses ?? (_resumeResponses = new List<SurveyResponseData>());
            }
        }

        protected IEnumerable<SurveyResponseData> EditResponses
        {
            set { _editResponses = value; }
            get
            {
                return _editResponses ?? (_editResponses = new List<SurveyResponseData>());
            }
        }

        protected string GetResumeText(DateTime started)
        {
            string format = TextManager.GetText("/pageText/responseSelectMobile.aspx/resume");
            return string.Format(format, GlobalizationManager.FormatTheDate(started));
        }

        protected string GetEditText(DateTime completed)
        {
            string format = TextManager.GetText("/pageText/responseSelectMobile.aspx/edit");
            return string.Format(format, GlobalizationManager.FormatTheDate(completed));
        }

        protected string StartNewText
        {
            get { return TextManager.GetText("/pageText/responseSelectMobile.aspx/new"); }
        }
    }
}