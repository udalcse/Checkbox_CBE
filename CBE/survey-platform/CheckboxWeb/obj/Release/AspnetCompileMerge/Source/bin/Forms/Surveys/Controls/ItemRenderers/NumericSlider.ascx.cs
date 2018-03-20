using System;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class NumericSlider : SliderBase
    {
        /// <summary>
        /// Get current value
        /// </summary>
        public String CurrentValue
        {
            get
            {
                string value = Request[_currentValue.UniqueID];

                return string.IsNullOrEmpty(value) ? _currentValue.Text : value;
            }
        }
    }
}