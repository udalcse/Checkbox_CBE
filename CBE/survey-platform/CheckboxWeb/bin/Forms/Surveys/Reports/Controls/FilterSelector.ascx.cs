using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Web.Analytics.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    /// <summary>
    /// Select filters to apply to report or report item
    /// </summary>
    public partial class FilterSelector : Checkbox.Web.Common.UserControlBase, IFilterSelector
    {
        private List<CheckBox> _availableItemList;
        private List<CheckBox> _selectedItemList;

        /// <summary>
        /// Get/set language code
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// List of available filters
        /// </summary>
        private List<FilterData> AllFilters
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_AllFilters"] == null)
                {
                    HttpContext.Current.Session[ID + "_AllFilters"] = new List<FilterData>();
                }

                return (List<FilterData>)HttpContext.Current.Session[ID + "_AllFilters"];
            }
            
            set
            {
                HttpContext.Current.Session[ID + "_AllFilters"] = value;
            }
        }

        /// <summary>
        /// List of applied filters
        /// </summary>
        private List<FilterData> AppliedFilters
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_AppliedFilters"] == null)
                {
                    HttpContext.Current.Session[ID + "_AppliedFilters"] = new List<FilterData>();
                }

                return (List<FilterData>)HttpContext.Current.Session[ID + "_AppliedFilters"];
            }
            
            set
            {
                HttpContext.Current.Session[ID + "_AppliedFilters"] = value;
            }
        }

        /// <summary>
        /// Bind event handlers
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            _addBtn.Click += _addBtn_Click;
            _removeBtn.Click += _removeBtn_Click;

            _appliedFiltersView.ItemCreated += _appliedFiltersView_ItemCreated;
            _availableFiltersView.ItemCreated += _availableFiltersView_ItemCreated;

            if (LanguageCode != null) //initialized already
                BindViews();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _availableFiltersView_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            CheckBox checkbox = e.Item.FindControl("_addChk") as CheckBox;

            if (checkbox != null)
            {
                checkbox.ID = "_addChk_" + ((FilterData)((ListViewDataItem)e.Item).DataItem).ID;

                _availableItemList.Add(checkbox);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _appliedFiltersView_ItemCreated(object sender, ListViewItemEventArgs e)
        {
            CheckBox checkbox = e.Item.FindControl("_removeChk") as CheckBox;

            if (checkbox != null)
            {
                checkbox.ID = "_removeChk_" + ((FilterData)((ListViewDataItem)e.Item).DataItem).ID;
                _selectedItemList.Add(checkbox);
            }
        }

        /// <summary>
        /// Handle clicking remove button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _removeBtn_Click(object sender, System.EventArgs e)
        {
            if (_selectedItemList != null)
            {
                foreach (CheckBox checkbox in _selectedItemList)
                {
                    if (checkbox.Checked)
                    {
                        int filterId;

                        if (int.TryParse(checkbox.ID.Replace("_removeChk_", string.Empty), out filterId))
                        {
                            AppliedFilters.RemoveAll(a => a.ID == filterId);
                        }
                    }
                }

                BindViews();
            }
        }

        /// <summary>
        /// Handle clicking add button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _addBtn_Click(object sender, System.EventArgs e)
        {
            if (_availableItemList != null)
            {
                foreach (CheckBox checkbox in _availableItemList)
                {
                    if (checkbox.Checked)
                    {
                        int filterId;

                        if (int.TryParse(checkbox.ID.Replace("_addChk_", string.Empty), out filterId))
                        {
                            if (AppliedFilters.Find(a => a.ID == filterId) == null)
                            {
                                AppliedFilters.Add(FilterFactory.GetFilterData(filterId));
                            }
                        }
                    }
                }

                BindViews();
            }
        }

        /// <summary>
        /// Initialize interface
        /// </summary>
        /// <param name="appliedFilters"></param>
        /// <param name="allFilters"></param>
        /// <param name="languageCode"></param>
        /// <param name="pageIsPostBack"></param>
        public void Initialize(List<FilterData> appliedFilters, List<FilterData> allFilters, string languageCode, bool pageIsPostBack)
        {
            AllFilters = allFilters;
            LanguageCode = languageCode;

            //Available filters view is list of all filters that are NOT in the list of 
            if(!pageIsPostBack)
            {
                AppliedFilters = appliedFilters;
            }

            if (Page != null)
            {
                //already placed to the form
                BindViews();
            }
        }

        /// <summary>
        /// Get list of applied filters
        /// </summary>
        /// <returns></returns>
        public List<FilterData> GetAppliedFilters()
        {
            return AppliedFilters;
        }

        /// <summary>
        /// Bind views to data
        /// </summary>
        private void BindViews()
        {
            _availableItemList = new List<CheckBox>();
            _selectedItemList = new List<CheckBox>();

            _availableFiltersView.DataSource = AllFilters.Where(all => (AppliedFilters.Find(applied => applied.ID == all.ID) == null));
            _availableFiltersView.DataBind();

            _appliedFiltersView.DataSource = AppliedFilters;
            _appliedFiltersView.DataBind();

            _addBtn.Visible = _availableFiltersView.Items.Count > 0;
            _noAvailableFiltersLbl.Visible = _availableFiltersView.Items.Count == 0;

            _removeBtn.Visible = AppliedFilters.Count > 0;
            _noAppliedFiltersLbl.Visible = AppliedFilters.Count == 0;
        }
    }
}