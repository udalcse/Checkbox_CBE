using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class DropdownList : DropdownListControlBase
    {
        /// <summary>
        /// Got 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OptionsSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            e.ObjectInstance = this;
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

        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected override void SetLabelPosition()
        {
            //Set css Top
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer labelTop";
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabelTop dropdownList";
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected override void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPositionLeft";
        }

        /// <summary>
        /// Set other text input visibility
        /// </summary>
        protected override void SetOtherVisibility()
        {
            bool allowOther;

            bool.TryParse(Model.Metadata["AllowOther"], out allowOther);

            _otherPlace.Visible = allowOther;
        }

        protected override int? SelectedValue
        {
            get
            {
                string value = Request[_dropdownList.UniqueID] ?? _dropdownList.SelectedValue;
                return Utilities.AsInt(value);
            }
        }

        protected override string OtherText
        {
            get { return Request[_otherTxt.UniqueID] ?? _otherTxt.Text.Trim(); }
        }

        /// <summary>
        /// Drop down list data bound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DropDownList_DataBound(object sender, EventArgs e)
        {
            //Get default option
            SurveyResponseItemOption defaultOption = Model.Options.FirstOrDefault(o => o.IsDefault);

            if (defaultOption != null)
            {
                ListItem listItem = _dropdownList.Items.FindByValue(defaultOption.OptionId.ToString());
                listItem.Attributes["dataDefaultValue"] = "selected";
            }

            //Get selected option
            SurveyResponseItemOption selectedOption = Model.Options.FirstOrDefault(opt => opt.IsSelected);

            //Do nothing if no options selected
            if (selectedOption == null)
            {
                return;
            }

            //Set selected option and "other" text answer
            if (selectedOption.IsOther)
            {
                _otherTxt.Text = Model.InstanceData["OtherText"];
            }

            if (_dropdownList.Items.FindByValue(selectedOption.OptionId.ToString()) != null)
            {
                _dropdownList.SelectedValue = selectedOption.OptionId.ToString();
            }
        }
    }
}