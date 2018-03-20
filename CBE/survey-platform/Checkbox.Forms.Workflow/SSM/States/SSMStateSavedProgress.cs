using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Saved response state. The response can be resumed.
    /// </summary>
    [Serializable]
    class SSMStateSavedProgress : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.ResumeSavedResponse)
            {
                ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
                if (((SessionStateMachine)machine).ResponseStateMachine != null)
                {
                    ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSM.RSMAction.Resume, additionalData);
                }

                return machine[SSMState.HandleResponse];
            }
            if ((SSMAction)action == SSMAction.EditResponse)
            {
                ((SessionStateMachine)machine).ResponseSessionData.SessionState = ResponseSessionState.TakeSurvey;
                if (((SessionStateMachine)machine).ResponseStateMachine != null)
                {
                    ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSM.RSMAction.Edit, additionalData);
                    //set the one instance of session data
                    ((SessionStateMachine)machine).ResponseSessionData = ((SessionStateMachine)machine).ResponseStateMachine.ResponseSessionData;
                }

                return machine[SSMState.HandleResponse];
            }
            return this;
        }
    }
}
