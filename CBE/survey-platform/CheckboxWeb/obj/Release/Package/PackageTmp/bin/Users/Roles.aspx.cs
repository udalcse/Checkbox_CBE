using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users
{
    public partial class Roles : EditUserPage, IStatusPage
    {

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set the user to edit on the roleSelector
            _roleSelector.Initialize(UserToEdit, IsUserReadOnly);
            
            string title = IsUserReadOnly || IsExternalUser
                ? WebTextManager.GetText("/pageText/users/properties.aspx/titleView")
                : WebTextManager.GetText("/pageText/users/properties.aspx/rolesTab");

            if (UserToEdit != null)
            {
                title += " - " + UserToEdit.Identity.Name;
            }

            Master.SetTitle(title);
            Master.OkClick += SaveRolesButton_Click;

            if (IsUserReadOnly)
            {
                Master.OkVisible = false;
                _readOnlyUserWarningPanel.Visible = true;
            }

        }

        /// <summary>
        /// Register hide status script
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
        }


        #region Control event handlers

        /// <summary>
        /// Handles the click event of the save button on the roles panel
        /// - Saves changes to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveRolesButton_Click(object sender, EventArgs e)
        {
            if (IsUserReadOnly)
            {
                return;
            }

            try
            {               
                if (!_roleSelector.SelectedRoles.Any())
                    _roleRequiredError.Visible = true;
                else
                {
                   _roleSelector.SaveRoles();

                    Master.CloseDialog("{page: 'roles'}", true);
                }
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/properties.aspx/updateError"), err.Message), StatusMessageType.Error);
                return;
            }

        }


        #endregion

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
            //_statusControl.ActionText = actionText;
            //_statusControl.ActionArgument = actionArgument;
            _statusControl.ShowStatus();
        }

        #endregion   
    }
}
