using System;
using System.Text;
using System.Xml;
using Checkbox.Management;
using Checkbox.Forms;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Libraries
{
    /// <summary>
    /// 
    /// </summary>
	public partial class DownloadXml : Checkbox.Web.Page.BasePage
	{
		protected override void  OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);

			string idTxt = Request.QueryString["id"];

			if (string.IsNullOrEmpty(idTxt))
				return;

			int id = -1;

			if (!int.TryParse(idTxt, out id))
				return;

			LibraryTemplate lib = LibraryTemplateManager.GetLibraryTemplate(id);

			if (lib == null)
				return;

			string libName = TextManager.GetText(lib.NameTextID, TextManager.DefaultLanguage);

			libName = libName.Replace(' ', '_');

			GetDownloadResponse(libName + "_lib_export.xml");

		    var xmlWriter = new XmlTextWriter(Response.OutputStream, Encoding.UTF8) {Formatting = Formatting.Indented};
            lib.Export(xmlWriter);

			Response.Flush();
			Response.End();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		protected void GetDownloadResponse(string fileName)
		{
			Response.Expires = -1;
			Response.BufferOutput = ApplicationManager.AppSettings.BufferResponseExport;
			Response.Clear();
			Response.ClearHeaders();
			Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
			Response.ContentType = "application/octet-stream";

		}
	}
}