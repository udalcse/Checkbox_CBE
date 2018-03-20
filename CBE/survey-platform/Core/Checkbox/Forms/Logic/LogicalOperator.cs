using System;

namespace Checkbox.Forms.Logic
{
	/// <summary>
	/// Enum of Logical operations
	/// </summary>
	[Serializable]
	public enum LogicalOperator
	{
        /// <summary>
        /// Default state for operator and not valid for evaluating conditional, branching, or filter logic.
        /// </summary>
		OperatorNotSpecified,

        /// <summary>
        /// Equality comparison.  In case of strings, the comparision is case INsensitive.
        /// </summary>
		Equal,

        /// <summary>
        /// Inequality comparision.  In case of strings, the comparision is case INsensitive.
        /// </summary>
		NotEqual,

        /// <summary>
        /// Greater-than comparison.
        /// </summary>
		GreaterThan,

        /// <summary>
        /// Greater-than or equal to comparision.
        /// </summary>
        GreaterThanEqual,

        /// <summary>
        /// Less-than comparison.
        /// </summary>
		LessThan,

        /// <summary>
        /// Less-than or equal to comparision.
        /// </summary>
        LessThanEqual,

        /// <summary>
        /// Contains evaluation.  For string comparisons, this is similar to string.Contains(...), but for select items, the comparison generally is evaluated is
        /// 'list of selected options contains'.
        /// </summary>
		Contains,

        /// <summary>
        /// Does not contain evaluation.  For string comparisons, this is similar to !string.Contains(...), but for select items, the comparison generally is evaluated is
        /// 'list of selected options does not contain'.
        /// </summary>
		DoesNotContain,

        /// <summary>
        /// Answered evaluation.  For string comparisons, this is similar to !string.IsNullOrEmpty(...), but for select items, the comparison generally is evaluated is
        /// 'list of selected options has more than 0 items'.
        /// </summary>
		Answered,

        /// <summary>
        /// Not answered evaluation.  For string comparisons, this is similar to string.IsNullOrEmpty(...), but for select items, the comparison generally is evaluated is
        /// 'list of selected options has 0 items'.
        /// </summary>
		NotAnswered,

        /// <summary>
        /// Is Null evalulation.  For string comparisons, this is similar to string.IsNullOrEmpty(...).
        /// </summary>
        IsNull,

        /// <summary>
        /// Is Not Null evalulation.  For string comparisons, this is similar to !string.IsNullOrEmpty(...).
        /// </summary>
        IsNotNull
	}

    /// <summary>
    /// Used to connect two or more <see cref="Expression"/>s within a logical compound statement
    /// </summary>
    [Serializable]
    public enum LogicalConnector
    {
        /// <summary>
        /// Result of expression evaluations should be connected by a logical OR.
        /// </summary>
        OR,

        /// <summary>
        /// Result of expression evaluations should be connected by a logical AND.
        /// </summary>
        AND
    }
}
