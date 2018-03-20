using System;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// A lightweight container for lists of surveys and folders.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SurveyListItem
    {
        /// <summary>
        /// The item's id.
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// The survey's GUID. If the item is a folder this value is null.
        /// </summary>
        [DataMember]
        public Guid? SurveyGuid { get; set; }

        /// <summary>
        /// Indicates if the item is a survey or a folder.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// The item's name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The name of the user who created the item.
        /// </summary>
        [DataMember]
        public string Creator { get; set; }

        /// <summary>
        /// The last time the item was modified.
        /// </summary>
        [DataMember]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// If the item is a survey this property indicates whether or not it is active.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// The total number of completed responses.
        /// </summary>
        [DataMember]
        public int CompletedResponseCount { get; set; }

        /// <summary>
        /// The total number of incomplete responses.
        /// </summary>
        [DataMember]
        public int IncompleteResponseCount { get; set; }

        /// <summary>
        /// The total number of test responses.
        /// </summary>
        [DataMember]
        public int TestResponseCount { get; set; }

        /// <summary>
        /// The total number of surveys and folder that are contained in a folder item.
        /// </summary>
        [DataMember]
        public int ChildrenCount { get; set; }

        /// <summary>
        /// The list of surveys and folders that are children of folder item.
        /// </summary>
        [DataMember]
        public SurveyListItem[] ChildItems { get; set; }
    }
}
