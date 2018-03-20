using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Prezza.Framework.Common;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Text decorator for email items
    /// </summary>
    [Serializable()]
    public class EmailResponseItemTextDecorator : EmailItemTextDecorator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public EmailResponseItemTextDecorator(EmailResponseItemData data, string language)
            : base(data, language)
        {
        }

        /// <summary>
        /// Get the item data
        /// </summary>
        new public EmailResponseItemData Data
        {
            get { return (EmailResponseItemData)base.Data; }
        }
    }
}