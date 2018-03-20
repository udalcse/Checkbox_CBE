using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for matrix radio buttons
    /// </summary>
    [Serializable()]
    public class MatrixRadioButtons : MatrixSelectLayout
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_RADIO_BUTTONS"; }
        }
    }
}
