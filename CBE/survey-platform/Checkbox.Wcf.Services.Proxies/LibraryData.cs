using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Data container for item library data.
    /// </summary>
    [DataContract]
    public class LibraryData
    {
        /// <summary>
        /// The id of the library.
        /// </summary>
        [DataMember]
        public int DatabaseId { get; set; }

        /// <summary>
        /// Get/set the name of the item library.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get/Set the description of the item library.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Get/set the creator.
        /// </summary>
        /// <remarks>This value is not modified in the Checkbox database when updating a library.</remarks>
        [DataMember]
        public string CreatedBy { get; set; }

      /// <summary>
        /// Get/set the date the library was created.
        /// </summary>
        /// <remarks>This value is not modified in the Checkbox database when updating a library.</remarks>
        [DataMember]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Get/set date library was last modified.
        /// </summary>
        [DataMember]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Get ids of items contained in library. 
        /// </summary>
        [DataMember]
        public int[] ItemIds { get; set; }

        /// <summary>
        /// Gets item aliases along with its ids
        /// </summary>
        [DataMember]
        public Dictionary<int, string> ItemAliases { get; set; }

        /// <summary>
        /// Gets item aliases along with its ids
        /// </summary>
        [DataMember]
        public Dictionary<int, bool> MenuSettings { get; set; }
    }
}
