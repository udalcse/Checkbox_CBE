using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for rule expression metadata
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExpressionMetaData
    {
        /// <summary>
        /// ID of expression
        /// </summary>
        [DataMember]
        public int ExpressionId { get; set; }

        /// <summary>
        /// Operator used to relate children during evaluation
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// Text of left operand (if any). Expressions with children typically will not have operands.
        /// </summary>
        [DataMember]
        public string LeftOperandText { get; set; }

        /// <summary>
        /// Type of the left parameter.
        /// </summary>
        [DataMember]
        public string SourceType { get; set; }        

        /// <summary>
        /// Text of right operand (if any).  Expressions with children typically will not have operands.
        /// </summary>
        [DataMember]
        public string RightOperandText { get; set; }

        /// <summary>
        /// Type of left operand (if any). 
        /// </summary>
        [DataMember]
        public string LeftOperandType { get; set; }

        /// <summary>
        /// Type of right operand (if any). 
        /// </summary>
        [DataMember]
        public string RightOperandType { get; set; }

        /// <summary>
        /// ID of right operand (if any). 
        /// </summary>
        [DataMember]
        public int? RightOperandID { get; set; }

        /// <summary>
        /// Children of this expression, which represent the AND (or product) expressions in the 
        /// boolean sum of products used for conditional evaluation.
        /// </summary>
        [DataMember]
        public ExpressionMetaData[] AndExpressions { get; set; }
    }
}
