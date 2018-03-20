using System;
using System.Collections.Generic;
using System.Xml;
using Prezza.Framework.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class IncludeExcludeActionData : ActionData
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public IncludeExcludeActionData()
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="receiverId"></param>
        ///<param name="receiverType"></param>
        public IncludeExcludeActionData(int receiverId, ActionReceiverType receiverType)
            : base(receiverId, receiverType)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Display " + ReceiverType;
        }
        
        /// <summary>
        /// Load item from configuration
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int actionId, RulesObjectSet rulesData)
        {
            ActionId = actionId;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("ReceiverType", ReceiverType.ToString());
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
		    var enumString = XmlUtility.GetAttributeText(xmlNode, "ReceiverType");

            if (string.IsNullOrEmpty(enumString))
            {
                enumString = ActionReceiverType.Item.ToString();
            }

            ReceiverType =
                (ActionReceiverType)
                Enum.Parse(typeof(ActionReceiverType), enumString);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rds"></param>
        /// <param name="itemIdMap"></param>
        /// <param name="optionIdMap"></param>
        public override void UpdatItemAndOptionIdMappings(RuleDataService rds, Dictionary<int, int> itemIdMap, Dictionary<int, int> optionIdMap)
        {
            return;
        }
    }
}
