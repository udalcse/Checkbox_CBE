using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.Globalization;

namespace Checkbox.Web.Charts
{
    /// <summary>
    /// Helper class which provides methods to improve chart layout based on different conditions.
    /// </summary>
    public static class ChartLayoutUtility
    {
        #region Internal fields

        // Minimum title font size
        private const double MIN_TITLEFONT_SIZE = 8;

        // Minimum axis label length after which we will not trncate it anymore
        private const double MIN_AXIS_LABEL_LENGTH = 5;

        #endregion Internal fields

        /// <summary>
        /// Method which improves layout of the chart control.
        /// </summary>
        public static void ImproveLayout(Chart chartControl)
        {
            if (chartControl != null)
            {
                Graphics graphics = null;

                try
                {
                    graphics = CreateGraphics();

                    // Change Legend docking based on the proportions of the chart.
                    // When Width going to be twice as big as height we will try re-docking
                    // legend to the right of the chart because there is more horizontal space.
                    LegendSetDocking(chartControl, graphics, 2);

                    // Prevent legend from taking significant amount of space when chart 
                    // becomes too small by reducing available space or hiding the legend.
                    LegendHideOrLimitSize(chartControl, graphics, 100, 30);

                    // Prevent title from taking significant amount of space when chart 
                    // becomes too small by reducing available space or hiding the title.
                    TitleHideOrLimitSize(chartControl, graphics, 100, 20);

                    // Truncate axes labels to prevent them from taking over the whole chart.
                    LimitAxesLabelsSize(chartControl, graphics, 30);

                    // Only displays labels for the N largest slices in the series.
                    LimitPieLabelsNumber(chartControl, graphics, 7);
                }
                finally
                {
                    if (graphics != null)
                    {
                        graphics.Dispose();
                    }
                }
            }
        }

        #region Axes Methods

        /// <summary>
        /// Chart value is formatted as a string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void FormatChartNumber(object sender, FormatNumberEventArgs e)
        {
            // Check the range of Y axis values and present them in thousands, 
            // millions or billions if nessesary.
            Axis axis = sender as Axis;
            if (axis != null &&
                e.ElementType == ChartElementType.AxisLabels &&
                e.ValueType != ChartValueType.String)
            {
                if (axis.AxisName == AxisName.Y || axis.AxisName == AxisName.Y2)
                {
                    double devider = 1.0;
                    string title = string.Empty;
                    string labelFormatString = string.Empty;
                    if (axis.Maximum > 1000000000 ||
                        axis.Minimum < -1000000000)
                    {
                        title = "(in billions)";
                        devider = 1000000000.0;
                    }
                    else if (axis.Maximum > 1000000 ||
                        axis.Minimum < -1000000)
                    {
                        title = "(in millions)";
                        devider = 1000000.0;
                    }
                    else if (axis.Maximum > 1000 ||
                        axis.Minimum < -1000)
                    {
                        title = "(in thousands)";
                        devider = 1000.0;
                    }

                    if (title.Length > 0)
                    {
                        // Add axis title.
                        if (!axis.Title.EndsWith(title))
                        {
                            if (axis.Title.Length > 0)
                            {
                                title = " " + title;
                            }
                            axis.Title += title;
                        }

                        // Apply axis labels format to show data in thousands, ...
                        e.LocalizedValue = string.Format("{0:" + e.Format + "}", e.Value / devider);
                    }
                }
            }
        }

        /// <summary>
        /// Limits axes labels size to fit specified maximumrestrictions.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="maxAvailableSize">Maximum axis labels size.</param>
        /// <param name="chartControl"></param>
        private static void LimitAxesLabelsSize(Chart chartControl, Graphics graphics, int maxAvailableSize)
        {
            foreach (ChartArea chartArea in chartControl.ChartAreas)
            {
                LimitAxesLabelsSize(chartControl, graphics, chartArea, chartArea.AxisX, maxAvailableSize);
                LimitAxesLabelsSize(chartControl, graphics, chartArea, chartArea.AxisX2, maxAvailableSize);
                LimitAxesLabelsSize(chartControl, graphics, chartArea, chartArea.AxisY, maxAvailableSize);
                LimitAxesLabelsSize(chartControl, graphics, chartArea, chartArea.AxisY2, maxAvailableSize);
            }
        }

