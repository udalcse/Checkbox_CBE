using System;
using System.Linq;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class RadioButtonScale_Vertical : RadioButtonScaleBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected SurveyResponseItemOption OtherOption
        {
            get
            {
                return GetAllOptions().FirstOrDefault(o => o.IsOther);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool VerticalSeparator
        {
            get { return "Yes".Equals(Appearance["ShowSeparator"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Get the ID of the selected option
        /// </summary>
        /// <returns></returns>
        public override int? GetSelectedOptionId()
        {
            var val = Request[UniqueID + "_rating_scale"];

            //Get selected option
            if (val != null)
            {
                int? optionId = Utilities.AsInt(val);
                return optionId;
            }

            return null;
        }

    }
}