using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Checkbox.Security;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// User profile logic operands
    /// </summary>
    [Serializable]
    public class ProfileOperandData : OperandData
    {
        /// <summary>
        /// 
        /// </summary>
        public string ProfileKey { get; set; }

        /// <summary>
        /// Get a text type name for this operand
        /// </summary>
        public override string OperandTypeName
        {
            get { return "ProfileOperand"; }
        }

        /// <summary>
        /// Load operand data
        /// </summary>
        /// <param name="operandId"></param>
        /// <param name="rulesData"></param>
        public override void Load(int operandId, RulesObjectSet rulesData)
        {
            var operand = rulesData.GetOperand(operandId);

            if (operand == null)
            {
                throw new Exception("Unable to find data for Profile operand with id: " + operandId);
            }

            OperandId = operandId;
            ProfileKey = operand.ProfileKey;
        }

        /// <summary>
        /// Update rule data
        /// </summary>
        /// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            base.UpdateRuleData(ruleDataSet);

            //Missing data
            if(OperandId == null || string.IsNullOrEmpty(ProfileKey))
            {
                return;
            }

            var operand = ruleDataSet.GetOperand(OperandId.Value);

            if (operand == null)
            {
                throw new Exception("blah");
                /*
                profile = ruleDataSet.InsertProfileOperand(ProfileKey);
                profile.Expressions*/
            }
            else
            {
                operand.ProfileKey = ProfileKey;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteElementString("ProfileKey", ProfileKey);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public override void Load(XmlNode xmlNode)
		{
		    ProfileKey = XmlUtility.GetNodeText(xmlNode.SelectSingleNode("ProfileKey"));
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
        public override bool Validate()
        {
            return ProfileManager.ListPropertyNames().Contains(ProfileKey, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override string ToString(string languageCode)
        {
            return ProfileKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ProfileKey;
        }
    }
}
