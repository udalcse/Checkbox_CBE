using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    public class StyleMetaData
    {
        /// <summary>
        /// Style database id
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Get/set the non-localized name of the style
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Date style was created
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Name of creator
        /// </summary>
        /// <remarks>This value is not modified in the Checkbox database when updating a style.</remarks>
        [DataMember]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Is style usable by the public?
        /// </summary>
        [DataMember]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Are other users able to edit this style?
        /// </summary>
        /// <remarks>This value does not prevent the creator from editing the style</remarks>
        [DataMember]
        public bool IsEditable { get; set; }

        /// <summary>
        /// Text id to retrieve ML Header
        /// </summary>
        [DataMember]
        public string HeaderTextId { get; set; }

        /// <summary>
        /// Text id to retrieve ML Footer
        /// </summary>
        [DataMember]
        public string FooterTextId { get; set; }
    }
}
