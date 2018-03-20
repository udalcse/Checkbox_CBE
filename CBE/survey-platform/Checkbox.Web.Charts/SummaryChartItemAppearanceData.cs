using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using Checkbox.Analytics.Items;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.UI;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Prezza.Framework.Logging;

namespace Checkbox.Web.Charts
{
    /// <summary>
    /// Summary chart appearance
    /// </summary>
    public class SummaryChartItemAppearanceData : AnalysisItemAppearanceData, ISummaryChartItemAppearance
    {
        private const int verticalLegendMargin = 100;
        private const int horizontalLegendMargin = 250;
        private const int defaultMargin = 50;

        public override string AppearanceCode
        {
            get { return "ANALYSIS_SUMMARY_CHART"; }
        }

         /// <summary>
        /// Returns a comma separated list of colors supported by PieGraphs
        /// </summary>
        private static string DefaultPieGraphColors
        {
            get
            {
                //Default to dundas colors
                Color[] colorList = ChartPaletteColors.GetColorList("Dundas");

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < colorList.Length; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    if (colorList[i].IsNamedColor)
                    {
                        sb.Append(colorList[i].Name);
                    }
                    else
                    {
                        sb.Append(Utilities.ColorToHex(colorList[i]));
                    }
                }

                return sb.ToString();
            }
        }

        public void AdjustAutoMarginValues(string prevLegendAlign, string prevLegendVertAlign)
        {
            const int legendDefaultSize = 70;

            if (prevLegendVertAlign != LegendVerticalAlign)
            {
                if (ShowLegend)
                {
                    switch (LegendVerticalAlign)
                    {
                        case "bottom":
                            if (ChartMarginBottom < legendDefaultSize)
                                ChartMarginBottom = legendDefaultSize;
                            break;
                        case "top":
                            if (ChartMarginTop < legendDefaultSize)
                                ChartMarginTop = legendDefaultSize;
                            if (ChartMarginBottom > legendDefaultSize)
                                ChartMarginBottom -= legendDefaultSize;
                            break;
                        case "center":
                            if (ChartMarginBottom > legendDefaultSize)
                                ChartMarginBottom -= legendDefaultSize;
                            break;
                    }
                }
                else
                {
                    if (ChartMarginBottom > legendDefaultSize)
                        ChartMarginBottom -= legendDefaultSize;
                }
            }

            if (prevLegendAlign != LegendAlign)
            {
                if (ShowLegend)
                {
                    switch (LegendAlign)
                    {
                        case "right":
                            if (ChartMarginRight < legendDefaultSize)
                                ChartMarginRight = legendDefaultSize;
                            if (ChartMarginLeft > legendDefaultSize)
                                ChartMarginLeft -= legendDefaultSize;
                            break;
                        case "left":
                            if (ChartMarginRight > legendDefaultSize)
                                ChartMarginRight -= legendDefaultSize;
                            if (ChartMarginLeft < legendDefaultSize)
                                ChartMarginLeft = legendDefaultSize;
                            break;
                        case "center":
                            if (ChartMarginRight > legendDefaultSize)
                                ChartMarginRight -= legendDefaultSize;
                            if (ChartMarginLeft > legendDefaultSize)
                                ChartMarginLeft -= legendDefaultSize;
                            break;
                    }
                }
                else
                {
                    if (ChartMarginRight > legendDefaultSize)
                        ChartMarginRight -= legendDefaultSize;
                    if (ChartMarginLeft > legendDefaultSize)
                        ChartMarginLeft -= legendDefaultSize;
                }
            }
        }

