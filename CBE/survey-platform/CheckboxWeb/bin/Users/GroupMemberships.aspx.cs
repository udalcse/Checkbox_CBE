using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// 
    /// </summary>
    public partial class GroupMemberships : EditUserPage, IStatusPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set the user to edit on the groupSelector
            _groupSelector.Initialize(UserToEdit, IsUserReadOnly);

            string title = IsUserReadOnly || IsExternalUser
                ? WebTextManager.GetText("/pageText/users/properties.aspx/titleView")
                : WebTextManager.GetText("/pageText/users/properties.aspx/groupsTab");

            if (UserToEdit != null)
            {
                title += " - " + UserToEdit.Identity.Name;
            }

            Master.SetTitle(title);
            Master.OkClick += SaveGroupsButtton_Click;

            if (IsUserReadOnly)
            {
                _readOnlyUserWarningPanel.Visible = true;
                Master.OkVisible = false;
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
        /// Handles the click event of the save button on the profile properties panel
        /// - Saves changes to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveGroupsButtton_Click(object sender, EventArgs e)
        {
            if (IsUserReadOnly)
            {
                return;
            }

            try
            {
                List<int> currentMemberships = GroupManager.ListGroupMembershipIds(UserToEdit.Identity.Name);
                List<int> selectedGroups = _groupSelector.SelectedGroupIDs;

                bool needToInvalidateCache = false;

                foreach (int currentGroupId in currentMemberships)
                {
                    if (!selectedGroups.Contains(currentGroupId))
                    {
                        Group g = GroupManager.GetGroup(currentGroupId);

                        if (g != null)
                        {
                            g.RemoveUser(UserToEdit.Identity.Name);
                            UserManager.SetUserModifier(UserToEdit.Identity.Name, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name);
                            needToInvalidateCache = true;
                            g.Save();
                        }
                    }
                }
                if (selectedGroups.Count > 0)
                {
                    foreach (Int32 groupID in selectedGroups)
                    {
                        Group group = GroupManager.GetGroup(groupID);

                        if (group != null)
                        {
                            group.AddUser(UserToEdit.Identity);
                            UserManager.SetUserModifier(UserToEdit.Identity.Name, ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name);
                            needToInvalidateCache = true;
                            group.Save();
                        }
                    }
                }

                if (needToInvalidateCache)
                {
                    GroupManager.InvalidateUserMemberships(UserToEdit.Identity.Name);
                }

                Master.CloseDialog("{page: 'groups'}", true);
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