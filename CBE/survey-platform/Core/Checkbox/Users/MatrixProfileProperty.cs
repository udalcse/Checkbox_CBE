using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Users
{
    [Serializable]
    public class MatrixProfileProperty : ProfileProperty
    {
        public int RowsCount { get; set; }

        public int ColumnsCount { get; set; }

        public string GridLines { get; set; }

        public List<Cell> ColumnHeaders { get; set; }

        public List<Cell> RowHeaders { get; set; }

        public bool IsRowsFixed { get; set; }

        public bool IsColumnsFixed { get; set; }

        public MatrixProfileProperty(MatrixField matrixField)
        {
            RowsCount = matrixField.RowsCount;
            ColumnsCount = matrixField.ColumnCount;
            GridLines = matrixField.GridLines;
            RowHeaders = matrixField.RowsHeaders.Select(h => new Cell() { Name = h.Data, ColumnWidth = h.ColumnWidth }).ToList();
            ColumnHeaders = matrixField.ColumnHeaders.Select(h => new Cell(){ Name = h.Data, ColumnWidth = h.ColumnWidth }).ToList();
            Name = matrixField.FieldName;
            FieldType = CustomFieldType.Matrix;
            IsHidden = false;
            IsRowsFixed = matrixField.IsRowsFixed;
            IsColumnsFixed = matrixField.IsColumnsFixed;
        }

        public MatrixProfileProperty()
        {
        }

        /// <summary>
        /// Represents a cell in a matrix
        /// </summary>
        public class Cell
        {
            public string Name { get; set; }
            public int? ColumnWidth { get; set; }
        }
    }
}