using System;
using System.Collections.Generic;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class ResponseSettings : SecuredPage
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
            
            _properties.IncludeIncompleteResponses = Report.IncludeIncompleteResponses;
            _properties.IncludeTestResponses = Report.IncludeTestResponses;

            Master.OkClick += OkBtn_Click;

            Master.Title = WebTextManager.GetText("/pageMenu/analysis_management/_response");
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
                Report.IncludeIncompleteResponses = _properties.IncludeIncompleteResponses;
                Report.IncludeTestResponses = _properties.IncludeTestResponses;

                Report.Save();
                AnalysisTemplateManager.CleanupAnalysisTemplatesCache(Report.ID.Value);

                //Close window
                var args = new Dictionary<string, string> { { "op", "properties" }, { "reportId", Report.ID.ToString() } };
                Master.CloseDialog(args);
            }
        }
    }
}