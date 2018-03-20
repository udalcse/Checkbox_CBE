using System;
using System.Globalization;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SystemPreferences : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            if (!Page.IsPostBack)
            {
                _useHtmlEditor.Checked = ApplicationManager.AppSettings.UseHTMLEditor;
                _useDatePicker.Checked = ApplicationManager.AppSettings.UseDatePicker;
                _storeImagesInDatabase.Checked = ApplicationManager.AppSettings.StoreImagesInDatabase;
                _showNavigationLinksWhenNotLoggedIn.Checked = ApplicationManager.AppSettings.ShowNavWhenNotAuthenticated;
                _showAvailableSurveysList.Checked = ApplicationManager.AppSettings.DisplayAvailableSurveyList;
                _showAvailableReportsList.Checked = ApplicationManager.AppSettings.DisplayAvailableReportList;
                _enableEmailAddressValidation.Checked = ApplicationManager.AppSettings.EnableEmailAddressValidation;
                _displayProductTour.Checked = ApplicationManager.AppSettings.DisplayProductTour;
                _showCreatedByForFolders.Checked = ApplicationManager.AppSettings.ShowCreatedBy;
                _displayMachineName.Checked = ApplicationManager.AppSettings.DisplayMachineName;
                _timeZone.SelectedValue = ApplicationManager.AppSettings.TimeZone.ToString().Replace(',', '.');
                _customerUpdateHeader.Checked = ApplicationManager.AppSettings.CustomerUpdateHeader;
            }

            _storeImagesInDatabase.Visible = !ApplicationManager.AppSettings.EnableMultiDatabase;

            //Set up the page title with link back to mananger
            var titleControl = new PlaceHolder();

            var managerLink = new HyperLink
                                  {
                                      NavigateUrl = "~/Settings/Manage.aspx",
                                      Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title")
                                  };

            var pageTitleLabel = new Label {Text = " - "};
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/systemPreferences.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);
        }


        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Master.OkClick += Master_OkClick; 
        }

        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.UseHTMLEditor = _useHtmlEditor.Checked;
            ApplicationManager.AppSettings.UseDatePicker = _useDatePicker.Checked;
            ApplicationManager.AppSettings.StoreImagesInDatabase = _storeImagesInDatabase.Checked;
            ApplicationManager.AppSettings.ShowNavWhenNotAuthenticated = _showNavigationLinksWhenNotLoggedIn.Checked;
            ApplicationManager.AppSettings.DisplayAvailableSurveyList = _showAvailableSurveysList.Checked;
            ApplicationManager.AppSettings.DisplayAvailableReportList = _showAvailableReportsList.Checked;
            ApplicationManager.AppSettings.EnableEmailAddressValidation = _enableEmailAddressValidation.Checked;
            ApplicationManager.AppSettings.DisplayProductTour = _displayProductTour.Checked;
            ApplicationManager.AppSettings.TimeZone = DoubleParse(_timeZone.SelectedValue);
            ApplicationManager.AppSettings.ShowCreatedBy = _showCreatedByForFolders.Checked;
            ApplicationManager.AppSettings.DisplayMachineName = _displayMachineName.Checked;
            ApplicationManager.AppSettings.CustomerUpdateHeader = _customerUpdateHeader.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }

        /// <summary>
        /// Parse double string regardless of current culture settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double DoubleParse(string value)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            double temp;
            return Double.TryParse(value, NumberStyles.Float, culture, out temp) ? temp : 0;
        }
    }
}
