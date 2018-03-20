using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.Security;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Providers;
using Checkbox.Users;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using System.Security.Principal;

namespace Checkbox.Web.Providers
{
    /// <summary>
    /// Checkbox-specific membership provider to plug into ASP.NET provider model.
    /// </summary>
    /// <remarks>
    /// To maintain compatibility with non-ASP.NET uses of Checkbox application code, core membership functions
    /// are performed by Checkbox.Security.Providers.BaseMembershipProvider.  This class proxies calls to the 
    /// BaseMembershipProvider which uses configured IAuthenticationProviders for authentication and implements
    /// some update functionality that was present in the UserManager class prior to 5.x releases.
    /// </remarks>
    public class CheckboxMembershipProvider : MembershipProvider, ICheckboxMembershipProvider
    {
        private const string MEMBERSHIP_DATABASE_NAME = "CheckboxMembershipProvider";

        private const string CHECKBOX_NETWORK_USER = "CheckboxNetworkUser";
        private const string CHECKBOX_PASSWORD_USER = "CheckboxPasswordUser";
        private const string EXTERNAL_USER = "ExternalUser";


        //Backing fields for implementation of abstract properties
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        private string _passwordStrengthRegularExpression;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;

        private object _lockObject = new Object();
        //expiration time of user non profile properties cache in seconds
        private const int expirationTime = 15;

        private static CacheManager _userIdentityCache;
        private static object _lockCacheObject = new Object();
        /// <summary>
        /// Dictionary that contains all users.
        /// The key is an application context.
        /// </summary>
        private Dictionary<string, List<string>> _allUsersList = new Dictionary<string, List<string>>();

        /// <summary>
        /// Get/set application name for use with membership provider
        /// </summary>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override bool EnablePasswordReset { get { return _enablePasswordReset; } }

        /// <summary>
        /// 
        /// </summary>
        public override bool EnablePasswordRetrieval { get { return _enablePasswordRetrieval; } }

        /// <summary>
        /// 
        /// </summary>
        public override int MaxInvalidPasswordAttempts { get { return _maxInvalidPasswordAttempts; } }

        /// <summary>
        /// 
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters { get { return _minRequiredNonAlphanumericCharacters; } }

        /// <summary>
        /// 
        /// </summary>
        public override int MinRequiredPasswordLength { get { return _minRequiredPasswordLength; } }

        /// <summary>
        /// 
        /// </summary>
        public override int PasswordAttemptWindow { get { return _passwordAttemptWindow; } }

        /// <summary>
        /// 
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat { get { return _passwordFormat; } }

        /// <summary>
        /// 
        /// </summary>
        public override string PasswordStrengthRegularExpression { get { return _passwordStrengthRegularExpression; } }

        /// <summary>
        /// 
        /// </summary>
        public override bool RequiresQuestionAndAnswer { get { return _requiresQuestionAndAnswer; } }

        /// <summary>
        /// 
        /// </summary>
        public override bool RequiresUniqueEmail { get { return _requiresUniqueEmail; } }

        /// <summary>
        /// Checkbox membership provider
        /// </summary>
        public string ProviderName { get { return MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME; } }

        private List<string> AllUsersList
        {
            get
            {
                //TODO: WebFarm safe caching
                if (!_allUsersList.ContainsKey(ApplicationManager.ApplicationDataContext))
                {
                    lock (_lockObject)
                    {
                        _allUsersList.Add(ApplicationManager.ApplicationDataContext, new List<string>());
                    }
                }
                return _allUsersList[ApplicationManager.ApplicationDataContext];
            }
            set
            {
                lock (_lockObject)
                {
                    _allUsersList[ApplicationManager.ApplicationDataContext] = value;
                }
            }
        }

        /// <summary>
        /// Initialize the provider with its configuration data.
        /// </summary>
        /// <param name="name">Name of configuration provider.</param>
        /// <param name="config">NameValueCollection with configuration settings.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            //Call base
            base.Initialize(name, config);

