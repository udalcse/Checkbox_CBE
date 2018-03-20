using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Management;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Users;
using Checkbox.Web;
using Prezza.Framework.Security;
using Checkbox.Web.Page;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Content;
using Checkbox.LicenseLibrary;

namespace CheckboxWeb.Libraries.Controls
{
    public partial class LibraryPreview : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var surveyLanguages = WebTextManager.GetSurveyLanguagesDictionary();

            foreach (string languageCode in surveyLanguages.Keys)
            {
                var localizedLanguageName = surveyLanguages.ContainsKey(languageCode)
                    ? surveyLanguages[languageCode]
                    : languageCode;

                _languageList.Items.Add(new ListItem(
                    localizedLanguageName,
                    languageCode));
            }

            if (_languageList.Items.FindByValue(TextManager.DefaultLanguage) != null)
            {
                _languageList.SelectedValue = TextManager.DefaultLanguage;
            }

            String errorMsg;

            //Check for multiLanguage support
            if ((Page is LicenseProtectedPage) && (Page as LicenseProtectedPage).ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                _languageList.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (IsPostBack)
			{
				string target = Request.Params.Get("__EVENTTARGET");
				string idTxt = Request.Params.Get("__EVENTARGUMENT");

				int libId;

				if (target == _exportLink.ID)
				{
					if (string.IsNullOrEmpty(idTxt))
						return;

					if (!int.TryParse(idTxt, out libId))
						return;

					LibraryTemplate lib = LibraryTemplateManager.GetLibraryTemplate(libId);

					if (lib == null)
						return;

					string libName = TextManager.GetText(lib.NameTextID);

					GetDownloadResponse(libName.Replace(" ", "_") + "_lib_export.xml");

                    var xmlWriter = new XmlTextWriter(Response.Output) { Formatting = Formatting.Indented };
                    collectImageData(lib);
                    lib.Export(xmlWriter);

					Response.Flush();
					Response.End();

					return;
				}

				if (target == _copyLink.ID)
				{
					if (string.IsNullOrEmpty(idTxt))
						return;

					if (!int.TryParse(idTxt, out libId))
						return;

					LibraryTemplate lib = LibraryTemplateManager.GetLibraryTemplate(libId);

					if (lib == null)
						return;

					LibraryTemplate newLib = LibraryTemplate.Copy(lib);

					return;
				}
			}
		}

        /// <summary>
        /// Reads all images data
        /// </summary>
        /// <param name="lib"></param>
        private void collectImageData(LibraryTemplate lib)
        {
            int[] itemIDs = lib.ListTemplateItemIds();

            foreach (int itemID in itemIDs)
            {
                ItemData itemData = lib.GetItem(itemID);

                if (itemData is ImageItemData)
                {
                    ImageItemData img = (ImageItemData)itemData;
                    if (img.ImageID != null)
                    {
                        if (img.ImagePath.Contains("ViewContent.aspx?contentID=" + img.ImageID))
                        {
                            DBContentItem content = DBContentManager.GetItem(img.ImageID.Value);
                            img.ImageData = content.Data;
                        }
                        if (img.ImagePath.Contains("ViewContent.aspx?ImageID=" + img.ImageID))
                        {
                            img.ImageData = ImageHelper.GetImageData(img.ImageID.Value);
                        }
                    }
                }
            }
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