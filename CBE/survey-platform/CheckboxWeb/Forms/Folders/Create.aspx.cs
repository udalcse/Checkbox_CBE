using System;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Folders
{
    /// <summary>
    /// Create new folder
    /// </summary>
    public partial class Create : SecuredPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += _okBtn_Click;

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/forms/folders/create.aspx/createFolder");

            _nameRequiredValidator.Text = WebTextManager.GetText("/pageText/forms/folders/create.aspx/nameRequired");
            _folderNameInUseValidator.Text = WebTextManager.GetText("/pageText/forms/folders/create.aspx/nameInUse");
            _folderNameInUseValidator.ServerValidate += _folderNameInUseValidator_ServerValidate;
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (Page != null)
            {
                Page.Title = WebTextManager.GetText("/pageText/forms/folders/create.aspx/createFolder");
            }
        }

        /// <summary>
        /// Validate folder name in use
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        void _folderNameInUseValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string value = args.Value.Replace("'", "");

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = Server.HtmlEncode(value);
            }

            args.IsValid = !FolderManager.FolderExists(null, value, User as CheckboxPrincipal);
        }

        /// <summary>
        /// Handle user ok click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    string name = ApplicationManager.AppSettings.AllowHTMLNames
                        ? _folderNameTxt.Text.Trim()
                        : Utilities.StripHtml(_folderNameTxt.Text.Trim(), null);

					name = name.Replace("'","");

                    //Create & Save
                    var folder = new FormFolder { Name = name };
                    folder.Save(UserManager.GetCurrentPrincipal(), ApplicationManager.UseSimpleSecurity);
                    
                    folder.DefaultPolicy.Permissions.AddRange(new string[] { "FormFolder.FullControl", "FormFolder.Read" });
                    folder.DefaultPolicy.Persist();

                    //Close and redirect parent
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "Redirect",
                        "closeWindowAndRedirectParentPage('', null, '../Manage.aspx?nf=" + folder.ID + "');",
                        true);
                }
            }
            catch (Exception)
            {
                _nameErrorLbl.Text = WebTextManager.GetText("/pageText/forms/folders/create.aspx/nameInUse");
            }
        }
    }
}
