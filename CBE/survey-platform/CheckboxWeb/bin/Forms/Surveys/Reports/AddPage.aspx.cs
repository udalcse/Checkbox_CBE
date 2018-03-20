using System;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddPage : SecuredPage
    {
        private AnalysisTemplate _analysisTemplate;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("r")]
        public int AnalysisTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }
        
        /// <summary>
        /// Get response template to add items to
        /// </summary>
        public AnalysisTemplate AnalysisTemplate
        {
            get
            {
                if (_analysisTemplate == null && AnalysisTemplateId > 0)
                {
                    _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(AnalysisTemplateId);
                }

                return _analysisTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += _okButton_Click;
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/addPageToReport"));

            PopulatePageList();
        }

        /// <summary>
        /// Populate list of pages
        /// </summary>
        private void PopulatePageList()
        {
            //Get survey content pages
            int[] pageIds = AnalysisTemplate.ListTemplatePageIds();

            //Add "first page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/firstPage"),
                "1"));

            string afterText = WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/afterPage");
            foreach (int pageId in pageIds)
            {
                TemplatePage page = AnalysisTemplate.GetPage(pageId);

                if (page != null
                    && page.PageType == TemplatePageType.ContentPage)
                {
                    _pageList.Items.Add(new ListItem(
                        string.Format(afterText, page.Position),
                        (page.Position + 1).ToString()));

                }
            }

            //Remove the last option of the pageList becase it is duplicate of the "lastPage" option.
            if (_pageList.Items.Count > 1)
                _pageList.Items.RemoveAt(_pageList.Items.Count - 1);

            //Add "last page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/lastPage"),
                (pageIds.Length + 1).ToString()));

            //Make last page item selected by default
            _pageList.SelectedIndex = _pageList.Items.Count - 1;
        }

        /// <summary>
        /// Handle ok click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButton_Click(object sender, EventArgs e)
        {
            int newPageId = AnalysisTemplate.AddPageToTemplate(int.Parse(_pageList.SelectedValue), true);
            AnalysisTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            AnalysisTemplate.Save();

            //Fire event on finish
            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.templateEditor.onDialogClosed, {op: 'newPage', result: 'ok', targetPage:" + newPageId + "});", true);
        }
    }
}
