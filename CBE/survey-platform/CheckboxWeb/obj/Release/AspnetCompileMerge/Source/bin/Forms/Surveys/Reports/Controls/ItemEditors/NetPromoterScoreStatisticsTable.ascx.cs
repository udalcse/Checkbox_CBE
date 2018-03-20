using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class NetPromoterScoreStatisticsTable : UserControlAnalysisItemEditorBase
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
        public override IFilterSelector FilterSelector { get { return _filterSelector; } }

        /// <summary>
        /// Add appearance editor to item control hierarchy.
        /// </summary>
        /// <param name="appearanceEditor"></param>
        protected override void AddAppearanceEditorToControl(IAppearanceEditor appearanceEditor)
        {
            _appearanceEditorPlace.Controls.Clear();
            _appearanceEditorPlace.Controls.Add((Control)appearanceEditor);
        }

        /// <summary>
        /// 
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
            _currentTabIndex.Text = HidePreview ? "1" : "0";

            var itemData = ItemData as NetPromoterScoreStatisticsItemData;
            if (itemData != null)
            {
                int? sourceResponseTemplateId = GetSourceResponseTemplateId();

                if (sourceResponseTemplateId.HasValue)
                {
                    List<string> allowedItemTypes = new List<string> { "NetPromoterScore" };

                    _sourceItemSelector.Initialize(
                        sourceResponseTemplateId.Value,
                        CurrentLanguage,
                        allowedItemTypes,
                        new List<string> { "RankOrder", "Matrix" },
                        null,
                        itemData.SourceItemIds,
                        IsPagePostBack);

                    _options.Initialize(itemData);
                }

            }
        }

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
        /// Update item data with configured values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            var data = ItemData as NetPromoterScoreStatisticsItemData;
            if (data != null)
            {
                data.SourceItemIds.Clear();
                data.SourceItemIds.AddRange(_sourceItemSelector.GetSelectedItems());

                _options.UpdateData(data);

                _sourceItemSelector.UpdateSelectedItems(_sourceItemSelector.GetSelectedItems());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            _currentTabIndex.Text = HidePreview ? "1" : "0";

            return base.SaveData();
        }
    }
}