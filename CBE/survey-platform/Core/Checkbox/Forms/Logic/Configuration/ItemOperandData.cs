using System;
using System.Data;
using System.Threading;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class ItemOperandData : OperandData
    {
        ///<summary>
        ///</summary>
        public ItemOperandData()
            : this(null)
        {

        }

        ///<summary>
        ///</summary>
        ///<param name="itemID"></param>
        public ItemOperandData(int? itemID)
        {
            ItemId = itemID;
        }

		/// <summary>
		/// 
		/// </summary>
		public int? ItemId { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public override string DataTableName { get { return "ItemOperand"; } }

        /// <summary>
        /// Get a text type name for this operand
        /// </summary>
        public override string OperandTypeName { get { return "ItemOperand"; } }

        /// <summary>
        /// Get operand data
        /// </summary>
        /// <param name="operandId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int operandId, RulesObjectSet rulesData)
        {
            var operand = rulesData.GetOperand(operandId);

            if (operand == null)
            {
                throw new Exception("Unable to find data for item operand with id: " + operandId);
            }

            OperandId = operandId;
            ItemId = operand.ItemId;
        }

        /// <summary>
        /// Update rule data
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            //If no item id or operand id, something is wrong
            if(!ItemId.HasValue|| !OperandId.HasValue)
            {
                return;
            }

            var operand = ruleDataSet.GetOperand(OperandId.Value);

            if (operand == null)
            {
                throw new Exception("blah");
              //  ruleDataSet.InsertItemOperand(OperandId.Value, ItemId.Value, null);
            }
            else
            {
                operand.ItemId = ItemId.Value;
            }
        }

        ///// <summary>
        ///// Get the operand as a string
        ///// </summary>
        ///// <returns></returns>
        public override string ToString(string languageCode)
        {
            if (!ItemId.HasValue)
            {
                return "[No Item ID]";
            }

            var itemData = SurveyMetaDataProxy.GetItemData(ItemId.Value, true);

            if (itemData == null)
            {
                return "Unable to get data for item: " + ItemId.Value;
            }

            return Utilities.StripHtml(itemData.PositionText + "  " + itemData.GetText(false, languageCode), 64);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("ItemId", ItemId.HasValue ? ItemId.Value.ToString() : "0");
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
		    ItemId = XmlUtility.GetAttributeInt(xmlNode, "ItemId");
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        public override bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            //Unable to map to new item id do nothing
            if (!ItemId.HasValue
                || !itemIdMap.ContainsKey(ItemId.Value))
            {
                return false;
            }

            //Otherwise, update item
            ItemId = itemIdMap[ItemId.Value];

            return true;
        }


        /// <summary>
        /// Validate the expression
        /// </summary>
        public override bool Validate()
        {
            //TODO: Validate
            return true;

            //ItemData item = Context.GetItem(ItemId.Value);
            //if (item == null)
            //{
            //    OnValidationFailed();
            //}
        }

    }
}
