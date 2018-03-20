using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Slider : SelectLayout
    {
        /// <summary>
        /// 
        /// </summary>
        public Slider()
        {
            this["ShowValue"] = true.ToString();
            this["Height"] = "200";
        }

        /// <summary>
        /// Get the appearance code for the slider item
        /// </summary>
        public override string AppearanceCode
        {
            get { return "SLIDER"; }
        }
    }
}
