using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for html items
    /// </summary>
    [Serializable]
    public class Html : AppearanceData
    {
        /// <summary>
        /// Get the appearance code for an HTML item
        /// </summary>
        public override string AppearanceCode
        {
            get { return "HTML"; }
        }
    }
}
