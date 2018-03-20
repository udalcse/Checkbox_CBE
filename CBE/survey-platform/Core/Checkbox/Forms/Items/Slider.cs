using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Input item that allows selection of one of a number of options from a range or a list.
    /// </summary>
    [Serializable]
    public class Slider : Select1
    {
        /// <summary>
        /// Get/Set Value Type
        /// </summary>
        public SliderValueType ValueType { get; set; }

        /// <summary>
        /// Get/Set Min Value
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// Get/Set Max Value
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// Size of a given “step” when moving the slider.  
        /// </summary>
        public int? StepSize { get; set; }

        /// <summary>
        /// Initial starting value for the slider. Uses only in NumberRange value type.
        /// </summary>
        public int? DefaultValue { get; set; }

        /// <summary>
        /// Get/set value list option type
        /// </summary>
        public SliderValueListOptionType ValueListOptionType { get; set; }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            var data = (SliderItemData)configuration;
            ValueType = data.ValueType;
            MinValue = data.MinValue;
            MaxValue = data.MaxValue;
            StepSize = data.StepSize;
            DefaultValue = data.DefaultValue;
            ValueListOptionType = data.ValueListOptionType;
        }

        /// <summary>
        /// Get instance collection
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection collection = base.GetInstanceDataValuesForSerialization();

            if (MinValue.HasValue)
                collection["MinValue"] = MinValue.ToString();

            if (MaxValue.HasValue)
                collection["MaxValue"] = MaxValue.ToString();

            if (StepSize.HasValue)
                collection["StepSize"] = StepSize.ToString();

            if (DefaultValue.HasValue)
                collection["DefaultValue"] = DefaultValue.ToString();

            collection["ValueType"] = ValueType.ToString();
            collection["ValueListOptionType"] = ValueListOptionType.ToString();

            return collection;
        }
       
        /// <summary>
        /// Set the answer
        /// </summary>
        /// <param name="answer"></param>
        public override void SetAnswer(object answer)
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
        /// Override this method to handle posting slider answers.
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            //Do nothing if object is not expected type
            if (AnswerData == null || !(dto is SurveyResponseItem))
                return;

            SurveyResponseItemAnswer[] postedAnswers = ((SurveyResponseItem)dto).Answers;

            //If slider type is numeric - save answer as simple 'text'
            if (postedAnswers.Length > 0 && !String.IsNullOrEmpty(postedAnswers[0].AnswerText))
            {
                DeleteAnswers();
                SetAnswer(postedAnswers[0].AnswerText);                
            }
            else
            {
                //"Select" Option
                Select(null, new List<int>(postedAnswers.Where(opt => opt.OptionId.HasValue).Select(opt => opt.OptionId.Value)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerList"></param>
        protected override void BuildDataTransferObjectAnswerList(List<SurveyResponseItemAnswer> answerList)
        {
            //Check for null answer data, which is the case when previewing or otherwise not running
            // within the context of a survey response.
            if (AnswerData == null)
                return;

            answerList.AddRange(AnswerData.BuildDataTransferObjectAnswerList(ID));
        }

        /// <summary>
        /// Write answers to select items
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="isText"></param>
        protected override void WriteAnswers(System.Xml.XmlWriter writer, bool isText)
        {
            if (HasAnswer)
            {
                if (ValueType == SliderValueType.NumberRange)
                {
                    writer.WriteStartElement("answer");
                    String imageUrl;
                    String answer = GetAnswer(out imageUrl);
                    GetAnswerData();

                    if (!String.IsNullOrEmpty(imageUrl))
                        writer.WriteAttributeString("imageUrl", imageUrl);

                    writer.WriteCData(answer);
                    writer.WriteEndElement();
                }
                else
                {
                    base.WriteAnswers(writer, isText);
                }
            }
        }

        /// <summary>
        /// Determine if the item has an answer or not
        /// </summary>
        public override bool HasAnswer
        {
            get { return !String.IsNullOrEmpty(GetAnswer()); }            
        }

        /// <summary>
        /// Get the answer for an item in string form
        /// </summary>
        /// <returns></returns>
        public override string GetAnswer()
        {
            String temp;
            return GetAnswer(out temp);
        }

        /// <summary>
        /// Get the answer for an item in string form. If this is image slider, set imageUrl of answer image.
        /// </summary>
        /// <returns></returns>
        public string GetAnswer(out String imageUrl)
        {
            imageUrl = string.Empty;

            if (AnswerData != null && !Excluded)
            {
                if (ValueType == SliderValueType.NumberRange)
                    return GetAnswerForNumericSlider(false);

                if (AnswerData.IsAnswered(ID))
                {
                    var optionAnswers = AnswerData.GetOptionAnswersForItem(ID);

                    if (optionAnswers.Any())
                    {
                        int optionId = optionAnswers.First().Key;

                        var option = Options.First(p => p.ID == optionId);

                        if (ValueListOptionType == SliderValueListOptionType.Text)
                            return option.Text;

                        imageUrl = ApplicationManager.ApplicationRoot +
                                            "/ViewContent.aspx?ContentId=" + option.ContentID;

                        return option.Alias;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAnswerForNumericSlider(bool returnDefaultIfEmpty)
        {
            //if empty answer is set, skip it
            if (AnswerData.HasEmptyAnswer(ID) && (Excluded || (Parent != null && Parent.Excluded)))
                return string.Empty;

            var answer = AnswerData.GetTextAnswerForItem(ID);

            if (returnDefaultIfEmpty && string.IsNullOrEmpty(answer))
            {
                if (DefaultValue.HasValue)
                    answer = DefaultValue.ToString();
                else
                {
                    int? average = null;
                    if (MinValue.HasValue && MaxValue.HasValue)
                        average = (MaxValue - MinValue) / 2;

                    var value = average ?? (MinValue ?? MaxValue);

                    if (value.HasValue)
                        answer = value.ToString();
                }
            }

            return answer;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnItemExcluded()
        {
            base.OnItemExcluded();

            //add an empty answer for a numeric slider to avoid default value calculation 
            if (ValueType == SliderValueType.NumberRange && AnswerData != null)
                AnswerData.SetEmptyAnswerForItem(ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override double GetScore()
        {
            if (ValueType == SliderValueType.NumberRange)
            {
                double res;
                string sRes = GetAnswer();
                if (double.TryParse(sRes, out res))
                    return res;

                return 0.0;                
            }
            return base.GetScore();
        }

        public override double GetPossibleMaxScore()
        {
            if (ValueType == SliderValueType.NumberRange)
            {
                return MaxValue.HasValue ? MaxValue.Value : 0d;
            }
            return base.GetPossibleMaxScore();
        }

        /// <summary>
        /// 
        /// </summary>
        internal override void InitializeDefaults()
        {
            ListOption firstOption = null;

            //If there are no answer rows, mark default option values as selected
            if (AnswerData == null || !AnswerData.IsAnswered(ID))
            {
                foreach (ListOption option in OptionsDictionary.Values)
                {
                    if (firstOption == null)
                        firstOption = option;                            
                         
                    option.IsSelected = option.IsDefault;
                    if (option.IsDefault)
                        return;
                }

                //no default -- so make the first one as selected
                if (firstOption != null)
                    firstOption.IsSelected = true;
            }
        }
    }
}
