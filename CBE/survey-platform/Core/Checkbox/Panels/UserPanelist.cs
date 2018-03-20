using System;
using Checkbox.Security.Principal;
using Checkbox.Users;

namespace Checkbox.Panels
{
    /// <summary>
    /// Invitation panelist that is a user in the checkbox system.
    /// </summary>
    [Serializable]
    public class UserPanelist : Panelist
    {
        /// <summary>
        /// Get the unique identifier of the panelist.
        /// </summary>
        public string UniqueIdentifier
        {
            get { return GetProperty("UniqueIdentifier"); }
            set { SetProperty("UniqueIdentifier", value); }
        }

        /// <summary>
        /// Get a property value
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override string GetProperty(string propertyName)
        {
            if (!PropertiesDictionary.ContainsKey(propertyName))
            {
                GetProfileProperty(propertyName);
            }

            return PropertiesDictionary[propertyName];
        }

        /// <summary>
        /// Get value of profile property and assign it to a value in the properties dicitonary.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void GetProfileProperty(string propertyName)
        {
            CheckboxPrincipal principal = UserManager.GetUserPrincipal(UniqueIdentifier);

         
            if (principal != null)
            {
                //Handle email specially
                if ("Email".Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
                {
                    SetProperty(propertyName, principal.Email);
                }
                else
                {
                    //Set property, defaulting to empty string for null values
                    SetProperty(propertyName, principal[propertyName]);
                }
            }
            else
            {
                SetProperty(propertyName, string.Empty);
            }
        }
    }
}
