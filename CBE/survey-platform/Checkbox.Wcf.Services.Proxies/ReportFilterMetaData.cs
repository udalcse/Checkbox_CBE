using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for a list of survey reports
    /// </summary>
    [DataContract]
    [Serializable]
    public class ReportFilterMetaData
    {
        /// <summary>
        /// Get the database id of the report filter
        /// </summary>
        [DataMember]
        public int ReportFilterId { get; set; }

        /// <summary>
        /// Get the filter type name.
        /// </summary>
        /// <remarks>Checkbox supports three filter types:
        ///   Item          --  Filter based on answers to a question.
        ///   Profile       --  Filter based on profile attributes of the respondent.
        ///   Response      --  Filter based on properties of the response.
        /// </remarks>
        [DataMember]
        public string FilterTypeName { get; set; }

        /// <summary>
        /// Get/set <see cref="LogicalOperator"/> used by the filter for comparing values.
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// Get/set value to use for comparison
        /// </summary>
        [DataMember]
        public object Value { get; set; }

        /// <summary>
        /// Get/set value to use for comparison
        /// </summary>
        [DataMember]
        public virtual string LValue 
        { 
            get
            {
                return "";
            } 
        }

    }
}
