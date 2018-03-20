using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms.Workflow.StateMachine
{
    /// <summary>
    /// Base class for all states of any state machine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class State<T>
    {
        /// <summary>
        /// Makes all necessary checks and return the new state where state machine should be switched to.
        /// </summary>
        /// <param name="machine">State machine</param>
        /// <param name="action">Action</param>
        /// <param name="additionalData">Data passes along with the action</param>
        /// <returns></returns>
        public abstract State<T> PerformAction(StateMachine<T> machine, Enum action, object additionalData);
    }
}
