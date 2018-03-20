//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using Checkbox.Users;
using Prezza.Framework.Common;
using Prezza.Framework.Logging;
using Prezza.Framework.Security;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Security.Providers
{
    /// <summary>
    /// Summary description for DbAuthorizationProvider.
    /// </summary>
    public class DbAuthorizationProvider : IAuthorizationProvider
    {
        /// <summary>
        /// Provider configuration
        /// </summary>
        private DbAuthorizationProviderData _config;

        #region IAuthorizationProvider Members

        /// <summary>
        /// Get the type of the last error
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizationErrorType()
        {
            return string.Empty;
        }

        /// <summary>
        /// Authorize the specified principal to access a given resource against the specified context.
        /// </summary>
        /// <param name="principal">the <see cref="Prezza.Framework.Security.Principal.ExtendedPrincipal"/> to authorize.</param>
        /// <param name="resource">the <see cref="IAccessControllable"/> resource to authorize against</param>
        /// <param name="context">Context to authorize the principal against.</param>
        /// <returns>True if authorization is given, false otherwise.</returns>
        public bool Authorize(ExtendedPrincipal principal, IAccessControllable resource, string context)
        {
            try
            {
                ArgumentValidation.CheckForNullReference(resource, "resource");
                ArgumentValidation.CheckForEmptyString(context, "context");

                if (_config == null)
                    throw new Exception("This authorization provider instance has no configuration information.");

                bool groupAuthorized = false;
                int groupAclCount = 0;
                Policy policy = null;

                if (principal != null)
                {
                    if (principal.IsInRole("System Administrator"))
                    {
                        return true;
                    }

                    if (principal.IsInRole("Respondent") && context == "Form.Edit")
                    {
                        return true;
                    }

                    // First authorize the principal to perform the action at all
                    if (!Authorize(principal, context))
                    {
                        //Logger.Write("Authorization failed for " + principal.Identity.Name + " to " + context + " failed.", "Warning", 1, -1, Severity.Warning);
                        return false;
                    }

                    if (resource.ACL != null)
                    {
                        //First, check the principal policy.  If the principal is in the ACL, those permissions override the group
                        // or role permissions

                        if (resource.ACL.IsInList(principal))
                        {
                            policy = resource.ACL.GetPolicy(principal);

                            return CheckPolicy(policy, context, principal);
                        }

                        //Second, check the policies of groups the principal is a member of
                        //The user only need be a member of a group with permission on the entry
                        List<Group> groupMemberships = GroupManager.GetGroupMemberships(principal.Identity.Name);

                        foreach (Group group in groupMemberships)
                        {
                            if (group != null)
                            {
                                if (resource.ACL.IsInList(group))
                                {
                                    //Increment the count
                                    groupAclCount++;

                                    policy = resource.ACL.GetPolicy(group);

                                    if (CheckPolicy(policy, context, principal))
                                    {
                                        groupAuthorized = true;
                                    }
                                }
                            }
                        }

                        if (groupAclCount > 0)
                        {
                            return groupAuthorized;
                        }

                        //CURRENTLY: Roles do not live on ACLs
                        //Finally, check the ACL entries for this principal's roles
                        // For each role, see if the ACL includes the role and that role has permission
                        /*string[] roles = RoleManager.GetRoles(principal.Identity);
						
                        bool roleAuthorized = false;

                        foreach(string role in roles)
                        {
                            if(resource.ACL.IsInList(RoleManager.GetRole(role)))
                            {
                                policy = resource.ACL.GetPolicy(RoleManager.GetRole(role));
								
                                if(CheckPolicy(policy, context, principal))
                                {
                                    roleAuthorized = true;
                                }
                            }
                        }

                        if(roles.Length > 0)
                        {
                            return roleAuthorized;
                        }*/
                    }
                }


                //Add a special check for the everyone group
                EveryoneGroup everyone = GroupManager.GetEveryoneGroup();
                if (resource.ACL.IsInList(everyone))
                {
                    //Increment the count
                    groupAclCount++;

                    policy = resource.ACL.GetPolicy(everyone);

                    if (CheckPolicy(policy, context, principal))
                    {
                        groupAuthorized = true;
                    }
                }

                //If the user is a member of groups, each of which has no access, then the user does not have 
                // access.
                if (groupAclCount > 0)
                {
                    return groupAuthorized;
                }
                //As a last resort, check the default policy, which should happen when
                //  - The principal is not on the resource ACL
                //  - The principal is not a member of a group that is on the resource ACL
                //	- The principal is not in a role that is on the resource ACL
                return CheckPolicy(resource.DefaultPolicy, context, principal);

            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                    throw;

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="context"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private static bool CheckPolicy(Policy policy, string context, ExtendedPrincipal principal)
        {
            if (policy == null || !policy.HasPermission(context))
            {
                //if (principal != null)
                //{
                //    Logger.Write("Authorization failed for " + principal.Identity.Name + " to " + context + ".", "Warning", 1, -1, Severity.Warning);
                //}
                //else
                //{
                //    Logger.Write("Authorization failed for null principal to " + context + ".", "Warning", 1, -1, Severity.Warning);
                //}

                return false;
            }

            //if (principal != null)
            //{
            //    Logger.Write("Authorization succeeded for " + principal.Identity.Name + " to " + context + ".", "Info", 1, -1, Severity.Information);
            //}
            //else
            //{
            //    Logger.Write("Authorization succeeded for null principal to " + context + ".", "Warning", 1, -1, Severity.Warning);
            //}

            return true;
        }

        /// <summary>
        /// Authorize the specified principal against the specified
        /// context.
        /// </summary>
        /// <param name="principal"><see cref="System.Security.Principal.IPrincipal" /> to authorize.</param>
        /// <param name="context">Context to authorize the principal against.</param>
        /// <returns>True if authorization is given, false otherwise.</returns>
        public bool Authorize(ExtendedPrincipal principal, string context)
        {
            ArgumentValidation.CheckForEmptyString(context, "context");

            if (principal == null)
            {
                return false;
            }

            string[] roles = principal.GetRoles();
            for (int i = 0; i < roles.Length; i++)
            {
                if (roles[i] == "System Administrator")
                {
                    return true;
                }

                if (context.Contains(","))
                {
                    string[] contexts = context.Split(',');
                    foreach (var s in contexts)
                    {
                        if (RoleManager.GetRole(roles[i]).HasPermission(s))
                        {
                            return true;
                        }
                    }
                }

                if (RoleManager.GetRole(roles[i]).HasPermission(context))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region IConfigurationProvider Members

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Initialize the authorization provider with the supplied configuration object.
        /// </summary>
        /// <param name="config">the configuration object</param>
        public void Initialize(ConfigurationBase config)
        {
            _config = (DbAuthorizationProviderData)config;
        }

        #endregion
    }
}
