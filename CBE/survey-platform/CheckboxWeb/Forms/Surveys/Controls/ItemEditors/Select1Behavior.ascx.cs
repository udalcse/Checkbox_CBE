using System;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Editor widget for configuring radio button behaviors
    /// </summary>
    public partial class Select1Behavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected int TemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected EditMode EditMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// Default other text
        /// </summary>
        protected string DefaultOtherText { get; private set; }

        /// <summary>
        /// Disables/Enables other option formatting in html editor
        /// </summary>
        public bool DisableHtmlFormattedOtherOption { set; get; }

        /// <summary>
        /// Initialize the behavior editor with the specified item data
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="showRandomize"></param>
        /// <param name="showAllOther"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(SelectItemTextDecorator textDecorator, bool showRandomize, bool showAllOther, int templateId, int? pagePosition, EditMode editMode)
        {
            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);
            string itemId = HttpContext.Current.Request.Params["i"];
            string column = HttpContext.Current.Request.Params["c"];

            _aliasText.Text = textDecorator.Data.Alias;
            _allowOtherCheck.Checked = textDecorator.Data.AllowOther;
            _randomizeOptionsCheck.Checked = textDecorator.Data.Randomize;
            _requiredCheck.Checked = textDecorator.Data.IsRequired;

            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;

            _randomizeOptionsPlace.Visible = showRandomize;
            _allowOtherPlace.Visible = showAllOther;

            //Initialize other option editor
            if (Page != null && !IsPostBack)
            {
                if (fromHtmlRedactor && !string.IsNullOrEmpty(HtmlEditedOptionValue(itemId, column)))
                {
                    DefaultOtherText = HtmlEditedOptionValue(itemId, column);
                }
                else //init other option editor with textDecorator data or defaults
                {
                    DefaultOtherText = Utilities.IsNullOrEmpty(textDecorator.OtherText)
                                           ? TextManager.GetText("/common/otherTextDefault", textDecorator.Language)
                                           : Utilities.IsHtmlFormattedText(textDecorator.OtherText) ? textDecorator.OtherText : Utilities.AdvancedHtmlEncode(textDecorator.OtherText);
                }

                _otherOptionEditor.Initialize(DefaultOtherText, templateId, pagePosition, editMode,
                                              textDecorator.Language, textDecorator.Data.AllowOther, DisableHtmlFormattedOtherOption);
            }

            _otherOptionsPanel.Enabled = _allowOtherCheck.Checked;
        }

        private string HtmlEditedOptionValue(string itemId, string column)
        {
            return Session["temporary_otherOption_" + itemId + "_c=" + column] as string;
        }

        /// <summary>
        /// Initialize the editor item id
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(SelectItemTextDecorator textDecorator)
        {
            textDecorator.Data.Alias = _aliasText.Text;
            textDecorator.Data.AllowOther = _allowOtherCheck.Checked;
            textDecorator.Data.Randomize = _randomizeOptionsCheck.Checked;
            textDecorator.Data.IsRequired = _requiredCheck.Checked;

            DefaultOtherText = textDecorator.OtherText = Utilities.AdvancedHtmlDecode(_otherOptionEditor.OtherLabelText);

            //Initialize other option editor after text changing
            _otherOptionEditor.Initialize(DefaultOtherText, TemplateId, PagePosition, EditMode, textDecorator.Language, textDecorator.Data.AllowOther, DisableHtmlFormattedOtherOption);
        }
    }
}