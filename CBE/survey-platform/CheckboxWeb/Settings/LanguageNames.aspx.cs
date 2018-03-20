using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Management;
using Checkbox.LicenseLibrary;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LanguageNames : TextSettings
    {
        /// <summary>
        /// Page title text id
        /// </summary>
        protected override string PageTitleTextId { get { return "/pageText/settings/languageNames.aspx/title"; } }
        
        /// <summary>
        /// Bind event handlers
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            Master.OkClick += Master_OkClick;

            BindLanguageDropDown(_languageNamesLanguageList, GetLanguageList(), DefaultLanguage);

            string errorMsg;

            //Check for multiLanguage support.            
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                _multiLanguageNotAllowedWarningPanel.Visible = true;
                _languageNamesLanguageList.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            if (!ApplicationManager.AppSettings.AllowMultiLanguage)
            {
                Response.Redirect(UserDefaultRedirectUrl);
                return;
            }

            base.OnPageLoad();

            //Change id to prevent viewstate from carrying over values
            _txtRepeater.ID = "txtRepeater_" + _languageNamesLanguageList.SelectedValue.Replace("-", string.Empty);
            _txtRepeater.DataSource = GetLanguageList();
            _txtRepeater.DataBind();

        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            if (!ApplicationManager.AppSettings.AllowMultiLanguage)
            {
                return;
            }

            foreach (RepeaterItem item in _txtRepeater.Items)
            {
                TextBox txtInput = item.FindControl("_languageText") as TextBox;

                if(txtInput != null
                    && Utilities.IsNotNullOrEmpty(_languageNamesLanguageList.SelectedValue)
                    && Utilities.IsNotNullOrEmpty(txtInput.Attributes["LanguageCode"]))
                {
                    TextManager.SetText(
                        "/languageText/" + txtInput.Attributes["LanguageCode"],
                        _languageNamesLanguageList.SelectedValue,
                        txtInput.Text);
                }
            }

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/languageNames.aspx/textChangesSaved"), StatusMessageType.Success);
        }
    }
}
