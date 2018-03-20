using System;

using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Item implementation for a multi-line text box
    /// </summary>
    [Serializable]
    public class MultiLineTextBoxItem : TextItem
    {
        /// <summary>
        /// Configure this item with the supplied configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            ArgumentValidation.CheckExpectedType(configuration, typeof(MultiLineTextItemData));

            base.Configure(configuration, languageCode, templateId);

            Format = AnswerFormat.None;
        }
    }
}
