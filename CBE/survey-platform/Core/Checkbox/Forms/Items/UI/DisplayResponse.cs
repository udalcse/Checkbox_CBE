using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for display response item
    /// </summary>
    [Serializable]
    public class DisplayResponse : AppearanceData
    {
        /// <summary>
        /// Display response appearance
        /// </summary>
        public override string AppearanceCode
        {
            get { return "DISPLAY_RESPONSE"; }
        }
    }
}
