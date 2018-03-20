using System;
using System.Collections.Generic;
using System.IO;
using Checkbox.Common;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    /// <summary>
    /// Show product tour messagse
    /// </summary>
    public partial class Messages : SecuredPage
    {
        private bool _abort = false;

        protected override void OnPageInit()
        {
            base.OnPageInit();

            _doNotShowAgainButton.Click += DoNotShowAgain_Click;
            _closeButton.Click += Close_Click;

            if (UserManager.GetCurrentPrincipal() == null)
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "Close", "closeWindowAndRefreshParentPage();", true);
                _abort = true;
            }

            Master.HideDialogButtons();
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            _pageText.Text = string.Empty;

            List<FileInfo> messages = GetSessionValue<List<FileInfo>>("Messages", null);
            if (messages != null)
            {
                foreach (FileInfo message in messages)
                {
                    _pageText.Text += FileUtilities.ReadTextFile(message.FullName);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, EventArgs e)
        {
            Session["Messages"] = null;
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "Close", "closeWindowAndRefreshParentPage();", true);
        }

        /// <summary>
        /// Opt out of messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoNotShowAgain_Click(object sender, EventArgs e)
        {
            CheckboxPrincipal currentPrincipal = UserManager.GetCurrentPrincipal();

            //Check may be redundant, but do it any way.
            if (currentPrincipal != null && currentPrincipal.Identity != null)
            {
                UserManager.OptOutOfProductTourMessages(currentPrincipal.Identity.Name, GetSessionValue<List<FileInfo>>("Messages", null));
            }

            Session["Messages"] = null;
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "Close", "closeWindowAndRefreshParentPage();", true);
        }
    }
}
