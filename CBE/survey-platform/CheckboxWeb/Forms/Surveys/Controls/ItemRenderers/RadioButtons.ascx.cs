using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class RadioButtons : RadioButtonsControlBase
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

        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                _inputPanel.Attributes.Add("bindedfield", "true");
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool HorizontalOriented
        {
            get
            {
                if (ExportMode == ExportMode.Pdf)
                {
                    var typeInfo = ItemConfigurationManager.GetItemTypeInfo(Model.ItemId);
                    if (typeInfo != null && typeInfo.TypeName == "DropdownList")
                        return false;
                }

                return "Horizontal".Equals(Appearance["Layout"], StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Get column numbers for item
        /// </summary>
        protected int NumberOfColumns
        {
            get
            {
                int columns;
                return int.TryParse(Appearance["Columns"], out columns) ? columns : 0;
            }
        }

        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected override void SetLabelPosition()
        {
            //When label is set to bottom, we need to move controls from the top panel
            // to the bottom panel.  Otherwise, position changes are managed by setting
            // CSS class.
            if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                //Move text controls to bottom
                _bottomAndOrRightPanel.Controls.Add(_textContainer);

                //Move input to top
                _topAndOrLeftPanel.Controls.Add(_inputPanel);
            }

            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected override void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");
        }

    }
}