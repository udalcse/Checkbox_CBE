using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Renderer for vertical rating scales
    /// </summary>
    public partial class RadioButtonScale_Vertical : RadioButtonScaleBase
    {
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
        /// Dictionary of inputs.
        /// </summary>
        protected Dictionary<int, RadioButton> InputDictionary { get; set; }

        /// <summary>
        /// Height of options
        /// </summary>
        protected Unit? OptionHeight { get; set; }

        /// <summary>
        /// Height of middle text element
        /// </summary>
        protected Unit? MidTextHeight { get; set; }

        /// <summary>
        /// Get/set container height
        /// </summary>
        protected Unit? ContainerHeight { get; set; }

        /// <summary>
        /// Selected option id
        /// </summary>
        protected int? SelectedOptionId { get; set; }

        /// <summary>
        /// Specify whether selected option is the na option
        /// </summary>
        protected bool NaOptionSelected { get; set; }

        /// <summary>
        /// Determine  if NA option enabled.
        /// </summary>
        /// <returns></returns>
        protected bool IsNotApplicableOptionEnabled()
        {
            //N/A is enabled if the item contains an "other" option.
            return Model.Options.FirstOrDefault(option => option.IsOther) != null;
        }

        /// <summary>
        /// Get height of inputs
        /// </summary>
        /// <returns></returns>
        protected Unit GetTextHeight(string position)
        {
            if ("middle".Equals(position, StringComparison.InvariantCultureIgnoreCase))
            {
                return MidTextHeight.Value;
            }

            return OptionHeight.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool VerticalSeparator
        {
            get { return "Yes".Equals(Appearance["ShowSeparator"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Calcule late heights of various elements
        /// </summary>
        protected void CalculateHeights()
        {
            //Choose an arbitrary value for pixels of height per input
            const int pixelsPerInput = 25;

            int optionCount = Model.Options.Length;

            OptionHeight = Unit.Pixel(pixelsPerInput);
            ContainerHeight = Unit.Pixel(optionCount * pixelsPerInput);

            //Figure out height of middle text item, which will be
            // container height - 2 * option height since start and end
            // text have height = option height.  Since N/A text appears
            // below end text, factor that into calculation
            int textContainerHeight = IsNotApplicableOptionEnabled()
                ? (optionCount - 1) * pixelsPerInput
                : optionCount * pixelsPerInput;

            MidTextHeight = Unit.Pixel(textContainerHeight - (2 * pixelsPerInput));
        }

        /// <summary>
        /// Perform some calculations when option repeater is bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionRepeater_DataBinding(object sender, EventArgs e)
        {
            //Locate selected options
            SurveyResponseItemOption selectedOption = GetAllOptions().FirstOrDefault(opt => opt.IsSelected);

            SelectedOptionId = (selectedOption != null)
                ? selectedOption.OptionId
                : (int?)null;

            NaOptionSelected = (selectedOption != null)
                ? selectedOption.IsOther
                : false;

            //Calculate various heights
            CalculateHeights();

            //Initialize radio buttons collection
            InputDictionary = new Dictionary<int, RadioButton>();

            var naOption = GetAllOptions().FirstOrDefault(opt => opt.IsOther);
            if (naOption != null)
            {
                InputDictionary[naOption.OptionId] = _naInputRad;
            }
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
            /*foreach (int optionId in InputDictionary.Keys)
            {
                if (InputDictionary[optionId].Checked)
                    return optionId;
            }*/

            return null;
        }

    }
}