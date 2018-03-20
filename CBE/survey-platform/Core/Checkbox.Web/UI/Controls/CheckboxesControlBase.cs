using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class CheckboxesControlBase : UserControlSurveyItemRendererBase
    {
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
        /// Initialize child user controls to set repeat columns and other properties
        /// </summary>
        protected override void InlineInitialize()
        {
            SetLabelPosition();
            SetItemPosition();
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetAllOptions()
        {
            if ("True".Equals(Model.AppearanceData["ShowNumberLabels"]))
            {
                var result = new List<SurveyResponseItemOption>();
                var num = 1;
                foreach (var option in Model.Options.Where(option => !option.IsOther && !option.IsNoneOfAbove))
                {
                    result.Add(new SurveyResponseItemOption
                    {
                        Text = string.Format("{0}. {1}", num, option.Text),
                        ContentId = option.ContentId,
                        IsDefault = option.IsDefault,
                        IsNoneOfAbove = option.IsNoneOfAbove,
                        IsOther = option.IsOther,
                        IsSelected = option.IsSelected,
                        OptionId = option.OptionId,
                        Points = option.Points
                    });
                    num++;
                }

                return result.Union((from o in Model.Options
                                     where o.IsOther
                                     select o)).Union((from o in Model.Options
                                                       where o.IsNoneOfAbove
                                                       select o));
            }
            return Model.Options;
        }

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
        /// Record selected answers
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            List<int> selectedOptionIds = new List<int>();

            //Handle other option
            string otherText = string.Empty;

            //Get selected options
            foreach (var option in Model.Options)
            {
                var val = Request[option.OptionId.ToString()];

                if (val != null)
                {
                    int? itemId = Utilities.AsInt(val);

                    if (itemId.HasValue)
                    {
                        selectedOptionIds.Add(itemId.Value);
                    }

                    if (option.IsOther)
                        otherText = Utilities.StripHtml(Request[UniqueID + "otherTxt"]);
                }
                else
                    option.IsSelected = false;
            }

            UpsertOptionAnswers(selectedOptionIds, otherText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GenerateCheckboxMarkup(SurveyResponseItemOption option)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<input type=\"checkbox\" ");

            if (option.IsSelected)
                builder.Append(" checked=\"checked\" ");

            if (option.IsDefault)
                builder.Append(" dataDefaultValue=\"checked\" ");

            if (option.IsOther)
                builder.Append(" otherOption=\"true\" ");

            if (option.IsNoneOfAbove)
                builder.Append(" noneOfAbove=\"true\" ");

            builder.AppendFormat("name=\"{0}\" value=\"{0}\" id=\"{0}\" />", option.OptionId);

            return builder.ToString();
        }
    }
}
