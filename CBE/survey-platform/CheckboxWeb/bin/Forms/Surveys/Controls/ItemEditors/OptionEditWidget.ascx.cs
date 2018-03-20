using System;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Widget for editing an option edit.
    /// </summary>
    public partial class OptionEditWidget : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/
        /// </summary>
        public bool AllowMultiDefaultSelect { get; private set; }

        /// <summary>
        /// Set initial empty texts
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            //_optionText.EmptyMessage = WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionEditWidget/enterChoiceText");
            //_optionAlias.EmptyMessage = WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/enterAlias");

            this.DataBind();
        }


        /// <summary>
        /// Simple widget for editing list options
        /// </summary>
        /// <param name="optionData"></param>
        /// <param name="optionText"></param>
        /// <param name="allowMultiSelect"></param>
        /// <param name="responseTemplate"></param>
        /// <param name="pagePosition"></param>
        public void Initialize(ListOptionData optionData, string optionText, bool allowMultiSelect, ResponseTemplate responseTemplate, int pagePosition)
        {
            _optionText.Text = optionText;
            _selectedCheck.Checked = optionData.IsDefault;
            _selectedRadio.Checked = optionData.IsDefault;
            _optionAlias.Text = optionData.Alias;
            //_optionPoint.EmptyMessage =
                //WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionEditWidget/points");
            _optionPoint.Text = optionData.Points == 0 ? "" : optionData.Points.ToString("r");

            bool enableScoring = true;
            if (responseTemplate != null)
            {
                enableScoring = responseTemplate.BehaviorSettings.EnableScoring;
            }
            _optionPoint.Visible = enableScoring;

            _deleteBtn.CommandArgument = optionData.OptionID.ToString();
            AllowMultiDefaultSelect = allowMultiSelect;

            //Comment out for now until I get a chance to make piping client side
            //_pipeSelector.Initialize(responseTemplate, pagePosition, _optionText);
            _selectedCheck.Visible = allowMultiSelect;
            _selectedRadio.Visible = !_selectedCheck.Visible;
        }

        /// <summary>
        /// Update option
        /// </summary>
        /// <param name="optionData"></param>
        /// <param name="optionText"></param>
        public void UpdateOption(ListOptionData optionData, out string optionText)
        {
            optionData.IsDefault = AllowMultiDefaultSelect
                ? _selectedCheck.Checked
                : _selectedRadio.Checked;

            optionData.Alias = _optionAlias.Text.Trim();

            optionText = _optionText.Text.Trim();

            optionData.Points = String.IsNullOrEmpty(_optionPoint.Text) ? 0 : Convert.ToDouble(_optionPoint.Text);
        }


        /// <summary>
        /// Validate if Point is double
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            try
            {
                if (!String.IsNullOrEmpty(_optionPoint.Text))
                    Convert.ToDouble(_optionPoint.Text);
                _errorPanel.Visible = _errorLabel.Visible = false;
                return true;
            }
            catch (FormatException)
            {
                _errorPanel.Visible = _errorLabel.Visible = true;
                return false;
            }
        }
    }
}