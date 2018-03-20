using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// A command object which performs some action
    /// </summary>
    [Serializable]
    public abstract class Action
    {
        /// <summary>
        /// Receiver type for action.
        /// </summary>
        public enum ActionReceiverType
        {
            /// <summary>
            /// Action operates on an item.
            /// </summary>
            Item,

            /// <summary>
            /// Action operates on a page.
            /// </summary>
            Page
        }

        /// <summary>
        /// Construct action
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="receiverType"></param>
        protected Action(ActionReceiverType receiverType, int receiverId)
        {
            ReceiverType = receiverType;
            ReceiverId = receiverId;
        }

        /// <summary>
        /// Type of action receiver
        /// </summary>
        public ActionReceiverType ReceiverType { get; private set; }

        /// <summary>
        /// Receiver id
        /// </summary>
        public int ReceiverId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        internal int? Identity { get; set; }

        /// <summary>
        /// Encapsulates a specific action to execute
        /// </summary>
        /// <remarks>
        /// Action is a command object.  Inheritors of Action should pay special attention to 
        /// the executing threads operation and may wish to implement Execute in an asynchronous thread.
        /// </remarks>
        /// <param name="directive">the evaluation result for a Rule.</param>
        /// <param name="response">Response</param>
        public abstract void Execute(bool directive, Response response);
    }
}