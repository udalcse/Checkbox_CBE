using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance class for upload item
    /// </summary>
    [Serializable()]
    public class Upload : LabelledItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "FILE_UPLOAD"; }
        }
    }
}
