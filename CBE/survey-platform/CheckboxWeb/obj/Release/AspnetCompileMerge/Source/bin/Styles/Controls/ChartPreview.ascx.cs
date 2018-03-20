using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Analytics.Items.UI;
using Checkbox.Web.Charts;
using CheckboxWeb.Controls.Charts;
using CheckboxWeb.Styles.Charts.EditorControls;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Styles.Controls
{
    public partial class ChartPreview : ChartStyleUserControl
    {
        protected virtual Font LabelFont { get { return new Font(AppearanceData.FontFamily, AppearanceData.LabelFontSize, FontStyle.Regular); } }

        protected virtual Font TitleFont { get { return new Font(AppearanceData.FontFamily, AppearanceData.TitleFontSize, FontStyle.Bold); } }

        protected Color LabelColor { get { return Utilities.GetColor(AppearanceData.TextColor, false); } }

        /// <summary>
        /// The sample response count
        /// </summary>
        protected int ResponseCount { get { return 14; } }

        /// <summary>
        /// Appearance data to modify
        /// </summary>
        private SummaryChartItemAppearanceData AppearanceData { get; set; }

        /// <summary>
        /// Initialize with a chart appearancy.
        /// </summary>
        /// <param name="chartAppearance"></param>
        public override void Initialize(SummaryChartItemAppearanceData chartAppearance)
        {
            if (_graphType.Items.FindByValue(chartAppearance["GraphType"]) != null)
            {
                _graphType.SelectedValue = chartAppearance["GraphType"];
            }

            AppearanceData = chartAppearance;
        }

        /// <summary>
        /// Get the line color for the grid lines
        /// </summary>
        protected Color GridLineColor { get { return Color.FromArgb(200, 200, 200); } }

        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                List<ListItem> graphTypes = GetEnumListItems(typeof(GraphType));
                foreach (ListItem item in graphTypes)
                {
                    if (item.Value != GraphType.CrossTab.ToString()
                        && item.Value != GraphType.SummaryTable.ToString())
                    {
                        _graphType.Items.Add(item);
                    }
                }
            }

            UpdateChart();
        }

        /// <summary>
        /// Get chart control for preview
        /// </summary>
        /// <returns></returns>
        protected Control GetChart()
        {
            string chartControlPath;

            AppearanceData["GraphType"] = _graphType.SelectedValue;

            switch (_graphType.SelectedValue.ToLower())
            {
                case "piegraph":
                    chartControlPath = "~/Controls/Charts/PieGraph.ascx";
                    break;

                case "linegraph":
                    chartControlPath = "~/Controls/Charts/LineGraph.ascx";
                    break;

                case "bargraph":
                    chartControlPath = "~/Controls/Charts/BarGraph.ascx";
                    break;

                case "doughnut":
                    chartControlPath = "~/Controls/Charts/DoughnutGraph.ascx";
                    break;

                default:
                    chartControlPath = "~/Controls/Charts/ColumnGraph.ascx";
                    break;
            }

            Control chartControl = Page.LoadControl(chartControlPath);

            if (chartControl is ChartControlBase)
            {
                ((ChartControlBase)chartControl).InitializeAndBind(
                    new ReportItemInstanceData
                        {
                            AggregateResults = GetChartDataSource(ResponseCount),
                            SourceItems = new[] {new ReportItemSourceItemData{ItemId = 1000, ReportingText =WebTextManager.GetText("/pageText/styles/charts/chartPreview.ascx/sampleTitle")}}
                        },
                    AppearanceData.GetPropertiesAsNameValueCollection());
            }

            return chartControl;
        }

       /* private string GetTitleText()
        {
            string text = WebTextManager.GetText("/pageText/styles/charts/chartPreview.ascx/sampleTitle");

            if (AppearanceData.ShowResponseCountInTitle)
            {
                text += string.Format("{0}({1} {2})", Environment.NewLine, ResponseCount, WebTextManager.GetText("/pageText/styles/charts/chartPreview.ascx/responses"));
            }

            return text;
        }*/

        private static AggregateResult[] GetChartDataSource(int totalResponseCount)
        {
            var list = new List<AggregateResult>();

            for (int i = 0; i < 5; i++)
            {
                var result = new AggregateResult
                                 {
                                     AnswerCount = i + 1,
                                     AnswerPercent = (i + 1)/(double) totalResponseCount,
                                     ResultText = string.Format("{0} {1}", WebTextManager.GetText("/pageText/styles/charts/chartPreview.ascx/answer"), (i + 1))
                                 };
                list.Add(result);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateChart()
        {
            Control chart = GetChart();

            _chartPlace.Controls.Clear();
            _chartPlace.Controls.Add(chart);
        }
    }
}