using System;
using System.Xml;
using System.Collections.Generic;


namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public abstract class OperandData
    {
        /// <summary>
        /// Get operand data id
        /// </summary>
        public int? OperandId { get; protected set; }

        /// <summary>
        /// Get a string type name for the operand
        /// </summary>
        public abstract string OperandTypeName { get; }

        /// <summary>
        /// Provides an Array of LogicalOperators by which this OperandData may be compared
        /// </summary>
        public virtual Array SupportedLogicalOperators
        {
            get { return Enum.GetNames(typeof(LogicalOperator)); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public virtual string ToString(string languageCode)
        {
            return string.Empty;
        }

        /// <summary>
        /// Update rule data with item configuration
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public virtual void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            if(!OperandId.HasValue)
            {
                OperandId = ruleDataSet.InsertVoidOperand(GetType()).Id;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlNode"></param>
        public abstract void Load(XmlNode xmlNode);

        /// <summary>
        /// 
        /// </summary>
		/// <param name="operandId"></param>
		/// <param name="rulesData"></param>
		public abstract void Load(int operandId, RulesObjectSet rulesData);

        /// <summary>
        /// Validate the operand data
        /// </summary>
        public abstract bool Validate();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public abstract void WriteXml(XmlWriter writer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        /// <param name="prototypesMap"></param>
        public abstract bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap);
	}
}