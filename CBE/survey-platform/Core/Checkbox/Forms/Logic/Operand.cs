using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// An Operand acts as a type-specific wrapper for objects that can participate in logical expression evaluations.  
    /// </summary>
    /// <remarks>
    /// An Operand acts a one part of an <see cref="Expression"/> statement, which will consist of a Left Operand, a Right Operand and a 
    /// <see cref="LogicalOperator"/>.  When an Expression is evaluated, the Operands are compared according to the LogicalOperator to 
    /// produce a boolean result.
    /// </remarks>
    [Serializable]
    public abstract class Operand
    {
        /// <summary>
        /// The object to which the Operand provides access.  
        /// </summary>
        /// <remarks>
        /// The Value is used to initialize an operand value object.
        /// </remarks>
        protected abstract object GetValue(Response response);

        /// <summary>
        /// Get a value object representing the operand value
        /// </summary>
        public virtual OperandValue GetOperandValue(Response response)
        {
            OperandValue value = CreateOperandValue();
            InitializeOperandValue(value, GetValue(response), response);

            return value;
        }

        /// <summary>
        /// Create an operand value object
        /// </summary>
        /// <returns></returns>
        public virtual OperandValue CreateOperandValue()
        {
            return new OperandValue();
        }

        /// <summary>
        /// Initialize an operand value object with its initialization value
        /// </summary>
        /// <param name="operandValue"></param>
        /// <param name="initializationValue"></param>
        /// <param name="response"></param>
        public virtual void InitializeOperandValue(OperandValue operandValue, object initializationValue, Response response)
        {
            if (operandValue != null)
            {
                operandValue.Initialize(initializationValue);
            }
        }

        /// <summary>
        /// Compare operands
        /// </summary>
        /// <param name="other"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public int CompareTo(Operand other, Response response)
        {
            return GetOperandValue(response).CompareTo(other.GetOperandValue(response));
        }
    }
}