using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class ValueListSlider : SliderBase
    {
        /// <summary>
        /// Get selected option ID
        /// </summary>
        public int SelectedOptionID
        {
            get
            {
                var val = Request[_slider.UniqueID] ?? _slider.SelectedValue;
                return string.IsNullOrEmpty(val) ? -1 : int.Parse(val);
            }
        }

        public override void Initialize(SurveyResponseItem radioButtonScaleItem)
        {
            base.Initialize(radioButtonScaleItem);

            _slider.Attributes["data-images"] = (ValueListOptionType == SliderValueListOptionType.Image).ToString().ToLower();
            _slider.Attributes["data-approot"] = ApplicationManager.ApplicationPath;
            _slider.Attributes["data-show-aliases"] = (ValueListOptionType != SliderValueListOptionType.Image || AliasPosition != "dontshow").ToString().ToLower();
        }
    }
}