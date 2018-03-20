using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for single line text items contained in a matrix.
    /// </summary>
    [Serializable]
    public class MatrixSingleLineText : SingleLineText
    {
        /// <summary>
        /// Get the appearance code for this appearance type.
        /// </summary>
        public override string AppearanceCode { get { return "MATRIX_SINGLE_LINE_TEXT"; } }
    }
}
