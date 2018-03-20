

using System;

namespace Checkbox.Forms.Items.Configuration
{
    [Serializable]
    public class CategorizedSelectOneData : Select1Data, ICategorizedItemData
    {
        public string Category { get; set; }
    }
}
