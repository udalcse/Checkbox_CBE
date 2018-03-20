using System;
using System.Data;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.LicenseLibrary;

namespace CheckboxWeb.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ValidationText : TextSettings
    {
        protected override string PageTitleTextId { get { return "/pageText/settings/validationText.aspx/title"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            base.OnPageInit();

            BindLanguageDropDown(_validationPageLanguageList, SurveyLanguages, DefaultLanguage);

            Master.OkClick += Master_OkClick;

            string errorMsg;

            //Check for multiLanguage support.            
            if (ActiveLicense.MultiLanguageLimit.Validate(out errorMsg) != LimitValidationResult.LimitNotReached)
            {
                _multiLanguageNotAllowedWarningPanel.Visible = true;
                _validationPageLanguageList.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            //Change id of repeater to avoid accidental viewstate restoration
            _validationMessageRepeater.ID = "_textRepeater" + _validationPageLanguageList.SelectedValue.Replace("-", string.Empty);
            _validationMessageRepeater.DataSource = GetValidationTextData();
            _validationMessageRepeater.DataBind();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in _validationMessageRepeater.Items)
            {
                TextBox txtInput = item.FindControl("_textValue") as TextBox;

                if (txtInput != null
                    && Utilities.IsNotNullOrEmpty(txtInput.Attributes["TextId"]))
                {
                    TextManager.SetText(txtInput.Attributes["TextId"], _validationPageLanguageList.SelectedValue, txtInput.Text);
                }
            }

            Master.ShowStatusMessage(WebTextManager.GetText("/pageText/settings/surveyText.aspx/textChangesSaved"), StatusMessageType.Success);
        }

        /// <summary>
        /// Get text data for validation messages
        /// </summary>
        /// <returns></returns>
        private DataTable GetValidationTextData()
        {
            return GetTextTable(_validationPageLanguageList.SelectedValue ?? DefaultLanguage, "/validationMessages/");
        }
    }
}
