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
	/// Summary description for SummaryGraphRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SummaryGraphRenderer runat=server></{0}:SummaryTableRenderer>")]
	public class SummaryGraphRenderer : AnalysisItemRenderer 
	{


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			XmlNode data = this.mItemData;

			//XslTransform xslt = new XslTransform();
			//string xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/SummaryGraphItemRenderTransform.xslt");
			//xslt.Load(@xsltPath);
			
			//XsltArgumentList args = new XsltArgumentList();
			//XmlUrlResolver resolver = new XmlUrlResolver();

	
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

				string movie = "../Reports/Graphs/COLUMN.swf";

				string flash = "<OBJECT " + 
					"classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" " +
					"codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=5,0,0,0\" " +
					"WIDTH=\"565\" HEIGHT=\"500\" id=\"FCPie\" ALIGN=\"\"> " +
					"<PARAM NAME=movie VALUE=\"" + movie + "?dataXML=" + result + "\"> " +
					"<PARAM NAME=quality VALUE=high> <PARAM NAME=bgcolor VALUE=#FFFFFF>" +
					"<EMBED src=\"" + movie + ".?dataXml=" + result + "\"" + " quality=high bgcolor=#FFFFFF " +
					"WIDTH=\"565\" HEIGHT=\"500\" NAME=\"FCPie\" ALIGN=\"\" " +
					"TYPE=\"application/x-shockwave-flash\" " + 
					"PLUGINSPAGE=\"http://www.macromedia.com/go/getflashplayer\"></EMBED> " +
					"</OBJECT><BR> ";

				output.WriteLine(flash);

	

				//output.Write(flash);
				
				//XslTransform xslt = new XslTransform();
				//string xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/OtherTableItemRenderTransform.xslt");
				//xslt.Load(@xsltPath);
			
				//XsltArgumentList args = new XsltArgumentList();
				//XmlUrlResolver resolver = new XmlUrlResolver();
				//output.WriteLine(xslt.Transform(doc, args));
			}


			//string outputString = xslt.Transform(
			//xslt.Transform(data, args, output, resolver);

		}


	}
}
