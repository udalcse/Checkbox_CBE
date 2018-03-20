using System.IO;
using System.Xml;
using Checkbox.Forms.Items.UI;
using Prezza.Framework.Common;

namespace Checkbox.Analytics.Items.UI
{
    /// <summary>
    /// Load an analyis item appearance with graph options
    /// </summary>
    public static class GraphOptionsLoader
    {
        /// <summary>
        /// Populate an appearance data with default options from the specified file path
        /// </summary>
        /// <param name="appearance"></param>
        /// <param name="graphOptionsPath"></param>
        public static void PopulateAppearanceData(AnalysisItemAppearanceData appearance, string graphOptionsPath)
        {
            try
            {
                if (File.Exists(graphOptionsPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(graphOptionsPath);

                    //TODO: Is this necessary any more?
//                    string graphType = GetGraphTypeString(appearance);

                   
                    //appearance.BackgroundColor = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/background/color"));
                    //appearance.BackgroundGradient = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/background/use_gradient"));
                    //appearance.Color = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/color"));
                    //appearance.Explosion = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/explosion"));
                    //appearance.ForegroundColor = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/foreColor"));
                    //appearance.GraphHeight = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/chart_height"));
                    //appearance.GraphSpacing = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/spacing"));
                    //appearance.GraphWidth = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/chart_width"));
                    //appearance.LegendWidth = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/legend/width"));
                    //appearance.MaxColumnWidth = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/" + graphType + "/max_column_width"));
                    //appearance.PieGraphColors = XmlUtility.GetNodeTextAsCSV(doc.SelectNodes("/GraphOptions/" + graphType + "/colors/color"));
                    //appearance.Precision = XmlUtility.GetNodeInt(doc.SelectSingleNode("/GraphOptions/precision"));
                    //appearance.Separator = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/data_labels/separator"));
                    //appearance.ShowDataLabels = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/data_labels/visible"));
                    //appearance.ShowDataLabelsXAxisTitle = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/data_labels/showXTitle"));
                    //appearance.ShowDataLabelValues = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/data_labels/show_value"));
                    //appearance.ShowDataLabelZeroValues = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/data_labels/show_zero_values"));
                    //appearance.ShowHeader = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/text_header/visible"));
                    //appearance.ShowLegend = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/legend/visible"));
                    //appearance.ShowPercent = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/show_percent"));
                    //appearance.ShowResponseCountInTitle = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/title/show_response_count"));
                    //appearance.ShowTitle = XmlUtility.GetNodeBool(doc.SelectSingleNode("/GraphOptions/" + graphType + "/title/visible"));
                    //appearance.TitleColor = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/title/color"));
                    //appearance.TitleFont = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/title/font"));
                    //appearance.LegendFont = XmlUtility.GetNodeText(doc.SelectSingleNode("/GraphOptions/" + graphType + "/legend/font"));
                }
            }
            catch
            {
            }
        }

        ///// <summary>
        ///// Get the graph type based on the string
        ///// </summary>
        ///// <param name="appearance"></param>
        ///// <returns></returns>
        //private static string GetGraphTypeString(AppearanceData appearance)
        //{
        //    switch (appearance.GraphType)
        //    {
        //        case GraphType.BarGraph:
        //            return "BarGraphOptions";
        //        case GraphType.ColumnGraph:
        //            return "ColumnGraphOptions";
        //        case GraphType.LineGraph:
        //            return "LineGraphOptions";
        //        case GraphType.PieGraph:
        //            return "PieGraphOptions";
        //        case GraphType.CrossTab:
        //            return "CrossTabOptions";
        //        case GraphType.SummaryTable:
        //            return "SummaryTableOptions";
        //        case GraphType.Doughnut:
        //            return "PieGraphOptions";
        //        default:
        //            return "OtherGraphTypeOptions";
        //    }
        //}
    }
}
