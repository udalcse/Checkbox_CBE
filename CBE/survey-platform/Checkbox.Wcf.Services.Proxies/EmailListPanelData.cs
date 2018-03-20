using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Lightweight email list panel information class suitable for XML 
    /// serialization.  This class does not provide visibility to the members
    /// of the email list panel.  To query this information, a separate method
    /// of the Invitation Web Service should be used.
    /// </summary>
    [Serializable]
    [DataContract]
    public class EmailListPanelData
    {
        /// <summary>
        /// Get/set the database id of the panel
        /// </summary>
        /// <remarks>When updating an email list panel through the web services API, the 
        /// Database Id is used as the key to look up the panel to update. </remarks>
        [DataMember]
        public int DatabaseId { get; set; }

        /// <summary>
        /// Get/set the email list panel name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Get/set the email list panel description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Get number of addresses in panel
        /// </summary>
        [DataMember]
        public int AddressCount { get; set; }
    }
}
