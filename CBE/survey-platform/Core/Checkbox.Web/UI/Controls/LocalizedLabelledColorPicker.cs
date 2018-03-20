using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Web.UI.Controls.Validation;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Localized color picker
    /// </summary>
    public class LocalizedLabelledColorPicker : LocalizedLabelledTextBox
    {
        private ColorPicker _colorPicker;
        private CalloutColorValidator _validator;

        /// <summary>
        /// Get the validator
        /// </summary>
        public CalloutColorValidator Validator
        {
            get
            {
                if (_validator == null)
                {
                    _validator = new CalloutColorValidator();
                }

                return _validator;
            }
        }

        /// <summary>
        /// Get the color picker
        /// </summary>
        public ColorPicker ColorPicker
        {
            get
            {
                if (_colorPicker == null)
                {
                    _colorPicker = new ColorPicker();
                }

                return _colorPicker;
            }
        }

        /// <summary>
        /// Get/set validator text id
        /// </summary>
        public string ValidatorTextID
        {
            get { return Validator.TextID; }
            set { Validator.TextID = value; }
        }

        /// <summary>
        /// Get/set validator sub text id
        /// </summary>
        public string ValidatorSubTextID
        {
            get { return Validator.SubTextID; }
            set { Validator.SubTextID = value; }
        }

        /// <summary>
        /// Create the input control
        /// </summary>
        /// <returns></returns>
        protected override Control GetInputControl()
        {
            Panel p = new Panel();
            p.Style["display"] = "inline";

            ColorPicker.ColorPickerField = TextBox;
            ColorPicker.ID = "ColorPicker";

            Panel textBoxPanel = new Panel();
            textBoxPanel.Style["float"] = "left";
            textBoxPanel.Controls.Add(TextBox);
            
            Panel colorPickerPanel = new Panel();
            colorPickerPanel.Style["float"] = "left";
            colorPickerPanel.Controls.Add(ColorPicker);

            p.Controls.Add(textBoxPanel);
            p.Controls.Add(colorPickerPanel);
            p.Controls.Add(Validator);

            Validator.ControlToValidate = TextBox.ID;

            return p;
        }

        /// <summary>
        /// Override enabled to show/hide color picker
        /// </summary>
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                ColorPicker.Visible = value;
                base.Enabled = false;
            }
        }
    }
}
