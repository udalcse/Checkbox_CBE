using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Linq;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Analytics.UI.Editing;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    public partial class Frequency : UserControlAnalysisItemEditorBase
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

            if (!isPagePostBack)
            {
                _currentTabIndex.Text = HidePreview ? "1" : "0";
            }

            if (ItemData is AnalysisItemData)
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
                        ((AnalysisItemData)ItemData).SourceItemIds,
                        IsPagePostBack);
                }

                _optionsEditor.Initialize((AnalysisItemData)ItemData);

            }
        }

        /// <summary>
        /// Update item data with configured values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (ItemData is AnalysisItemData)
            {
                var copyOfList = new List<int>(_sourceItemSelector.GetSelectedItems());
                ((AnalysisItemData)ItemData).SourceItemIds.Clear();
                ((AnalysisItemData)ItemData).SourceItemIds.AddRange(copyOfList);

                _optionsEditor.UpdateItemData((AnalysisItemData)ItemData);

                _sourceItemSelector.UpdateSelectedItems(copyOfList);
            }
        }
    }
}