using System;
using System.Xml;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;
using Checkbox.Styles;
using Checkbox.Security.Principal;
using Prezza.Framework.ExceptionHandling;
using System.Collections.Generic;

namespace CheckboxWeb.Styles.Forms
{
    public partial class Import : SecuredPage
    {
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

		    Master.Title = WebTextManager.GetText("/pageText/styles/forms/import.aspx/importStyle");
		    Master.OkClick += Submit_OnClick;
		}

        protected void Submit_OnClick(object obj, EventArgs e)
        {
            try
            {
                StyleTemplate newStyle = StyleTemplateManager.CreateStyleTemplate(_uploader.GetUploadedFileDocument(), Context.User as CheckboxPrincipal);
                newStyle.Type = StyleTemplateType.PC;
                StyleTemplateManager.SaveTemplate(newStyle, Context.User as CheckboxPrincipal);

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.onDialogClosed,'refresh');", true);
                //Master.CloseDialog("window.top.onDialogClosed", new Dictionary<string, string>{ {"refresh", "refresh"} );
            }
            catch (XmlException xmlEx)
            {
                ExceptionPolicy.HandleException(xmlEx, "UIProcess");

                _uploadValidation.Text = WebTextManager.GetText("/controlText/forms/Styles/StyleUploader.ascx/fileNotValidXml");
                _uploadValidation.Visible = true;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _uploadValidation.Text = WebTextManager.GetText("/pageText/style/forms/import.aspx/uploadError");
                _uploadValidation.Visible = true;
            }
        }
    }
}
