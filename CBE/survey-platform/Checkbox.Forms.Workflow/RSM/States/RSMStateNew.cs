using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.RSM.States
{
    /// <summary>
    /// State of the new response
    /// 
    /// Transistion to the InProgress state is allowed.
    /// </summary>
    [Serializable]
    public class RSMStateNew : State<RSMState>
    {
        /// <summary>
        /// Makes all necessary checks and return the new state where state machine should be switched to.
        /// </summary>
        /// <param name="machine">State machine</param>
        /// <param name="action">Action</param>
        /// <param name="additionalData">Data passes along with the action</param>
        /// <returns></returns>
        public override State<RSMState> PerformAction(StateMachine<RSMState> machine, Enum action, object additionalData)
        {
            if ((RSMAction)action == RSMAction.Start)
            {
                ResponseTemplate template =
                    ResponseTemplateManager.GetResponseTemplate(((ResponseStateMachine)machine).ResponseSessionData.ResponseTemplateId.Value);

                Response response = template.CreateResponse(((ResponseStateMachine)machine).ResponseSessionData.SelectedLanguage);
                response.ResponseCompleted += new Response.ResponseCompletedHandler(((ResponseStateMachine)machine).OnResponseCompleted);
                response.ResponseSaved += new Response.ResponseSavedHandler(((ResponseStateMachine)machine).OnResponseSaved);

                response.Initialize(
                    ((ResponseStateMachine)machine).ResponseSessionData.RespondentIpAddress,
                    ((ResponseStateMachine)machine).ResponseSessionData.NetworkUser,
                    ((ResponseStateMachine)machine).ResponseSessionData.SelectedLanguage,
                    ((ResponseStateMachine)machine).ResponseSessionData.IsTest,
                    ((ResponseStateMachine)machine).ResponseSessionData.Invitee,
                    ((ResponseStateMachine)machine).Respondent,
                    ((ResponseStateMachine)machine).ResponseSessionData.SessionGuid);

                ((ResponseStateMachine)machine).ResponseSessionData.ResponseGuid = response.GUID;

                return machine[RSMState.InProgress];
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
