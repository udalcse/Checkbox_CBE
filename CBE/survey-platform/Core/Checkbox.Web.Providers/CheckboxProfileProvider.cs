using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Profile;
using Checkbox.Common;
using Checkbox.Security.Principal;
using Checkbox.Security.Providers;
using Prezza.Framework.Data;
using Checkbox.Users;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Web.Providers
{
    /// <summary>
    /// Implementation of profile provider to fit ASP.NET provider model.
    /// </summary>
    public class CheckboxProfileProvider : ProfileProvider, ICheckboxProfileProvider
    {
        private const string PROFILE_PROVIDER_DATABASE_NAME = "CheckboxProfileProvider";

        /// <summary>
        /// Get/set application name provider is associated with.
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Checkbox profile provider
        /// </summary>
        public string ProviderName
        {
            get { return "CheckboxProfileProvider"; }
        }

        /// <summary>
        /// Delete profiles for the specified user.
        /// </summary>
        /// <param name="userNames"></param>
        /// <returns></returns>
        public override int DeleteProfiles(string[] userNames)
        {
            foreach (string userName in userNames)
            {
                Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_Delete");
                command.AddInParameter("UniqueIdentifier", DbType.String, userName);

                db.ExecuteNonQuery(command);
            }

            return userNames.Length;
        }

        /// <summary>
        /// Delete the specified profiles.
        /// </summary>
        /// <param name="profiles"></param>
        /// <returns></returns>
        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            return DeleteProfiles((from ProfileInfo profile in profiles select profile.UserName).ToArray());
        }

        /// <summary>
        /// Get profile property values.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection collection)
        {
            var values = new SettingsPropertyValueCollection();

            //Make sure user name is present
            if (!context.ContainsKey("UserName")
                || context["UserName"] == null
                || Utilities.IsNullOrEmpty(context["UserName"].ToString()))
            {
                return values;
            }

            //Assign values to collection
            Dictionary<string, string> profileProperties = GetProfile(context["UserName"].ToString());

            foreach (SettingsProperty property in collection)
            {
                var propertyValue = new SettingsPropertyValue(property)
                {
                    Deserialized = true,
                    PropertyValue = profileProperties.ContainsKey(property.Name)
                        ? profileProperties[property.Name]
                        : string.Empty
                };

                values.Add(propertyValue);
            }

            return values;
        }

        /// <summary>
        /// Persist profile values to the database.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection"></param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            //Make sure user name is present
            if (!context.ContainsKey("UserName")
                || context["UserName"] == null)
            {
                return;
            }

            var userUniqueIdentifier = context["UserName"] as string;

            //If no user name, return empty collection
            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                return;
            }

            //Dictionary to store property/value pairs
            var valuesDictionary = new Dictionary<string, string>();

            //Populate dictionary w/values
            foreach (SettingsPropertyValue value in collection)
            {
                valuesDictionary[value.Name] = (value.PropertyValue as string) ?? string.Empty;
            }

            //Store values
            StoreProfile(userUniqueIdentifier, valuesDictionary);
        }

        /// <summary>
        /// Get the profile for a user.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetProfile(string userUniqueIdentifier)
        {
            var propertyValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            //Ensure user name is passed
            //If no user name, return empty collection
            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                return propertyValues;
            }

            //Otherwise, attempt to get property values
            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_Get");
            command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {

                        string propertyName = DbUtility.GetValueFromDataReader(reader, "PropertyName", string.Empty);
                        string propertyValue = DbUtility.GetValueFromDataReader(reader, "PropertyValue", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(propertyName))
                        {
                            propertyValues[propertyName] = propertyValue ?? string.Empty;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return propertyValues;
        }

        /// <summary>
        /// Get the profile for a user with field type.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <returns></returns>
        public List<ProfileProperty> GetProfileProperties(string userUniqueIdentifier)
        {
            List<ProfileProperty> properties = new List<ProfileProperty>();

            //Ensure user name is passed
            //If no user name, return empty collection
            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                return properties;
            }

            //Otherwise, attempt to get property values
            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ProfileProperties_Get");
            command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        ProfileProperty property = new ProfileProperty
                        {
                            FieldId = DbUtility.GetValueFromDataReader(reader, "CustomUserFieldID", 0),
                            Name = DbUtility.GetValueFromDataReader(reader, "PropertyName", string.Empty),
                            Value = DbUtility.GetValueFromDataReader(reader, "PropertyValue", string.Empty),
                            IsHidden = DbUtility.GetValueFromDataReader(reader, "IsHidden", false),
                            FieldType =
                                (CustomFieldType)
                                Enum.Parse(typeof(CustomFieldType),
                                    DbUtility.GetValueFromDataReader(reader, "FieldType", string.Empty))
                        };
                        properties.Add(property);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return properties;
        }

        public List<ProfileProperty> GetPropertiesList()
        {
            List<ProfileProperty> properties = new List<ProfileProperty>();

            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ProfilePropertiesList_Get");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        ProfileProperty property = new ProfileProperty
                        {
                            Name = DbUtility.GetValueFromDataReader(reader, "PropertyName", string.Empty),
                            FieldType =
                                (CustomFieldType)
                                Enum.Parse(typeof(CustomFieldType),
                                    DbUtility.GetValueFromDataReader(reader, "FieldType", string.Empty)),
                            //If if profile property not binded to any item - return 0
                            BindedItemId = Array.ConvertAll(
                                (DbUtility.GetValueFromDataReader(reader, "ItemIds", string.Empty)).Split(','),
                                s => s.Length > 0 ? int.Parse(s) : 0).ToList(),
                            FieldId = DbUtility.GetValueFromDataReader(reader, "FieldId", 0)
                        };
                        properties.Add(property);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return properties;
        }

        public List<ProfileProperty> GetFiledTypesList()
        {
            List<ProfileProperty> properties = new List<ProfileProperty>();

            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command =
                db.GetSqlStringCommandWrapper("SELECT CustomFieldType FROM ckbx_CustomUserFieldType");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        ProfileProperty property = new ProfileProperty
                        {
                            FieldType =
                                (CustomFieldType)
                                Enum.Parse(typeof(CustomFieldType),
                                    DbUtility.GetValueFromDataReader(reader, "CustomFieldType", string.Empty))
                        };
                        properties.Add(property);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return properties;
        }

        /// <summary>
        /// Persist a user's profile.
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="profileProperties"></param>
        public void StoreProfile(string userUniqueIdentifier, Dictionary<string, string> profileProperties)
        {
            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);

            Dictionary<string, string> prop = new Dictionary<string, string>(profileProperties);

            foreach (string key in prop.Keys.ToList())
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_UpSertProperty");
                command.AddInParameter("UniqueIdentifier", DbType.String, userUniqueIdentifier);
                command.AddInParameter("PropertyName", DbType.String, key);
                command.AddInParameter("PropertyValue", DbType.String, prop[key] ?? string.Empty);

                db.ExecuteNonQuery(command);
            }

            //Change the modifier of user
            var currentUser = HttpContext.Current.User as CheckboxPrincipal;
            string modifier = currentUser != null ? currentUser.Identity.Name : userUniqueIdentifier;

            UserManager.SetUserModifier(userUniqueIdentifier, modifier);
        }

        public List<string> ListPropertyNames(int customFieldTypeId)
        {
            var propertyNames = new List<string>();

            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetFieldsByTypeId");
            command.AddInParameter("@CustomFieldTypeId", DbType.Int32, customFieldTypeId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string propertyName = DbUtility.GetValueFromDataReader(reader, "CustomUserFieldName",
                            string.Empty);

                        if (Utilities.IsNotNullOrEmpty(propertyName))
                        {
                            propertyNames.Add(propertyName);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return propertyNames;
        }

        /// <summary>
        /// List property names in a profile.
        /// </summary>
        /// <returns></returns>
        public List<string> ListPropertyNames()
        {
            var propertyNames = new List<string>();

            Database db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetFields");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string propertyName = DbUtility.GetValueFromDataReader(reader, "CustomUserFieldName",
                            string.Empty);

                        if (Utilities.IsNotNullOrEmpty(propertyName))
                        {
                            propertyNames.Add(propertyName);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return propertyNames;
        }

        #region Batch Pre-load Profile Members

        /// <summary>
        /// Load all profile data for respondents to a particular survey.  Assumes standard formats for Checkbox
        /// identities and profiles.
        /// </summary>
        /// <param name="responseTemplateId"></param>
        public void PreLoadProfilesForTemplateResponses(int responseTemplateId)
        {
            var db = DatabaseFactory.CreateDatabase(PROFILE_PROVIDER_DATABASE_NAME);
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_LoadForResponseTemplate");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            var currentIdentifier = string.Empty;
            Dictionary<string, string> currentProfile = null;

            using (var reader = db.ExecuteReader(command))
            {

                try
                {
                    while (reader.Read())
                    {
                        //Get user id
                        string uniqueIdentifier = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier",
                            string.Empty);

                        //Ensure id found
                        if (string.IsNullOrEmpty(uniqueIdentifier))
                        {
                            continue;
                        }

                        //Determine whether we are on next profile or not
                        if (!currentIdentifier.Equals(uniqueIdentifier, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //Cache profile
                            if (currentProfile != null)
                            {
                                Checkbox.Security.ProfileManager.CacheProfile(currentIdentifier, currentProfile);
                            }

                            //Start building new
                            currentIdentifier = uniqueIdentifier;
                            currentProfile = new Dictionary<string, string>();
                        }

                        //Store properties
                        string propertyName = DbUtility.GetValueFromDataReader(reader, "CustomUserFieldName",
                            string.Empty);

                        //Do nothing if no field name
                        if (string.IsNullOrEmpty(propertyName))
                        {
                            continue;
                        }

                        //Add to current profile
                        if (currentProfile != null)
                        {
                            currentProfile[propertyName] = DbUtility.GetValueFromDataReader(reader, "Value",
                                string.Empty);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        #endregion


        #region Not Supported 

        /// <summary>
        /// Find profiles for a given user name.
        /// </summary>
        /// <param name="authenticationOption">Not used.</param>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="usernameToMatch"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection FindInactiveProfilesByUserName(
            ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate,
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <returns></returns>
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not suupported.
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption,
            int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="authenticationOption"></param>
        /// <param name="userInactiveSinceDate"></param>
        /// <returns></returns>
        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
