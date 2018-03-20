using System;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.UI
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class SingleLineText : LabelledItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code for a SingleLineText
        /// </summary>
        public override string AppearanceCode
        {
            get { return "SINGLE_LINE_TEXT"; }
        }
    }
}
