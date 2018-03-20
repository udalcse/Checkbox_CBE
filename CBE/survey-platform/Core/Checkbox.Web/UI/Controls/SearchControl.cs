using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Management;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Summary description for SearchControl.
    /// </summary>
    public class SearchControl : Panel
    {
        private TextBox _searchString;
        private DropDownList _searchField;
        private ImageButton _go;
        private ImageButton _cancel;

        /// <summary>
        /// Search events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void SearchEventHandler(object sender, SearchEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public event SearchEventHandler OnSearch;

        /// <summary>
        /// 
        /// </summary>
        public event SearchEventHandler OnCancel;

        /// <summary>
        /// Adds field to search <see cref="DropDownList"/>
        /// </summary>
        /// <param name="textId">Text to be displayed in the <see cref="DropDownList"/></param>
        /// <param name="fieldValue">Value to be passed by the <see cref="DropDownList"/></param>
        public void AddField(string textId, string fieldValue)
        {
            EnsureChildControls();
            ListItem li = new ListItem(WebTextManager.GetText(textId), fieldValue);
            _searchField.Items.Add(li);
        }

        /// <summary>
        /// Gets or sets the string value of the search text
        /// </summary>
        public string SearchString
        {
            get
            {
                if (ViewState["SearchString"] != null)
                {
                    return (string)ViewState["SearchString"];
                }
                
                return null;
            }
            set
            {
                EnsureChildControls();
                ViewState["SearchString"] = value;
                _searchString.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the string value of the field to be searched
        /// </summary>
        public string Field
        {
            get
            {
                if (ViewState["SearchField"] != null)
                {
                    return (string)ViewState["SearchField"];
                }
                
                return null;
            }
            set
            {
                EnsureChildControls();

                //Make sure selected value exists in list of fields
                if (_searchField.Items.FindByValue(value) != null)
                {
                    ViewState["SearchField"] = value;
                    _searchField.SelectedValue = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the search field is visible
        /// </summary>
        public bool ShowField
        {
            get
            {
                if (ViewState["ShowField"] == null)
                {
                    ViewState["ShowField"] = true;
                }

                return (bool)ViewState["ShowField"];
            }
            set
            {
                ViewState["ShowField"] = value;

                EnsureChildControls();
                _searchField.Visible = value;
            }
        }

        /// <summary>
        /// Creates the controls that make up the search control
        /// </summary>
        protected override void CreateChildControls()
        {
            _searchString = new TextBox {Width = 200};
            _searchField = new DropDownList();
            _go = new ImageButton {ImageUrl = (ApplicationManager.ApplicationRoot + "/Images/search-go.gif")};
            _go.Click += Go_Click;
            _go.ID = "Go";
            _cancel = new ImageButton {ImageUrl = (ApplicationManager.ApplicationRoot + "/Images/search-cancel.gif")};
            _cancel.Click += Cancel_Click;

            Controls.Add(new LiteralControl("<table><tr><td>"));
            Controls.Add(_searchString);
            Controls.Add(new LiteralControl("</td><td>"));
            Controls.Add(_searchField);
            Controls.Add(new LiteralControl("</td><td>"));
            Controls.Add(_go);
            Controls.Add(new LiteralControl("</td><td>"));
            Controls.Add(_cancel);
            Controls.Add(new LiteralControl("</td></tr></table>"));

            _searchField.Visible = ShowField;

            base.CreateChildControls();

            DefaultButton = _go.ID;

            //if (Page != null)
                //Page.ClientScript.RegisterHiddenField("__EVENTTARGET", go.ClientID.Replace("_Go", ":Go"));
        }

        /// <summary>
        /// Accepts a click of the search button
        /// </summary>
        /// <param name="sender">The search button</param>
        /// <param name="e">Arguments for the image click event</param>
        private void Go_Click(object sender, ImageClickEventArgs e)
        {
            // set the viewstate with the search values
            SearchString = _searchString.Text;
            Field = _searchField.SelectedValue;

            // create a SearchEventArgs with the search values and fire OnSearch Event
            SearchEventArgs sa = new SearchEventArgs {
                SearchString = (ApplicationManager.AppSettings.AllowHTMLNames ? SearchString : Checkbox.Common.Utilities.SimpleHtmlEncode(SearchString)), 
                Field = Field};

            if (OnSearch != null)
            {
                OnSearch(this, sa);
            }
        }

        /// <summary>
        /// Accepts a click of the cancel button
        /// </summary>
        /// <param name="sender">The cancel button</param>
        /// <param name="e">Arguments for the image click event</param>
        private void Cancel_Click(object sender, ImageClickEventArgs e)
        {
            _searchString.Text = String.Empty;

            if (_searchField.Items.Count > 0)
            {
                _searchField.SelectedIndex = 0;
            }

            SearchString = null;
            Field = null;
            SearchEventArgs sa = new SearchEventArgs();

            if (OnCancel != null)
            {
                OnCancel(sender, sa);
            }
        }
    }

    /// <summary>
    /// Contains public methods to access the search string and search field
    /// </summary>
    public class SearchEventArgs
    {
        /// <summary>
        /// Get or set the string of the value to search for
        /// </summary>
        public string SearchString { get; set; }

        /// <summary>
        /// Get or set the string of the field to search on
        /// </summary>
        public string Field { get; set; }
    }
}