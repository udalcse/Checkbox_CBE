using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class SliderBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
             "jquery.numeric.js",
             ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize the editor with the data
        /// </summary>
        /// <param name="textItemDecorator"></param>
        /// <param name="matrixSlider"></param>
        public void Initialize(SelectItemTextDecorator textItemDecorator, bool matrixSlider)
        {
            SliderItemData itemData = textItemDecorator.Data as SliderItemData;

            if (itemData == null)
                return;

            //temporaty fix for matrix slider
            if (matrixSlider)
            {
                var image = _valueTypeList.Items.FindByValue("Image");
                _valueTypeList.Items.Remove(image);
            }

            if (_valueTypeList.Items.FindByValue(itemData.ValueType.ToString()) != null)
            {
                _valueTypeList.SelectedValue = itemData.ValueType.ToString();
            }
            else if (_valueTypeList.Items.FindByValue(itemData.ValueListOptionType.ToString()) != null)
            {
                _valueTypeList.SelectedValue = itemData.ValueListOptionType.ToString();
            }

            if (_optionTypeList.Items.FindByValue(itemData.ValueListOptionType.ToString()) != null)
            {
                _optionTypeList.SelectedValue = itemData.ValueListOptionType.ToString();
            }

            //Set panel visibility
            if (_valueTypeList.SelectedValue == "NumberRange")
            {
                _numbersOptionsPanel.Style.Remove("display");
                _valueListOptionsPanel.Style["display"] = "none";
            }
            else
            {
                _valueListOptionsPanel.Style.Remove("display");
                _numbersOptionsPanel.Style["display"] = "none";
            }

            _aliasText.Text = textItemDecorator.Data.Alias;
            
            _minValueTxt.Text = itemData.MinValue.HasValue ? itemData.MinValue.ToString() : string.Empty;
            _maxValueTxt.Text = itemData.MaxValue.HasValue ? itemData.MaxValue.ToString() : string.Empty;
            _stepSizeTxt.Text = itemData.StepSize.HasValue ? itemData.StepSize.ToString() : string.Empty;
            _defaultValueTxt.Text = itemData.DefaultValue.HasValue ? itemData.DefaultValue.ToString() : string.Empty;
        }
       
        /// <summary>
        /// Update data with user inputs
        /// </summary>
        /// <param name="textItemDecorator"></param>
        public void UpdateData(SelectItemTextDecorator textItemDecorator)
        {
            SliderItemData itemData = textItemDecorator.Data as SliderItemData;

            if (itemData == null)
                return;

            itemData.ValueType = (SliderValueType)Enum.Parse(typeof(SliderValueType), _valueTypeList.SelectedValue);
            itemData.ValueListOptionType =
                (SliderValueListOptionType)
                Enum.Parse(typeof(SliderValueListOptionType), _optionTypeList.SelectedValue);

            textItemDecorator.Data.Alias = _aliasText.Text;

            //Set To nulls            
            itemData.MinValue = null;
            itemData.MaxValue = null;
            itemData.StepSize = null;
            itemData.DefaultValue = null;

            int temp;

            //Check for real values
            if (int.TryParse(_minValueTxt.Text, out temp))
                itemData.MinValue = temp;

            if (int.TryParse(_maxValueTxt.Text, out temp))
                itemData.MaxValue = temp;

            if (int.TryParse(_stepSizeTxt.Text, out temp))
                itemData.StepSize = temp;

            if (int.TryParse(_defaultValueTxt.Text, out temp))
                itemData.DefaultValue = temp;

            //Set panel visibility
            if (_valueTypeList.SelectedValue == "NumberRange")
            {
                _numbersOptionsPanel.Style.Remove("display");
                _valueListOptionsPanel.Style["display"] = "none";
            }
            else
            {
                _valueListOptionsPanel.Style.Remove("display");
                _numbersOptionsPanel.Style["display"] = "none";
            }

            //Remove all old options.
            //for (int i = itemData.Options.Count() - 1; i >= 0; i--)
            //    itemData.RemoveOption(itemData.Options[i].OptionID);
        }

        /// <summary>
        /// Validate the behavior of slider item
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            int? min = null, max = null, defaultValue = null;
            int temp;

            if (int.TryParse(_minValueTxt.Text, out temp))
                min = temp;

            if (int.TryParse(_maxValueTxt.Text, out temp))
                max = temp;

            if (int.TryParse(_defaultValueTxt.Text, out temp))
                defaultValue = temp;

            if (min.HasValue && max.HasValue && min.Value > max.Value)
            {
                _errorLbl.Text = TextManager.GetText("/controlText/sliderEditor/minLessOrEqualMaxError");
                _errorPanel.Visible = true;
                return false;
            }

            if ((min.HasValue && defaultValue.HasValue && min.Value > defaultValue.Value) || (max.HasValue && defaultValue.HasValue && max.Value < defaultValue.Value))
            {
                _errorLbl.Text = TextManager.GetText("/controlText/sliderEditor/defaultValueBetweenMinAndMaxError");
                _errorPanel.Visible = true;
                return false;
            }

            _errorPanel.Visible = false;
            return true;
        }
    }
}