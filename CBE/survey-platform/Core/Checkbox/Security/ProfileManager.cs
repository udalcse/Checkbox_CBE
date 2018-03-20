using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Checkbox.Common;
using Checkbox.Invitations;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;
using Prezza.Framework.Logging;
using System.Text.RegularExpressions;
using System.Web;
using Checkbox.Globalization.Text;
using Newtonsoft.Json;
using Prezza.Framework.Caching;

namespace Checkbox.Security
{
    /// <summary>
    /// Static manager for application to use to access profile information.
    /// </summary>
    public static class ProfileManager
    {
        private static ICheckboxProfileProvider _profileProvider;

        //Cache of user profile to avoid repeated calls to get profiles that will would add
        // significant "chatter" to database.
        private static CacheManager _profileCache;

        /// <summary>
        /// The default item text pattern
        /// </summary>
        private const string DefaultItemTextPattern = "/textItemData/{0}/defaultText";

        /// <summary>
        /// The default language code
        /// </summary>
        private const string DefaultLanguageCode = "en-US";
        
        /// <summary>
        /// Initialize the manager with the profile provider to use.
        /// </summary>
        /// <param name="profileProvider"></param>
        public static void Initialize(ICheckboxProfileProvider profileProvider)
        {
            //Probably  not necessary, but add to prevent double initialization
            lock (typeof(ProfileManager))
            {
                if (_profileProvider == null)
                {
                    _profileProvider = profileProvider;
                }

                _profileCache = CacheFactory.GetCacheManager("userProfileCacheManager");
            }
        }

        /// <summary>
        /// Get a reference to the profile provider.
        /// </summary>
        public static ICheckboxProfileProvider CheckboxProvider
        {
            get
            {
                if (_profileProvider == null)
                {
                    throw new Exception("Profile Manager has not been initialized with a Checkbox profile provider.");
                }

                return _profileProvider;
            }
        }

        /// <summary>
        /// Get a profile dictionary for the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="useCache"></param>
        /// <param name="encode"> </param>
        /// <returns></returns>
        public static Dictionary<string, string> GetProfile(string uniqueIdentifier, bool useCache = true, bool encode = false)
        {
            //Check argument
            if (Utilities.IsNullOrEmpty(uniqueIdentifier))
            {
                return null;
            }

            Dictionary<string, string> profile;

            //Check cache
            if (useCache && _profileCache.Contains(uniqueIdentifier))
            {
                profile = (Dictionary<string, string>)_profileCache[uniqueIdentifier];
            }
            else
            {
                //Merge profile and identity properties
                profile = CheckboxProvider.GetProfile(uniqueIdentifier);
                CacheProfile(uniqueIdentifier, profile);
            }

            return encode ? profile.ToDictionary(property => property.Key, property => Utilities.SimpleHtmlEncode(property.Value)) : profile;
        }

        /// <summary>
        /// Get a profile dictionary for the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier.</param>
        /// <param name="useCache">if set to <c>true</c> [use cache].</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="addMatrixJsonValue">if set to <c>true</c> [add matrix json value].</param>
        /// <param name="addRadioJsonValue">if set to true then value for radio button will be serialized to json</param>
        /// <returns></returns>
        public static List<ProfileProperty> GetProfileProperties(string uniqueIdentifier, bool useCache = true,
            bool encode = false, Guid? userId = null, bool addMatrixJsonValue = true)
        {
            //Check argument
            if (Utilities.IsNullOrEmpty(uniqueIdentifier))
            {
                return null;
            }

            
            //Merge profile and identity properties
            var profileProperties = CheckboxProvider.GetProfileProperties(uniqueIdentifier);

            if (userId.HasValue && addMatrixJsonValue)
                MatrixField.PopulateMatrixFields(ref profileProperties, userId);

            if (userId.HasValue)
                PopulateRadioFields(ref profileProperties, userId);

            // check if matrix has values in cell or now, if yes put value as true to validate matrix dependent conditions in operand comparer file. 
            if (userId.HasValue && !addMatrixJsonValue)
            {
                var matrixProperties = profileProperties.Where(t => t.FieldType == CustomFieldType.Matrix);
                foreach (var item in matrixProperties)
                {
                    var matrixField = GetMatrixField(item.Name, userId.Value);
                    if (matrixField.Cells.Any())
                    {
                        item.Value = matrixField.Cells.FirstOrDefault(cell => !string.IsNullOrEmpty(cell.Data.Trim()) && !cell.IsRowHeader && !cell.IsHeader) != null
                            ? "true"
                            : string.Empty;
                    }
                }
            }

            if (encode)
                foreach (var item in profileProperties)
                    item.Value = Utilities.SimpleHtmlEncode(item.Value);

            return profileProperties;
        }

