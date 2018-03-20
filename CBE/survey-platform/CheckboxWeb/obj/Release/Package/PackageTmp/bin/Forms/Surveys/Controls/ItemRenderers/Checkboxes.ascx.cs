using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    public partial class Checkboxes : CheckboxesControlBase
    {
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
        /// 
        /// </summary>
        protected virtual bool HorizontalOriented
        {
            get { return "Horizontal".Equals(Appearance["Layout"], StringComparison.InvariantCultureIgnoreCase); }
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