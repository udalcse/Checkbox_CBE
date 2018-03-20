using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Users;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Users.Groups
{
    public partial class Properties : GroupEditorPage, IStatusPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set the controls' initial state
            _groupName.Text = Group.Name;
            _groupDescription.Text = Group.Description;

            Master.OkClick += SaveGroupInfoButtton_Click;
            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/users/groups/members.aspx/propertiesTab");


        }

        #region Control event handlers

        /// <summary>
        /// Handles the click event of the save button 
        /// - Saves changes to the group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveGroupInfoButtton_Click(object sender, EventArgs e)
        {
            string cleanName = _groupName.Text.Trim();
            string cleanDescription = _groupDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            if (GroupManager.IsDuplicateName(Group.ID, cleanName))
            {
                _groupNameErrorLabel.Text = WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameExists");
                _groupNameErrorLabel.Visible = true;
                return;
            }

            try
            {
                bool needRename = false;
                if (!Group.Name.Equals(cleanName))
                {
                    Group.Name = cleanName;
                    needRename = true;
                }
                Group.Description = cleanDescription;
                Group.Save();
                
                ShowStatusMessage(WebTextManager.GetText("/pageText/users/groups/properties.aspx/groupUpdateSuccess"), StatusMessageType.Success);

                //Close window
                var args = new Dictionary<string, string>
                               {
                                   {"page", "properties"},
                                   {"needRename", needRename.ToString().ToLower()},
                                   {"groupId", Group.ID.ToString()},
                                   {"groupName", cleanName}
                               };
                Master.CloseDialog(args);
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/groups/properties.aspx/groupUpdateError"), err.Message), StatusMessageType.Error);
            }
        }
        /*
        /// <summary>
        /// Deletes all users in the current group from the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DeleteAllButton_Click(object sender, EventArgs e)
        {
            //First remove the users from this group
            string removeMsg;
            StatusMessageType removeStatus;
            RemoveAllUsersFromGroup(out removeMsg, out removeStatus);

            List<string> usersToDelete = new List<string>();
            bool usersSkipped = false;

            foreach (string userUniqueIdentifier in Group.GetUserIdentifiers())
            {
                if (!UserManager.IsCheckboxUser(userUniqueIdentifier))
                {
                    usersSkipped = true;
                    continue;
                }

                usersToDelete.Add(userUniqueIdentifier);
            }

            string deletionStatus = String.Empty;
            bool deletionSuccess = true;
            int deletionCount = 0;
            if (usersToDelete.Count > 0)
            {

                foreach (string user in usersToDelete)
                {
                    deletionSuccess = UserManager.DeleteUser(HttpContext.Current.User as CheckboxPrincipal, user, false, out deletionStatus);
                    if (deletionSuccess)
                    {
                        deletionCount++;
                    }
                }
            }

            //Assemble the status message depending on what happened
            // possible states:
            // - Checkbox users deleted, (show count)
            // - Network users skipped, users deleted (show skipped msg, show count)
            // - Error during deletion (show error msg, show count)
            // - Network users skipped, error during deletion (show error msg, show skipped msg,  show count)
            //if any of the multi-state messages need to be displayed, display the messages as an unordered list, otherwise just show the count
            StringBuilder statusMessage = new StringBuilder();
            StatusMessageType msgType = StatusMessageType.Warning;
            if (!usersSkipped && deletionSuccess)
            {
                statusMessage.Append(String.Format(WebTextManager.GetText("/pageText/Users/Manage.aspx/usersDeleted"), deletionCount));
            }
            else
            {
                statusMessage.Append("<ul>");
                if (!deletionSuccess)
                {
                    statusMessage.Append("<li>");
                    statusMessage.Append(String.Format(WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteUserError"), deletionStatus));
                    statusMessage.Append("</li>");
                    msgType = StatusMessageType.Error;
                }

                if (usersSkipped)
                {
                    statusMessage.Append("<li>");
                    statusMessage.Append(WebTextManager.GetText("/pageText/Users/Manage.aspx/unableToDeleteNetworkUser"));
                    statusMessage.Append("</li>");
                }

                statusMessage.Append("<li>");
                statusMessage.Append(String.Format(WebTextManager.GetText("/pageText/Users/Manage.aspx/usersDeleted"), deletionCount));
                statusMessage.Append("</li></ul>");

            }

            ShowStatusMessage(statusMessage.ToString(), msgType);

        }

        /// <summary>
        /// Removes all users from the current group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RemoveAllButton_Click(object sender, EventArgs e)
        {
            string statusMessage = string.Empty;
            StatusMessageType msgType;
            RemoveAllUsersFromGroup(out statusMessage, out msgType);
            ShowStatusMessage(statusMessage, msgType);
        }

        private void RemoveAllUsersFromGroup(out string statusMessage, out StatusMessageType msgType)
        {
            bool deletionSuccess = true;
            int deletionCount = 0;
            string deletionStatus = string.Empty;

            try
            {
                List<string> usersToDelete = new List<string>(Group.GetUserIdentifiers());
                foreach (string userUniqueIdentifier in usersToDelete)
                {
                    Group.RemoveUser(userUniqueIdentifier);
                    deletionCount++;
                }
            }
            catch (Exception err)
            {
                deletionStatus = err.Message;
                deletionSuccess = false;
            }

            //Assemble the status message depending on what happened
            StringBuilder statusMessageBuilder = new StringBuilder();
            msgType = StatusMessageType.Success;
            if (deletionSuccess)
            {
                statusMessageBuilder.Append(String.Format(WebTextManager.GetText("/pageText/Users/groups/members.aspx/usersDeleted"), deletionCount));
            }
            else
            {
                statusMessageBuilder.Append("<ul>");
                if (!deletionSuccess)
                {
                    statusMessageBuilder.Append("<li>");
                    statusMessageBuilder.Append(String.Format(WebTextManager.GetText("/pageText/Users/groups/members.aspx//deleteUserError"), deletionStatus));
                    statusMessageBuilder.Append("</li>");
                    msgType = StatusMessageType.Error;
                }

                statusMessageBuilder.Append("<li>");
                statusMessageBuilder.Append(String.Format(WebTextManager.GetText("/pageText/Users/groups/members.aspx/usersDeleted"), deletionCount));
                statusMessageBuilder.Append("</li></ul>");

            }

            statusMessage = statusMessageBuilder.ToString();
            Group.Save();
            ShowStatusMessage(statusMessageBuilder.ToString(), msgType);

        }*/

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
