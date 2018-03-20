using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// In this state machine expects to recive the password
    /// </summary>
    [Serializable]
    class SSMStatePasswordRequired : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.SetPassword)
            {
                //password was given -- save it
                ((SessionStateMachine)machine).ResponseSessionData.EnteredPassword = (string)additionalData;

                //check the password again
                return ((SessionStateMachine)machine).CheckPassword(); 
            }
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
