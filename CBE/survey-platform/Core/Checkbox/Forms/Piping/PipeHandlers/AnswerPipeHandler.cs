using Checkbox.Forms.Items;
using Checkbox.Forms.Piping.Tokens;

namespace Checkbox.Forms.Piping.PipeHandlers
{
    /// <summary>
    /// Handler for Answer pipes-- variable info that is piped from the recorded answer to a given Item.
    /// </summary>
    public class AnswerPipeHandler : PipeHandler
    {
        /// <summary>
        /// Get the value of a pipe
        /// </summary>
        /// <param name="token"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string GetTokenValue(Token token, object context)
        {
            if (token is ItemToken && context is Response)
            {
                int itemID = ((ItemToken)token).ItemID;
                Item theItem = ((Response)context).GetItem(itemID);

                if (theItem != null && theItem is IAnswerable && !theItem.Excluded)
                {
                    return ((IAnswerable)theItem).GetAnswer();
                }
            }

            return string.Empty;
        }
    }
}