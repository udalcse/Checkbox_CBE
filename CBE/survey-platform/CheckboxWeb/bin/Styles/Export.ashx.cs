using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

using Checkbox.Styles;
using Checkbox.Forms.Items.UI;

using Ionic.Zip;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Export : Checkbox.Web.Page.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ids = Request.QueryString["id"];

			if (ids == null)
				return;

			string[] idArr = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			if (idArr.Length > 1)
			{
				ZipFile archive = new ZipFile("styles.zip");

				if (Request.QueryString["type"] == "form")
				{
					foreach (string id in idArr)
					{
						StyleTemplate template = StyleTemplateManager.GetStyleTemplate(Int32.Parse(id));
						XmlDocument xmlDoc = template.ToXml();
                        MemoryStream xmlStream = new MemoryStream();
                        xmlDoc.Save(xmlStream);

                        xmlStream.Flush(); 
                        xmlStream.Position = 0;

                        archive.AddEntry(template.Name + ".xml", xmlStream);

					}
				}
				else if (Request.QueryString["type"] == "chart")
				{
					//foreach (string id in idArr)
					//{
					//    ChartStyle style = ChartStyleManager.GetChartStyle(Int32.Parse(id));

					//    archive.AddFileFromString(style.Name + ".xml", string.Empty, doc.OuterXml);
					//}
				}

				SetupResponse("CheckboxStyleTemplates.zip");

				archive.Save(Response.OutputStream);

			}

			if (idArr.Length == 1)
			{
				var template2 = StyleTemplateManager.GetStyleTemplate(Int32.Parse(idArr[0]));
				var xmlDoc2 = template2.ToXml();

				SetupResponse(string.Format("{0}.xml", template2.Name));

				xmlDoc2.Save(Response.OutputStream);
			}

            Response.Flush();
            Response.End();
        }

		private void SetupResponse(string attach)
		{
			Response.Expires = -1;
			Response.BufferOutput = true;
			Response.Clear();
			Response.ClearHeaders();
			Response.AddHeader("Content-Disposition", "attachment;filename=" + attach);
			Response.ContentType = "application/octet-stream";
		}
    }
}
