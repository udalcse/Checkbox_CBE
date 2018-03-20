using System;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// Appearance for address verifier items
    /// </summary>
    [Serializable]
    public class AddressVerifier : LabelledItemAppearanceData
    {
        /// <summary>
        /// Address verifier appearance
        /// </summary>
        public override string AppearanceCode
        {
            get { return "ADDRESS_VERIFIER";}
        }
    }
}
