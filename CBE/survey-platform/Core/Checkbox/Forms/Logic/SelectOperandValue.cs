using System;
using System.Collections.Generic;
using System.Text;
using Checkbox.Common;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand value for comparing select operands
    /// </summary>
    public class SelectOperandValue : GroupOperandValue<int>
    {
        private Dictionary<int, string> _textValues;

        /// <summary>
        /// Add special handling for comparing to a single number
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override int CompareTo(OperandValue other)
        {
            //If other has a value which is a number, try to compare option ids or values as necessary
            // option ids to be compared.
            if (other.Value != null
                && other.HasValue
                && Utilities.IsNumeric(other.Value.ToString()))
            {
                //If the other value is an option and the value is an int, compare option ids.
                if ((other.DataType & OperandDataType.Option) == OperandDataType.Option
                    && Utilities.IsInt32(other.Value.ToString()))
                {
                    return CompareTo(Utilities.GetInt32(other.Value.ToString()).Value);
                }

                //NOTE:
                //If the select item has multiple values, then the meaning of greater-than, less-than, etc. is undetermined at this time

                //Attempt to get double values
                double? thisValue = Utilities.AsDouble(GetValueTextsString());
                double? otherValue = Utilities.AsDouble(other.Value.ToString());

                //If values are all doubles, do a straight double comparison
                if (thisValue.HasValue && otherValue.HasValue)
                {

                    return thisValue.Value.CompareTo(otherValue.Value);
                }
            }

            //Fall back to base handling
            return base.CompareTo(other);
        }

        /// <summary>
        /// Get the value of this operand as a CSV string of text, rather than option id, values.
        /// </summary>
        /// <returns></returns>
        protected string GetValueTextsString()
        {
            //Otherwise, attempt to compare this select operand's text value as a double with the other's value as a double
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (int optionId in Values)
            {
                if (TextValues.ContainsKey(optionId))
                {
                    if (count > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(TextValues[optionId]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get the operand data type
        /// </summary>
        /// <returns></returns>
        protected override OperandDataType GetDataType()
        {
            return base.GetDataType() | OperandDataType.Option;
        }

        /// <summary>
        /// Compare to an other value
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected virtual int CompareTo(int other)
        {
            if (Values.Count == 1)
            {
                return Values[0].CompareTo(other);
            }
            return Values.Contains(other) ? 0 : 1;
        }

        /// <summary>
        /// Get the value of the operand, which is a CSV list of option ids
        /// </summary>
        public override object Value
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < Values.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }

                    sb.Append(Values[i].ToString());
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Get text values associated with this operand
        /// </summary>
        public Dictionary<int, string> TextValues
        {
            get
            {
                if (_textValues == null)
                {
                    _textValues = new Dictionary<int, string>();
                }

                return _textValues;
            }
        }

        /// <summary>
        /// Perform contains comparison by checking text values of currently selected value.
        /// </summary>
        /// <param name="otherOperandValue"></param>
        /// <returns></returns>
        public override bool Contains(OperandValue otherOperandValue)
        {
            //Check contains by checking option text values rather than option ids stored in Values string.
            if (HasValue && otherOperandValue.HasValue)
            {
                return GetValueTextsString().ToLower().Contains(otherOperandValue.Value.ToString().ToLower());
            }

            return false;
        }

        /// <summary>
        /// Handle comparing against other group operand value items
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected override int CompareTo(GroupOperandValue<int> other)
        {
            throw new NotImplementedException("Option list to option list comparison not supported.");
        }
    }
}
