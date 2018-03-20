using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for matrix slider
    /// </summary>
    [Serializable()]
    public class MatrixSlider : MatrixSelectLayout
    {
        /// <summary>
        /// Get the apppearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_SLIDER"; }
        }
    }
}
