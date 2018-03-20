using System;
using Checkbox.Common;

namespace Checkbox.Forms.ResponseStateEntities
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResponseAnswerEntity : EntityBase
    {
        private int _itemId;
        private string _answerText;
        private int? _optionId;
        private bool? _deleted;
        private double? _points;

        public ResponseAnswerEntity(int id)
        {
        }

        public ResponseAnswerEntity()
        {
        }

        public int ItemId
        {
            set
            {
                _itemId = value;
                Update();
            }
            get { return _itemId; }
        }

        public string AnswerText
        {
            set
            {
                _answerText = value;
                Update();
            }
            get { return _answerText; }
        }

        public int? OptionId
        {
            set
            {
                _optionId = value;
                Update();
            }
            get { return _optionId; }
        }

        public bool? Deleted
        {
            set
            {
                _deleted = value;
                Update();
            }
            get { return _deleted; }
        }

        public double? Points
        {
            set
            {
                _points = value;
                Update();
            }
            get { return _points; }
        }

        public void Empty()
        {
            State = EntityState.Empty;
        }

        public long? AnswerId { set; get; }
        public long? ResponseId { get; set; }
        public DateTime? DateCreated { set; get; }
        public DateTime? ModifiedDate { set; get; }
    }
}
