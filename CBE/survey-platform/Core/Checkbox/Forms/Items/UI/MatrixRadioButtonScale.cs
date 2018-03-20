using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance code specific to radio buttons contained in a matrix question.
    /// </summary>
    [Serializable]
    public class MatrixRadioButtonScale : MatrixRatingScale
    {
        /// <summary>
        /// Get the appearance code for this appearance type.
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MATRIX_RADIO_BUTTON_SCALE";}
        }
    }
}
