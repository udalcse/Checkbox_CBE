using System;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MatrixItemOperandData : ItemOperandData
    {
        ///<summary>
        ///</summary>
        public int? MatrixId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public MatrixItemOperandData()
            : this(null, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="parentItemID"></param>
        public MatrixItemOperandData(int? itemID, int? parentItemID)
            : base(itemID)
        {
            MatrixId = parentItemID;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ruleDataSet"></param>
        public override void UpdateRuleData(RulesObjectSet ruleDataSet)
        {
            throw new NotImplementedException();
        }
    }
}
