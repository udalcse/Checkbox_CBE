using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class CopyPage : SecuredPage
    {
        private AnalysisTemplate _analysisTemplate;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("r", IsRequired = true)]
        public int ReportId { get; set; }

        [QueryParameter("p", IsRequired = true)]
        public int PageId { get; set; }

        [QueryParameter("np")]
        public int? NewPagePosition { get; set; }

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
                if (_analysisTemplate == null && ReportId > 0)
                {
                    _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId);
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

            PopulatePageList();

            Master.SetTitle(WebTextManager.GetText("/pageText/analytics/copyPage.aspx/title") + " - " + Utilities.StripHtml(AnalysisTemplate.Name, 64));
            Master.ClientCancelClickArgs = "{op: 'copyPage', result: 'cancel'}";
        }

        /// <summary>
        /// Populate list of pages
        /// </summary>
        private void PopulatePageList()
        {
            _pageList.Items.Clear();

            //Get survey content pages
            List<int> pageIds = new List<int>(AnalysisTemplate.ListTemplatePageIds());

            //Add "first page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/firstPage"),
                "1"));

            string afterText = WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/makePage");

            foreach (int pageId in pageIds)
            {
                if (pageId == pageIds.Last())
                    break;

                var page = AnalysisTemplate.GetPage(pageId);

                if (page != null
                    && page.PageType == TemplatePageType.ContentPage)
                {
                    _pageList.Items.Add(new ListItem(
                        string.Format(afterText, page.Position + 1),
                        (page.Position + 1).ToString()));

                }
            }

            //Add "last page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/lastPage"),
                (pageIds.Count + 1).ToString()));

            //Set last page selected by default
            _pageList.SelectedIndex = _pageList.Items.Count - 1;
        }


        /// <summary>
        /// Handle ok click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButton_Click(object sender, EventArgs e)
        {
            int selectedValue = int.Parse(_pageList.SelectedValue);
            if (_radCopy.Checked)
            {
                AnalysisTemplate.CopyPage(PageId, selectedValue, (CheckboxPrincipal)HttpContext.Current.User);
            }
            else
            {
                AnalysisTemplate.MovePage(PageId, selectedValue -
                    (_pageList.SelectedIndex == _pageList.Items.Count - 1 ? 1 : 0));
            }

            AnalysisTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            AnalysisTemplate.Save();

            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.templateEditor.onDialogClosed, {op: 'copyPage', result: 'ok'});", true);
        }
    }
}
