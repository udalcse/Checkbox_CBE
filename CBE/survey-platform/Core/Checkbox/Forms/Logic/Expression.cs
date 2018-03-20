using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Represents a Logical (boolean) operation
    /// </summary>
    [Serializable]
    public class Expression
    {
        private readonly Operand _left;
        private readonly Operand _right;
        private readonly LogicalOperator _operator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="left">the left-hand <see cref="Operand"/></param>
        /// <param name="right">the right-hand <see cref="Operand"/></param>
        /// <param name="operation">the <see cref="LogicalOperator"/></param>
        public Expression(Operand left, Operand right, LogicalOperator operation)
        {
            _left = left;
            _right = right;
            _operator = operation;
        }

        /// <summary>
        /// Gets or sets the database ID of this Expression as found in the ckbx_Expression table's PK.
        /// </summary>
        public int? Identity { get; set; }

        /// <summary>
        /// Gets or sets the parent CompositeExpression for this Expression.
        /// </summary>
        public CompositeExpression Parent { get; set; }


        /// <summary>
        /// Evaluates an Expression using the registered <see cref="IOperandComparer"/>s and the <see cref="LogicalOperator"/> 
        /// specified during construction
        /// </summary>
        /// <returns>true or false</returns>
        internal virtual bool Evaluate(Response response)
        {
            return OperandComparer.Compare(_left, _operator, _right, response);
        }

        /// <summary>
        /// Left Operand
        /// </summary>
        public Operand LeftOperand
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Right Operand
        /// </summary>
        public Operand RightOperand
        {
            get
            {
                return _right;
            }
        }
    }
}
