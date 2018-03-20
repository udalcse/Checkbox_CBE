using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;
using Checkbox.Forms;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Analytics.UI.Rendering;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using System.Text;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class MatrixSummary : UserControlAnalysisItemEditorBase
    {
        /// <summary>
        /// Editor supports embedded editors
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }

        /// <summary>
        /// Preview placeholder
        /// </summary>
        protected override Control PreviewContainer { get { return _previewPlace; } }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay { get { return _ruleDisplay; } }

        /// <summary>
        /// Get filter selector control
        /// </summary>
        public override IFilterSelector FilterSelector { get { return _filterSelector; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Initialize the control
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

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            if (ItemData is MatrixSummaryItemData)
            {
                int? sourceResponseTemplateId = GetSourceResponseTemplateId();

                if (sourceResponseTemplateId.HasValue)
                {
                    initializeMatrixSelector(sourceResponseTemplateId.Value, ((MatrixSummaryItemData)ItemData).MatrixSourceItem);
                }
                _options.Initialize((MatrixSummaryItemData)ItemData);
            }
        }

        /// <summary>
        /// Fills the dropdown of matrixes with values and sets the selected one
        /// </summary>
        /// <param name="responseTemplateID"></param>
        /// <param name="selectedItemId"></param>
        private void initializeMatrixSelector(int responseTemplateID, int? selectedItemId)
        {
            List<System.Web.UI.WebControls.ListItem> itemList = ItemConfigurationManager.ListResponseTemplateItems(
                responseTemplateID,
                null,
                false,
                true,
                true,
                WebTextManager.GetUserLanguage()).Where(data => data.ItemType == "Matrix").
                Select(item => new System.Web.UI.WebControls.ListItem() { Text = item.PositionText + " " + GetItemText(item), Value = item.ItemId.ToString() }).ToList();

     //       itemList.Insert(0, new System.Web.UI.WebControls.ListItem() { Text = WebTextManager.GetText("/controlText/matrixSummaryItemEditor/noneSelected"), Value = "" });
            _selectedMatrix.Items.AddRange(itemList.ToArray());

            //sets selected item
            if (selectedItemId != null)
            {
                for (int i = 0; i < _selectedMatrix.Items.Count; i++)
                {
                    if (_selectedMatrix.Items[i].Value.Equals(selectedItemId.ToString())) 
                    {
                        _selectedMatrix.SelectedIndex = i;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemMetaData"></param>
        /// <returns></returns>
        protected string GetItemNumber(LightweightItemMetaData itemMetaData)
        {
            //If item is contained in another item, such as a matrix
            // question.
            if (itemMetaData.Coordinate != null)
            {
                return string.Format(
                    "{0}.{1}.{2}.{3}",
                    itemMetaData.PagePosition,
                    itemMetaData.ItemPosition,
                    itemMetaData.Coordinate.Y,      //Row
                    itemMetaData.Coordinate.X);     //Column
            }

            return string.Format(
                "{0}.{1}",
                itemMetaData.PagePosition,
                itemMetaData.ItemPosition);
        }

        /// <summary>
        /// Get text of item
        /// </summary>
        /// <param name="itemMetaData"></param>
        /// <returns></returns>
        protected string GetItemText(LightweightItemMetaData itemMetaData)
        {
            //var itemText = new StringBuilder();
            //string LanguageCode = WebTextManager.GetUserLanguage();

            ////Append matrix row text/alias if necessary
            //if (!string.IsNullOrEmpty(itemMetaData.GetMatrixRowText(LanguageCode)))
            //{
            //    itemText.Append(itemMetaData.GetMatrixRowText(LanguageCode));
            //    itemText.Append(" - ");
            //}
            //else if (!string.IsNullOrEmpty(itemMetaData.MatrixRowAlias))
            //{
            //    itemText.Append(itemMetaData.MatrixRowAlias);
            //    itemText.Append(" - ");
            //}

            //itemText.Append(itemMetaData.GetText(false, LanguageCode));

            //return Utilities.StripHtml(itemText.ToString(), 55);

            return Utilities.StripHtml(itemMetaData.GetText(false, WebTextManager.GetUserLanguage()), 55);
        }


        /// <summary>
        /// Update item data with configured values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (ItemData is MatrixSummaryItemData)
            {
                if (!String.IsNullOrEmpty(_selectedMatrix.SelectedValue))
                {
                    ((MatrixSummaryItemData)ItemData).MatrixSourceItem = int.Parse(_selectedMatrix.SelectedValue);
                }
                else
                {
                    ((MatrixSummaryItemData)ItemData).MatrixSourceItem = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            _currentTabIndex.Text = HidePreview ? "1" : "0";
            if (ItemData is MatrixSummaryItemData)
            {
                _options.UpdateItemData((MatrixSummaryItemData)ItemData);
            }

            return base.SaveData();
        }
    }
}