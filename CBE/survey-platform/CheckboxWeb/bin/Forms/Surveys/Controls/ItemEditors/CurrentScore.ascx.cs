using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Editor control for current score item
    /// </summary>
    public partial class CurrentScore : UserControlItemEditorBase
    {

        /// <summary>
        /// Editor supports embedded appearance editor
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }

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
        }

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

            if (TextDecorator is MessageItemTextDecorator
                && data is CurrentScoreItemData)
            {
                _questionTextEditor.Initialize(((MessageItemTextDecorator)TextDecorator).Message, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _currentScoreBehavior.Initialize((CurrentScoreItemData)data, templateId, pagePosition);
            }
        }

        /// <summary>
        /// Update item
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator is MessageItemTextDecorator
                && TextDecorator.Data is CurrentScoreItemData)
            {
                ((MessageItemTextDecorator)TextDecorator).Message = _questionTextEditor.Text;   
                _currentScoreBehavior.UpdateData((CurrentScoreItemData)TextDecorator.Data);
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