        public void AdjustAutoMarginValuesForSpecificItems(int numberOfSourceItems, int? sourceItemId=null)
        {
            if (SelectedGraph == GraphType.BarGraph)
            {
                const int defaultColumnSize = 60;

                if (sourceItemId != null)
                {
                    //calculate the minimal margin
                    var sourceItem = SurveyMetaDataProxy.GetItemData((int) sourceItemId, true);
                    if (sourceItem.Options != null && sourceItem.Options.Count > 0)
                    {
                        var optionsText = (from optionId in sourceItem.Options
                                           let optionData = SurveyMetaDataProxy.GetOptionData(optionId, sourceItemId, true)
                                           where optionData != null
                                           select Utilities.DecodeAndStripHtml(optionData.GetText(sourceItem.PopulatedLanguages.FirstOrDefault())).ToArray());
                        if (optionsText.Any())
                        {
                            int minimalLeftMargin = optionsText.Max(option => option.Length) * 7;

                            if (ChartMarginLeft < minimalLeftMargin)
                            {
                                ChartMarginLeft = minimalLeftMargin;
                            }
                        }
                    }
                }
                if (ChartMarginLeft < defaultColumnSize * numberOfSourceItems && sourceItemId == null)
                    ChartMarginLeft = defaultColumnSize * numberOfSourceItems;
                if (!ShowLegend && ChartMarginRight > defaultColumnSize / 2)
                    ChartMarginRight = defaultColumnSize/2;
                if (!ShowLegend && ChartMarginBottom > defaultColumnSize / 2)
                    ChartMarginBottom = defaultColumnSize / 2;
            }
        }

        public void AdjustTopMarginForTitle(int numberOfSourceItems)
        {
            //adjust top margin for header
            var newMarginValue = numberOfSourceItems * 2 * TitleFontSize;
            if (ChartMarginTop < newMarginValue)
            {
                if (Height * 4 / 10 < newMarginValue)
                    ChartMarginTop = Height * 4 / 10;
                else
                    ChartMarginTop = newMarginValue;
            }
        }

        public void UpdateWrapTitleChars()
        {
            if (WrapTitleChars > 25 && ShowResponseCountInTitle && TitleFontSize >= 18 && Width <= 600)
                WrapTitleChars = 25;
        }

        /// <summary>
        /// Set default values
        /// </summary>
        public SummaryChartItemAppearanceData()
        {
            FontFamily = "Arial";
            TextColor = "#000000";
            TitleFontSize = 13;
            LegendFontSize = 12;
            LabelFontSize = 10;
            BorderSkinStyle = BorderSkinStyle.Emboss;
            BorderLineStyle = ChartDashStyle.NotSet;
            BorderLineWidth = 0;
            BorderLineColor = "#000000";
            BorderFrameBackgroundColor = "#a3a3a3";
            DoughnutRadius = 70;
            Exploded = false;
            Enable3D = true;
            XAngle = 30;
            YAngle = 30;
            Perspective = 0;
            BarDrawingStyle = DrawingStyle.Default;
            OptionsOrder = OptionsOrder.Survey;
            HatchStyle = ChartHatchStyle.None;
            BarColor = "#2368ff";
            HatchBackgroundColor = "#FFFFFF";
            BackgroundColor = "#FFFFFF";
            PlotAreaBackgroundColor = "#FFFFFF";
            LegendBackgroundColor = "#FFFFFF";
            PieBorderColor = "#FFFFFF";
            AllowExporting = true;
            PieGraphColors = DefaultPieGraphColors;
            PixelPointWidth = 50;

            try
            {
                PieLabelStyle = (PieLabelStyles)Enum.Parse(typeof(PieLabelStyles), ApplicationManager.AppSettings.AutogenReportDefaultPieLabelStyle);
            }
            catch
            {
                Logger.Write(string.Format("AutogenReportDefaultPieLabelStyle is set to an invalid value [{0}].", ApplicationManager.AppSettings.AutogenReportDefaultPieLabelStyle), "Warning", -1, -1, Severity.Warning);
                PieLabelStyle = PieLabelStyles.Inside;
            }

            EnableXAxis = true;
            EnableYAxis = true;
            EnableYAxisGrid = true;
            EnableXAxisGrid = true;
            DisplayAnswerTextInDataPoint = false;
            Transparency = 100;

            BorderRadius = 5;
        }

