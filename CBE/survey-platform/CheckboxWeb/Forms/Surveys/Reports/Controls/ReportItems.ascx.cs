using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web;
using System.Collections.Generic;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportItems : Checkbox.Web.Common.UserControlBase
    {
        public string RadioButtonGraphType { get { return _radioButtons.SelectedValue; } }
        public string CheckboxGraphType { get { return _checkboxes.SelectedValue; } }
        public string SingleLineTextGraphType { get { return _singleLineText.SelectedValue; } }
        public string MultiLineTextGraphType { get { return _multiLineText.SelectedValue; } }
        public string RatingScaleGraphType { get { return _ratingScale.SelectedValue; } }
        public string DropDownListGraphType { get { return _dropDownList.SelectedValue; } }
        public string MatrixGraphType { get { return _matrix.SelectedValue; } }
        public string HiddenItemGraphType { get { return _hiddenItems.SelectedValue; } }
        public string SliderGraphType { get { return _slider.SelectedValue; } }
        public string RankOrderGraphType { get { return _rankOrder.SelectedValue; } }
        public string NetPromoterScoreGraphType { get { return _netPromoterScore.SelectedValue; } }

        public bool UserInputItemsVisibility
        {
            set { _userInputItemsPanel.Visible = value; }
            get { return _userInputItemsPanel.Visible; }
        }

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
        /// Show/hide ui elements
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadGraphOptions();
                BindData();
            }
        }

        private void LoadGraphOptions()
        {
            _checkboxes.Items.AddRange(CreateChartTypesOptions());
            _dropDownList.Items.AddRange(CreateChartTypesOptions());
            _radioButtons.Items.AddRange(CreateChartTypesOptions());
            _singleLineText.Items.AddRange(CreateTextChartTypesOptions());
            _multiLineText.Items.AddRange(CreateTextChartTypesOptions());
            _slider.Items.AddRange(CreateChartTypesOptions());

            _ratingScale.Items.Add(new ListItem(WebTextManager.GetText("/itemType/StatisticsTable/name"), "StatisticsTable"));
            _ratingScale.Items.AddRange(CreateChartTypesOptions());

            _netPromoterScore.Items.Add(new ListItem(WebTextManager.GetText("/itemType/netPromoterScoreTable/name"), "NetPromoterScoreTable"));
            _netPromoterScore.Items.Add(new ListItem(WebTextManager.GetText("/itemType/netPromoterScoreStatisticsTable/name"), "NetPromoterScoreStatisticsTable"));
            _netPromoterScore.Items.AddRange(CreateChartTypesOptions());

            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/itemType/matrixSummary/name"), "MatrixSummary"));
            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/matrixSummaryAndNested"), "MatrixSummaryAndNested"));
            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/nestedItemsOnly"), "NestedItemsOnly"));
            _matrix.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"), "DoNotShow"));

            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/enum/graphType/SummaryTable"), "SummaryTable"));
            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/itemType/details/name"), "Details"));
            _hiddenItems.Items.Add(new ListItem(WebTextManager.GetText("/pageText/settings/reportPreferences.aspx/doNotShow"), "DoNotShow"));

            _rankOrder.Items.AddRange(CreateChartTypesOptions());
        }

        /// <summary>
        /// Bind!
        /// </summary>
        private void BindData()
        {
            // set the selected values in the drop down lists to the application default value
            _checkboxes.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultCheckboxes;
            _dropDownList.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultDropDownList;
            _matrix.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultMatrix;
            _multiLineText.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultMultiLineText;
            _radioButtons.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRadioButtons;
            _ratingScale.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRadioButtonScale;
            _singleLineText.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultSingleLineText;
            _hiddenItems.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultHiddenItems;
            _slider.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultSlider;
            _rankOrder.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultRankOrder;
            _netPromoterScore.SelectedValue = ApplicationManager.AppSettings.AutogenReportDefaultNetPromoterScore;
        }
    }
}
