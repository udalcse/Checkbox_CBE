using System;
using System.Collections.Generic;
using System.Text;

namespace Checkbox.Forms.Piping.Tokens
{
    /// <summary>
    /// Item token
    /// </summary>
    [Serializable()]
    public class ItemToken : Token
    {
        private Int32 _itemID;

        /// <summary>
        /// Item token class
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="token"></param>
        public ItemToken(string token, Int32 itemID) : base(token, TokenType.Answer)
        {
            _itemID = itemID;
        }

        /// <summary>
        /// Get/set the id of the item this token represents
        /// </summary>
        public Int32 ItemID
        {
            get { return _itemID; }
            set { _itemID = value; }
        }
    }
}
