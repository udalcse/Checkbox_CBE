using System.Collections.Generic;
using System.Threading;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Users;
using Checkbox.Users.Data;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// Hash passwords
    /// </summary>
    public partial class HashPasswords : SettingsPage
    {
        /// <summary>
        /// Override oninit
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            Master.HideDialogButtons();
            Master.SetTitle(WebTextManager.GetText("/pageText/settings/security.aspx/encryptPasswords"));

            _doEncrypt.Click += DoEncrypt;
        }

        /// <summary>
        /// Perform encryption work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoEncrypt(object sender, System.EventArgs e)
        {
            //Seed progress cache so it's not empty on first request to get progress
            ProgressProvider.SetProgress(
                "SecuritySettings_Hasher",
                WebTextManager.GetText("/pageText/settings/security.aspx/encryptionStarted"),
                string.Empty,
                ProgressStatus.Pending,
                0,
                100);

            //Start hashing and progress monitoring
            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "ProgressStart",
                "$(document).ready(function(){startProgress('SecuritySettings_Hasher');});",
                true);
        }
      }
}
