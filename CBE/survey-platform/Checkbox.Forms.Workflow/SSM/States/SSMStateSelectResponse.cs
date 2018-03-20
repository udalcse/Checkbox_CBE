using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Analytics;


namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Select response state. Terminal state, because for the resuming responses and editing existing the new machines will be created
    /// </summary>
    [Serializable]
    class SSMStateSelectResponse : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
