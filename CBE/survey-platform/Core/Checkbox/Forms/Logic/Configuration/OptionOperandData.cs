using System;
using System.Data;
using System.Linq;
using System.Xml;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using System.Collections.Generic;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Security;
using System.Web;
using Checkbox.Security.Principal;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class OptionOperandData : OperandData
    {
        ///<summary>
        ///ID of item option assoicated with.
        ///</summary>
        public int? ItemID { get; private set; }

        ///<summary>
        ///Answer text value
        ///</summary>
        public string AnswerValue { get; private set; }

        ///<summary>
        ///</summary>
        public OptionOperandData()
            : this(null, null)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="itemID"></param>
        ///<param name="optionID"></param>
        public OptionOperandData(int? itemID, int? optionID)
            : this(itemID, optionID, null)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="itemID"></param>
        ///<param name="optionID"></param>
        ///<param name="answerValue"></param>
        public OptionOperandData(int? itemID, int? optionID, string answerValue)
        {
            OptionID = optionID;
            ItemID = itemID;
            AnswerValue = answerValue;
        }

        /// <summary>
        /// Load option operand
        /// </summary>
        /// <param name="operandId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int operandId, RulesObjectSet rulesData)
        {
            var operand = rulesData.GetOperand(operandId);

            if (operand == null)
            {
                throw new Exception("Unable to find data for Option (value) operand with id: " + operandId);
            }
            
            OperandId = operandId;
            OptionID = operand.OptionId;
            ItemID = operand.ItemId;
            AnswerValue = operand.AnswerValue;
        }

        /// <summary>
        /// Update rule data
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            //Missing data, do nothing
            if(!OperandId.HasValue || !ItemID.HasValue || !OptionID.HasValue)
            {
                return;
            }

            var operand = ruleDataSet.GetOperand(OperandId.Value);
            operand.ItemId = ItemID;
            operand.AnswerValue = AnswerValue;
            operand.OptionId = OptionID;
        }

        /// <summary>
        /// Get a text type name for this operand
        /// </summary>
        public override string OperandTypeName
        {
            get { return "OptionOperand"; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override bool Validate()
        {
            //TODO: Validate
            return true;

            //SelectItemData itemData = (SelectItemData)Context.GetItem(_itemID.Value);
            //if (itemData != null)
            //{
            //    foreach (ListOptionData option in itemData.Options)
            //    {
            //        if (option.OptionID == OptionID.Value)
            //        {
            //            return;
            //        }
            //    }
            //}
            //OnValidationFailed();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString(string languageCode)
        {
            if (!ItemID.HasValue)
            {
                return "[No Item ID]";
            }

            if (!OptionID.HasValue)
            {
                return "[No Option ID]";
            }

            var name = ProfileManager.GetConnectedProfileFieldName(ItemID.Value);

            if (!string.IsNullOrEmpty(name))
            {
                var radioButtonField = ProfileManager.GetRadioButtonField(name, ((CheckboxPrincipal)HttpContext.Current.User).UserGuid);
                var option = radioButtonField.Options.First(s => s.ItemId == OptionID.Value);

                if (option != null)
                {
                    return Utilities.DecodeAndStripHtml(option.Name, 64);
                }
            }

            //TODO: Language Code
            return Utilities.DecodeAndStripHtml(SurveyMetaDataProxy.GetOptionText(ItemID.Value, OptionID.Value, languageCode, false, false), 64);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("Item", ItemID.HasValue ? ItemID.Value.ToString() : "0");
			writer.WriteAttributeString("Option", OptionID.HasValue ? OptionID.Value.ToString() : "0");
			writer.WriteElementString("Answer", AnswerValue);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{

		    ItemID = XmlUtility.GetAttributeInt(xmlNode, "Item");
            OptionID = XmlUtility.GetAttributeInt(xmlNode, "Option");
		    AnswerValue = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("Answer"));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        public override bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            //Unable to map to new item id do nothing
            if(!ItemID.HasValue 
                || !itemIdMap.ContainsKey(ItemID.Value)
                || !OptionID.HasValue
                || !optionPositionMap.ContainsKey(OptionID.Value))
            {
                return false;
            }

            //Otherwise, update item
            int realItemID = itemIdMap[ItemID.Value];

            var optionPosition = optionPositionMap[OptionID.Value];
            var itemData = ItemConfigurationManager.GetConfigurationData(realItemID);

            if(itemData == null || !(itemData is SelectItemData))
            {
                return false;
            }

            //update item ID
            ItemID = realItemID;

            //Find new option at proper position
            var newOption =
                ((SelectItemData) itemData).Options.FirstOrDefault(option => option.Position == optionPosition);
            
            if(newOption != null)
            {
                OptionID = newOption.OptionID;
            }
            else
            {
                //special case: option exists in the matrix column prototype
                if (prototypesMap.ContainsKey(realItemID))
                    realItemID = prototypesMap[realItemID];

                itemData = ItemConfigurationManager.GetConfigurationData(realItemID);
                if (itemData == null || !(itemData is SelectItemData))
                {
                    return false;
                }
                //Find new option at proper position
                newOption =
                    ((SelectItemData)itemData).Options.FirstOrDefault(option => option.Position == optionPosition);

                if (newOption != null)
                {
                    OptionID = newOption.OptionID;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the option operand text
        /// </summary>
        public int? OptionID { get; set; }
    }
}
