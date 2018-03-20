using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Base control for editing ml text items
    /// </summary>
    public partial class MultiLineText : UserControlItemEditorBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// Appearance editor may be embedded in the control
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }

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
        protected override void OnLoad(EventArgs e)
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
        /// Initialize the editor
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

            if (TextDecorator != null
                && TextDecorator is TextItemDecorator)
            {
                _questionTextEditor.Initialize(((TextItemDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode, CustomFieldType.MultiLine);
                _descriptionTextEditor.Initialize(((TextItemDecorator)TextDecorator).SubText, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _behaviorEditor.Initialize((TextItemDecorator)TextDecorator, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
            }

       
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (!(TextDecorator is TextItemDecorator))
            {
                return;
            }

            ((TextItemDecorator)TextDecorator).Text = _questionTextEditor.Text;
            ((TextItemDecorator)TextDecorator).SubText = _descriptionTextEditor.Text;

            _behaviorEditor.UpdateData(((TextItemDecorator)TextDecorator));
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