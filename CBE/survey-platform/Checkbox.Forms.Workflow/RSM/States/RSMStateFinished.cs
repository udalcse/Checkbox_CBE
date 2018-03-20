using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Workflow.RSM.States
{
    /// <summary>
    /// State of the finished response.
    /// 
    /// Allows transition to the InProgress state on Edit action.
    /// </summary>
    [Serializable]
    public class RSMStateFinished : State<RSMState>
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
            Response response = ((ResponseStateMachine)machine).Response;
            response.ResponseCompleted += new Response.ResponseCompletedHandler(((ResponseStateMachine)machine).OnResponseCompleted);
            response.ResponseSaved += new Response.ResponseSavedHandler(((ResponseStateMachine)machine).OnResponseSaved);
            if ((RSMAction)action == RSMAction.Edit)
            {
                ((ResponseStateMachine)machine).ResponseSessionData = (ResponseSessionData)additionalData;
                ((ResponseStateMachine)machine).Response.MoveToStart();
                ((ResponseStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
                return machine[RSMState.InProgress];
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
