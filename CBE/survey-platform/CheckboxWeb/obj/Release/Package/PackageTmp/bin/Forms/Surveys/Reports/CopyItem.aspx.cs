using System;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Forms;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.UI;
using Checkbox.Analytics;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Reports
{
    /// <summary>
    /// Move/Copy survey page
    /// </summary>
    public partial class CopyItem : SecuredPage
    {
        private AnalysisTemplate _reportTemplate;
        private string _editLanguage;

        [QueryParameter("r", IsRequired = true)]
        public int ReportId { get; set; }

        [QueryParameter("i", IsRequired=true)]
        public int ItemId { get; set; }

        [QueryParameter("a")]
        public string Action { get; set; }

        [QueryParameter("ip")]
        public int? ItemPosition { get; set; }

        [QueryParameter("p")]
        public int? TargetPageId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }


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
        public AnalysisTemplate ReportTemplate
        {
            get
            {
                if (_reportTemplate == null && ReportId > 0)
                {
                    _reportTemplate = AnalysisTemplateManager.GetAnalysisTemplate(ReportId);
                }

                return _reportTemplate;
            }
        }

        /// <summary>
        /// Get position of selected page
        /// </summary>
        private TemplatePage SelectedPage
        {
            get
            {
                int pageId;

                if (int.TryParse(_destinationPageList.SelectedValue, out pageId))
                {
                    return ReportTemplate.GetPage(pageId);
                }

                return null;
            }
        }

        /// <summary>
        /// Get position for item
        /// </summary>
        private int SelectedItemPosition
        {
            get
            {
                int position;
                if (int.TryParse(_destinationItemList.SelectedValue, out position))
                {
                    return position;
                }
                return -1;
            }
        }

        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                if (String.IsNullOrEmpty(_editLanguage))
                {
                    if (Utilities.IsNotNullOrEmpty(LanguageCode))
                    {
                        _editLanguage = LanguageCode;
                    }
                    else
                    {
                        var lightWeightRt = ResponseTemplateManager.GetLightweightResponseTemplate(ReportTemplate.ResponseTemplateID);

                        _editLanguage = lightWeightRt.DefaultLanguage;

                        if (string.IsNullOrEmpty(_editLanguage))
                        {
                            _editLanguage = TextManager.DefaultLanguage;
                        }
                    }
                }
                return _editLanguage;
            }
        }


        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += _okButton_Click;
            Master.SetTitle(WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/moveCopyItem"));

            PopulatePageList();
            PopulateItemList();

            Master.ClientCancelClickArgs = "{op: 'moveItem', result: 'cancel'}";

            if("c".Equals(Action, StringComparison.InvariantCultureIgnoreCase))
            {
                _radCopy.Checked = true;
                _radMove.Checked = false;
            }
            else
            {
                _radCopy.Checked = false;
                _radMove.Checked = true;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateItemList()
        {
            _destinationItemList.Items.Clear();

            //Add "First Item" Option
            _destinationItemList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/firstItem"), "1"));

            //If no page selected, disable control
            if (_destinationPageList.SelectedIndex <0)
            {
                _destinationItemList.Enabled = false;
                return;
            }

            if (SelectedPage == null)
            {
                //Page not found, show an error
                throw new Exception("Unable to load selected page.");
            }

            //Now list items
            int[] pageItemIds = SelectedPage.ListItemIds();
            int positionCount = 1;

            string afterItemText = WebTextManager.GetText("/pageText/forms/surveys/reports/copyMoveItem.aspx/afterItem", WebTextManager.GetUserLanguage(), "After item {0} - {1}");

            foreach (int itemId in pageItemIds)
            {
                ItemData itemData = ReportTemplate.GetItem(itemId);

                if (itemData != null)
                {
                    string itemText = Utilities.StripHtml(
                        ItemConfigurationManager.GetItemText(itemData.ID.Value, EditLanguage, null, false, false),
                        64);

                    _destinationItemList.Items.Add(new ListItem(
                        string.Format(afterItemText, positionCount, itemText),
                        (positionCount + 1).ToString()));       //Add 1 to position count since we want to add item "after"

                    positionCount++;
                }
            }

            if (_destinationItemList.Items.Count > 1)
            {
                //Add "Last Item" option
                _destinationItemList.Items.Add(new ListItem(
                    WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/lastItem"),
                    positionCount.ToString()));
            }

            //if (_destinationItemList.Items.Count == 1)
            //{
            //    _destinationItemList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/forms/surveys/reports/copyItem.aspx/noItems"), "-1"));
            //}

            //Attempt to set default position, if any
            if (ItemPosition.HasValue
                && SelectedPage.ID == TargetPageId
                && _destinationItemList.Items.FindByValue(ItemPosition.ToString()) != null)
            {
                _destinationItemList.SelectedValue = ItemPosition.ToString();
            }
            else
            {
                //Otherwise, choose "last item" by default
                _destinationItemList.SelectedIndex = _destinationItemList.Items.Count - 1;
            }
        }

        /// <summary>
        /// Populate list of pages and their items
        /// </summary>
        private void PopulatePageList()
        {
            _destinationPageList.Items.Clear();
            _destinationPageList.Enabled = true;

            int[] pageIds = ReportTemplate.ListTemplatePageIds();

            //Loop through pages.
            for (int pageIndex = 0; pageIndex < pageIds.Length; pageIndex++)
            {
                _destinationPageList.Items.Add(new ListItem(
                    WebTextManager.GetText("/pageText/forms/surveys/reports/addItem.aspx/page") + (pageIndex+1),
                    pageIds[pageIndex].ToString()));
            }

            //Use specified page, if any, otherwise default to page
            // containing target item
            if (TargetPageId.HasValue
                && _destinationPageList.Items.FindByValue(TargetPageId.ToString()) != null)
            {
                _destinationPageList.SelectedValue = TargetPageId.ToString();
            }
            else
            {
                var itemPage = ReportTemplate.GetPagePositionForItem(ItemId);

                if (itemPage.HasValue)
                {
                    var pageIndex = itemPage.Value - 1;

                    if (pageIndex >= 0 && pageIndex < _destinationPageList.Items.Count)
                    {
                        _destinationPageList.SelectedIndex = pageIndex;
                    }
                }
            }
        }

        protected void _destinationPageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateItemList();
        }

        /// <summary>
        /// Handle ok button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _okButton_Click(object sender, System.EventArgs e)
        {
            if (_radCopy.Checked)
            {
                CopyReportItem(SelectedItemPosition);
            }
            else
            {
                MoveReportItem(SelectedItemPosition);
            }

            //Close window and report status to status page.
            //ClientScript.RegisterClientScriptBlock(GetType(), "CloseReload", "CloseAndReload();", true);
        }

        private void MoveReportItem(int position)
        {
            if (ReportTemplate == null)
            {
                throw new Exception("Unable to load template [" + ReportId + "] to move/copy item");
            }

            var oldPagePosition = ReportTemplate.GetPagePositionForItem(ItemId);

            TemplatePage oldPage = null;
            if (oldPagePosition.HasValue)
                oldPage = ReportTemplate.GetPageAtPosition(oldPagePosition.Value);

            if (oldPage.ID.HasValue)
            {
                ReportTemplate.MoveItemToPage(ItemId, oldPage.ID.Value, SelectedPage.ID.Value, position);
                ResponseTemplateManager.MarkTemplateUpdated(ReportId);
                ReportTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                ReportTemplate.Save();
            }

            var oldPageId = oldPage != null && oldPage.ID.HasValue ? oldPage.ID.Value : -1;

            Master.CloseDialog("{op: 'moveItem', result: 'ok', sourcePage: " + oldPageId + ", targetPage: '" + SelectedPage.ID + "'}", true);
        }

        private void CopyReportItem(int position)
        {
            if (ReportTemplate == null)
            {
                throw new Exception("Unable to load template with [" + ReportId + "]");
            }

            ItemData item = ReportTemplate.GetItem(ItemId);
            ItemData copy = ItemConfigurationManager.CopyItem(item, (CheckboxPrincipal)HttpContext.Current.User);

            if (copy == null)
                return;

            if (copy.ID.HasValue)
            {
                ReportTemplate.AddItemToPage(SelectedPage.ID.Value, copy.ID.Value, position);
            }
            else
            {
                throw new Exception("Error creating copy of item.");
            }
            ResponseTemplateManager.MarkTemplateUpdated(ReportId);
            ReportTemplate.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
            ReportTemplate.Save();

            Master.CloseDialog("{op: 'moveItem', result: 'ok', targetPage:'" + SelectedPage.ID + "'}", true);
        }
    }
}
