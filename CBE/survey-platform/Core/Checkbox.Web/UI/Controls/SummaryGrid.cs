using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Analytics.Items.UI;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Summary grid item for reports
    /// </summary>
    public class SummaryGrid : CompositeControl
    {
        private GridView _gridView;
        private readonly Label _summaryTitle;

        private readonly Dictionary<string, Int32> _columnGroups;
        private readonly Dictionary<string, Int32> _rowGroups;

        private AnalysisItemAppearanceData _appearance;

        /// <summary>
        /// Constructor
        /// </summary>
        public SummaryGrid()
        {
            InitializeGrid();
            _columnGroups = new Dictionary<string, Int32>();
            _rowGroups = new Dictionary<string, int>();
            _summaryTitle = new Label();

            ShowTotalInFooter = true;
            ShowAverageInFooter = false;
            ShowHeader = true;
            ShowFooter = true;
        }

        /// <summary>
        /// Add a column grouping
        /// </summary>
        /// <param name="groupText"></param>
        /// <param name="columnSpan"></param>
        public virtual void AddColumnGroup(string groupText, Int32 columnSpan)
        {
            _columnGroups[groupText] = columnSpan;
        }

        /// <summary>
        /// Add a row group
        /// </summary>
        /// <param name="groupText"></param>
        /// <param name="rowPos"></param>
        public virtual void AddRowGroup(string groupText, Int32 rowPos)
        {
            _rowGroups[groupText] = rowPos;
        }

        /// <summary>
        /// Get/set the analysis item appearance
        /// </summary>
        public virtual AnalysisItemAppearanceData Appearance
        {
            get { return _appearance; }
            set { _appearance = value; }
        }

        /// <summary>
        /// Show an average value in the footer
        /// </summary>
        public virtual bool ShowAverageInFooter { get; set; }

        /// <summary>
        /// Show a total in the footer
        /// </summary>
        public virtual bool ShowTotalInFooter { get; set; }

        /// <summary>
        /// Get/set whether to show the grid footer
        /// </summary>
        public virtual bool ShowFooter
        {
            get { return _gridView.ShowFooter; }
            set { _gridView.ShowFooter = value; }
        }

        /// <summary>
        /// Get/set whether to show the grid header
        /// </summary>
        public virtual bool ShowHeader
        {
            get { return _gridView.ShowHeader; }
            set { _gridView.ShowHeader = value; }
        }

        /// <summary>
        /// Initialize the data grid
        /// </summary>
        protected virtual void InitializeGrid()
        {
            _gridView = new GridView
            {
                SkinID = "CrossTabGrid",
                AutoGenerateColumns = false,
                ShowFooter = false,
                ShowHeader = true,
                Width = Unit.Percentage(100)
            };

            _gridView.CssClass = "RadGrid_Checkbox";
            _gridView.HeaderStyle.CssClass = "rgHeader";
            _gridView.FooterStyle.CssClass = "rgFooter";
            _gridView.AlternatingRowStyle.CssClass = "rgAltRow";
            _gridView.RowStyle.CssClass = "rgRow";

            _gridView.DataBound += _gridView_DataBound;
        }

        /// <summary>
        /// Get/set the grid data source
        /// </summary>
        public virtual object DataSource
        {
            get { return _gridView.DataSource; }
            set { _gridView.DataSource = value; }
        }

        /// <summary>
        /// Bind data
        /// </summary>
        public override void DataBind()
        {
            EnsureChildControls();

            _gridView.ShowFooter = ShowFooter;
            _gridView.ShowHeader = ShowHeader;
            _gridView.DataBind();
        }

        /// <summary>
        /// Get/set whether to show a title for the summary
        /// </summary>
        public virtual bool ShowSummaryTitle { get; set; }

        /// <summary>
        /// Get/set the summary title
        /// </summary>
        public virtual string SummaryTitle
        {
            get { return _summaryTitle.Text; }
            set
            {
                _summaryTitle.Text = value;

                if (_summaryTitle.Text != null)
                {
                    _summaryTitle.Text = _summaryTitle.Text.Replace(Environment.NewLine, "<br />");
                }
            }
        }

        /// <summary>
        /// Clear the columns of the grid
        /// </summary>
        public virtual void ClearColumns()
        {
            _gridView.Columns.Clear();
        }

        /// <summary>
        /// Add a bound column to the grid
        /// </summary>
        /// <param name="dataField"></param>
        /// <param name="headerText"></param>
        /// <param name="headerAlign"></param>
        /// <param name="itemAlign"></param>
        /// <param name="textFormat"></param>
        public virtual void AddBoundColumn(string dataField, string headerText, string textFormat, HorizontalAlign itemAlign, HorizontalAlign headerAlign)
        {
            BoundField field = new BoundField
            {
                DataField = dataField,
                HeaderText = headerText,
                HtmlEncode = false,
                DataFormatString = textFormat
            };

            field.ItemStyle.HorizontalAlign = itemAlign;
            field.HeaderStyle.HorizontalAlign = headerAlign;
            field.HtmlEncode = false;

            _gridView.Columns.Add(field);
        }

        /// <summary>
        /// Create child controls for the grid
        /// </summary>
        protected override void CreateChildControls()
        {
            //Container table
            Table t = new Table { CellSpacing = 1, CellPadding = 2, EnableViewState = false };

            //Header row
            TableRow headerRow = new TableRow {};
            TableCell headerCell = new TableCell { HorizontalAlign = HorizontalAlign.Center };
            headerCell.Controls.Add(_summaryTitle);
            headerRow.Cells.Add(headerCell);
            t.Rows.Add(headerRow);

            //Grid Row
            TableRow gridRow = new TableRow();
            TableCell gridCell = new TableCell();

            gridCell.Controls.Add(_gridView);
            gridRow.Cells.Add(gridCell);
            t.Rows.Add(gridRow);

            Controls.Add(t);

            //Bind appearance
            if (_appearance != null)
            {
                t.Width = Unit.Pixel(_appearance.Width);

                _summaryTitle.Visible = _appearance.ShowTitle;

                if (!string.IsNullOrEmpty(_appearance.TitleColor))
                {
                    _summaryTitle.Style[HtmlTextWriterStyle.Color] = _appearance.TitleColor;
                }

                if (!string.IsNullOrEmpty(_appearance.TitleFont))
                {
                    _summaryTitle.Style[HtmlTextWriterStyle.FontFamily] = _appearance.TitleFont;
                }
            }
        }

        /// <summary>
        /// Add any additional rows after databinding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _gridView_DataBound(object sender, EventArgs e)
        {
            if (_gridView.Controls.Count <= 0)
            {
                return;
            }

            if (_gridView.Controls[0] is Table)
            {
                Table table = (Table)_gridView.Controls[0];

                Int32 rowsAdded = 0;

                //Add column groups
                if (_columnGroups.Count > 0)
                {
                    GridViewRow groupingHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);

                    //Add a blank cell for the first cell
                    TableHeaderCell th = new TableHeaderCell();
                    groupingHeader.Cells.Add(th);
                    groupingHeader.Cells.Add(th);

                    //Add the grouping cells
                    foreach (string key in _columnGroups.Keys)
                    {
                        TableHeaderCell headerCell = new TableHeaderCell
                        {
                            ColumnSpan = _columnGroups[key],
                            Text = key,
                            HorizontalAlign = HorizontalAlign.Center
                        };

                        groupingHeader.Cells.Add(headerCell);
                    }

                    table.Rows.AddAt(0, groupingHeader);
                    rowsAdded++;
                }

                //Add row groupings
                if (_rowGroups.Count > 0)
                {
                    foreach (string key in _rowGroups.Keys)
                    {
                        GridViewRow groupingHeader = new GridViewRow(0, -1, DataControlRowType.Separator, DataControlRowState.Normal);

                        //Add the text
                        TableCell rowSeparator = new TableCell
                        {
                            Text = key,
                            ColumnSpan = 1,
                        };

                        groupingHeader.Cells.Add(rowSeparator);


                        //Add a blank cell for the rest of the matrix
                        TableCell th = new TableCell
                        {
                            ColumnSpan = (_gridView.Columns.Count - 1)
                        };

                        groupingHeader.Cells.Add(th);
                        groupingHeader.Cells.Add(th);


                        int rowPos = rowsAdded;

                        if (_gridView.ShowHeader)
                        {
                            rowPos++;
                        }
                        #region Modified
                        //if (_gridView.Rows.Count >= rowPos)
                        //{
                        table.Rows.AddAt(1 + rowsAdded + _rowGroups[key], groupingHeader);
                        rowsAdded++;
                        //}
                        #endregion
                    }
                }

                //Add total
                if (ShowTotalInFooter)
                {
                    //Find the maximum number of columns in any given row
                    int maxCells = 0;
                    foreach (TableRow row in table.Rows)
                    {
                        if (row.Cells.Count > maxCells)
                        {
                            maxCells = row.Cells.Count;
                        }
                    }

                    if (maxCells > 2)
                    {
                        try
                        {
                            GridViewRow footerRow = new GridViewRow(0, -1, DataControlRowType.Footer, DataControlRowState.Normal);
                            footerRow.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");

                            //Now add the footer
                            TableCell cell1 = new TableCell
                            {
                                HorizontalAlign = HorizontalAlign.Right,
                                Text = WebTextManager.GetText("/controlText/summaryTable/totalResponses"),
                                ColumnSpan = (maxCells - 2)
                            };
                            footerRow.Cells.Add(cell1);

                            TableCell cell2 = new TableCell
                            {
                                HorizontalAlign = HorizontalAlign.Right,
                                Text = GetTotalAnswerCount().ToString()
                            };
                            footerRow.Cells.Add(cell2);

                            string formatString = "{0:F" + _appearance.Precision + "}";

                            TableCell cell3 = new TableCell
                            {
                                HorizontalAlign = HorizontalAlign.Right,
                                Text = String.Format(formatString, 100.0)
                            };

                            footerRow.Cells.Add(cell3);

                            table.Rows.Add(footerRow);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the total answer count from the data
        /// </summary>
        /// <returns></returns>
        private int GetTotalAnswerCount()
        {
            if (_gridView != null && _gridView.DataSource != null && _gridView.DataSource is DataTable)
            {
                DataTable dataSource = (DataTable)_gridView.DataSource;

                string countColumn = string.Empty;

                foreach (BoundField column in _gridView.Columns)
                {
                    if (dataSource.Columns.Contains(column.DataField) && dataSource.Columns[column.DataField].DataType == typeof(Int32))
                    {
                        countColumn = column.DataField;
                        break;
                    }
                }

                if (countColumn != string.Empty)
                {
                    return Convert.ToInt32(dataSource.Compute("SUM(" + countColumn + ")", null));
                }
            }

            return 0;
        }
    }
}
