using System;
using System.Web.UI;
using Checkbox.Web;
using Checkbox.Web.Page;
using CheckboxWeb.Users.Controls;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users
{
    public partial class GroupsUserProfile : EditUserPage, IStatusPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.Title = "Edit profile information";
            Master.OkClick += SaveUserInfo_Click;

            //init controls 
            _profileEditor.Initialize(UserToEdit, IsUserReadOnly);
            _credentialEditor.Initialize(UserToEdit, IsUserReadOnly, IsExternalUser, IsCheckboxNetworkUser,
                ExternalUserId);
            _roleSelector.Initialize(UserToEdit, IsUserReadOnly);
        }
     
        protected void SaveUserInfo_Click(object sender, EventArgs e)
        {
            if (IsUserReadOnly)
                return;

            try
            {
                if (ValidateModal())
                {
                    _profileEditor.SaveProfileProperties();
                    _roleSelector.SaveRoles();
                    _credentialEditor.SaveUserInfoButton();

                    Master.CloseDialog("{page: 'groupsuserprofile'}", true);
                }
            }
            catch (Exception exception)
            {
                ExceptionPolicy.HandleException(exception, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), exception.Message), StatusMessageType.Error);
            }
        
        }

        private bool ValidateModal()
        {
            bool isValid = true;

            //validate roles selector user control
            if (!_roleSelector.IsValid)
            {
                _roleRequiredError.Visible = true;
                isValid = false;
            }

            var credentialStatusCode = _credentialEditor.Validate();

            if (credentialStatusCode != CredentialEditorErrorCode.None)
            {
                ShowCredentialMessage(credentialStatusCode);
                isValid = false;
            }

            return isValid;
        }

        private void ShowCredentialMessage(CredentialEditorErrorCode code)
        {

            switch (code)
            {
                    case CredentialEditorErrorCode.UserNameEmailRulesViolated:
                    ShowStatusMessage(
                        string.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"),
                            WebTextManager.GetText("/pageText/users/properties.aspx/userNameEmailRulesViolated")),
                        StatusMessageType.Error);
                    break;
                    case CredentialEditorErrorCode.EmailIsNotValid:
                    ShowStatusMessage(
                        string.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"),
                            WebTextManager.GetText("/pageText/users/properties.aspx/emailInvalid")),
                        StatusMessageType.Error);
                    break;
            }

        }

        #region IStatusPage Members

        public void WireStatusControl(Control sourceControl)
        {
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
            _statusControl.ShowStatus();
        }

        #endregion
    }
}