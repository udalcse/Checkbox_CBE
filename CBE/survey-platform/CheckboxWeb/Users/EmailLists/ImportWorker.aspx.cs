using System;
using System.Collections.Generic;
using Checkbox.Progress;
using Checkbox.Web;

namespace CheckboxWeb.Users.EmailLists
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ImportWorker : EmailListEditorPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected List<string> AddressesToLoad
        {
            get { return GetSessionValue("EmailListAddressesToLoad", new List<string>()); }
            set { Session["EmailListAddressesToLoad"] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GenerateProgressKey()
        {
            return Session.SessionID + "_AddAddresses";
        }


        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Various counters for progress tracking
            int addressCount = AddressesToLoad.Count;
            int createdCount = 0;

            try
            {
                var progressBaseMessage =
                    WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/importing");

                //Start progress tracking
                ProgressProvider.SetProgress(ProgressKey,
                                             WebTextManager.GetText(
                                                 "/pageText/users/emailLists/addAddresses.aspx/preparing"),
                                             string.Empty, ProgressStatus.Pending, 0, addressCount);

                foreach (string address in AddressesToLoad)
                {
                    EmailList.AddPanelist(address);
                    createdCount++;


                    //Set progress status
                    ProgressProvider.SetProgressCounter(ProgressKey, progressBaseMessage,
                                                        createdCount,
                                                        addressCount, 100, 100);

                }
                EmailList.ModifiedBy = Page.User.Identity.Name;
                EmailList.Save();
            }
            catch (Exception ex)
            {
                //Set progress status
                ProgressProvider.SetProgress(ProgressKey, "An error occurred while importing users.", ex.Message,
                                             ProgressStatus.Error, 100, 100);

                WriteResult(new {success = false, error = ex.Message});
            }

            //Set progress status
            //Set progress status
            ProgressProvider.SetProgress(ProgressKey,
                                         string.Format(
                                             WebTextManager.GetText(
                                                 "/pageText/users/emailLists/addAddresses.aspx/fcompleted"),
                                             createdCount), string.Empty,
                                         ProgressStatus.Completed, 100, 100);


            WriteResult(new {success = true});
        }
    }
}