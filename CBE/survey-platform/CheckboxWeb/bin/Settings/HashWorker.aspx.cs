using System;
using System.Collections.Generic;
using Checkbox.Management;
using Checkbox.Progress;
using Checkbox.Users;
using Checkbox.Users.Data;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Settings
{
    public partial class HashWorker : SettingsPage
    {
        protected int UnencryptedPasswordsTotal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey() { return "SecuritySettings_Hasher"; }

        /// <summary>
        /// Set progress message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currentPercent"></param>
        protected void SetProgress(string message, int currentPercent)
        {
            //Otherwise, we are go for hashing
            ProgressProvider.SetProgress(
                ProgressKey,
                message,
                string.Empty,
                ProgressStatus.Running,
                currentPercent,
                100);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            ApplicationManager.AppSettings.UseEncryption = true;

            var userList = UserManager.ListUnencryptedPasswordUsers();
            UnencryptedPasswordsTotal = userList.Count;

            try
            {
                int index;
                for (index = 0; index < UnencryptedPasswordsTotal; index++)
                {
                    UserManager.EncryptUserPassword(userList[index], ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name);

                    var percent = (int)(((double)index / UnencryptedPasswordsTotal) * 100);
                    SetProgress(string.Format(WebTextManager.GetText("/pageText/settings/security.aspx/encryptedCount"), index, UnencryptedPasswordsTotal), percent);
                }

                var finishedText = WebTextManager.GetText("/pageText/settings/security.aspx/encryptionCompleted");

                //Set progress
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        Message = finishedText,
                        Status = ProgressStatus.Completed,
                        CurrentItem = index,
                        TotalItemCount = UnencryptedPasswordsTotal
                    }
                );

                WriteResult(new { success = true });
            }
            catch (Exception ex)
            {
                //Set progress status
                ProgressProvider.SetProgress(ProgressKey, "An error occurred while encrypting user passwords.", ex.Message, ProgressStatus.Error, 100, 100);
                WriteResult(new { success = false, error = ex.Message });
            }
        }
    }
}