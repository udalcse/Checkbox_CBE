using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Items.UI
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NetPromoterScore : RatingScale
    {
        /// <summary>
        /// Get the appearance code for net promoter score
        /// </summary>
        public override string AppearanceCode { get { return "NET_PROMOTER_SCORE"; } }
    }
}
