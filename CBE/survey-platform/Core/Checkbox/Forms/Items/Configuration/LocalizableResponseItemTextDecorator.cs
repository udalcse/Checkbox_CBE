using System;
using System.Data;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Base text decorator for survey items that support localized text values.
    /// </summary>
    [Serializable]
    public class LocalizableResponseItemTextDecorator : ItemTextDecorator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="languageCode"></param>
        public LocalizableResponseItemTextDecorator(ItemData data, string languageCode)
            : base(data, languageCode)
        {
        }
    }
}
