using System;

namespace CheckboxWeb.Users.EmailLists
{
    public partial class Security : EmailListEditorPage
    {
        /// <summary>
        /// Simple page that sets up session and redirects to ACL editor.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RedirectToAclEditor();
        }

        protected override string GetAclPermissionsToGrant()
        {
            return "EmailList.View,EmailList.Edit";
        }
    }
}