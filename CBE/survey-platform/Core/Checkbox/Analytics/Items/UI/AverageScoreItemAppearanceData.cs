using System;
using Checkbox.Common;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AverageScoreItemAppearanceData : AnalysisItemAppearanceData
    {
        /// <summary>
        /// Set default values
        /// </summary>
        public AverageScoreItemAppearanceData()
        {
            FontFamily = "Arial";
            TextColor = "#000000";
            TitleFontSize = 13;
            LegendFontSize = 12;
            LabelFontSize = 10;
            BorderLineWidth = 0;
            BorderLineColor = "#000000";
            BarColor = "#2368ff";
            BackgroundColor = "#FFFFFF";
            PlotAreaBackgroundColor = "#FFFFFF";
            LegendBackgroundColor = "#FFFFFF";
            PieBorderColor = "#FFFFFF";
            AllowExporting = true;
            BorderRadius = 5;
        }

        /// <summary>
        /// Get the summary item appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ANALYSIS_AVERAGE_SCORE"; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ChartMarginTop
        {
            get { return Utilities.AsInt(this["ChartMarginTop"], 50); }
            set { this["ChartMarginTop"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ChartMarginBottom
        {
            get { return Utilities.AsInt(this["ChartMarginBottom"], 50); }
            set { this["ChartMarginBottom"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ChartMarginLeft
        {
            get { return Utilities.AsInt(this["ChartMarginLeft"], 50); }
            set { this["ChartMarginLeft"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ChartMarginRight
        {
            get { return Utilities.AsInt(this["ChartMarginRight"], 50); }
            set { this["ChartMarginRight"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set font family for text
        /// </summary>
        public string FontFamily
        {
            get { return this["FontFamily"]; }
            set { this["FontFamily"] = value; }
        }

        /// <summary>
        /// Get/set the title font size
        /// </summary>
        public int TitleFontSize
        {
            get { return Utilities.AsInt(this["TitleFontSize"], 18); }
            set { this["TitleFontSize"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the border line width
        /// </summary>
        public int BorderLineWidth
        {
            get { return Utilities.AsInt(this["BorderLineWidth"], 0); }
            set { this["BorderLineWidth"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the border radius
        /// </summary>
        public int BorderRadius
        {
            get { return Utilities.AsInt(this["BorderRadius"], 5); }
            set { this["BorderRadius"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the border line color
        /// </summary>
        public string BorderLineColor
        {
            get { return this["BorderLineColor"]; }
            set { this["BorderLineColor"] = value; }
        }

        /// <summary>
        /// Get the bar color for bar/column/line graphs
        /// </summary>
        public string BarColor
        {
            get { return this["BarColor"]; }
            set { this["BarColor"] = value; }
        }

        /// <summary>
        /// Get/set whether a chart should be animated
        /// </summary>
        public bool Animation
        {
            get { return Utilities.AsBool(this["Animation"], true); }
            set { this["Animation"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the color of the pie border
        /// </summary>
        public virtual string PieBorderColor
        {
            get { return this["PieBorderColor"] ?? "#FFFFFF"; }
            set { this["PieBorderColor"] = value; }
        }

        /// <summary>
        /// Get/set the font size for the chart legend
        /// </summary>
        public int LegendFontSize
        {
            get { return Utilities.AsInt(this["LegendFontSize"], 12); }
            set { this["LegendFontSize"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the text color
        /// </summary>
        public string TextColor
        {
            get { return this["TextColor"]; }
            set { this["TextColor"] = value; }
        }

        /// <summary>
        /// Get/set the text color
        /// </summary>
        public string LegendTextColor
        {
            get { return this["LegendTextColor"]; }
            set { this["LegendTextColor"] = value; }
        }

        /// <summary>
        /// Get/set the text color
        /// </summary>
        public string HintTextColor
        {
            get { return this["HintTextColor"]; }
            set { this["HintTextColor"] = value; }
        }

        /// <summary>
        /// Get/set the font size for the label
        /// </summary>
        public int LabelFontSize
        {
            get { return Utilities.AsInt(this["LabelFontSize"], 10); }
            set { this["LabelFontSize"] = value.ToString(); }
        }
    }
}
