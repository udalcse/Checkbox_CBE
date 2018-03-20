using System;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportWizardOptions : Checkbox.Web.Common.UserControlBase
    {
        public int MaxOptions
        {
            get
            {
                int value;
                if (int.TryParse(maxOptionsTxtBox.Text.Trim(), out value))
                    return value;

                return -1;
            }
        }

        public string ItemPosition
        {
            get { return "Center"; }
        }

        public bool UseAlias
        {
            get { return _useAliasesChk.Checked; }
        }

        public bool IsSinglePageReport
        {
            get { return multiPageCkbx.Checked; }
        }

        public bool DisplayStatistics
        {
            get { return _displayStatisticsForSummaryCharts.Checked; }
        }

        public bool DisplayAnswers
        {
            get { return _displayAnswersForSummaryCharts.Checked; }
        }

        public bool ReportIncompleteResponses
        {
            get { return _responseProperties.IncludeIncompleteResponses; }
        }

        public bool ReportTestResponses
        {
            get { return _responseProperties.IncludeTestResponses; }
        }

        /// <summary>
        /// Show/hide ui elements
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));

            if (!Page.IsPostBack)
            {
                itemPositionList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/left"), "Left"));
                itemPositionList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/center"), "Center"));
                itemPositionList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/right"), "Right"));

                maxOptionsTxtBox.Text = ApplicationManager.AppSettings.AutogenReportDefaultMaxOptions.ToString();
                _useAliasesChk.Checked = ApplicationManager.AppSettings.AutogenReportDefaultUseAliases;
                multiPageCkbx.Checked = ApplicationManager.AppSettings.AutogenReportDefaultMultiplePages;
                itemPositionList.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultItemPosition;
                _displayAnswersForSummaryCharts.Checked = ApplicationManager.AppSettings.AutogenReportDisplayStatistics;
                _displayStatisticsForSummaryCharts.Checked = ApplicationManager.AppSettings.AutogenReportDisplayAnswers;

                _responseProperties.IncludeIncompleteResponses = ApplicationManager.AppSettings.ReportIncompleteResponses;
                _responseProperties.IncludeTestResponses = ApplicationManager.AppSettings.ReportTestResponses;
            }
            try
            {
                if (Convert.ToInt32(maxOptionsTxtBox.Text) > 0)
                    maxOptionsErrLbl.Visible = false;
                else
                    throw new FormatException();
            }
            catch (FormatException)
            {
                maxOptionsErrLbl.Visible = true;
                maxOptionsErrLbl.Text = WebTextManager.GetText("/pageText/ReportWizardOptions.ascx/maxOptionsErrLbl");
            }
        }
    }
}