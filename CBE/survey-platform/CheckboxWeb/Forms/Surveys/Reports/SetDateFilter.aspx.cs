using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Analytics;
using Checkbox.Forms;
using Page = Checkbox.Forms.Page;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class SetDateFilter : SecuredPage 
    {
        private AnalysisTemplate _currentReport;

        /// <summary>
        /// Get a reference to current report
        /// </summary>
        private AnalysisTemplate Report
        {
            get
            {
                if (_currentReport==null)
                {
                    if (Utilities.IsNotNullOrEmpty(Request.QueryString["r"]))
                        _currentReport = AnalysisTemplateManager.GetAnalysisTemplate(int.Parse(Request.QueryString["r"]));
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

            Master.OkClick += OkBtn_Click;

            Master.Title = WebTextManager.GetText("/pageText/surveys/reports/setdatefilter.aspx/title");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _datePickerLocaleResolver.Source = "~/Resources/" + GlobalizationManager.GetDatePickerLocalizationFile();

            if (!IsPostBack)
            {
                if (Report.FilterStartDate.HasValue)
                    _startDate.Text = GlobalizationManager.FormatTheDate(Report.FilterStartDate.Value);
                if (Report.FilterEndDate.HasValue)
                    _endDate.Text = GlobalizationManager.FormatTheDate(Report.FilterEndDate.Value);
            }
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
                DateTime? dtStart = null;
                DateTime? dtFinish = null;

                try
                {
                    dtStart = string.IsNullOrEmpty(_startDate.Text) ? null : (DateTime?)WebUtilities.ConvertFromClientToServerTimeZone(DateTime.Parse(_startDate.Text));
                }
                catch (Exception ex)
                {
                    ShowError(string.Format(WebTextManager.GetText("/pageText/surveys/reports/setdatefilter.aspx/error/parsestartdate"), _startDate.Text), ex);
                    return;
                }


                try
                {
                    dtFinish = string.IsNullOrEmpty(_endDate.Text) ? null : (DateTime?)WebUtilities.ConvertFromClientToServerTimeZone(DateTime.Parse(_endDate.Text));
                }
                catch (Exception ex)
                {
                    ShowError(string.Format(WebTextManager.GetText("/pageText/surveys/reports/setdatefilter.aspx/error/parseenddate"), _endDate.Text), ex);
                    return;
                }


                if (dtFinish.HasValue && dtStart.HasValue && dtStart.Value >= dtFinish.Value)
                {
                    ShowError(WebTextManager.GetText("/pageText/surveys/reports/setdatefilter.aspx/error/startMustbeLessThanFinish"), null);
                    return;
                }
                
                Report.FilterStartDate = dtStart;
                Report.FilterEndDate = dtFinish;

                //cleanup the cache
                AnalysisTemplateManager.CleanupAnalysisTemplatesCache(Report.ID.Value);

                try
                {
                    Report.Save();
                }
                catch (Exception ex)
                {
                    ShowError(WebTextManager.GetText("/pageText/surveys/reports/setdatefilter.aspx/error/failedtosave"), ex);
                    return;
                }
                
                Master.CloseDialog(new Dictionary<string, string> { { "op", "properties" }, { "reportId", Report.ID.ToString() } });
            }
        }
    }
}