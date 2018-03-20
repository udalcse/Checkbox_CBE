using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using Checkbox.Forms.Validation;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Input item that allows selection of one of a number of options from a range or a list.
    /// </summary>
    [Serializable]
    public class RankOrder : SelectItem
    {
        /// <summary>
        /// Get/Set Type of the item
        /// </summary>
        public RankOrderType RankOrderType { get; set; }

        /// <summary>
        /// Get/Set Rank order option type
        /// </summary>
        public RankOrderOptionType RankOrderOptionType { get; set; }

        /// <summary>
        /// N, top N items will be selected
        /// </summary>
        public int? N { get; set; }

        /// <summary>
        /// Items should be randomized when presented
        /// </summary>
        public bool? Randomize { get; set; }

        /// <summary>
        /// Determine the count of shown options
        /// </summary>
        public int ShownOptionsCount
        {
            get
            {
                if (!N.HasValue)
                    return Options.Count;

                return Math.Min(N.Value, Options.Count);
            }
        }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            var data = (RankOrderItemData)configuration;
            RankOrderType = data.RankOrderType;
            RankOrderOptionType = data.RankOrderOptionType;
            N = data.N;
            Randomize = data.Randomize;
        }

        /// <summary>
        /// Get instance collection
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection collection = base.GetInstanceDataValuesForSerialization();

            if (N.HasValue)
                collection["N"] = N.ToString();

            if (Randomize.HasValue)
                collection["Randomize"] = Randomize.ToString();

            collection["RankOrderType"] = RankOrderType.ToString();
            collection["RankOrderOptionType"] = RankOrderOptionType.ToString();

            return collection;
        }

 
        /// <summary>
        /// Override on answer data set to synchronize options points
        /// </summary>
        protected override void OnAnswerDataSet()
        {
            base.OnAnswerDataSet();

            SynchronizePoinsOptions();
        }
        

        /// <summary>
        /// Override this method to handle posting rank order answers.
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            //Do nothing if object is not expected type
            if (AnswerData == null
                || !(dto is SurveyResponseItem))
            {
                return;
            }

            //"Select" Options
            SurveyResponseItemAnswer[] postedAnswers = ((SurveyResponseItem)dto).Answers;
            Dictionary<int, double> optionsIdsWithPoints = new Dictionary<int, double>();

            foreach (var answer in postedAnswers)
            {
                if (answer.OptionId.HasValue && answer.Points.HasValue)
                    optionsIdsWithPoints.Add(answer.OptionId.Value, answer.Points.Value);
            }

            Select(optionsIdsWithPoints);
        }

        /// <summary>
        /// Select the answer
        /// </summary>
        /// <param name="optionsIdsWithPoints"></param>
        public virtual void Select(Dictionary<int, double> optionsIdsWithPoints)
        {
            var newlySelectedOptions = optionsIdsWithPoints.Select(p => p.Key).ToList();

            //Build a list of currently selected options
            var currentlySelectedOptions = AnswerData.GetOptionAnswersForItem(ID).Select(o => o.Key);

            //Now add new options
            foreach (int optionId in newlySelectedOptions)
            {
                if (currentlySelectedOptions.Contains(optionId))
                {
                    UpdateAnswer(optionId, optionsIdsWithPoints[optionId]);
                }
                else
                {
                    AddAnswer(optionId, optionsIdsWithPoints[optionId]);
                }
            }

            //Now remove unselected options
            foreach (int optionId in currentlySelectedOptions)
            {
                if (!newlySelectedOptions.Contains(optionId))
                {
                    DeleteAnswer(optionId);
                }
            }

            //Resync. selected options and options points
            SynchronizeSelectedOptions();
            SynchronizePoinsOptions();

            //Fire answer changed event
            OnAnswerChanged();
        }

        /// <summary>
        /// Synchronize options selected status with answer data
        /// </summary>
        protected override void SynchronizeSelectedOptions()
        {
            base.SynchronizeSelectedOptions();

            if (RankOrderType == RankOrderType.TopN)
            {
                var selectedOptions = AnswerData.GetOptionAnswersForItem(ID);
                foreach (var option in OptionsDictionary.Values)
                {
                    if (!selectedOptions.ContainsKey(option.ID))
                        option.Points = 0;
                }
            }
        }

        /// <summary>
        /// Synchronize options points with answer data
        /// </summary>
        public virtual void SynchronizePoinsOptions()
        {
            //If there are no answer rows, do nothing
            if (AnswerData == null || !AnswerData.IsAnswered(ID))
                return;

            var answeredOptions = new Dictionary<int, double>();

            //Now build the list of selected options from the answer data
            foreach (var option in AnswerData.GetOptionAnswersForItem(ID))
            {
                double points = 0;
                if (option.Value.HasValue)
                    points = (double) option.Value;

                answeredOptions.Add(option.Key, points);
            }

            foreach (ListOption option in OptionsDictionary.Values)
            {
                option.IsSelected = answeredOptions.ContainsKey(option.ID);
                option.Points = option.IsSelected ? answeredOptions[option.ID] : 0;
            }
        }
        

        /// <summary>
        /// Add an answer with points
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="points"></param>
        public virtual void AddAnswer(int optionId, double points)
        {
            //Don't allow adding answers to non-existant options
            if (!OptionsDictionary.ContainsKey(optionId))
            {
                return;
            }

            AnswerData.SetOptionAnswerForItem(ID, optionId, points, string.Empty);
        }

        /// <summary>
        /// Update the answer with the new points
        /// </summary>
        /// <param name="optionId"></param>
        /// <param name="points"></param>
        public virtual void UpdateAnswer(int optionId, double points)
        {
            var answers = AnswerData.GetOptionAnswersForItem(ID).Where(o => o.Key == optionId);

            foreach (var answer in answers)
            {
                if (answer.Value != points)
                {
                    AnswerData.SetOptionAnswerForItem(ID, answer.Key, points, null);
                }
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
        protected override void WriteAnswers(XmlWriter writer, bool isText)
        {
            List<ListOption> listOptions = ApplicationManager.AppSettings.ResponseDisplayRankOrderPoints ?
                Options : Options.OrderByDescending(o => o.Points).ToList();

            foreach (ListOption option in listOptions)
            {
                if (option.IsSelected)
                {
                    WriteOptionAnswer(option, writer);
                }
            }
        }

        /// <summary>
        /// Write the answer for a specific option
        /// </summary>
        /// <param name="option"></param>
        /// <param name="writer"></param>
        protected override void WriteOptionAnswer(ListOption option, XmlWriter writer)
        {
            writer.WriteStartElement("answer");
            writer.WriteAttributeString("optionId", option.ID.ToString());

            if (RankOrderOptionType == RankOrderOptionType.Image)
                writer.WriteCData(option.IsOther ? OtherText : string.IsNullOrEmpty(option.Alias) ? option.Text : option.Alias);
            else
                writer.WriteCData(option.IsOther ? OtherText : option.Text);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Validate answer. Ensure that all the textbox (dropDowns) were used.
        /// </summary>
        /// <returns></returns>
        protected override bool ValidateAnswers()
        {
            var rankOrderItemValidator = new RankOrderValidator();

            if (!rankOrderItemValidator.Validate(this))
            {
                ValidationErrors.Add(rankOrderItemValidator.GetMessage(LanguageCode));
                return false;
            }

            return true;
        }
    }
}
