using System;
using System.Globalization;
using Checkbox.Common;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Simple container for operand values to support easy querying/comparison
    /// with other operand values.
    /// </summary>
    public class OperandValue : IComparable<OperandValue>
    {
        private OperandDataType? _dataType;
        private object _value;

        /// <summary>
        /// Initialize an operand value object
        /// </summary>
        /// <param name="value"></param>
        public virtual void Initialize(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Get the raw value
        /// </summary>
        public virtual object Value
        {
            get { return _value; }
        }

        /// <summary>
        ///  Get a  data type flags enum for the supported data type
        /// </summary>
        public OperandDataType DataType
        {
            get
            {
                if (!_dataType.HasValue)
                {
                    _dataType = GetDataType();
                }

                return _dataType.Value;
            }
        }

        /// <summary>
        /// Determine whether the operand has a value
        /// </summary>
        public virtual bool HasValue
        {
            get 
            { 
                return (Value != null && Value.ToString().Trim().Length != 0);
            }
        }

        /// <summary>
        /// Get the operand data type
        /// </summary>
        /// <returns></returns>
        protected virtual OperandDataType GetDataType()
        {
            OperandDataType dataType = OperandDataType.String;

            //If null, not too much more to do
            if (!string.IsNullOrEmpty(Value?.ToString()))
            {
                string valueString = Value.ToString().Trim();

                //Check for int
                if (Utilities.IsInt32(valueString))
                {
                    dataType |= OperandDataType.Integer;
                }

                //Check for euro double or US double
                else if (Utilities.IsDouble(valueString))
                {
                    dataType |= OperandDataType.Double;
                }

                ////Check for date time
                //else if (Utilities.IsDate(valueString))
                //{
                //    dataType |= OperandDataType.Date;
                //}

                ////Check for currency
                //else if (Utilities.IsCurrency(valueString))
                //{
                //    dataType |= OperandDataType.Currency;
                //}
            }

            return dataType;
        }

        #region IComparable<OperandValue> Members

        /// <summary>
        /// Compare this operand value to another operand value.  For numerics and dates,
        /// value is a numeric/date comparison.  For strings, comparison is case-insensitive
        /// comparison of trimmed string values.
        /// When comparing two null values, the result will always be equal. When comparing
        /// null values with non-null values, the non-null value will always be "greater".
        /// </summary>
        /// <param name="other">Value to compare with</param>
        /// <returns>
        /// Less than zero when this instance is less than other. 
        /// Zero when this instance is equal to obj. 
        /// Greater than zero when this instance is greater than obj.  
        /// </returns>
        public virtual int CompareTo(OperandValue other)
        {
            if (Value == null && other.Value == null)
            {
                return 0;
            }
            
            if (Value == null && other.Value != null)
            {
                return -1;
            }
            
            if (Value != null && other.Value == null)
            {
                return 1;
            }
            
            return DoCompareTo(other);
        }

        #endregion

        /// <summary>
        /// Compare non-null values of operand value objects.  For string comparison, case-insensitive
        /// comparisons are made.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected int DoCompareTo(OperandValue other)
        {
            //Find the flags in common & compare the values
            OperandDataType commonFlags = DataType & other.DataType;

            string valueString1 = Value.ToString().Trim();
            string valueString2 = other.Value.ToString().Trim();

            //Check types
            if ((commonFlags & OperandDataType.Integer) == OperandDataType.Integer)
            {
                int intValue1 = Utilities.GetInt32(valueString1).Value;
                int intValue2 = Utilities.GetInt32(valueString2).Value;

                //Both values should be non-null if we got to this point
                return intValue1.CompareTo(intValue2);
            }
            
            if ((commonFlags & OperandDataType.Double) == OperandDataType.Double)
            {
                double doubleValue1 = Utilities.GetDouble(valueString1).Value;
                double doubleValue2 = Utilities.GetDouble(valueString2).Value;

                return doubleValue1.CompareTo(doubleValue2);
            }
            
            //if ((commonFlags & OperandDataType.Date) == OperandDataType.Date)
            //{
            //    //Try to find a common culture so the comparison can be made between dates with
            //    // same culture.
            //    CultureInfo currentCulture = CultureInfo.CurrentCulture;
            //    CultureInfo usCulture = Utilities.GetUsCulture();
            //    CultureInfo euCulture = Utilities.GetRotwCulture();

            //    DateTime? dateValue1;
            //    DateTime? dateValue2;

            //    //Check current culture
            //    if(Utilities.IsDate(valueString1, currentCulture) && Utilities.IsDate(valueString2, currentCulture))
            //    {
            //        dateValue1 = Utilities.GetDate(valueString1, currentCulture);
            //        dateValue2 = Utilities.GetDate(valueString2, currentCulture);
            //    }
            //    else if (Utilities.IsDate(valueString1, usCulture) && Utilities.IsDate(valueString2, usCulture))
            //    {
            //        dateValue1 = Utilities.GetDate(valueString1, usCulture);
            //        dateValue2 = Utilities.GetDate(valueString2, usCulture);
            //    }
            //    else if (Utilities.IsDate(valueString1, euCulture) && Utilities.IsDate(valueString2, euCulture))
            //    {
            //        dateValue1 = Utilities.GetDate(valueString1, euCulture);
            //        dateValue2 = Utilities.GetDate(valueString2, euCulture);
            //    }
            //        //Give up and mix dates
            //    else
            //    {
            //        dateValue1 = Utilities.GetDate(valueString1);
            //        dateValue2 = Utilities.GetDate(valueString2);
            //    }

            //    return dateValue1.Value.CompareTo(dateValue2.Value);

            //}
            
            //if ((commonFlags & OperandDataType.Currency) == OperandDataType.Currency)
            //{
            //    double currencyVal1 = Utilities.GetCurrencyNumericValue(valueString1).Value;
            //    double currencyVal2 = Utilities.GetCurrencyNumericValue(valueString2).Value;

            //    return currencyVal1.CompareTo(currencyVal2);
            //}
                //The "other" flag will only be set if someone were to extend the operand value
                // class.
            
            if ((commonFlags & OperandDataType.Other) == OperandDataType.Other)
            {
                return CompareOtherValues(Value, other.Value);
            }
            
            return string.Compare(valueString1, valueString2, true);
        }

        /// <summary>
        /// Return a boolean indicating if the current operand value "contains" the provided
        /// value.
        /// </summary>
        /// <param name="otherOperandValue">Value to check.</param>
        /// <returns></returns>
        public virtual bool Contains(OperandValue otherOperandValue)
        {
            if (HasValue && otherOperandValue.HasValue)
            {
                return Value.ToString().ToLower().Contains(otherOperandValue.Value.ToString().ToLower());
            }

            //Otherwise, return false
            return false;
        }

        /// <summary>
        /// Compare two "other" objects.
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected virtual int CompareOtherValues(object value1, object value2)
        {
            throw new NotImplementedException("Comparison only supported when extending operands.");
        }             
    }
}
