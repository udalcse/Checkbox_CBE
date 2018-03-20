using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms.Workflow.StateMachine;

namespace Checkbox.Forms.Workflow.StateMachine
{
    [Serializable]
    public class StateMachine<T>
    {
        protected static readonly Dictionary<T, State<T>> _availableStates;

        protected State<T> _currentState;

        internal State<T> this[T state]
        {
            get
            {
                return _availableStates[state];
            }
        }

        static StateMachine()
        {
            Array a = typeof(T).GetEnumValues();

            _availableStates = new Dictionary<T, State<T>>();

            Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            for (int i = 0; i < a.Length; i++)
            {
                string stateTypeName = typeof(T).Name + a.GetValue(i).ToString();
                Type stateClass = (from t in types where t.Name.Equals(stateTypeName) select t).FirstOrDefault();                    
                if (stateClass != null)
                    _availableStates[(T)a.GetValue(i)] = Activator.CreateInstance(stateClass) as State<T>;
            }
        }

        public virtual void Initialize(T state)
        {
            _currentState = this[state];
        }

        public virtual bool Valid
        {
            get
            {
                return _availableStates.Count > 0 && _currentState != null;
            }
        }

        public void PerformAction(Enum action, object additionalData)
        {
            if (Valid)
            {
#if DEBUG
                System.Diagnostics.Trace.Write((_currentState == null ? "NULL" : _currentState.GetType().Name) + " -[" + action.ToString() + "]-> ");
#endif
                _currentState = _currentState.PerformAction(this, action, additionalData);
#if DEBUG
                System.Diagnostics.Trace.WriteLine(_currentState.GetType().Name);
#endif
            }
        }

        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public virtual void CleanupBeforeCaching()
        {
        }

        /// <summary>
        /// Deletes unnecessary internal objects
        /// </summary>
        public virtual void RestoreAfterCaching(CacheContext cacheContext)
        {
        }
    }
}