        /// <summary>
        /// Get matrix in JSON format by Item id
        /// </summary>
        /// <param name="itemId"></param>
        /// /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public static string GetMatrixJsonByItemId(int itemId, string uniqueIdentifier)
        {
            //consider one item may be binded to only one profile property

            var profileProperties = GetProfileProperties(uniqueIdentifier, false, false, UserManager.GetUserGuid(uniqueIdentifier), true)
                .FirstOrDefault(p => p.FieldType == CustomFieldType.Matrix);

            return !string.IsNullOrEmpty(profileProperties?.Value) ? profileProperties.Value : string.Empty;
        }

        private static void PopulateRadioFields(ref List<ProfileProperty> profileProperties, Guid? userId)
        {
            if (!userId.HasValue) return;
            var radioProperties = profileProperties.Where(p => p.FieldType == CustomFieldType.RadioButton);
            foreach (var prop in radioProperties)
            {
                var radioField = GetRadioButtonField(prop.Name, userId.Value);
                if (radioField != null)
                {
                    var propIndex = profileProperties.IndexOf(prop);
                    var selectedProp = radioField.Options.FirstOrDefault(o => o.IsSelected);
                    profileProperties[propIndex].Value = selectedProp != null ? selectedProp.Name : string.Empty;
                }
            }
        }

        private static void PopulateMatrixFields(ref List<ProfileProperty> profileProperties, Guid? userId)
        {
            if (!userId.HasValue) return;
            var matrixProperties = profileProperties.Where(t => t.FieldType == CustomFieldType.Matrix);
            foreach (var prop in matrixProperties)
            {
                var matrixField = GetMatrixField(prop.Name, userId.Value);
                if (matrixField != null)
                {
                    var headers = matrixField.Cells.Where(c => c.IsHeader 
                        && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.ColumnNumber).ToList();
                    var rowHeaders = matrixField.Cells.Where(c => c.IsRowHeader 
                        && !string.IsNullOrEmpty(c.Data)).OrderBy(c => c.RowNumber).ToList();
                    matrixField.FillUpRows();

                    matrixField.Rows.RemoveAll(r => r.Cells.Count == 0);

                    if (matrixField.Rows.Count == 0 ||  matrixField.Rows.Any(r => r.Cells.Count < matrixField.ColumnCount))
                    {
                        FillMatrixWithEmptyCells(ref matrixField);
                    }
                    if (headers.Count > 0)
                    {
                        AddHeaders(ref matrixField, headers);
                    }
                    if (rowHeaders.Count > 0)
                    {
                        AddRowHeaders(ref matrixField, rowHeaders);
                    }
                    var matrixFieldJson = JsonConvert.SerializeObject(matrixField.Rows);
                    var propIndex = profileProperties.IndexOf(prop);
                    profileProperties[propIndex].Value = matrixFieldJson;
                }
            }
        }

        private static void FillMatrixWithEmptyCells(ref MatrixField matrixField)
        {
            //Create empty cells
            matrixField.Rows.Clear();
            for (int i = 0; i < matrixField.RowsHeaders.Count; i++)
            {
                var row = new Row();
                for (int j = 0; j < matrixField.ColumnHeaders.Count; j++)
                {
                    row.Cells.Add(new Cell()
                    {
                        ColumnNumber = j,
                        RowNumber = i,
                        IsHeader = false,
                        IsRowHeader = false,
                        CustomUserCell = false,
                        Data = string.Empty
                    });
                }
                matrixField.Rows.Add(row);
            }
        }

        private static void AddHeaders(ref MatrixField matrixField, List<Cell> headers)
        {
            Row headerRow = new Row();

            if (matrixField.ColumnHeaders.Count > 0
                && matrixField.RowsHeaders.Count > 0
                && matrixField.HasRowHeaders)
            {
                Cell firstCell = new Cell()
                {
                    ColumnNumber = 0,
                    RowNumber = 0,
                    IsHeader = true,
                    IsRowHeader = true,
                    CustomUserCell = false,
                    Data = "Headers"
                };

                headers.Insert(0, firstCell);
            }

            headerRow.Cells = headers;
            matrixField.Rows.Insert(0, headerRow);
        }

