using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container to expose user properties via services.
    /// </summary>
    [DataContract]
    public class UserData : IEqualityComparer<UserData>
    {
        private SimpleNameValueCollection _profile;
        private SimpleNameValueCollection _optedOutSurveys;
        private string[] _roles;
        private string[] _groupMemberships;

        /// <summary>
        /// The user's type.
        /// </summary>
        [DataMember]
        public string AuthenticationType { get; set; }

        /// <summary>
        /// The user's unique identifier.
        /// </summary>
        [DataMember]
        public string UniqueIdentifier { get; set; }

        /// <summary>
        /// "Escaped" version of unique identifier that replaces backslash with
        /// _BACKSLASH_ to avoid issues where jquery ajax removes characters from
        /// URL
        /// </summary>
        [DataMember]
        public string EscapedUniqueIdentifier {
            get { return UniqueIdentifier.Replace(@"\", @"_BACKSLASH_"); }
            set { ; }
        }

        /// <summary>
        /// <summary>
        /// The user's GUID.
        /// </summary>
        [DataMember]
        public Guid UserGuid { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the additional emails.
        /// </summary>
        /// <value>
        /// The additional emails.
        /// </value>
        [DataMember]
        public List<string> AllEmails { get; set; }

        /// <summary>
        /// The user's profile properties.
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection Profile
        {
            get { return _profile ?? (_profile = new SimpleNameValueCollection()); }

            set { _profile = value; }
        }

        /// <summary>
        /// The list of opted out surveys
        /// </summary>
        [DataMember]
        public SimpleNameValueCollection OptedOutSurveys
        {
            get { return _optedOutSurveys ?? (_optedOutSurveys = new SimpleNameValueCollection()); }

            set { _optedOutSurveys = value; }
        }

        /// <summary>
        /// </summary>
        [DataMember]
        public string OptedOutFromAccount { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string OptedOutFromAccountComment { get; set; }

        /// <summary>
        /// The list of roles assigned to the user.
        /// </summary>
        [DataMember]
        public string[] RoleMemberships
        {
            get { return _roles ?? (_roles = new string[] {}); }
            set { _roles = value; }
        }

        /// <summary>
        /// The list of groups that the user is a member of.
        /// </summary>
        [DataMember]
        public string[] GroupMemberships
        {
            get { return _groupMemberships ?? (_groupMemberships = new string[] {}); }
            set { _groupMemberships = value; }
        }

        /// <summary>
        /// Is the user locked out
        /// </summary>
        [DataMember]
        public bool LockedOut { get; set; }

        /// <summary>
        /// Uses to check user availability
        /// </summary>
        [DataMember]
        public bool IsInList { get; set; }

        #region IEqualityComparer<UserData> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(UserData x, UserData y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.UniqueIdentifier.Equals(y.UniqueIdentifier, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(UserData obj)
        {
            return obj.UniqueIdentifier.ToLower().GetHashCode();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PageItemUserData GetPageItemUserData()
        {
            return new PageItemUserData
            {
                Email = Email,
                UniqueIdentifier = UniqueIdentifier,
                IsInList = IsInList,
                AllEmails = AllEmails
                
            };
        }
    }
}
