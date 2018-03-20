using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class Javascript : UserControlItemEditorBase
    {
        /// <summary>
        /// Initialize item editor
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

            _activeDisplay.Initialize(data);

            var jsItemData = data as JavascriptItemData;
            if (jsItemData != null)
            {
                _scriptEditor.Script = jsItemData.Script;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IRuleEditor RuleEditor
        {
            get { return _conditionEditor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay
        {
            get { return _ruleDisplay; }
        }

        public override void UpdateData()
        {
            base.UpdateData();

            var jsItemData = ItemData as JavascriptItemData;
            if (jsItemData != null)
            {
                jsItemData.Script = _scriptEditor.Script.Trim();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            int result = base.SaveData();


            return result;
        }
    }
}