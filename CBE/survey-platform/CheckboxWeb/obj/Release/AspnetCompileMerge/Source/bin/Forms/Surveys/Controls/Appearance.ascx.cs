using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// User control for editing survey appearance
    /// </summary>
    public partial class Appearance : Checkbox.Web.Common.UserControlBase
    {
        int _rtId;
        protected int SurveyID
        {
            get
            {
                return _rtId;
            }
        }

        protected bool PreviewButtonVisible
        {
            get
            {
                return _styleList.Visible;
            }
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BindStyleList();

            //
            _styleList.Enabled = ApplicationManager.AppSettings.AllowEditSurveyStyleTemplate;
        }

        /// <summary>
        /// Bind style list
        /// </summary>
        private void BindStyleList()
        {
            //Sort by stripped name
            List<LightweightStyleTemplate> styleTemplates = StyleTemplateManager.ListStyleTemplates(UserManager.GetCurrentPrincipal());

            styleTemplates.OrderBy(st => Utilities.StripHtml(st.Name, 64));

            foreach (LightweightStyleTemplate template in styleTemplates)
            {
                _styleList.Items.Add(new ListItem(
                    Utilities.StripHtml(template.Name, 64),
                    template.TemplateId.ToString()));
            }

            //Add "None" Option
            _styleList.Items.Insert(
                0,
                new ListItem(WebTextManager.GetText("/pageText/forms/surveys/import.aspx/none"), "0"));

            _styleList.Visible = styleTemplates.Count > 0;
            _noStylesLbl.Visible = !_styleList.Visible;
        }

        /// <summary>
        /// Initialize control with survey
        /// </summary>
        /// <param name="rt"></param>
        public void Initialize(ResponseTemplate rt)
        {
            //Set selected style template
            if (rt.StyleSettings.StyleTemplateId.HasValue
                && _styleList.Items.FindByValue(rt.StyleSettings.StyleTemplateId.ToString()) != null)
            {
                _styleList.SelectedValue = rt.StyleSettings.StyleTemplateId.ToString();
            }

            //Set check state of style options
            _titleChk.Checked = rt.StyleSettings.ShowTitle;
            _randomizeChk.Checked = rt.BehaviorSettings.RandomizeItemsInPages;
            _progressBarChk.Checked = rt.StyleSettings.ShowProgressBar;
            _pageNumbersChk.Checked = rt.StyleSettings.ShowPageNumbers;
            _itemNumberChk.Checked = rt.StyleSettings.ShowItemNumbers;
            _dynamicItemNumbersChk.Checked = rt.StyleSettings.EnableDynamicItemNumbers;
            _dynamicPageNumbersChk.Checked = rt.StyleSettings.EnableDynamicPageNumbers;
            _alertChk.Checked = rt.StyleSettings.ShowValidationErrorAlert;
            _asterisksChk.Checked = rt.StyleSettings.ShowAsterisks;
            _rtId = rt.ID.Value;
        }

        /// <summary>
        /// Update template with selected options
        /// </summary>
        /// <param name="rt"></param>
        public void Update(ResponseTemplate rt)
        {
            //Update template
            rt.StyleSettings.ShowTitle = _titleChk.Checked;
            rt.BehaviorSettings.RandomizeItemsInPages = _randomizeChk.Checked;
            rt.StyleSettings.ShowProgressBar = _progressBarChk.Checked;
            rt.StyleSettings.ShowPageNumbers = _pageNumbersChk.Checked;
            rt.StyleSettings.ShowItemNumbers = _itemNumberChk.Checked;
            rt.StyleSettings.EnableDynamicItemNumbers = _dynamicItemNumbersChk.Checked;
            rt.StyleSettings.EnableDynamicPageNumbers = _dynamicPageNumbersChk.Checked;
            rt.StyleSettings.ShowValidationErrorAlert = _alertChk.Checked;
            rt.StyleSettings.ShowAsterisks = _asterisksChk.Checked;

            rt.StyleSettings.StyleTemplateId = (_styleList.SelectedValue != null && _styleList.SelectedValue != "0")
                ? Utilities.AsInt(_styleList.SelectedValue)
                : null;
        }
    }
}