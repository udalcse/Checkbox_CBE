using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Item editor for display report link.
    /// </summary>
    public partial class DisplayAnalysis : UserControlItemEditorBase
    {
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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _optionList.SelectedIndexChanged += _optionList_SelectedIndexChanged;

            _optionList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/redirectItemEditor/redirectAutomatically"),
                "redirectAutomatically"));

            _optionList.Items.Add(new ListItem(
                WebTextManager.GetText("/controlText/redirectItemEditor/linkToUrl"),
                "displayLink"));
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
            RegisterClientScriptInclude(
                "jquery.numeric.js",
                ResolveUrl("~/Resources/jquery.numeric.js"));
        }


        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// Editor supports embedded editors
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }

        /// <summary>
        /// Add appearance editor to the control hierarchy
        /// </summary>
        /// <param name="appearanceEditor"></param>
        protected override void AddAppearanceEditorToControl(IAppearanceEditor appearanceEditor)
        {
            if (appearanceEditor is Control)
            {
                _appearancePlace.Controls.Add((Control)appearanceEditor);
            }
        }

        /// <summary>
        /// Handle index changed to update whether link text is enabled or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _optionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _linkTxtLbl.Enabled = _optionList.SelectedValue.Equals("displayLink", StringComparison.InvariantCultureIgnoreCase);
            _linkTxt.Enabled = _linkTxtLbl.Enabled;
        }

        /// <summary>
        /// Initialize renderer
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
            {
                _currentTabIndex.Text = hidePreview ? "1" : "0";
            }

            if (TextDecorator is RedirectItemTextDecorator)
            {
                _optionList.SelectedValue = ((RedirectItemTextDecorator)TextDecorator).Data.RedirectAutomatically
                    ? "redirectAutomatically"
                    : "displayLink";

                _linkTxt.Text = ((RedirectItemTextDecorator)TextDecorator).LinkText;

                if (TextDecorator.Data is DisplayAnalysisItemData)
                    _newTabChk.Checked = ((DisplayAnalysisItemData)TextDecorator.Data).ShowInNewTab;

                _linkTxtLbl.Enabled = _optionList.SelectedValue.Equals("displayLink", StringComparison.InvariantCultureIgnoreCase);
                _linkTxt.Enabled = _linkTxtLbl.Enabled;

                //Populate and bind report list
                PopulateReportList();
            }
        }

        /// <summary>
        /// Populate report list
        /// </summary>
        private void PopulateReportList()
        {
            List<LightweightAnalysisTemplate> analysisList = AnalysisTemplateManager.ListAnalysisTemplatesForSurvey(
                UserManager.GetCurrentPrincipal(),
                TemplateId);

            Guid displayReportGuid = ((DisplayAnalysisItemData)TextDecorator.Data).AnalysisGUID;
            bool setSelected = false;

            foreach (LightweightAnalysisTemplate lightweightTemplate in analysisList)
            {
                var listItem = new ListItem(
                    Utilities.StripHtml(lightweightTemplate.Name, 64),
                    lightweightTemplate.Guid.ToString());

                //Set selected value, but ensure it is set only once.  It shouldn't be necessary to make
                //this check, but just to be safe...
                if (!setSelected && lightweightTemplate.Guid == displayReportGuid)
                {
                    listItem.Selected = true;
                    setSelected = true;
                }

                _reportList.Items.Add(listItem);
            }

            if (_reportList.Items.Count == 0)
            {
                _reportList.Items.Add(new ListItem(WebTextManager.GetText("/controlText/displayAnalysisItemEditor/noneCreated"), "__NONE_AVAILABLE__"));
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator is DisplayAnalysisTextDecorator)
            {
                ((DisplayAnalysisTextDecorator)TextDecorator).Data.RedirectAutomatically
                    = _optionList.SelectedValue.Equals("redirectAutomatically", StringComparison.InvariantCultureIgnoreCase);

                if (_reportList.SelectedValue != null && _reportList.SelectedValue != "__NONE_AVAILABLE__")
                    ((DisplayAnalysisTextDecorator)TextDecorator).Data.AnalysisGUID = new Guid(_reportList.SelectedValue); 

                ((DisplayAnalysisTextDecorator)TextDecorator).LinkText = _linkTxt.Text.Trim();
                ((DisplayAnalysisTextDecorator)TextDecorator).Data.ShowInNewTab = _newTabChk.Checked;
            }
        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            if (_reportList.SelectedValue == null || _reportList.SelectedValue == "__NONE_AVAILABLE__")
            {
                _reportRequiredErrorPanel.Visible = true;
                return false;
            }

            _reportRequiredErrorPanel.Visible = false;
            return true;
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