        private static void AddRowHeaders(ref MatrixField matrixField, List<Cell> rowHeaders)
        {
            bool hasHeaders = matrixField.Rows.First().Cells.First().IsHeader;
            int dataRowCount = hasHeaders ? matrixField.Rows.Count - 1 : matrixField.Rows.Count;
            if (rowHeaders.Count != dataRowCount) return;

            int dataRowStartIdx = hasHeaders ? 1 : 0;
            for (int i = dataRowStartIdx; i < matrixField.Rows.Count; i++)
            {
                int rowHeaderIdx = hasHeaders ? i - 1 : i;
                matrixField.Rows[i].Cells.Insert(0, rowHeaders[rowHeaderIdx]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static void CacheProfile(string uniqueIdentifier, Dictionary<string, string> profile)
        {
            _profileCache.Add(uniqueIdentifier, profile);
        }

        /// <summary>
        /// Persist a user's profile.
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="profileProperties"></param>
        /// <param name="useChache"></param>
        /// <returns></returns>
        public static void StoreProfile(string uniqueIdentifier, Dictionary<string, string> profileProperties, bool useChache = true)
        {
            CheckboxProvider.StoreProfile(uniqueIdentifier, profileProperties);

            if (useChache)
            {
                //Ensure user and profile updated
                UserManager.ExpireCachedPrincipal(uniqueIdentifier);

                CacheProfile(uniqueIdentifier, profileProperties);
            }
        }

        /// <summary>
        /// Persist a user's profile.
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="profileProperties"></param>
        /// <returns></returns>
        public static void StoreProfilePropertiesModel(string uniqueIdentifier, List<ProfileProperty> profileProperties)
        {
            Dictionary<string,string> properties = profileProperties.ToDictionary(item => item.Name, item => item.Value);

            CheckboxProvider.StoreProfile(uniqueIdentifier, properties);

            //Ensure user and profile updated
            UserManager.ExpireCachedPrincipal(uniqueIdentifier);

            _profileCache.Add(uniqueIdentifier, properties);
        }

        /// <summary>
        /// Delete the profile for the spedified user.
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void DeleteProfile(string uniqueIdentifier)
        {
            CheckboxProvider.DeleteProfiles(new[] { uniqueIdentifier });
          
            //Ensure user and profile updated
            UserManager.ExpireCachedPrincipal(uniqueIdentifier);
            RemoveProfileFromCache(uniqueIdentifier);
        }

        /// <summary>
        /// List available profile properties.
        /// </summary>
        /// <returns></returns>
        public static List<string> ListPropertyNames()
        {
            return CheckboxProvider.ListPropertyNames();
        }

        /// <summary>
        /// List available profile properties.
        /// </summary>
        /// <returns></returns>
        public static List<string> ListPropertyNames(int customFieldTypeId)
        {
            return CheckboxProvider.ListPropertyNames(customFieldTypeId);
        }

        /// <summary>
        /// gets profile properties list
        /// </summary>
        /// <returns></returns>
        public static List<ProfileProperty> GetPropertiesList()
        {
            return CheckboxProvider.GetPropertiesList();
        }

        /// <summary>
        /// Gets profile properties list
        /// </summary>
        /// <returns></returns>
        public static List<ProfileProperty> GetFiledTypesList()
        {
            return _profileProvider.GetFiledTypesList();
        }

        /// <summary>
        /// Lists profile properties that are marked for display on the user manager
        /// </summary>
        /// <returns></returns>
        public static List<string> ListUserManagerPropertyNames()
        {
            var propertyNames = new List<string>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_ListShowInUserManager");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string propertyName = DbUtility.GetValueFromDataReader(reader, "CustomUserFieldName", string.Empty);

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
        /// Gets radio button field
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static RadioButtonField GetRadioButtonField(string name, Guid userID)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "field must have value");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetRadioField");
            command.AddInParameter("FieldName", DbType.String, name);
            command.AddInParameter("UserID", DbType.Guid, userID);

            RadioButtonField radioButtonField = new RadioButtonField()
            {
                Name = name,
                Options = new List<RadioButtonFieldOption>()
            };
            
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int id = DbUtility.GetValueFromDataReader(reader, "Id", 0);
                        string optionText = DbUtility.GetValueFromDataReader(reader, "OptionText", string.Empty);
                        bool isSelected = DbUtility.GetValueFromDataReader(reader, "IsSelected", false);

                        radioButtonField.Options.Add(new RadioButtonFieldOption()
                        {
                            Id = id,
                            Name = optionText,
                            IsSelected = isSelected,
                        });
                    }
                    reader.NextResult();
                }
                finally
                {
                    reader.Close();
                }
            }

