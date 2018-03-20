using System;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for multi-line text items
    /// </summary>
    [Serializable]
    public class MultiLineText : LabelledItemAppearanceData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MULTI_LINE_TEXT"; }
        }
    }
}
