using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Users;

namespace CheckboxWeb.Users.Controls
{
    public partial class MatrixGrid : Checkbox.Web.Common.UserControlBase
    {
        public MatrixField Matrix { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMatrix();
            }
        }

        /// <summary>
        /// Initializes the specified matrix cells.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        public void Initialize(MatrixField matrix)
        {
            this.Matrix = matrix;
        }


        public void BindMatrix()
        {
            _matrixField.Columns.Clear();


            for (int i = 0; i < Matrix.ColumnHeaders.Count; i++)
            {
                //if we don't have title for header , set it as index 
                var headerTitle = string.IsNullOrEmpty(Matrix.ColumnHeaders[i].Data) ?  i.ToString() : Matrix.ColumnHeaders[i].Data;

                ITemplate itemTemplate;

                if (!Matrix.IsColumnsFixed && i != 0)
                    itemTemplate = new GridViewTemplate(ListItemType.Header, headerTitle, true, this.Matrix.FieldName);
                else
                    itemTemplate = new GridViewTemplate(ListItemType.Header, headerTitle);


                TemplateField field = new TemplateField
                {
                    HeaderTemplate = itemTemplate,
                    ItemTemplate = new GridViewTemplate(ListItemType.Item, headerTitle)
                };

                _matrixField.Columns.Add(field);
            }

            //disable headers if grid is without them 
            if (!Matrix.HasHeaders)
                _matrixField.ShowHeader = false;

            if (!Matrix.IsRowsFixed)
            {
                _matrixField.Columns.Add(new CommandField()
                {
                    CausesValidation = false,
                    HeaderText = @"Actions",
                    HeaderStyle = { Width = 50}
                });
            }

            if (this.Matrix.RowsHeaders.Any(item => !string.IsNullOrEmpty(item.Data)))
            {
                TemplateField field = new TemplateField
                {
                    HeaderTemplate = new GridViewTemplate(ListItemType.Header, "Headers")
                };

                _matrixField.Columns.Insert(0, field);
            }

            var table = GenerateTable(Matrix.Cells);

            _matrixField.ID += "_" + this.Matrix.FieldName;
            _matrixField.Attributes.Add("field-name",this.Matrix.FieldName);
            _matrixField.Attributes.Add("has-rows-headers", this.Matrix.HasRowHeaders.ToString());
            _matrixField.Attributes.Add("has-column-headers", this.Matrix.HasHeaders.ToString());
            
            _matrixField.DataSource = table;
            
            //_matrixField.Width = 300;
            _matrixField.DataBind();

            if (!this.Matrix.IsRowsFixed)
                BindAddRowTable(this.Matrix.ColumnCount);


            if (!this.Matrix.IsColumnsFixed)
            {
                _deleteColumn.Visible = true;
                _addColumnBtn.Visible = true;
                _addColumnBtn.Attributes.Add("table-id", _matrixField.ClientID);
                _deleteColumn.Attributes.Add("table-id", _matrixField.ClientID);
                if (Matrix.ColumnCount == 1)
                    _deleteColumn.Attributes.Add("disabled","disabled");
                else
                    _deleteColumn.Attributes.Remove("disabled");
            }

            _rowsCountTxt.Text = this.Matrix.RowsCount.ToString();
            _columnsCountTxt.Text = this.Matrix.ColumnCount.ToString();

            _rowsCountTxt.ID += this.Matrix.FieldName;
            _columnsCountTxt.ID += this.Matrix.FieldName;

            _rowsCountTxt.Style.Add("visibility", "hidden");
            _columnsCountTxt.Style.Add("visibility", "hidden");

            _matrixField.Attributes.Add("rows-count-txt-id", _rowsCountTxt.ClientID);
            _matrixField.Attributes.Add("columns-count-txt-id", _columnsCountTxt.ClientID);
        }


        private DataTable GenerateTable(List<Cell> cells)
        {
            DataTable dataTable = new DataTable();
            var headerCells = cells.Where(item => item.IsHeader).ToList();
            var rowsHeaders = cells.Where(item => item.IsRowHeader).ToList();

            cells = cells.Except(headerCells).Except(rowsHeaders).ToList();

            for (int i = 0; i < this.Matrix.ColumnCount; i++)
            {
                if (headerCells.Any() && !string.IsNullOrEmpty(headerCells[i].Data))
                    dataTable.Columns.Add(new DataColumn(headerCells[i].Data, typeof (string)));
                else
                    dataTable.Columns.Add(new DataColumn(i.ToString()) );
            }
            

            for (int i = 0; i < this.Matrix.RowsCount; i++)
                dataTable.Rows.Add(dataTable.NewRow());


            //fill with values
            for (int rowIndex = 0; rowIndex < this.Matrix.RowsCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < this.Matrix.ColumnCount; columnIndex++)
                {
                    var currentCell =
                        cells.FirstOrDefault(item => item.RowNumber == rowIndex && item.ColumnNumber == columnIndex);

                    if (currentCell != null)
                        dataTable.Rows[rowIndex][columnIndex] = currentCell.Data;
                }
            }

