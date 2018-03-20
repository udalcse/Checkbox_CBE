using System;
using System.Web.UI.WebControls;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Settings
{
    public partial class JavascriptItem : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            //Set up the page title with link back to mananger
            var titleControl = new PlaceHolder();

            var managerLink = new HyperLink
            {
                NavigateUrl = "~/Settings/Manage.aspx",
                Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title")
            };

            var pageTitleLabel = new Label { Text = " - " };
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/javascriptItem.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.OkClick += Master_OkClick;

            if (!Page.IsPostBack)
            {
                _enableJavascriptItem.Checked = ApplicationManager.AppSettings.EnableJavascriptItem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            ApplicationManager.AppSettings.EnableJavascriptItem = ApplicationManager.AppSettings.AllowJavascriptItem && _enableJavascriptItem.Checked;

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
        }
    }
}