        /// <summary>
        /// Limits axes labels size to fit specified maximumrestrictions.
        /// </summary>
        /// <param name="chartArea"></param>
        /// <param name="axis">Axis to check.</param>
        /// <param name="maxAvailableSize">Maximum axis labels size.</param>
        /// <param name="chartControl"></param>
        /// <param name="graphics"></param>
        private static void LimitAxesLabelsSize(Chart chartControl, Graphics graphics, ChartArea chartArea, Axis axis, int maxAvailableSize)
        {
            if (axis.CustomLabels.Count > 0)
            {
                while (!IsAxisLabelsFit(chartControl, graphics, chartArea, axis, maxAvailableSize))
                {
                    if (!TruncateAxisLabels(chartArea, axis))
                    {
                        break;
                    }
                }
            }
        }

        private static bool TruncateAxisLabels(ChartArea chartArea, Axis axis)
        {
            const int charactersToRemove = 5;
            const string truncatedStringSuffix = "...";

            // Get maximum length of the label
            int labelLength = 0;
            bool trucated = false;
            foreach (CustomLabel label in axis.CustomLabels)
            {
                // Remove '...' at the end
                if (label.Text.EndsWith(truncatedStringSuffix))
                {
                    label.Text = label.Text.Substring(0, label.Text.Length - 3);
                }

                labelLength = Math.Max(labelLength, label.Text.Length);
            }

            // Reduce label size and chech for restrictions
            labelLength -= charactersToRemove;
            if (labelLength >= MIN_AXIS_LABEL_LENGTH)
            {
                foreach (CustomLabel label in axis.CustomLabels)
                {
                    if (label.Text.Length > labelLength)
                    {
                        label.Text = label.Text.Substring(0, labelLength) + truncatedStringSuffix;
                        trucated = true;
                    }
                }
            }

            return trucated;
        }

        /// <summary>
        /// Check if specified axis labels fits.
        /// </summary>
        /// <param name="axis">Axis to check.</param>
        /// <param name="maxAvailableSize">Maximum available size.</param>
        /// <param name="chartControl"></param>
        /// <param name="graphics"></param>
        /// <param name="chartArea"></param>
        /// <returns>True if labels fits.</returns>
        private static bool IsAxisLabelsFit(Chart chartControl, Graphics graphics, ChartArea chartArea, Axis axis, int maxAvailableSize)
        {
            // Calculate title maximum size in pixels based on the docking side
            bool isVerticalAxis = false;
            double maxPixelsize = chartControl.Height.Value / 100.0 * maxAvailableSize;
            if (axis.AxisName == AxisName.Y || axis.AxisName == AxisName.Y2 || IsAxisFlipped(chartControl))
            {
                isVerticalAxis = true;
                maxPixelsize = chartControl.Width.Value / 100.0 * maxAvailableSize;
            }

            // Measure size of all axeis labels
            SizeF labelsSize = MeasureAllAxisLabels(chartControl, graphics, chartArea, axis);

            // NOTE: For both verical and horizontal axes we will be using the width of 
            // the label as the most critical size. We also make an assumption that long
            // axis labels on the horizontal axis will be positioned vertically.
            return labelsSize.Width <= maxPixelsize;
        }

