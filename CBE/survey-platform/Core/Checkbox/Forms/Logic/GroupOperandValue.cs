using System.Collections.Generic;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Handle special case of operands of for groups of items.
    /// </summary>
    public abstract class GroupOperandValue<T> : OperandValue
    {
        /// <summary>
        /// Initialize the object
        /// </summary>
        /// <param name="value"></param>
        public override void Initialize(object value)
        {
            if (value is List<T>)
            {
                Values = (List<T>)value;
            }
            else
            {
                Values = new List<T> {(T) value };
            }
        }

        /// <summary>
        /// Get the values for the operand
        /// </summary>
        public List<T> Values { get; private set; }

        /// <summary>
        /// Get a boolean indicating if values are present
        /// </summary>
        public override bool HasValue
        {
            get{return (Values.Count > 0);}
        }

        /// <summary>
        /// Compare to 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override int CompareTo(OperandValue other)
        {
            if (other is GroupOperandValue<T>)
            {
                return CompareTo((GroupOperandValue<T>)other);
            }
            
            return base.CompareTo(other);
        }

        /// <summary>
        /// Compare two group items
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected abstract int CompareTo(GroupOperandValue<T> other);
    }
}
