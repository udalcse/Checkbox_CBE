using System;


namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for email item
    /// </summary>
    [Serializable]
    public class Email : AppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "EMAIL"; }
        }
    }
}
