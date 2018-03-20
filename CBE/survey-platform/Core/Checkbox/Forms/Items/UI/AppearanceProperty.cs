using System;

namespace Checkbox.Forms.Items.UI
{
    
    /// <summary>
    /// Container for appearance property values
    /// </summary>
    [Serializable]
    public class AppearanceProperty
    {
        /// <summary>
        /// Get/set property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set value
        /// </summary>
        public string ValueAsString { get; set; }

        /// <summary>
        /// Get/set value type string
        /// </summary>
        public string ValueTypeString { get; set; }

        #region Converion Operators

        /// <summary>
        /// Conversion operator for property to string
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator string(AppearanceProperty property)
        {
            return property.ValueAsString;
        }

        /// <summary>
        /// Conversion operator for property to int.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator int(AppearanceProperty property)
        {
            int result;

            if (int.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return default(int);
        }

        /// <summary>
        /// Conversion operator for property to nullable int.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator int?(AppearanceProperty property)
        {
            int result;

            if (int.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Conversion operator for property to double
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator double(AppearanceProperty property)
        {
            double result;

            if (double.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return default(double);
        }

        /// <summary>
        /// Conversion operator for property to nullable double
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator double?(AppearanceProperty property)
        {
            double result;

            if (double.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Conver to boolean
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator bool(AppearanceProperty property)
        {
            bool result;

            if (bool.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return default(bool);
        }

        /// <summary>
        /// Conver to boolean
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static explicit operator bool?(AppearanceProperty property)
        {
            bool result;

            if (bool.TryParse(property.ValueAsString, out result))
            {
                return result;
            }

            return null;
        }

        #endregion

		public override string ToString()
		{
			return string.Format("{0} = {1}", Name, ValueAsString);
		}
    }
}
