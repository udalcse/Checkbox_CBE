using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScore : RatingScale
    {
        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            StartText = GetText("/controlText/netPromoterScore/startText");
            EndText = GetText("/controlText/netPromoterScore/endText");
        }
    }
}
