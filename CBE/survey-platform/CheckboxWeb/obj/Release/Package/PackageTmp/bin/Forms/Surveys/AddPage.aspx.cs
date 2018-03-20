using System;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Add page to survey
    /// </summary>
    public partial class AddPage : SecuredPage
    {
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get/set id of survey
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

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

            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/addPageToSurvey");

            PopulatePageList();
        }

        /// <summary>
        /// Populate list of pages
        /// </summary>
        private void PopulatePageList()
        {
            //Get survey content pages
            int[] pageIds = ResponseTemplate.ListTemplatePageIds();

            //Add "first page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/firstPage"),
                "2"));

            string afterText = WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/afterPage");

            foreach (int pageId in pageIds)
            {
                TemplatePage page = ResponseTemplate.GetPage(pageId);

                if (page != null
                    && page.PageType == TemplatePageType.ContentPage)
                {
                    //Page positions are 1-indexed, so Page 1 of survey actually has position = 2
                    // due to presence of hidden items page.

                    //Position of page to insert is AFTER listed page, so position of new page should be 1 more
                    // than position of selected page.
                    _pageList.Items.Add(new ListItem(
                        string.Format(afterText, page.Position - 1),
                        (page.Position + 1).ToString()));

                }
            }

            //Remove the last option of the pageList becase it is duplicate of the "lastPage" option.
            if(_pageList.Items.Count > 1)
                _pageList.Items.RemoveAt(_pageList.Items.Count - 1);

            //Add "last page"
            _pageList.Items.Add(new ListItem(
                WebTextManager.GetText("/pageText/forms/surveys/addPage.aspx/lastPage"),
                pageIds.Length.ToString()));

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
            int newPageId = ResponseTemplate.AddPageToTemplate(int.Parse(_pageList.SelectedValue), true);
            ResponseTemplate.ModifiedBy = User.Identity.Name;
            ResponseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

            //Fire event on finish
            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "closeWindow(window.top.templateEditor.onDialogClosed, {op: 'newPage', result: 'ok', targetPage:" + newPageId + "});", true);
        }
    }
}
