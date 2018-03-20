using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for horizontal line
    /// </summary>
    [Serializable]
    public class HorizontalLine : AppearanceData
    {
        /// <summary>
        /// Horizontal line appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "HORIZONTAL_LINE"; }
        }
    }
}
