using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Matrix column description
    /// </summary>
    [DataContract]
    [Serializable]
    public class MatrixItemColumn
    {
        /// <summary>
        /// Get/set column number
        /// </summary>
        [DataMember]
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Get/set validation errors for column
        /// </summary>
        [DataMember]
        public string[] ValidationErrors { get; set; }

        /// <summary>
        /// Get/set whether column requires unique answers
        /// </summary>
        [DataMember]
        public bool RequireUniqueAnswers { get; set; }

        /// <summary>
        /// Get/set width of column
        /// </summary>
        [DataMember]
        public int? Width { get; set; }

        /// <summary>
        /// Get/set whether column requires answers
        /// </summary>
        [DataMember]
        public bool AnswerRequired { get; set; }

        /// <summary>
        /// Get/set rating scale texts associated with the column
        /// </summary>
        [DataMember]
        public string ScaleStartText { get; set; }

        [DataMember]
        public string ScaleMidText { get; set; }

        [DataMember]
        public string ScaleEndText { get; set; }

        /// <summary>
        /// Option texts, if any for column.
        /// </summary>
        [DataMember]
        public string[] OptionTexts { get; set; }

        /// <summary>
        /// Column type
        /// </summary>
        [DataMember]
        public string ColumnType { get; set; }

        /// <summary>
        /// Get/set id of column protoype item
        /// </summary>
        [DataMember]
        public int PrototypeItemId { get; set; }

        /// <summary>
        /// Column header text
        /// </summary>
        [DataMember]
        public string HeaderText { get; set; }

        /// <summary>
        /// Alias of the column, used in reporting
        /// </summary>
        [DataMember]
        public string Alias { get; set; }
    }

    /// <summary>
    /// Matrix row information
    /// </summary>
    [DataContract]
    [Serializable]
    public class MatrixItemRow
    {
        /// <summary>
        /// Get/set matrix row number
        /// </summary>
        [DataMember]
        public int RowNumber { get; set; }

        /// <summary>
        /// Get/set row's type
        /// </summary>
        [DataMember]
        public string RowType { get; set; }

        /// <summary>
        /// Get/set row text
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Get/set row alias
        /// </summary>
        [DataMember]
        public string Alias { get; set; }
    }

    /// <summary>
    /// Data container for matrix-specific data to be included with SurveyResponseItem objects.
    /// </summary>
    [Serializable]
    [DataContract]
    public class MatrixAdditionalData
    {
        /// <summary>
        /// Matrix columns listed in order.
        /// </summary>
        [DataMember]
        public MatrixItemColumn[] Columns { get; set; }

        /// <summary>
        /// Matrix rows listed in order.
        /// </summary>
        [DataMember]
        public MatrixItemRow[] Rows { get; set; }

        /// <summary>
        /// Get/set ids of child items.  First element indicates row and second indicates columns.
        /// </summary>
        [DataMember]
        public IItemProxyObject[][] ChildItems { get; set; }

        /// <summary>
        /// Gets or sets the bounced matrix information.
        /// </summary>
        /// <value>
        /// The bounced matrix information.
        /// </value>
        [DataMember]
        public BindedMatrixInfo BindedMatrixInfo { get; set; }

        /// <summary>
        /// Label column index
        /// </summary>
        [DataMember]
        public int LabelColumnIndex { get; set; }

        /// <summary>
        /// Determines whether grid lines should be rendered for survey item
        /// </summary>
        [DataMember]
        public string GridLines { get; set; }

       
    }

    /// <summary>
    /// Data container for bounced matrix information
    /// </summary>
    [Serializable]
    [DataContract]
    public class BindedMatrixInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rows static.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rows static; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public int BaseRowCount { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is rows static.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rows static; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public int BaseColumnCount { get; set; }


        /// <summary>
        /// Gets or sets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        [DataMember]
        public int ColumnCount { get; set; }


        /// <summary>
        /// Gets or sets the rows count.
        /// </summary>
        /// <value>
        /// The rows count.
        /// </value>
        [DataMember]
        public int RowsCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rows static.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is rows static; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsRowsStatic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is columns static.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is columns static; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsColumnsStatic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has row headers.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has row headers; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool HasRowHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has column headers.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has column headers; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool HasColumnHeaders { get; set; }

        [DataMember]
        public string GridLines { get; set; }
    }
}
