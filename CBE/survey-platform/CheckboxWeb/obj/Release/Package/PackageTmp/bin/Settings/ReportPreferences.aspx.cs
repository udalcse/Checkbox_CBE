using System;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Styles;
using System.Collections.Generic;

namespace CheckboxWeb.Settings
{
    public partial class ReportPreferences : SettingsPage
    {
        private ListItem[] CreateChartTypesOptions()
        {
            var chartTypes = new List<ListItem>
                                 {
                                     new ListItem(WebTextManager.GetText("/enum/graphType/SummaryTable"), "SummaryTable"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/ColumnGraph"), "ColumnGraph"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/PieGraph"), "PieGraph"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/LineGraph"), "LineGraph"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/BarGraph"), "BarGraph"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/Details"), "Details"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/doughnut"), "DoughnutGraph"),
                                     new ListItem(
                                         WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"),
                                         "DoNotShow")
                                 };

            return chartTypes.ToArray();
        }

        private ListItem[] CreateTextChartTypesOptions()
        {
            var chartTypes = new List<ListItem>
                                 {
                                     new ListItem(WebTextManager.GetText("/enum/graphType/SummaryTable"), "SummaryTable"),
                                     new ListItem(WebTextManager.GetText("/enum/graphType/Details"), "Details"),
                                     new ListItem(
                                         WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"),
                                         "DoNotShow")
                                 };

            return chartTypes.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();
            if (!Page.IsPostBack)
            {
                LoadGraphOptions();
                LoadStyleTemplates();
                LoadChartStyles();

                _radioButtons.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRadioButtons;
                _checkboxes.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultCheckboxes;
                _singleLineText.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultSingleLineText;
                _multiLineText.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultMultiLineText;
                _ratingScale.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRadioButtonScale;
                _dropDownList.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultDropDownList;
                _matrix.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultMatrix;
                _hiddenItems.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultHiddenItems;
                _slider.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultSlider;
                _rankOrder.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRankOrder;
                _netPromoterScore.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultNetPromoterScore;

                _maxNumberOfOptions.Text = ApplicationManager.AppSettings.AutogenReportDefaultMaxOptions.ToString();

                _styleTemplate.SelectedValue = ApplicationManager.AppSettings.DefaultStyleTemplate.ToString();
                _chartStyle.SelectedValue = ApplicationManager.AppSettings.DefaultChartStyle.ToString();
                _itemPosition.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultItemPosition;

                _useAliases.Checked = ApplicationManager.AppSettings.AutogenReportDefaultUseAliases;
                _includeIncompleteResponses.Checked = ApplicationManager.AppSettings.ReportIncompleteResponses;
                _includeTestResponses.Checked = ApplicationManager.AppSettings.ReportTestResponses;
                _placeAllAnalysisItemsOnASinglePage.Checked = ApplicationManager.AppSettings.AutogenReportDefaultMultiplePages;
                _displaySurveyTitle.Checked = ApplicationManager.AppSettings.DisplaySurveyTitle;
                _displayPdfExportButton.Checked = ApplicationManager.AppSettings.DisplayPdfExportButton;
            }

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/title");

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
            if (ValidateInputs())
            {
                ApplicationManager.AppSettings.AutogenReportDefaultRadioButtons = _radioButtons.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultCheckboxes = _checkboxes.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultSingleLineText = _singleLineText.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultMultiLineText = _multiLineText.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultRadioButtonScale = _ratingScale.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultDropDownList = _dropDownList.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultMatrix = _matrix.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultHiddenItems = _hiddenItems.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultSlider = _slider.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultRankOrder = _rankOrder.SelectedValue;
                ApplicationManager.AppSettings.AutogenReportDefaultNetPromoterScore = _netPromoterScore.SelectedValue;

                ApplicationManager.AppSettings.AutogenReportDefaultMaxOptions = Int32.Parse(_maxNumberOfOptions.Text.Trim());

                ApplicationManager.AppSettings.DefaultStyleTemplate = Convert.ToInt32(_styleTemplate.SelectedValue);
                ApplicationManager.AppSettings.DefaultChartStyle = Convert.ToInt32(_chartStyle.SelectedValue);
                ApplicationManager.AppSettings.AutogenReportDefaultItemPosition = _itemPosition.SelectedValue;

                ApplicationManager.AppSettings.AutogenReportDefaultUseAliases = _useAliases.Checked;
                ApplicationManager.AppSettings.ReportIncompleteResponses = _includeIncompleteResponses.Checked;
                ApplicationManager.AppSettings.ReportTestResponses = _includeTestResponses.Checked;
                ApplicationManager.AppSettings.AutogenReportDefaultMultiplePages = _placeAllAnalysisItemsOnASinglePage.Checked;
                ApplicationManager.AppSettings.DisplaySurveyTitle = _displaySurveyTitle.Checked;
                ApplicationManager.AppSettings.DisplayPdfExportButton = _displayPdfExportButton.Checked;

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }

        private bool ValidateInputs()
        {
            bool isValid = true;
            int value;

            return isValid;
        }

        private void LoadGraphOptions()
        {
            _checkboxes.Items.AddRange(CreateChartTypesOptions());
            _dropDownList.Items.AddRange(CreateChartTypesOptions());
            _radioButtons.Items.AddRange(CreateChartTypesOptions());
            _singleLineText.Items.AddRange(CreateTextChartTypesOptions());
            _multiLineText.Items.AddRange(CreateTextChartTypesOptions());
            _slider.Items.AddRange(CreateChartTypesOptions());

            _netPromoterScore.Items.Add(new ListItem(WebTextManager.GetText("/itemType/netPromoterScoreTable/name"), "NetPromoterScoreTable"));
            _netPromoterScore.Items.Add(new ListItem(WebTextManager.GetText("/itemType/netPromoterScoreStatisticsTable/name"), "NetPromoterScoreStatisticsTable"));
            _netPromoterScore.Items.AddRange(CreateChartTypesOptions());

            _ratingScale.Items.Add(new ListItem(WebTextManager.GetText("/itemType/StatisticsTable/name"), "StatisticsTable"));
            _ratingScale.Items.AddRange(CreateChartTypesOptions());

            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/itemType/matrixSummary/name"), "MatrixSummary"));
            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"), "DoNotShow"));

            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/enum/graphType/SummaryTable"), "SummaryTable"));
            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/itemType/details/name"), "Details"));
            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"), "DoNotShow"));

            _itemPosition.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/left"), "Left"));
            _itemPosition.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/center"), "Center"));
            _itemPosition.Items.Add(new ListItem(WebTextManager.GetText("/controlText/analysisItemAppearanceEditor/right"), "Right"));

            _rankOrder.Items.AddRange(CreateChartTypesOptions());
        }

        private void LoadStyleTemplates()
        {
            List<LightweightStyleTemplate> templates = StyleTemplateManager.ListStyleTemplates(null);

			_styleTemplate.Items.Add(new ListItem("None Selected", "-1"));
			
			foreach (LightweightStyleTemplate template in templates)
            {
                _styleTemplate.Items.Add(new ListItem(template.Name, template.TemplateId.ToString()));
            }

			//if (_styleTemplate.Items.Count == 0)
			//{
			//    _styleTemplate.Items.Add(new ListItem("No public surveys", "-1"));
			//}
        }

        private void LoadChartStyles()
        {
            List<LightweightStyleTemplate> styles = ChartStyleManager.ListChartStyles(null, false);

            if (styles.Count > 0)
            {
                foreach (var style in styles)
                {
                    _chartStyle.Items.Add(new ListItem(style.Name, style.TemplateId.ToString()));
                }
            }
            else
            {
                _chartStyle.Items.Add(new ListItem("No public chart styles.", "-1"));
            }
        }
    }
}
