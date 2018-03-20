using System;
using System.Collections.Generic;
using System.Security.Principal;
using Checkbox.Users;
using Checkbox.Security;
using Checkbox.Invitations;
using Checkbox.Forms.Security.Principal;
using Checkbox.Management;
using Prezza.Framework.Common;
using Prezza.Framework.Security;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Forms.Security.Providers
{
    /// <summary>
    /// Authorization provider for surveys
    /// </summary>
    public class ResponseTemplateAuthorizationProvider : IAuthorizationProvider
    {
        public const string LoginRequired = "loginrequired";
        public const string SurveyNotActive = "surveynotactive";
        public const string ResponseLimitReached = "responselimitreached";
        public const string NotAuthorized = "notAuthorized";
        public const string EditOnly = "editonly";
        public const string BeforeStartDate = "beforestartdate";
        public const string AfterEndDate = "afterenddate";
        public const string InvitationOnly = "invitationonly";

            /// <summary>
        /// Provider configuration
        /// </summary>
        private string _lastError;

        #region IAuthorizationProvider Members

        /// <summary>
        /// Get the type of the last error
        /// </summary>
        /// <returns></returns>
        public string GetAuthorizationErrorType()
        {
            return _lastError;
        }

        /// <summary>
        /// Get/set the last error
        /// </summary>
        protected string LastError
        {
            get { return _lastError; }
            set { _lastError = value; }
        }

        /// <summary>
        /// Authorize the specified principal to access a given resource against the specified context.
        /// </summary>
        /// <param name="principal">the <see cref="Prezza.Framework.Security.Principal.ExtendedPrincipal"/> to authorize.</param>
        /// <param name="resource">the <see cref="IAccessControllable"/> resource to authorize against</param>
        /// <param name="context">Context to authorize the principal against.</param>
        /// <returns>True if authorization is given, false otherwise.</returns>
        public virtual bool Authorize(ExtendedPrincipal principal, IAccessControllable resource, string context)
        {
            return Authorize(principal, resource, context, null);
        }

        /// <summary>
        /// Authorize the specified principal to access a given resource against the specified context.
        /// </summary>
        /// <param name="principal">the <see cref="Prezza.Framework.Security.Principal.ExtendedPrincipal"/> to authorize.</param>
        /// <param name="resource">the <see cref="IAccessControllable"/> resource to authorize against</param>
        /// <param name="context">Context to authorize the principal against.</param>
        /// <param name="recipientGuid">Guid representing an invitation recipient</param>
        /// <returns>True if authorization is given, false otherwise.</returns>
        public virtual bool Authorize(ExtendedPrincipal principal, IAccessControllable resource, string context, Guid? recipientGuid)
        {
            try
            {
                var appSettings = new AppSettings();

                ArgumentValidation.CheckExpectedType(resource, typeof(ResponseTemplate));
                ArgumentValidation.CheckForNullReference(resource, "resource");
                ArgumentValidation.CheckForEmptyString(context, "context");

                if (principal == null)
                {
                    throw new Exception("Unable to authorize null principal for form.");
                }

                //Super user is always allowed
                if (principal.IsInRole("System Administrator"))
                {
                    return true;
                }

                var rt = (ResponseTemplate)resource;

                bool responseLimitReached = false;

                //If context is "Form.Fill" perform some pre-screening based on other properties
                if (string.Compare(context, "Form.Fill", true) == 0)
                {
                    //Survey editors and admins always have permission to take the survey, so do a quick check
                    if(Authorize(principal, resource, "Form.Edit"))
                    {
                        _lastError = SurveyNotActive;
                        return true;
                    }

                    if (!ApplicationManager.AppSettings.IsPrepMode)
                    {
                        //Otherwise check activation, response counts, etc
                        if (!rt.BehaviorSettings.GetIsActiveOnDate(DateTime.Now, out _lastError))
                        {
                            return false;
                        }
                    }

                    //Check if login is required
                    if ((rt.BehaviorSettings.SecurityType == SecurityType.AccessControlList || rt.BehaviorSettings.SecurityType == SecurityType.AllRegisteredUsers) && principal is AnonymousRespondent)
                    {
                        _lastError = LoginRequired;
                        return false;
                    }
                    
                    //Check if invitation is required
                    if (rt.BehaviorSettings.SecurityType == SecurityType.InvitationOnly)
                    {
                        if (!ValidateInvitationRecipient(recipientGuid, rt.GUID))
                        {
                            _lastError = InvitationOnly;
                            return false;
                        }
                    }


                    if (!appSettings.IsPrepMode)
                    {
                        //Check response counts for survey & principal, this check now includes anonymous respondents
                        if (!ResponseTemplateManager.MoreResponsesAllowed(rt.ID.Value, rt.BehaviorSettings.MaxTotalResponses, rt.BehaviorSettings.MaxResponsesPerUser, principal, rt.BehaviorSettings.AnonymizeResponses))
                        {
                            responseLimitReached = true;
                        }
                    }
                  
                }
                
                //If we reached the response limit and editing is not allowed, return false, otherwise check and see if access is granted to
                //at least edit responses.
                if (!rt.BehaviorSettings.AllowEdit && responseLimitReached)
                {
                    _lastError = ResponseLimitReached;
                    return false;
                }

                // First authorize the principal to perform the action at all
                if (!Authorize(principal, context))
                {
                    //Logger.Write("Authorization failed for " + principal.Identity.Name + " to " + context + " failed.", "Warning", 1, -1, Severity.Warning);
                    _lastError = NotAuthorized;
                    return false;
                }

                if (resource.ACL != null)
                {
                    //First, check the principal policy.  If the principal is in the ACL, those permissions override the group
                    // or role permissions
                    Policy policy;

                    if (resource.ACL.IsInList(principal))
                    {
                        policy = resource.ACL.GetPolicy(principal);

                        if (!CheckPolicy(policy, context, principal))
                        {
                            _lastError = NotAuthorized;
                            return false;
                        }
                        
                        if (responseLimitReached)
                        {
                            _lastError = EditOnly;
                            return false;
                        }
                        
                        return true;
                    }

                    //Second, check the policies of groups the principal is a member of
                    //The user only need be a member of a group with permission on the entry
                    List<Group> groupMemberships = GroupManager.GetGroupMemberships(principal.Identity.Name);

                    bool groupAuthorized = false;
                    int groupAclCount = 0;

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
                    
                    EveryoneGroup everyone = GroupManager.GetEveryoneGroup();

                    //Add a special check for the everyone group
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
                        if (groupAuthorized)
                        {
                            if (responseLimitReached)
                            {
                                _lastError = EditOnly;
                                return false;
                            }
                            
                            return true;
                        }

                        _lastError = NotAuthorized;
                        return false;
                    }
                }

                //As a last resort, check the default policy, which should happen when
                //  - The principal is not on the resource ACL
                //  - The principal is not a member of a group that is on the resource ACL
                //	- The principal is not in a role that is on the resource ACL
                if (CheckPolicy(resource.DefaultPolicy, context, principal))
                {
                    if (responseLimitReached)
                    {
                        _lastError = EditOnly;
                        return false;
                    }
                    
                    return true;
                }

                _lastError = NotAuthorized;
                return false;
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
        /// Validate that an invitation is valid
        /// </summary>
        /// <param name="invitationRecipient"></param>
        /// <param name="rtGuid"></param>
        /// <returns></returns>
        protected virtual bool ValidateInvitationRecipient(Guid? invitationRecipient, Guid rtGuid)
        {
            //Check for valid value
            if (!invitationRecipient.HasValue)
            {
                LastError = "noInvitation";
                return false;
            }
            
            //Ensure invitation is for this survey
            Guid? invitationRtGuid = InvitationManager.GetResponseTemplateGuidForInvitation(invitationRecipient.Value);

            if (invitationRtGuid.HasValue && invitationRtGuid.Value == rtGuid)
            {
                return true;
            }
            
            LastError = "invalidInvitation";
            return false;
        }

        /// <summary>
        /// Check for access based on the policy
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="context"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private static bool CheckPolicy(Role policy, string context, IPrincipal principal)
        {
            if(policy == null || !policy.HasPermission(context))
            {
                //if(principal != null)
                //{
                //    Logger.Write("Authorization failed for " + principal.Identity.Name + " to " + context + ".", "Warning", 1, -1, Severity.Warning);
                //}
                //else
                //{
                //    Logger.Write("Authorization failed for null principal to " + context + ".", "Warning", 1, -1, Severity.Warning);
                //}

                return false;
            }
            
            //if(principal != null)
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
            ArgumentValidation.CheckForNullReference(principal, "principal");

            //string[] roles = RoleManager.GetRoles(principal.Identity);
            string[] roles = principal.GetRoles();

            foreach (string role in roles)
            {
                if (role.Contains("Administrator") && ApplicationManager.AppSettings.IsPrepMode)
                    return true;

                if(role == "System Administrator")
                {
                    return true;
                }

                if(RoleManager.GetRole(role).HasPermission(context))
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
        public virtual void Initialize(ConfigurationBase config)
        {
            //An extending class would store configuration properties here
        }

        #endregion
    }
}
