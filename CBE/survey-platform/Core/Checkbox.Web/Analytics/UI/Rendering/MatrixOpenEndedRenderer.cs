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
	/// Summary description for MatrixOpenEndedRenderer.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MatrixOpenEndedRenderer runat=server></{0}:MatrixOpenEndedRenderer>")]
	public class MatrixOpenEndedRenderer : AnalysisItemRenderer 
	{


		/// <summary> 
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			Logger.Write("Begin render MatrixOpenEndedRenderer", "Debug");
			DateTime begin = DateTime.Now;

			XmlNode data = this.mItemData;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(data.OuterXml);
			
			XslTransform xslt = new XslTransform();
			string xsltPath = this.Context.Server.MapPath(Checkbox.Web.Configuration.ApplicationRoot + "/Analytics/MatrixOpenEndedItemRenderTransform.xslt");
			xslt.Load(@xsltPath);
			
			XsltArgumentList args = new XsltArgumentList();
			XmlUrlResolver resolver = new XmlUrlResolver();
			xslt.Transform(doc, args, output, resolver);

			DateTime end = DateTime.Now;
			Logger.Write("End render MatrixOpenEndedRenderer, Elapsed: " + ((TimeSpan)end.Subtract(begin)).TotalMilliseconds, "Debug");


		}


	}
}
