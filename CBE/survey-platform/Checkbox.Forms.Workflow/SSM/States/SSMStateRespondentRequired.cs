using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Invitations;
using Checkbox.Globalization.Text;
using Checkbox.Users;
using Checkbox.Security.Principal;
using Checkbox.Management;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// State for waiting for respondent data
    /// </summary>
    [Serializable]
    class SSMStateRespondentRequired : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.SetRespondent)
            {
                ResponseSessionData recievedData = (ResponseSessionData)additionalData;
                //Disallow anonymouse users to see the survey in prep mode
                if(string.IsNullOrEmpty(recievedData.AuthenticatedRespondentUid) && ApplicationManager.AppSettings.IsPrepMode)
                {
                    ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.InvitationRequired;
                    return ((SessionStateMachine)machine)[SSMState.Authorize];
                }
                //Figure out if we need to look up the respondent
                if (recievedData.InvitationRecipientGuid.HasValue)
                {
                    var invitation =
                        InvitationManager.GetInvitationForRecipient(recievedData.InvitationRecipientGuid.Value);


                    if (invitation != null)
                    {
                        var recipientId =
                            InvitationManager.GetRecipientId(
                                recievedData.InvitationRecipientGuid.Value);

                        if (recipientId.HasValue)
                        {
                            var recipient = invitation.GetRecipients(new[] { recipientId.Value }).FirstOrDefault();

                            if (recipient != null)
                            {
                                ((SessionStateMachine)machine).ResponseSessionData.InvitationRecipientId = recipient.ID;
                                ((SessionStateMachine)machine).ResponseSessionData.Invitee = recipient.EmailToAddress;

                                if (invitation.Template.LoginOption == LoginOption.Auto || ((SessionStateMachine)machine).SurveyIs(SecurityType.Public))
                                {
                                    string authenticatedRespondentUid = recipient.UniqueIdentifier;
                                    
                                    //check that this user exists
                                    CheckboxPrincipal user = UserManager.GetUserPrincipal(authenticatedRespondentUid);
                                    if (user == null || user.UserGuid.Equals(Guid.Empty))
                                    {
                                        //search for user by email then
                                        user = UserManager.GetUserWithEmail(recipient.EmailToAddress);
                                        if (user != null)
                                            authenticatedRespondentUid = user.Identity.Name;
                                        else
                                        {
                                            //no user with this email
                                            if (((SessionStateMachine)machine).SurveyIs(SecurityType.AllRegisteredUsers))
                                            {
                                                ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.LoginRequired;
                                                return ((SessionStateMachine)machine)[SSMState.Authorize];
                                            }
                                        }
                                    }

                                    ((SessionStateMachine)machine).ResponseSessionData.AuthenticatedRespondentUid = string.IsNullOrEmpty(authenticatedRespondentUid) ? recipient.UniqueIdentifier : authenticatedRespondentUid;
                                }
                                else
                                {
                                    if (((SessionStateMachine)machine).SurveyIs(SecurityType.PasswordProtected))
                                    {                                        
                                        return ((SessionStateMachine)machine).CheckPassword();
                                    }
                                    ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.LoginRequired;
                                    return ((SessionStateMachine)machine)[SSMState.Authorize];
                                }
                            }
                        }
                    }
                }
                else if (recievedData.DirectInvitationRecipientGuid.HasValue)
                {
                     var recipientName = InvitationManager.GetUserInvitationForRecipient(recievedData.DirectInvitationRecipientGuid.Value);
                     ((SessionStateMachine)machine).ResponseSessionData.AuthenticatedRespondentUid = recipientName;
                }
                else
                {
                    if (!string.IsNullOrEmpty(recievedData.AuthenticatedRespondentUid))
                    {
                        ((SessionStateMachine)machine).ResponseSessionData.AuthenticatedRespondentUid =
                            recievedData.AuthenticatedRespondentUid;
                    }
                    else if (recievedData.AnonymousRespondentGuid.HasValue)
                    {
                        ((SessionStateMachine)machine).ResponseSessionData.AnonymousRespondentGuid =
                            recievedData.AnonymousRespondentGuid;
                    }
                }

                //check the language
                return ((SessionStateMachine)machine).CheckPassword();
            }
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }


    }
}
