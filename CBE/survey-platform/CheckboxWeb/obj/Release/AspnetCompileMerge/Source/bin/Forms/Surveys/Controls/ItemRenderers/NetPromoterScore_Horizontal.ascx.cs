using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class NetPromoterScore_Horizontal : RadioButtonScaleBase
    {
        /// <summary>
        /// Dictionary of inputs.
        /// </summary>
        protected Dictionary<int, RadioButton> InputDictionary { get; set; }

        /// <summary>
        /// Width for individual options
        /// </summary>
        protected Unit? OptionWidth { get; set; }

        /// <summary>
        /// Selected option id
        /// </summary>
        protected int? SelectedOptionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionsSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
        }

        /// <summary>
        /// Calculate width of option header text
        /// </summary>
        /// <returns></returns>
        protected void CalculateContainerWidths()
        {
            //Header text container width needs to be set so that it is large enough
            // to contain all text and span all options except the N/A option (if any).
            //So, the width will be the greater of a fixed value * (number of options) or 
            // another fixed value * (number of characters in largest word in scale text).

            const int pixelsPerOption = 30;
            const int pixelsPerLetter = 15;

            string startText = Utilities.IsNotNullOrEmpty(Model.InstanceData["StartText"]) ? Model.InstanceData["StartText"] : string.Empty;
            string endText = Utilities.IsNotNullOrEmpty(Model.InstanceData["EndText"]) ? Model.InstanceData["EndText"] : string.Empty;

            //Get sum of lengths of longest words for start/mid/end text so that we can
            // ensure word will fit w/out need for wrapping in middle of word.
            int maxWordLengthSum = new[]
                {
                Utilities.IsNotNullOrEmpty(startText) ? startText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(endText) ? endText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1
            }.Sum();

            //Sum lengths

            //Min header text size (in pixels)
            int minHeaderTextSize = maxWordLengthSum * pixelsPerLetter;
            int minOptionSize = GetNonOtherOptions().Count() * pixelsPerOption;

            int containerWidth = Math.Max(minOptionSize, minHeaderTextSize);


            //Divide container width by number of non-other options to set option width,
            // which is necessary in cases where start/mid/end texts require more room
            // than options.
            OptionWidth = Unit.Pixel(containerWidth / Math.Max(GetNonOtherOptions().Count(), 1));

            //Finally, check to see if option width specified in appearance
            int? appearanceOptionWidth = Utilities.AsInt(Appearance["OptionWidth"]);

            if (appearanceOptionWidth.HasValue)
                OptionWidth = Unit.Pixel(appearanceOptionWidth.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetMinContainerWidth()
        {
            EnsureInitialized();

            return ((GetNonOtherOptions().Count() * OptionWidth.Value.Value) + 25) + "px";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        protected string GetHeaderDivText(int itemIndex)
        {
            string returnVal = string.Empty;

            if (itemIndex == 0)
                returnVal = Model.InstanceData["StartText"];
            else if (itemIndex == (GetNonOtherOptions().Count() - 1))
                returnVal = Model.InstanceData["EndText"];

            return string.IsNullOrEmpty(returnVal) ? "&nbsp;" : Utilities.AdvancedHtmlEncode(returnVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        protected bool IsMiddleDiv(int itemIndex)
        {
            var optionCount = GetNonOtherOptions().Count();

            //If odd number, middle div = ((count + 1) / 2) - 1 (add 1 after to account for 0-index)
            if (optionCount % 2 == 1)
            {
                //
                return itemIndex == ((optionCount + 1) / 2) - 1;
            }

            return itemIndex == (optionCount / 2) - 1;
        }


        /// <summary>
        /// Handle repeater binding event to perform calculations, identify selected option, etc.
        /// </summary>
        protected void EnsureInitialized()
        {
            if (InputDictionary != null)
            {
                return;
            }

            //Locate selected options
            SurveyResponseItemOption selectedOption = GetAllOptions().FirstOrDefault(opt => opt.IsSelected);

            SelectedOptionId = (selectedOption != null)
                ? selectedOption.OptionId
                : (int?)null;

            //Calculate various widthds
            CalculateContainerWidths();

            //Initialize radio buttons collection
            InputDictionary = new Dictionary<int, RadioButton>();
        }


        /// <summary>
        /// Handle repeater binding event to perform calculations, identify selected option, etc.
        /// </summary>
        protected void OptionRepeater_DataBinding(object sender, EventArgs e)
        {
            EnsureInitialized();
        }

        /// <summary>
        /// Handle item created event to store a reference to created radio buttons for easy reference.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            SurveyResponseItemOption itemOption = e.Item.DataItem as SurveyResponseItemOption;

            if (e.Item.ItemType == ListItemType.Item
                || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (itemOption == null)
                {
                    return;
                }

                RadioButton radInput = e.Item.FindControl("_inputRad") as RadioButton;

                if (radInput != null)
                {
                    InputDictionary[itemOption.OptionId] = radInput;
                }
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                RadioButton radInput = e.Item.FindControl("_naInputRad") as RadioButton;
                var naOption = GetAllOptions().FirstOrDefault(opt => opt.IsOther);

                if (radInput != null && InputDictionary != null && naOption != null)
                {
                    InputDictionary[naOption.OptionId] = radInput;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetBorderOverride()
        {
            return "Yes".Equals(Appearance["ShowSeparator"], StringComparison.InvariantCultureIgnoreCase)
                ? string.Empty
                : "border-style:none;";
        }

        /// <summary>
        /// Return a boolean indicating if the specified option should be checked.
        /// </summary>
        /// <param name="optionIdAsObject"></param>
        /// <returns></returns>
        protected bool GetOptionChecked(object optionIdAsObject)
        {
            //If no options selected, do nothing and return false
            if (!SelectedOptionId.HasValue)
            {
                return false;
            }

            //Otherwise, convert to option id.
            if (optionIdAsObject == null)
            {
                return false;
            }

            try
            {
                return (int)optionIdAsObject == SelectedOptionId.Value;
            }
            catch
            {
            }

            //Error occurred, return false;
            return false;
        }

        /// <summary>
        /// Get the ID of the selected option
        /// </summary>
        /// <returns></returns>
        public override int? GetSelectedOptionId()
        {
            var selectedValue = Request["ScaleRads_" + Model.ItemId];
            var radio = InputDictionary.FirstOrDefault(i => i.Value.ClientID == selectedValue);
            if (radio.Key != default(int))
                return radio.Key;

            //Return first selected option id.
            /* foreach(int optionId in InputDictionary.Keys)
             {
                 if (InputDictionary[optionId].Checked)
                     return optionId;
             }*/

            return null;
        }

    }
}