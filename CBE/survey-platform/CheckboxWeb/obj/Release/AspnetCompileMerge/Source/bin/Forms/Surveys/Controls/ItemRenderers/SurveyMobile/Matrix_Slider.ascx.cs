using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class Matrix_Slider : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Get min value of the item
        /// </summary>
        protected int MinValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["MinValue"], out temp) ? temp : 0;
            }
        }

        /// <summary>
        /// Get max value of the item
        /// </summary>
        protected int MaxValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["MaxValue"], out temp) ? temp : 0;
            }
        }

        /// <summary>
        /// Get steo size of the item
        /// </summary>
        protected int StepSize
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["StepSize"], out temp) ? temp : 1;
            }
        }

        /// <summary>
        /// Get DefaultValue of the item. This value is used only in Numeric Slider
        /// </summary>
        protected int DefaultValue
        {
            get
            {
                int temp;
                return int.TryParse(Model.InstanceData["DefaultValue"], out temp) ? temp : (MinValue + MaxValue) / 2;
            }
        }

        /// <summary>
        /// Get value tyoe
        /// </summary>
        protected SliderValueType ValueType
        {
            get
            {
                SliderValueType temp;
                return Enum.TryParse(Model.InstanceData["ValueType"], out temp) ? temp : SliderValueType.NumberRange;
            }
        }

        /// <summary>
        /// Determine if value should be shown or not.
        /// </summary>
        protected bool ShowValue
        {
            get
            {
                bool temp;
                return bool.TryParse(Model.AppearanceData["ShowValue"], out temp) ? temp : false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetSliderWidth()
        {
            if (Width.HasValue)
            {
                if (Width.Value.Type == UnitType.Pixel)
                {
                    return Width.Value.Value + "px";
                }

                return Width.ToString();
            }

            return "100%";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCellClassName()
        {
            var parent = Parent as MatrixChildrensItemRenderer;

            if (parent == null)
            {
                return string.Empty;
            }

            //Only add right border to last option.
            if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
                return "BorderRight";

            //Add top border to all
            if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
                return "BorderTop";

            //If applying "both". 
            if ("Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            {
                return "BorderTop BorderRight";
            }

            return String.Empty;
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