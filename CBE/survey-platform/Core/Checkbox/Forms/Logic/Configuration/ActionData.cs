using System;
using System.Collections.Generic;
using System.Xml;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public abstract class ActionData
    {
        /// <summary>
        /// Get action id
        /// </summary>
        public int ActionId { get; protected set; }

        ///<summary>
        /// Get receiver object id
        ///</summary>
        public int ReceiverId { get; protected set; }

        /// <summary>
        /// Get  receiver type
        /// </summary>
        public ActionReceiverType ReceiverType { get; protected set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ActionData()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="receiverType"></param>
        protected ActionData(int receiverId, ActionReceiverType receiverType)
        {
            ReceiverId = receiverId;
            ReceiverType = receiverType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionNode"></param>
        public abstract void Load(XmlNode actionNode);

        /// <summary>
        /// Load action data from config
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="rulesData"></param>
        public abstract void Load(int actionId, RulesObjectSet rulesData);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public abstract void WriteXml(XmlWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rds"></param>
        /// <param name="itemIdMap"></param>
        /// <param name="optionIdMap"></param>
        public abstract void UpdatItemAndOptionIdMappings(RuleDataService rds, Dictionary<int, int> itemIdMap, Dictionary<int, int> optionIdMap);
    }
}