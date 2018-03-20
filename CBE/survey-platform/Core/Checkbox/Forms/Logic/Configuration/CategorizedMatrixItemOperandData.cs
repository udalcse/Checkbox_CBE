using System;
using System.Data;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class CategorizedMatrixItemOperandData : ItemOperandData
    {
        /// <summary>
        /// 
        /// </summary>
        public int? ColumnNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CategorizedMatrixItemOperandData()
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        public CategorizedMatrixItemOperandData(int? itemID)
            :base(itemID)
        {            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operandId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int operandId, RulesObjectSet rulesData)
        {
            base.Load(operandId, rulesData);

            var operand = rulesData.GetOperand(operandId);
           
            OperandId = operandId;
            ColumnNumber = operand.ColumnNumber;
            Category = operand.Category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            //Some data is missing, do nothing
            if(!OperandId.HasValue || !ItemId.HasValue || !ColumnNumber.HasValue || string.IsNullOrEmpty(Category))
            {
                return;
            }

            //Base method inserted no data, something is wrong
            var operand = ruleDataSet.GetOperand(OperandId.Value);

            if (operand == null)
            {
                return;
            }

            operand.ColumnNumber = ColumnNumber;
            operand.Category = Category;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
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

            return Utilities.StripHtml(itemData.PositionText + "." + ColumnNumber + " " + itemData.GetText(false, languageCode), 64);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().Name);
            writer.WriteAttributeString("Item", ItemId.HasValue ? ItemId.Value.ToString() : "0");
            writer.WriteAttributeString("ColumnNumber", ColumnNumber.HasValue ? ColumnNumber.Value.ToString() : "0");
            writer.WriteElementString("Category", Category);
            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        public override void Load(XmlNode xmlNode)
        {
            ItemId = XmlUtility.GetAttributeInt(xmlNode, "Item");
            ColumnNumber = XmlUtility.GetAttributeInt(xmlNode, "ColumnNumber");
            Category = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Category"));
        }
    }
}
