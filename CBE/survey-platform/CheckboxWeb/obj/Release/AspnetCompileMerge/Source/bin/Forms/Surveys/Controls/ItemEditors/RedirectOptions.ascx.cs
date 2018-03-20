using System;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Control for configuring hyperlink/redirect item options.
    /// </summary>
    public partial class RedirectOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Bind handler for index changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _linkTypeList.SelectedIndexChanged += _linkTypeList_SelectedIndexChanged;
        }

        /// <summary>
        /// Enable/disable link url/text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _linkTypeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabledControls();
        }

        /// <summary>
        /// Initialize redirect item options editor.
        /// </summary>
        /// <param name="decorator"></param>
        /// <param name="isPagePostback"></param>
        public void Initialize(RedirectItemTextDecorator decorator, bool isPagePostback, string currentLanguage, int templateId, int? pagePosition, EditMode editMode)
        {
            //Initialize pipe selectors
            switch (editMode)
            {
                case EditMode.Survey:
                    _pipeSelectorURL.Initialize(templateId, pagePosition, currentLanguage, _urlTxt.ClientID);
                    _pipeSelectorLink.Initialize(templateId, pagePosition, currentLanguage, _linkTextTxt.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelectorURL.Initialize(null, null, currentLanguage, _urlTxt.ClientID);
                    _pipeSelectorLink.Initialize(null, null, currentLanguage, _linkTextTxt.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelectorURL.Visible = false;
                    _pipeSelectorLink.Visible = false;
                    break;
            }

            if (!isPagePostback)
            {
                _linkTextTxt.Text = decorator.LinkText;
                _urlTxt.Text = decorator.Data.URL;

                if (decorator.Data.RedirectAutomatically
                    && decorator.Data.RestartSurvey)
                {
                    _linkTypeList.SelectedValue = "AutomaticallyRestart";
                }
                else if (decorator.Data.RedirectAutomatically
                    && !decorator.Data.RestartSurvey)
                {
                    _linkTypeList.SelectedValue = "RedirectToUrl";
                }
                else if (!decorator.Data.RedirectAutomatically
                    && decorator.Data.RestartSurvey)
                {
                    _linkTypeList.SelectedValue = "LinkToRestart";
                }
                else
                {
                    _linkTypeList.SelectedValue = "LinkToUrl";
                }

                _linkTargetList.SelectedValue = decorator.Data.OpenInNewWindow ? "NewWindow" : "SameWindow";

                if (decorator.Data.AutoRedirectDelayTime != null)
                    _delayTimeTxt.Text = decorator.Data.AutoRedirectDelayTime.ToString();

                UpdateEnabledControls();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateEnabledControls()
        {
            _linkUrlPanel.Enabled =
                "LinkToUrl".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                || "RedirectToUrl".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            _linkTextPanel.Enabled =
                "LinkToRestart".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                || "LinkToUrl".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            _delayTimePnl.Visible = "RedirectToUrl".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                || "AutomaticallyRestart".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Configure redirect item with current user inputs.
        /// </summary>
        /// <param name="decorator"></param>
        public void UpdateData(RedirectItemTextDecorator decorator)
        {
            decorator.LinkText = _linkTextTxt.Text.Trim();
            decorator.Data.URL = _urlTxt.Text.Trim();

            decorator.Data.RedirectAutomatically =
                "AutomaticallyRestart".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                || "RedirectToUrl".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            decorator.Data.RestartSurvey =
                "AutomaticallyRestart".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                || "LinkToRestart".Equals(_linkTypeList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            decorator.Data.OpenInNewWindow = 
                "NewWindow".Equals(_linkTargetList.SelectedValue, StringComparison.InvariantCultureIgnoreCase);

            int delayTime;
            if(int.TryParse(_delayTimeTxt.Text, out delayTime))
                decorator.Data.AutoRedirectDelayTime = delayTime;
        }
    }
}