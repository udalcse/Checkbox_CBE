using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Panels;
using Checkbox.Web.Page;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Common;

namespace CheckboxWeb.Users.EmailLists
{
    public partial class Properties : EmailListEditorPage, IStatusPage
    {

        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set up the page title with link back to email list mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Users/EmailLists/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += String.Format(WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/title"), Utilities.TruncateText(EmailList.Name, 50));

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            //Set the controls' initial state
            _emailListName.Text = EmailList.Name;
            _emailListDescription.Text = EmailList.Description;
            
            Master.OkClick += SaveEmailListInfoButton_Click;
        }

        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            RegisterClientScriptInclude("HideStatus", ResolveUrl("~/Resources/HideStatus.js"));
        }

        #region Control event handlers


        /// <summary>
        /// Handles the click event of the save button 
        /// - Saves changes to the group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveEmailListInfoButton_Click(object sender, EventArgs e)
        {
            string cleanName = _emailListName.Text.Trim();
            string cleanDescription = _emailListDescription.Text.Trim();

            if (!Checkbox.Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = Server.HtmlEncode(cleanName);
                cleanDescription = Server.HtmlEncode(cleanDescription);
            }

            if (EmailListManager.IsDuplicateName(EmailList.ID, cleanName))
            {
                _emailListNameErrorLabel.Text = WebTextManager.GetText("/pageText/users/emaillists/properties.aspx/emailListNameExists");
                _emailListNameErrorLabel.Visible = true;
                return;
            }

            try
            {
                EmailList.Name = cleanName;
                EmailList.Description = cleanDescription;
                EmailList.ModifiedBy = User.Identity.Name;
                EmailList.Save();
                ShowStatusMessage(WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/emailListUpdateSuccess"), StatusMessageType.Success);

                //Close window
                var args = new Dictionary<string, string>
                               {
                                   {"page", "properties"},
                                   {"emailListId", EmailList.ID.ToString()}
                               };
                Master.CloseDialog(args);
            }
            catch (Exception err)
            {
                ExceptionPolicy.HandleException(err, "UIProcess");
                ShowStatusMessage(String.Format(WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/emailListUpdateError"), err.Message), StatusMessageType.Error);
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