        /// <summary>
        /// Method estimates axis labels size.
        /// Thing NOT taken in consideration in this method:
        ///     - Auto fit font
        ///     - Rotation other than 90 degrees
        ///     - Staggraed labels
        ///     Wrapped text.
        /// </summary>
        /// <param name="chartArea"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static SizeF MeasureAllAxisLabels(Chart chartControl, Graphics graphics, ChartArea chartArea, Axis axis)
        {
            SizeF size = SizeF.Empty;

            SizeF chartSize = new SizeF((float)chartControl.Width.Value, (float)chartControl.Height.Value);

            // Iterate through all labels and get maximum size
            foreach (CustomLabel label in axis.CustomLabels)
            {
                SizeF lableSize = graphics.MeasureString(label.Text, axis.LabelStyle.Font, chartSize);
                size.Width = Math.Max(size.Width, lableSize.Width);
                size.Height = Math.Max(size.Height, lableSize.Height);
            }

            return size;
        }

        /// <summary>
        /// Checks if X and Y axes flipped
        /// </summary>
        /// <returns></returns>
        private static bool IsAxisFlipped(Chart chartControl)
        {
            // Checks series chart type to determine if axes are flipped.
            // TODO: Check which chart area and axes are used.
            foreach (Series series in chartControl.Series)
            {
                if (series.ChartType == SeriesChartType.Bar ||
                    series.ChartType == SeriesChartType.StackedBar ||
                    series.ChartType == SeriesChartType.StackedBar100 ||
                    series.ChartType == SeriesChartType.RangeBar)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Axes Methods

        #region Title Methods

        /// <summary>
        /// Sets maximum size available to the title and hides titles
        /// when chart becomes too small.
        /// </summary>
        /// <param name="hideThreshold">Size threshold when chart titles should be hidden.</param>
        /// <param name="maxAvailableSize">Maximum available space for the title in percent 
        /// of the overall chart size.</param>
        private static void TitleHideOrLimitSize(Chart chartControl, Graphics graphics, int hideThreshold, int maxAvailableSize)
        {
            List<Title> hiddenTitles = new List<Title>();
            Dictionary<Title, TitleInfo> titleStoredSettings = new Dictionary<Title, TitleInfo>();

            // Hide all legends when either chart Width or Height
            // is smaller than specified threshold.
            if (chartControl.Width.Value < hideThreshold || chartControl.Height.Value < hideThreshold)
            {
                foreach (Title title in chartControl.Titles)
                {
                    hiddenTitles.Add(title);
                    title.Visible = false;
                }
            }
            else
            {
                foreach (Title title in hiddenTitles)
                {
                    title.Visible = true;
                }
                hiddenTitles.Clear();

                // Reduce title font and then truncate text to fit
                foreach (Title title in chartControl.Titles)
                {
                    ReduceTitleSize(chartControl, graphics, title, maxAvailableSize, titleStoredSettings);
                }
            }
        }

        /// <summary>
        /// Method checks if chart title fits into the available space and 
        /// if not tries reducing font and then truncating text.
        /// </summary>
        /// <param name="title">Title to check.</param>
        /// <param name="maxAvailableSize">Maximum available size.</param>
        /// <param name="titleStoredSettings"></param>
        private static void ReduceTitleSize(Chart chartControl, Graphics graphics, Title title, int maxAvailableSize, Dictionary<Title, TitleInfo> titleStoredSettings)
        {
            // Restore original settings
            if (titleStoredSettings.ContainsKey(title))
            {
                title.Text = titleStoredSettings[title].Text;
                Font oldFont = title.Font;
                title.Font = new System.Drawing.Font(
                    title.Font.FontFamily,
                    titleStoredSettings[title].FontSize,
                    title.Font.Style);
                oldFont.Dispose();

                titleStoredSettings.Remove(title);
            }

            // Check if title fits
            bool fits = false;
            if (!IsTitleFits(chartControl, graphics, title, maxAvailableSize))
            {
                // Remeber original chart title settings
                titleStoredSettings.Add(title, new TitleInfo(title.Font.Size, title.Text));

                // Try reducing title font untill it fits
                while (title.Font.Size > MIN_TITLEFONT_SIZE)
                {
                    // Reduce fonts size
                    Font oldFont = title.Font;
                    title.Font = new System.Drawing.Font(
                        title.Font.FontFamily,
                        title.Font.Size - 1,
                        title.Font.Style);
                    oldFont.Dispose();

                    // Check fit
                    if (IsTitleFits(chartControl, graphics, title, maxAvailableSize))
                    {
                        fits = true;
                        break;
                    }
                }

                // If title does not fit after reducing the font size
                // try truncating title text
                bool doneFlag = false;
                while (!doneFlag && !fits)
                {
                    if (TruncateTitleText(chartControl, graphics, title))
                    {
                        // Check fit
                        if (IsTitleFits(chartControl, graphics, title, maxAvailableSize))
                        {
                            fits = true;
                            break;
                        }
                    }
                    else
                    {
                        doneFlag = true;
                    }
                }
            }
        }

        /// <summary>
        /// Truncates title text on the last space. Title must take less vertical
        /// space after truncation.
        /// </summary>
        /// <param name="title">Title to truncate.</param>
        /// <returns>True, if title was succesfully truncated.</returns>
        private static bool TruncateTitleText(Chart chartControl, Graphics graphics, Title title)
        {
            bool successFlag = false;
            bool doneFlag = false;
            string oldText = title.Text;

            // Measure title size

            SizeF chartSize = new SizeF((float)chartControl.Width.Value, (float)chartControl.Height.Value);
            SizeF titleSize = graphics.MeasureString(title.Text, title.Font, chartSize);

            while (!doneFlag)
            {
                // Truncate title text on the whitespace
                int spaceIndex = title.Text.LastIndexOf(' ');
                if (spaceIndex > 0)
                {
                    title.Text = title.Text.Substring(0, spaceIndex) + "...";
                }
                else
                {
                    doneFlag = true;
                    title.Text = oldText;
                    break;
                }

                // Check if title height was reduced
                SizeF currentTitleSize = graphics.MeasureString(title.Text, title.Font, chartSize);
                if (currentTitleSize.Height < titleSize.Height)
                {
                    doneFlag = true;
                    successFlag = true;
                    break;
                }
            }

            return successFlag;
        }

        /// <summary>
        /// Check if specified title fits.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="title">Title to check.</param>
        /// <param name="maxAvailableSize">Maximum available size.</param>
        /// <param name="chartControl"></param>
        /// <returns>True if title fits.</returns>
        private static bool IsTitleFits(Chart chartControl, Graphics graphics, Title title, int maxAvailableSize)
        {
            // Calculate title maximum size in pixels based on the docking side
            double maxPixelsize = chartControl.Height.Value / 100.0 * maxAvailableSize;
            if (title.Docking == Docking.Right || title.Docking == Docking.Left)
            {
                maxPixelsize = chartControl.Width.Value / 100.0 * maxAvailableSize;
            }

            // Measure title size

            SizeF chartSize = new SizeF((float)chartControl.Width.Value, (float)chartControl.Height.Value);
            SizeF titleSize = graphics.MeasureString(title.Text, title.Font, chartSize);

            if (title.Docking == Docking.Right || title.Docking == Docking.Left)
            {
                return titleSize.Width <= maxPixelsize;
            }
            return titleSize.Height <= maxPixelsize;
        }

        /// <summary>
        /// Creates graphics used to measure strings
        /// </summary>
        private static Graphics CreateGraphics()
        {
            // Create graphics neede
            return Graphics.FromImage(new Bitmap(100, 100));
        }

        /// <summary>
        /// Helper class which stores title text and font size.
        /// </summary>
        internal class TitleInfo
        {
            public TitleInfo(float fontSize, string text)
            {
                FontSize = fontSize;
                Text = text;
            }

            public float FontSize { set; get; }
            public string Text { set; get; }
        }

        #endregion Title Methods

        #region Legend Methods

        /// <summary>
        /// Re-docks chart legend to the right or bottom based on the 
        /// ratio of the chart width and height.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="ratio"></param>
        /// <param name="chartControl"></param>
        private static void LegendSetDocking(Chart chartControl, Graphics graphics, int ratio)
        {
            // Determine what will be the preferred docking for the legend
            Docking legendDocking = Docking.Right;
            if (chartControl.Width.Value < chartControl.Height.Value * ratio)
            {
                legendDocking = Docking.Bottom;
            }

            // Check if there is at least one legend already docked to the same side
            bool reDockLegend = true;
            foreach (Legend legend in chartControl.Legends)
            {
                if (legendDocking == Docking.Bottom &&
                    (legend.Docking == Docking.Bottom || legend.Docking == Docking.Top))
                {
                    // There is a legend which is docked to top/bottom side
                    reDockLegend = false;
                    break;
                }

                if (legendDocking == Docking.Right &&
                    (legend.Docking == Docking.Right || legend.Docking == Docking.Left))
                {
                    // There is a legend which is docked to right/left side
                    reDockLegend = false;
                    break;
                }
            }

            // Re-dock first legend only
            if (reDockLegend && chartControl.Legends.Count > 0)
            {
                chartControl.Legends[0].Docking = legendDocking;
            }
        }

        /// <summary>
        /// Sets maximum size available to the legend and hides legends
        /// when chart becomes too small.
        /// </summary>
        /// <param name="hideThreshold">Size threshold when chart legends should be hidden.</param>
        /// <param name="maxAvailableSize">Maximum available space for the legend in percent 
        /// of the overall chart size.</param>
        private static void LegendHideOrLimitSize(Chart chartControl, Graphics graphics, int hideThreshold, int maxAvailableSize)
        {
            // Set maximum available size of the legend in percent of the overall 
            // chart size.
            foreach (Legend legend in chartControl.Legends)
            {
                legend.MaximumAutoSize = maxAvailableSize;
            }
            
            List<Legend> hiddenLegends = new List<Legend>();

            // Hide all legends when either chart Width or Height
            // is smaller than specified threshold.
            if (chartControl.Width.Value < hideThreshold || chartControl.Height.Value < hideThreshold)
            {
                foreach (Legend legend in chartControl.Legends)
                {
                    hiddenLegends.Add(legend);
                    legend.Enabled = false;
                }
            }
            else
            {
                foreach (Legend legend in hiddenLegends)
                {
                    legend.Enabled = true;
                }
                hiddenLegends.Clear();
            }
        }

        #endregion Legend Methods

        #region Pie Chart Methods

        /// <summary>
        /// Restricts number of labels in the Pie or Doughnut chart types.
        /// </summary>
        /// <param name="maxLabelsCount">Maximum number of labels to show.</param>
        public static void LimitPieLabelsNumber(Chart chartControl, Graphics graphics, int maxLabelsCount)
        {
            foreach (Series series in chartControl.Series)
            {
                if (series.ChartType == SeriesChartType.Pie ||
                    series.ChartType == SeriesChartType.Doughnut)
                {
                    if (series.IsValueShownAsLabel)
                    {
                        // Reset any previously hidden labels.
                        foreach (DataPoint point in series.Points)
                        {
                            point.DeleteCustomProperty("IsValueShownAsLabel");
                        }

                        // Find top N data points
                        if (series.Points.Count > maxLabelsCount)
                        {
                            int index = 0;
                            IEnumerable<DataPoint> points = series.Points.OrderByDescending((item) => item.YValues[0]);
                            foreach (DataPoint dataPoint in points)
                            {
                                dataPoint.IsValueShownAsLabel = (index < maxLabelsCount) ? true : false;
                                ++index;
                            }
                        }
                    }
                }
            }
        }

        #endregion Pie Chart Methods
    }
}
