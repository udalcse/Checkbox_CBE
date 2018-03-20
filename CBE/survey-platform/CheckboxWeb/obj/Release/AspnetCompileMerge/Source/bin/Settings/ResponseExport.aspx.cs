using System;
using Checkbox.Analytics.Export;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ResponseExport : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            if (!Page.IsPostBack)
            {
                _defaultExportType.DataSource = ExportFormat.SupportedFormats(WebTextManager.GetUserLanguage());
                _defaultExportType.DataTextField = "text";
                _defaultExportType.DataValueField = "value";
                _defaultExportType.DataBind();

                _defaultExportType.SelectedValue = ApplicationManager.AppSettings.DefaultExportType;

                _detailedResponseInfo.Checked = ApplicationManager.AppSettings.CsvExportIncludeResponseDetails;
                _detailedUserInfo.Checked = ApplicationManager.AppSettings.CsvExportIncludeUserDetails;
                _mergeCheckboxResults.Checked = ApplicationManager.AppSettings.CsvExportMergeCheckboxResults;
                _exportOpenendedResults.Checked = ApplicationManager.AppSettings.CsvExportIncludeOpenendedResults;
                _exportWithAliases.Checked = ApplicationManager.AppSettings.CsvExportUseAliases;
                _exportHiddenItems.Checked = ApplicationManager.AppSettings.CsvExportIncludeHiddenItems;
                _exportIncompleteResponses.Checked = ApplicationManager.AppSettings.CsvExportIncludeIncompleteResponses;
                _exportStripHtmlTags.Checked = ApplicationManager.AppSettings.CsvExportStripHtmlTagsFromAnswers;
                _splitExport.Checked = ApplicationManager.AppSettings.CsvExportSplitExport;
                _newLineReplacement.Text = ApplicationManager.AppSettings.NewLineReplacement;
                _outputEncoding.SelectedValue = ApplicationManager.AppSettings.DefaultExportEncoding;
                _rankOrderPoints.Checked = ApplicationManager.AppSettings.CsvExportRankOrderPoints;
                _detailedScoreInfo.Checked = ApplicationManager.AppSettings.CsvExportIncludeDetailedScoreInfo;
                _possibleScore.Checked = ApplicationManager.AppSettings.CsvExportIncludePossibleScore;

                _includeResponseIdInExport.Checked = ApplicationManager.AppSettings.SpssExportIncludeResponseId;
                _exportIncompleteResponsesSpss.Checked = ApplicationManager.AppSettings.SpssExportIncludeIncompleteResponses;
                _exportOpenEndedResultsSpss.Checked = ApplicationManager.AppSettings.SpssExportIncludeOpenendedResults;

                _replaceNewLine.Checked = ApplicationManager.AppSettings.ReplaceNewLine;
                _newLineReplacement.Enabled = _replaceNewLine.Checked;
            }

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/responseExport.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            Master.OkClick += Master_OkClick;
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.DefaultExportType = _defaultExportType.SelectedValue;

            ApplicationManager.AppSettings.CsvExportIncludeResponseDetails = _detailedResponseInfo.Checked;
            ApplicationManager.AppSettings.CsvExportIncludeUserDetails = _detailedUserInfo.Checked;
            ApplicationManager.AppSettings.CsvExportMergeCheckboxResults = _mergeCheckboxResults.Checked;
            ApplicationManager.AppSettings.CsvExportIncludeOpenendedResults = _exportOpenendedResults.Checked;
            ApplicationManager.AppSettings.CsvExportUseAliases = _exportWithAliases.Checked;
            ApplicationManager.AppSettings.CsvExportIncludeHiddenItems = _exportHiddenItems.Checked;
            ApplicationManager.AppSettings.CsvExportIncludeIncompleteResponses = _exportIncompleteResponses.Checked;
            ApplicationManager.AppSettings.CsvExportStripHtmlTagsFromAnswers = _exportStripHtmlTags.Checked;
            ApplicationManager.AppSettings.CsvExportSplitExport = _splitExport.Checked;
            ApplicationManager.AppSettings.CsvExportRankOrderPoints = _rankOrderPoints.Checked;
            ApplicationManager.AppSettings.DefaultExportEncoding = _outputEncoding.SelectedValue;
            ApplicationManager.AppSettings.CsvExportIncludeDetailedScoreInfo = _detailedScoreInfo.Checked;
            ApplicationManager.AppSettings.CsvExportIncludePossibleScore = _possibleScore.Checked;

            ApplicationManager.AppSettings.ReplaceNewLine = _replaceNewLine.Checked;
            if (ApplicationManager.AppSettings.ReplaceNewLine)
                ApplicationManager.AppSettings.NewLineReplacement = _newLineReplacement.Text;

            ApplicationManager.AppSettings.SpssExportIncludeResponseId = _includeResponseIdInExport.Checked;
            ApplicationManager.AppSettings.SpssExportIncludeIncompleteResponses = _exportIncompleteResponsesSpss.Checked;
            ApplicationManager.AppSettings.SpssExportIncludeOpenendedResults = _exportOpenEndedResultsSpss.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }

        protected void NewLine_ClickEvent(object sender, EventArgs e)
        {
            _newLineReplacement.Enabled = _replaceNewLine.Checked;
        }
    }
}
