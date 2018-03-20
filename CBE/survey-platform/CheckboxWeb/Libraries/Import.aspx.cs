using System;
using System.Web;
using System.IO;
using System.Xml;
using Checkbox;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Libraries
{
    public partial class Import : SecuredPage
    {

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Master.Title = WebTextManager.GetText("/pageText/libraryimport.aspx/title");
            Master.OkClick += Submit_OnClick;
        }

		/// <summary>
        /// Handle upload button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_OnClick(object sender, EventArgs e)
		{
            try
            {
                string uName = LibraryTemplate.GetUniqueName(_uploader.LibraryName);
                LibraryTemplate template = LibraryTemplateManager.CreateLibraryTemplate(uName,
                                                                                        HttpContext.Current.User as
                                                                                        Checkbox.Security.Principal.
                                                                                            CheckboxPrincipal);
                XmlDocument document = _uploader.GetUploadedFileDocument();
                template.Import(document.DocumentElement, null, HttpContext.Current.User as Checkbox.Security.Principal.CheckboxPrincipal);
                template.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                template.Save();

                Page.ClientScript.RegisterClientScriptBlock(
                    GetType(),
                    "Redirect",
                    "closeWindowAndRedirectParentPage('', null, 'Manage.aspx?l=" + template.ID.Value + "');",
                    true);
            }
            catch(XmlException xmlEx)
            {
                ExceptionPolicy.HandleException(xmlEx, "UIProcess");

                _uploadValidation.Text = WebTextManager.GetText("/controlText/forms/libraries/libraryUploader.ascx/fileNotValidXml");
                _uploadValidation.Visible = true;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _uploadValidation.Text = WebTextManager.GetText("/pageText/libraryImport.aspx/uploadError");
                _uploadValidation.Visible = true;
            }
		}
    }
}
