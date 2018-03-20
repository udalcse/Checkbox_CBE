using System;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using System.Web.UI;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Editor for display response items
    /// </summary>
    public partial class DisplayResponse : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;
        private EditMode _editMode;
        /// <summary>
        /// Editor supports embedded editors
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
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
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
        /// Get response template
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_editMode != Checkbox.Forms.EditMode.Survey)
                    return null;
                return _responseTemplate ??
                       (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(TemplateId));
            }
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
        /// Bind event hander
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _optionsList.SelectedIndexChanged += _optionsList_SelectedIndexChanged;

            _optionsList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/displayResponseItemEditor/displayInline"),
                "displayInline"));

            _optionsList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/displayResponseItemEditor/displayLink"),
                "displayLink"));
        }

        /// <summary>
        /// Handle change of link option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _optionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _linkTxtLbl.Enabled = _optionsList.SelectedValue.Equals("displayLink", StringComparison.InvariantCultureIgnoreCase);
            _linkTxt.Enabled = _linkTxtLbl.Enabled;
        }

        /// <summary>
        /// Initialize editor control
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
            _editMode = editMode;
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            if (!isPagePostBack)
            {
                BindInputs();
            }

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is SelectItemTextDecorator)
            {
            }            
        }

        /// <summary>
        /// Bind editor inputs to editor controls
        /// </summary>
        private void BindInputs()
        {
            if (TextDecorator is DisplayResponseItemTextDecorator)
            {
                _linkTxtLbl.Enabled = !((DisplayResponseItemTextDecorator)TextDecorator).Data.DisplayInlineResponse;
                _linkTxt.Enabled = _linkTxtLbl.Enabled;

                _optionsList.SelectedValue = ((DisplayResponseItemTextDecorator)TextDecorator).Data.DisplayInlineResponse 
                    ? "displayInline" 
                    : "displayLink";

                _linkTxt.Text = ((DisplayResponseItemTextDecorator)TextDecorator).LinkText;

                _includeResponseDetailsChk.Checked = ((DisplayResponseItemTextDecorator)TextDecorator).Data.IncludeResponseDetails;
                _includeMessageItemsChk.Checked = ((DisplayResponseItemTextDecorator)TextDecorator).Data.IncludeMessageItems;
                _showPageNumbersChk.Checked = ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowPageNumbers;
                _showQuestionNumbersChk.Checked = ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowQuestionNumbers;
                _showHiddenItemsChk.Checked = ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowHiddenItems;
            }
        }

        /// <summary>
        /// Update item data with selected values
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator is DisplayResponseItemTextDecorator)
            {
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.DisplayInlineResponse = _optionsList.SelectedValue.Equals("displayInline", StringComparison.InvariantCultureIgnoreCase);
                ((DisplayResponseItemTextDecorator)TextDecorator).LinkText = _linkTxt.Text.Trim();
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.IncludeResponseDetails = _includeResponseDetailsChk.Checked;
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.IncludeMessageItems = _includeMessageItemsChk.Checked;
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowPageNumbers = _showPageNumbersChk.Checked;
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowQuestionNumbers = _showQuestionNumbersChk.Checked;
                ((DisplayResponseItemTextDecorator)TextDecorator).Data.ShowHiddenItems = _showHiddenItemsChk.Checked;
            }
        }
    }
}