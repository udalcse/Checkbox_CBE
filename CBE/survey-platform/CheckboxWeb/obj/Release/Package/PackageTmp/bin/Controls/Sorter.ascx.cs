using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Controls
{
    #region SortOptions
    /// <summary>
    /// This class describes an option of a sortList
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// Get/set field for sorting
        /// </summary>
        public String SortField { get; set; }

        /// <summary>
        /// Get/set the identifier for the localized text string
        /// </summary>
        public String TextId { get; set; }

        /// <summary>
        /// Get/set the text for the option
        /// </summary>
        public String Text { get; set; }
    }

    public class SortOptionCollection : List<SortOption>
    {

    }

    #endregion


    /// <summary>
    /// Sorter control for the grid.
    /// </summary>
    [ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Options")]
    public partial class Sorter : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/set control's text
        /// </summary>
        public String Text { get; set; }


        /// <summary>
        /// Determine if the sorting is ascending.
        /// </summary>
        public bool Ascending
        {
            get { return _down.Attributes["style"] != null && _down.Attributes["style"].Contains("display:none"); }
            set
            {
                if (value)
                {
                    _up.Attributes.Remove("style");
                    _down.Attributes["style"] = "display:none;";
                }
                else
                {
                    _up.Attributes["style"] = "display:none";
                    _down.Attributes.Remove("style");
                }
            }
        }

        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public SortOptionCollection Options { get; set; }

        protected Grid AssociatedGrid { get; set; }


        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="grid"></param>
        public void Initialize(Grid grid)
        {
            AssociatedGrid = grid;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _textLabel.Text = WebTextManager.GetText("/controlText/sorter/sortBy");

            PopulateSortOptions();
        }


        private void PopulateSortOptions()
        {
            _listOptions.Items.Clear();

            String optiontext;

            foreach (SortOption sortOption in Options)
            {
                optiontext = WebTextManager.GetText(sortOption.TextId, null, sortOption.Text);
                _listOptions.Items.Add(new ListItem(optiontext, sortOption.SortField));
            }
        }

    }
}