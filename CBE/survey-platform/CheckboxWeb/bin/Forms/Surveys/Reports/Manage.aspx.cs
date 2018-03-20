using System;
using System.Collections.Generic;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Analytics;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Manage : ResponseTemplatePage
    {        
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("ret")]
        public string ReturnPage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("term")]
        public string SearchTerm { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "reportID", DefaultValue = "0", IsRequired = false, IsDefaultUsedForInvalid = true)]
        public int ReportID { get; set; }

        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        protected override ResponseTemplate ResponseTemplate
        {
            get
            {
                if (ResponseTemplateId <= 0 && ReportID > 0)
                {
                    AnalysisTemplate r = AnalysisTemplateManager.GetAnalysisTemplate(ReportID, true);                    
                    ResponseTemplateId = r.ResponseTemplateID;
                }

                return base.ResponseTemplate;
            }
        }

        /// <summary>
        /// Require analysis administer permission to create a report
        /// </summary>
        protected override string ControllableEntityRequiredPermission
        {
            get { return "Analysis.Create"; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get
            {
                return ResponseTemplate != null ?
                    String.Format(WebTextManager.GetText("/pageText/forms/surveys/reports/manage.aspx/title"), ResponseTemplate.Name) :
                    string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Administer"; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override List<PageControl> GetControlsToAuthorize()
        {
            var pageControls = base.GetControlsToAuthorize();

            pageControls.Add(new PageControl(null, _titleButtonsPlace, "Analysis.Create", ResponseTemplate));

            return pageControls;
        }

        /// <summary>
        /// Initialize controls
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _reportList.SurveyId = ResponseTemplateId;

            //Set title
            Master.SetTitle(PageSpecificTitle);

            var returnUrl = ResolveUrl("~/Forms/Manage.aspx?s=" + (ResponseTemplate == null ? "" : ResponseTemplate.ID.ToString()));

            if ("search".Equals(ReturnPage, StringComparison.InvariantCultureIgnoreCase))
            {
                returnUrl = ResolveUrl("~/Search.aspx") + "?term=" + Server.UrlEncode(SearchTerm);
            }

			Master.ShowBackButton(returnUrl, true);

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for uframe
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                 "svcSurveyEditor.js",
                 ResolveUrl("~/Services/js/svcSurveyEditor.js"));

            RegisterClientScriptInclude(
                "svcAuthorization.js",
                ResolveUrl("~/Services/js/svcAuthorization.js"));

            LoadDatePickerLocalized();
        }

        /// <summary>
        /// Returns ID of the current survey
        /// </summary>
        public string SurveyID
        {
            get
            {
                return ResponseTemplateId.ToString();
            }
        }

    }
}