using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Drawing;

using WebChart;
using Checkbox.Management;

namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Summary description for PieGraphRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:PieGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
	public class PieGraphRenderer : AnalysisItemRenderer 
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
		
			if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/text_header/visible")))
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
				//Get XML document 
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
							
							if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/title/visible")) == true)
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

								//if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/title/show_response_count")) == true)
								//{
								//	chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + XmlResourceManager.SelectSingleResource("strings", "labels", "responses").InnerText;
								//}

								chart.ChartTitle.Text = chartTitle;

								chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("PieGraphOptions/title/color"));
			
							}

							
							//Instantiate pie chart
							PieChart pieChart = new PieChart();
	
							//Set pie chart colors
							XPathDocument colorDoc = new XPathDocument(this.Context.Server.MapPath(Configuration.ApplicationRoot + "/Analytics/GraphOptions.xml"));
							XPathNodeIterator itr = colorDoc.CreateNavigator().Select("//GraphOptions/PieGraphOptions/colors/color");

							int numberColors = itr.Count;		
							Color[] colors = new Color[numberColors];
				
							while(itr.MoveNext())
							{
								colors[(itr.CurrentPosition - 1)] = System.Drawing.Color.FromName(itr.Current.Value);
							}

							pieChart.Colors = colors;
				
				

							//Set pie chart properties
							pieChart.Explosion = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/explosion"));
				
							pieChart.DataLabels.Visible = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/visible"));
							pieChart.DataLabels.ShowXTitle = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/showXTitle"));
							pieChart.DataLabels.ShowZeroValues = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/show_zero_values"));
							pieChart.DataLabels.ShowValue = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/show_value"));
							pieChart.DataLabels.Separator = this.GetGraphOption("PieGraphOptions/data_labels/separator");
					
							chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/legend/visible"));
							chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("PieGraphOptions/legend/width"));
							chart.Width = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_width"));
							chart.Height = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_height"));

							string answerText;

							//Add answers to pie chart
							foreach(System.Xml.XmlNode answerNode in setAnswerNode.SelectNodes("Answer"))
							{
								if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
									answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
								else
									answerText = answerNode.SelectSingleNode("@answerText").InnerText;

								if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/show_percent")) == true)
								{
									int precision = Convert.ToInt32(this.GetGraphOption("precision"));
									string percent = Convert.ToString(Math.Round(Convert.ToDecimal(answerNode.SelectSingleNode("@answerPercent").InnerText), precision));
									answerText += " - " + percent  + "%";

								}

								pieChart.Data.Add(new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText)));
							}
				

							
							//Set chart object properties
							chart.Legend.Position = WebChart.LegendPosition.Right;
							chart.Background.Type = WebChart.InteriorType.Solid;
							chart.GridLines = WebChart.GridLines.None;


								
							if(this.GetGraphOption("PieGraphOptions/background/color") != String.Empty)
								chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("PieGraphOptions/background/color"));
				
				
							if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/background/use_gradient")) == true)
							{
								chart.Background.Type = WebChart.InteriorType.LinearGradient;
								chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_height")));
				
							}
			
							//Add the pie chart to the chart control
							chart.Charts.Add(pieChart);

							//Draw the chart
							chart.RedrawChart();

							//Output the chart
							chart.RenderControl(output);
							//output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\"></img>");
