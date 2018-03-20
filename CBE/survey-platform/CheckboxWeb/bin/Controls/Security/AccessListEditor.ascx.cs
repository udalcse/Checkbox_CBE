using System;
using Checkbox.Web;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Controls.Security
{
    /// <summary>
    /// Security editor control
    /// </summary>
    public partial class AccessListEditor : SecurityEditorControl
    {
        #region Display Related Settings

        /// <summary>
        /// Get/set whether or not to hide instructions
        /// </summary>
        public bool ShowInstructions
        {
            get { return _instructionsPanel.Visible; }
            set { _instructionsPanel.Visible = value; }
        }
        
        #endregion

        /// <summary>
        /// Add script to fire ajax request on list update
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Visible)
            {
                _aclGrid.ItemClickCallback = "onAclItemSelect";
                _aclGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/aclGridListTemplate.html");
                _aclGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/aclGridListItemTemplate.html");
                _aclGrid.LoadDataCallback = "loadAclGridAjax";
                _aclGrid.EmptyGridText = WebTextManager.GetText("/pageText/security/securityEditor.aspx/noEntriesAdded");
                _aclGrid.IsAjaxScrollModeEnabled = true;
                _aclGrid.InitialFilterField = "UniqueIdentifier";
                _aclGrid.FilterItemType = "acl";

                _permissionsGrid.LoadDataCallback = "loadPermissions";
                _permissionsGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionsListTemplate.html");
                _permissionsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionsListItemTemplate.html");
                _permissionsGrid.DelayLoad = !Checkbox.Management.ApplicationManager.UseSimpleSecurity;

                _permissionMasksGrid.LoadDataCallback = "loadPermissions";
                _permissionMasksGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionMasksListTemplate.html");
                _permissionMasksGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionMasksListItemTemplate.html");
                _permissionMasksGrid.DelayLoad = !Checkbox.Management.ApplicationManager.UseSimpleSecurity;
            }

            //Status control
            RegisterClientScriptInclude(
                "statusControl.js",
                ResolveUrl("~/Resources/statusControl.js"));

            //Security Management Service
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcSecurityManagement.js",
                ResolveUrl("~/Services/js/svcSecurityManagement.js"));

            RegisterClientScriptInclude(
                "gridLiveSearch.js",
                ResolveUrl("~/Resources/gridLiveSearch.js"));
        }
    }
}