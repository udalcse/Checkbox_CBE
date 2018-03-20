//using System;
//using System.DirectoryServices;
//using System.Security.Principal;

//using Checkbox.Management;

//using Prezza.Framework.Common;
//using Prezza.Framework.Configuration;
//using Prezza.Framework.Logging;
//using Prezza.Framework.Security;

//namespace Checkbox.ActiveDirectory.Providers
//{
//    /// <summary>
//    /// An authentication provider which provides support for authenticating users via the 
//    /// lightweight directory access protocol (LDAP).
//    /// </summary>
//    public class LDAPAuthenticationProvider : IAuthenticationProvider
//    {
//        /// <summary>
//        /// Provider configuration
//        /// </summary>
//        private LDAPAuthenticationProviderData config;

//        /// <summary>
//        /// Provider name. Required for IConfigurationProvider interface
//        /// </summary>
//        private string configurationName;

//        #region IAuthenticationProvider Members

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="credentials"></param>
//        /// <param name="identity"></param>
//        /// <returns></returns>
//        public bool Authenticate(object credentials, out IIdentity identity)
//        {
//            Logger.Write("Authenticating based on credentials type: " + credentials.GetType(), "Info", 1, -1, Severity.Information);

//            //Validate arguments
//            ArgumentValidation.CheckForNullReference(credentials, "credentials");

//            //Validate that we received the proper credentials
//            if (!(credentials is NamePasswordCredential))
//            {
//                throw new ArgumentException("Passed in credentials object was not the expected type [NamePasswordCredential].");
//            }

//            if (!Authenticate((NamePasswordCredential)credentials))
//            {
//                identity = null;
//                if (((NamePasswordCredential)credentials).Name != null)
//                {
//                    Logger.Write("Authentication failed for " + ((NamePasswordCredential)credentials).Name, "Warning", 1, -1, Severity.Warning);
//                }
//                else
//                {
//                    Logger.Write("Authentication failed for [NULL name].", "Warning", 1, -1, Severity.Warning);
//                }

//                return false;
//            }
//            else
//            {
//                //append the domain to the username in order to identify this users as a network
//                string domain = ActiveDirectoryManager.GetDomain();
//                identity = new GenericIdentity(domain + "/" + ActiveDirectoryManager.GetUsername(((NamePasswordCredential)credentials).Name), config.Name);
//                Logger.Write("Identity name " + identity.Name + "successfully authenticated.", "Info", 1, -1, Severity.Information);
//                return true;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="credentials"></param>
//        /// <returns></returns>
//        protected static bool Authenticate(NamePasswordCredential credentials)
//        {
//            string namingContext = ApplicationManager.AppSettings.ActiveDirectoryNamingContext;
//            string name = credentials.Name;
//            string password = credentials.Password;

//            //validate inputs
//            if (namingContext == null || namingContext == string.Empty) { return false; }
//            if (name == null || name == string.Empty) { return false; }

//            string path = "LDAP://" + namingContext;
//            DirectoryEntry entry = new DirectoryEntry(path, ActiveDirectoryManager.GetQualifiedUsername(name), password,
//                ActiveDirectoryManager.SecureAuthenticationFlags | ActiveDirectoryManager.FastAuthenticationFlags);

//            try
//            {
//                // Bind to the native AdsObject to force authentication.
//                Object obj = entry.NativeObject;
//                DirectorySearcher search = new DirectorySearcher(entry);
//                search.Filter = string.Format("(SAMAccountName={0})", ActiveDirectoryManager.GetUsername(name));

//                search.PropertiesToLoad.Add("cn");
//                //search.PropertiesToLoad.Add("memberof");
//                SearchResult result = search.FindOne();
//                if (result == null)
//                {
//                    return false;
//                }
//                else
//                {

//                    //                    foreach (string value in result.Properties["memberof"])
//                    //                    {
//                    //                        string s = value;
//                    //                    }
//                    return true;
//                }
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message.Contains("unknown user name or bad password"))
//                {
//                    //TODO: better error handling?
//                    throw;
//                }
//                else
//                {
//                    throw new Exception("Error authenticating user. " + ex.Message);
//                }
//            }
//        }
//        #endregion

//        #region IConfigurationProvider Members

//        /// <summary>
//        /// Name of the provider. Required for IConfigurationProvider interface
//        /// </summary>
//        public string ConfigurationName
//        {
//            get { return configurationName; }
//            set { configurationName = value; }
//        }

//        /// <summary>
//        /// Initialize the authentication provider with the supplied configuration object.
//        /// </summary>
//        /// <param name="baseConfig"></param>
//        public void Initialize(ConfigurationBase baseConfig)
//        {
//            config = (LDAPAuthenticationProviderData)baseConfig;
//        }

//        #endregion
//    }
//}