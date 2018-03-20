using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management.Licensing.Limits;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.LicenseLibrary;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SurveyText : TextSettings
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageTitleTextId { get { return "/pageText/settings/surveyText.aspx/title"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            BindLanguageDropDown(_miscTextLanguageList, SurveyLanguages, DefaultLanguage);

            Master.OkClick += Master_OkClick;

            string errorMsg;

            //Check for multiLanguage support.            
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                _multiLanguageNotAllowedWarningPanel.Visible = true;
                _miscTextLanguageList.Enabled = false;
            }
        }

        /// <summary>
        /// Handle page load
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            BindRepeater();
        }

        /// <summary>
        /// Bind repeater
        /// </summary>
        private void BindRepeater()
        {
            //Change id of repeater to avoid accidental viewstate restoration
            _miscTextRepeater.ID = "_textRepeater" + _miscTextLanguageList.SelectedValue.Replace("-", string.Empty);
            _miscTextRepeater.DataSource = GetMiscTextData();
            _miscTextRepeater.DataBind();
        }

        /// <summary>
        /// Get misc. text data
        /// </summary>
        /// <returns></returns>
        private DataTable GetMiscTextData()
        {
            return GetTextTable(
                _miscTextLanguageList.SelectedValue,
                "/common/dropDownDefault",
                "/pageText/survey.aspx/pageValidationPopup",
                "/pageText/survey.aspx/",
                "/pageText/takeSurvey.aspx/selectLanguage",
                "/pageText/takeSurvey.aspx/continue",
                "/controlText/responseView/");
        }

        /// <summary>
        /// Save text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in _miscTextRepeater.Items)
            {
                TextBox textBox = item.FindControl("_textValue") as TextBox;

                if (textBox != null
                    && Utilities.IsNotNullOrEmpty(textBox.Attributes["TextId"]))
                {
                    TextManager.SetText(textBox.Attributes["TextId"], _miscTextLanguageList.SelectedValue, textBox.Text);
                }
            }

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/surveyText.aspx/textChangesSaved"), StatusMessageType.Success);
        }
    }
}
