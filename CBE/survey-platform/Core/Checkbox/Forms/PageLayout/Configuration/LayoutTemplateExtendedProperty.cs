using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.PageLayout.Configuration
{
    /// <summary>
    /// Simple container for extended properties
    /// </summary>
    [Serializable]
    public class LayoutTemplateExtendedProperty
    {
        ///<summary>
        ///Name of property.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Value of property.
        ///</summary>
        public object Value { get; set; }

        /// <summary>
        /// Max allowed value (if any) for property.
        /// </summary>
        public double? MaxValue { get; set; }

        ///<summary>
        /// Minimum allowed value (if any) for property.
        ///</summary>
        public double? MinValue { get; set; }

        ///<summary>
        /// List of possible values for property.
        ///</summary>
        public List<string> PossibleValues { get; set; }

        /// <summary>
        /// Type of property.
        /// </summary>
        public LayoutTemplateExtendedPropertyType Type { get; set; }
    }
}
