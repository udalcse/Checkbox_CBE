using System;
using System.Collections.Generic;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Base class for email item editor user control
    /// </summary>
    public partial class Email : UserControlItemEditorBase
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
        /// Handle on load to set initial tab value
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _emailOptions.MessageFormatChanged += _emailOptions_MessageFormatChanged;
        }

        /// <summary>
        /// Handle onload events to update control visibility
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateControlVisibility();

            _pipeSelector_messageEditor.Initialize(TemplateId, PagePosition, CurrentLanguage, _messageTextTxt.ClientID);


            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Handle message format change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _emailOptions_MessageFormatChanged(object sender, EventArgs e)
        {
            UpdateControlVisibility();

            //Copy values from one editor to another
            if (_messageEditor.Visible)
            {
                _messageEditor.Text = _messageTextTxt.Text;
            }
            else
            {
                _messageTextTxt.Text = _messageEditor.Text;
            }
        }

        /// <summary>
        /// Update control visibility
        /// </summary>
        private void UpdateControlVisibility()
        {
            _messageEditor.Visible = "Html".Equals(_emailOptions.SelectedMessageFormat, StringComparison.InvariantCultureIgnoreCase);
            _textMessageEditor.Visible = !_messageEditor.Visible;
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

            if (!isPagePostBack)
                _currentTabIndex.Text = HidePreview ? "1" : "0";

            if (_aliasText != null)
                _aliasText.Text = data.Alias;

            if (TextDecorator != null
                && TextDecorator is EmailItemTextDecorator)
            {
                _messageEditor.Initialize(((EmailItemTextDecorator)TextDecorator).Body, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _messageTextTxt.Text = ((EmailItemTextDecorator)TextDecorator).Body;

                _emailOptions.Initialize((EmailItemTextDecorator)TextDecorator, templateId, pagePosition, currentLanguage);
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator != null
                && TextDecorator is EmailItemTextDecorator)
            {
                //Get message text
                ((EmailItemTextDecorator)TextDecorator).Body = _textMessageEditor.Visible
                    ? _messageTextTxt.Text.Trim()
                    : _messageEditor.Text;

                //Set other options
                _emailOptions.UpdateData((EmailItemTextDecorator)TextDecorator);
                ((EmailItemTextDecorator)TextDecorator).Data.Alias = _aliasText.Text;
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