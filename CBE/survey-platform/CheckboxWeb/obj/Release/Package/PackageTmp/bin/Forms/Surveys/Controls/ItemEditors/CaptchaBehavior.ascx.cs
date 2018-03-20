using System;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Common.Captcha;
using Checkbox.Common.Captcha.Image;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Behavior editor for captcha item
    /// </summary>
    public partial class CaptchaBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
              "jquery.numeric.js",
              ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="itemData"></param>
        public void Initialize(CaptchaItemData itemData)
        {
			_aliasText.Text = itemData.Alias;

            if (_codeTypeList.Items.FindByValue(itemData.CodeType.ToString()) != null)
            {
                _codeTypeList.SelectedValue = itemData.CodeType.ToString();
            }

            if (_imageFormatList.Items.FindByValue(itemData.ImageFormat.ToString()) != null)
            {
                _imageFormatList.SelectedValue = itemData.ImageFormat.ToString();
            }

            _enableSoundChk.Checked = itemData.EnableSound;

            _maxCodeLengthTxt.Text = itemData.MaxCodeLength > 0 
                ? itemData.MaxCodeLength.ToString()
                : string.Empty;
            
            _minCodeLengthTxt.Text = itemData.MinCodeLength > 0
                ? itemData.MinCodeLength.ToString()
                : string.Empty;

            _imageHeightTxt.Text = itemData.ImageHeight > 0
                ? itemData.ImageHeight.ToString()
                : string.Empty;

            _imageWidthTxt.Text = itemData.ImageWidth > 0
                ? itemData.ImageWidth.ToString()
                : string.Empty;
        }

        /// <summary>
        /// Update captcha item data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(CaptchaItemData itemData)
        {
			itemData.Alias = _aliasText.Text;
            itemData.ImageFormat = (ImageFormatEnum)Enum.Parse(typeof(ImageFormatEnum), _imageFormatList.SelectedValue);
            itemData.CodeType = (CodeType)Enum.Parse(typeof(CodeType), _codeTypeList.SelectedValue);
            itemData.EnableSound = _enableSoundChk.Checked;

            itemData.MinCodeLength = Utilities.AsInt(_minCodeLengthTxt.Text).Value;
            itemData.MaxCodeLength = Utilities.AsInt(_maxCodeLengthTxt.Text).Value;
            itemData.ImageHeight = Utilities.AsInt(_imageHeightTxt.Text).Value;
            itemData.ImageWidth = Utilities.AsInt(_imageWidthTxt.Text).Value;
        }
    }
}