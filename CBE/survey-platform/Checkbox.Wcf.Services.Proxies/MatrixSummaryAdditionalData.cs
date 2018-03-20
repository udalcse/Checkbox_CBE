using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Additional data associated with matrix summary item results
    /// </summary>
    [Serializable]
    [DataContract]
    public class MatrixSummaryAdditionalData
    {
        [DataMember]
        public int[] ColumnSourceItems { get; set; }

        /// <summary>
        /// item id and is the row subheader
        /// </summary>
        [DataMember]
        public Dictionary<int, bool> RowSourceItems { get; set; }

        [DataMember]
        public SimpleNameValueCollection SumTotalAverages { get; set; }

        [DataMember]
        public SimpleNameValueCollection RatingScaleAverages { get; set; }

        [DataMember]
        public SimpleNameValueCollection SliderAverages { get; set; }

        /// <summary>
        /// Children of matrix.  Format of keys is ROW_COLUMN and value is item id
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection MatrixChildren { get; set; }

        /// <summary>
        /// Map column item ids to their columns.  The column source items list may not be contiguous as it 
        /// may contain columns not supported by the summary item.
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection ColumnItemPositions { get; set; }
    }
}
