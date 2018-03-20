using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Search data about one found entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class SearchSettingsInfo
    {
        /// <summary>
        /// Type of the entity
        /// </summary>
        [DataMember]
        public string ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Include entities in the search or not
        /// </summary>
        [DataMember]
        public bool Included
        {
            get;
            set;
        }

        /// <summary>
        /// Order how the search results will be displayed
        /// </summary>
        [DataMember]
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Roles that can view objects of this type
        /// </summary>
        [DataMember]
        public string Roles
        {
            get;
            set;
        }
    }
}
