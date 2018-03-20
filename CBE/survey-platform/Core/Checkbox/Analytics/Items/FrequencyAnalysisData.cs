using System;
using System.Collections.Generic;
using Checkbox.Analytics.Data;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Analytics.Items
{
    /// <summary>
    /// Container transporting results to frequency item data.
    /// </summary>
    [Serializable]
    public class FrequencyAnalysisData
    {
        private readonly Dictionary<string, int> _answerCounts;
        private readonly List<string> _otherTexts;

        /// <summary>
        /// Get the response count
        /// </summary>
        public Int32 ResponseCount
        {
            get { return _answerCounts.Count; }
        }

        /// <summary>
        /// Handling of "other" 
        /// </summary>
        public OtherOption OtherOption { get; set; }

        /// <summary>
        /// Get/set the "other" text
        /// </summary>
        public string OtherText { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FrequencyAnalysisData()
        {
            _answerCounts = new Dictionary<string, int>();
            _otherTexts = new List<string>();
            TotalAnswers = 0;
            OtherOption = OtherOption.Aggregate;
        }

        /// <summary>
        /// Add a possible answer
        /// </summary>
        /// <param name="answer"></param>
        public void AddPossibleAnswer(string answer)
        {
            if (!_answerCounts.ContainsKey(answer))
            {
                _answerCounts[answer] = 0;
            }
        }

        /// <summary>
        /// Increment an answer count
        /// </summary>
        /// <param name="answer"></param>
        public void Increment(string answer)
        {
            if (answer == null)
            {
                answer = string.Empty;
            }

            if (_answerCounts.ContainsKey(answer))
            {
                _answerCounts[answer] = _answerCounts[answer] + 1;
            }
            else
            {
                _answerCounts[answer] = 1;
            }

            TotalAnswers++;
        }

        /// <summary>
        /// Add an other text
        /// </summary>
        /// <param name="text"></param>
        public void AddOtherText(string text)
        {
            _otherTexts.Add(text);

            Increment(OtherOption == OtherOption.Display ? text : OtherText);
        }

        /// <summary>
        /// Get the answer counts
        /// </summary>
        public Dictionary<string, Int32> AnswerCounts
        {
            get { return _answerCounts; }
        }

        /// <summary>
        /// Get the other texts
        /// </summary>
        public List<string> OtherTexts
        {
            get { return _otherTexts; }
        }

        /// <summary>
        /// Get the total answers
        /// </summary>
        public Int32 TotalAnswers { get; private set; }

        /// <summary>
        /// Get the results
        /// </summary>
        public AnalysisItemResult ResultsData
        {
            get
            {
                var itemResult = new AnalysisItemResult();

                var resultList = new List<AggregateResult>(_answerCounts.Keys.Count);

                foreach (string key in _answerCounts.Keys)
                {
                    double percent;

                    if (TotalAnswers > 0)
                    {
                        percent = (double)100 * ((double)_answerCounts[key] / (double)TotalAnswers);
                    }
                    else
                    {
                        percent = (double)0;
                    }

                    resultList.Add(new AggregateResult
                    {
                        ResultText = String.Empty,
                        ResultKey = key,
                        AnswerCount = _answerCounts[key],
                        AnswerPercent = Math.Round(percent, 2) //,
                        //ResultPercentString = string.Format("{0} - {1}%", key, Convert.ToString(Math.Round(percent, 2)))
                    });
                }

                itemResult.AggregateResults = resultList.ToArray();
                return itemResult;
            }
        }
    }
}