        /// <summary>
        /// Get/set point width for graphs.  This affects the bar width for bar/column graphs.
        /// </summary>
        public int PixelPointWidth
        {
            get { return Utilities.AsInt(this["PixelPointWidth"], 50); }
            set { this["PixelPointWidth"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the radius of doughnut graphs.  Possible values are 0-99.  Smaller values = larger doughnut hole.
        /// A value of 99 means there is no hole, and the display will be the same as a pie chart.  If the graph type
        /// is PieGraph, this value always returns 99.
        /// </summary>
        public int DoughnutRadius
        {
            get { return Utilities.AsInt(this["DoughnutRadius"], 70); }
            set
            {
                if (value <= 99 && value >= 0)
                {
                    this["DoughnutRadius"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Get/set whether pie graph slices are "exploded" or not
        /// </summary>
        public bool Exploded
        {
            get { return Utilities.AsBool(this["Exploded"], false); }
            set { this["Exploded"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set whether 3D is enabled for this item
        /// </summary>
        public bool Enable3D
        {
            get { return Utilities.AsBool(this["Enable3D"], true); }
            set { this["Enable3D"] = value.ToString(); }
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
        /// Get/set X view angle for 3d charts.  Valid values are -180 to +180.
        /// </summary>
        public int XAngle
        {
            get { return Utilities.AsInt(this["3dXAngle"], 30); }
            set
            {
                if (value <= 180 && value >= -180)
                {
                    this["3dXAngle"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Get/set Y view angle for 3d charts.  Valid values are -180 to +180.
        /// </summary>
        public int YAngle
        {
            get { return Utilities.AsInt(this["3dYAngle"], 30); }
            set
            {
                if (value <= 180 && value >= -180)
                {
                    this["3dYAngle"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Get/set amount of perspective to apply to 3d charts.    Valid values are 0 to 100.
        /// </summary>
        public int Perspective
        {
            get { return Utilities.AsInt(this["3dPerspective"], 0); }
            set
            {
                if (value <= 100 && value >= 0)
                {
                    this["3dPerspective"] = value.ToString();
                }
            }
        }

        /// <summary>
        /// Get/set the drawing style for bar and column charts
        /// </summary>
        public DrawingStyle BarDrawingStyle
        {
            get
            {
                try { return (DrawingStyle)Enum.Parse(typeof(DrawingStyle), this["BarDrawingStyle"]); }
                catch { return DrawingStyle.Default; }
            }
            set { this["BarDrawingStyle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set the options order
        /// </summary>
        public OptionsOrder OptionsOrder
        {
            get
            {
                try { return (OptionsOrder)Enum.Parse(typeof(OptionsOrder), this["OptionsOrder"]); }
                catch { return OptionsOrder.Default; }
            }
            set { this["OptionsOrder"] = value.ToString(); }
        }


        /// <summary>
        /// Get/set the hatching style to use for bar/column charts
        /// </summary>
        public ChartHatchStyle HatchStyle
        {
            get
            {
                try { return (ChartHatchStyle)Enum.Parse(typeof(ChartHatchStyle), this["HatchStyle"]); }
                catch { return ChartHatchStyle.None; }
            }
            set { this["HatchStyle"] = value.ToString(); }
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
        /// Get/set the font size for the label
        /// </summary>
        public int LabelFontSize
        {
            get { return Utilities.AsInt(this["LabelFontSize"], 10); }
            set { this["LabelFontSize"] = value.ToString(); }
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
        /// Get the bar color for bar/column/line graphs
        /// </summary>
        public string BarColor
        {
            get { return this["BarColor"]; }
            set { this["BarColor"] = value; }
        }

        /// <summary>
        /// Get the background color for hatching
        /// </summary>
        public string HatchBackgroundColor
        {
            get { return this["HatchBackgroundColor"]; }
            set { this["HatchBackgroundColor"] = value; }
        }

        /// <summary>
        /// Get/set the border style
        /// </summary>
        public BorderSkinStyle BorderSkinStyle
        {
            get
            {
                try { return (BorderSkinStyle)Enum.Parse(typeof(BorderSkinStyle), this["BorderStyle"]); }
                catch { return BorderSkinStyle.Emboss; }
            }
            set { this["BorderStyle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set line style for border
        /// </summary>
        public ChartDashStyle BorderLineStyle
        {
            get
            {
                try
                {
                    return (ChartDashStyle)Enum.Parse(typeof(ChartDashStyle), this["BorderLineStyle"]);
                }
                catch { return ChartDashStyle.Solid; }
            }
            set { this["BorderLineStyle"] = value.ToString(); }
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
        /// Get/set the border frame background color
        /// </summary>
        public string BorderFrameBackgroundColor
        {
            get { return this["BorderFrameBackgroundColor"]; }
            set { this["BorderFrameBackgroundColor"] = value; }
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
        /// Return an array of color objects for the chart palette.  Colors include
        /// any configured transparency information.
        /// </summary>
        /// <returns></returns>
        public Color[] GetPaletteColors()
        {
            List<Color> colors;

            if (SelectedGraph == GraphType.PieGraph || SelectedGraph == GraphType.Doughnut)
            {
                colors = new List<Color>();

                foreach (string color in PieGraphColors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    colors.Add(Utilities.GetColor(color.Trim(), Transparency, false));
                }
            }
            else
            {
                colors = new List<Color> { Utilities.GetColor(BarColor, Transparency, false) };
            }

            return colors.ToArray();
        }

        /// <summary>
        /// Get/set the style for labels in pie/doughnut graphs
        /// </summary>
        public PieLabelStyles PieLabelStyle
        {
            get
            {
                try
                { return (PieLabelStyles)Enum.Parse(typeof(PieLabelStyles), this["PieLabelStyle"]); }
                catch { return PieLabelStyles.Inside; }
            }
        
            set { this["PieLabelStyle"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set bar label style
        /// </summary>
        public string BarLabelStyle
        {
            get { return this["BarLabelStyle"]; }
            set { this["BarLabelStyle"] = value; }
        }

        /// <summary>
        /// Get/set whether x axis should be displayed
        /// </summary>
        public bool EnableXAxis
        {
            get { return Utilities.AsBool(this["EnableXAxis"], true); }
            set { this["EnableXAxis"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set whether y axis should be displayed
        /// </summary>
        public bool EnableYAxis
        {
            get { return Utilities.AsBool(this["EnableYAxis"], true); }
            set { this["EnableYAxis"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set whether X axis grid should be displayed
        /// </summary>
        public bool EnableXAxisGrid
        {
            get { return Utilities.AsBool(this["EnableXAxisGrid"], true); }
            set { this["EnableXAxisGrid"] = value.ToString(); }
        }


        /// <summary>
        /// Get/set whether Y axis grid should be displayed
        /// </summary>
        public bool EnableYAxisGrid
        {
            get { return Utilities.AsBool(this["EnableYAxisGrid"], true); }
            set { this["EnableYAxisGrid"] = value.ToString(); }
        }

        /// <summary>
        /// Get/set whether to show the answer text with the data point
        /// </summary>
        public bool DisplayAnswerTextInDataPoint
        {
            get { return Utilities.AsBool(this["DisplayAnswerTextInDataPoint"], false); }
            set { this["DisplayAnswerTextInDataPoint"] = value.ToString(); }
        }


        /// <summary>
        /// Get/set the percentage of transparency for the chart colors
        /// </summary>
        public int Transparency
        {
            get { return Utilities.AsInt(this["Transparency"], 100); }
            set { this["Transparency"] = value.ToString(); }
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
        /// 
        /// </summary>
        public int LegendMargin
        {
            get { return Utilities.AsInt(this["LegendMargin"], 15); }
            set { this["LegendMargin"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LegendTopItemMargin
        {
            get { return Utilities.AsInt(this["LegendTopItemMargin"], 0); }
            set { this["LegendTopItemMargin"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LegendBottomItemMargin
        {
            get { return Utilities.AsInt(this["LegendBottomItemMargin"], 0); }
            set { this["LegendBottomItemMargin"] = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TitleMargin
        {
            get { return Utilities.AsInt(this["TitleMargin"], 15); }
            set { this["TitleMargin"] = value.ToString(); }
        }


        //#region CRUD

        ///// <summary>
        ///// Create a new instance of Dundas Appearance Data in the database
        ///// </summary>
        ///// <param name="t"></param>
        //protected override void CreateAnalysisItemAppearanceData(IDbTransaction t)
        //{
        //    UpSert(t);
        //}

        ///// <summary>
        ///// Update existing dundas frequency appearance data
        ///// </summary>
        ///// <param name="t"></param>
        //protected override void UpdateAnalysisItemAppearanceData(IDbTransaction t)
        //{
        //    UpSert(t);
        //}


        ///// <summary>
        ///// Handle update/insert of dundas frequency appearance data into the database
        ///// </summary>
        ///// <param name="t"></param>
        //private void UpSert(IDbTransaction t)
        //{
        //    if (!ID.HasValue)
        //    {
        //        throw new Exception("Unable to save dundas frequency appearance data with no id.");
        //    }

        //    Database db = DatabaseFactory.CreateDatabase();
        //    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Appearance_DundasFrequency_UpSert");

        //    AddInParameterToCommand(command, "AppearanceID", DbType.Int32, ID);
        //    AddInParameterToCommand(command, "ShowResponseCountInTitle", DbType.Boolean, ShowResponseCountInTitle);
        //    AddInParameterToCommand(command, "DoughnutRadius", DbType.Int32, DoughnutRadius);
        //    AddInParameterToCommand(command, "Exploded", DbType.Boolean, Exploded);
        //    AddInParameterToCommand(command, "Enable3d", DbType.Boolean, Enable3D);
        //    AddInParameterToCommand(command, "XAngle", DbType.Int32, XAngle);
        //    AddInParameterToCommand(command, "YAngle", DbType.Int32, YAngle);
        //    AddInParameterToCommand(command, "Perspective", DbType.Int32, Perspective);
        //    AddInParameterToCommand(command, "DrawingStyle", DbType.String, BarDrawingStyle.ToString());
        //    AddInParameterToCommand(command, "HatchStyle", DbType.String, HatchStyle.ToString());
        //    AddInParameterToCommand(command, "ChartWidth", DbType.Int32, GraphWidth);
        //    AddInParameterToCommand(command, "ChartHeight", DbType.Int32, GraphHeight);
        //    AddInParameterToCommand(command, "BackgroundColor", DbType.String, BackgroundColor);
        //    AddInParameterToCommand(command, "FontFamily", DbType.String, FontFamily);
        //    AddInParameterToCommand(command, "TitleFontSize", DbType.Int32, TitleFontSize);
        //    AddInParameterToCommand(command, "LegendFontSize", DbType.Int32, LegendFontSize);
        //    AddInParameterToCommand(command, "LabelFontSize", DbType.Int32, LabelFontSize);
        //    AddInParameterToCommand(command, "TextColor", DbType.String, TextColor);
        //    AddInParameterToCommand(command, "BorderLineWidth", DbType.Int32, BorderLineWidth);
        //    AddInParameterToCommand(command, "BorderFrameBGColor", DbType.String, BorderFrameBackgroundColor);
        //    AddInParameterToCommand(command, "BorderLineColor", DbType.String, BorderLineColor);
        //    AddInParameterToCommand(command, "BarColor", DbType.String, BarColor);
        //    AddInParameterToCommand(command, "HatchBGColor", DbType.String, HatchBackgroundColor);
        //    AddInParameterToCommand(command, "BorderLineStyle", DbType.String, BorderLineStyle.ToString());
        //    AddInParameterToCommand(command, "BorderSkinStyle", DbType.String, BorderStyle.ToString());
        //    AddInParameterToCommand(command, "ShowPercent", DbType.Boolean, ShowPercent);
        //    AddInParameterToCommand(command, "PieGraphColors", DbType.String, PieGraphColors);
        //    AddInParameterToCommand(command, "PieLabelStyle", DbType.String, PieLabelStyle.ToString());
        //    AddInParameterToCommand(command, "PixelPointWidth", DbType.Int32, PixelPointWidth);
        //    AddInParameterToCommand(command, "ShowAnswerCount", DbType.Boolean, ShowAnswerCount);
        //    AddInParameterToCommand(command, "BarLabelStyle", DbType.String, BarLabelStyle);
        //    AddInParameterToCommand(command, "EnableXAxisGrid", DbType.Boolean, EnableXAxisGrid);
        //    AddInParameterToCommand(command, "EnableYAxisGrid", DbType.Boolean, EnableYAxisGrid);
        //    AddInParameterToCommand(command, "EnableXAxis", DbType.Boolean, EnableXAxis);
        //    AddInParameterToCommand(command, "EnableYAxis", DbType.Boolean, EnableYAxis);
        //    AddInParameterToCommand(command, "ShowAnswerTextInPointLabel", DbType.Boolean, DisplayAnswerTextInDataPoint);
        //    AddInParameterToCommand(command, "Transparency", DbType.Int32, Transparency);
        //    AddInParameterToCommand(command, "ShowDLZeroValues", DbType.Boolean, ShowDataLabelZeroValues);
        //    AddInParameterToCommand(command, "ShowPercentageInLegend", DbType.Boolean, ShowPercentageInLegend);

        //    db.ExecuteNonQuery(command, t);
        //}

        ///// <summary>
        ///// Add a parameter to a db command, making the appropriate substitution of DBNull for null
        ///// </summary>
        ///// <param name="command"></param>
        ///// <param name="parameterName"></param>
        ///// <param name="parameterType"></param>
        ///// <param name="value"></param>
        //private static void AddInParameterToCommand(DBCommandWrapper command, string parameterName, DbType parameterType, object value)
        //{
        //    //Null check no longer necessary due to fix in database provider
        //    command.AddInParameter(parameterName, parameterType, value);
        //}

        ///// <summary>
        ///// Delete dundas frequency appearance data from the database
        ///// </summary>
        ///// <param name="t"></param>
        //public override void Delete(IDbTransaction t)
        //{
        //    base.Delete(t);

        //    if (!ID.HasValue)
        //    {
        //        throw new Exception("Unable to delete dundas frequency appearance data with no id.");
        //    }

        //    Database db = DatabaseFactory.CreateDatabase();
        //    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Appearance_DundasFrequency_Delete");
        //    command.AddInParameter("AppearanceID", DbType.Int32, ID);

        //    db.ExecuteNonQuery(command, t);
        //}

        ///// <summary>
        ///// Get the configuration for a dundas frequency item
        ///// </summary>
        ///// <returns></returns>
        //protected override DataSet GetConcreteConfigurationDataSet()
        //{
        //    if (!ID.HasValue)
        //    {
        //        throw new Exception("Unable to get dundas frequency appearance data with no id.");
        //    }

        //    Database db = DatabaseFactory.CreateDatabase();
        //    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Appearance_DundasFrequency_Get");
        //    command.AddInParameter("AppearanceID", DbType.Int32, ID);

        //    DataSet ds = new DataSet();

        //    db.LoadDataSet(
        //        command,
        //        ds,
        //        new[] { DataTableName });

        //    return ds;
        //}

        ///// <summary>
        ///// Load the appearance data from a data row
        ///// </summary>
        ///// <param name="data"></param>
        //protected override void LoadFromDataRow(DataRow data)
        //{
        //    //Note: Graph type comes from parent table
        //    DoughnutRadius = DbUtility.GetValueFromDataRow(data, "DoughnutRadius", 70);
        //    Exploded = DbUtility.GetValueFromDataRow(data, "Exploded", false);
        //    Enable3D = DbUtility.GetValueFromDataRow(data, "Enable3d", false);
        //    XAngle = DbUtility.GetValueFromDataRow(data, "XAngle", 30);
        //    YAngle = DbUtility.GetValueFromDataRow(data, "YAngle", 30);
        //    Perspective = DbUtility.GetValueFromDataRow(data, "Perspective", 0);
        //    ShowResponseCountInTitle = DbUtility.GetValueFromDataRow(data, "ShowResponseCountInTitle", false);
        //    GraphWidth = DbUtility.GetValueFromDataRow(data, "ChartWidth", 600);
        //    GraphHeight = DbUtility.GetValueFromDataRow(data, "ChartHeight", 500);
        //    BackgroundColor = DbUtility.GetValueFromDataRow(data, "BackgroundColor", "#FFFFFF");
        //    FontFamily = DbUtility.GetValueFromDataRow(data, "FontFamily", "Arial");
        //    TitleFontSize = DbUtility.GetValueFromDataRow(data, "TitleFontSize", 18);
        //    LegendFontSize = DbUtility.GetValueFromDataRow(data, "LegendFontSize", 12);
        //    LabelFontSize = DbUtility.GetValueFromDataRow(data, "LabelFontSize", 10);
        //    TextColor = DbUtility.GetValueFromDataRow(data, "TextColor", "#000000");
        //    BorderLineWidth = DbUtility.GetValueFromDataRow(data, "BorderLineWidth", 1);
        //    BorderFrameBackgroundColor = DbUtility.GetValueFromDataRow(data, "BorderFrameBGColor", "#FFFFFF");
        //    BorderLineColor = DbUtility.GetValueFromDataRow(data, "BorderLineColor", "#000000");
        //    BarColor = DbUtility.GetValueFromDataRow(data, "BarColor", "#2368ff");
        //    HatchBackgroundColor = DbUtility.GetValueFromDataRow(data, "HatchBGColor", "#FFFFFF");
        //    ShowPercent = DbUtility.GetValueFromDataRow(data, "ShowPercent", false);
        //    ShowAnswerCount = DbUtility.GetValueFromDataRow(data, "ShowAnswerCount", false);
        //    PieGraphColors = DbUtility.GetValueFromDataRow(data, "PieGraphColors", DefaultPieGraphColors);
        //    PixelPointWidth = DbUtility.GetValueFromDataRow(data, "PixelPointWidth", 100);
        //    ShowPercentageInLegend = DbUtility.GetValueFromDataRow(data, "ShowPercentageInLegend", false);

        //    string drawingStyle = DbUtility.GetValueFromDataRow(data, "DrawingStyle", "Default");
        //    string chartHatchStyle = DbUtility.GetValueFromDataRow(data, "HatchStyle", "None");
        //    string borderDashStyle = DbUtility.GetValueFromDataRow(data, "BorderLineStyle", "NotSet");
        //    string borderSkinStyle = DbUtility.GetValueFromDataRow(data, "BorderSkinStyle", "None");

        //    string defaultPieLabelStlye = ApplicationManager.AppSettings.AutogenReportDefaultPieLabelStyle;
        //    if (!"Inside".Equals(defaultPieLabelStlye) || !"Outside".Equals(defaultPieLabelStlye) || !"Disabled".Equals(defaultPieLabelStlye))
        //    {
        //        defaultPieLabelStlye = "Inside";
        //    }

        //    string pieLabelStyle = DbUtility.GetValueFromDataRow(data, "PieLabelStyle", defaultPieLabelStlye);

        //    BarDrawingStyle = (DrawingStyle)Enum.Parse(typeof(DrawingStyle), drawingStyle);
        //    HatchStyle = (ChartHatchStyle)Enum.Parse(typeof(ChartHatchStyle), chartHatchStyle);
        //    BorderLineStyle = (ChartDashStyle)Enum.Parse(typeof(ChartDashStyle), borderDashStyle);
        //    BorderStyle = (BorderSkinStyle)Enum.Parse(typeof(BorderSkinStyle), borderSkinStyle);
        //    PieLabelStyle = (PieLabelStyles)Enum.Parse(typeof(PieLabelStyles), pieLabelStyle);

        //    BarLabelStyle = DbUtility.GetValueFromDataRow(data, "BarLabelStyle", "Outside");
        //    EnableXAxisGrid = DbUtility.GetValueFromDataRow(data, "EnableXAxisGrid", true);
        //    EnableYAxisGrid = DbUtility.GetValueFromDataRow(data, "EnableXAxisGrid", true);
        //    EnableXAxis = DbUtility.GetValueFromDataRow(data, "EnableXAxis", true);
        //    EnableYAxis = DbUtility.GetValueFromDataRow(data, "EnableYAxis", true);
        //    DisplayAnswerTextInDataPoint = DbUtility.GetValueFromDataRow(data, "ShowAnswerTextInPointLabel", false);
        //    Transparency = DbUtility.GetValueFromDataRow(data, "Transparency", 100);
        //    ShowDataLabelZeroValues = DbUtility.GetValueFromDataRow(data, "ShowDLZeroValues", false);
        //}


        //#endregion

        /// <summary>
        /// Copy the apperance data
        /// </summary>
        /// <returns></returns>
        public override AppearanceData Copy()
        {
            SummaryChartItemAppearanceData copy = new SummaryChartItemAppearanceData();
            copy.BackgroundColor = BackgroundColor;
            copy.BackgroundGradient = BackgroundGradient;
            copy.BarColor = BarColor;
            copy.BarDrawingStyle = BarDrawingStyle;
            copy.BorderFrameBackgroundColor = BorderFrameBackgroundColor;
            copy.BorderLineColor = BorderLineColor;
            copy.BorderLineStyle = BorderLineStyle;
            copy.BorderLineWidth = BorderLineWidth;
            copy.BorderSkinStyle = BorderSkinStyle;
            copy.Color = Color;
            copy.DoughnutRadius = DoughnutRadius;
            copy.Enable3D = Enable3D;
            copy.Exploded = Exploded;
            copy.Explosion = Explosion;
            copy.FontFamily = FontFamily;
            copy.ForegroundColor = ForegroundColor;
            copy.Height = Height;
            //copy.GraphType = GraphType;
            copy.GraphSpacing = GraphSpacing;
            copy.Width = Width;
            copy.HatchBackgroundColor = HatchBackgroundColor;
            copy.OptionsOrder = OptionsOrder;
            copy.HatchStyle = HatchStyle;
            copy.ItemPosition = ItemPosition;
            copy.LabelFontSize = LabelFontSize;
            copy.LegendFont = LegendFont;
            copy.LegendFontSize = LegendFontSize;
            copy.LegendWidth = LegendWidth;
            copy.MaxColumnWidth = MaxColumnWidth;
            copy.Perspective = Perspective;
            copy.PieGraphColors = PieGraphColors;
            copy.PieLabelStyle = PieLabelStyle;
            copy.Precision = Precision;
            copy.Separator = Separator;
            copy.ShowDataLabels = ShowDataLabels;
            copy.ShowDataLabelsXAxisTitle = ShowDataLabelsXAxisTitle;
            copy.ShowDataLabelValues = ShowDataLabelValues;
            copy.ShowDataLabelZeroValues = ShowDataLabelZeroValues;
            copy.ShowHeader = ShowHeader;
            copy.ShowLegend = ShowLegend;
            copy.ShowPercent = ShowPercent;
            copy.ShowResponseCountInTitle = ShowResponseCountInTitle;
            copy.ShowTitle = ShowTitle;
            copy.TextColor = TextColor;
            copy.TitleColor = TitleColor;
            copy.TitleFont = TitleFont;
            copy.TitleFontSize = TitleFontSize;
            copy.XAngle = XAngle;
            copy.YAngle = YAngle;
            copy.PixelPointWidth = PixelPointWidth;
            copy.ShowAnswerCount = ShowAnswerCount;
            copy.BarLabelStyle = BarLabelStyle;
            copy.EnableXAxisGrid = EnableXAxisGrid;
            copy.EnableYAxisGrid = EnableYAxisGrid;
            copy.EnableXAxis = EnableXAxis;
            copy.EnableYAxis = EnableYAxis;
            copy.DisplayAnswerTextInDataPoint = DisplayAnswerTextInDataPoint;
            copy.Transparency = Transparency;
            copy.ShowPercentageInLegend = ShowPercentageInLegend;

            return copy;
        }

        protected GraphType SelectedGraph
        {
            get
            {
                //avoid an exception
                if (this["GraphType"] == null)
                    return GraphType.ColumnGraph;

                try
                { 
                    return (GraphType)Enum.Parse(typeof(GraphType), this["GraphType"]); 
                }
                catch 
                { 
                    return GraphType.ColumnGraph; 
                }
            }
            set { this["GraphType"] = value.ToString(); }
        }


        public void correctChartMargins()
        {
            if (LegendVerticalAlign.Equals("top", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ChartMarginTop < verticalLegendMargin)
                    ChartMarginTop = verticalLegendMargin;
                if (ChartMarginBottom > defaultMargin)
                    ChartMarginBottom = defaultMargin;
            }
            else if (LegendVerticalAlign.Equals("bottom", StringComparison.InvariantCultureIgnoreCase))
            {
                if (ChartMarginBottom < verticalLegendMargin)
                    ChartMarginBottom = verticalLegendMargin;
                if (ChartMarginTop > defaultMargin)
                    ChartMarginTop = defaultMargin;
            }
            if (LegendVerticalAlign.Equals("middle", StringComparison.InvariantCultureIgnoreCase))
            {
                if (LegendAlign.Equals("left", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ChartMarginLeft < 250)
                        ChartMarginLeft = 250;
                    if (ChartMarginRight > defaultMargin)
                        ChartMarginRight = defaultMargin;
                }
                else if (LegendAlign.Equals("right", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ChartMarginRight < 250)
                        ChartMarginRight = 250;
                    if (ChartMarginLeft > defaultMargin)
                        ChartMarginLeft = defaultMargin;
                }
            }
        }
    }
}
