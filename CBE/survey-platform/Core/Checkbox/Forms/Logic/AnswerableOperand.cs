using System;
using System.Collections.Generic;
using Checkbox.Forms.Items;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// An Operand that wraps a <see cref="Checkbox.Forms.Items.Item"/> object, using the Item's 
    /// state to construct the Operand value
    /// </summary>
    [Serializable]
    public class AnswerableOperand : ItemDependentOperand
    {
        private readonly int _itemId;

        /// <summary>
        /// Construct a new item operand
        /// </summary>
        /// <param name="itemId"></param>
        public AnswerableOperand(int itemId)
        {
            _itemId = itemId;
        }

        /// <summary>
        /// 
        /// </summary>
        public int ItemID
        {
            get
            {
                return _itemId;
            }
        }

        /// <summary>
        /// Get the "value" of the item. Null indicates no value.
        /// </summary>
        protected override object GetValue(Response response)
        {
            var item = response.GetItem(_itemId);
            IAnswerable answerableItem = item as IAnswerable;

            if (answerableItem == null)
            {
                return null;
            }

            string answer = null;

            var slider = item as Slider;
            if (slider != null && slider.ValueType == SliderValueType.NumberRange)
            {
                answer = slider.GetAnswerForNumericSlider(true);
            }
            else
            {
                if (!answerableItem.HasAnswer)
                    return null;

                answer = answerableItem.GetAnswer();

                if (string.IsNullOrEmpty(answer))
                    return answer;

                if (answerableItem is MultiLineTextBoxItem)
                    answer = Utilities.StripHtmlTags(answer);
            }

            return answer;
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
