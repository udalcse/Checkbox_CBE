using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data checkboxes
    /// </summary>
    [Serializable()]
    public class Checkboxes : SelectLayout
    {
        /// <summary>
        /// Checkbox appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CHECKBOXES"; }
        }

		/// <summary>
		/// 
		/// </summary>
		public Checkboxes()
		{
			SetPropertyValue("Layout", Layout.Vertical);
		}
    }
}
