using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for radio button scale.
    /// </summary>
    [Serializable]
    public class RadioButtonScale : RatingScale
    {
        /// <summary>
        /// Get the appearance code for radio button scales
        /// </summary>
        public override string AppearanceCode { get { return "RADIO_BUTTON_SCALE"; } }
    }
}
