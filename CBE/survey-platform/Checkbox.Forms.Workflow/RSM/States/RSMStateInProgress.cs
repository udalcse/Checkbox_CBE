using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Management;
using Checkbox.Invitations;

namespace Checkbox.Forms.Workflow.RSM.States
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RSMStateInProgress : State<RSMState>
    {
        public override State<RSMState> PerformAction(StateMachine<RSMState> machine, Enum action, object additionalData)
        {
            Response response = ((ResponseStateMachine)machine).Response;
            response.ResponseCompleted += new Response.ResponseCompletedHandler(((ResponseStateMachine)machine).OnResponseCompleted);
            response.ResponseSaved += new Response.ResponseSavedHandler(((ResponseStateMachine)machine).OnResponseSaved);

            switch ((RSMAction)action)
            {
                case RSMAction.Save:
                {
                    if (ApplicationManager.AppSettings.SessionMode == AppSettings.SessionType.Cookieless)
                        ResponseSessionDataManager.SaveResponseSessionData(((ResponseStateMachine)machine).ResponseSessionData);

                    ((ResponseStateMachine)machine).Response.SaveCurrentState();
                    ((ResponseStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.SavedProgress;
                    return machine[RSMState.Saved];
                }
                case RSMAction.Forward:
                {
                    ((ResponseStateMachine)machine).Response.MoveNext();

                    bool complete = ((ResponseStateMachine)machine).Response.CurrentPage.PageType == TemplatePageType.Completion;
                    //If on completion page, set state to completed
                    ((ResponseStateMachine)machine).ResponseSessionData.SessionState = complete ? ResponseSessionState.Completed : ResponseSessionState.TakeSurvey;

                    //If completed, mark as responded
                    if (((ResponseStateMachine)machine).Response.CurrentPage.PageType == TemplatePageType.Completion
                        && ((ResponseStateMachine)machine).ResponseSessionData.InvitationRecipientId.HasValue)
                    {
                        Invitation.MarkRecipientsResponded(new long[] { ((ResponseStateMachine)machine).ResponseSessionData.InvitationRecipientId.Value });

                        if (!ApplicationManager.AppSettings.IsPrepMode)
                        {
                            Invitation.RecordResponse(
                                ((ResponseStateMachine) machine).ResponseSessionData.InvitationRecipientId.Value,
                                ((ResponseStateMachine) machine).Response.ID.Value);
                        }
                    }
                    return machine[complete ? RSMState.Finished : RSMState.InProgress];
                }
                case RSMAction.Backward:
                {
                    if (ApplicationManager.AppSettings.SavePartialResponsesOnBackNavigation)
                    {
                        ((ResponseStateMachine)machine).Response.SaveCurrentState();
                    }

                    ((ResponseStateMachine)machine).Response.MovePrevious();
                    return machine[RSMState.InProgress];
                }
                case RSMAction.UpdateConditions:
                {
                    ((ResponseStateMachine)machine).Response.UpdateCurrentPageConditions();
                    return machine[RSMState.InProgress];
                }
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
