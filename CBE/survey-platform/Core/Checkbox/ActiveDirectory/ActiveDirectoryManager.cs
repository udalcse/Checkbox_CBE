using System;
using System.DirectoryServices;

using Checkbox.Management;

namespace Checkbox.ActiveDirectory
{
    /// <summary>
    /// Management class for interacting with Active Directory.
    /// </summary>
    public class ActiveDirectoryManager
    {
        /// <summary>
        /// Finds the environments Default Naming Context.
        /// If no results are found String.Empty is returned.
        /// </summary>
        /// <returns></returns>
        public string FindDefaultNamingContext()
        {
            try
            {
                DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
                string dnc = (string)rootDSE.Properties["defaultNamingContext"].Value;

                return dnc;
            }
            catch (System.Runtime.InteropServices.COMException comEx)
            {
                if (comEx.Message.StartsWith("The specified domain either does not exist or could not be contacted."))
                {
                    throw new Exception("The specified domain either does not exist or could not be contacted. It is possible this error has been caused by account permissions. Checkbox is currently running under the " + Environment.UserName + ".");
                }

                throw;
            }
        }

        /// <summary>
        /// Attempts to connect to the specified Active Directory Naming Context. 
        /// If a connection can be made True is returned.
        /// If a connection cannot be made False is returned.
        /// </summary>
        /// <param name="context">The active directory naming context which is checked</param>
        /// <returns></returns>
        public bool CanConnectToNamingContext(string context)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + context);
                //string dnc = (string)de.Properties["defaultNamingContext"].Value;
                string name = (string)de.Properties["name"].Value;
                if (name == null || string.Empty.Equals(name.Trim()))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ensures that a username is not prefixed with a domain.
        /// </summary>
        /// <param name="username">The unformatted username.</param>
        /// <returns>A username without a domain name.</returns>
        public static string GetUsername(string username)
        {
            char? delimiter = null;

            if (username == null) { return null; }

            if (username.Contains(@"\"))
                delimiter = '\\';
            else if (username.Contains("/"))
                delimiter = '/';

            if (delimiter != null)
            {
                string[] usernameSplit = username.Split(delimiter.Value);
                if (usernameSplit.Length > 1)
                {
                    return usernameSplit[1];
                }
            }

            return username;
        }

        ///<summary>
        /// Ensures that a username is properly formatted for use with a DirectoryEntry object.
        ///</summary>
        ///<param name="username">The unformatted username.</param>
        ///<returns>A username in the following format domain\username .</returns>
        public static string GetQualifiedUsername(string username)
        {
            if (username == null) { return null; }

            username = username.Replace("/", @"\");

            if (username.Contains(@"\"))
            {
                return username;
            }
            else
            {
                return string.Format(@"{0}\{1}", GetDomain(), username);
            }
        }


        /// <summary>
        /// Determines the domain name from the Active Directory naming context.
        /// If the naming context is not specified or is not properly formed string.Empty is returned.
        /// </summary>
        /// <returns></returns>
        public static string GetDomain()
        {
            //            //it is possible that the following can be used as a substitute. It is unclear if this 
            //            //will return the desired results or if it is specific to network architecture.
            //            DirectoryEntry de = new DirectoryEntry("LDAP://" + context);
            //            string name = (string)de.Properties["name"].Value;

            string namingContext = ApplicationManager.AppSettings.ActiveDirectoryNamingContext;
            string domain = string.Empty;

            if (namingContext == null || string.Empty.Equals(namingContext.Trim()))
            {
                return string.Empty;
            }

            string[] namingContextElements = namingContext.Split(',');

            //example naming context: DC=prezzatech,DC=com
            foreach (string element in namingContextElements)
            {
                if (element.Trim().StartsWith("DC="))
                {
                    domain = element.Replace("DC=", "");
                    break;
                }
            }

            return domain;
        }

        /// <summary>
        /// Returns the set of AuthenticationTypes recommended for a secure connection.
        /// </summary>
        public static AuthenticationTypes SecureAuthenticationFlags
        {
            get
            {
                //AuthenticationTypes.Secure - This option indicates to use the Windows Security Support Provider Interface (SSPI) authentication system when binding to the directory.
                //AuthenticationTypes.Sealing - This flag says to use the encryption capabilities of SSPI to encrypt traffic after the security context is established.
                //AuthenticationTypes.Signing - Signing uses the signing capabilities of SSPI to sign network traffic and verify whether someone has tampered with it.
                return AuthenticationTypes.Secure | AuthenticationTypes.Sealing | AuthenticationTypes.Signing;
            }
        }

        /// <summary>
        /// Returns the set of AuthenticationTypes optimized for lightweight connections.
        /// These flags requires that an explicit AdsPath is set and limit the number 
        /// of ADSI interfaces available.
        /// </summary>
        public static AuthenticationTypes FastAuthenticationFlags
        {
            get
            {
                //AuthenticationTypes.FastBind - Disables this initial search to determine the objectClass attribute. Limits ADSI interfaces available.
                //AuthenticationTypes.ServerBind - A performance optimization which eliminates the extra DNS traffic involved in the dynamic discovery process. 
                //                                 It eliminates Active Directory's automatic failover capabilities and can cause brittleness as a result.
                return AuthenticationTypes.FastBind | AuthenticationTypes.ServerBind;
            }
        }
    }
}