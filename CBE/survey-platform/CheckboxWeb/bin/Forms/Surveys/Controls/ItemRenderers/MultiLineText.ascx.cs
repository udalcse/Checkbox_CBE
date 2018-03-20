using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Users;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Renderer for multiline text input
    /// </summary>
    public partial class MultiLineText : UserControlSurveyItemRendererBase
    {
        public bool IsTinyMce
        {
            get
            {
                bool result;

                bool.TryParse(Model.Metadata["isHtmlFormattedData"], out result);

                return result;
            }
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
            SetInputSize();
        }

        /// <summary>
        /// Bind model to inputs
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            _textInput.Value = Model.Answers.Length > 0
                ? Model.Answers[0].AnswerText
                : (Model.Metadata["DefaultText"] ?? string.Empty);

            //set the default text
            _textInput.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? string.Empty;

            _textInput.Attributes.Add("tinymce", IsTinyMce.ToString().ToLower());

            //add binded field flag to spot that it is binded control 
            if (!string.IsNullOrWhiteSpace(Model.Metadata["ConnectedCustomFieldKey"]))
                _textInput.Attributes.Add("bindedfield", "true");
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


        /// <summary>
        /// Checks whether the new property value passes validation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsInputValid(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Length > 1;
        }

        /// <summary>
        /// Update model with selected answer
        /// </summary>
        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            string text = (Request[_textInput.UniqueID] ?? _textInput.Value).Trim();

            UpsertTextAnswer(Model.Metadata["isHtmlFormattedData"] == "True"
                ? Utilities.RemoveScript(HttpUtility.HtmlDecode(text)).Replace("&apos;", "'")
                : text);

            UpdateConnectedProfileData(text);
        }

        /// <summary>
        /// Set size of inputs
        /// </summary>
        protected void SetInputSize()
        {
            int? rows = Utilities.AsInt(Appearance["Rows"]);
            int? columns = Utilities.AsInt(Appearance["Columns"]);

            if (rows.HasValue)
            {
                _textInput.Rows = rows.Value;
            }

            if (columns.HasValue)
            {
                _textInput.Cols = columns.Value;
            }
        }

        /// <summary>
        /// Reorganize controls and/or apply specific styles depending
        /// on item's label position setting.
        /// </summary>
        protected void SetLabelPosition()
        {
            //When label is set to bottom, we need to move controls from the top panel
            // to the bottom panel.  Otherwise, position changes are managed by setting
            // CSS class.
            if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                //Move text controls to bottom
                _bottomAndOrRightPanel.Controls.Add(_textContainer);

                //Move input to top
                _topAndOrLeftPanel.Controls.Add(_inputPanel);
            }

            //Set css classes
            _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" +
                                          (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"])
                                              ? Appearance["LabelPosition"]
                                              : "Top");
            _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" +
                                              (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"])
                                                  ? Appearance["LabelPosition"]
                                                  : "Top");
        }

        /// <summary>
        /// Set item position.
        /// </summary>
        protected void SetItemPosition()
        {
            _containerPanel.CssClass = "itemContainer itemPosition" +
                                       (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"])
                                           ? Appearance["ItemPosition"]
                                           : "Left");

            if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
            }
        }
    }
}