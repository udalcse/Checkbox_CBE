using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Checkbox.Invitations;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Newtonsoft.Json;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Users
{
    /// <summary>
    /// PropertyBindingManager
    /// </summary>
    public class PropertyBindingManager
    {
        /// <summary>
        /// Gets the current user unique identifier based on current user principal or ivnitation query param.
        /// </summary>
        /// <returns></returns>
        public static Guid GetCurrentUserGuid()
        {
            var principal = GetUserPrincipal();
            var userGuid = principal?.UserGuid ?? Guid.Empty;

            if (userGuid.Equals(Guid.Empty))
            {
                if (HttpContext.Current.User != null && HttpContext.Current.User is CheckboxPrincipal)
                {
                    var guidFromSession = ((CheckboxPrincipal)HttpContext.Current.User).UserGuid;

                    if (guidFromSession != null && !guidFromSession.Equals(Guid.Empty))
                    {
                        userGuid = guidFromSession;
                    }
                }
            }

            return userGuid;
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserName()
        {
            return GetUserPrincipal()?.Identity?.Name;
        }

        /// <summary>
        /// Gets the binded property by item identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static ProfileProperty GetBindedPropertyByItemId(int id)
        {
            return ProfileManager.GetPropertiesList().FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == id));
        }

        private static CheckboxPrincipal GetUserPrincipal()
        {
            var invitationQueryParameter = HttpContext.Current.Request?.QueryString.Get("i") ?? HttpContext.Current.Request?.QueryString.Get("amp;i");
            Guid invitationId;

            if (!String.IsNullOrWhiteSpace(invitationQueryParameter) &&
                Guid.TryParse(invitationQueryParameter, out invitationId))
            {
                var recipient = InvitationManager.GetRecipientByGuid(invitationId);
                return UserManager.GetUserPrincipal(recipient.UniqueIdentifier);
            }

            var userQueryParameter = HttpContext.Current.Request?.QueryString.Get("respondentGuid") ?? HttpContext.Current.Request?.QueryString.Get("amp;respondentGuid");
            Guid userId;

            if (!String.IsNullOrWhiteSpace(userQueryParameter) &&
                Guid.TryParse(userQueryParameter, out userId))
            {
                return UserManager.GetUserByGuid(userId);
            }

            return UserManager.GetCurrentPrincipal();
        }

        /// <summary>
        /// Saves the binded field response.
        /// </summary>
        /// <param name="itemID">The item identifier.</param>
        /// <param name="responseGuid">The response unique identifier.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="fieldType">Type of the field.</param>
        public static void SaveBindedFieldResponse(int itemID, Guid responseGuid, CheckboxPrincipal principal,
            string fieldType)
        {
            switch (fieldType)
            {
                case "Matrix":
                    SaveResponseMatrixState(itemID, responseGuid, principal);
                    break;
                case "RadioButtons":
                    SaveResponseRadioButtonState(itemID, responseGuid, principal);
                    break;
            }
        }
        /// <summary>
        /// Gets the binded RadioButton json by item identifier.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <returns></returns>
        public static string GetBindedRadioButtonJsonByItemId(int itemId, Guid uniqueIdentifier)
        {
            var profileField = ProfileManager.GetPropertiesList()
                .FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == itemId));

            if (profileField != null)
            {
                var radioButton = ProfileManager.GetRadioButtonField(profileField.Name, uniqueIdentifier);

                var aliases = ProfileManager.GetRadioOptionAliases(itemId, radioButton.Name);

                foreach (var option in radioButton.Options)
                {
                    option.Alias = aliases.FirstOrDefault(opt => opt.Key == option.Name).Value;
                }

                if (radioButton != null)
                {
                    return JsonConvert.SerializeObject(radioButton);
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Saves the state of the response RadioButton.
        /// </summary>
        /// <param name="itemID">The item identifier.</param>
        /// <param name="responseGuid">The response unique identifier.</param>
        /// <param name="principal">The principal.</param>
        public static void SaveResponseRadioButtonState(int itemID, Guid responseGuid, CheckboxPrincipal principal)
        {
            var profileFields = ProfileManager.GetPropertiesList();

            var field = profileFields.FirstOrDefault(prop => prop.BindedItemId.Any(item => item.Equals(itemID)));

            if (field != null)
            {
                var jsonRadioButtonState = principal.Identity.AuthenticationType.Equals("Anonymous Respondent")
                    ? GetSessionAnonymousBindedFieldJson(field.Name)
                    : GetBindedRadioButtonJsonByItemId(itemID, principal.UserGuid);

                var db = DatabaseFactory.CreateDatabase();

                using (var connection = db.GetConnection())
                {
                    try
                    {
                        connection.Open();

                        var cmd =
                            db.GetStoredProcCommandWrapper("ckbx_sp_UserFieldResponseState_InsertResponse");

                        cmd.AddInParameter("ItemId", DbType.Int32, itemID);
                        cmd.AddInParameter("UserName", DbType.String, principal.AclEntryIdentifier);
                        cmd.AddInParameter("ResponseData", DbType.String, jsonRadioButtonState);
                        cmd.AddInParameter("Response", DbType.Guid, responseGuid);
                        cmd.AddInParameter("CustomUserFieldId", DbType.Int32, field.FieldId);
                        cmd.AddInParameter("FieldName", DbType.String, field.Name);

                        db.ExecuteNonQuery(cmd);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Saves matrix user field state on response completed
        /// </summary>
        /// <param name="itemID">The item identifier.</param>
        /// <param name="responseGuid">The response unique identifier.</param>
        /// <param name="principal">The principal.</param>
        public static void SaveResponseMatrixState(int itemID, Guid responseGuid, CheckboxPrincipal principal)
        {
            var profileFields = ProfileManager.GetPropertiesList();

            var field = profileFields.FirstOrDefault(prop => prop.BindedItemId.Any(item => item.Equals(itemID)));

            if (field != null)
            {
                string jsonMatrixState;

                if (principal.Identity.AuthenticationType.Equals("Anonymous Respondent"))
                {
                    jsonMatrixState = GetSessionAnonymousBindedFieldJson(field.Name);
                }
                else
                {
                    jsonMatrixState = MatrixField.GetBindedMatrixJsonByItemId(itemID, principal.UserGuid);
                }

                var db = DatabaseFactory.CreateDatabase();

                using (var connection = db.GetConnection())
                {
                    try
                    {
                        connection.Open();

                        var cmd =
                            db.GetStoredProcCommandWrapper("ckbx_sp_UserFieldResponseState_InsertResponse");

                        cmd.AddInParameter("ItemId", DbType.Int32, itemID);
                        cmd.AddInParameter("UserName", DbType.String, principal.AclEntryIdentifier);
                        cmd.AddInParameter("ResponseData", DbType.String, jsonMatrixState);
                        cmd.AddInParameter("Response", DbType.Guid, responseGuid);
                        cmd.AddInParameter("CustomUserFieldId", DbType.Int32, field.FieldId);
                        cmd.AddInParameter("FieldName", DbType.String, field.Name);

                        db.ExecuteNonQuery(cmd);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

        }

        /// <summary>
        /// Gets the session matrces json for anonymous user.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">fieldName</exception>
        public static string GetSessionAnonymousBindedFieldJson(string fieldName)
        {
            if (String.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException ("fieldName");

            var session = HttpContext.Current?.Session;
            var suerveyGuid = HttpContext.Current?.Request.QueryString["s"];
            if (session != null && suerveyGuid != null)
            {
                var state = ((ResponseSessionData)session["SurveySession_" + Guid.Parse(suerveyGuid)]);
                if (state?.AnonymousBindedFields != null && state.AnonymousBindedFields.ContainsKey(fieldName))
                {
                    var json = state?.AnonymousBindedFields?[fieldName];

                    if (json != null)
                        return json;
                }
               
            }

            return String.Empty;
        }


        /// <summary>
        /// Gets the response state field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="responseGuid">The response unique identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static T GetResponseStateField<T> (int itemId, Guid responseGuid, string userName) where T: new()
        {
            var jsonState = GetResponseFieldState(itemId, responseGuid, userName);
            var field = new T();
            if (!string.IsNullOrWhiteSpace(jsonState))
                field = JsonConvert.DeserializeObject<T>(jsonState);

            return field;
        }

        /// <summary>
        ///     Get user profile field item state
        /// </summary>
        public static string GetResponseFieldState(int itemId, Guid responseGuid, string userName)
        {
            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_UserFieldResponseState_GetResponse");

            command.AddInParameter("ItemId", DbType.Int32, itemId);
            command.AddInParameter("UserName", DbType.String, userName);
            command.AddInParameter("Response", DbType.Guid, responseGuid);

            string response = null;

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        response = DbUtility.GetValueFromDataReader(reader, "ResponseData", String.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the name of all RadioButton options by.
        /// </summary>
        /// <param name="radioButtonName">Name of the radio button.</param>
        /// <returns></returns>
        public static List<RadioButtonFieldOption> GetAllRadioButtonOptionsByName(string radioButtonName)
        {
            ArgumentValidation.CheckForEmptyString(radioButtonName, "radioButtonName");

            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetAllRadioButtonOptions");

            command.AddInParameter("PropertyName", DbType.String, radioButtonName);

            List<RadioButtonFieldOption> options = new List<RadioButtonFieldOption>();


            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        RadioButtonFieldOption option = new RadioButtonFieldOption()
                        {
                            Alias = DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty),
                            Id = DbUtility.GetValueFromDataReader(reader, "Id", 0),
                            Name = DbUtility.GetValueFromDataReader(reader, "OptionText", string.Empty),
                            ItemId = DbUtility.GetValueFromDataReader(reader, "ItemId", 0)
                        };

                        options.Add(option);
                    }
                    reader.NextResult();
                }
                finally
                {
                    reader.Close();
                }
            }

            return options;
        }

        /// <summary>
        /// Gets the name of the binded item ids by property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static List<int> GetBindedItemIdsByPropertyName(string name)
        {
            ArgumentValidation.CheckForEmptyString(name, "name");

            var db = DatabaseFactory.CreateDatabase();
            var command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetBindedItemIdsByPropertyName");

            command.AddInParameter("PropertyName", DbType.String, name);

            List<int> result = new List<int>();

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(DbUtility.GetValueFromDataReader(reader, "ItemId", 0));
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }

        /// <summary>
        ///     Remove all item states for response
        /// </summary>
        public static void RemoveResponseMatrixSate(int responseId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    var cmd =
                        db.GetStoredProcCommandWrapper("ckbx_sp_UserFieldResponseState_DeleteResponse");

                    cmd.AddInParameter("ResponseId", DbType.Int32, responseId);

                    db.ExecuteNonQuery(cmd);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        ///     Revove all survey responses for response template
        /// </summary>
        /// <param name="responseTemplateId"></param>
        public static void RemoveAllSurveyMatrixResponses(int responseTemplateId)
        {
            var db = DatabaseFactory.CreateDatabase();

            using (var connection = db.GetConnection())
            {
                try
                {
                    connection.Open();

                    var cmd =
                        db.GetStoredProcCommandWrapper("ckbx_sp_UserFieldResponseState_DeleteAllResponsesForTemplate");

                    cmd.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

                    db.ExecuteNonQuery(cmd);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Check if item is binded
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool IsBinded(int itemId)
        {
            return ProfileManager.GetPropertiesList()
                .FirstOrDefault(prop => prop.BindedItemId.Any(bindedItemId => bindedItemId == itemId)) != null;
        }

        /// <summary>
        /// Validate property type and adds the item mapping .
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="profilePropertyId">The profile property identifier.</param>
        /// <param name="type">The type.</param>
        public static bool AddItemMapping(int itemId, int profilePropertyId, CustomFieldType type)
        {
            var user = UserManager.GetCurrentPrincipal();

            if (user != null)
            {
                var property = ProfileManager.GetProfileProperties(user.Identity.Name)
                    .FirstOrDefault(item => item.FieldId == profilePropertyId);

                if (property != null && property.FieldType == type)
                {
                    AddSurveyItemProfilePropertyMapping(itemId, profilePropertyId);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Saves profile property and survey item mapping to the database
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="profilePropertyId"></param>
        public static void AddSurveyItemProfilePropertyMapping(int itemId, int profilePropertyId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_CreateFieldItemMap");
            command.AddInParameter("@CustomUserFieldId", DbType.Int32, profilePropertyId);
            command.AddInParameter("@ItemId", DbType.Int32, itemId);
            try
            {
                db.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                db.GetConnection().Close();
            }
        }
    }
}
