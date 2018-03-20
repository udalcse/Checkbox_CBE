using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Globalization;
using Checkbox.Web;
using Checkbox.Common;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Editor control for rules
    /// </summary>
    public partial class RuleEditor : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string TabStyle { get; set; }

        private ExpressionEditorParams _editorParams;

        /// <summary>
        /// Get/set  editor params
        /// </summary>
        protected ExpressionEditorParams Params
        {
            get {
                return _editorParams ??
                       (_editorParams = HttpContext.Current.Session["_EditorParams"] as ExpressionEditorParams);
            }
            set
            {
                _editorParams = value;
                HttpContext.Current.Session["_EditorParams"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasConditions
        {
            get
            {
                if (Params == null || Params.RuleDataService == null || Params.DependentItemId == null)
                    return false;
                return Params.RuleDataService.ItemHasCondition(Params.DependentItemId.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RuleEditor()
        {
            TabStyle = "default";
        }
        
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _basicRuleRepeater.ItemDataBound += _ruleRepeater_ItemDataBound;
            _advancedRuleRepeater.ItemDataBound += _advancedRuleRepeater_ItemDataBound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string file = GlobalizationManager.GetDatePickerLocalizationFile();
            if (!string.IsNullOrEmpty(file))
            {
                RegisterClientScriptInclude(
                    file,
                    ResolveUrl("~/Resources/" + file));
            }

            //Localization for datepicker
            RegisterClientScriptInclude(
                "jquery.localize.js",
                ResolveUrl("~/Resources/jquery.localize.js"));

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            //Service Helper
            RegisterClientScriptInclude(
               "serviceHelper.js",
               ResolveUrl("~/Services/js/serviceHelper.js"));

            //Survey management
            RegisterClientScriptInclude(
                "svcSurveyManagement.js",
                ResolveUrl("~/Services/js/svcSurveyManagement.js"));

            //expression editor is needed for basic and advanced editor
            RegisterClientScriptInclude(
                "expressionEditor.js",
                ResolveUrl("~/Resources/expressionEditor.js"));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleDataService"></param>
        /// <param name="ruleType"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="maxSourceQuestionPagePosition"></param>
        /// <param name="dependentPageId"></param>
        /// <param name="dependentItemId"></param>
        /// <param name="languageCode"></param>
        public void Initialize(RuleDataService ruleDataService, RuleType ruleType, int responseTemplateId, int maxSourceQuestionPagePosition, int? dependentPageId, int? dependentItemId, string languageCode)
        {
            Params = new ExpressionEditorParams
            {
                RuleType = ruleType,
                RuleDataService = ruleDataService,
                ResponseTemplateId = responseTemplateId,
                MaxSourceQuestionPagePosition = maxSourceQuestionPagePosition,
                DependentItemId = dependentItemId,
                DependentPageId = dependentPageId,
                LanguageCode = languageCode
            };



            //Hide advanced/basic option for page branches, which only support "Advanced"
            if (ruleType == RuleType.PageBranchCondition)
            {
                HideBasicViewTab();
            }

            ConfigureBrachPanelVisibility(ruleType);

            //Show new branch panel when appropriate
            if (_newBranchPanel.Visible)
            {
                var newBranchPageList = FindControl("_newBranchPageList") as DropDownList;
                if (newBranchPageList != null)
                {
                    newBranchPageList.Items.Clear();
                    newBranchPageList.Items.AddRange(GetPageNames());
                    newBranchPageList.SelectedIndex = 0;
                }         
                _newBranchEditor.Initialize(Params);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleType"></param>
        public void ConfigureBrachPanelVisibility(RuleType ruleType)
        {
            _newBranchPanel.Visible = ruleType == RuleType.PageBranchCondition;
        }

        /// <summary>
        /// 
        /// </summary>
        private void HideBasicViewTab()
        {
            _currentTabIndex.Text = "1";
            _tabPanel.Style["display"] = "none";
            _basicRuleRepeater.Visible = false;
        }

        /// <summary>
        /// Initialize rule editor
        /// </summary>
        public void Bind()
        {
            if (Params == null)
            {
                throw new Exception("Rule editor control must be Initialized before it is bound.");
            }

            if (Params.RuleDataService == null)
            {
                throw new Exception("Rule editor params RuleDataService is null.");
            }

            _basicRuleRepeater.DataSource = ExpressionEditorControl.GetRuleData(Params);
            _basicRuleRepeater.DataBind();

            _advancedRuleRepeater.DataSource = ExpressionEditorControl.GetRuleData(Params);
            _advancedRuleRepeater.DataBind();
        }

        /// <summary>
        /// Save changes to rule data
        /// </summary>
        public void SaveChanges()
        {
            //Try/catch is set to avoid from error while there is no Source selected.
            try
            {
                if (Params != null
                && Params.RuleDataService != null)
                {
                    //Save rule data
                    Params.RuleDataService.SaveRuleData();

                    //Ensure source template updated and it's rule data is reloaded.  Failure to do this
                    // will cause cached version of template to be missing new rules even though UI will look
                    // correct due to separate loading of rule data service below.
                    ResponseTemplateManager.MarkTemplateUpdated(Params.ResponseTemplateId);

                    //Reload rule data
                    Params.RuleDataService = ResponseTemplate.CreateRuleDataService(Params.ResponseTemplateId);

                    //Rebind
                    Bind();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ruleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var expressionEditor = e.Item.FindControl("_basicEditor") as BasicExpressionEditor;

            if (expressionEditor != null
                && e.Item.DataItem is RuleData
                && ((RuleData)e.Item.DataItem).Expression is CompositeExpressionData)
            {
                expressionEditor.Initialize(Params);

                //If basic mode not allowed when editing page/item conditions, disable option
                if (Params.RuleType != RuleType.PageBranchCondition)
                {
                    if (!expressionEditor.BasicModeAllowed)
                    {
                        HideBasicViewTab();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _advancedRuleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var expressionEditor = e.Item.FindControl("_advancedEditor") as AdvancedExpressionEditor;

            if (expressionEditor != null
                && e.Item.DataItem is RuleData
                && ((RuleData)e.Item.DataItem).Expression is CompositeExpressionData)
            {
                expressionEditor.Initialize(Params);
                expressionEditor.Bind((CompositeExpressionData)((RuleData)e.Item.DataItem).Expression);
            }

            //Show hide branching panel
            var branchingDestinationPanel = e.Item.FindControl("_goToPagePanel") as Panel;

            if (branchingDestinationPanel != null)
            {
                branchingDestinationPanel.Visible = Params.RuleType == RuleType.PageBranchCondition;
            }

            //If editing page branches, bind "GoTo" list.
            if (Params.RuleType == RuleType.PageBranchCondition)
            {
                var gotoPageList = e.Item.FindControl("_goToPageList") as DropDownList;

                if (gotoPageList != null)
                {
                    gotoPageList.SelectedIndexChanged += gotoPageList_SelectedIndexChanged;

                    int ruleId = ((RuleData) e.Item.DataItem).RuleId;

                    gotoPageList.Items.Clear();
                    gotoPageList.Items.AddRange(GetPageNames());
                    gotoPageList.Attributes["ruleId"] = ruleId.ToString();
                    gotoPageList.SelectedValue =
                        Params.RuleDataService.GetTargetPageId(ruleId).ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gotoPageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pageList = sender as DropDownList;

            if (pageList != null)
            {
                Params.RuleDataService.SetPageBranchTargetPageId(int.Parse(pageList.Attributes["ruleId"]),
                                                                 Convert.ToInt32(pageList.SelectedValue));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ListItem[] GetPageNames()
        {
            ResponseTemplate responseTemplate = ResponseTemplateManager.GetResponseTemplate(Params.ResponseTemplateId);
            var result = new List<ListItem>();

            foreach (int pageId in responseTemplate.ListTemplatePageIds())
            {
                String pageName;
                if (GetPageName(pageId, responseTemplate, out pageName))
                    result.Add(new ListItem(pageName, pageId.ToString()));
            }
            return result.ToArray();

        }


        /// <summary>
        /// get page name for dropDownList
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="responseTemplate"></param>
        /// <param name="pageName"></param>
        /// <returns>true - if this page can be in this branch, another - false</returns>
        private bool GetPageName(int pageId, ResponseTemplate responseTemplate, out String pageName)
        {
            TemplatePage page = responseTemplate.GetPage(pageId);
            pageName = string.Empty;

            if (page.PageType != TemplatePageType.ContentPage)
            {
                if (page.PageType.ToString() == "Completion")
                    pageName = WebTextManager.GetText("/enum/templatePageType/" + page.PageType, WebTextManager.GetUserLanguage(), page.PageType.ToString());
                else
                    return false;
            }
            else
            {
                var pageIds = new List<int>(responseTemplate.ListTemplatePageIds());

                if (Params.DependentPageId == null)
                    return false;

                if (pageIds.IndexOf(pageId) <= pageIds.IndexOf(Params.DependentPageId.Value))
                    return false;

                //1st content page is page w/position 2 in survey
                //TODO: Upgrade task to number page positions?
                pageName = string.Format("{0} {1}", WebTextManager.GetText("/pageText/forms/surveys/edit.aspx/page"), page.Position - 1);

                if (Utilities.IsNotNullOrEmpty(page.Title))
                {
                    pageName += ":  " + Utilities.TruncateText(page.Title, 32);
                }
            }

            return true;
        }


    }
}
