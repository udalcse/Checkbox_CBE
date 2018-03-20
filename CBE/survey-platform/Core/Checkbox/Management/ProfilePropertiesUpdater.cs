using Checkbox.Security;
using Checkbox.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Web;
using Affirma.ThreeSharp.Model;
using Checkbox.Forms.Validation;
using Checkbox.Security.Principal;

namespace Checkbox.Management
{
    /// <summary>
    /// Updates profile properties and user email
    /// </summary>
    public class ProfilePropertiesUpdater
    {
        /// <summary>
        /// Updates profile properties and user email
        /// </summary>
        /// <param name="oldPropertyValue"></param>
        /// <param name="newPropertyValue"></param>
        /// <param name="propertyKey"></param>
        /// <param name="userName"></param>
        public bool UpdateUserProfileData(string newPropertyValue, string propertyKey, string userName)
        {
          
            if (string.Equals(propertyKey, "email", StringComparison.OrdinalIgnoreCase))
            {
                EmailValidator validator = new EmailValidator();
                if (!validator.Validate(newPropertyValue)) return false;
                if(UpdateEmail(userName, newPropertyValue) == null) return false;
            }
            else
            {
                UpdateProfileProperties(userName, propertyKey, newPropertyValue);
            }
            return true;
        }

        private void UpdateProfileProperties(string user, string propertyKey, string newPropertyValue)
        {
            var properties = ProfileManager.GetProfile(user, false);
            if (properties.ContainsKey(propertyKey))
                properties[propertyKey] = newPropertyValue;
            ProfileManager.StoreProfile(user, properties, true);
        }

        private CheckboxPrincipal UpdateEmail(string user, string newPropertyValue)
        {
            string status = string.Empty;
            return UserManager.UpdateUser(user, null, null, null, newPropertyValue,
                ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name, out status);
        }

        /// <summary>
        /// Gets the name of the connected profile field.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns></returns>
        public static string GetConnectedProfileFieldName(int itemId)
        {
            return ProfileManager.GetConnectedProfileFieldName(itemId);
        }

        /// <summary>
        /// Gets the name of the connected profile field.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetConnectedProfileFieldValue(int itemId, string userName)
        {
            return ProfileManager.GetCustomFieldDataByItemId(itemId,userName);
        }
    }
}
