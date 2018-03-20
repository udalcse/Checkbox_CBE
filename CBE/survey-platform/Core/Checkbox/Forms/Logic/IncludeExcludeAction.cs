using System;

using Checkbox.Forms.Items;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class IncludeExcludeAction : Action
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverType"></param>
        /// <param name="recevierId"></param>
        public IncludeExcludeAction(ActionReceiverType receiverType, int recevierId)
            : base(receiverType, recevierId)
        { }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directive"></param>
        /// <param name="response"></param>
        public override void Execute(bool directive, Response response)
        {
            if (ReceiverType == ActionReceiverType.Item)
            {
                Item itemReceiver = response.GetItem(ReceiverId);

                if (itemReceiver != null)
                {
                    itemReceiver.Excluded = !directive;
                }
            }

            if (ReceiverType == ActionReceiverType.Page)
            {
                ResponsePage pageReceiver = response.GetPage(ReceiverId);

                if (pageReceiver != null)
                {
                    pageReceiver.Excluded = !directive;
                }
            }
        }
    }
}