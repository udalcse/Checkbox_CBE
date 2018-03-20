﻿using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class Details : UserControlAnalysisItemEditorBase
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
        public override IRuleEditor RuleEditor { get { return _conditionEditor; } }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay { get { return _ruleDisplay; } }

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
        public override void Initialize(int templateId, int pagePosition, Checkbox.Forms.Items.Configuration.ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            if (ItemData is DetailsItemData)
            {
                int? sourceResponseTemplateId = GetSourceResponseTemplateId();

                if (sourceResponseTemplateId.HasValue)
                {
                    ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(sourceResponseTemplateId.Value);
                    rt.ListTemplateItemIds();

                    _sourceItemSelector.Initialize(
                        sourceResponseTemplateId.Value,
                        CurrentLanguage,
                        new List<string>(),
                        new List<string> { "RankOrder", "Matrix" },
                        null,
                        ((DetailsItemData)ItemData).SourceItemIds,
                        IsPagePostBack);

                }

                _detailsOptions.Initialize((DetailsItemData)ItemData);
            }
        }



        /// <summary>
        /// Update item data with configured values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (ItemData is DetailsItemData)
            {
                var copyOfList = new List<int>(_sourceItemSelector.GetSelectedItems());
                ((DetailsItemData)ItemData).SourceItemIds.Clear();
                ((DetailsItemData)ItemData).SourceItemIds.AddRange(copyOfList);

                _detailsOptions.UpdateData((DetailsItemData)ItemData);

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