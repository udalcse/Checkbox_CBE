using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// Machine demands user to initialize the language
    /// </summary>
    [Serializable]
    class SSMStateLanguageRequired : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.SetResponseLanguage)
            {
                //set the given language
                ((SessionStateMachine)machine).ResponseSessionData.SelectedLanguage = additionalData as string;

                //ask for password if needed
                return ((SessionStateMachine)machine).CheckPassword();

            }
            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }

        private bool passwordNeeded(StateMachine<SSMState> machine, ResponseTemplate template)
        {
            return (template.BehaviorSettings.SecurityType == SecurityType.PasswordProtected &&
                   !template.BehaviorSettings.Password.Equals(
                   ((SessionStateMachine)machine).ResponseSessionData.EnteredPassword));
        }
    }
}
