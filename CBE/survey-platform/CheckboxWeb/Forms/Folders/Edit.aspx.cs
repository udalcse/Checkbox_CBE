using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Folders
{
    /// <summary>
    /// Edit folder properties
    /// </summary>
    public partial class Edit : SecuredPage
    {
        /// <summary>
        /// Get folder id
        /// </summary>
        [QueryParameter("f")]
        public int? FolderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _okBtn.Click += _okBtn_Click;

            FormFolder folder = FolderManager.GetFolder(FolderId.Value);

            if (folder != null)
            {
                _folderNameTxt.Text = folder.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okBtn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string folderName = ApplicationManager.AppSettings.AllowHTMLNames
                ? _folderNameTxt.Text.Trim()
                : Utilities.StripHtml(_folderNameTxt.Text.Trim(), null);

                FormFolder folder = FolderManager.GetFolder(FolderId.Value);

                if (folder != null)
                {
                    folder.Name = folderName;
                    folder.Save(UserManager.GetCurrentPrincipal());
                }

                Page.ClientScript.RegisterClientScriptBlock(
                   GetType(),
                   "Redirect",
                   "closeWindowAndRedirectParentPage('', null, '../Manage.aspx?f=" + FolderId + "&rn=true');",
                   true);
            }
        }
    }
}
