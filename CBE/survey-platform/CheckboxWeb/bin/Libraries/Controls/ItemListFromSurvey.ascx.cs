using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Pagination;
using Checkbox.Users;
using Checkbox.Web.UI.Controls.GridTemplates;
using Prezza.Framework.Security;

namespace CheckboxWeb.Libraries.Controls
{
    public partial class ItemListFromSurvey : Checkbox.Web.Common.UserControlBase
    {
        private List<LightweightAccessControllable> _surveys;
        private ResponseTemplate _currentSurvey;
        private DataTable _allItems;


        /// <summary>
        /// Item count per page
        /// </summary>
        public int ItemCountPerPage { get; set; }

        /// <summary>
        /// Survey count per page
        /// </summary>
        public int SurveyCountPerPage { get; set; }


        /// <summary>
        /// Get/Set current survey id
        /// </summary>
        private int CurrentSurveyId
        {
            get
            {
                if (ViewState["currentSurveyId"] == null)
                    throw new Exception("Current Survey Id wasn't initialized.");
                return (int)ViewState["currentSurveyId"];
            }
            set { ViewState["currentSurveyId"] = value; }
        }

        /// <summary>
        /// Get current survey
        /// </summary>
        public ResponseTemplate CurrentSurvey
        {
            get
            {

                if (_currentSurvey == null || _currentSurvey.ID != CurrentSurveyId)
                {
                    _currentSurvey = ResponseTemplateManager.GetResponseTemplate(CurrentSurveyId);
                }

                return _currentSurvey;
            }
        }

        /// <summary>
        /// List of available response templates
        /// </summary>
        private List<LightweightAccessControllable> AvailableSurveys
        {
            get
            {
                if (_surveys == null)
                {
                    var paginationContext = new PaginationContext
                    {
                        CurrentPage = 1,
                        PageSize = 100000, //Max count of libraries in this control
                        Permissions = new List<string>(new[] { "Form.Edit" })
                    };
                    _surveys = ResponseTemplateManager.ListAccessibleTemplates(UserManager.GetCurrentPrincipal(), null, paginationContext, true, true);
                }

                return _surveys;
            }
        }


        /// <summary>
        /// Indexes os selected rows
        /// </summary>
        private List<int> SelectedIndexes
        {
            get
            {
                if (Session["_selectedItemsForSurvey"] == null)
                    Session["_selectedItemsForSurvey"] = new List<int>();
                return Session["_selectedItemsForSurvey"] as List<int>;
            }
            set { Session["_selectedItemsForSurvey"] = value; }
        }

        /// <summary>
        /// Get collection of selected item ids.
        /// </summary>
        public ReadOnlyCollection<int> SelectedItemIds
        {
            get
            {
                UpdateSelectedIndexes();
                List<int> ids = new List<int>();
                foreach (int i in (SelectedIndexes))
                {
                    ids.Add(Convert.ToInt32(AllItems.Rows[i]["ItemID"]));
                }
                return new ReadOnlyCollection<int>(ids);
            }
        }


        /// <summary>
        /// Data table that contains all items of selected survey
        /// </summary>
        private DataTable AllItems
        {
            get
            {
                if (_allItems == null)
                {
                    _allItems = CreateSurveyItemsTable();
                }
                return _allItems;
            }
        }

        /// <summary>
        /// Get/set previous selected page.
        /// </summary>
        private int PreviousPage
        {
            get
            {
                if (Session["ItemListFromSurvey_PrevPage"] == null)
                    Session["ItemListFromSurvey_PrevPage"] = 1;
                return (int)Session["ItemListFromSurvey_PrevPage"];
            }
            set { Session["ItemListFromSurvey_PrevPage"] = value; }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (!IsPostBack)
            {
                _panelSurveys.Visible = AvailableSurveys.Count > 0;
                _panelSurveysNotFound.Visible = AvailableSurveys.Count == 0;

                SelectedIndexes = null;
                if (AvailableSurveys.Count > 0)
                {
                    _surveyPager.PageCount = AvailableSurveys.Count / SurveyCountPerPage +
                                             ((AvailableSurveys.Count % SurveyCountPerPage > 0) ? 1 : 0);
                    _surveyPager.PageNumber = 1;
                    _surveyPager.ItemCount = AvailableSurveys.Count;
                    LoadSurveyList();
                }
                else
                {
                    _panelItemList.Visible = false;
                    _panelEmptyItemList.Visible = false;
                }
            }
        }

        protected void _surveyList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewSurvey")
            {                
                CurrentSurveyId = int.Parse(e.CommandArgument.ToString());
                
                _itemPager.PageCount = AllItems.Rows.Count / ItemCountPerPage +
                                   ((AllItems.Rows.Count % ItemCountPerPage > 0) ? 1 : 0);
                _itemPager.PageNumber = 1;
                _itemPager.ItemCount = AllItems.Rows.Count;
                _itemPager.Reload();
                SelectedIndexes = null;
                PreviousPage = 1;
                LoadItemList();
                _panelSurveys.Visible = false;                
            }
        }

