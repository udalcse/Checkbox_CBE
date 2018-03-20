using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Checkbox.Users
{

    /// <summary>
    /// Class that defines profile fields in the account section 
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(MatrixProfileProperty))]
    [XmlInclude(typeof(RadioButtonField))]
    public class ProfileProperty
    {
        /// <summary>
        /// Gets Field Id
        /// </summary>
        /// <value>
        /// Id
        /// </value>
        [XmlIgnore]
        public int FieldId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [XmlIgnore]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public CustomFieldType FieldType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// If userFieldType is binded to any Item
        /// </summary>
        /// <value>
        ///   <c>Item Id</c> if this instance is binded; otherwise, <c>0</c>.
        /// </value>
        [XmlIgnore]
        public List<int> BindedItemId { get; set; }
    }
}
