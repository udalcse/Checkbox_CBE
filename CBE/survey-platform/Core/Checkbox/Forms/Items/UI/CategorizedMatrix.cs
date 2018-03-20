using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for matrix items
    /// </summary>
    [Serializable]
    public class CategorizedMatrix : Matrix
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CATEGORIZED_MATRIX"; }
        }
    }
}