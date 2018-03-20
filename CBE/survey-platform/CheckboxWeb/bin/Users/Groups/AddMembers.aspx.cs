using System;
using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Users.Groups
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddMembers : GroupEditorPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            
            Master.HideDialogButtons();

            var pageTitle = WebTextManager.GetText("/pageText/users/groups/addMembers.aspx/title");

            Master.SetTitle(pageTitle.Replace("{0}", Utilities.StripHtml(Group.Name, 64)));

            //Initialize grid
            _availableUsersGrid.InitialSortField = "UserIdentifier";
            _availableUsersGrid.ListTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/availableUserListTemplate.html");
            _availableUsersGrid.ListItemTemplatePath = ResolveUrl("~/Users/Groups/jqtmpl/availableUserListItemTemplate.html");
            _availableUsersGrid.LoadDataCallback = ClientID + "loadAvailableGridAjax";
            _availableUsersGrid.EmptyGridText = WebTextManager.GetText("/pageText/Users/groups/addMembers.aspx/noUsersFound");
            _availableUsersGrid.DelayLoad = false;
            _availableUsersGrid.IsAjaxScrollModeEnabled = true;
            _availableUsersGrid.LoadingTextId = WebTextManager.GetText("/pageText/Users/groups/addMembers.aspx/findingUsers");
            _availableUsersGrid.FilterItemType = "usersNotInGroup";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Service helper
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            //Survey management
            RegisterClientScriptInclude(
                "svcUserManagement.js",
                ResolveUrl("~/Services/js/svcUserManagement.js"));

            //Highlight
            RegisterClientScriptInclude(
                "jquery.highlight-3.yui.js",
                ResolveUrl("~/Resources/jquery.highlight-3.yui.js"));

            //Text helper
            RegisterClientScriptInclude(
                "textHelper.js",
                ResolveUrl("~/Resources/textHelper.js"));

            //Status control
            RegisterClientScriptInclude(
                "statusControl.js",
                ResolveUrl("~/Resources/statusControl.js"));
        
            //live search
            RegisterClientScriptInclude(
                "gridLiveSearch.js",
                ResolveUrl("~/Resources/gridLiveSearch.js"));
        }

        #region Properties

        /// <summary>
        /// Get the role permission required for the page
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Group.Edit"; }
        }

        #endregion

    }
}
