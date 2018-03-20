using System;
using System.Data;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance data for message items
    /// </summary>
    [Serializable]
    public class Message : AppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "MESSAGE"; }
        }
    }
}
