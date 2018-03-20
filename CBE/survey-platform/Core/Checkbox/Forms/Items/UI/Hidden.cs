using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for hidden items (ironic...)
    /// </summary>
    [Serializable]
    public class Hidden : AppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "HIDDEN"; }
        }
    }
}
