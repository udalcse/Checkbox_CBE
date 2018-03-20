using System;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace Checkbox.Analytics.Filters.Validators
{
    ///<summary>
    ///</summary>
    public class NumericSliderFilterValidator : IFilterValidator
    {
        ///<summary>
        ///</summary>
        ///<param name="value"></param>
        ///<param name="errorMessage"></param>
        ///<param name="itemData"></param>
        ///<returns></returns>
        ///<exception cref="NotImplementedException"></exception>
        public bool Validate(ItemData itemData, string value, out string errorMessage)
        {
            errorMessage = null;

            if (itemData is SliderItemData && (itemData as SliderItemData).ValueType == SliderValueType.NumberRange)
            {
                if (string.IsNullOrEmpty(value))
                {
                    errorMessage =
                        TextManager.GetText(
                            "/pageText/forms/surveys/reports/filters/add.aspx/validationSliderEmpty");
                    return false;
                }

                int numVal;
                if (!int.TryParse(value, out numVal))
                {
                    errorMessage =
                        TextManager.GetText(
                            "/pageText/forms/surveys/reports/filters/add.aspx/validationSliderOnlyNumber");
                    return false;
                }

                if (((itemData as SliderItemData).MinValue.HasValue && numVal < (itemData as SliderItemData).MinValue) ||
                    ((itemData as SliderItemData).MaxValue.HasValue && numVal > (itemData as SliderItemData).MaxValue))
                {
                    errorMessage = TextManager.GetText(
                        "/pageText/forms/surveys/reports/filters/add.aspx/validationSliderMinMaxLimit").Replace("{0}",
                        (itemData as SliderItemData).MinValue.ToString()).Replace("{1}", (itemData as SliderItemData).MaxValue.ToString());

                    return false;
                }

            }

            return true;
        }
    }
}