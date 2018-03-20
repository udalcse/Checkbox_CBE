using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for matrix checkboxes
    /// </summary>
    [Serializable()]
    public class MatrixCheckboxes : MatrixSelectLayout
    {
        /// <summary>
        /// Get the apppearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_CHECKBOXES"; }
        }
    }
}
