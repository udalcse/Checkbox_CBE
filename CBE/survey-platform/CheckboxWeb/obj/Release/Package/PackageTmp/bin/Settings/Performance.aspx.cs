using System;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    public partial class Performance : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            if (!Page.IsPostBack)
            {
                _cacheResponseTemplates.Checked = ApplicationManager.AppSettings.CacheResponseTemplates;
                _persistViewStateToDb.Checked = ApplicationManager.AppSettings.PersistViewStateToDb;
                _allowExclusionaryAclEntries.Checked = ApplicationManager.AppSettings.AllowExclusionaryAclEntries;
                _groupCacheSize.Text = ApplicationManager.AppSettings.GroupCacheSize.ToString();

                _bufferResponseExport.Checked = ApplicationManager.AppSettings.BufferResponseExport;
                _maxReportPreviewOptions.Text = ApplicationManager.AppSettings.MaxReportPreviewOptions.ToString();
                _responseDataExportChunckSize.Text = ApplicationManager.AppSettings.ResponseDataExportChunkSize.ToString();

                _disableAdUserList.Checked = ApplicationManager.AppSettings.DisableUserListForAD;

                _messageThrottle.Text = ApplicationManager.AppSettings.MessageThrottle.ToString();
                _messageThrottleDelay.Text = ApplicationManager.AppSettings.MessageThrottleDelay.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();
            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();

            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");

            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/performance.aspx/title");

            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);

            Master.SetTitleControl(titleControl);

            Master.OkClick += new EventHandler(Master_OkClick);
        }

        protected void Master_OkClick(object sender, EventArgs e)
        {
            if (Page.IsValid & ValidateInputs())
            {
                ApplicationManager.AppSettings.CacheResponseTemplates = _cacheResponseTemplates.Checked;
                ApplicationManager.AppSettings.PersistViewStateToDb = _persistViewStateToDb.Checked;
                ApplicationManager.AppSettings.AllowExclusionaryAclEntries = _allowExclusionaryAclEntries.Checked;
                ApplicationManager.AppSettings.GroupCacheSize = Int32.Parse(_groupCacheSize.Text);

                ApplicationManager.AppSettings.BufferResponseExport = _bufferResponseExport.Checked;
                ApplicationManager.AppSettings.MaxReportPreviewOptions = Int32.Parse(_maxReportPreviewOptions.Text);
                ApplicationManager.AppSettings.ResponseDataExportChunkSize = Int32.Parse(_responseDataExportChunckSize.Text);

                ApplicationManager.AppSettings.DisableUserListForAD = _disableAdUserList.Checked;

                ApplicationManager.AppSettings.MessageThrottle = Int32.Parse(_messageThrottle.Text);
                ApplicationManager.AppSettings.MessageThrottleDelay = Int32.Parse(_messageThrottleDelay.Text);

                Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/updateSuccessful"), StatusMessageType.Success);
            }
        }

        private bool ValidateInputs()
        {
            _groupCacheSizeRequiredValidator.Visible = false;
            _groupCacheSizeFormatValidator.Visible = false;
            _maxReportPreviewOptionsRequiredValidator.Visible = false;
            _maxReportPreviewOptionsFormatValidator.Visible = false;
            _responseDataExportChunckSizeRequiredValidator.Visible = false;
            _responseDataExportChunckSizeFormatValidator.Visible = false;
            _messageThrottleRequiredValidator.Visible = false;
            _messageThrottleFormatValidator.Visible = false;
            _messageThrottleRequiredValidator.Visible = false;
            _messageThrottleFormatValidator.Visible = false;

            bool isValid = true;
            int value;

            if (Utilities.IsNullOrEmpty(_groupCacheSize.Text))
            {
                _groupCacheSizeRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_groupCacheSize.Text, out value))
            {
                _groupCacheSizeFormatValidator.Visible = true;
                isValid = false;
            }

            if (Utilities.IsNullOrEmpty(_maxReportPreviewOptions.Text))
            {
                _maxReportPreviewOptionsRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_maxReportPreviewOptions.Text, out value))
            {
                _maxReportPreviewOptionsFormatValidator.Visible = true;
                isValid = false;
            }

            if (Utilities.IsNullOrEmpty(_responseDataExportChunckSize.Text))
            {
                _responseDataExportChunckSizeRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_responseDataExportChunckSize.Text, out value))
            {
                _responseDataExportChunckSizeFormatValidator.Visible = true;
                isValid = false;
            }

            if (Utilities.IsNullOrEmpty(_messageThrottle.Text))
            {
                _messageThrottleRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_messageThrottle.Text, out value))
            {
                _messageThrottleFormatValidator.Visible = true;
                isValid = false;
            }

            if (Utilities.IsNullOrEmpty(_messageThrottleDelay.Text))
            {
                _messageThrottleRequiredValidator.Visible = true;
                isValid = false;
            }
            else if (!Int32.TryParse(_messageThrottleDelay.Text, out value))
            {
                _messageThrottleFormatValidator.Visible = true;
                isValid = false;
            }

            return isValid;
        }
    }
}
