using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Initial state. User can create a session and start checking flow. 
    /// First of all the respondent is required.
    /// </summary>
    [Serializable]
    class SSMStateInitial : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.CreateSession)
            {
                ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.RespondentRequired;
                return machine[SSMState.RespondentRequired];
            }
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
