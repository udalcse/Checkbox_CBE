using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Drawing;

using Checkbox.Management;
using WebChart;

namespace Checkbox.Web.Analytics.UI.Rendering
{
    /// <summary>
    /// Summary description for BarGraphRenderer.
    /// </summary>
    [DefaultProperty("Text"),
        ToolboxData("<{0}:BarGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
    public class BarGraphRenderer : AnalysisItemRenderer
    {
        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            XmlNode data = this.mItemData;

            XslTransform itemHeaderTransform = new XslTransform();
            itemHeaderTransform.Load(this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/ItemHeader.xslt"));
            StringWriter itemHeader = new StringWriter();

            if (Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/text_header/visible")))
            {
                XmlDocument headerDoc = new XmlDocument();
                headerDoc.LoadXml(data.OuterXml);
                itemHeaderTransform.Transform(headerDoc, null, itemHeader, null);
                output.WriteLine(itemHeader.ToString());
            }

            XslTransform transform = new XslTransform();

            // Load the xslt to transform the form
            transform.Load(this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/SummaryGraphItemRenderTransform.xslt"));

            foreach (System.Xml.XmlNode node in data.SelectNodes("SourceItem"))
            {
                int answerCount = 0;

                //Load the XML document
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(node.OuterXml);

                //Instantiate the chart control
                ChartControl chart = new ChartControl();

                ChartControl.PerformCleanUp();

                //Instantiate the column chart
                ColumnChart columnChart = new ColumnChart();

                string answerText;

                //Add the answers to the column chart
                foreach (System.Xml.XmlNode answerNode in doc.SelectNodes("SourceItem/Answer"))
                {
                    if (doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
                        answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
                    else
                        answerText = answerNode.SelectSingleNode("@answerText").InnerText;

                    if (Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/show_percent")) == true)
                    {
                        int precision = Convert.ToInt32(this.GetGraphOption("precision"));
                        string percent = Convert.ToString(Math.Round(Convert.ToDecimal(answerNode.SelectSingleNode("@answerPercent").InnerText), precision));
                        answerText += " - " + percent + "%";

                    }

                    ChartPoint cp = new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText));

                    columnChart.Data.Add(cp);

                    answerCount++;
                }



                //Set the chart properties
                chart.HasChartLegend = false;
                //chart.Width = Unit.Pixel(240);
                //chart.Height = Unit.Pixel(answerCount * 80);

                //WebChart.ChartInterior interior = new ChartInterior();

                //interior.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;


                chart.RenderHorizontally = true;



                columnChart.Fill.Type = WebChart.InteriorType.LinearGradient;
                columnChart.Fill.Angle = 90;

                columnChart.Fill.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("BarGraphOptions/foreColor"));
                columnChart.Fill.Color = System.Drawing.Color.FromName(this.GetGraphOption(" BarGraphOptions/color"));



                columnChart.Fill.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
                columnChart.Fill.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_width")));
                columnChart.MaxColumnWidth = Convert.ToInt32(this.GetGraphOption("BarGraphOptions/max_column_width")); ;

                chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/legend/visible"));
                chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("BarGraphOptions/legend/width"));
                chart.Legend.Position = WebChart.LegendPosition.Bottom;


                chart.Width = Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_width"));
                chart.Height = Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_height"));

                if (this.GetGraphOption("BarGraphOptions/background/color") != String.Empty)
                    chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("BarGraphOptions/background/color"));

                if (Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/background/use_gradient")) == true)
                {
                    chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("BarGraphOptions/chart_height")));
                    chart.Background.Type = WebChart.InteriorType.LinearGradient;
                }

                if (Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/title/visible")) == true)
                {
                    //Set the chart title
                    string chartTitle;

                    if (doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && doc.SelectSingleNode("SourceItem/@itemAlias").InnerText != "")
                        chartTitle = doc.SelectSingleNode("SourceItem/@itemAlias").InnerText;
                    else
                        chartTitle = doc.SelectSingleNode("SourceItem/@itemText").InnerText;


                    if (Convert.ToBoolean(this.GetGraphOption("BarGraphOptions/title/show_response_count")) == true)
                    {
                        chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + WebTextManager.GetText("/controlText/barGraphRenderer/responses");
                    }


                    chart.ChartTitle.Text = chartTitle;

                    chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("BarGraphOptions/title/color"));
                }

                chart.ChartPadding = 30;

                //Add the column chart to the chart control
                chart.Charts.Add(columnChart);

                //Draw the chart
                chart.RedrawChart();

                //Output the chart
                chart.RenderControl(output);

                //				output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\"></img>");

                string spacer = this.GetGraphOption("LineGraphOptions/spacing");

                output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\">");
            }
        }
    }
}
