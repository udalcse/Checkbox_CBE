using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Operand wrapper for string values;
    /// </summary>
    [Serializable]
    public class StringOperandData : OperandData
    {

        ///<summary>
        ///</summary>
        public string Value { get; set; }

        /// <summary>
        /// Get a text type name for this operand
        /// </summary>
        public override string OperandTypeName
        {
            get { return "StringOperand"; }
        }

        /// <summary>
        /// Load operand data for string operand
        /// </summary>
        /// <param name="operandId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int operandId, RulesObjectSet rulesData)
        {
            var operand = rulesData.GetOperand(operandId);

            if (operand == null)
            {
                throw new Exception("Unable to find data for String (value) operand with id: " + operandId);
            }

            OperandId = operandId;
            Value = operand.AnswerValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            if(OperandId == null)
            {
                return;
            }

            var operand = ruleDataSet.GetOperand(OperandId.Value);

            if (operand == null)
            {
                throw  new Exception("blah");
            }
            else
            {
                operand.AnswerValue = Value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteElementString("AnswerValue", Value);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
            Value = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("AnswerValue"));
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
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Validate()
        {
            return true;
        }
    }
}
