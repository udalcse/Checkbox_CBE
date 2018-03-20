using System;

namespace Checkbox.Forms.Items.Configuration
{
    [Serializable]
    public class CategorizedSelectManyData : SelectManyData, ICategorizedItemData
    {
        public string Category { get; set; }
    }
}
