using System;
using Checkbox.Forms.Workflow.RSM;
using Checkbox.Forms.Workflow.StateMachine;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Workflow.SSM.States
{
    /// <summary>
    /// State of the response when the user posts pages one by one.
    /// 
    /// There are transitions to CompleteResponse and SavedProgress states
    /// </summary>
    [Serializable]
    class SSMStateHandleResponse : State<SSMState>
    {
        public override State<SSMState> PerformAction(StateMachine<SSMState> machine, Enum action, object additionalData)
        {
            if ((SSMAction)action == SSMAction.EditResponse)
            {
                //user can continue editing existing response and take the machine from the cache
                return machine[SSMState.HandleResponse];
            }
            else if ((SSMAction)action == SSMAction.PostPage)
            {
                SurveyPageAction pageAction = (SurveyPageAction)additionalData;
                switch (pageAction)
                {
                    case SurveyPageAction.MoveForward:
                        ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSMAction.Forward, null);
                        return ((SessionStateMachine)machine).ResponseSessionData.SessionState == ResponseSessionState.Completed ?
                            machine[SSMState.CompleteResponse] :
                            machine[SSMState.HandleResponse];
                    case SurveyPageAction.MoveBackward:
                        ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSMAction.Backward, null);
                        return machine[SSMState.HandleResponse];
                    case SurveyPageAction.Save:
                        ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSMAction.Save, null);
                        return machine[SSMState.SavedProgress];
                    case SurveyPageAction.UpdateConditions:
                        ((SessionStateMachine)machine).ResponseStateMachine.PerformAction(RSMAction.UpdateConditions, null);
                        return machine[SSMState.HandleResponse];
                }
            }
            else if ((SSMAction)action == SSMAction.SetResponseLanguage)
            {
                //set the given language
                if (((SessionStateMachine)machine).ResponseSessionData != null)
                {
                ((SessionStateMachine)machine).ResponseSessionData.SelectedLanguage = additionalData as string;
                }
                if (((SessionStateMachine)machine).ResponseStateMachine != null)
                {
                    ((SessionStateMachine)machine).ResponseStateMachine.SetLanguage(additionalData as string);
                }
                return machine[SSMState.HandleResponse];
            }

            throw new Exception(string.Format("Can not perform action {0} being in the state {1}.", action, GetType().Name));
        }
    }
}