            return dataTable;
        }

        private void BindAddRowTable(int columnCount)
        {
            for (int index = 0; index <= columnCount ; index++)
            {
                TableCell cell = new TableCell();

                if (columnCount == index)
                {
                    var btn = new Button
                    {
                        ID = "btnAdd",
                        Text = "Add",
                        CssClass = "add-row-btn ckbxButton silverButton",
                        OnClientClick = "return false;",
                        Width =  54 
                    };
                    
                    btn.Attributes.Add("table-id", _matrixField.ClientID);

                    cell.Controls.Add(btn);
                   

                    _matrixTable.Rows[0].Cells.Add(cell);
                }
                else
                {
                    TextBox textBox = new TextBox { ID = "txt" + index, Width = 120 , MaxLength = 400};
                    cell.Controls.Add(textBox);
                    _matrixTable.Rows[0].Cells.Add(cell);
                }
            }

            if (this.Matrix.HasRowHeaders)
            {
                TableCell cell = new TableCell();
                TextBox textBox = new TextBox
                {
                    ID = "txtRowHeader",
                    Width = this.Matrix.HasRowHeaders ? 96 : 51,
                    MaxLength = 400
                };

                cell.Controls.Add(textBox);
                _matrixTable.Rows[0].Cells.AddAt(0,cell);
            }

            _matrixTable.Visible = true;
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // if we can add rows and it is not first row
                if (!Matrix.IsRowsFixed && e.Row.RowIndex != 0  )
                {

                    LinkButton btn = new LinkButton()
                    {
                        CssClass = "remove-row-btn",
                        Text = "Remove",
                        OnClientClick = "return false"
                    };

                    btn.Attributes.Add("table-id", _matrixField.ClientID);

                    e.Row.Cells[e.Row.Cells.Count - 1].Controls.Add(btn);
                }

                if (this.Matrix.HasRowHeaders)
                {
                    if (this.Matrix.RowsHeaders.Count > e.Row.RowIndex)
                    {
                        var rowsHeaders = this.Matrix.Cells.Where(item => item.IsRowHeader).ToList();
                        Label label = new Label() { Text = rowsHeaders[e.Row.RowIndex].Data };
                        label.Style.Add("min-width", "100px;");

                        if (!this.Matrix.IsRowsFixed)
                        {

                            // we don't need to add custom row header as first header is static 
                            if (e.Row.RowIndex != 0 )
                            {
                                HiddenField textBox = new HiddenField
                                {
                                    Value = rowsHeaders[e.Row.RowIndex].Data,
                                    ID = this.Matrix.FieldName + "_RowHeader_" + e.Row.RowIndex
                                };
                                e.Row.Cells[0].Controls.Add(textBox);
                            }
                            
                        }
                        
                        e.Row.Cells[0].Controls.Add(label);
                       
                    }
                }
            }
        }
    }

    public class GridViewTemplate : ITemplate
    {
        private ListItemType _templateType;

        private string _columnName;

        private bool _hiddenInputToHeader;

        private string _matrixFieldName;

        public GridViewTemplate(ListItemType type, string colname, bool hiddenInputToHeader = false, string fieldName = "")
        {
            _templateType = type;
            _columnName = colname;
            _hiddenInputToHeader = hiddenInputToHeader;
            _matrixFieldName = fieldName;
        }

        public void InstantiateIn(Control container)
        {
            switch (_templateType)
            {
                case ListItemType.Header:
                    Label label = new Label {Text = _columnName};

                    HiddenField hiddenField = new HiddenField
                    {
                        Value = _columnName,
                        ID = _matrixFieldName + "_ColumnHeader_" + _columnName
                    };

                    if (_hiddenInputToHeader)
                        container.Controls.Add(hiddenField);

                    
                    container.Controls.Add(label);
                    break;

                case ListItemType.Item:
                    TextBox textBox = new TextBox();
                    textBox.Width = 120;
                    textBox.DataBinding += new EventHandler(textBox_DataBinding);
                    textBox.Columns = 2;
                    textBox.MaxLength = 400;
                    container.Controls.Add(textBox);
                    break;
                case ListItemType.EditItem:
                    break;
                case ListItemType.Footer:
                    CheckBox chkColumn = new CheckBox();
                    chkColumn.ID = "Chk" + _columnName;
                    container.Controls.Add(chkColumn);
                    break;
            }
        }

        public void textBox_DataBinding(object sender, EventArgs args)
        {
            TextBox txtdata = (TextBox) sender;
            GridViewRow container = (GridViewRow) txtdata.NamingContainer;

            try
            {
                object dataValue = DataBinder.Eval(container.DataItem, _columnName);
                if (dataValue != DBNull.Value)
                {
                    txtdata.Text = dataValue.ToString();
                }
            }
            catch (Exception e)
            {
                var dataRow = ((DataRowView)container.DataItem);
                var listIndex = dataRow.DataView.Table.Columns.IndexOf(_columnName);
                txtdata.Text = dataRow.Row[listIndex].ToString();
            }
        }
    }
}