using System.Collections.Generic;
using System.Web;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile
{
    public partial class MultiLineText : UserControlSurveyItemRendererBase
    {
        private string OriginValue { get; set; }

        /// <summary>
        /// Default multi line rows count
        /// </summary>
        private const int MultiLineRowsCount = 6;

        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected void SetLabelPosition()
        {
            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer labelTop";
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabelTop";
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPositionLeft";
        }

        /// <summary>
        /// 
        /// </summary>
        public override List<UserControlItemRendererBase> ChildUserControls
        {
            get
            {
                var childControls = base.ChildUserControls;
                childControls.Add(_questionText);
                return childControls;
            }
        }

        /// <summary>
        /// Initialize child user controls to set repeat columns and other appearance properties
        /// </summary>
        protected override void InlineInitialize()
        {
            //Item and label position
            SetLabelPosition();
            SetItemPosition();
        }

        /// <summary>
        /// Bind model to inputs
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _textInput.Rows = MultiLineRowsCount;

            _textInput.Text = Model.Answers.Length > 0
                ? Model.Answers[0].AnswerText
                : (Model.Metadata["DefaultText"] ?? string.Empty);

            //save origin value
            OriginValue = _textInput.Text;

            _textInput.Text = Utilities.StripHtml(_textInput.Text, null, false);

            //set the default text
            _textInput.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                _textInput.Attributes.Add("bindedfield", "true");
        }

        /// <summary>
        /// Update model with selected answer
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            var initialTextValue = Request[_textInput.UniqueID].Trim();

            var text = Utilities.RemoveNewLineCharacters(initialTextValue);

            var originalStrippedHtmlText = Utilities.RemoveNewLineCharacters(Utilities.StripHtml(OriginValue).Trim());

            text = originalStrippedHtmlText.Equals(text)
                ? OriginValue
                : Utilities.StripHtml(initialTextValue);


            if (Model.Metadata["isHtmlFormattedData"] == "True")
            {
                text = Utilities.RemoveScript(HttpUtility.HtmlDecode(text)).Replace("&apos;", "'");
                text = text.Replace("\n", "<br />");
            }
           
            UpsertTextAnswer(text);

            UpdateConnectedProfileData(text);
        }

        /// <summary>
        /// Updates Profile Properties if the Profile Property connected the the item
        /// It means that the whole question is connected to Profile Property value
        /// And no matter which answer will be chosen by survey taker 
        /// it will be saved to this profile property
        /// </summary>
        /// <param name="inputText"></param>
        private void UpdateConnectedProfileData(string inputText)
        {
            // if there is no property key , don't save connected value
            if (string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                return;

            var name = PropertyBindingManager.GetCurrentUserName();

            if (!string.IsNullOrWhiteSpace(name))
            {
                ProfilePropertiesUpdater propertiesUpdater = new ProfilePropertiesUpdater();
                propertiesUpdater.UpdateUserProfileData(inputText,
                    Model.Metadata["ConnectedCustomFieldKey"], name);
            }
        }
    }
}