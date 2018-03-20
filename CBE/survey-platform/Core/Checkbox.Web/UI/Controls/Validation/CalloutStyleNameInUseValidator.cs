using System;
using Checkbox.Styles;
using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.Validation
{
    /// <summary>
    /// Valiator for validating style template names
    /// </summary>
    public class CalloutStyleNameInUseValidator : WarningCalloutValidator
    {
        /// <summary>
        /// Get/set the ID of the "Current" style template so a name error won't occur in cases
        /// where the name is not being changed.
        /// </summary>
        public int? StyleTemplateID { get; set; }

        ///<summary>
        ///</summary>
        public bool IsChartStyle { get; set; }

        /// <summary>
        /// Validate the input
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateInput()
        {
            string value = GetControlValidationValue(ControlToValidate);

            if (value != null)
            {
                if (IsChartStyle)
                {
                    if (StyleTemplateID.HasValue)
                    {
                        var style = ChartStyleManager.GetChartStyle(StyleTemplateID.Value);

                        if (style != null && style.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                        
                        return !ChartStyleManager.IsStyleNameInUse(value.Trim());
                    }
                    
                    return !ChartStyleManager.IsStyleNameInUse(value.Trim());
                }
                
                if (StyleTemplateID.HasValue)
                {
                    StyleTemplate template = StyleTemplateManager.GetStyleTemplate(StyleTemplateID.Value);

                    if (template != null && template.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                    
                    return !StyleTemplateManager.IsStyleNameInUse(value.Trim());
                }
                
                return !StyleTemplateManager.IsStyleNameInUse(value.Trim());
            }

            return true;
        }
    }
}
