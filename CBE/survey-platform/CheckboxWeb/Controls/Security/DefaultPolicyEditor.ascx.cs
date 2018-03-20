using System;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Controls.Security
{
    /// <summary>
    /// Default policy editor control
    /// </summary>
    public partial class DefaultPolicyEditor : SecurityEditorControl
    {
        /// <summary>
        /// 
        /// </summary>
        public int DefaultPolicyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Status control
            RegisterClientScriptInclude(
                "statusControl.js",
                ResolveUrl("~/Resources/statusControl.js"));

                
            _defaultPolicyPermissionsGrid.LoadDataCallback = "loadDefaultPolicyPermissions";
            _defaultPolicyPermissionsGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionsListTemplate.html");
            _defaultPolicyPermissionsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionsListItemTemplate.html");
            _defaultPolicyPermissionsGrid.DelayLoad = true;

            _defaultPolicyPermissionMasksGrid.LoadDataCallback = "loadDefaultPolicyPermissions";
            _defaultPolicyPermissionMasksGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionMasksListTemplate.html");
            _defaultPolicyPermissionMasksGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/policyPermissionMasksListItemTemplate.html");
            _defaultPolicyPermissionMasksGrid.DelayLoad = false;
        }
    }
}