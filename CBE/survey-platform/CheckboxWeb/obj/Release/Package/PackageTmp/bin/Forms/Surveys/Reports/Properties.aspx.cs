using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Analytics;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Properties : SecuredPage
    {
        private AnalysisTemplate _currentReport;

        /// <summary>
        /// Get a reference to current report
        /// </summary>
        private AnalysisTemplate Report
        {
            get
            {
                if (_currentReport == null)
                {
                    if (Utilities.IsNotNullOrEmpty(Request.QueryString["ag"]))
                        _currentReport = AnalysisTemplateManager.GetAnalysisTemplate(new Guid(Request.QueryString["ag"]));
                    else
                    {
                        throw new Exception("Url doesn't contain a report's guid.");
                    }
                }
                return _currentReport;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// Set initial values.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LightweightResponseTemplate responseTemplate =
                ResponseTemplateManager.GetLightweightResponseTemplate(Report.ResponseTemplateID);

            _properties.SurveyName = responseTemplate.Name;
            _properties.SurveyId = Report.ResponseTemplateID;
            _properties.StyleTemplateId = Report.StyleTemplateID;
            _properties.ChartStyleId = Report.ChartStyleID ?? -1;
            _properties.ReportName = Server.HtmlDecode(Report.Name);
            _properties.IsNewReport = false;
            _properties.DisplaySurveyTitle = Report.DisplaySurveyTitle;
            _properties.DisplayPdfExportButton = Report.DisplayPdfExportButton;

            Master.OkClick += OkBtn_Click;

            Master.Title = WebTextManager.GetText("/pageMenu/analysis_management/_properties");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (AnalysisTemplateManager.AnalysisTemplateExists(_properties.ReportName, Report.ResponseTemplateID, Report.ID))
                {
                    _properties.NameValidator.Visible = true;
                    return;
                }

                Report.Name = _properties.ReportName;
                if (_properties.ChartStyleId == -1)
                    Report.ChartStyleID = null;
                else
                    Report.ChartStyleID = _properties.ChartStyleId;

                Report.StyleTemplateID = _properties.StyleTemplateId;
                Report.DisplaySurveyTitle = _properties.DisplaySurveyTitle;
                Report.DisplayPdfExportButton = _properties.DisplayPdfExportButton;

                Report.Save();
                AnalysisTemplateManager.CleanupAnalysisTemplatesCache(Report.ID.Value);

                //Close window
                var args = new Dictionary<string, string> { { "op", "properties" }, { "reportId", Report.ID.ToString() } };
                Master.CloseDialog(args);
            }
        }
    }
}