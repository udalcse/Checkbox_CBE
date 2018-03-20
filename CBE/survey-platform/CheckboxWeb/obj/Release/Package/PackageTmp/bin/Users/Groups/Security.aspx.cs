using System;

namespace CheckboxWeb.Users.Groups
{
    public partial class Security : GroupEditorPage
    {
        /// <summary>
        /// Simple page that sets up session and redirects to ACL editor.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RedirectToAclEditor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetAclPermissionsToGrant()
        {
            return "Group.View";
        }
    }
}