            //Store configuration values
            _enablePasswordReset = Utilities.AsBool(config["enablePasswordReset"], false);
            _enablePasswordRetrieval = Utilities.AsBool(config["enablePasswordRetrieval"], false);
            _maxInvalidPasswordAttempts = Utilities.AsInt(config["maxInvalidPasswordAttempts"], -1);
            _minRequiredNonAlphanumericCharacters = Utilities.AsInt(config["minRequiredNonAlphanumericCharacters"], -1);
            _minRequiredPasswordLength = Utilities.AsInt(config["minRequiredPasswordLength"], -1);
            _passwordAttemptWindow = Utilities.AsInt(config["passwordAttemptWindow"], -1);
            _passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            _requiresQuestionAndAnswer = Utilities.AsBool(config["requiresQuestionAndAnswer"], false);
            _requiresUniqueEmail = Utilities.AsBool(config["requiresUniqueEmail"], false);

            if (config["passwordFormat"] != null)
            {
                try
                {
                    _passwordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config["passwordFormat"]);
                }
                catch
                {
                    _passwordFormat = MembershipPasswordFormat.Clear;
                }
            }

            _userIdentityCache = CacheFactory.GetCacheManager("userNonProfilePropertiesCacheManager");
        }

        public void CacheProfile(string uniqueIdentifier, Dictionary<string, object> profile)
        {
            _userIdentityCache.Add(uniqueIdentifier, profile);
        }

        /// <summary>
        /// Remove user from cache with user non profile properties
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public void ExpireCachedUserNonProfileProperties(string uniqueIdentifier)
        {
            if (_userIdentityCache.Contains(uniqueIdentifier))
            {
                lock (_lockCacheObject)
                {
                    //Re check in case item removed betweeen first check and lock
                    if (_userIdentityCache.Contains(uniqueIdentifier))
                    {
                        _userIdentityCache.Remove(uniqueIdentifier);
                    }
                }
            }
        }

        /// <summary>
        /// Change a user's password
        /// </summary>
        /// <param name="username">User name of user to change password for.</param>
        /// <param name="oldPassword">User's previous password.</param>
        /// <param name="newPassword">User's new password.</param>
        /// <returns>Boolean indicating if password change was successful.</returns>
        /// <remarks>Old password is not verified before changing the password.</remarks>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Credential_ChangePassword");
            command.AddInParameter("UniqueIdentifier", DbType.String, newPassword);
            command.AddInParameter("Password", DbType.String, newPassword);

            db.ExecuteNonQuery(command);

            return true;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a new user with the specified parameters.
        /// </summary>
        /// <param name="uniqueIdentifier">Username for new user.</param>
        /// <param name="password">Password for new user.</param>
        /// <param name="email">New user's email address.</param>
        /// <param name="passwordQuestion">User's reset password question.</param>
        /// <param name="passwordAnswer">User's reset password answer.</param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <remarks>Password question/answer, approval, and provider user key are not supported.</remarks>
        public override MembershipUser CreateUser(string uniqueIdentifier, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            string statusText;

            //Call base membership provider to do work.
            string createdUserUniqueIdentifier = CreateUser(
                uniqueIdentifier,
                password,
                email,
                passwordQuestion,
                passwordAnswer,
                isApproved,
                providerUserKey,
                null,
                out statusText);

            //Check for failure
            if (Utilities.IsNullOrEmpty(createdUserUniqueIdentifier))
            {
                //TODO: Use status text to set more specific error condition
                status = MembershipCreateStatus.ProviderError;
                return null;
            }

            //Otherwise return membership user
            status = MembershipCreateStatus.Success;

            DateTime now = DateTime.Now;

            return new MembershipUser(
                Name,
                createdUserUniqueIdentifier,
                null,
                email,
                null,
                null,
                true,
                false,
                now,
                now,
                now,
                now,
                now);
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        /// <param name="providerUserKey"></param>
        /// <param name="statusText"></param>
        /// <returns></returns>
        public string CreateUser(string uniqueIdentifier, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, string creator, out string statusText)
        {
            var userGuid = Guid.NewGuid();

            //TODO: Validate password, etc.
            if (ApplicationManager.AppSettings.UseEncryption && password != null)
            {
                password = Encryption.HashString(password, userGuid.ToString());
            }

            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);

            string[] splitName = SplitUniqueIdentifier(uniqueIdentifier);

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_Create");
            command.AddInParameter("UserName", DbType.String, splitName[0]);
            command.AddInParameter("Domain", DbType.String, splitName[1]);
            command.AddInParameter("Password", DbType.String, password);
            command.AddInParameter("Email", DbType.String, email);
            command.AddInParameter("Guid", DbType.Guid, userGuid);
            command.AddInParameter("Encrypted", DbType.Boolean, ApplicationManager.AppSettings.UseEncryption);
            command.AddInParameter("CreatedBy", DbType.String, creator);
            command.AddOutParameter("UniqueIdentifier", DbType.String, 1222);
            command.AddOutParameter("StatusMessage", DbType.String, 1222);

            //Execute
            db.ExecuteNonQuery(command);

            //Read out values
            object uniqueIdentifierObject = command.GetParameterValue("UniqueIdentifier");
            object statusObject = command.GetParameterValue("StatusMessage");

            //Set status and ensure a non-null value
            statusText = (statusObject as string) ?? string.Empty;

            var userIdAsString = uniqueIdentifierObject as string;

            if (string.IsNullOrEmpty(userIdAsString))
            {
                return string.Empty;
            }

            //Clear AllUserList so that it will be reloaded.  Attempting to add new user and resort
            // is too time consuming when importing users.
            AllUsersList = new List<string>();
 

            //Return unique identifier and ensure a non-null value
            return (uniqueIdentifierObject as string) ?? string.Empty;
        }

        /// <summary>
        /// Split the unique identifier for a user into domain and user name.
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns>Two element array with user name as first element and domain as second.</returns>
        private static string[] SplitUniqueIdentifier(string uniqueIdentifier)
        {
            if (uniqueIdentifier.Contains("/")
                || uniqueIdentifier.Contains(@"\"))
            {
                //Unique id has form DOMAIN\User or possibly DOMAIN/User 
                string[] parts = uniqueIdentifier.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 1)
                {
                    return new[] { parts[1].Trim(), parts[0].Trim() };
                }
            }

            //Return user name with no domain.
            return new[] { uniqueIdentifier.Trim(), string.Empty };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string userName, bool deleteAllRelatedData)
        {
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                bool returnVal;

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_Delete");
                    command.AddInParameter("UniqueIdentifier", DbType.String, userName);
                    command.AddInParameter("DeleteResponses", DbType.Boolean, deleteAllRelatedData);
                    command.AddOutParameter("StatusMessage", DbType.String, 255);

                    db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();

                    returnVal =
                        command.GetParameterValue("StatusMessage") == DBNull.Value
                        || Utilities.IsNullOrEmpty((string)command.GetParameterValue("StatusMessage"));
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

                //Remove users from list
                if (AllUsersList.BinarySearch(userName) >= 0)
                {
                    lock (_lockObject)
                    {
                        AllUsersList.Remove(userName);
                    }
                }

                return returnVal;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public string[] ListUsersByEmail(string emailToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords)
        {
            var paginationContext = new PaginationContext
            {
                FilterField = "Email",
                FilterValue = emailToMatch,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending
            };

            var result = ListAllUsers(paginationContext);

            totalRecords = paginationContext.ItemCount;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public string[] ListUsersByName(string usernameToMatch, int pageIndex, int pageSize, string sortField, bool sortAscending, out int totalRecords)
        {
            var paginationContext = new PaginationContext
            {
                FilterField = "UniqueIdentifier",
                FilterValue = usernameToMatch,
                CurrentPage = pageIndex,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending
            };

            var result = ListAllUsers(paginationContext);

            totalRecords = paginationContext.ItemCount;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            string[] userList = ListUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);

            var userCollection = new MembershipUserCollection();

            foreach (string userName in userList)
            {
                MembershipUser user = GetUser(userName, false);

                if (user != null)
                {
                    userCollection.Add(user);
                }
            }

            return userCollection;
        }

        /// <summary>
        /// Find users with the specified email address
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public string[] ListUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var paginationContext = new PaginationContext
            {
                FilterField = "Email",
                FilterValue = emailToMatch,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };

            var result = ListAllUsers(paginationContext);

            totalRecords = paginationContext.ItemCount;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return BuildMembershipUserCollectionUserList(ListUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords));
        }

        /// <summary>
        /// List users by name
        /// </summary>
        /// <param name="userNameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public string[] ListUsersByName(string userNameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var paginationContext = new PaginationContext
            {
                FilterField = "UniqueIdentifier",
                FilterValue = userNameToMatch,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };

            var result = ListAllUsers(paginationContext);

            totalRecords = paginationContext.ItemCount;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        private MembershipUserCollection BuildMembershipUserCollectionUserList(IEnumerable<string> userList)
        {
            //Build user collection
            var userCollection = new MembershipUserCollection();

            foreach (string userName in userList)
            {
                MembershipUser user = GetUser(userName, false);

                if (user != null)
                {
                    userCollection.Add(user);
                }
            }

            return userCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var paginationContext = new PaginationContext { CurrentPage = pageIndex, PageSize = pageSize };

            var result = BuildMembershipUserCollectionUserList(ListAllUsers(paginationContext));

            totalRecords = paginationContext.ItemCount;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] ListAllUsers(PaginationContext paginationContext)
        {
            //Store "All users" only if not paged or filtered
            if (AllUsersList.Count > 0
                && (paginationContext.PageSize <= 0 || paginationContext.CurrentPage <= 0)
                && !paginationContext.IsFiltered)
            {
                paginationContext.ItemCount = AllUsersList.Count;
                return AllUsersList.ToArray();
            }

            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_List");

            //validate parameters
            string sortField = paginationContext.SortField;
            switch (sortField)
            {
                case "UniqueIdentifier":
                case "Email":
                    break;
                default:
                    sortField = "UniqueIdentifier";
                    break;
            }

            var dateFieldName = paginationContext.DateFieldName;
            switch (dateFieldName)
            {
                case "Created":
                case "ModifiedDate":
                    break;
                default:
                    dateFieldName = string.Empty;
                    break;
            }

            command.AddInParameter("PageIndex", DbType.Int32, paginationContext.CurrentPage);
            command.AddInParameter("PageSize", DbType.Int32, paginationContext.PageSize);
            command.AddInParameter("SortField", DbType.String, sortField);
            command.AddInParameter("SortAscending", DbType.Boolean, paginationContext.SortAscending);
            command.AddInParameter("FilterField", DbType.String, paginationContext.FilterField);
            command.AddInParameter("FilterValue", DbType.String, paginationContext.FilterValue);
            command.AddInParameter("StartDate", DbType.DateTime, paginationContext.StartDate);
            command.AddInParameter("DateFieldName", DbType.String, dateFieldName);

            int totalItemCount;

            var userList = BuildUserListFromDbCommand(db, command, out totalItemCount);

            paginationContext.ItemCount = totalItemCount;

            if (userList.Count == totalItemCount && !paginationContext.IsFiltered)
            {
                AllUsersList = userList;
            }

            return userList.ToArray();
        }

        public string[] ListOnlyTenantUsers(PaginationContext paginationContext)
        {
            //Store "All users" only if not paged or filtered
            if (AllUsersList.Count > 0
                && (paginationContext.PageSize <= 0 || paginationContext.CurrentPage <= 0)
                && !paginationContext.IsFiltered)
            {
                paginationContext.ItemCount = AllUsersList.Count;
                return AllUsersList.ToArray();
            }

            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_GetOnlyTenantUsers");


            command.AddInParameter("PageIndex", DbType.Int32, paginationContext.CurrentPage);
            command.AddInParameter("PageSize", DbType.Int32, paginationContext.PageSize);

            int totalItemCount;

            var userList = BuildUserListFromDbCommand(db, command, out totalItemCount);

            paginationContext.ItemCount = totalItemCount;

            if (userList.Count == totalItemCount && !paginationContext.IsFiltered)
            {
                AllUsersList = userList;
            }

            return userList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        /// <remarks>Answer parameter is not used.</remarks>
        public override string GetPassword(string username, string answer)
        {
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_GetPassword");

            command.AddInParameter("UniqueIdentifier", DbType.String, username);
            command.AddOutParameter("Password", DbType.String, 255);

            db.ExecuteNonQuery(command);

            object pwObj = command.GetParameterValue("Password");

            if (pwObj == null || pwObj == DBNull.Value)
            {
                return string.Empty;
            }

            //Return password
            return pwObj.ToString();
        }

        /// <summary>
        /// Get a dictionary containing the intrinsic (i.e. non-profile) properties of a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetUserIntrinsicProperties(string userName)
        {
            return GetUserIntrinsicProperties(userName, ApplicationManager.AppSettings.CacheUserNonProfileProperties);
        }

        /// <summary>
        /// Get a dictionary containing the intrinsic (i.e. non-profile) properties of a user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="useCache"> </param>
        /// <returns></returns>
        public Dictionary<string, object> GetUserIntrinsicProperties(string userName, bool useCache)
        {
            //Check cache
            if (useCache && _userIdentityCache.Contains(userName))
            {
                Dictionary<string, object> result = (Dictionary<string, object>) _userIdentityCache.GetData(userName);
                if (result != null)
                    return result;
            }
            
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Identity_Get");
            command.AddInParameter("UniqueIdentifier", DbType.String, userName);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //TODO: Other dates should be creation date when no value exists.  Defined in MSDN docs for 
                    // membership providers.
                    if (reader.Read())
                    {
                        var userPropertiesDictionary = BuildUserPropertiesDictionaryFromReader(reader);
                        CacheProfile(userName, userPropertiesDictionary);
                        return (Dictionary<string, object>)_userIdentityCache[userName];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Build a dictionary of user intrinsic properties from a data reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static Dictionary<string, object> BuildUserPropertiesDictionaryFromReader(IDataReader reader)
        {
            var propertiesDictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            DateTime creationDate = DbUtility.GetValueFromDataReader(reader, "Created", DateTime.Now);

            propertiesDictionary["ProviderName"] = MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME;
            propertiesDictionary["UniqueIdentifier"] = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);
            propertiesDictionary["GUID"] = DbUtility.GetValueFromDataReader(reader, "GUID", Guid.Empty).ToString();
            propertiesDictionary["Email"] = DbUtility.GetValueFromDataReader(reader, "Email", string.Empty);
            propertiesDictionary["Created"] = creationDate;
            propertiesDictionary["LastLogin"] = DbUtility.GetValueFromDataReader(reader, "LastLogin", creationDate);
            propertiesDictionary["LastActivity"] = DbUtility.GetValueFromDataReader(reader, "LastActivity", creationDate);
            propertiesDictionary["LastPasswordChange"] = DbUtility.GetValueFromDataReader(reader, "LastPasswordChange", creationDate);
            propertiesDictionary["LastLockedOut"] = DbUtility.GetValueFromDataReader(reader, "LastLockedOut", creationDate);
            propertiesDictionary["Password"] = DbUtility.GetValueFromDataReader(reader, "Password", string.Empty);
            propertiesDictionary["Domain"] = DbUtility.GetValueFromDataReader(reader, "Domain", string.Empty);
            //suppress the errors otherwise patching will be unavailable
            try
            {
                propertiesDictionary["FailedLogins"] = DbUtility.GetValueFromDataReader(reader, "FailedLogins", 0);
                propertiesDictionary["LockedOut"] = DbUtility.GetValueFromDataReader(reader, "LockedOut", false);
            }
            catch (Exception)
            {
            }

            return propertiesDictionary;
        }

        /// <summary>
        /// Updates locked-out state for the user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fails"></param>
        /// <param name="lockedOut"></param>
        private void UpdateLockOut(string userName, int fails, bool lockedOut)
        {
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_UpdateLockOut");
            command.AddInParameter("UserName", DbType.String, userName);
            command.AddInParameter("FailedLogins", DbType.Int32, fails);
            command.AddInParameter("LockedOut", DbType.Boolean, lockedOut);

            db.ExecuteNonQuery(command);

            ExpireCachedUserNonProfileProperties(userName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(string userName, bool userIsOnline)
        {
            Dictionary<string, object> userProperties = GetUserIntrinsicProperties(userName);

            if (userProperties == null)
            {
                return null;
            }

            //Store "Now" time to use as default value
            DateTime now = DateTime.Now;

            string userType = userProperties.ContainsKey("Domain") && Utilities.IsNotNullOrEmpty(userProperties["Domain"].ToString())
                ? CHECKBOX_NETWORK_USER
                : CHECKBOX_PASSWORD_USER;

            return new MembershipUser(
                Name,
                userProperties.ContainsKey("UniqueIdentifier") ? userProperties["UniqueIdentifier"] as string : string.Empty,
                userProperties.ContainsKey("GUID") ? new Guid((string)userProperties["GUID"]) : Guid.Empty,
                userProperties.ContainsKey("Email") ? userProperties["Email"] as string : string.Empty,
                null,
                userType,
                true,
                userProperties.ContainsKey("LockedOut") ? (bool)userProperties["LockedOut"] : false,
                userProperties.ContainsKey("Created") ? (DateTime)userProperties["Created"] : now,
                userProperties.ContainsKey("LastLogin") ? (DateTime)userProperties["LastLogin"] : now,
                userProperties.ContainsKey("LastActivity") ? (DateTime)userProperties["LastActivity"] : now,
                userProperties.ContainsKey("LastPasswordChange") ? (DateTime)userProperties["LastPasswordChange"] : now,
                userProperties.ContainsKey("LastLockedOut") ? (DateTime)userProperties["LastLockedOut"] : now);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerUserKey"></param>
        /// <param name="userIsOnline"></param>
        /// <returns></returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return GetUser(providerUserKey.ToString(), userIsOnline);
        }

        /// <summary>
        /// Get a user's name from email
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public string GetUserNameFromGuid(Guid userGuid)
        {
            Database db = DatabaseFactory.CreateDatabase(MEMBERSHIP_DATABASE_NAME);
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_GetByGuid");
            command.AddInParameter("Guid", DbType.Guid, userGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override string GetUserNameByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return string.Empty;
            }

            int recordCount;

            var resultList = ListUsersByEmail(email, -1, -1, out recordCount);

            return resultList.FirstOrDefault(val => email.Equals(val, StringComparison.InvariantCultureIgnoreCase)) ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public override void UpdateUser(MembershipUser user)
        {
            UpdateUser(user, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(MembershipUser user, string modifier)
        {
            string status;

            UpdateUser(
                user.UserName,
                user.UserName,
                string.Empty,
                string.Empty,
                user.Email,
                modifier,
                out status);
        }


        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="userUniqueIdentifier"></param>
        /// <param name="newUserName"></param>
        /// <param name="newDomain"></param>
        /// <param name="newPassword"></param>
        /// <param name="newEmailAddress"></param>
        /// <param name="status"></param>
        public bool UpdateUser(string userUniqueIdentifier, string newUserName, string newDomain, string newPassword, string newEmailAddress, string modifier, out string status)
        {
            var user = GetUser(userUniqueIdentifier, false);

            //Make sure a user name was passed.
            if (Utilities.IsNullOrEmpty(userUniqueIdentifier))
            {
                status = "No user name specified.";
                return false;
            }

            if (Utilities.IsNullOrEmpty(newUserName))
            {
                newUserName = userUniqueIdentifier;
            }

            if (Utilities.IsNullOrEmpty(newUserName.Replace("'", string.Empty)))
            {
                status = "User name must contain at least one alphanumeric character.";
                return false;
            }

            //Prevent blank passwords so set password = null (= no change) if empty value passed
            if (newPassword == string.Empty)
            {
                newPassword = null;
            }

            //Hash new password if necessary
            if (ApplicationManager.AppSettings.UseEncryption && newPassword != null)
            {
                newPassword = Encryption.HashString(newPassword, user.ProviderUserKey.ToString());
            }

            //Construct uniqueidentifier by appending domain if present.
            string newUserUniqueIdentifier = Utilities.IsNullOrEmpty(newDomain)
                ? newUserName
                : string.Format("{0}/{1}", newDomain, newUserName);

            //Create a transaction to wrap update sproc. call which calls
            // other stored procedures.
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    bool rename = false;
                    //If newUserName != oldUserName, then user is being renamed.  Make sure a user with
                    // the new name doesn't already exist.
                    if (!newUserUniqueIdentifier.Equals(userUniqueIdentifier, StringComparison.InvariantCultureIgnoreCase))
                    {
                        rename = true;
                        DBCommandWrapper exists = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_Exists");
                        exists.AddInParameter("UniqueIdentifier", DbType.String, newUserUniqueIdentifier);

                        var count = (int)db.ExecuteScalar(exists, transaction);

                        if (count != 0) // uniqueness conflict, break
                        {
                            transaction.Rollback();
                            status = "USERNOTUNIQUE";
                            return false;
                        }

                    }

                    // if we reach this point in the transaction, the username is unique.  Proceed with update.
                    DBCommandWrapper update = db.GetStoredProcCommandWrapper("ckbx_sp_Identity_Update");
                    update.AddInParameter("OldUniqueIdentifier", DbType.String, userUniqueIdentifier);
                    update.AddInParameter("NewUniqueIdentifier", DbType.String, newUserUniqueIdentifier);
                    update.AddInParameter("UserName", DbType.String, newUserName);
                    update.AddInParameter("Domain", DbType.String, newDomain);
                    update.AddInParameter("Password", DbType.String, newPassword);
                    update.AddInParameter("Email", DbType.String, newEmailAddress);
                    update.AddInParameter("ModifiedBy", DbType.String, modifier);
                    update.AddInParameter("Encrypted", DbType.Int32, ApplicationManager.AppSettings.UseEncryption ? 1 : 0);

                    db.ExecuteNonQuery(update, transaction);

                    transaction.Commit();

                    //If renaming, update all users list
                    if (rename)
                    {
                        lock (_lockObject)
                        {
                            AllUsersList.Remove(userUniqueIdentifier);
                            AllUsersList.Add(newUserUniqueIdentifier);
                            AllUsersList.Sort();
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionPolicy.HandleException(ex, "BusinessPublic");
                    status = "An error occurred while updating the user.";

                    return false;
                }
            }

            status = "Update successful.";
            ExpireCachedUserNonProfileProperties(userUniqueIdentifier);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override bool ValidateUser(string userName, string password)
        {
            //If user name or password are not specified, return false.
            if (Utilities.IsNullOrEmpty(userName)
                || Utilities.IsNullOrEmpty(password))
            {
                return false;
            }

            Dictionary<string, object> userProperties = GetUserIntrinsicProperties(userName, false);

            if (userProperties != null)
            {
                //If a user is a "network user" and in Checkbox, user is authenticated.
                if (UserManager.IsDomainUser(userName))
                {
                    return true;
                }

                bool locked = !userProperties.ContainsKey("LockedOut") ? false : (bool)userProperties["LockedOut"];
                if (locked)
                    return false;

                //If no password in collection, something went wrong so access should be denied
                if (!userProperties.ContainsKey("Password"))
                {
                    return false;
                }

                //For other users, get password and compare
                var storedPassword = userProperties["Password"] as string;

                if (Utilities.IsNotNullOrEmpty(storedPassword))
                {

                    //If necessary, "encrypt" password
                    if (ApplicationManager.AppSettings.UseEncryption)
                    {
                        //Framework 2.0 and 1.1 had slightly different hash functions, so we need to compare
                        // hashed password stored in db with both versions to support customers that hashed
                        // strings using Ultimate Survey .NET, which ran on framework 1.1
                        //Check both strings and return

                        if (storedPassword.Equals(Encryption.HashString(password, userProperties["GUID"] as string))
                            || storedPassword.Equals(Encryption.HashOldString(password))
                            || storedPassword.Equals(Encryption.HashStringDotNetOneFormat(password)))
                        {
                            if (ApplicationManager.AppSettings.MaxFailedLoginAttempts > 0)
                                UpdateLockOut(userName, 0, false);
                            return true;
                        }
                    }

                    //Perform normal string comparison
                    if (storedPassword.Equals(password))
                    {
                        if (ApplicationManager.AppSettings.MaxFailedLoginAttempts > 0)
                            UpdateLockOut(userName, 0, false);
                        return true;
                    }
                }

                int fails = !userProperties.ContainsKey("FailedLogins") ? 0 : (int)userProperties["FailedLogins"];
                if (ApplicationManager.AppSettings.MaxFailedLoginAttempts > 0)
                {
                    UpdateLockOut(userName, ++fails, fails >= ApplicationManager.AppSettings.MaxFailedLoginAttempts);
                    //force cache to update on the next call
                    if (fails >= ApplicationManager.AppSettings.MaxFailedLoginAttempts)
                        UserManager.ExpireCachedPrincipal(userName);
                }
            }

            //If we got here, then something went wrong and authentication should fail.
            return false;
        }

        /// <summary>
        /// Create a list of uniqueidentifiers from a data reader.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="command"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        private static List<string> BuildUserListFromDbCommand(Database db, DBCommandWrapper command, out int totalRecords)
        {
            var userList = new List<string>();
            totalRecords = 0;

            if (db == null || command == null)
            {
                return userList;
            }

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        userList.Add(DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty));
                    }

                    if (reader.NextResult()
                        && reader.Read())
                    {
                        totalRecords = DbUtility.GetValueFromDataReader(reader, "TotalRecords", 0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return userList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueIdentifer"></param>
        /// <returns></returns>
        public IIdentity GetUserIdentity(string uniqueIdentifer)
        {
            MembershipUser user = GetUser(uniqueIdentifer, false);

            if (user != null)
            {
                //Use auth type to represent user as a checkbox password/network user, or other
                return new GenericIdentity(uniqueIdentifer, user.Comment);
            }

            return null;
        }
    }
}
