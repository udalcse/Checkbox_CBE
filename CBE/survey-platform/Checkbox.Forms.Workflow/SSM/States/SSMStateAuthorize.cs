using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;


namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Authorizes the respondent
    /// </summary>
    [Serializable]
    class SSMStateAuthorize : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.LogUserIn)
            {
                ((SessionStateMachine)machine).ResponseSessionData.AuthenticatedRespondentUid = (string)additionalData;
                return ((SessionStateMachine)machine).CheckAuthorization();
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
