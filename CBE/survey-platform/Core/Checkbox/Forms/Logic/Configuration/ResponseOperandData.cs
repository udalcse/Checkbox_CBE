using System;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class ResponseOperandData : OperandData
    {
        ///<summary>
        ///</summary>
        public string Key { get; set; }

        /// <summary>
        /// Get a text type name for this operand
        /// </summary>
        public override string OperandTypeName
        {
            get { return "ResponseOperand"; }
        }

        /// <summary>
        /// Load response operand data
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
            Key = operand.ResponseKey;
        }

        /// <summary>
        /// Update rule data
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            //Missing data, do nothing
            if(OperandId == null || string.IsNullOrEmpty(Key))
            {
                return;
            }

            var operand = ruleDataSet.GetOperand(OperandId.Value);

            if (operand == null)
            {
                throw new Exception("blah");

               // ruleDataSet.InsertResponseOperand(Key);
            }
            else
            {
                operand.ResponseKey = Key;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteElementString("ResponseKey", Key);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
            Key = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ResponseKey"));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        public override bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString(string languageCode)
        {
            return Key;
        }


        /// <summary>
        /// Validate the operand data
        /// </summary>
        public override bool Validate()
        {
            return true;
        }
    }
}
