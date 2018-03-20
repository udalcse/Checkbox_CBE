using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{

    /// <summary>
    /// Response completed.
    /// 
    /// One transition to the HandleResponse state is allowed on EditResponse action
    /// </summary>
    [Serializable]
    class SSMStateCompleteResponse : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.EditResponse)
            {
                ((SessionStateMachine)machine).ResponseSessionData = (ResponseSessionData)additionalData;
                ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSM.RSMAction.Edit, additionalData);

                ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
                
                return machine[SSMState.HandleResponse];
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
