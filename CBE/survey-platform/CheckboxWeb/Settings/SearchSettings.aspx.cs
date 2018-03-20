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
    public partial class SearchSettings : SettingsPage
    {
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();
            Master.OkClick += new EventHandler(Master_OkClick);
            if (!Page.IsPostBack)
            {
                _searchAccessibleObjectExpPeriodSeconds.Text = ApplicationManager.AppSettings.SearchAccessibleObjectExpPeriodSeconds.ToString();
                _searchResultsExpPeriodSeconds.Text = ApplicationManager.AppSettings.SearchResultsExpPeriodSeconds.ToString();
                _searchPageSize.Text = ApplicationManager.AppSettings.SearchPageSize.ToString();
                _searchMaxResultRecordsPerObjectType.Text = ApplicationManager.AppSettings.SearchMaxResultRecordsPerObjectType.ToString();
                _searchCachePeriodDays.Text = ApplicationManager.AppSettings.SearchCachePeriodDays.ToString();
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
                ApplicationManager.AppSettings.SearchAccessibleObjectExpPeriodSeconds = Convert.ToInt32(_searchAccessibleObjectExpPeriodSeconds.Text);
                ApplicationManager.AppSettings.SearchResultsExpPeriodSeconds = Convert.ToInt32(_searchResultsExpPeriodSeconds.Text);
                ApplicationManager.AppSettings.SearchPageSize = Convert.ToInt32(_searchPageSize.Text);
                ApplicationManager.AppSettings.SearchMaxResultRecordsPerObjectType = Convert.ToInt32(_searchMaxResultRecordsPerObjectType.Text);
                ApplicationManager.AppSettings.SearchCachePeriodDays = Convert.ToInt32(_searchCachePeriodDays.Text);

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }
    }
}