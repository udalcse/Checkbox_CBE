using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Compare operands for evaluating conditions
    /// </summary>
    [Serializable]
    public static class OperandComparer
    {
        /// <summary>
        /// Compare two operands with the specified logical operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="operation"></param>
        /// <param name="right"></param>
        /// <param name="response">Optional argument when running in surveys.</param>
        /// <returns></returns>
        public static bool Compare(Operand left, LogicalOperator operation, Operand right, Response response)
        {
            //If there is no left operand, return false
            if (left == null || right == null)
            {
                return false;
            }
            OperandValue leftValue;
            OperandValue rightValue;

            //Check the operator
            switch (operation)
            {
                case LogicalOperator.Answered:
                    return left.GetOperandValue(response).HasValue;

                case LogicalOperator.NotAnswered:
                    return !left.GetOperandValue(response).HasValue;
                    
                case LogicalOperator.Contains:
                    leftValue = left.GetOperandValue(response);
                    rightValue = right.GetOperandValue(response);

                    if (leftValue.HasValue && rightValue.HasValue)
                    {
                        return leftValue.Contains(rightValue);
                    }
                    
                    return false;

                case LogicalOperator.DoesNotContain:
                    leftValue = left.GetOperandValue(response);
                    rightValue = right.GetOperandValue(response);

                    if (leftValue.HasValue && rightValue.HasValue)
                    {
                        return !leftValue.Contains(rightValue);
                    }
                    
                    return true;

                case LogicalOperator.Equal:
                    return (left.CompareTo(right, response) == 0);

                case LogicalOperator.GreaterThan:
                    return (left.CompareTo(right, response) > 0);
                    
                case LogicalOperator.GreaterThanEqual:
                    return (left.GetOperandValue(response).HasValue && (left.CompareTo(right, response) >= 0));

                case LogicalOperator.IsNotNull:
                    return left.GetOperandValue(response).HasValue;
                    
                case LogicalOperator.IsNull:
                    return !left.GetOperandValue(response).HasValue;
                    
                case LogicalOperator.LessThan:
                    return (left.CompareTo(right, response) < 0);

                case LogicalOperator.LessThanEqual:
                    return (left.GetOperandValue(response).HasValue && (left.CompareTo(right, response) <= 0));
                    
                case LogicalOperator.NotEqual:
                    return (left.CompareTo(right, response) != 0);
                
                default:
                    return false;
            }
        }
    }
}
