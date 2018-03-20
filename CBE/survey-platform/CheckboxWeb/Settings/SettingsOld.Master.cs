using System;
using System.Web.UI;
using Checkbox.Web.Page;
using CheckboxWeb.Controls.Button;
using Telerik.Web.UI;

namespace CheckboxWeb.Settings
{
    public partial class SettingsOld : BaseMasterPage, IStatusPage
    {
        public CheckboxButton SaveButton { get { return _saveButton; } }

        public void DisableSave()
        {
            SaveButton.Visible = false;
            SaveButton.Enabled = false;
        }

        public override void SetTitle(string title)
        {
            ((BaseMasterPage)Master).SetTitle(title);
        }

        public override void SetTitleControl(System.Web.UI.Control titleControl)
        {
            ((BaseMasterPage)Master).SetTitleControl(titleControl);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RadAjaxManager manager = RadAjaxManager.GetCurrent(Page);
            Page.ClientScript.RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            manager.ClientEvents.OnResponseEnd = "HideStatus();";
        }

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
            RadAjaxManager manager = RadAjaxManager.GetCurrent(Page);
            manager.AjaxSettings.AddAjaxSetting(sourceControl, _statusControl);


            Page.ClientScript.RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
            manager.ClientEvents.OnResponseEnd = "HideStatus();";

        }

        public void WireUndoControl(Control sourceControl)
        {
            throw new NotImplementedException();
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType)
        {
            ShowStatusMessage(message, messageType, string.Empty, string.Empty);
        }

        public void ShowStatusMessage(string message, StatusMessageType messageType, string actionText, string actionArgument)
        {
            _statusControl.Message = message;
            _statusControl.MessageType = messageType;
            _statusControl.ActionText = actionText;
            _statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        #endregion
    }
}
