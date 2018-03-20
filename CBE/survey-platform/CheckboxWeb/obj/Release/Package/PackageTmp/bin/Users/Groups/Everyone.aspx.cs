namespace CheckboxWeb.Users.Groups
{
    public partial class Everyone : GroupEditorPage
    {
        /// <summary>
        /// Simple page that sets up session and redirects to ACL editor.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            RedirectToAclEditor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetAclPermissionsToGrant()
        {
            return "Group.View,Group.Edit";
        }
    }
}
