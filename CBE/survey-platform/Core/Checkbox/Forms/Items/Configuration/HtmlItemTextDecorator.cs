using System;


namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for HTML items.
    /// </summary>
    [Serializable]
    public class HtmlItemTextDecorator : LocalizableResponseItemTextDecorator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public HtmlItemTextDecorator(HtmlItemData data, string language) : base(data, language) {}

        /// <summary>
        /// Get the underlying data object
        /// </summary>
        new public HtmlItemData Data
        {
            get { return (HtmlItemData)base.Data; }
        }
    }
}