            return radioButtonField;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRadioOptionAliases(int itemId, string fieldName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetRadioOptionAliases");
            command.AddInParameter("FieldName", DbType.String, fieldName);
            command.AddInParameter("ItemID", DbType.Int32, itemId);
            var optionAliases = new Dictionary<string, string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string option = DbUtility.GetValueFromDataReader(reader, "OptionText", string.Empty);
                        string alias = DbUtility.GetValueFromDataReader(reader, "Alias", string.Empty);

                        optionAliases.Add(option, alias);
                    }
                    reader.NextResult();
                }
                finally
                {
                    reader.Close();
                }
            }

            return optionAliases;
        }

        /// <summary>
        /// Gets the matrix field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static MatrixField GetMatrixField(string name, Guid userID)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name),"field must have value");

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_GetMatrixField");
            command.AddInParameter("FieldName", DbType.String, name);
            command.AddInParameter("UserID", DbType.Guid, userID);

            MatrixField matrixField = new MatrixField
            {
                FieldName = name,
                UserID = userID
            };

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int rowNumber = DbUtility.GetValueFromDataReader(reader, "RowNumber", 0);
                        int columnNumber = DbUtility.GetValueFromDataReader(reader, "ColumnNumber", 0);
                        string data = DbUtility.GetValueFromDataReader(reader, "Data", string.Empty);
                        bool isHeader = DbUtility.GetValueFromDataReader(reader, "IsHeader", false);
                        bool isRowHeader = DbUtility.GetValueFromDataReader(reader, "IsRowHeader", false);
                        bool userCell = DbUtility.GetValueFromDataReader(reader, "UserCell", false);
                        int columnWidth = DbUtility.GetValueFromDataReader(reader, "ColumnWidth", 0);
                        matrixField.IsRowsFixed = DbUtility.GetValueFromDataReader(reader, "IsRowsFixed", false);
                        matrixField.IsColumnsFixed = DbUtility.GetValueFromDataReader(reader, "IsColumnsFixed", false);
                        matrixField.Cells.Add(new Cell()
                        {
                            RowNumber = rowNumber,
                            ColumnNumber = columnNumber,
                            Data = data,
                            IsHeader = isHeader,
                            IsRowHeader= isRowHeader,
                            CustomUserCell = userCell,
                            ColumnWidth = columnWidth
                        });
                    }

                    reader.NextResult();

                    while (reader.Read())
                    {
                        matrixField.GridLines = DbUtility.GetValueFromDataReader(reader, "GridLines", String.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return matrixField;
        }

        /// <summary>
        /// Add a custom user field to the database.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="isDeletable"></param>
        /// <param name="isHidden"></param>
        public static void AddProfileField(string fieldName, bool isDeletable, bool isHidden)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper createPropertyCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_CreateProperty");
                        createPropertyCommand.AddInParameter("CustomUserFieldName", DbType.String, fieldName);
                        createPropertyCommand.AddInParameter("isDeletable", DbType.Boolean, isDeletable);
                        createPropertyCommand.AddInParameter("Hidden", DbType.Boolean, isHidden);

                        DBCommandWrapper dropViewCommand = db.GetSqlStringCommandWrapper("IF EXISTS (select * from sysobjects where name = 'ckbx_IdentityProfile' and type = 'V') DROP VIEW ckbx_IdentityProfile");
                        DBCommandWrapper createViewCommand = db.GetSqlStringCommandWrapper(BuildIdentityProfileViewString(fieldName, true));

                        db.ExecuteNonQuery(createPropertyCommand, transaction);
                        db.ExecuteNonQuery(dropViewCommand, transaction);
                        db.ExecuteNonQuery(createViewCommand, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                Logger.Write("Unable to add a custom user field with a null or blank name.", "Warning", 3, -1, Severity.Warning);
            }
        }

        /// <summary>
        /// Updates custom user field type
        /// </summary>
        /// <param name="property"></param>
        public static void UpdateProfileField(ProfileProperty property)
        {
            if (property != null)
            {
                if (!string.IsNullOrEmpty(property.Name))
                {
                    var db = DatabaseFactory.CreateDatabase();

                    using (var connection = db.GetConnection())
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();

                        try
                        {

                            //Clean up matrix field if exists 
                            //Clean up matrix field if exists 
                            var removeMatrixCellCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_RemoveMatrixField");
                            removeMatrixCellCmd.AddInParameter("@FieldName", DbType.String, property.Name);

                            db.ExecuteNonQuery(removeMatrixCellCmd, transaction);

                            var createPropertyCommand =
                                db.GetStoredProcCommandWrapper("ckbx_sp_Profile_UpdateField");

                            createPropertyCommand.AddInParameter("FieldName",
                                                                DbType.String,
                                                                property.Name);
                            createPropertyCommand.AddInParameter("FieldTypeName",
                                                                DbType.String,
                                                                property.FieldType.ToString());

                            db.ExecuteNonQuery(createPropertyCommand, transaction);

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else
                { 
                    Logger.Write("Unable to update a custom user field type with a null or blank name.", "Warning", 3, -1, Severity.Warning);
                }
            }
            else
            {
                Logger.Write("Unable to update custom user field with empty model", "Error", 3, -1, Severity.Warning);
            }
        }

        /// <summary>
        /// Adds radio button field
        /// </summary>
        /// <param name="radioButtonField"></param>
        /// <param name="userId"></param>
        public static void AddRadioButtonField(RadioButtonField radioButtonField, Guid? userId = null)
        {
            if (radioButtonField != null)
            {
                if (!string.IsNullOrEmpty(radioButtonField.Name))
                {
                    var db = DatabaseFactory.CreateDatabase();
                    using (var connection = db.GetConnection())
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();
                        try
                        {
                            //Clean up radio button field if exists 
                            var removeRadioOptionsCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_CleanUpRadioButtonField");
                            removeRadioOptionsCmd.AddInParameter("@FieldName", DbType.String, radioButtonField.Name);

                            db.ExecuteNonQuery(removeRadioOptionsCmd, transaction);

                            var createPropertyCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_UpdateField");
                            createPropertyCommand.AddInParameter("FieldName", DbType.String, radioButtonField.Name);
                            createPropertyCommand.AddInParameter("FieldTypeName", DbType.String, CustomFieldType.RadioButton.ToString());

                            db.ExecuteNonQuery(createPropertyCommand, transaction);

                            var addRadioCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddRadioField");
                            addRadioCmd.AddInParameter("PropertyName", DbType.String, radioButtonField.Name);
                            db.ExecuteNonQuery(addRadioCmd, transaction);

                            foreach (var item in radioButtonField.Options)
                            {
                                var addRadioOptionCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddRadioFieldOption");

                                addRadioOptionCmd.AddInParameter("UserID", DbType.Guid, userId);
                                addRadioOptionCmd.AddInParameter("FieldName", DbType.String, radioButtonField.Name);
                                addRadioOptionCmd.AddInParameter("Value", DbType.String, item.Name);

                                db.ExecuteNonQuery(addRadioOptionCmd, transaction);
                            }

                            // TO DO
                            // Add selected radio field option
                            var selectedOption = radioButtonField.Options.Where(o => o.IsSelected).FirstOrDefault();
                            if (selectedOption != null)
                            {
                                var selectedOptionName = selectedOption.Name;
                                var addSelectedOptionCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddSelectedRadioFieldOption");
                                addSelectedOptionCmd.AddInParameter("UserID", DbType.Guid, userId);
                                addSelectedOptionCmd.AddInParameter("SelectedOptionText", DbType.String, selectedOptionName);
                                addSelectedOptionCmd.AddInParameter("FieldName", DbType.String, radioButtonField.Name);

                                db.ExecuteNonQuery(addSelectedOptionCmd, transaction);
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes radio button field along with its options
        /// </summary>
        /// <param name="fieldName"></param>
        public static void DeleteRadioButtonOptions(string fieldName)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                var db = DatabaseFactory.CreateDatabase();
                using (var connection = db.GetConnection())
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    try
                    {
                        var removeRadioOptionsCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_CleanUpRadioButtonField");
                        removeRadioOptionsCmd.AddInParameter("@FieldName", DbType.String, fieldName);

                        db.ExecuteNonQuery(removeRadioOptionsCmd, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Adds or updates radio field options aliases for given item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="options"></param>
        public static void AddRadioButtonFieldOptionAlias(int itemId, List<RadioButtonFieldOption> options, string bindedPropertyName = null)
        {
            var db = DatabaseFactory.CreateDatabase();
            using (var connection = db.GetConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    DeleteRadioButtonFieldOptionAliases(itemId);

                    foreach (RadioButtonFieldOption option in options)
                    {
                        var addOptionAliasCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddRadioFieldOptionAlias");
                        addOptionAliasCmd.AddInParameter("@ItemId", DbType.Int32, itemId);
                        addOptionAliasCmd.AddInParameter("@RadioButtonOptionId", DbType.Int32, option.Id);
                        addOptionAliasCmd.AddInParameter("@Alias", DbType.String, option.Alias);

                        db.ExecuteNonQuery(addOptionAliasCmd, transaction);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();

                    var cacheManager = CacheFactory.GetCacheManager();
                    var progressKey = cacheManager.GetData("UploadProgressKey");

                    if (progressKey != null)
                    {
                        var data = cacheManager.GetData(progressKey.ToString());
                        List<string> listValues;

                        if (data != null)
                        {
                            listValues = data as List<string>;
                            listValues.Add(bindedPropertyName);
                        }
                        else
                        {
                            listValues = new List<string>();
                            listValues.Add(bindedPropertyName);
                        }

                        cacheManager.Add(progressKey.ToString(), listValues);
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Deletes all the option aliases for given item
        /// </summary>
        /// <param name="itemId"></param>
        public static void DeleteRadioButtonFieldOptionAliases(int itemId)
        {
            var db = DatabaseFactory.CreateDatabase();
            using (var connection = db.GetConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    var deleteOptionAliasCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_CleanUpRadioFieldOptionAlias");
                    deleteOptionAliasCmd.AddInParameter("@ItemId", DbType.Int32, itemId);

                    db.ExecuteNonQuery(deleteOptionAliasCmd, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Adds the type of the matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="withHeaders">if set to <c>true</c> [with headers].</param>
        public static void AddMatrixField(MatrixField matrix, bool withHeaders = true, Guid? userId = null)
        {
            if (matrix != null)
            {
                if (!string.IsNullOrEmpty(matrix.FieldName))
                {
                    var db = DatabaseFactory.CreateDatabase();

                    using (var connection = db.GetConnection())
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();

                        try
                        {

                            var createPropertyCommand =
                            db.GetStoredProcCommandWrapper("ckbx_sp_Profile_UpdateField");

                            //clean up the matrix field on adding new one
                            var removeMatrixCellCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_RemoveMatrixField");
                            removeMatrixCellCmd.AddInParameter("@FieldName", DbType.String, matrix.FieldName);

                            db.ExecuteNonQuery(removeMatrixCellCmd, transaction);


                            createPropertyCommand.AddInParameter("FieldName",
                                                                DbType.String,
                                                                matrix.FieldName);
                            createPropertyCommand.AddInParameter("FieldTypeName",
                                                                DbType.String,
                                                                CustomFieldType.Matrix.ToString());

                            db.ExecuteNonQuery(createPropertyCommand, transaction);

                            var addMatrixCmd =
                                db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddMatrixField");

                            addMatrixCmd.AddInParameter("PropertyName",
                                                                DbType.String,
                                                                matrix.FieldName);

                            addMatrixCmd.AddInParameter("IsRowsFixed",
                                                           DbType.Boolean,
                                                           matrix.IsRowsFixed);

                            addMatrixCmd.AddInParameter("IsColumnsFixed",
                                                           DbType.Boolean,
                                                           matrix.IsColumnsFixed);

                            addMatrixCmd.AddInParameter("GridLines",
                                                           DbType.String,
                                                           matrix.GridLines);

                            db.ExecuteNonQuery(addMatrixCmd, transaction);

                            AddMatrixCells(matrix, db, transaction, false);

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else
                {
                    Logger.Write("Unable to update a custom user field type with a null or blank name.", "Warning", 3, -1, Severity.Warning);
                }
            }
            else
            {
                Logger.Write("Unable to update custom user field with empty model", "Error", 3, -1, Severity.Warning);
            }
        }


        /// <summary>
        /// Adds the custom matrix cells.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="userId">The user identifier.</param>
        public static void AddCustomMatrixCells(MatrixField matrix, Guid userId)
        {
            if (matrix != null)
            {
                if (!string.IsNullOrEmpty(matrix.FieldName))
                {
                    var db = DatabaseFactory.CreateDatabase();

                    using (var connection = db.GetConnection())
                    {
                        connection.Open();
                        var transaction = connection.BeginTransaction();

                        try
                        {
                            //Clean up matrix field if exists 
                            var removeCustomMatrixCells = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_RemoveCustomMatrixCells");
                            removeCustomMatrixCells.AddInParameter("@FieldName", DbType.String, matrix.FieldName);
                            removeCustomMatrixCells.AddInParameter("@UserID", DbType.Guid, userId);

                            db.ExecuteNonQuery(removeCustomMatrixCells, transaction);

                            AddMatrixCells(matrix, db, transaction, true);

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
                else
                {
                    Logger.Write("Unable to update a custom user field type with a null or blank name.", "Warning", 3, -1, Severity.Warning);
                }
            }
            else
            {
                Logger.Write("Unable to update custom user field with empty model", "Error", 3, -1, Severity.Warning);
            }
        }

        /// <summary>
        /// Adds matrix cells.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="db">The database.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="customOnly">if set to <c>true</c> [custom only].</param>
        private static void AddMatrixCells(MatrixField matrix, Database db, IDbTransaction transaction, bool customOnly)
        {
            //filter cells by custom user cell field
            var cells = customOnly ? matrix.Cells.Where(item => item.CustomUserCell) : matrix.Cells;

            foreach (var item in cells)
            {
                var addMatrixCellsCmd = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_AddMatrixFieldCell");

                addMatrixCellsCmd.AddInParameter("UserID",
                    DbType.Guid,
                    matrix.UserID);


                addMatrixCellsCmd.AddInParameter("FieldName",
                    DbType.String,
                    matrix.FieldName);

                addMatrixCellsCmd.AddInParameter("RowNumber",
                    DbType.Int32,
                    item.RowNumber);

                addMatrixCellsCmd.AddInParameter("ColumnNumber",
                    DbType.Int32,
                    item.ColumnNumber);

                addMatrixCellsCmd.AddInParameter("Value",
                    DbType.String,
                    item.Data);

                addMatrixCellsCmd.AddInParameter("IsHeader",
                    DbType.String,
                    item.IsHeader);

                addMatrixCellsCmd.AddInParameter("IsRowHeader",
                    DbType.String,
                    item.IsRowHeader);
                
                addMatrixCellsCmd.AddInParameter("CustomUserCell",
                    DbType.Boolean,
                    item.CustomUserCell);

                addMatrixCellsCmd.AddInParameter("ColumnWidth",
                    DbType.Int32,
                    item.ColumnWidth);

                db.ExecuteNonQuery(addMatrixCellsCmd, transaction);
            }


        }

        /// <summary>
        /// Update the custom user field metadata table to delete the field name
        /// </summary>
        /// <param name="fieldName">Name of the field to delete.</param>
        /// <api>User Management</api>
        public static void DeleteProfileField(string fieldName)
        {
            if (!string.IsNullOrEmpty(fieldName))
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        ClearPropertyDefaultTextValues(fieldName);

                        DBCommandWrapper deletePropertyCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_DeleteProperty");
                        deletePropertyCommand.AddInParameter("CustomUserFieldName", DbType.String, fieldName);

                        DBCommandWrapper dropViewCommand = db.GetSqlStringCommandWrapper("IF EXISTS (select * from sysobjects where name = 'ckbx_IdentityProfile' and type = 'V') DROP VIEW ckbx_IdentityProfile");
                        DBCommandWrapper createViewCommand = db.GetSqlStringCommandWrapper(BuildIdentityProfileViewString(fieldName, false));

                        db.ExecuteNonQuery(deletePropertyCommand, transaction);
                        db.ExecuteNonQuery(dropViewCommand, transaction);
                        db.ExecuteNonQuery(createViewCommand, transaction);


                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        _profileCache.Flush();
                        connection.Close();
                    }
                }
            }
            else
            {
                Logger.Write("Unable to remove a custom user field with a null or blank name.", "Warning", 3, -1, Severity.Warning);
            }
        }


        /// <summary>
        /// Clears the property default text values.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private static void ClearPropertyDefaultTextValues(string propertyName)
        {
            var connectedItemids = PropertyBindingManager.GetBindedItemIdsByPropertyName(propertyName);

            if (connectedItemids != null && connectedItemids.Any())
            {
                foreach (var itemId in connectedItemids)
                    TextManager.SetText(string.Format(DefaultItemTextPattern, itemId), DefaultLanguageCode, string.Empty);
            }

        }

        /// <summary>
        /// Build the query to create the identity profile view
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        private static string BuildIdentityProfileViewString(string propName, bool add)
        {
            var sb = new StringBuilder();

            //First, get the properties list
            List<string> propertyList = ListPropertyNames();

            sb.Append("CREATE VIEW ckbx_IdentityProfile AS SELECT DISTINCT ckbx_Credential.UniqueIdentifier,ckbx_Credential.UserName,ckbx_Credential.Domain,ckbx_Credential.GUID,ckbx_Credential.Password");

            if (add && Utilities.IsNotNullOrEmpty(propName))
            {
                propertyList.Add(propName);
            }
            else if (!add && Utilities.IsNotNullOrEmpty(propName))
            {
                propertyList.Remove(propName);
            }

            //Now iterate and build the query
            foreach (string t in propertyList)
            {
                sb.Append(",(SELECT Value From ckbx_CustomUserFieldMap INNER JOIN ckbx_CustomUserField on ckbx_CustomUserField.CustomUserFieldID = ckbx_CustomUserFieldMap.CustomUserFieldID WHERE ckbx_CustomUserField.CustomUserFieldName = '" + DbUtility.EscapeValue(t, false) + "' AND ckbx_Credential.UniqueIdentifier = ckbx_CustomUserFieldMap.UniqueIdentifier) AS '" + DbUtility.EscapeValue(t, false) + "'");
            }

            sb.Append(" FROM ckbx_Credential LEFT OUTER JOIN ckbx_CustomUserFieldMap ON ckbx_CustomUserFieldMap.UniqueIdentifier = ckbx_Credential.UniqueIdentifier LEFT OUTER JOIN ckbx_CustomUserField ON ckbx_CustomUserField.CustomUserFieldID = ckbx_CustomUserFieldMap.CustomUserFieldID");

            return sb.ToString();
        }

        /// <summary>
        /// Update the custom user field metadata table to increment or decrement a property position.
        /// </summary>
        /// <param name="fieldName">Name of the field to move.</param>
        /// <param name="moveUp">If true, position is decremented, otherwise it is decremented.</param>
        /// <api>User Management</api>
        public static void MoveProfileField(string fieldName, bool moveUp)
        {
            if (Utilities.IsNotNullOrEmpty(fieldName))
            {
                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction t = connection.BeginTransaction();

                    try
                    {
                        DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_MoveProperty");
                        command.AddInParameter("CustomUserFieldName", DbType.String, fieldName);
                        command.AddInParameter("MoveUp", DbType.Boolean, moveUp);

                        db.ExecuteNonQuery(command, t);
                        t.Commit();
                    }
                    catch
                    {
                        t.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                Logger.Write("Unable to add a custom user field with a null or blank name.", "Warning", 3, -1, Severity.Warning);
            }
        }

        /// <summary>
        /// Determines if a proposed property name is valid.
        /// </summary>
        /// <param name="newProperty">The name of the new property</param>
        /// <returns></returns>
        public static bool IsValidFieldName(string newProperty)
        {
            if (Utilities.IsNullOrEmpty(newProperty))
                return false;

            //Don't allow adjacent single-quotes
            if (newProperty.IndexOf("''") >= 0)
                return false;

            //Don't allow spaces.
            if (newProperty.Contains(" "))
                return false;

            if (Regex.IsMatch(newProperty, @"[^a-zA-Z0-9_]"))
                return false;

            //Don't allow duplicates
            List<string> properties = ListPropertyNames();
            properties.Add("uniqueidentifier");
            properties.Add("username");
            properties.Add("domain");
            properties.Add("language");
            properties.Add("email");

            //add company properties
            properties.AddRange(CompanyProfileFacade.ListCompanyPropertyNames());

            return !properties.Contains(newProperty, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines if a custom user field is visible to users
        /// </summary>
        /// <param name="name">The name of the custom field</param>
        /// <returns></returns>
        public static bool IsFieldHidden(string name)
        {
            object value;
            Database db = DatabaseFactory.CreateDatabase();
            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetHidden");
                command.AddInParameter("CustomUserFieldName", DbType.String, name);

                value = db.ExecuteScalar(command);
            }

            //A custom field is only hidden if the value is explicitly set
            if (value == DBNull.Value)
            {
                return false;
            }

            bool isHidden;
            return Boolean.TryParse(value.ToString(), out isHidden) && isHidden;
        }

        /// <summary>
        /// Remove profile from profile cache
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void RemoveProfileFromCache(string uniqueIdentifier)
        {
            _profileCache.Remove(uniqueIdentifier);
        }

        /// <summary>
        /// Saves bulk of profile values into the database
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="profilePropertyValues"></param>
        /// <param name="selectedUserFields"></param>
        public static void UpdateUserProfile(string uniqueIdentifier, List<string> profilePropertyValues, List<string> selectedUserFields)
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction t = connection.BeginTransaction();

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Profile_UpdateUserProfile");
                    command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);
                    command.AddInParameter("Values", DbType.String, listToString(profilePropertyValues));
                    command.AddInParameter("Fields", DbType.String, listToString(selectedUserFields));

                    db.ExecuteNonQuery(command, t);
                    t.Commit();
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Returns profile property keys along with its id
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetProfileKeysWithIds(string customFieldType)
        {
            if (string.IsNullOrWhiteSpace(customFieldType))
                throw new ArgumentNullException(nameof(customFieldType));

            Dictionary<int, string> result = new Dictionary<int, string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetKeysAndIds");
            command.AddInParameter("@CustomFieldType", DbType.String, customFieldType);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int profileKeyId = reader.GetInt32(0);
                        string profileKey = reader.GetString(1);

                        if (Utilities.IsNotNullOrEmpty(profileKey))
                        {
                            result.Add(profileKeyId, profileKey);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
                return result;
            }
        }

        /// <summary>
        /// Returns custom field key and custom field value if it has been connected to a survey item
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetCustomFieldDataByItemId(int itemId, string userName)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetCustomFieldByItemId");
            command.AddInParameter("@ItemId", DbType.Int32, itemId);
            command.AddInParameter("@UniqueIdentifier", DbType.String, userName);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string profileKey = reader.GetString(0);
                        string profileValue = reader.GetString(1);

                        result[profileKey] = !string.IsNullOrWhiteSpace(profileKey) ? profileValue : string.Empty;
                    }
                }
                finally
                {
                    reader.Close();
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the name of the connected profile field.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns></returns>
        public static string GetConnectedProfileFieldName(int itemId)
        {
            string result = string.Empty;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_GetConnectedProfileFieldNameById");
            command.AddInParameter("@ItemId", DbType.Int32, itemId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string propertyName = reader.GetString(0);

                        if (!string.IsNullOrEmpty(propertyName))
                            result =  propertyName;
                    }
                }
                finally
                {
                    reader.Close();
                }

                return result;
            }
        }

        /// <summary>
        /// Determines whether [is item binded to] [the specified item identifier].
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="profilePropertyId">The profile property identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is item binded to] [the specified item identifier]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsItemBindedTo(int itemId, int profilePropertyId)
        {
           
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Item_IsItemBindedToProfileField");
            command.AddInParameter("@ItemId", DbType.Int32, itemId);
            command.AddInParameter("@PropertyId", DbType.Int32, profilePropertyId);

            var scalarValue = db.ExecuteScalar(command);

            if (db.GetConnection() != null)
                db.GetConnection().Close();

            if (scalarValue != null)
                return (bool) scalarValue;

            return false;
        }

        /// <summary>
        /// Lists to string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private static string listToString(List<string> values)
        {
            var s = new StringBuilder();

            foreach (var v in values)
            {
                s.AppendFormat("{0},", v.Replace(",", "~c~"));
            }
            return s.ToString();
        }

        /// <summary>
        /// Unbind item from Profile filed
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool UnbindItemFromProfileField(int itemId)
        {
           Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction t = connection.BeginTransaction();

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_DeleteItemFieldMapping");
                    command.AddInParameter("ItemID", DbType.Int32, itemId);

                    db.ExecuteNonQuery(command, t);
                    t.Commit();
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return true;
        }


        /// <summary>
        /// Clears the matrix rows and columns.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        public static void ClearMatrixRowsAndColumns(int itemId)
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Matrix_DeleteRowsAndColumns");
                    command.AddInParameter("ItemId", DbType.Int32, itemId);

                    db.ExecuteNonQuery(command);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Updates radio button field selected option
        /// </summary>
        /// <param name="optionText"></param>
        /// <param name="propertyKey"></param>
        /// <param name="user"></param>
        public static void UpdateRadioFieldSelectedOption(string optionText, string propertyKey, Guid user)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_CustomUserField_UpdateSelectedRadioOption");
            command.AddInParameter("@OptionText", DbType.String, optionText);
            command.AddInParameter("@FieldName", DbType.String, propertyKey);
            command.AddInParameter("@UserID", DbType.Guid, user);
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
