using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Terminal state for the machine if the user isn't allowed to do something
    /// </summary>
    [Serializable]
    class SSMStateStopped : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
