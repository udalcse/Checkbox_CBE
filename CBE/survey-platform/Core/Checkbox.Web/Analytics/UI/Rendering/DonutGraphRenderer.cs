using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using System.IO;


namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Summary description for DonutGraphRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:DonutGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
	public class DonutGraphRenderer : AnalysisItemRenderer 
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
			XmlDocument headerDoc = new XmlDocument();
			headerDoc.LoadXml(data.OuterXml);
			itemHeaderTransform.Transform(headerDoc, null, itemHeader, null);
			output.WriteLine(itemHeader.ToString());

			XslTransform transform = new XslTransform();
			
			
			// Load the xslt to transform the form
			transform.Load(this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/SummaryGraphItemRenderTransform.xslt"));

			

			XPathDocument colorDoc = new XPathDocument(this.Context.Server.MapPath(Configuration.ApplicationRoot + "/Analytics/GraphOptions.xml"));
			XPathNodeIterator itr = colorDoc.CreateNavigator().Select("//GraphOptions/color");

			
			int numberColors = itr.Count;		
			string[] colors = new string[numberColors];

			while(itr.MoveNext())
			{
				colors[(itr.CurrentPosition - 1)] = itr.Current.Value;
			}
			
			
			
			foreach(System.Xml.XmlNode node in data.SelectNodes("SourceItem"))
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(node.OuterXml);
				
				int nodeCount = 0;

				foreach(System.Xml.XmlNode answerNode in doc.SelectNodes("SourceItem/Answer"))	
				{
					nodeCount = nodeCount + 1;
					XmlNode colorAtt = doc.CreateNode(XmlNodeType.Attribute, "Color", null);
					//System.Xml.XmlAttribute colorAtt = answerNode.Attributes.Append(;
					int optionPosition = (nodeCount - 1);
					if ((optionPosition) > (numberColors - 1))
					{
						optionPosition = (optionPosition % numberColors);
					}
					string colorValue = (string)colors[optionPosition];
					colorAtt.Value = colorValue;
					answerNode.Attributes.Append((XmlAttribute)colorAtt);
					//answerNode.AppendChild(colorAtt);

					
				}

				System.Xml.XmlNode numberResponsesNode = doc.SelectSingleNode("SourceItem");
				int yAxisMax = 0;
				if(numberResponsesNode.Attributes["totalAnswers"].Value == "0")
				{
					yAxisMax = 1;
				}
				else
				{
					yAxisMax = Convert.ToInt32(numberResponsesNode.Attributes["totalAnswers"].Value == "0");
				}
				XmlNode axisMax = doc.CreateNode(XmlNodeType.Attribute, "yAxisMax", null);
				axisMax.Value = yAxisMax.ToString();
				numberResponsesNode.Attributes.Append((XmlAttribute)axisMax);
	
				if(nodeCount == 0)
				{
					return;


				}

				StringWriter sw = new StringWriter();

				transform.Transform(doc, null, sw, null);
				
				//transform.Transform(data, null, sw);
				string result = sw.ToString();

				result = result.Replace("&","%26");
				result = result.Replace("?","%63");
				result = result.Replace("\"", "&quot;");

				result = result.Replace("\r\n", String.Empty);

				string movie = "../Reports/Graphs/DONUT.swf";

				string flash = "<object classid=\"clsid:d27cdb6e-ae6d-11cf-96b8-444553540000\" " + 
					" codebase=\"http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,0,0\" WIDTH=\"565\" HEIGHT=\"500\" id=\"dominionA\" " +
					" align=\"middle\"> " +
					" <param name=\"allowScriptAccess\" value=\"sameDomain\" /> " + 
					" <param name=\"movie\" value=\"" + movie + "?dataXML=" + result + "\" /> " + 
					" <param name=\"quality\" value=\"high\" /> " +
					" <param name=\"bgcolor\" value=\"#ffffff\" /> " + 
					" <embed src=\"" + movie + "?dataXML=" + result +  "\" quality=\"high\" bgcolor=\"#cccccc\" " +
					" WIDTH=\"565\" HEIGHT=\"500\"  name=\"dominionA\" align=\"middle\" " +
					" allowScriptAccess=\"sameDomain\" type=\"application/x-shockwave-flash\" " +
					" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" /> " +
					" </object>"; 

				output.WriteLine(flash);

				XslTransform otherTransform = new XslTransform();
				otherTransform.Load(this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/OtherTableItemRenderTransform.xslt"));
				StringWriter otherWriter = new StringWriter();
				headerDoc.LoadXml(data.OuterXml);
				otherTransform.Transform(doc, null, otherWriter, null);
				output.WriteLine(otherWriter.ToString());
			}



		}


	}
}
