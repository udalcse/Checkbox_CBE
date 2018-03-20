using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for matrix checkboxes
    /// </summary>
    [Serializable]
    public class RowSelectAppearanceData : MatrixSelectLayout
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_ROW_SELECT"; }
        }
    }
}