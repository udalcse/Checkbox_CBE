using System.Collections.Generic;
using System.Linq;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class RadioButtons : RadioButtonsControlBase
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
        protected SurveyResponseItemOption OtherOption
        {
            get
            {
                if (!AllowOther)
                    return null;

                return GetAllOptions().FirstOrDefault(o => o.IsOther);
            }
        }

        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                _inputPanel.Attributes.Add("binded-field", "true");
        }


        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected override void SetLabelPosition()
        {
            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer labelTop";
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabelTop";
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected override void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPositionLeft";
        }

    }
}