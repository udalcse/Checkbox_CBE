using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class TimelineCommonSettings : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();
            Master.OkClick += new EventHandler(Master_OkClick);
            if (!Page.IsPostBack)
            {
                _recordsPerPage.Text = ApplicationManager.AppSettings.TimelineRecordsPerPage.ToString();
                _requestExpiration.Text = ApplicationManager.AppSettings.TimelineRequestExpiration.ToString();
            }
        }

        /// <summary>
        /// Save changes
        /// </summary>
        void Master_OkClick(object sender, EventArgs e)
        {
            //Set the properties
            if (IsValid)
            {
                ApplicationManager.AppSettings.TimelineRecordsPerPage = Convert.ToInt32(_recordsPerPage.Text);
                ApplicationManager.AppSettings.TimelineRequestExpiration = Convert.ToInt32(_requestExpiration.Text);

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }
    }
}