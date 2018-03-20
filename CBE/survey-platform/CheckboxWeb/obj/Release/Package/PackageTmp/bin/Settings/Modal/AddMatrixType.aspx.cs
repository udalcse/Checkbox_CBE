using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Logging;

using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;



namespace CheckboxWeb.Settings.Modal
{
    public partial class AddMatrixType : SecuredPage
    {
        /// <summary>
        /// The header identifier
        /// </summary>
        private const string HeaderId = "header";

        private const int DefaultColumnWidth = 150;

        /// <summary>
        /// The header identifier
        /// </summary>
        private const string RowHeaderId = "headerRow";

        private List<int> columnSizes;

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        private string FieldName { get; set; }
        private DataTable dtMatrix { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            FieldName = ViewState["fieldName"]?.ToString() ?? Request["fieldName"];
            Master.OkBtn.Enabled = false;

            _currentFieldName.Value = this.FieldName;
            if (!IsPostBack)
            {

                GridStructureChangeEvent(sender, e);
                ViewState["Count"] = 1;
            }

        }

        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/settings/modal/addMatrixType");
            Master.OkClick += SaveButtonClickEventHandler;
            Master.OkBtn.OnClientClick = "return MatrixGridValidation()";

            //Master.CancelClick += CancelBtnClickHandler;
            _rowsCount.TextChanged += GridStructureChangeEvent;
            _columnCount.TextChanged += GridStructureChangeEvent;
            _headersCheckbox.CheckedChanged += GridStructureChangeEvent;
            _rowHeadersCheckbox.CheckedChanged += GridStructureChangeEvent;

            columnSizes = new List<int>();

