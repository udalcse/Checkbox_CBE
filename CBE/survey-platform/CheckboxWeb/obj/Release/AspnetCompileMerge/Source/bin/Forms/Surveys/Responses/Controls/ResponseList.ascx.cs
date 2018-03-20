using System;
using Checkbox.Analytics;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Timeline;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Web;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CheckboxWeb.Forms.Surveys.Responses.Controls
{
    public partial class ResponseList : Checkbox.Web.Common.UserControlBase
    {
        private long? _responseId;

        /// <summary>
        /// Id of survey to list responses for
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        /// Admin Timeline period
        /// </summary>
        public int TimelinePeriod { get; set; }

        /// <summary>
        /// Callback for response selected
        /// </summary>
        public string ResponseSelectedClientCallback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { set; get; }

        private long? ResponseId
        {
            get
            {
                if (!_responseId.HasValue)
                {
                    long g;
                    if (long.TryParse(Request.QueryString["r"], out g))
                        _responseId = g;
                }

                return _responseId;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public int NumberOfCompleteResponses
        {
            get
            {
                var data = ResponseDataServiceImplementation.GetResponseSummary(UserManager.GetCurrentPrincipal(), SurveyId);
                if (data != null)
                    return data.CompletedResponseCount;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NumberOfIncompleteResponses
        {
            get
            {
                var data = ResponseDataServiceImplementation.GetResponseSummary(UserManager.GetCurrentPrincipal(), SurveyId);
                if (data != null)
                    return data.IncompleteResponseCount;

                return 0;
            }
        }

        private int GetPageNumber()
        {
            if (!ResponseId.HasValue)
                return 1;

            DateTime? start, end;
            TimelineManager.GetPeriodDates(TimelinePeriod, out start, out end);

            return ResponseManager.GetPageNumberByResponseId(ResponseId.Value,
                true, start, end, ApplicationManager.AppSettings.PagingResultsPerPage,
                "ResponseID", true, string.Empty, string.Empty);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _responseGrid.InitialSortField = "Started";
            _responseGrid.ItemClickCallback = ResponseSelectedClientCallback;
            _responseGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Responses/jqtmpl/responseListTemplate.html");
            _responseGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Responses/jqtmpl/responseListItemTemplate.html");
            _responseGrid.LoadDataCallback = "loadResponseList";
            _responseGrid.EmptyGridText = WebTextManager.GetText("/pageText/viewResponses.aspx/noResponses");
            _responseGrid.RenderCompleteCallback = "gridRenderComplete";
            _responseGrid.InitialPageNumber = GetPageNumber();
            _responseGrid.InitialFilterKey = "complete,incomplete,test";

            BindData();
        }

        private void BindData()
        {
            var propertyList = ProfileManager.GetPropertiesList().Where(s => s.FieldType == CustomFieldType.SingleLine || s.FieldType == CustomFieldType.RadioButton)
                .Select(s => new KeyValuePair<int, string>(s.FieldId, s.Name));

            _searchProperties.DataTextField = "Value";
            _searchProperties.DataValueField = "Key";
            _searchProperties.DataSource = propertyList;
            _searchProperties.DataBind();
        }
    }
}