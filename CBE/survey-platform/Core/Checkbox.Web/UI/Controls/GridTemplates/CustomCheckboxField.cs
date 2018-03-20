using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Checkbox.Common;

namespace Checkbox.Web.UI.Controls.GridTemplates
{
    /// <summary>
    /// Custom field for adding checkboxes to a grid
    /// </summary>
    public class CustomCheckBoxField : CheckBoxField
    {
        private readonly List<CheckBox> _checkBoxes;
        private CheckBox _selectAllCheck;

        /// <summary>
        /// Constructor
        /// </summary>
        public CustomCheckBoxField()
        {
            _checkBoxes = new List<CheckBox>();
            AllowSelectAll = true;
        }


        /// <summary>
        /// Get/set indexes of the checkboxes, which is selected
        /// </summary>
        public List<int> InitialSelectionIndexes { get; set; }


        /// <summary>
        /// Get/set whether to allow select all
        /// </summary>
        public bool AllowSelectAll { get; set; }

        /// <summary>
        /// Determine if this row is checked.
        /// </summary>
        public String Checked { get; set; }

        /// <summary>
        /// Get/set the text id for the header text
        /// </summary>
        public string HeaderTextID { get; set; }

        /// <summary>
        /// Get/set jscript handle 
        /// </summary>
        public string OnClientClick { get; set; }

        /// <summary>
        /// Css which will be used by jQuery to select/deselect all checkboxes.
        /// </summary>
        public static string CheckboxesCss
        {
            get { return "_selectCheckBox"; }
        }


        /// <summary>
        /// This method is called by the ExtractRowValues methods of 
        /// GridView and DetailsView. Retrieve the current value of the 
        /// cell from the Checked state of the Radio button.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="cell"></param>
        /// <param name="rowState"></param>
        /// <param name="includeReadOnly"></param>
        public override void ExtractValuesFromCell(IOrderedDictionary dictionary,
                                                   DataControlFieldCell cell,
                                                   DataControlRowState rowState,
                                                   bool includeReadOnly)
        {
            // Determine whether the cell contains a CheckBox 
            // in its Controls collection.
            if (cell.Controls.Count > 0)
            {
                CheckBox check = cell.Controls[0] as CheckBox;

                object checkedValue;

                if (null == check)
                {
                    // A CheckBox is expected, but a null is encountered.
                    // Add error handling.
                    throw new InvalidOperationException
                        ("CustomCheckBoxField could not extract control.");
                }

                checkedValue = check.Checked;


                // Add the value of the Checked attribute of the
                // CheckBox to the dictionary.
                if (dictionary.Contains(DataField))
                    dictionary[DataField] = checkedValue;
                else
                    dictionary.Add(DataField, checkedValue);
            }
        }

        /// <summary>
        /// Initialize the cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="cellType"></param>
        /// <param name="rowState"></param>
        /// <param name="rowIndex"></param>
        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);

            if (cellType == DataControlCellType.Header)
            {
                if (cell != null)
                {
                    Panel _textPanel = new Panel();
                    _textPanel.Style["float"] = "left";

                    Panel _checkboxPanel = new Panel();
                    _checkboxPanel.Style["float"] = "right";

                    Label headerTextLbl = new Label();
                    headerTextLbl.DataBinding += headerTextLbl_DataBinding;

                    _selectAllCheck = new CheckBox { Checked = false };
                    _selectAllCheck.Attributes.Add("onclick", OnClientClick+"(this);");

                    _checkboxPanel.Visible = AllowSelectAll;

                    _textPanel.Controls.Add(headerTextLbl);
                    _checkboxPanel.Controls.Add(_selectAllCheck);

                    cell.Controls.Add(_textPanel);
                    cell.Controls.Add(_checkboxPanel);
                }                
            }
        }

        /// <summary>
        /// Handle binding to set header text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void headerTextLbl_DataBinding(object sender, EventArgs e)
        {
            if (Utilities.IsNotNullOrEmpty(HeaderTextID))
            {
                ((Label)sender).Text = WebTextManager.GetText(HeaderTextID);
            }
            else if (Utilities.IsNotNullOrEmpty(HeaderText))
            {
                ((Label)sender).Text = HeaderText;
            }
        }


        /// <summary>
        /// This method adds a RadioButton control and any other 
        /// content to the cell's Controls collection.
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="rowState"></param>
        protected override void InitializeDataCell
            (DataControlFieldCell cell, DataControlRowState rowState)
        {
            CheckBox check = new CheckBox();

            // If the RadioButton is bound to a DataField, add
            // the OnDataBindingField method event handler to the
            // DataBinding event.
            if (DataField.Length != 0)
            {
                check.DataBinding += check_DataBinding;
            }

            check.Text = Text;
            check.Enabled = true;
            check.CssClass = CheckboxesCss;
            
            if (ItemStyle.HorizontalAlign != HorizontalAlign.NotSet)
                cell.Attributes.Add("align",ItemStyle.HorizontalAlign.ToString());
            if (!ItemStyle.Width.IsEmpty)
                cell.Attributes.Add("width", ItemStyle.Width.ToString());

            cell.Controls.Add(check);
            _checkBoxes.Add(check);
        }

        void check_DataBinding(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            check.Checked = (bool)GetValue(check.NamingContainer);            
        }     

        /// <summary>
        /// Get the selected indexes
        /// </summary>
        public ReadOnlyCollection<Int32> SelectedIndexes
        {
            get
            {
                List<Int32> selectedIndexes = new List<int>();

                for (int i = 0; i < _checkBoxes.Count; i++)
                {
                    if (_checkBoxes[i].Checked)
                    {
                        selectedIndexes.Add(i);
                        
                    }
                }

                return new ReadOnlyCollection<int>(selectedIndexes);
            }
        }

        /// <summary>
        /// Uncheck item
        /// </summary>
        /// <param name="index"></param>
        public void Uncheck(int index)
        {
            _checkBoxes[index].Checked = false;
        }
    }
}
