using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class AverageScoreByPage : UserControlAnalysisItemEditorBase
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
        /// Get filter selector control
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

            var itemData = ItemData as AverageScoreByPageItemData;
            if (itemData != null)
            {
                int? sourceResponseTemplateId = GetSourceResponseTemplateId();

                if (sourceResponseTemplateId.HasValue)
                {
                    _sourcePageSelector.Initialize(
                        sourceResponseTemplateId.Value,
                        CurrentLanguage,
                        itemData.SourcePageIds,
                        itemData.AverageScoreCalculation == AverageScoreCalculation.PageAveragesWithTotalScore);
                }

               // _scoreOptions.Initialize((AverageScoreItemData)ItemData);
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

            var selected = _sourcePageSelector.GetSelectedPages();
            var includeTotalScore = _sourcePageSelector.IncludeTotalSurveyScore;

            var data = ItemData as AverageScoreByPageItemData;
            if (data != null)
            {
                data.SourcePageIds.Clear();
                data.SourcePageIds.AddRange(selected);
                data.AverageScoreCalculation = includeTotalScore ?
                    AverageScoreCalculation.PageAveragesWithTotalScore : AverageScoreCalculation.PageAverages;
            }
            //      _scoreOptions.UpdateData((AverageScoreItemData)ItemData);
            _sourcePageSelector.UpdateSelectedPages(selected, includeTotalScore);
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