            //Helper for uframe
            RegisterClientScriptInclude(
                "htmlparser.js",
                ResolveUrl("~/Resources/htmlparser.js"));

            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                "jquery.fileDownload.js",
                ResolveUrl("~/Resources/jquery.fileDownload.js"));
        }

        void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["fieldName"] = FieldName;
        }

        /// <summary>
        /// Grids the structure change event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GridStructureChangeEvent(object sender, EventArgs e)
        {
            int rows, columns;


            dtMatrix = GetMatrixInfo(FieldName);



            if (dtMatrix.Rows.Count > 0)
            {
                if (ViewState["Count"] == null)
                {
                    _rowsCount.Text = Convert.ToString(dtMatrix.Rows[0]["TotalRow"]);
                    _columnCount.Text = Convert.ToString(dtMatrix.Rows[0]["TotalColumn"]);
                    //_rowsCount.Text =rows;
                    _headersCheckbox.Checked = Convert.ToBoolean(dtMatrix.Rows[0]["ColumnHeader"]);
                   _rowHeadersCheckbox.Checked = Convert.ToBoolean(dtMatrix.Rows[0]["RowHeader"]);
                }
            }


            //else
            //{
            int.TryParse(_rowsCount.Text, out rows);
            int.TryParse(_columnCount.Text, out columns);
            //}



            if (rows >= 1 && columns == 0)
                columns = 1;

            if (rows == 0 && columns >= 1)
                rows = 1;

            var addColumnHeaders = _headersCheckbox.Checked;
            var addRowHeaders = _rowHeadersCheckbox.Checked;

            CreateDataSource(rows, columns, addColumnHeaders, addRowHeaders);

            if (rows >= 1 && columns >= 1)
                Master.OkBtn.Enabled = true;

            columnSizes = new List<int>();

            if (_colSizeRepeater.Items.Count == 0)
            {
                columnSizes.AddRange(Enumerable.Repeat(0, columns));
                if (addRowHeaders)
                    columnSizes.Add(0);
            }
            else
            {
                for (int i = 0; i < _colSizeRepeater.Items.Count; i++)
                {
                    if ((!addRowHeaders && i == columns) || (addRowHeaders && i == columns + 1)) break;
                    TextBox itemSize = (TextBox)_colSizeRepeater.Items[i].FindControl("_colSize");
                    int colSize = 0;
                    int.TryParse(itemSize.Text, out colSize);
                    columnSizes.Add(colSize);
                }
                if (!addRowHeaders && columnSizes.Count < columns)
                    columnSizes.AddRange(Enumerable.Repeat(0, columns - columnSizes.Count));
                else if (addRowHeaders && columnSizes.Count < columns + 1)
                    columnSizes.AddRange(Enumerable.Repeat(0, columns + 1 - columnSizes.Count));
            }

            _colSizeRepeater.DataSource = columnSizes;
            _colSizeRepeater.DataBind();
        }


        /// <summary>
        /// GetMatrixInfo
        /// </summary>
        /// <returns></returns>
        private DataTable GetMatrixInfo(string strFieldName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Setting_GetMatrix");
            command.AddInParameter("FieldName", DbType.String, strFieldName);

            DataSet dsMatrix = db.ExecuteDataSet(command);

            return dsMatrix.Tables[0];

        }



        /// <summary>
        /// Creates the data source for grid.
        /// </summary>
        /// <param name="rowsCount">The rows count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="addColumnHeaders">if set to <c>true</c> [add column headers].</param>
        /// <param name="addRowHeaders">if set to <c>true</c> [add row headers].</param>
        private void CreateDataSource(int rowsCount, int columnCount, bool addColumnHeaders, bool addRowHeaders)
        {
            matrixGrid.Columns.Clear();

            DataTable dataTable = new DataTable();

            // add row header column
            if (addRowHeaders)
                matrixGrid.Columns.Insert(0, new TemplateField());

            for (int i = 0; i < rowsCount; i++)
                dataTable.Rows.Add(dataTable.NewRow());

            //if(dtMatrix.Rows.Count>0)
            //{  
            //for (int i = 0; i < columnCount; i++)
            //    dataTable.Columns.Add(new DataColumn(Convert.ToString(dtMatrix.Rows[i]["Data"])));
            //}else
            //{
            for (int i = 0; i < columnCount; i++)
                dataTable.Columns.Add(new DataColumn());
            //}

            matrixGrid.ShowHeader = addColumnHeaders;
            matrixGrid.DataSource = dataTable;
            matrixGrid.Width = 210;

            matrixGrid.DataBind();

            //since header row is not editable we need to add textbox to be able to save header values
            if (matrixGrid.HeaderRow != null && _headersCheckbox.Checked)
            {
                matrixGrid.HeaderRow.Cells.Clear();


                if (addRowHeaders)
                {
                    Label label = new Label
                    {
                        Text = "Headers",
                        Width = 100
                    };

                    TableCell rowHeaderCell = new TableCell();
                    rowHeaderCell.Controls.Add(label);
                    matrixGrid.HeaderRow.Cells.Add(rowHeaderCell);
                }

                if (dtMatrix.Rows.Count > 0)
                {
                    for (int index = 0; index < columnCount; index++)
                    {
                        TableCell tableCell = new TableCell();
                        TextBox textBox = new TextBox { ID = "header" + (index) };
                        if (index < Convert.ToInt32(dtMatrix.Rows[0]["TotalColumn"]))
                        {
                            textBox.Attributes.Add("value", $"{dtMatrix.Rows[index]["Data"]}");
                        }
                        else
                        {
                            textBox.Attributes.Add("placeholder", $"Column Header{index}");
                        }

                        textBox.MaxLength = 400;
                        textBox.Font.Bold = true;

                        tableCell.Controls.Add(textBox);
                        matrixGrid.HeaderRow.Cells.Add(tableCell);
                    }
                }


                else
                {
                    for (int index = 1; index <= columnCount; index++)
                    {
                        TableCell tableCell = new TableCell();
                        TextBox textBox = new TextBox { ID = "header" + (index - 1) };
                        textBox.Attributes.Add("placeholder", $"Column Header{index}");
                        textBox.MaxLength = 400;
                        textBox.Font.Bold = true;

                        tableCell.Controls.Add(textBox);
                        matrixGrid.HeaderRow.Cells.Add(tableCell);
                    }
                }


            }
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow[] drRowsData = dtMatrix.Select("IsRowHeader=1");
                if (_rowHeadersCheckbox.Checked)
                {
                    TextBox textBox = new TextBox { ID = "headerRow" + e.Row.RowIndex };

                    if (e.Row.RowIndex < drRowsData.Count())
                    {
                        textBox.Attributes.Add("value", $"{drRowsData[e.Row.RowIndex]["Data"]}");
                    }
                    else
                    {
                        textBox.Attributes.Add("placeholder", $"Row Header{e.Row.RowIndex + 1}");
                    }
                    textBox.Font.Bold = true;
                    textBox.MaxLength = 400;

                    e.Row.Cells[0].Controls.Add(textBox);

                    //set css styles for cells if only rows headers are enabled
                    if (_rowHeadersCheckbox.Checked && !_headersCheckbox.Checked)
                    {
                        for (int i = 1; i < e.Row.Cells.Count; i++)
                            e.Row.Cells[i].CssClass = "add-matrix-table-cell";
                    }
                }
            }
        }

        /// <summary>
        /// Saves button click event handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveButtonClickEventHandler(object sender, EventArgs e)
        {
            int rows, columns;

            int.TryParse(_rowsCount.Text, out rows);
            int.TryParse(_columnCount.Text, out columns);

            MatrixField matrixField = new MatrixField();

            if (rows >= 1 && columns == 0)
            {
                columns = 1;
                matrixField.IsColumnsFixed = false;
            }

            if (rows == 0 && columns >= 1)
            {
                rows = 1;
                matrixField.IsRowsFixed = false;
            }

            var headerValues = new List<string>();
            var rowHeaderValues = new List<string>();

            for (int index = 0; index < columns; index++)
            {
                var headerValue = WebUtilities.GetRequestFormValueBySubstring(HeaderId + index);
                if (headerValue != null)
                    headerValues.Add(headerValue);
            }

            for (int index = 0; index < rows; index++)
            {
                var rowHeaderValue = WebUtilities.GetRequestFormValueBySubstring(RowHeaderId + index);
                if (rowHeaderValue != null)
                    rowHeaderValues.Add(rowHeaderValue);
            }

            if (!string.IsNullOrEmpty(FieldName))
            {
                columnSizes = new List<int>();
                foreach (RepeaterItem item in _colSizeRepeater.Items)
                {
                    TextBox itemSize = (TextBox)item.FindControl("_colSize");
                    int colSize = 0;
                    int.TryParse(itemSize.Text, out colSize);
                    columnSizes.Add(colSize);
                }

                matrixField.FieldName = FieldName;
                matrixField.Cells = BuildMatrixCells(headerValues, rowHeaderValues, rows, columns);
                matrixField.GridLines = _gridLinesList.SelectedValue;

                ProfileManager.AddMatrixField(matrixField);

                //Close popup window
                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "Redirect",
                    "closeWindowModal('', null);",
                    true);
            }
        }


        public void CancelBtnClickHandler(object sender, EventArgs e)
        {
            if (Page != null)
                Page.ClientScript.RegisterStartupScript(GetType(), "closeDialog", "$(document).ready(function () { closeWindow('',null);  showSettingsPage('test.aspx') }); ", true);
        }

        /// <summary>
        /// Builds matrix cells.
        /// </summary>
        /// <param name="headerValues">The header values.</param>
        /// <param name="rowHeaderValues">The row header values.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        private List<Cell> BuildMatrixCells(List<string> headerValues, List<string> rowHeaderValues, int rows,
            int columns)
        {
            List<Cell> cells = new List<Cell>();


            //----- generate headers -----//

            if (headerValues != null && headerValues.Any())
            {
                cells.AddRange(headerValues.Select((t, i) => new Cell() { RowNumber = 0, ColumnNumber = i, Data = t, IsHeader = true }));
            }

            if (rowHeaderValues != null && rowHeaderValues.Any())
            {
                cells.AddRange(rowHeaderValues.Select((t, i) => new Cell()
                {
                    RowNumber = i,
                    ColumnNumber = 0,
                    Data = t,
                    IsRowHeader = true
                }));
            }

            // if we don't have any valus for header we need to generate headers with empty values with empty data
            if (headerValues != null && !headerValues.Any())
            {
                for (int i = 0; i < columns; i++)
                    cells.Add(new Cell() { RowNumber = 0, ColumnNumber = i, Data = string.Empty, IsHeader = true });
            }

            if (rowHeaderValues != null && !rowHeaderValues.Any())
            {
                for (int i = 0; i < rows; i++)
                    cells.Add(new Cell() { RowNumber = i, ColumnNumber = 0, Data = string.Empty, IsRowHeader = true });
            }

            if (rowHeaderValues != null && rowHeaderValues.Any())
            {
                cells.ForEach(c =>
                {
                    if (c.IsHeader)
                        c.ColumnWidth = columnSizes[c.ColumnNumber + 1] != 0 ? columnSizes[c.ColumnNumber + 1] : DefaultColumnWidth;
                    else if (c.IsRowHeader)
                        c.ColumnWidth = columnSizes[0] != 0 ? columnSizes[0] : DefaultColumnWidth;
                });
            }
            else
            {
                cells.ForEach(c =>
                {
                    if (c.IsHeader)
                        c.ColumnWidth = columnSizes[c.ColumnNumber] != 0 ? columnSizes[c.ColumnNumber] : DefaultColumnWidth;
                });
            }


            //----- generate headers -----//

            return cells;
        }
    }
}