        protected void _surveyPager_OnPagingChanged(object sender, Checkbox.Web.UI.Controls.CustomPagingEventArgs e)
        {
            LoadSurveyList();
        }

        protected void _toSurveySelectionLink_Click(object sender, EventArgs e)
        {
            _panelEmptyItemList.Visible = false;
            _panelItemList.Visible = false;
            _panelSurveys.Visible = true;
        }

        /// <summary>
        /// Update selected indexes based on the selected rows in _itemList
        /// </summary>
        private void UpdateSelectedIndexes()
        {
            int firstItemNumber = (PreviousPage - 1) * ItemCountPerPage;

            //This collection contains indexes from 0 to (itemPerPage-1). So we have to add firstItemNumber to work with the all items
            ReadOnlyCollection<int> selected = (_itemList.Columns[1] as CustomCheckBoxField).SelectedIndexes;

            for (int i = 0; i < ItemCountPerPage; i++)
            {
                if (selected.Contains(i))
                {
                    if (!SelectedIndexes.Contains(i + firstItemNumber))
                        SelectedIndexes.Add(i + firstItemNumber);
                }
                else
                {
                    SelectedIndexes.Remove(i + firstItemNumber);
                }
            }
        }

        /// <summary>
        /// Page change handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _pager_OnPagingChanged(object sender, Checkbox.Web.UI.Controls.CustomPagingEventArgs e)
        {
            UpdateSelectedIndexes();
            LoadItemList();
            PreviousPage = _itemPager.PageNumber;
        }

        /// <summary>
        /// Library change handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _libraryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _itemPager.PageNumber = 1;
            PreviousPage = 1;
            _itemPager.PageCount = AllItems.Rows.Count / ItemCountPerPage + ((AllItems.Rows.Count % ItemCountPerPage > 0) ? 1 : 0);
            _itemPager.ItemCount = AllItems.Rows.Count;
            _itemPager.Reload();
            SelectedIndexes = null;
            LoadItemList();
        }


        /// <summary>
        /// Load item list for current page and current library.
        /// </summary>
        private void LoadItemList()
        {
            DataTable table = GetItemListForPage(_itemPager.PageNumber);
            if (table.Rows.Count > 0)
            {
                _itemList.DataSource = table;
                _itemList.DataBind();
                _panelItemList.Visible = table.Rows.Count > 0;
            }
            else
            {
                _panelEmptyItemList.Visible = table.Rows.Count == 0;
            }
        }


        private void LoadSurveyList()
        {
            DataTable dt = CreateSurveyListForPage(_surveyPager.PageNumber);
            if (dt.Rows.Count > 0)
            {
                _surveyList.DataSource = dt;
                _surveyList.DataBind();
                _panelSurveys.Visible = true;
            }
            else
            {
                _panelSurveys.Visible = false;
            }

        }


        private DataTable CreateSurveyListForPage(int pageNumber)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SurveyID", typeof(Int32));
            dt.Columns.Add("SurveyName", typeof(string));

            int firstItemNumber = (pageNumber - 1) * SurveyCountPerPage;
            int lastItemNumber = Math.Min(AvailableSurveys.Count, pageNumber * SurveyCountPerPage);

            for (int i = firstItemNumber; i < lastItemNumber; i++)
            {
                dt.Rows.Add(AvailableSurveys[i].ID, AvailableSurveys[i].Name);
            }

            return dt;
        }

        /// <summary>
        /// Create a table for a library data source
        /// </summary>
        /// <returns></returns>
        private DataTable CreateSurveyItemsTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemID", typeof(Int32));
            dt.Columns.Add("ItemType", typeof(string));

            int[] templatePages = CurrentSurvey.ListTemplatePageIds();

            foreach (int pageId in templatePages)
            {
                var page = CurrentSurvey.GetPage(pageId);
                int[] items = page.ListItemIds();

                foreach (int itemId in items)
                {
                    var item = CurrentSurvey.GetItem(itemId);
                    dt.Rows.Add(item.ID, item.ItemTypeName);
                }
            }

            return dt;
        }

        /// <summary>
        /// Get Item List for current page and current library
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        private DataTable GetItemListForPage(int pageNumber)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemID", typeof(Int32));
            dt.Columns.Add("ItemType", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

            int firstItemNumber = (pageNumber - 1) * ItemCountPerPage;
            int lastItemNumber = Math.Min(AllItems.Rows.Count, pageNumber * ItemCountPerPage);

            for (int i = firstItemNumber; i < lastItemNumber; i++)
            {
                dt.Rows.Add(AllItems.Rows[i]["ItemID"], AllItems.Rows[i]["ItemType"], SelectedIndexes.Contains(i));
            }

            return dt;
        }
    }
}