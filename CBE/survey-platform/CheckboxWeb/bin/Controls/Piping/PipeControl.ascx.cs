using System;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web.Common;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Controls.Piping
{
    /// <summary>
    /// </summary>
    public partial class PipeControl : UserControlBase
    {
        /// <summary>
        ///     Control which is associated with pipeControl
        /// </summary>
        protected string AssociatedControlId { get; set; }

        /// <summary>
        /// </summary>
        protected int? ResponseTemplateId { get; set; }

        /// <summary>
        /// </summary>
        protected int? MaxPagePosition { get; set; }

        /// <summary>
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// </summary>
        protected CustomFieldType? ProfileFieldType { get; set; }

        //if it is matrix , single line , multiline or radio button control, put two way binding dropdown list
        public bool IsTwoWayBindingControl => ProfileFieldType.HasValue &&
                                              (ProfileFieldType == CustomFieldType.SingleLine || ProfileFieldType == CustomFieldType.MultiLine ||
                                               ProfileFieldType == CustomFieldType.RadioButton || ProfileFieldType == CustomFieldType.Matrix);

        public bool UseTerms = true;

        /// <summary>
        /// Initialize the pipe control
        /// </summary>
        /// <param name="responseTemplateId">The response template identifier.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="associatedControlId">The associated control identifier.</param>
        /// <param name="profileFieldType">Type of the profile field.</param>
        public void Initialize(int? responseTemplateId, int? currentPage, string languageCode,
            string associatedControlId, CustomFieldType? profileFieldType = null)
        {
            MaxPagePosition = currentPage.HasValue ? currentPage - 1 : null;
            AssociatedControlId = associatedControlId;
            LanguageCode = languageCode;
            ResponseTemplateId = responseTemplateId;

            if (profileFieldType.HasValue)
            {
                this.ProfileFieldType = profileFieldType;
                _questionBindingSelector.Initialize(ProfileManager.GetProfileKeysWithIds(ProfileFieldType.ToString()));
            }
            
            if (responseTemplateId != null)
                _termBindingSelector.Initialize(responseTemplateId.Value, associatedControlId);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcSurveyEditor.js",
                ResolveUrl("~/Services/js/svcSurveyEditor.js"));

            RegisterClientScriptInclude(
                "svcSurveyManagement.js",
                ResolveUrl("~/Services/js/svcSurveyManagement.js"));

            RegisterClientScriptInclude(
                "jquery.groupedDropDown.js",
                ResolveUrl("~/Resources/jquery.groupedDropDown.js"));

            RegisterClientScriptInclude(
                "PipeHandler.js",
                ResolveUrl("~/Resources/PipeHandler.js"));

            RegisterClientScriptInclude(
                "TermHandler.js",
                ResolveUrl("~/Resources/TermHandler.js"));

            RegisterClientScriptInclude(
                "jquery.a-tools-1.5.2.min.js",
                ResolveUrl("~/Resources/jquery.a-tools-1.5.2.min.js"));

            _mergeButton.Attributes["mergefor"] = ID + "_sourceList";
        }
    }
}