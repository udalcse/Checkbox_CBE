using System;
using System.Linq;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;


namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Answerable item
    /// </summary>
    [Serializable]
    public abstract class AnswerableItem : ResponseItem, IAnswerable
    {
        #region IAnswerable Members

        /// <summary>
        /// 
        /// </summary>
        private IAnswerData _answerData;

        /// <summary>
        /// Event fired when the item's answer changes
        /// </summary>
        public event EventHandler AnswerChanged;

        /// <summary>
        /// Get/set the response answer data
        /// </summary>
        public IAnswerData AnswerData
        {
            get { return GetAnswerData(); }
            set
            {
                //Set the answer data
                SetAnswerData(value);

                if (value != null)
                {
                    //Call answer data set
                    OnAnswerDataSet();
                }
            }
        }

        /// <summary>
        /// Overridable method to set answer data
        /// </summary>
        /// <param name="answerData"></param>
        protected virtual void SetAnswerData(IAnswerData answerData)
        {
            _answerData = answerData;
        }

        /// <summary>
        /// Overridable method to get answer data
        /// </summary>
        /// <returns></returns>
        protected virtual IAnswerData GetAnswerData()
        {
            return _answerData;
        }

        /// <summary>
        /// Overridable method to allow items that extend the response item to
        /// perform some processing when the item's answer data is set.
        /// </summary>
        protected virtual void OnAnswerDataSet()
        {
            //Reset the valid flag
            Valid = true;
        }

        /// <summary>
        /// Get the answer for an item in string form
        /// </summary>
        /// <returns></returns>
        public virtual string GetAnswer()
        {
            if (AnswerData != null)
                return AnswerData.GetTextAnswerForItem(ID);

            return string.Empty;
        }

        /// <summary>
        /// Returns all answer data
        /// </summary>
        /// <returns></returns>
        public virtual string GetRawAnswer()
        {
            return GetAnswer();
        }

        /// <summary>
        /// Set the answer
        /// </summary>
        /// <param name="answer"></param>
        public virtual void SetAnswer(object answer)
        {
            if (AnswerData != null)
            {
                AnswerData.SetTextAnswersForItem(ID, answer != null
                        ? answer.ToString().Trim()
                        : string.Empty);

                //Fire answer changed event
                OnAnswerChanged();
            }
        }

        /// <summary>
        /// Determine if the item has an answer or not
        /// </summary>
        public virtual bool HasAnswer
        {
            get
            {
                if (AnswerData == null)
                    return false;
                                
                return !string.IsNullOrEmpty(AnswerData.GetTextAnswerForItem(ID));
            }
        }

        /// <summary>
        /// Delete answers for the item
        /// </summary>
        public virtual void DeleteAnswers()
        {
            if (AnswerData != null)
            {
                AnswerData.DeleteAllAnswersForItem(ID);

                //Fire answer changed event
                OnAnswerChanged();
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void UpdateMergedText()
        {
            OnAnswerChanged();
        }

        /// <summary>
        /// Overridable method called when the response state is restored
        /// </summary>
        protected override void OnStateRestored()
        {
            if (AnswerData != null)
            {
                if (AnswerData.IsAnswered(ID))
                {
                    OnAnswerChanged();
                }
            }
        }

        /// <summary>
        /// Fire answer changed event
        /// </summary>
        protected virtual void OnAnswerChanged()
        {
            if (AnswerChanged != null)
            {
                AnswerChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Delete answers when item is excluded
        /// </summary>
        protected override void OnItemExcluded()
        {
            base.OnItemExcluded();

            DeleteAnswers();
        }

        /// <summary>
        /// Validate the item's answers
        /// </summary>
        protected override bool DoValidateItem()
        {
            return ValidateAnswers();
        }

        /// <summary>
        /// Validate the answers to the item
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidateAnswers();


        /// <summary>
        /// Build data transfer object
        /// </summary>
        /// <param name="itemDto"></param>
        protected override void BuildDataTransferObject(IItemProxyObject itemDto)
        {
            base.BuildDataTransferObject(itemDto);

            if (itemDto is SurveyResponseItem)
            {
                var answers = new List<SurveyResponseItemAnswer>();

                BuildDataTransferObjectAnswerList(answers);

                ((SurveyResponseItem) itemDto).IsAnswerable = true;
                ((SurveyResponseItem) itemDto).Answers = answers.ToArray();
            }
        }

        /// <summary>
        /// Build answer list for data transfer object
        /// </summary>
        /// <param name="answerList"></param>
        protected virtual void BuildDataTransferObjectAnswerList(List<SurveyResponseItemAnswer> answerList)
        {
            //Check for null answer data, which is the case when previewing or otherwise not running
            // within the context of a survey response.
            if (AnswerData == null)
                return;

            answerList.AddRange(AnswerData.BuildDataTransferObjectAnswerList(ID));
        }

        /// <summary>
        /// Store posted answers from item dto.
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            base.UpdateFromDataTransferObject(dto);

            if(AnswerData == null
                || !(dto is SurveyResponseItem))
            {
                return;
            }

            //Add new text answers.
            SurveyResponseItemAnswer[] postedAnswers = ((SurveyResponseItem)dto).Answers;

            if(postedAnswers.Length > 0)
            {
                SetAnswer(postedAnswers[0].AnswerText);
            }
            else
            {
                DeleteAnswers();
            }
        }

        #region Xml Serialization

        /// <summary>
        /// Get metadata data values
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetMetaDataValuesForSerialization()
        {
            NameValueCollection values = base.GetMetaDataValuesForSerialization();
            values["answerRequired"] = Required.ToString();

            return values;
        }

        /// <summary>
        /// Write xml instance information, including answers
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        public override void WriteXmlInstanceData(XmlWriter writer, bool isText)
        {
            base.WriteXmlInstanceData(writer, isText);

            writer.WriteStartElement("answers");
            WriteAnswers(writer, isText);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write answer(s) to xml
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        protected virtual void WriteAnswers(XmlWriter writer, bool isText)
        {
            if (HasAnswer)
            {
                writer.WriteStartElement("answer");
                string rawAnswer = GetRawAnswer();
                writer.WriteCData(Common.Utilities.RemoveScript(rawAnswer));
                writer.WriteEndElement();
            }
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcItem"></param>
		/// <param name="answers"></param>
		protected internal override void ImportAnswers(Analytics.Import.ItemInfo srcItem, List<Analytics.Data.ItemAnswer> answers)
		{
			if (answers == null)
				return;

			for (int i = 0; i < answers.Count; i++)
			{
				if (answers[i].ItemId == srcItem.itemId)
				{
					SetAnswer(answers[i].AnswerText);
					break;
				}
			}
		}
    }
}
