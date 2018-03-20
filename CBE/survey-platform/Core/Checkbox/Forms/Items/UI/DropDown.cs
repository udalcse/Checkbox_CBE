using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for drop down items
    /// </summary>
    [Serializable]
    public class DropDown : SelectLayout
    {
        /// <summary>
        /// Drop down appearance
        /// </summary>
        public override string AppearanceCode
        {
            get { return "DROPDOWN_LIST";}
        }
    }
}
