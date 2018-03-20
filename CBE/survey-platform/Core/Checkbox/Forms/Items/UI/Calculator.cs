using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Calculator : SelectLayout
    {
        /// <summary>
        /// Get the appearance code for the slider item
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CALCULATOR"; }
        }
    }
}
