using System.Collections.Generic;
using Checkbox.Management;

namespace Checkbox.Users
{
    /// <summary>
    /// Simple container for caching results of the last access to some
    /// group methods...Done to speed up access in cases where iteration
    /// causes group memberships, etc. to be loaded many times but also in
    /// places (such as authorization providers) where local data caching
    /// is not appropriate.
    /// </summary>
    internal class GroupCache
    {
        private List<Group> _groupList;
        private List<int> _membershipList;

        /// <summary>
        /// Get/set identity name group memberships were last listed
        /// for.
        /// </summary>
        public string IdentityName { get; set; }

        /// <summary>
        /// Get/set list of groups belonged-to by a user
        /// </summary>
        public List<int> GroupMemberships
        {
            get
            {
                if (_membershipList == null)
                {
                    _membershipList = new List<int>();
                }

                return _membershipList;
            }

            set { _membershipList = value; }
        }

        /// <summary>
        /// Get list of loaded groups
        /// </summary>
        protected List<Group> GroupList
        {
            get
            {
                if (_groupList == null)
                {
                    _groupList = new List<Group>();
                }

                return _groupList;
            }
        }

        /// <summary>
        /// Get a group, if it exists in the cache.
        /// </summary>
        /// <param name="groupId">ID of group.</param>
        /// <returns>NULL if group not found. </returns>
        public Group GetGroup(int groupId)
        {
            return GroupList.Find(group => group.ID == groupId);
        }

        /// <summary>
        /// Add a group to the cache.
        /// </summary>
        /// <param name="groupToAdd"></param>
        /// <returns></returns>
        public void AddGroup(Group groupToAdd)
        {
            //Replace group if already present
            int groupIndex = GroupList.FindIndex(group => group.ID == groupToAdd.ID);

            if (groupIndex >= 0)
            {
                GroupList[groupIndex] = groupToAdd;
            }

            //Otherwise, check cache size and clear space if necessary.
            while (GroupList.Count >= ApplicationManager.AppSettings.GroupCacheSize
                && GroupList.Count > 0)
            {
                //Remove first element
                GroupList.RemoveAt(0);
            }

            //Now add item
            GroupList.Add(groupToAdd);
        }

        /// <summary>
        /// Remove a group from the list
        /// </summary>
        /// <param name="groupId"></param>
        public void RemoveGroup(int groupId)
        {
            int groupIndex = GroupList.FindIndex(group => group.ID == groupId);

            if (groupIndex >= 0)
            {
                GroupList.RemoveAt(groupIndex);
            }
        }

        /// <summary>
        /// Remove a group from the list
        /// </summary>
        /// <param name="group"></param>
        public void RemoveGroup(Group group)
        {
            GroupList.Remove(group);
        }
    }
}
