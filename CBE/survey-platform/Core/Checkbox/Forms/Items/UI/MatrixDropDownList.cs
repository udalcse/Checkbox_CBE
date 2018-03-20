using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for matrix drop down lists
    /// </summary>
    [Serializable()]
    public class MatrixDropDownList : MatrixSelectLayout
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_DROPDOWN_LIST"; }
        }
    }
}
