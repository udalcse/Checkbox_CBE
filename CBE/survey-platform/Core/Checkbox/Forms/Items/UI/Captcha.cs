using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance class for captcha item
    /// </summary>
    [Serializable()]
    public class Captcha : LabelledItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "CAPTCHA"; }
        }
    }
}
