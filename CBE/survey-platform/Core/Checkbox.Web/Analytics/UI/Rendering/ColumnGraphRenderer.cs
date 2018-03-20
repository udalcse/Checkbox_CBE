using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Drawing;
using System.IO;

using Checkbox.Management;
using WebChart;

namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Summary description for ColumnGraphRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ColumnGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
	public class ColumnGraphRenderer : AnalysisItemRenderer 
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
			
			if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/text_header/visible")))
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
				int answerCount = 0;
				
				//Load the XML document
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(node.OuterXml);
				

				if(doc.SelectNodes("SourceItem//Set").Count > 0)
				{
					foreach(System.Xml.XmlNode categoryNode in doc.SelectNodes("SourceItem//Category"))
					{
						
						foreach(System.Xml.XmlNode setAnswerNode in categoryNode.SelectNodes("SetAnswers"))
						{

							//Instantiate chart control
							ChartControl chart = new ChartControl();
				
							ChartControl.PerformCleanUp();
							
							if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/title/visible")) == true)
							{
								//Set chart title
								string chartTitle;
								string itemText;
								string categoryText;
								string setText;

								if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && doc.SelectSingleNode("SourceItem/@itemAlias").InnerText != "")
									itemText = doc.SelectSingleNode("SourceItem/@itemAlias").InnerText;
								else
									itemText = doc.SelectSingleNode("SourceItem/@itemText").InnerText;

								if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && setAnswerNode.Attributes.GetNamedItem("SetAlias").InnerText != "")
									setText = setAnswerNode.Attributes.GetNamedItem("SetAlias").InnerText;
								else
									setText = setAnswerNode.Attributes.GetNamedItem("SetName").InnerText;

								if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && categoryNode.Attributes.GetNamedItem("CategoryAlias").InnerText != "")
									categoryText = categoryNode.Attributes.GetNamedItem("CategoryAlias").InnerText;
								else
									categoryText = categoryNode.Attributes.GetNamedItem("CategoryName").InnerText;


								chartTitle = itemText + "-" + categoryText + "-" + setText;

								//if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/title/show_response_count")) == true)
								//{
								//	chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + XmlResourceManager.SelectSingleResource("strings", "labels", "responses").InnerText;
								//}

								chart.ChartTitle.Text = chartTitle;

								chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/title/color"));
			
							}

							
							//Instantiate column chart
							ColumnChart columnChart = new ColumnChart();
	
							//Set the chart properties
			
			
							columnChart.Fill.Type = WebChart.InteriorType.LinearGradient;
							columnChart.Fill.Angle = 90;
							columnChart.Fill.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
							columnChart.Fill.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/foreColor"));
							columnChart.Fill.Color = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/color"));
							columnChart.Fill.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")));
							columnChart.MaxColumnWidth = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/max_column_width"));

							chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/legend/visible"));
							chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/legend/width"));
					
							chart.Width = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width"));
							chart.Height = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_height"));

							if(this.GetGraphOption("ColumnGraphOptions/background/color") != String.Empty)
								chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/background/color"));
			
							if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/background/use_gradient")) == true)
							{
								chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_height")));
								chart.Background.Type = WebChart.InteriorType.LinearGradient;
							}


							string answerText;

							//Add answers to column chart
							foreach(System.Xml.XmlNode answerNode in setAnswerNode.SelectNodes("Answer"))
							{
								if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
									answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
								else
									answerText = answerNode.SelectSingleNode("@answerText").InnerText;

								if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/show_percent")) == true)
								{
									int precision = Convert.ToInt32(this.GetGraphOption("precision"));
									string percent = Convert.ToString(Math.Round(Convert.ToDecimal(answerNode.SelectSingleNode("@answerPercent").InnerText), precision));
									answerText += " - " + percent  + "%";

								}

								columnChart.Data.Add(new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText)));
							}


							chart.Legend.Position = WebChart.LegendPosition.Bottom;
							chart.ChartPadding = 30;

			
							//Add the column chart to the chart control
							chart.Charts.Add(columnChart);

							//Draw the chart
							chart.RedrawChart();

							//Output the chart
							chart.RenderControl(output);
//							output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\"></img>");

							string spacer = this.GetGraphOption("LineGraphOptions/spacing");

							output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\"><br>");

						}
					}
					
					return;
				}
				else
				{

					//Instantiate the chart control
					ChartControl chart = new ChartControl();
				
				
					ChartControl.PerformCleanUp();

					//Instantiate the column chart
					ColumnChart columnChart = new ColumnChart();

					string answerText;

					//Add the answers to the column chart
					foreach(System.Xml.XmlNode answerNode in doc.SelectNodes("SourceItem/Answer"))
					{
						if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
							answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
						else
							answerText = answerNode.SelectSingleNode("@answerText").InnerText;

						if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/show_percent")) == true)
						{
							int precision = Convert.ToInt32(this.GetGraphOption("precision"));
							string percent = Convert.ToString(Math.Round(Convert.ToDecimal(answerNode.SelectSingleNode("@answerPercent").InnerText), precision));
							answerText += " - " + percent  + "%";
						}

						columnChart.Data.Add(new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText)));
				
						answerCount ++;
					}

	
					//Set the chart properties
			
			
					columnChart.Fill.Type = WebChart.InteriorType.LinearGradient;
					columnChart.Fill.Angle = 90;
					columnChart.Fill.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
					columnChart.Fill.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/foreColor"));
					columnChart.Fill.Color = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/color"));
					columnChart.Fill.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")));
					columnChart.MaxColumnWidth = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/max_column_width"));

					chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/legend/visible"));
					chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/legend/width"));
					
					chart.Width = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width"));
					chart.Height = Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_height"));

					if(this.GetGraphOption("ColumnGraphOptions/background/color") != String.Empty)
						chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/background/color"));
			
					if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/background/use_gradient")) == true)
					{
						chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("ColumnGraphOptions/chart_height")));
						chart.Background.Type = WebChart.InteriorType.LinearGradient;
					}

					if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/title/visible")) == true)
					{
						//Set the chart title
						string chartTitle;

						if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && doc.SelectSingleNode("SourceItem/@itemAlias").InnerText != "")
							chartTitle = doc.SelectSingleNode("SourceItem/@itemAlias").InnerText;
						else
							chartTitle = doc.SelectSingleNode("SourceItem/@itemText").InnerText;

						if(Convert.ToBoolean(this.GetGraphOption("ColumnGraphOptions/title/show_response_count")) == true)
						{
                            chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + WebTextManager.GetText("/controlText/columnGraphRenderer/responese");
						}

						chart.ChartTitle.Text = chartTitle;

						chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("ColumnGraphOptions/title/color"));
					}

					chart.Legend.Position = WebChart.LegendPosition.Bottom;
					chart.ChartPadding = 30;
				
			
					//Add the column chart to the chart control
					chart.Charts.Add(columnChart);

				

					//Draw the chart
					chart.RedrawChart();

					//Output the chart
					chart.RenderControl(output);
//					output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\"></img>");

					string spacer = this.GetGraphOption("LineGraphOptions/spacing");

					output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\">");
				}
			}

		}

		


		}


	}
