using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class NetPromoterScore : UserControlSurveyItemRendererBase
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
        /// Flag used to ensure horizontal buttons are not double-bound
        /// </summary>
        protected bool HorizontalButtonsDataBound { get; set; }

        /// <summary>
        /// Flag used to ensure vertical buttons are not double-bound
        /// </summary>
        protected bool VerticalButtonsDataBound { get; set; }

        /// <summary>
        /// Initialize controls that are children of this one
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        /// <param name="exportMode"></param>
        protected override void InitializeChildUserControls(IItemProxyObject dataTransferObject, int? itemNumber, ExportMode exportMode)
        {
            base.InitializeChildUserControls(dataTransferObject, itemNumber, exportMode);

            _verticalButtons.Initialize(Model);
        }

        /// <summary>
        /// Initialize child user controls to set repeat columns and other properties
        /// </summary>
        protected override void InlineInitialize()
        {
            SetLabelPosition();
            SetItemPosition();
            SetInputVisibility();
        }

        /// <summary>
        /// Ensure proper vertical or horizontal scale is shown.
        /// </summary>
        protected void SetInputVisibility()
        {
        }

        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected void SetLabelPosition()
        {
            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer labelTop";
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabelTop";
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPositionLeft";
        }

        /// <summary>
        /// Set flag indicating horizontal buttons data bound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void HorizontalButtons_DataBound(object sender, EventArgs e)
        {
            HorizontalButtonsDataBound = true;
        }

        /// <summary>
        /// Set flag indicating vertical buttons data bound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VerticalButtons_DataBound(object sender, EventArgs e)
        {
            VerticalButtonsDataBound = true;
        }

        /// <summary>
        /// Update validation messages, if any
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            if (_verticalButtons.Visible
                && !VerticalButtonsDataBound)
            {
                _verticalButtons.DataBind();
            }
        }

        /// <summary>
        /// Set selected value
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            var selectedOptionIds = new List<int>();

            int? selectedOptionId = _verticalButtons.GetSelectedOptionId();

            if (selectedOptionId.HasValue)
            {
                selectedOptionIds.Add(selectedOptionId.Value);
            }

            UpsertOptionAnswers(selectedOptionIds, null);
        }
    }
}