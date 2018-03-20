using System;
using System.Collections.Generic;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class Slider : UserControlSurveyItemRendererBase
    {     
        /// <summary>
        /// Get value tyoe
        /// </summary>
        protected SliderValueType ValueType
        {
            get { return (SliderValueType)Enum.Parse(typeof(SliderValueType), Model.InstanceData["ValueType"]); }
        }

        /// <summary>
        /// Get slider value list option type
        /// </summary>
        protected SliderValueListOptionType ValueListOptionType
        {
            get { return (SliderValueListOptionType)Enum.Parse(typeof(SliderValueListOptionType), Model.InstanceData["ValueListOptionType"]); }
        }

        protected override void InlineInitialize()
        {
            switch (ValueType)
            {
                case SliderValueType.NumberRange:
                    _numericSlider.Initialize(Model);
                    _numericSlider.Visible = true;
                    break;
                case SliderValueType.Text:
                case SliderValueType.Image:
                    _valueListSlider.Initialize(Model);
                   _valueListSlider.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override List<UserControlItemRendererBase> ChildUserControls
        {
            get
            {
                var childControls = base.ChildUserControls;
                childControls.Add(_questionText);
                return childControls;
            }
        }

        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            if (ValueType == SliderValueType.NumberRange)
                UpsertTextAnswer(_numericSlider.CurrentValue);
            else
                UpsertOptionAnswers(new List<int> { _valueListSlider.SelectedOptionID }, null);
        }
    }
}