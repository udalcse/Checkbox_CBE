using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Prezza.Framework.Security.Principal;
using System.Runtime.Serialization;

namespace Checkbox.Security.Principal
{
    /// <summary>
    /// Extension of Extended Principal specific to Checkbox.  Allows principal to include 
    /// profile properties/settings. 
    /// </summary>
    [Serializable]
    public class CheckboxPrincipal : ExtendedPrincipal
    {
        private Dictionary<string, string> _profileProperties;

        public CheckboxPrincipal() : base (null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="intrinsicUserProperties"></param>
        /// <param name="identityRoles"></param>
        /// <param name="profileProperties"></param>
        public CheckboxPrincipal(IIdentity identity, Dictionary<string, object> intrinsicUserProperties, string[] identityRoles, Dictionary<string, string> profileProperties) :
            base(identity, identityRoles)
        {
            _profileProperties = profileProperties;

            //Add user name, guid, and email to profile properties to ease piping
            if (intrinsicUserProperties != null)
            {
                if (intrinsicUserProperties.ContainsKey("GUID"))
                {
                    UserGuid = new Guid(intrinsicUserProperties["GUID"].ToString());
                }
                if (intrinsicUserProperties.ContainsKey("Email"))
                {
                    Email = intrinsicUserProperties["Email"].ToString();
                }
                if (intrinsicUserProperties.ContainsKey("LockedOut"))
                {
                    LockedOut = (bool)intrinsicUserProperties["LockedOut"];
                }
            }

            if (_profileProperties != null)
            {
                _profileProperties["GUID"] = UserGuid.ToString();
                _profileProperties["UserName"] = Identity.Name;
                _profileProperties["Email"] = Email ?? string.Empty;
            }
        }
        

        /// <summary>
        /// Get the user's profile
        /// </summary>
        public Dictionary<string, string> ProfileProperties
        {
            get { return _profileProperties ?? (_profileProperties = LoadProfileProperties()); }
        }

        /// <summary>
        /// Get/set user guid
        /// </summary>
        public Guid UserGuid { get; protected set; }

        /// <summary>
        /// Get/Set user's email address
        /// </summary>
        /// <remarks>The email address is a required part of a user identity, hence its inclusion here rather than as a profile property</remarks>
        public String Email { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LockedOut { get; protected set; }

        /// <summary>
        /// Shortcut to get/set principal profile properties in string format.
        /// </summary>
        /// <param name="profilePropertyName"></param>
        /// <returns>Value of property or string.empty if property does not exist/is null.</returns>
        /// <remarks>Setting profile value does not cause profile to be saved.  Profile must still be saved
        /// w/call to user manager.</remarks>
        public string this[string profilePropertyName]
        {
            get
            {
                if (ProfileProperties == null
                    || !ProfileProperties.ContainsKey(profilePropertyName))
                {
                     return string.Empty;
                }

                return ProfileProperties[profilePropertyName];
            }
            set
            {
                ProfileProperties[profilePropertyName] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual Dictionary<string, string> LoadProfileProperties()
        {
            var profilePropertiesWithTypes = ProfileManager.GetProfileProperties(Identity.Name, false, false, this.UserGuid, false);

            var profileProperties = profilePropertiesWithTypes.ToDictionary(item => item.Name, item => item.Value);


            profileProperties["GUID"] = UserGuid.ToString();
            profileProperties["UserName"] = Identity.Name;
            profileProperties["Email"] = Email ?? string.Empty;

            return profileProperties;
        }

        /// <summary>
        /// Persist profile to database.
        /// </summary>
        public void SaveProfile()
        {
            var propertiesDictionary = new Dictionary<string, string>();

            foreach (string key in ProfileProperties.Keys)
            {
                propertiesDictionary[key] = ProfileProperties[key] ?? string.Empty;
            }

            ProfileManager.StoreProfile(
                Identity.Name,
                propertiesDictionary);
        }
    }
}
