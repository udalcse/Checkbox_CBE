using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text;
using Checkbox.Web;

namespace CheckboxWeb.Controls.Search
{
    public delegate void SearchEventHandler(object sender, SearchEventArgs e);

    /// <summary>
    /// Search control
    /// </summary>
    public partial class SearchControl : Checkbox.Web.Common.UserControlBase
    {
        public event SearchEventHandler Search;
        public event EventHandler ClearSearch;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _searchContainer.Visible = SearchFields.Count > 0;

            //Register scripts
            if (Page != null)
            {
                RegisterClientScriptInclude(
                    GetType(),
                    "SearchControl",
                    ResolveUrl("~/Resources/searchControl.js"));

                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "bindSearchPress" + ID,
                    "searchControl.bindSearch('" + _searchTerm.ClientID + "', '" + _searchButton.LinkButtonName + "');",
                    true);
            }
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        NotifyParentProperty(true)]
        public ListItemCollection SearchFields
        {
            get { return _searchField.Items; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SearchField
        {
            get { return _searchField.SelectedValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SearchValue
        {
			get { return _searchTerm.Text.Trim().Replace("'", "''"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Unit SearchTextBoxWidth
        {
            get { return _searchTerm.Width; }
            set { _searchTerm.Width = value; }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handles the click event of the search button
        /// - Shows the search status panel
        /// - Raises the search event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(_searchTerm.Text))
            {
                SearchEventArgs args;
                var searchStatusLabel = new StringBuilder();
                if (_searchTerm.Visible)
                {
                    searchStatusLabel.Append(WebTextManager.GetText("/controlText/SearchControl.ascx/FieldSearchPreamble"));
                    searchStatusLabel.Append(" <strong>");
                    searchStatusLabel.Append(_searchField.SelectedItem.Text.Trim());
                    searchStatusLabel.Append("</strong> ");
                    searchStatusLabel.Append(WebTextManager.GetText("/controlText/SearchControl.ascx/FieldSearchFor"));
                    searchStatusLabel.Append(" <strong>");
                    searchStatusLabel.Append(_searchTerm.Text.Trim());
                    searchStatusLabel.Append("</strong>");

                    args = new SearchEventArgs(_searchTerm.Text.Trim(), _searchField.SelectedValue.Trim());
                }
                else
                {
                    searchStatusLabel.Append(WebTextManager.GetText("/controlText/SearchControl.ascx/FieldSearchPreamble"));
                    searchStatusLabel.Append(" ");
                    searchStatusLabel.Append(WebTextManager.GetText("/controlText/SearchControl.ascx/FieldSearchFor"));
                    searchStatusLabel.Append(" <strong>");
                    searchStatusLabel.Append(_searchTerm.Text.Trim());
                    searchStatusLabel.Append("</strong>");

                    args = new SearchEventArgs(_searchTerm.Text.Trim());
                }

                _appliedSearchLabel.Text = searchStatusLabel.ToString();
                _appliedSearchPanel.Visible = true;

                OnSearched(args);
            }
            else
                OnSearchCleared(new EventArgs());
        }

        /// <summary>
        /// Handles the click event of the clear search button
        /// - Hides the search status panel
        /// - Raises the clearsearch event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ClearSearchButton_Click(object sender, EventArgs e)
        {
            _appliedSearchPanel.Visible = false;
            _searchTerm.Text = string.Empty;

            OnSearchCleared(e);
        }

        /// <summary>
        /// Raises the search event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSearched(SearchEventArgs e)
        {
            if (Search != null)
            {
                Search(this, e);
            }
        }

        /// <summary>
        /// Raises the search cleared event
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSearchCleared(EventArgs e)
        {
            if (ClearSearch != null)
            {
                ClearSearch(this, e);
            }
        }

        #endregion
    }

    /// <summary>
    /// Search event args
    /// </summary>
    public class SearchEventArgs : EventArgs
    {
		string _term;

        /// <summary>
        /// 
        /// </summary>
        public string SearchTerm 
		{ 
			get {return _term;}
			set { _term = SafeSqlLiteral(value); } 
		}

        /// <summary>
        /// 
        /// </summary>
        public string SearchField { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        public SearchEventArgs(String searchTerm) : this(searchTerm, null){ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="searchField"></param>
        public SearchEventArgs(String searchTerm, String searchField)
        {
            SearchTerm = searchTerm;
            SearchField = searchField;
        }

		private string SafeSqlLiteral(string inputSQL)
		{
			return inputSQL.Replace("'", "''");
		}


    }
}