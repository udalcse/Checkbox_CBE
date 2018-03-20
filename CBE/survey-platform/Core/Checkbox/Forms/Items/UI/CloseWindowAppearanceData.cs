using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for close window item
    /// </summary>
    [Serializable]
    public class CloseWindowAppearanceData : AppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CLOSE_WINDOW"; }
        }
    }
}
