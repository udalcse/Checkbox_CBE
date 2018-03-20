using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for radio buttons.
    /// </summary>
    [Serializable]
    public class RadioButtons : SelectLayout
    {
        /// <summary>
        /// Get the appearance code for this appearance type.
        /// </summary>
        public override string AppearanceCode { get { return "RADIO_BUTTONS"; } }

		/// <summary>
		/// 
		/// </summary>
		public RadioButtons()
		{
			SetPropertyValue("Layout", Layout.Vertical);
		}
    }
}
