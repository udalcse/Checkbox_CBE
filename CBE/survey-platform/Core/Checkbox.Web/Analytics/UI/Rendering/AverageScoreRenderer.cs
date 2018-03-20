using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;


namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Summary description for AverageScoreRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AverageScoreRenderer runat=server></{0}:AverageScoreRenderer>")]
	public class AverageScoreRenderer : AnalysisItemRenderer 
	{


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			XmlNode data = this.mItemData;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(data.OuterXml);

			XslTransform xslt = new XslTransform();
			string xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/AverageScoreItemRenderTransform.xslt");
			xslt.Load(@xsltPath);
			
			XsltArgumentList args = new XsltArgumentList();
			XmlUrlResolver resolver = new XmlUrlResolver();
			xslt.Transform(doc, args, output, resolver);

			//xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/OtherTableItemRenderTransform.xslt");
			//xslt.Load(@xsltPath);
			//xslt.Transform(doc, args, output, resolver);
		}


	}
}
