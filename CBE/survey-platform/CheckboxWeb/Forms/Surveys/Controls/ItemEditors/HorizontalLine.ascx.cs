using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Item editor for horizontal line item.
    /// </summary>
    public partial class HorizontalLine : UserControlItemEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer { get { return _previewPlace; } }

        /// <summary>
        /// 
        /// </summary>
        public override IRuleEditor RuleEditor { get { return _conditionEditor; } }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay { get { return _ruleDisplay; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            RegisterClientScriptInclude(
                "colorPicker.js",
                ResolveUrl("~/Resources/mColorPicker.min.js"));
            
            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = hidePreview ? "1" : "0";

            if (data is HorizontalLineItemData)
            {
                _thicknessTxt.Text = ((HorizontalLineItemData)data).Thickness.HasValue 
                    ? ((HorizontalLineItemData)data).Thickness.ToString() 
                    : string.Empty;

                _widthTxt.Text = ((HorizontalLineItemData)data).LineWidth.HasValue
                    ? ((HorizontalLineItemData)data).LineWidth.ToString()
                    : string.Empty;

                if (_unitList.Items.FindByValue(((HorizontalLineItemData)data).WidthUnit) != null)
                {
                    _unitList.SelectedValue = ((HorizontalLineItemData)data).WidthUnit;
                }

                _selectedColor.Text = ((HorizontalLineItemData) data).Color;
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (ItemData is HorizontalLineItemData)
            {
                ((HorizontalLineItemData) ItemData).Color = _selectedColor.Text;
                ((HorizontalLineItemData)ItemData).WidthUnit = _unitList.SelectedValue;
                ((HorizontalLineItemData)ItemData).Thickness = Utilities.AsInt(_thicknessTxt.Text);
                ((HorizontalLineItemData)ItemData).LineWidth = Utilities.AsInt(_widthTxt.Text);
            }
        }
    }
}