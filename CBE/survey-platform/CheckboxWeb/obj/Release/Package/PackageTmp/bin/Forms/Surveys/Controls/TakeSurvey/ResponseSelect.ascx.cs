using System;
using System.Linq;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.TakeSurvey;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ResponseSelect : ResponseSelectBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="authenticatedUserGuid"></param>
        /// <param name="languageCode"></param>
        /// <param name="listResponsesCallback"></param>
        /// <param name="moreResponsesAllowedCallback"></param>
        public override void Initialize(ResponseSessionData sessionData, Guid? authenticatedUserGuid, string languageCode, Func<Guid, SurveyResponseData[]> listResponsesCallback, Func<Guid, bool> moreResponsesAllowedCallback)
        {
            base.Initialize(sessionData, authenticatedUserGuid, languageCode, listResponsesCallback, moreResponsesAllowedCallback);

            if (listResponsesCallback == null || moreResponsesAllowedCallback == null)
                return;

            AuthenticatedUserGuid = authenticatedUserGuid;

            _newResponseLink.Visible = moreResponsesAllowedCallback(sessionData.SessionGuid);

            if (_newResponseLink.Visible)
            {
                _newResponseLink.NavigateUrl = GetNewResponseLinkUrl(sessionData, authenticatedUserGuid);
            }

            var responseList = listResponsesCallback(sessionData.SessionGuid);

            _resumeGrid.DataSource = responseList.Where(responseData => !responseData.DateCompleted.HasValue)
                .OrderByDescending(responseData => responseData.DateLastEdited).Take(20).OrderBy(responseData => responseData.DateLastEdited);
            _resumeGrid.DataBind();

            _editGrid.DataSource = responseList.Where(responseData => responseData.DateCompleted.HasValue);
            _editGrid.DataBind();

            _resumePanel.Visible = _resumeGrid.Rows.Count > 0;
            _editPanel.Visible = _editGrid.Rows.Count > 0;
        }

    }
}