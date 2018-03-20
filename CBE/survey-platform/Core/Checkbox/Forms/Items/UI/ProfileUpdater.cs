using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.UI
{
    [Serializable()]
    public class ProfileUpdater : LabelledItemAppearanceData
    {
        /// <summary>
        /// Get the appearance code
        /// </summary>
        public override string AppearanceCode
        {
            get { return "PROFILE_UPDATER"; }
        }
    }
}
