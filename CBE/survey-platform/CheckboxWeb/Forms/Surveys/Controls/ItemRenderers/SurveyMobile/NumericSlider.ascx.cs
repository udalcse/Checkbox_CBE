using System;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class NumericSlider : SliderBase
    {
        /// <summary>
        /// Get current value
        /// </summary>
        public String CurrentValue
        {
            get { return Request[_currentValue.UniqueID]; }
        }
    }
}