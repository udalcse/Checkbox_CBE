using System;
using System.Collections.Generic;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
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
            _numericSlider.Initialize(Model);
            _valueListSlider.Initialize(Model);

            if (ValueType == SliderValueType.NumberRange)
                _numericSlider.Visible = true;
            else
                _valueListSlider.Visible = true;
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

        protected string CssClass
        {
            get { return ValueType.ToString(); }
        }
    }
}