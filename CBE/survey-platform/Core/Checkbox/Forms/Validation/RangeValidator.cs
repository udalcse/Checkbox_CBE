using System.Text;

using Checkbox.Forms.Items;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Validation
{
    /// <summary>
    /// Abstract base class for comparing values
    /// </summary>
    public abstract class RangeValidator<T> : Validator<T>
    {
        private T _minValue;
        private T _maxValue;

        private bool _minValueSet;
        private bool _maxValueSet;

        /// <summary>
        /// Get a boolean indicating if the min value has been set
        /// </summary>
        public bool MinValueSet { get { return _minValueSet; } }

        /// <summary>
        /// Get a boolean indicating if the max value has been set
        /// </summary>
        public bool MaxValueSet { get { return _maxValueSet; } }

        /// <summary>
        /// Get/set min value
        /// </summary>
        public T MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                _minValueSet = true;
            }
        }

        /// <summary>
        /// Get/set max value
        /// </summary>
        public T MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                _maxValueSet = true;
            }
        }

        /// <summary>
        /// Get the language specific error message.
        /// </summary>
        /// <param name="languageCode">Language code</param>
        /// <returns>Localized error message.</returns>
        public override string GetMessage(string languageCode)
        {
            StringBuilder sb = new StringBuilder();

            if (_minValueSet && !_maxValueSet)
            {
                sb.Append(TextManager.GetText("/validationMessages/regex/rangeNoMax", languageCode));
                sb.Replace("{min}", _minValue.ToString());
            }
            else if (!_minValueSet && _maxValueSet)
            {
                sb.Append(TextManager.GetText("/validationMessages/regex/rangeNoMin", languageCode));
                sb.Replace("{max}", _maxValue.ToString());
            }
            else
            {
                sb.Append(TextManager.GetText("/validationMessages/regex/range", languageCode));
                sb.Replace("{min}", _minValue.ToString());
                sb.Replace("{max}", _maxValue.ToString());
            }

            return sb.ToString();
        }
    }
}
