using System;
using System.Runtime.Serialization;


namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Container for page conditions.
    /// </summary>
    [Serializable]
    [DataContract]
    public class RuleMetaData
    {
        /// <summary>
        /// ID of underlying rule for page conditions.
        /// </summary>
        [DataMember]
        public int RuleId { get; set; }

        /// <summary>
        /// ID of the root expression
        /// </summary>
        [DataMember]
        public int RootExpressionId { get; set; }

        /// <summary>
        /// All expressions are stored as a boolean sum of products. The Or expressions represent the sum
        /// expressions.
        /// </summary>
        [DataMember]
        public ExpressionMetaData[] OrExpressions;

        /// <summary>
        /// Action taken by the rule.  Possible values are "Show" or "Branch".
        /// </summary>
        [DataMember]
        public string RuleAction { get; set; }

        /// <summary>
        /// Get/set position of page that is target or "Branch" actions.
        /// </summary>
        [DataMember]
        public int? BranchTargetPosition { get; set; }

        /// <summary>
        /// Get/set total conditions count
        /// </summary>
        [DataMember]
        public int TotalConditionsCount { get; set; }
    }
}
