using System;
using System.Text;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class AnalysisItemAppearanceData : AppearanceData
    {
        #region Constructors
        /// <summary>
        /// Set default values
        /// </summary>
        protected AnalysisItemAppearanceData()
        {
            //Options common to all graph types
            Precision = 1;
            ShowTitle = true;
            ShowResponseCountInTitle = true;
            TitleColor = "Black";
            Width = 800;
            Height = 400;
            ShowLegend = false;
            LegendWidth = 200;
            BackgroundColor = "White";
            BackgroundGradient = false;
            ShowHeader = true;
            GraphSpacing = 20;
            ShowPercent = true;
            ShowDataLabelZeroValues = false;

            //PieGraph specific options
            Explosion = 10;
            PieGraphColors = SupportedPieGraphColors;
            ShowDataLabels = true;
            ShowDataLabelValues = true;
            ShowDataLabelsXAxisTitle = true;
            Separator = String.Empty;

            //Other options
            ForegroundColor = "Brown";
            Color = "Black";
            MaxColumnWidth = 30;
            TitleFont = "Arial";
            LegendFont = "Arial";

            ShowAnswerCount = true;
            ShowPercentageInLegend = false;
            ItemPosition = "LEFT";

            BackgroundColor = "#FFFFFF";
            PlotAreaBackgroundColor = "#FFFFFF";
            LegendBackgroundColor = "#FFFFFF";
            AllowExporting = false;
            LegendVerticalAlign = "bottom";
            LegendAlign = "center";
            LegendLayout = "horizontal";

            WrapTitleChars = 55; 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the position of the item
        /// </summary>
        public string ItemPosition
        {
            get { return this["ItemPosition"]; }
            set { this["ItemPosition"] = value; }
        }

        /// <summary>
        /// Get/Set the font used when displaying the legend.
        /// </summary>
        public string LegendFont
        {
            get { return this["LegendFont"]; }
            set { this["LegendFont"] = value; }
        }

        /// <summary>
        /// Get/Set the font used when displaying the title.
        /// </summary>
        public string TitleFont
        {
            get { return this["TitleFont"]; }
            set { this["TitleFont"] = value; }
        }

        /// <summary>
        /// Get/Set the level of precision used when displaying decimal numbers.
        /// </summary>
        public int Precision
        {
            get { return Utilities.AsInt(this["Precision"], 1); }
            set { this["Precision"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the level of explosion (spacing) used when displaying pie graphs.
        /// </summary>
        public virtual int Explosion
        {
            get { return Utilities.AsInt(this["Explosion"], 10); }
            set { this["Explosion"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if data labels are displayed.
        /// </summary>
        public virtual bool ShowDataLabels
        {
            get { return Utilities.AsBool(this["ShowDataLabels"], true); }
            set { this["ShowDataLabels"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which allows to exprort the chart image
        /// </summary>
        public virtual bool AllowExporting
        {
            get { return Utilities.AsBool(this["AllowExporting"], false); }
            set { this["AllowExporting"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if data labels with no value are displayed.
        /// </summary>
        public virtual bool ShowDataLabelZeroValues
        {
            get { return Utilities.AsBool(this["ShowDataLabelZeroValues"], false); }
            set { this["ShowDataLabelZeroValues"] = value.ToString(); }
        }
        /// <summary>
        /// Get/Set the flag which indicates if data label values are displayed.
        /// </summary>
        public virtual bool ShowDataLabelValues
        {
            get { return Utilities.AsBool(this["ShowDataLabelValues"], true); }
            set { this["ShowDataLabelValues"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the character used to separate labels.
        /// </summary>
        public virtual string Separator
        {
            get { return this["Separator"]; }
            set { this["Separator"] = value; }
        }

        /// <summary>
        /// Get/Set the flag which indicates if labels are displayed on the x axis.
        /// </summary>
        public virtual bool ShowDataLabelsXAxisTitle
        {
            get { return Utilities.AsBool(this["ShowDataLabelsXAxisTitle"], true); }
            set { this["ShowDataLabelsXAxisTitle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the graph's width.
        /// </summary>
        public virtual int Width
        {
            get { return Utilities.AsInt(this["Width"], 800); }
            set { this["Width"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the graph's height.
        /// </summary>
        public virtual int Height
        {
            get { return Utilities.AsInt(this["Height"], 400); }
            set { this["Height"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if a legend should be displayed.
        /// </summary>
        public virtual bool ShowLegend
        {
            get { return Utilities.AsBool(this["ShowLegend"], true); }
            set { this["ShowLegend"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the legends's width.
        /// </summary>
        public virtual int LegendWidth
        {
            get { return Utilities.AsInt(this["LegendWidth"], 200); }
            set { this["LegendWidth"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if the graph title should be displayed.
        /// </summary>
        public virtual bool ShowTitle
        {
            get { return Utilities.AsBool(this["ShowTitle"], true); }
            set { this["ShowTitle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if the response count should be displayed in the title.
        /// </summary>
        public virtual bool ShowResponseCountInTitle
        {
            get { return Utilities.AsBool(this["ShowResponseCountInTitle"], true); }
            set { this["ShowResponseCountInTitle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the color of the title text.
        /// </summary>
        public virtual string TitleColor
        {
            get { return this["TitleColor"]; }
            set { this["TitleColor"] = value; }
        }

        /// <summary>
        /// Get/Set the count of charactes in each line of the wrapped title.
        /// </summary>
        public virtual int WrapTitleChars
        {
            get { return Utilities.AsInt(this["WrapTitleChars"], 55); }
            set { this["WrapTitleChars"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the color of the background.
        /// </summary>
        public virtual string BackgroundColor
        {
            get { return this["BackgroundColor"] ?? "#FFFFFF"; }
            set { this["BackgroundColor"] = value; }
        }

        /// <summary>
        /// Get/Set the color of the plot area background.
        /// </summary>
        public virtual string PlotAreaBackgroundColor
        {
            get { return this["PlotAreaBackgroundColor"] ?? "#FFFFFF"; }
            set { this["PlotAreaBackgroundColor"] = value; }
        }

        /// <summary>
        /// Get/Set the color of the legend background.
        /// </summary>
        public virtual string LegendBackgroundColor
        {
            get { return this["LegendBackgroundColor"] ?? "#FFFFFF"; }
            set { this["LegendBackgroundColor"] = value; }
        }

        /// <summary>
        /// Get/Set the flag which indicates if a gradient should be used when displaying background color.
        /// </summary>
        public virtual bool BackgroundGradient
        {
            get { return Utilities.AsBool(this["BackgroundGradient"], false); }
            set { this["BackgroundGradient"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if a header should be displayed.
        /// </summary>
        public virtual bool ShowHeader
        {
            get { return Utilities.AsBool(this["ShowHeader"], true); }
            set { this["ShowHeader"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the graph spacing.
        /// </summary>
        public virtual int GraphSpacing
        {
            get { return Utilities.AsInt(this["GraphSpacing"], 20); }
            set { this["GraphSpacing"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the flag which indicates if percentages are displayed along side response counts.
        /// </summary>
        public virtual bool ShowPercent
        {
            get { return Utilities.AsBool(this["ShowPercent"], true); }
            set { this["ShowPercent"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the foreground color.
        /// </summary>
        public virtual string ForegroundColor
        {
            get { return this["ForegroundColor"]; }
            set { this["ForegroundColor"] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Color
        {
            get { return this["Color"]; }
            set { this["Color"] = value; }
        }

        /// <summary>
        /// Get/Set the color of pie graph slices.
        /// </summary>
        public string PieGraphColors
        {
            get { return this["PieGraphColors"]; }
            set { this["PieGraphColors"] = value; }
        }

        /// <summary>
        /// Get/Set the maximum width of graph columns.
        /// </summary>
        public virtual int MaxColumnWidth
        {
            get { return Utilities.AsInt(this["MaxColumnWidth"], 30); }
            set { this["MaxColumnWidth"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowAnswerCount
        {
            get { return Utilities.AsBool(this["ShowAnswerCount"], true); }
            set { this["ShowAnswerCount"] = value.ToString(); }
        }

        /// <summary>
        /// Determines if the percentage is displayed next to an option in chart legends.
        /// </summary>
        public bool ShowPercentageInLegend
        {
            get { return Utilities.AsBool(this["ShowPercentageInLegend"], false); }
            set { this["ShowPercentageInLegend"] = value.ToString(); }
        }

        /// <summary>
        /// Get/Set the legend layout
        /// </summary>
        public virtual string LegendLayout
        {
            get { return string.IsNullOrEmpty(this["LegendLayout"]) ? "horizontal" : this["LegendLayout"]; }
            set { this["LegendLayout"] = value; }
        }

        /// <summary>
        /// Get/Set the legend horizontal align
        /// </summary>
        public virtual string LegendAlign
        {
            get { return string.IsNullOrEmpty(this["LegendAlign"]) ? "center" : this["LegendAlign"]; }
            set { this["LegendAlign"] = value; }
        }


        /// <summary>
        /// Get/Set the legend vertical align
        /// </summary>
        public virtual string LegendVerticalAlign
        {
            get { return string.IsNullOrEmpty(this["LegendVerticalAlign"]) ? "bottom" : this["LegendVerticalAlign"]; }
            set { this["LegendVerticalAlign"] = value; }
        }

        /// <summary>
        /// Returns a comma separated list of colors supported by PieGraphs
        /// </summary>
        private static string SupportedPieGraphColors
        {
            get
            {
                StringBuilder colors = new StringBuilder();
                colors.Append("Red");
                colors.Append(",");
                colors.Append("Orange");
                colors.Append(",");
                colors.Append("Yellow");
                colors.Append(",");
                colors.Append("Green");
                colors.Append(",");
                colors.Append("Blue");
                colors.Append(",");
                colors.Append("Purple");
                colors.Append(",");
                colors.Append("Red");
                colors.Append(",");
                colors.Append("Pink");
                colors.Append(",");
                colors.Append("Teal");
                colors.Append(",");
                colors.Append("Maroon");
                colors.Append(",");
                colors.Append("Aqua");
                colors.Append(",");
                colors.Append("Gold");
                colors.Append(",");
                colors.Append("Lime");
                colors.Append(",");
                colors.Append("MidnightBlue");
                colors.Append(",");
                colors.Append("YellowGreen");
                colors.Append(",");
                colors.Append("DarkGreen");

                return colors.ToString();
            }
        }

        #endregion
    }
}