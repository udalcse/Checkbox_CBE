using System;
using System.Collections.Generic;
using System.Net;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Base class for user control item editor
    /// </summary>
    public partial class Message : UserControlItemEditorBase
    {
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

            //_reportableSection.Checked = ((MessageItemData) ItemData).ReportableSectionBreak;
            
            _sectionEditor.Initialize(data, templateId, isPagePostBack);

            if (_aliasText != null)
                _aliasText.Text = data.Alias;
            

            if (TextDecorator is MessageItemTextDecorator)
            {
                _textEditor.Initialize(((MessageItemTextDecorator)TextDecorator).Message, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
            }
        }

        /// <summary>
        /// Update data source
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            var decorator = TextDecorator as MessageItemTextDecorator;
            if (decorator != null)
            {
                decorator.Message = Utilities.ReplaceHtmlAttributes(WebUtility.HtmlEncode(_textEditor.Text), false);
                //((MessageItemTextDecorator)TextDecorator).ReportableSectionBreak = _reportableSection.Checked;
                decorator.Data.Alias = _aliasText.Text;
            }

            _sectionEditor.UpdateSectionEditor(decorator);
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