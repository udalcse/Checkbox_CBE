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
	/// Summary description for LineGraphRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:LineGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
	public class LineGraphRenderer : AnalysisItemRenderer 
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
			if(Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/text_header/visible")))
			{
				XmlDocument headerDoc = new XmlDocument();
				headerDoc.LoadXml(data.OuterXml);
				itemHeaderTransform.Transform(headerDoc, null, itemHeader, null);
				output.WriteLine(itemHeader.ToString());
			}
			XslTransform transform = new XslTransform();
			
			
			// Load the xslt to transform the form
			transform.Load(this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/SummaryGraphItemRenderTransform.xslt"));

			

			
			
			
			foreach(System.Xml.XmlNode node in data.SelectNodes("SourceItem"))
			{
				//Load the XML document
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(node.OuterXml);
				
				//Instantiate the chart control
				ChartControl chart = new ChartControl();
				
				ChartControl.PerformCleanUp();

				//Instantiate the line chart
				LineChart lineChart = new LineChart();

				string answerText;

				//Add the answers to the line chart
				foreach(System.Xml.XmlNode answerNode in doc.SelectNodes("SourceItem/Answer"))
				{
					if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
						answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
					else
						answerText = answerNode.SelectSingleNode("@answerText").InnerText;

					if(Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/show_percent")) == true)
					{
						answerText += " - " + answerNode.SelectSingleNode("@answerPercent").InnerText + "%";
					}

					lineChart.Data.Add(new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText)));
				}

				lineChart.Fill.Color = System.Drawing.Color.FromName(this.GetGraphOption("LineGraphOptions/color"));
				

				chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/legend/visible"));
				chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("LineGraphOptions/legend/width"));
					
				chart.Width = Convert.ToInt32(this.GetGraphOption("LineGraphOptions/chart_width"));
				chart.Height = Convert.ToInt32(this.GetGraphOption("LineGraphOptions/chart_height"));

				if(this.GetGraphOption("LineGraphOptions/background/color") != String.Empty)
					chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("LineGraphOptions/background/color"));
				
				if(Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/background/use_gradient")) == true)
				{
					chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("LineGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("LineGraphOptions/chart_height")));
					chart.Background.Type = WebChart.InteriorType.LinearGradient;
				}

				if(Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/title/visible")) == true)
				{
					//Set the chart title
					string chartTitle;

					if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && doc.SelectSingleNode("SourceItem/@itemAlias").InnerText != "")
						chartTitle = doc.SelectSingleNode("SourceItem/@itemAlias").InnerText;
					else
						chartTitle = doc.SelectSingleNode("SourceItem/@itemText").InnerText;


					if(Convert.ToBoolean(this.GetGraphOption("LineGraphOptions/title/show_response_count")) == true)
					{
                        chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + WebTextManager.GetText("/controlText/columnGraphRenderer/responese");
					}

					chart.ChartTitle.Text = chartTitle;

					chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("LineGraphOptions/title/color"));
				}
			
				//Add the line chart to the chart control
				chart.Charts.Add(lineChart);

				//Draw the chart
				chart.RedrawChart();

				//Output the chart
				chart.RenderControl(output);
//				output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\">");
	
				string spacer = this.GetGraphOption("LineGraphOptions/spacing");

				output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\">");


			}

		
		


			//string outputString = xslt.Transform(
			//xslt.Transform(data, args, output, resolver);

		}


	}
}
