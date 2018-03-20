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
    /// Data container for matrix items for transmission to/from survey workflow via commmunication
    /// services.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyMatrixItem : SurveyResponseItem
    {
        /// <summary>
        /// Matrix columns listed in order.
        /// </summary>
        public MatrixItemColumn[] Columns { get; set; }

        /// <summary>
        /// Matrix rows listed in order.
        /// </summary>
        public MatrixItemRow[] Rows { get; set; }

        /// <summary>
        /// Get/set ids of child items.  First element indicates row and second indicates columns.
        /// </summary>
        public IItemDataTransferObject[][] ChildItems { get; set; }

        /// <summary>
        /// Label column index
        /// </summary>
        public int LabelColumnIndex { get; set; }
    }
}
