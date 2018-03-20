using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;


namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Single line text behavior editor control
    /// </summary>
    public partial class AddressVerifierBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Update control visibility on load.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            //Update text box parameters and place holder visibility
            UpdateDynamicControls();
        }

        /// <summary>
        /// Update controls
        /// </summary>
        private void UpdateDynamicControls()
        {
        }

        /// <summary>
        /// Initialize control with data to edit.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        /// <param name="isOnlyNumericFormat"></param>
        public void Initialize(TextItemDecorator textItemDecorator, bool isOnlyNumericFormat)
        {
            if (!(textItemDecorator.Data is AddressVerifierItemData))
            {
                return;
            }

            //Set search type
            for (int i = 0; i < _searchType.Items.Count; i++)
            {
                if (_searchType.Items[i].Value.Equals((textItemDecorator.Data as AddressVerifierItemData).SearchType))
                {
                    _searchType.SelectedIndex = i;
                    break;
                }
            }

            //Set rule
            for (int i = 0; i < _rule.Items.Count; i++)
            {
                if (_rule.Items[i].Value.Equals((textItemDecorator.Data as AddressVerifierItemData).Rule))
                {
                    _rule.SelectedIndex = i;
                    break;
                }
            }

            //Set rural
            for (int i = 0; i < _rural.Items.Count; i++)
            {
                if (_rural.Items[i].Value.Equals((textItemDecorator.Data as AddressVerifierItemData).Rural))
                {
                    _rural.SelectedIndex = i;
                    break;
                }
            }

            //Set initial states for region
            if (string.IsNullOrEmpty((textItemDecorator.Data as AddressVerifierItemData).Region))
            {
                _regionList.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < _regionList.Items.Count; i++)
                {
                    if (_regionList.Items[i].Value.Equals((textItemDecorator.Data as AddressVerifierItemData).Region))
                    {
                        _regionList.SelectedIndex = i;
                        break;
                    }
                }
            }

            //Required
            _requiredChk.Checked = textItemDecorator.Data.IsRequired;

            //alias
            _aliasText.Text = textItemDecorator.Data.Alias;

            //Default value
            _defaultTextTxt.Text = textItemDecorator.DefaultText;
        }

        /// <summary>
        /// Update data with user inputs.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        public void UpdateData(TextItemDecorator textItemDecorator)
        {
            if (!(textItemDecorator.Data is AddressVerifierItemData))
            {
                return;
            }

            (textItemDecorator.Data as AddressVerifierItemData).Region = _regionList.SelectedValue;
            (textItemDecorator.Data as AddressVerifierItemData).SearchType = _searchType.SelectedValue;
            (textItemDecorator.Data as AddressVerifierItemData).Rule = _rule.SelectedValue;
            (textItemDecorator.Data as AddressVerifierItemData).Rural = _rural.SelectedValue;
            textItemDecorator.Data.Alias = _aliasText.Text;

            //Set format and text, which are used in all formats
            textItemDecorator.DefaultText = _defaultTextTxt.Text;
            textItemDecorator.Data.IsRequired = _requiredChk.Checked;
            //textItemDecorator.Data.MaxLength = Utilities.AsInt(_maxLengthTxt.Text);
        }
    }
}