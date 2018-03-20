using System;
using System.Data;

using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.UI
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class RedirectItemAppearanceData : Message
    {
        /// <summary>
        /// Get the appearance code for the item
        /// </summary>
        public override string AppearanceCode
        {
            get { return "REDIRECT"; }
        }
    }
}
