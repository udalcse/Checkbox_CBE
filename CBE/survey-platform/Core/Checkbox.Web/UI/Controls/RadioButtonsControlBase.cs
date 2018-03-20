using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Security;
using Newtonsoft.Json;


namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RadioButtonsControlBase : UserControlSurveyItemRendererBase
    {
        private string StoredResponseSessionDataKey
        {
            get { return "SurveySession_" + Guid.Parse(Request.QueryString["s"]); }
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
                foreach (var option in Model.Options.Where(option => !option.IsOther))
                {
                    result.Add(new SurveyResponseItemOption()
                    {
                        Text = string.Format("{0}. {1}", num, option.Text),
                        ContentId = option.ContentId,
                        IsDefault = option.IsDefault,
                        IsOther = option.IsOther,
                        IsNoneOfAbove = option.IsNoneOfAbove,
                        IsSelected = option.IsSelected,
                        OptionId = option.OptionId,
                        Points = option.Points,
                        Alias = option.Alias
                    });
                    num++;
                }

                return result.Union((from o in Model.Options
                                     where o.IsOther
                                     select o));
            }
            return Model.Options;
        }

        /// <summary>
        /// Get options for the item
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SurveyResponseItemOption> GetNonOtherOptions()
        {
            var options = Model.Options;
            foreach (var option in options)
            {
                option.Text = string.Format("<div class=\"radioButtonLabel\">{0}</div>", option.Text);
            }

            if ("True".Equals(Model.AppearanceData["ShowNumberLabels"]))
            {
                int num = 0;
                return (from o in options
                        where !o.IsOther
                        select new SurveyResponseItemOption
                        {
                            Text = string.Format("{0}. {1}", ++num, o.Text),
                            ContentId = o.ContentId,
                            IsDefault = o.IsDefault,
                            IsOther = o.IsOther,
                            IsNoneOfAbove = o.IsNoneOfAbove,
                            IsSelected = o.IsSelected,
                            OptionId = o.OptionId,
                            Points = o.Points
                        });
            }

            return options.Where(a => !a.IsOther);
        }

        /// <summary>
        /// Initialize child user controls to set repeat columns and other properties
        /// </summary>
        protected override void InlineInitialize()
        {
            SetLabelPosition();
            SetItemPosition();
        }

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
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected abstract void SetLabelPosition();

        /// <summary>
        /// Set item position.
        /// </summary>
        protected abstract void SetItemPosition();

        /// <summary>
        /// Update selected options
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            List<int> selectedOptionIds = new List<int>();

            //Handle other option
            string otherText = string.Empty;

            var val = Request[UniqueID + "_radio"];

            //Get selected option
            if (val != null)
            {
                int? itemId = Utilities.AsInt(val);

                if (itemId.HasValue)
                {
                    selectedOptionIds.Add(itemId.Value);

                    var option = Model.Options.FirstOrDefault(o => o.OptionId == itemId);
                    var optionIndex = Model.Options.ToList().FindIndex(o => o.OptionId.Equals(itemId));
                    if (option != null)
                    {
                        UpdateCustomFields(option.Text, optionIndex);
                    }

                    if (option != null && option.IsOther)
                        otherText = Utilities.StripHtml(Request[UniqueID + "_otherTxt"]);
                }
            }

            UpsertOptionAnswers(
                selectedOptionIds,
                otherText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        protected string GenerateRadioButtonMarkup(SurveyResponseItemOption option)
        {
            if (string.IsNullOrEmpty(option.Text)) return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.Append("<input type=\"radio\" ");

            if (option.IsSelected)
                builder.Append(" checked=\"checked\" ");

            if (option.IsDefault)
                builder.Append(" dataDefaultValue=\"checked\" ");

            builder.AppendFormat("name=\"{0}\" value=\"{1}\" id=\"{1}\" />", UniqueID + "_radio", option.OptionId);

            return builder.ToString();
        }

        private void UpdateCustomFields(string optionText, int optionIndex)
        {
            if (string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                return;

            var guid = PropertyBindingManager.GetCurrentUserGuid();
            var user = UserManager.GetUserByGuid(guid);

            var connectedRadioButton = ProfileManager.GetRadioButtonField(
                Model.Metadata["ConnectedCustomFieldKey"], guid);
            var optionAliases = ProfileManager.GetRadioOptionAliases(Model.ItemId, Model.Metadata["ConnectedCustomFieldKey"]);
            connectedRadioButton.Options.ForEach(o =>
            {
                var alias = optionAliases.Where(a => a.Key == o.Name);
                o.Alias = alias.Any() ? alias.FirstOrDefault().Value : string.Empty;
            });

            //user is not anonymous and has account
            if (!string.IsNullOrWhiteSpace(user?.Identity?.Name))
            {

                var selectedOption =
                    connectedRadioButton.Options.Where(o => o.Name == optionText || o.Alias == optionText)
                        .Select(p => p.Name)
                        .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(user.Identity?.Name) && !string.IsNullOrEmpty(selectedOption))
                {
                    ProfilePropertiesUpdater propertiesUpdater = new ProfilePropertiesUpdater();
                    propertiesUpdater.UpdateUserProfileData(selectedOption,
                        Model.Metadata["ConnectedCustomFieldKey"], user.Identity?.Name);

                    ProfileManager.UpdateRadioFieldSelectedOption(selectedOption,
                        Model.Metadata["ConnectedCustomFieldKey"], guid);
                }
            }
            else
            {

                var sessionState = ((ResponseSessionData) Session[StoredResponseSessionDataKey]);
                if (sessionState.AnonymousBindedFields == null)
                    sessionState.AnonymousBindedFields = new Dictionary<string, string>();
                connectedRadioButton.CheckOption(optionIndex);
                sessionState.AnonymousBindedFields[connectedRadioButton.Name] =
                    JsonConvert.SerializeObject(connectedRadioButton);
                Session[StoredResponseSessionDataKey] = sessionState;

            }

        }
    }
}