//							output.Write("img src=\"" + Checkbox.Web.Configuration.ApplicationRoot + "ViewImage.aspx?ImageID=" + chart.ImageDBID +"\"></img>");

							string spacer = this.GetGraphOption("LineGraphOptions/spacing");

							output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\"><br>");

						}
					}
					
					return;
				}
				else
				{

					//Instantiate chart control
					ChartControl chart = new ChartControl();
				
					ChartControl.PerformCleanUp();

					//Instantiate pie chart
					PieChart pieChart = new PieChart();
	
					//Set pie chart colors
					XPathDocument colorDoc = new XPathDocument(this.Context.Server.MapPath(Configuration.ApplicationRoot + "/Analytics/GraphOptions.xml"));
					XPathNodeIterator itr = colorDoc.CreateNavigator().Select("//GraphOptions/PieGraphOptions/colors/color");

					int numberColors = itr.Count;		
					Color[] colors = new Color[numberColors];
				
					while(itr.MoveNext())
					{
						colors[(itr.CurrentPosition - 1)] = System.Drawing.Color.FromName(itr.Current.Value);
					}

					pieChart.Colors = colors;
				
				

					//Set pie chart properties
					pieChart.Explosion = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/explosion"));
				
					pieChart.DataLabels.Visible = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/visible"));
					pieChart.DataLabels.ShowXTitle = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/showXTitle"));
					pieChart.DataLabels.ShowZeroValues = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/show_zero_values"));
					pieChart.DataLabels.ShowValue = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/data_labels/show_value"));
					pieChart.DataLabels.Separator = this.GetGraphOption("PieGraphOptions/data_labels/separator");
					
					chart.HasChartLegend = Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/legend/visible"));
					chart.Legend.Width = (short)Convert.ToInt32(this.GetGraphOption("PieGraphOptions/legend/width"));
					chart.Width = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_width"));
					chart.Height = Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_height"));

					string answerText;

					//Add answers to pie chart
					foreach(System.Xml.XmlNode answerNode in doc.SelectNodes("SourceItem/Answer"))
					{
						if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && answerNode.SelectSingleNode("@answerAlias").InnerText != "")
							answerText = answerNode.SelectSingleNode("@answerAlias").InnerText;
						else
							answerText = answerNode.SelectSingleNode("@answerText").InnerText;

						if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/show_percent")) == true)
						{
							int precision = Convert.ToInt32(this.GetGraphOption("precision"));
							string percent = Convert.ToString(Math.Round(Convert.ToDecimal(answerNode.SelectSingleNode("@answerPercent").InnerText), precision));
							answerText += " - " + percent  + "%";

						}

						pieChart.Data.Add(new ChartPoint(answerText, Convert.ToUInt32(answerNode.SelectSingleNode("@answerCount").InnerText)));
					}
				

							
					//Set chart object properties
					chart.Legend.Position = WebChart.LegendPosition.Right;
					chart.Background.Type = WebChart.InteriorType.Solid;
					chart.GridLines = WebChart.GridLines.None;


					if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/title/visible")) == true)
					{
						//Set chart title
						string chartTitle;

						if(doc.SelectSingleNode("SourceItem/@UseAlias").InnerText == "Yes" && doc.SelectSingleNode("SourceItem/@itemAlias").InnerText != "")
							chartTitle = doc.SelectSingleNode("SourceItem/@itemAlias").InnerText;
						else
							chartTitle = doc.SelectSingleNode("SourceItem/@itemText").InnerText;

						if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/title/show_response_count")) == true)
						{
                            chartTitle += " - " + doc.SelectSingleNode("SourceItem/@totalAnswers").InnerText + " " + WebTextManager.GetText("/controlText/columnGraphRenderer/responese");
						}

						chart.ChartTitle.Text = chartTitle;

						chart.ChartTitle.ForeColor = System.Drawing.Color.FromName(this.GetGraphOption("PieGraphOptions/title/color"));
			
					}
								
					if(this.GetGraphOption("PieGraphOptions/background/color") != String.Empty)
						chart.Background.Color = System.Drawing.Color.FromName(this.GetGraphOption("PieGraphOptions/background/color"));
				
				
					if(Convert.ToBoolean(this.GetGraphOption("PieGraphOptions/background/use_gradient")) == true)
					{
						chart.Background.Type = WebChart.InteriorType.LinearGradient;
						chart.Background.EndPoint = new Point(Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_width")), Convert.ToInt32(this.GetGraphOption("PieGraphOptions/chart_height")));
				
					}
			
					//Add the pie chart to the chart control
					chart.Charts.Add(pieChart);

					//Draw the chart
					chart.RedrawChart();

					//Output the chart
					chart.RenderControl(output);
					//output.Write("<img src=\"" + Checkbox.Web.Configuration.ApplicationRoot + "/ViewImage.aspx?ImageID=" + chart.ImageDBID +"\"></img>");
//					output.Write("<img src=\"../WebCharts/" + chart.ImageID + ".Png\"></img>");
//
					string spacer = this.GetGraphOption("LineGraphOptions/spacing");

					output.Write("<img src=\"../images/spacer.gif\" width=\"" + spacer + "\">");
				}
			}

		}
	}
}
