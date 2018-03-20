using System;
using System.Web;
using Checkbox.Web.Page;
using Checkbox.Forms;
using Checkbox.Security.Principal;
using Checkbox.Management;
using Checkbox.Globalization.Text;
using Checkbox.Web;

namespace CheckboxWeb.Libraries
{
    public partial class Create : SecuredPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _nameRequired.Text = WebTextManager.GetText("/pageText/libraries/create.aspx/nameRequired");
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

		    Master.Title = WebTextManager.GetText("/pageText/libraries/create.aspx/createLibrary");
		    Master.OkClick += SubmitBtn_Click;
		}

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            string name = _libraryName.Text.Trim();
            string desc = _description.Text.Trim();

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                name = Server.HtmlEncode(name);
                desc = Server.HtmlEncode(desc);
            }
            LibraryTemplate template = null;

            if (!LibraryTemplateManager.LibraryTemplateExists(name, null))
            {
                template = LibraryTemplateManager.CreateLibraryTemplate(name, HttpContext.Current.User as CheckboxPrincipal);

                TextManager.SetText(template.NameTextID, WebTextManager.GetUserLanguage(), name);
                TextManager.SetText(template.DescriptionTextID, WebTextManager.GetUserLanguage(), desc);

                Page.ClientScript.RegisterClientScriptBlock(
                    GetType(),
                    "Redirect",
                    "closeWindowAndRedirectParentPage('', null, 'Manage.aspx?l=" + template.ID.Value + "');",
                    true);
            }
            else
            {
                _duplicateName.Visible = true;
            }
        }
    }
}
