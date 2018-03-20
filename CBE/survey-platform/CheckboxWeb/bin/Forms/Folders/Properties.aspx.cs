using System;

using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Folders
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Properties : SecuredPage
    {
        private FormFolder _theFolder;

        /// <summary>
        /// Get folder id
        /// </summary>
        [QueryParameter("f", IsRequired = true)]
        public int FolderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "FormFolder.FullControl"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IAccessControllable GetControllableEntity()
        {
            return CurrentFolder;
        }

        /// <summary>
        /// 
        /// </summary>
        public FormFolder CurrentFolder
        {
            get { return _theFolder ?? (_theFolder = FolderManager.GetFolder(FolderId)); }
        }

        /// <summary>
        /// Ok click
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            
            if (!IsPostBack)
            {
                _folderNameTxt.Text = CurrentFolder.Name;
            }
            
            Master.OkClick += _okBtn_Click;

            Master.SetTitle(WebTextManager.GetText("/pageText/folderProperties.aspx/folderProperties"));

            _nameRequiredValidator.Text = WebTextManager.GetText("/pageText/forms/folders/create.aspx/nameRequired");
            _folderNameInUseValidator.Text = WebTextManager.GetText("/pageText/forms/folders/create.aspx/nameInUse");
            _folderNameInUseValidator.ServerValidate += _folderNameInUseValidator_ServerValidate;
        }

        //
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (Page != null)
            {
                Page.Title = string.Format("{0} - {1}", WebTextManager.GetText("/pageText/forms/manage.aspx/renameFolder"), Utilities.StripHtml(_folderNameTxt.Text, 64));
            }
        }

        /// <summary>
        /// Validate folder name in use
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        void _folderNameInUseValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string value = args.Value;

            if (!ApplicationManager.AppSettings.AllowHTMLNames)
            {
                value = Server.HtmlEncode(value);
            }

            if (value.Equals(CurrentFolder.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                args.IsValid = true;
                return;
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

                    name = name.Replace("'", "");

                    //Update the folder
                    CurrentFolder.Name = name;
                    CurrentFolder.Save(UserManager.GetCurrentPrincipal());

                    //Close and redirect parent
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "Redirect",
                        "closeWindowAndRedirectParentPage('', null, '../Manage.aspx?f=" + FolderId + "&nf=" + CurrentFolder.ID + "');",
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