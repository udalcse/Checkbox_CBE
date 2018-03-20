using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class CopyPage : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("s", IsRequired = true)]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("p", IsRequired = true)]
        public int PageId { get; set; }

        [QueryParameter("np")]
        public int? NewPagePosition { get; set; }

        /// <summary>
        /// Get response template to add items to
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null && ResponseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                }

                return _responseTemplate;
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

            Master.SetTitle(WebTextManager.GetText("/pageText/forms/copyPage.aspx/title", null, "Copy page") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64));
            Master.ClientCancelClickArgs = "{op: 'copyPage', result: 'cancel'}";

            //these events are needed because dropdownList values, which mean new positions of pages, are different for copying and moving actions
            _radMove.CheckedChanged += ChangePositionsForMovingCopyingPage;
            _radCopy.CheckedChanged += ChangePositionsForMovingCopyingPage;
        }

        private void ChangePositionsForMovingCopyingPage(object sender, EventArgs e)
        {
            ChangePositions();
        }

        private void ChangePositions()
        {
            if (_radCopy.Checked)
            {
                _pageList.Items[_pageList.Items.Count - 2].Value = (_pageList.Items.Count - 1).ToString();
                _pageList.Items[_pageList.Items.Count - 1].Value = (_pageList.Items.Count).ToString();
            }
            if (_radMove.Checked)
            {
                _pageList.Items[_pageList.Items.Count - 2].Value = (_pageList.Items.Count - 1).ToString();
                _pageList.Items[_pageList.Items.Count - 1].Value = (_pageList.Items.Count - 1).ToString();
            }

            //Set last page selected by default
            _pageList.SelectedIndex = _pageList.Items.Count - 1;
        }
        
        /// <summary>
        /// Populate list of pages
        /// </summary>
        private void PopulatePageList()
        {
            _pageList.Items.Clear();

            //Get survey content pages
            List<int> pageIds = new List<int>(ResponseTemplate.ListTemplatePageIds());

            //Add "first page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/firstPage"),
                "2"));

            string afterText = WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/makePage");

            foreach (int pageId in pageIds)
            {
                TemplatePage page = ResponseTemplate.GetPage(pageId);

                if (page != null
                    && page.PageType == TemplatePageType.ContentPage)
                {
                    //Page positions are 1-indexed, so Page 1 of survey actually has position = 2
                    // due to presence of hidden items page.
                  
                    _pageList.Items.Add(new ListItem(
                        string.Format(afterText, page.Position - 1),
                        (page.Position).ToString()));

                }
            }
            ChangePositions();

            //Add "last page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/lastPage"),
                pageIds.Count.ToString()));

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
            if (_radCopy.Checked)
                ResponseTemplate.CopyPage(PageId, int.Parse(_pageList.SelectedValue), (CheckboxPrincipal)HttpContext.Current.User);
            else
            {
                ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ResponseTemplate.MovePage(PageId, int.Parse(_pageList.SelectedValue));
            }

            ResponseTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ResponseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

            Master.CloseDialog("{op: 'copyPage', result: 'ok'}", true);
        }
    }
}
