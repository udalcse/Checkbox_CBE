using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Localized checkbox control
    /// </summary>
    public class LocalizedLabelledCheckBox : LocalizedLabelledControl
    {
        private CheckBox _checkBox;

        /// <summary>
        /// Get the input checkbox
        /// </summary>
        /// <returns></returns>
        protected override Control GetInputControl()
        {
            return CheckBox;
        }

        /// <summary>
        /// Get the checkbox
        /// </summary>
        protected CheckBox CheckBox
        {
            get
            {
                if (_checkBox == null)
                {
                    _checkBox = new CheckBox();
                }
                return _checkBox;
            }
        }

        /// <summary>
        /// Get/set check state
        /// </summary>
        public bool Checked
        {
            get { return CheckBox.Checked; }
            set { CheckBox.Checked = value; }
        }

        /// <summary>
        /// Get/set the input control
        /// </summary>
        public override Control InputControl
        {
            get { return CheckBox; }
            set { }
        }
        
        /// <summary>
        /// Get/set checkbox css class
        /// </summary>
        public string CheckBoxCssClass
        {
            get { return CheckBox.CssClass; }
            set { CheckBox.CssClass = value; }
        }
    }
}
