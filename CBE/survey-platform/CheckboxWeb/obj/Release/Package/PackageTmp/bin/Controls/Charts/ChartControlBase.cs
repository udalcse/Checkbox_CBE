using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Wcf.Services.Proxies;
using System.Text.RegularExpressions;

namespace CheckboxWeb.Controls.Charts
{
    /// <summary>
    /// Chart control base class
    /// </summary>
    public abstract class ChartControlBase : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="model"></param>
        /// <param name="appearanceData"></param>
        public void InitializeAndBind(ReportItemInstanceData model, SimpleNameValueCollection appearanceData)
        {
            //Store values
            Model = model;
            Appearance = appearanceData;

            DecodeOptions();

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("jQuery"))
                RegisterClientScriptInclude("jQuery", ResolveUrl("~/Resources/jquery-latest.min.js"));
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("highcharts.js"))
                RegisterClientScriptInclude("highcharts.js", Page.ResolveUrl("~/Resources/highcharts/highcharts.js"));
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("modules/exporting.js"))
                RegisterClientScriptInclude("modules/exporting.js", Page.ResolveUrl("~/Resources/highcharts/modules/exporting.js"));

           
            //initialize series
            _xAxisCategories = new List<List<string>>();
            _series = initSeries(_xAxisCategories);
        }

        private void DecodeOptions()
        {
            foreach (var result in Model.AggregateResults)
            {
                result.ResultText = Utilities.DecodeAndStripHtml(result.ResultText);
            }
        }

        /// <summary>
        /// Analysis item
        /// </summary>
        protected ReportItemInstanceData Model { get; private set; }

        /// <summary>
        /// Chart appearance
        /// </summary>
        protected SimpleNameValueCollection Appearance { get; private set; }

        /// <summary>
        /// Property that forces control to display Points instead of AnswerCount
        /// </summary>
        public bool UsePointsAsY
        {
            get;
            set;
        }

        /// <summary>
        /// Get the chart title
        /// </summary>
        protected virtual string GetTitle()
        {
            if ("false".Equals(Appearance["ShowTitle"], StringComparison.InvariantCultureIgnoreCase))
                return "";

            var sb = new StringBuilder();

            int titleFontSize = Utilities.AsInt(Appearance["TitleFontSize"], 18);

            bool showResponseCount = false; /* "true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase)
                 && "true".Equals(Appearance["ShowAnswerCount"], StringComparison.InvariantCultureIgnoreCase);*/
            bool multiSource = Model.SourceItems.Length > 1;

            foreach (var sourceItem in Model.SourceItems)
            {
                var sbForOneItem = new StringBuilder();

                string text = Utilities.DecodeAndStripHtml(sourceItem.ReportingText);
                sbForOneItem.Append(/*multiSource ? */Wrap(text)/* : text*/);

                if (showResponseCount)
                {
                    sbForOneItem.Append(multiSource ? "  <span style=\"font-size:" + titleFontSize * 2 / 3  + "pt;\">(" : Environment.NewLine);

                    int count = GetItemResponseCount(sourceItem.ItemId);
                    sbForOneItem.Append(count);
                    sbForOneItem.Append(" ");
                    sbForOneItem.Append(TextManager.GetText(count == 1 ? "/controlText/analysisItemRenderer/response" : "/controlText/analysisItemRenderer/responses", LanguageCode));

                    if (multiSource)
                        sbForOneItem.Append(")</span>");
                }

                sb.Append(sbForOneItem);
                if (multiSource)
                    sb.Append(Environment.NewLine);
                  
//                sb.Append(SplitAndWrap(sbForOneItem.ToString())/* Wrap()*/);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Splits long lines to several lines
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private string SplitAndWrap(string src)
        {
            if (WrapTitleChars == 0)
                return src;

            //remove html tags because some of them may be treated as different words
            src = Regex.Replace(src, "<.*?>", string.Empty);

            //separate the title to the lines
            var newLines = new List<string>();
            string[] lines = src.Split(new string[] { "<br>", "<br />", "\n\r", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > WrapTitleChars)
                {
                    //wrap the long line
                    string[] words = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 1)
                    {
                        StringBuilder lineBuilder = new StringBuilder();
                        int lineLen = 0;
                        for (int j = 0; j < words.Length; j++)
                        {
                            if (lineLen + words[j].Length > WrapTitleChars)
                            {
                                string word = words[j];
                                if (word.Length > WrapTitleChars)
                                {
                                    word = word.Substring(0, WrapTitleChars - 5) + "..." + word.Substring(word.Length - 2, 2);
                                }

                                newLines.Add(lineBuilder.ToString());
                                lineBuilder.Clear();
                                lineBuilder.Append(word);
                                lineLen = 0;
                            }
                            else
                            {
                                lineBuilder.AppendFormat(" {0}", words[j]);
                                lineLen += 1; //space added
                            }

                            lineLen += words[j].Length;
                        }
                        newLines.Add(lineBuilder.ToString());
                    }
                    else if (words.Length == 1 && words[0].Length > 10 && words[0].Length > WrapTitleChars)
                    {
                        lines[i] = words[0].Substring(0, WrapTitleChars - 5) + "..." + words[0].Substring(words[0].Length - 2, 2);
                        newLines.Add(lines[i]);
                    }
                }
                else
                {
                    newLines.Add(lines[i]);
                }
            }
            return string.Join("\n", newLines.ToArray());
        }

        /// <summary>
        /// Adds new line separators to wrap the text
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected string Wrap(string src)
        {
            if (WrapTitleChars == 0)
                return src;

            //remove html tags because some of them may be treated as different words
            src = Regex.Replace(src, "<.*?>", string.Empty);

            //separate the title to the lines
            string[] lines = src.Split(new string[] {"<br>", "<br />", "\n\r", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Length > WrapTitleChars)
                {
                    //wrap the long line
                    string[] words = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 1)
                    {
                        StringBuilder lineBuilder = new StringBuilder();
                        int lineLen = 0;
                        for (int j = 0; j < words.Length; j++)
                        {
                            if (lineLen > 0 && lineLen + words[j].Length > WrapTitleChars)
                            {
                                string lastWord = words[words.Length - 1];
                                lineBuilder.Append("...");
                                lineBuilder.Append(lastWord.Length < 3 ? lastWord : lastWord.Substring(lastWord.Length - 2, 2));
                                break;
                            }
                            if (lineLen > 0)
                                lineBuilder.Append(" ");
                            lineBuilder.Append(words[j].Length <= WrapTitleChars || words[j].Length < 10 ? words[j]
                                : words[j].Substring(0, WrapTitleChars - 5) + "..." + words[j].Substring(words[j].Length - 2, 2));
                            lineLen += words[j].Length;
                        }
                        lines[i] = lineBuilder.ToString();
                    }
                    else if (words.Length == 1 && words[0].Length > 10 && words[0].Length > WrapTitleChars)
                    {
                        lines[i] = words[0].Substring(0, WrapTitleChars - 5) + "..." + words[0].Substring(words[0].Length - 2, 2);
                    }
                }
            }
            return string.Join("\n", lines);
        }

        /// <summary>
        /// Maximum length for the substing of the title per one line
        /// </summary>
        protected int WrapTitleChars
        {
            get
            {
                return Utilities.AsInt(Appearance["WrapTitleChars"], 0);
            }
        }


        /// <summary>
        /// Width for the border
        /// </summary>
        protected int BorderLineWidth
        {
            get
            {
                return Utilities.AsInt(Appearance["BorderLineWidth"], 0);
            }
        }

        /// <summary>
        /// Color of the border
        /// </summary>
        protected string BorderLineColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["BorderLineColor"], false));
            }
        }

        /// <summary>
        /// Chart background color
        /// </summary>
        protected string BackgroundColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["BackgroundColor"], false));
            }
        }

        /// <summary>
        /// Plot area background color
        /// </summary>
        protected string PlotAreaBackgroundColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["PlotAreaBackgroundColor"], false));
            }
        }

        /// <summary>
        /// Legend background color
        /// </summary>
        protected string LegendBackgroundColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["LegendBackgroundColor"], false));
            }
        }

        /// <summary>
        /// Title text color
        /// </summary>
        protected string TextColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["TextColor"], false));
            }
        }

        /// <summary>
        /// Legend Text Color
        /// </summary>
        protected string LegendTextColor
        {
            get
            {
                if (string.IsNullOrEmpty(Appearance["PieBorderColor"]))
                    return ColorTranslator.ToHtml(Color.Black);
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["LegendTextColor"], false));
            }
        }

        /// <summary>
        /// Hint Text Color
        /// </summary>
        protected string HintTextColor
        {
            get
            {
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["HintTextColor"], false));
            }
        }

        /// <summary>
        /// Color For Borders of Pies, Bars or Columns
        /// </summary>
        protected string PieBorderColor
        {
            get
            {
                if (string.IsNullOrEmpty(Appearance["PieBorderColor"]))
                    return ColorTranslator.ToHtml(Color.White);
                return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["PieBorderColor"], false));
            }
        }

        /// <summary>
        /// Colors for the Chart Elements
        /// </summary>
        protected string[] _colors;

        /// <summary>
        /// Colors for the Chart Elements
        /// </summary>
        protected virtual string[] Colors
        {
            get
            {
                if (_colors != null)
                    return _colors;

                var transparency = Utilities.AsInt(Appearance["Transparency"], 100);

                if (string.IsNullOrEmpty(Appearance["BarColor"]))
                {
                    _colors = ParsePieGraphColors(Appearance["PieGraphColors"], transparency);
                }
                else
                {
                    _colors = new string[] { ColorTranslator.ToHtml(Utilities.GetColor(Appearance["BarColor"], transparency, false)) };
                }
                return _colors;
            }
        }

        /// <summary>
        /// Parse Colors from the Appearance
        /// </summary>
        /// <param name="p"></param>
        /// <param name="transparency"></param>
        /// <returns></returns>
        protected string[] ParsePieGraphColors(string p, int transparency)
        {
            return p.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(color => (transparency == 100 ? ColorTranslator.ToHtml(Utilities.GetColor(color.Trim(), 100, false)):
                                    string.Format("rgba({0},{1},{2},{3})", Utilities.GetColor(color.Trim(), 100, false).R,
                                    Utilities.GetColor(color.Trim(), 100, false).G,
                                    Utilities.GetColor(color.Trim(), 100, false).B,
                                    ((double)transparency / 100.0).ToString("F2").Replace(",", "."))))
                            .ToArray();
        }

        /// <summary>
        /// Is the Chart in Preview Mode
        /// </summary>
        protected bool PreviewMode
        {
            get
            {
                if (Model == null || Model.InstanceData == null)
                    return false;
                return "True".Equals(Model.InstanceData["PreviewMode"]);
            }
        }

        /// <summary>
        /// Tooltip text
        /// </summary>
        protected virtual string Tooltip
        {
            get
            {
                bool showPercent = Utilities.AsBool(Appearance["ShowPercent"], true);
                if (!showPercent)
                    return TooltipWithoutPercentage;

                bool showAnswerCount = Utilities.AsBool(Appearance["ShowAnswerCount"], true);
                return showAnswerCount ? "<b>{point.y} ({point.percentage}%)</b>" : "<b>{point.percentage}%</b>";
            }
        }

        /// <summary>
        /// TooltipWithoutPercentage text
        /// </summary>
        protected virtual string TooltipWithoutPercentage
        {
            get
            {
                bool showAnswerCount = Utilities.AsBool(Appearance["ShowAnswerCount"], true);
                return showAnswerCount ? "<b>{point.y}</b>" : string.Empty;
            }
        }

        /// <summary>
        /// Prepared Results
        /// </summary>
        private AggregateResult[] _aggregateResults;
        /// <summary>
        /// Prepares aggregate results for the chart
        /// </summary>
        protected AggregateResult[] AggregateResults
        {
            get
            {
                if (_aggregateResults != null)
                    return _aggregateResults;

                _aggregateResults = initAggregateResults();

                return _aggregateResults;                
            }
        }

        #region Helpers Methods

        /// <summary>
        /// Escape apostrophes
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string EscapeJSStringConst(string val)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in val)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\'':
                        sb.Append("\\'");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        break;
                    case '\n':
                        break;
                    case '\r':
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get text for legend for a series
        /// </summary>
        /// <param name="seriesName"></param>
        /// <returns></returns>
        protected string GetSeriesLegendText(string seriesName)
        {
            string seriesLegend = seriesName;

            //Check if series represents item id.  IF so, get item text/response count.  Otherwise return series name/response count.
            var seriesNameAsInt = Utilities.AsInt(seriesName);

            if (seriesNameAsInt.HasValue)
            {
                var sourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == seriesNameAsInt.Value);

                if (sourceItem != null)
                {
                    seriesLegend = Wrap(Utilities.DecodeAndStripHtml(sourceItem.ReportingText));
                }
            }

            bool showResponseCount = "true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase)
                                && "true".Equals(Appearance["ShowAnswerCount"], StringComparison.InvariantCultureIgnoreCase);


            if (showResponseCount && seriesNameAsInt.HasValue)
            {
                seriesLegend = string.Format(
                    "{0} ({1} {2})",
                    seriesLegend,
                    GetItemResponseCount(seriesNameAsInt.Value),
                    TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
            }

            return seriesLegend;
        }


        /// <summary>
        /// Get response count for item
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        protected int GetItemResponseCount(int itemId)
        {
            var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

            return itemData != null ? itemData.ResponseCount : 0;
        }

        /// <summary>
        /// Get language code for text
        /// </summary>
        protected string LanguageCode
        {
            get
            {
                return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                    ? TextManager.DefaultLanguage
                    : Model.InstanceData["LanguageCode"];
            }
        }

        /// <summary>
        /// Initializes Results
        /// </summary>
        /// <returns></returns>
        protected virtual AggregateResult[] initAggregateResults(string seriesName = null, List<string> xCategories = null)
        {
            var rawResults = seriesName == null ? Model.AggregateResults :
                (from ar in Model.AggregateResults where seriesName.Equals(ar.ResultKey) || ar.ResultKey.StartsWith(seriesName + "_") select ar);

            AggregateResult[] res = null;

            if (Utilities.AsBool(Appearance["ShowDataLabelZeroValues"], false))
            {
                if ("Survey".Equals(Appearance["OptionsOrder"]))
                    res = rawResults.OrderBy(result => result.ResultIndex)
                            .ToArray();
                else
                    res = rawResults.OrderBy(result => result.ResultIndex)
                            .OrderBy(result => result.ResultText)
                            .ToArray();

            }
            //Exclude items with no answeres
            else
            {
                if ("Survey".Equals(Appearance["OptionsOrder"]))
                    res = rawResults.Where(result => result.AnswerCount > 0)
                            .OrderBy(result => result.ResultIndex)
                            .ToArray();
                else
                    res = rawResults.Where(result => result.AnswerCount > 0)
                            .OrderBy(result => result.ResultIndex)
                            .OrderBy(result => result.ResultText)
                            .ToArray();
            }

            //build xTitles if needed
            if (xCategories != null)
            {
                foreach (AggregateResult ar in res)
                {
                    xCategories.Add(Utilities.DecodeAndStripHtml(ar.ResultText));
                }
            }

            return res;
        }

        /// <summary>
        /// Series Data contains full information about the Series
        /// </summary>
        public class SeriesData
        {
            public string SeriesID { get; set; }
            public string Name { get; set; }

            public AggregateResult[] AggregateResults { get; set; }
        }


        /// <summary>
        /// All Series Data
        /// </summary>
        private SeriesData[] _series;
        /// <summary>
        /// All Series Data
        /// </summary>
        protected virtual SeriesData[] Series
        {
            get
            {
                return _series;
            }
        }

        /// <summary>
        /// Independent Axis Categories
        /// </summary>
        private List<List<string>> _xAxisCategories;
        /// <summary>
        /// Independent Axis Categories
        /// </summary>
        protected virtual List<List<string>> xAxisCategories
        {
            get
            {
                return _xAxisCategories;
            }
        }

        /// <summary>
        /// Initialize All Series Data
        /// </summary>
        /// <returns></returns>
        protected virtual SeriesData[] initSeries(List<List<string>> xAxisCategories)
        {
            List<SeriesData> series = new List<SeriesData>();

            //Build list of result keys, which have format of
            // ItemID or ItemID_OptionId.  We want to build a series for each
            // item id present.
            var resultKeys =
                Model.AggregateResults
                    .OrderBy(result => result.ResultIndex)
                    .Select(result => result.ResultKey)
                    .Distinct()
                    .ToList();

            //Parse result keys for series keys
            foreach (var splitResult in resultKeys
                        .Select(resultKey => resultKey.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries))
                        .Where(splitResult => splitResult.Length > 0 &&
                            (from s in series where s.SeriesID.Equals(splitResult[0]) select s.SeriesID).Count() == 0))
            {
                List<string> xCategories = new List<string>();
                series.Add(new SeriesData()
                {
                    SeriesID = splitResult[0],
                    AggregateResults = initAggregateResults(splitResult[0], xCategories),
                    Name = GetSeriesLegendText(splitResult[0])
                });
                if (string.IsNullOrEmpty(Model.Metadata["PrimarySourceItemID"]) || Model.Metadata["PrimarySourceItemID"] == "0"
                    || Model.Metadata["PrimarySourceItemID"] == splitResult[0])
                {
                    xAxisCategories.Add(xCategories);
                }
            }

            //no values for primary source item, as a result we can't build titles for the row
            if (xAxisCategories.Count == 0)
            {
                int xAxisLabelCount = 2; //2 as the minimum x-axis labels
                foreach (var s in series)
                {
                    if (s.AggregateResults.Length > xAxisLabelCount)
                        xAxisLabelCount = s.AggregateResults.Length;
                }

                var fakeLabels = new List<string>();
                for (int i = 0; i < xAxisLabelCount; i++)
                {
                    fakeLabels.Add(" ");
                }

                xAxisCategories.Add(fakeLabels);
            }

            return series.ToArray();
        }
        #endregion
    }
}
