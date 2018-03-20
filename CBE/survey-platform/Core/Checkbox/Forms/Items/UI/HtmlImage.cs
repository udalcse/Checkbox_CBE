using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Image item appearance
    /// </summary>
    [Serializable]
    public class HtmlImage : AppearanceData
    {
        /// <summary>
        /// Get the appearance code for an image
        /// </summary>
        public override string AppearanceCode
        {
            get { return "IMAGE"; }
        }
    }
}
