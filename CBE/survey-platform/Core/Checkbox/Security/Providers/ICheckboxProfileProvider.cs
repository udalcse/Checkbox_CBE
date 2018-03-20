using System.Collections.Generic;
using Checkbox.Users;

namespace Checkbox.Security.Providers
{
    /// <summary>
    /// Base interface for profile provider used by Checkbox.  Definition is required so that 
    /// core Checkbox components can make use of web providers, such as chaining profile provider
    /// even though Checkbox assembly does not have reference to System.Web.
    /// </summary>
    public interface ICheckboxProfileProvider
    {
        /// <summary>
        /// Name of provider
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Delete profiles for the specified users.
        /// </summary>
        /// <param name="userUniqueIdentifiers">Names of user to delete profile of.</param>
        /// <returns>Number of profiles deleted.</returns>
        int DeleteProfiles(string[] userUniqueIdentifiers);

        /// <summary>
        /// Get dictionary of profile properties and values for the specified user.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        Dictionary<string, string> GetProfile(string userUniqueIdentifier);

        /// <summary>
        /// Gets profile properties.
        /// </summary>
        /// <param name="userUniqueIdentifier">The user unique identifier.</param>
        /// <returns></returns>
        List<ProfileProperty> GetProfileProperties(string userUniqueIdentifier);

        /// <summary>
        /// Gets profile properties list
        /// </summary>
        /// <returns></returns>
        List<ProfileProperty> GetPropertiesList();

        /// <summary>
        /// Gets full list of available filed types
        /// </summary>
        /// <returns></returns>
        List<ProfileProperty> GetFiledTypesList();

        /// <summary>
        /// Store profile data for the user.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="propertyValues"></param>
        void StoreProfile(string userUniqueIdentifier, Dictionary<string, string> propertyValues);

        /// <summary>
        /// List names of available profile properties.
        /// </summary>
        /// <returns></returns>
        List<string> ListPropertyNames();

        /// <summary>
        /// List names of available profile properties.
        /// </summary>
        /// <returns></returns>
        List<string> ListPropertyNames(int customeFieldTypeId);

        /// <summary>
        /// Load profile information for survey respondents.  Used for better efficiency in reports
        /// and exports
        /// </summary>
        /// <param name="responseTemplateId"></param>
        void PreLoadProfilesForTemplateResponses(int responseTemplateId);

    }
}
