using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Localized text decorator for a Display Analysis
    /// </summary>
    [Serializable]
    public class DisplayAnalysisTextDecorator : RedirectItemTextDecorator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public DisplayAnalysisTextDecorator(DisplayAnalysisItemData data, string language)
            : base(data, language)
        {
        }

        /// <summary>
        /// Get the data for the item
        /// </summary>
        new public DisplayAnalysisItemData Data
        {
            get
            {
                return (DisplayAnalysisItemData)base.Data;
            }
        }
    }
}
