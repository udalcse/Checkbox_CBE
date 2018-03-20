using System;
using System.ComponentModel;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Logic rule associated with survey items or pages.
    /// </summary>
    [Serializable]
    public sealed class Rule
    {
        [NonSerialized]
        private EventHandlerList _eventHandlers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trigger"></param>
        /// <param name="actions"></param>
        public Rule(Expression expression, RuleEventTrigger trigger, params Action[] actions)
        {
            Expression = expression;
            Actions = actions;
            Trigger = trigger;
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Action[] Actions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RuleEventTrigger Trigger { get; private set; }

        private EventHandlerList Events
        {
            get 
            { 
                if(_eventHandlers == null)
                    _eventHandlers = new EventHandlerList();
                return _eventHandlers; 
            }
        }

        /// <summary>
        /// Evaluates the Expression and executes any Actions
        /// </summary>
        public bool Run(Response response)
        {
            OnBeginRun();
            bool result = Expression.Evaluate(response);
            foreach (Action action in Actions)
            {
                action.Execute(result, response);
            }
            OnEndRun();

            return result;
        }

        private static readonly object EventBeginRun = new object();
        private static readonly object EventEndRun = new object();
        private static readonly object EventRuleChanged = new object();
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler RuleChanged
        {
            add { Events.AddHandler(EventRuleChanged, value); }
            remove { Events.RemoveHandler(EventRuleChanged, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler BeginRun
        {
            add { Events.AddHandler(EventBeginRun, value); }
            remove { Events.RemoveHandler(EventBeginRun, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler EndRun
        {
            add { Events.AddHandler(EventEndRun, value); }
            remove { Events.RemoveHandler(EventEndRun, value); }
        }

        private void OnBeginRun()
        {
            EventHandler handler = (EventHandler)Events[EventBeginRun];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnEndRun()
        {
            EventHandler handler = (EventHandler)Events[EventEndRun];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnRuleChanged()
        {
            EventHandler handler = (EventHandler)Events[EventRuleChanged];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Specify when rule should run.
    /// </summary>
    public enum RuleEventTrigger
    {
        /// <summary>
        /// Rule runs on page load (i.e. is a page/item condition)
        /// </summary>
        Load = 0,

        /// <summary>
        /// Rule runs on page unload (i.e. is a page branch)
        /// </summary>
        UnLoad = 1
    }
}
