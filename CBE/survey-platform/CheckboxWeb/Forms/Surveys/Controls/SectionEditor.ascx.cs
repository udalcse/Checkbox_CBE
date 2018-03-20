using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class SectionEditor : Checkbox.Web.Common.UserControlBase
    {
        private int _surveyId;
        private int _sectionId;
        private int _templateId;
        public void Initialize(ItemData data, int templateId, bool isPostBack)
        {
            _reportableSection.Checked = ((MessageItemData)data).ReportableSectionBreak;
            _surveyId = templateId;
            _sectionId = data.ID.Value;
            _templateId = templateId;

            if (!isPostBack)
               BuildDualBoxList(templateId);
        }


        private void BuildDualBoxList(int templateId)
        {
            var template = ResponseTemplateManager.GetResponseTemplate(templateId);

            if (template == null)
            {
                return;
            }

            var itemIds = template.ListTemplateItemIds();
            var allRatingScaleItems =
                itemIds.Select(
                    itemId =>
                        SurveyManagementServiceImplementation.GetSurveyItemMetaData(
                            UserManager.GetCurrentPrincipal(),
                            templateId, itemId)).Where(item => item.TypeName.Equals("RadioButtonScale")).ToList();

            var selectedItems = GetSelectedItems(allRatingScaleItems, _sectionId);
            var avaliableItems = GetAvaliableItems(allRatingScaleItems, templateId);

            _sectionDualBox.SelectionMode = ListSelectionMode.Multiple;

            _sectionDualBox.Items.Clear();

            // fill avaliable item box 
            foreach (var avaliableItem in avaliableItems)
            {
                var text = GetScaleBoxText(avaliableItem);
                _sectionDualBox.Items.Add(new ListItem(text, avaliableItem.ItemId.ToString()));
            }


            // fill selected item box 
            foreach (var currentItemData in selectedItems)
            {
                var text = GetScaleBoxText(currentItemData);

                var item = new ListItem(text, currentItemData.ItemId.ToString())
                {
                    Selected = true
                };

                item.Attributes.Add("selected", "selected");

                _sectionDualBox.Items.Add(item);
            }
        }

        private string GetScaleBoxText(IItemMetadata dataMetadata)
        {
            var surveyItemMetaData = dataMetadata as SurveyItemMetaData;
            if (surveyItemMetaData?.TextData != null && surveyItemMetaData.TextData.Any())
            {
                var textData = surveyItemMetaData.TextData[0];

                var strippedText = textData.TextValues["StrippedText"];

                if (!string.IsNullOrWhiteSpace(strippedText))
                {
                    return strippedText.Length >= 30 ? strippedText.Substring(0, 30) + "..." : strippedText;
                }
            }

            return dataMetadata.ItemId.ToString();
        }

        private List<IItemMetadata> GetSelectedItems(List<IItemMetadata> surveyItems ,int sectionId)
        {
            var currentSectionItemIds = SurveySectionManager.GetSectionItemIds(sectionId);

            return surveyItems.Where(itemData => currentSectionItemIds.Contains(itemData.ItemId)).ToList();
        }

        private List<IItemMetadata> GetAvaliableItems(List<IItemMetadata> surveyItems, int sectionId)
        {
            var allSurveySectionItems = SurveySectionManager.GetAllSurveySectionItemIds(sectionId);
            return (from itemData in surveyItems where !allSurveySectionItems.Contains(itemData.ItemId) select itemData).ToList();
        }

        public void UpdateSectionEditor(MessageItemTextDecorator decorator)
        {
            decorator.ReportableSectionBreak = _reportableSection.Checked;

            if (!decorator.ReportableSectionBreak)
            {
                //clear survey section mapping
                SurveySectionManager.DeleteSection(_sectionId);
            }
            else
            {
                SurveySectionManager.DeleteSection(_sectionId);

                var value = _sectionEditorValues.Value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    foreach (var item in value.Split(' '))
                        SurveySectionManager.AddSectionItemId(_sectionId, int.Parse(item), _surveyId);
                }

                BuildDualBoxList(_templateId);
            }
        }

    }
}