using Checkbox.Web;
using Checkbox.Web.UI.Controls.Security;

namespace CheckboxWeb.Controls.Security
{
    /// <summary>
    /// Editor for granting/removing access to an access controllable resource based
    /// on a permission.
    /// </summary>
    public partial class GrantAccessControl : SecurityEditorControl
    {

        /// <summary>
        /// Get/set text to dispay for available principals
        /// </summary>
        public string AvailableText
        {
            get { return _availableLbl.Text; }
            set { _availableLbl.Text = value; }
        }

        /// <summary>
        /// Get/set text for selected princpals
        /// </summary>
        public string SelectedText
        {
            get { return _currentLbl.Text; }
            set { _currentLbl.Text = value; }
        }

        /// Specify whether grids should be automatically loaded or not.  If delay is set to true, grids
        /// will not be loaded until reload grids method are called. Default value is true.
        public bool DelayLoad
        {
            get 
            {
                bool temp;
                return bool.TryParse(_delayLoad.Text, out temp) && temp;
            }
            set { _delayLoad.Text = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            _availableGrid.ItemClickCallback = ID + "onAvailableItemSelect";
            _availableGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/grantAccessAvailableListTemplate.html");
            _availableGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/grantAccessAvailableListItemTemplate.html");
            _availableGrid.LoadDataCallback = ID + "loadAvailableGridAjax";
            _availableGrid.EmptyGridText = WebTextManager.GetText("/pageText/security/securityEditor.aspx/noEntriesAdded");
            _availableGrid.DelayLoad = DelayLoad;
            _availableGrid.IsAjaxScrollModeEnabled = true;
            _availableGrid.InitialFilterField = "UniqueIdentifier";
            _availableGrid.FilterItemType = "available";

            _currentGrid.ItemClickCallback = ID + "onCurrentItemSelect";
            _currentGrid.ListTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/grantAccessCurrentListTemplate.html");
            _currentGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Security/jqtmpl/grantAccessCurrentListItemTemplate.html");
            _currentGrid.LoadDataCallback = ID + "loadCurrentGridAjax";
            _currentGrid.EmptyGridText = WebTextManager.GetText("/pageText/security/securityEditor.aspx/noEntriesAdded");
            _currentGrid.DelayLoad = DelayLoad;
            _currentGrid.IsAjaxScrollModeEnabled = true;
            _currentGrid.InitialFilterField = "UniqueIdentifier";
            _currentGrid.FilterItemType = "current";

            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcSecurityManagement.js",
                ResolveUrl("~/Services/js/svcSecurityManagement.js"));
        }
    }
}
