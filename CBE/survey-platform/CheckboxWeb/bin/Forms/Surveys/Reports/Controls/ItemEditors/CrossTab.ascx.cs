using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    /// <summary>
    /// Editor for cross tab report items.
    /// </summary>
    public partial class CrossTab : UserControlAnalysisItemEditorBase
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
        public override void Initialize(int templateId, int pagePosition, Checkbox.Forms.Items.Configuration.ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, Checkbox.Forms.EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            if (ItemData is CrossTabItemData)
            {
                int? sourceResponseTemplateId = GetSourceResponseTemplateId();

                if (sourceResponseTemplateId.HasValue)
                {
                    _xAxisItemsSelector.Initialize(
                        sourceResponseTemplateId.Value,
                        CurrentLanguage,
                        new List<string>(),
                        new List<string> { "RankOrder", "Matrix" },
                        null,
                        ((CrossTabItemData)ItemData).XAxisItemIds,
                        IsPagePostBack);

                    _yAxisItemsSelector.Initialize(
                       sourceResponseTemplateId.Value,
                       CurrentLanguage,
                       new List<string>(),
                       new List<string> { "RankOrder", "Matrix" },
                       null,
                       ((CrossTabItemData)ItemData).YAxisItemIds,
                       IsPagePostBack);

                    //if this control is shown in "Add item" dialog, it should be limited in height
                    if (HidePreview)
                        _xAxisItemsSelector.Height = _yAxisItemsSelector.Height = 370;

                    _crossTabOptions.Initialize((CrossTabItemData)ItemData);

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

            if (ItemData is CrossTabItemData)
            {
                ((CrossTabItemData)ItemData).XAxisItemIds.Clear();
                ((CrossTabItemData)ItemData).YAxisItemIds.Clear();

                ((CrossTabItemData)ItemData).XAxisItemIds.AddRange(_xAxisItemsSelector.GetSelectedItems());
                ((CrossTabItemData)ItemData).YAxisItemIds.AddRange(_yAxisItemsSelector.GetSelectedItems());

                _yAxisItemsSelector.UpdateSelectedItems(_yAxisItemsSelector.GetSelectedItems());
                _xAxisItemsSelector.UpdateSelectedItems(_xAxisItemsSelector.GetSelectedItems());

                _crossTabOptions.UpdateData((CrossTabItemData)ItemData);
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
