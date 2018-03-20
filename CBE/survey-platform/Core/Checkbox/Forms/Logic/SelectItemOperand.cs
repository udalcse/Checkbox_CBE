using System.Collections.Generic;
using Checkbox.Forms.Items;
using System;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Operand for comparing select items
    /// </summary>
    [Serializable]
    public class SelectItemOperand : ItemDependentOperand
    {
        private readonly int _itemId;

        /// <summary>
        /// Construct a select item operand
        /// </summary>
        /// <param name="itemId"></param>
        public SelectItemOperand(int itemId)
        {
            _itemId = itemId;
        }

        /// <summary>
        /// Initialize the operand value
        /// </summary>
        /// <param name="operandValue"></param>
        /// <param name="initializationValue"></param>
        /// <param name="response"></param>
        public override void InitializeOperandValue(OperandValue operandValue, object initializationValue, Response response)
        {
            //Call base message
            base.InitializeOperandValue(operandValue, initializationValue, response);

            //
            if (operandValue is SelectOperandValue)
            {
                SelectItem item = response.GetItem(_itemId) as SelectItem;

                if (item == null)
                {
                    return;
                }

                List<ListOption> options = item.Options;

                foreach (ListOption option in options)
                {
                    ((SelectOperandValue)operandValue).TextValues[option.ID] = option.Text;
                }
            }
        }

        /// <summary>
        /// Create the operand value object
        /// </summary>
        /// <returns></returns>
        public override OperandValue CreateOperandValue()
        {
            return new SelectOperandValue();
        }

        /// <summary>
        /// Get the select item ids
        /// </summary>
        /// <returns></returns>
        protected List<int> GetOptionIds(Response response)
        {
            List<int> optionIds = new List<int>();

            SelectItem item = response.GetItem(_itemId) as SelectItem;
            
            if (item == null)
            {
                return new List<int>();

            }
            
            List<ListOption> selectedOptions = item.SelectedOptions;

            foreach (ListOption option in selectedOptions)
            {
                optionIds.Add(option.ID);
            }

            return optionIds;            
        }

        /// <summary>
        /// Return list of selected options
        /// </summary>
        /// <returns></returns>
        protected override object GetValue(Response response)
        {
            return GetOptionIds(response);
        }

        /// <summary>
        /// Item ID
        /// </summary>
        public int ItemID
        {
            get
            {
                return _itemId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override List<int> SourceItemIds
        {
            get { return new List<int> { _itemId }; }
        }
    }
}
