using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DropdownListControlBase : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected abstract void SetLabelPosition();

        /// <summary>
        /// Set item position.
        /// </summary>
        protected abstract void SetItemPosition();

        /// <summary>
        /// Set other text input visibility
        /// </summary>
        protected abstract void SetOtherVisibility();

        /// <summary>
        /// 
        /// </summary>
        protected bool AllowOther
        {
            get
            {
                bool allowOther;
                bool.TryParse(Model.Metadata["AllowOther"], out allowOther);
                return allowOther;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsCombobox
        {
            get
            {
                bool isCombobox;
                bool.TryParse(Model.AppearanceData["ShowAsCombobox"], out isCombobox);
                return isCombobox;
            }
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetOptions()
        {
            var result = new List<SurveyResponseItemOption>(Model.Options);
            List<SurveyResponseItemOption> resultCollection;
            foreach (SurveyResponseItemOption o in result)
                o.Text = Utilities.SimpleHtmlDecode(o.Text);
            if ("True".Equals(Model.AppearanceData["ShowNumberLabels"]))
            {
                var num = 0;
                resultCollection = (from o in result
                                    where !o.IsOther
                                    select new SurveyResponseItemOption()
                                    {
                                        Text = string.Format("{0}. {1}", ++num, o.Text),
                                        ContentId = o.ContentId,
                                        IsDefault = o.IsDefault,
                                        IsOther = o.IsOther,
                                        IsNoneOfAbove = o.IsNoneOfAbove,
                                        IsSelected = o.IsSelected,
                                        OptionId = o.OptionId,
                                        Points = o.Points
                                    }).Union((from o in result
                                              where o.IsOther
                                              select o)).ToList();
            }
            else
            {
                resultCollection = result;
            }

            var defaultText = new SurveyResponseItemOption
            {
                Text = WebTextManager.GetText("/common/dropDownDefault", Model.LanguageCode)
            };

            if (string.IsNullOrEmpty(defaultText.Text))
                defaultText.Text = " ";

            defaultText.IsSelected = true;
            defaultText.IsDefault = true;

            resultCollection.Insert(0, defaultText);

            return resultCollection;
        }

        /// <summary>
        /// Initialize child user controls to set repeat columns and other properties
        /// </summary>
        protected override void InlineInitialize()
        {
            SetLabelPosition();
            SetItemPosition();
        }


        /// <summary>
        /// Update model
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            var selectedOptionIds = new List<int>();
            int? selectedOptionId = SelectedValue;

            string otherText = string.Empty;

            if (selectedOptionId.HasValue)
            {
                selectedOptionIds.Add(selectedOptionId.Value);

                //If selected option is an "other" option
                if (Model.Options.FirstOrDefault(opt => opt.OptionId == selectedOptionId.Value && opt.IsOther) != null)
                {
                    otherText = Utilities.StripHtml(OtherText);
                }
            }

            UpsertOptionAnswers(
                selectedOptionIds,
                otherText);
        }

        /// <summary>
        /// Get Id of other option if it is allowed
        /// </summary>
        public int OtherOptionId
        {
            get
            {
                return (AllowOther && Model.Options.FirstOrDefault(opt => opt.IsOther) != null)
                           ? Model.Options.FirstOrDefault(opt => opt.IsOther).OptionId
                           : -1;
            }
        }

        protected abstract int? SelectedValue { get; }
        protected abstract string OtherText { get; }


    }
}
