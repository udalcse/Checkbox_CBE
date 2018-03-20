using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// Base class for user control-based item renderers
    /// </summary>
    /// 
    public class UserControlItemRendererBase : Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected ExportMode ExportMode { set; get; }

        /// <summary>
        /// Control width
        /// </summary>
        public Unit? Width { get; set; }

        /// <summary>
        /// Get/set item to be renderered
        /// </summary>
        public IItemProxyObject DataTransferObject { get; private set; }

        readonly SimpleNameValueCollection _appearance = new SimpleNameValueCollection();
        /// <summary>
        /// Get/set appearance data associated with item
        /// If item has no appearance return an empty one
        /// </summary>
        public SimpleNameValueCollection Appearance { get { return (DataTransferObject == null || DataTransferObject.AppearanceData == null) ? _appearance : DataTransferObject.AppearanceData; } }

        /// <summary>
        /// Get a child user control that may need to be initialized as well.
        /// </summary>
        public virtual List<UserControlItemRendererBase> ChildUserControls { get { return new List<UserControlItemRendererBase>(); } }

        /// <summary>
        /// Overridable method to be used by renderer builders to perform any initialization in inline code.
        /// </summary>
        protected virtual void InlineInitialize() { }

        /// <summary>
        /// Initialize the control with model and appearance
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        /// <param name="exportMode"></param>
        public virtual void Initialize(IItemProxyObject dataTransferObject, int? itemNumber, ExportMode exportMode)
        {
            ExportMode = exportMode;
            DataTransferObject = dataTransferObject;

            InitializeChildUserControls(dataTransferObject, itemNumber, exportMode);

            //TODO: Special error handling for inline methods
            InlineInitialize();
        }

        /// <summary>
        /// Initialize the control with model and appearance
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        public virtual void Initialize(IItemProxyObject dataTransferObject, int? itemNumber)
        {
            Initialize(dataTransferObject, itemNumber, ExportMode.None);
        }

        /// <summary>
        /// Intialize any child controls
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        /// <param name="exportMode"></param>
        protected virtual void InitializeChildUserControls(IItemProxyObject dataTransferObject, int? itemNumber, ExportMode exportMode)
        {
            foreach (UserControlItemRendererBase childUserControl in ChildUserControls)
            {
                childUserControl.Initialize(dataTransferObject, itemNumber, exportMode);
            }
        }

        /// <summary>
        /// Bind renderer controls to model
        /// </summary>
        public virtual void BindModel()
        {
            BindChildUserControls();

            //TODO: Special error handling for inline methods
            InlineBindModel();
        }

        /// <summary>
        /// Bind child user controls
        /// </summary>
        protected virtual void BindChildUserControls()
        {
            foreach (UserControlItemRendererBase childUserControl in ChildUserControls)
            {
                childUserControl.BindModel();
            }
        }

        /// <summary>
        /// Update child user controls
        /// </summary>
        protected virtual void UpdateChildUserControls()
        {
            foreach (UserControlItemRendererBase childUserControl in ChildUserControls)
            {
                childUserControl.UpdateModel();
            }
        }


        /// <summary>
        /// Overridable method for binding controls in inline code.
        /// </summary>
        protected virtual void InlineBindModel()
        {
        }

        /// <summary>
        /// Update model with state of item
        /// </summary>
        public virtual void UpdateModel()
        {
            UpdateChildUserControls();

            //TODO: Special error handling for inline methods
            InlineUpdateModel();
        }

        /// <summary>
        /// Overridable method for binding controls in inline code.
        /// </summary>
        protected virtual void InlineUpdateModel()
        {
        }

        #region Error Messages

        /// <summary>
        /// Get the first error message, if any.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetErrorMessageText()
        {
            if (!(DataTransferObject is SurveyResponseItem))
            {
                return string.Empty;
            }

            return ((SurveyResponseItem)DataTransferObject).ValidationErrors.Length > 0
               ? ((SurveyResponseItem)DataTransferObject).ValidationErrors[0]
               : string.Empty;
        }

        #endregion

        #region Answer Related Methods

        /// <summary>
        /// Get the text answer for first answer in the item's answers collection.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTextAnswer()
        {
            if (!(DataTransferObject is SurveyResponseItem))
            {
                return string.Empty;
            }

            return ((SurveyResponseItem)DataTransferObject).Answers.Length > 0
                ? ((SurveyResponseItem)DataTransferObject).Answers[0].AnswerText
                : string.Empty;
        }

        /// <summary>
        /// Set text answer for item
        /// </summary>
        /// <param name="answerText"></param>
        protected virtual void UpsertTextAnswer(string answerText)
        {
            if (!(DataTransferObject is SurveyResponseItem))
            {
                return;
            }

            SurveyResponseItemAnswer answerToUpdate = ((SurveyResponseItem)DataTransferObject).Answers.Length == 0
                                                          ? new SurveyResponseItemAnswer()
                                                          : ((SurveyResponseItem)DataTransferObject).Answers[0];

            answerToUpdate.AnswerText = (answerText ?? string.Empty).Trim();

            ((SurveyResponseItem)DataTransferObject).Answers = new[] { answerToUpdate };
        }

        /// <summary>
        /// Set option answers for item
        /// </summary>
        /// <param name="optionIds"></param>
        /// <param name="otherText"></param>
        protected virtual void UpsertOptionAnswers(List<int> optionIds, string otherText)
        {
            if (!(DataTransferObject is SurveyResponseItem))
            {
                return;
            }

            SurveyResponseItemOption otherOption = ((SurveyResponseItem)DataTransferObject).Options.FirstOrDefault(opt => opt.IsOther);
            SurveyResponseItemAnswer[] currentAnswerList = ((SurveyResponseItem)DataTransferObject).Answers;

            //Add answers already selected
            var newAnswerList = currentAnswerList.Where(
                currentAnswer =>
                    currentAnswer.OptionId != null
                    && optionIds.Contains(currentAnswer.OptionId.Value)
            ).ToList();

            //Add newly selected answers
            foreach (int optionId in optionIds)
            {
                if (newAnswerList.Find(ans => ans.OptionId == optionId) == null)
                {
                    //Create new answer and set other text, if necsssary
                    var newAnswer = new SurveyResponseItemAnswer { OptionId = optionId };

                    if (otherOption != null && optionId == otherOption.OptionId)
                    {
                        newAnswer.AnswerText = otherText;
                    }
                    else if (!string.IsNullOrWhiteSpace(DataTransferObject.Metadata["ConnectedCustomFieldKey"]))
                    {
                        newAnswer.AnswerText = ((SurveyResponseItem)DataTransferObject).Options.First(s => s.OptionId == optionId).Text;
                    }

                    newAnswerList.Add(newAnswer);
                }
                else if (otherOption != null && optionId == otherOption.OptionId)
                {
                    var i = newAnswerList.FindIndex(ans => ans.OptionId == optionId);
                    newAnswerList[i].AnswerText = otherText;
                }
            }

            //Set answers on data object
            ((SurveyResponseItem)DataTransferObject).Answers = newAnswerList.ToArray();

        }

        /// <summary>
        /// Set option answers with points for the item
        /// </summary>
        /// <param name="optionsIDsWithPoints"></param>
        protected virtual void UpsertOptionAnswers(Dictionary<int, double> optionsIDsWithPoints)
        {
            if (!(DataTransferObject is SurveyResponseItem))
            {
                return;
            }

            SurveyResponseItemAnswer[] currentAnswerList = ((SurveyResponseItem)DataTransferObject).Answers;

            //Add answers already selected
            var newAnswerList = currentAnswerList.Where(
                currentAnswer =>
                    currentAnswer.OptionId != null
                    && optionsIDsWithPoints.ContainsKey(currentAnswer.OptionId.Value)
            ).ToList();
            
            foreach (var optionWithPoint in optionsIDsWithPoints)
            {
                //if this options is already in answer
                var answerOption = newAnswerList.Find(p => p.OptionId == optionWithPoint.Key);
                if (answerOption == null)
                {
                    //Add new answer.
                    newAnswerList.Add(new SurveyResponseItemAnswer { OptionId = optionWithPoint.Key, Points = optionWithPoint.Value });
                }
                else
                {
                    //Update the existed answer.
                    answerOption.Points = optionWithPoint.Value;
                }
            }

            //Set answers on data object
            ((SurveyResponseItem)DataTransferObject).Answers = newAnswerList.ToArray();
        }

        #endregion
    }
}
