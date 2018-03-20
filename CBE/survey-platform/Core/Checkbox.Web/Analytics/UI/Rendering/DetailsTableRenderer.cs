using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Xml;
using System.Xml.Xsl;

using Prezza.Framework.Logging;

namespace Checkbox.Web.Analytics.UI.Rendering
{
	/// <summary>
	/// Summary description for DetailsTableRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:DetailsTableRenderer runat=server></{0}:DetailsTableRenderer>")]
	public class DetailsTableRenderer : AnalysisItemRenderer 
	{


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			Logger.Write("Begin render DetailsTableRenderer", "Debug");
			DateTime begin = DateTime.Now;
			XmlNode data = this.mItemData;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(data.OuterXml);

			XslTransform xslt = new XslTransform();
			string xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/DetailsTableItemRenderTransform.xslt");

			Logger.Write("Loading DetailsTableRenderer xml", "Debug");
			xslt.Load(@xsltPath);
			Logger.Write("DetailsTableRenderer xml loaded", "Debug");
			
			XsltArgumentList args = new XsltArgumentList();
			XmlUrlResolver resolver = new XmlUrlResolver();


			xslt.Transform(doc, args, output, resolver);

			DateTime end = DateTime.Now;
			Logger.Write("End render DetailsTableRenderer, Elapsed: " + ((TimeSpan)end.Subtract(begin)).TotalMilliseconds, "Debug");

		}


	}
}
