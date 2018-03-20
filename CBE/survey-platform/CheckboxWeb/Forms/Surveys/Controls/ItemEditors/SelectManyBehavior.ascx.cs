using System;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Behavior options editor for select many items.
    /// </summary>
    public partial class SelectManyBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected bool IsMatrix { get; set; }
        
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
        /// 
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? CurrentMinToSelect { get { return Utilities.AsInt(_minToSelectTxt.Text); } }
        
        /// <summary>
        /// 
        /// </summary>
        public int? CurrentMaxToSelect { get { return Utilities.AsInt(_maxToSelectTxt.Text); } }
        
        /// <summary>
        /// 
        /// </summary>
        public bool CurrentAllowOtherCheck { get { return _allowOtherCheck.Checked; } }

        /// <summary>
        /// Override onload to enable/disable other option based on inputs
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

           RegisterClientScriptInclude(
            "jquery.numeric.js",
            ResolveUrl("~/Resources/jquery.numeric.js"));
        }

        /// <summary>
        /// Default other text
        /// </summary>
        protected string DefaultOtherText { get; private set; }

        /// <summary>
        /// Default none of above text
        /// </summary>
        protected string DefaultNoneOfAboveText { get; private set; }

        /// <summary>
        /// Initialize the behavior editor with the specified item data
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="showRandomize"></param>
        /// <param name="showAllOther"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        /// <param name="languageCode"></param>
        /// <param name="initNoneOfAbove"></param>
        /// <param name="isMatrix"></param>
        public void Initialize(SelectItemTextDecorator textDecorator, bool showRandomize, bool showAllOther,
            int templateId, int? pagePosition, EditMode editMode, string languageCode, bool initNoneOfAbove, bool isMatrix)
        {
            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);
            string itemId = HttpContext.Current.Request.Params["i"];
            string column = HttpContext.Current.Request.Params["c"];

            if(!(textDecorator.Data is SelectManyData))
            {
                return;
            }
            
            _allowOtherCheck.Checked = textDecorator.Data.AllowOther;
            _randomizeOptionsCheck.Checked = textDecorator.Data.Randomize;
            _allowNonOfAbove.Checked = ((SelectManyData)textDecorator.Data).AllowNoneOfAbove;

            IsMatrix = isMatrix;
            LanguageCode = languageCode;
            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;

			_aliasText.Text = textDecorator.Data.Alias;

            _minToSelectTxt.Text = ((SelectManyData)textDecorator.Data).MinToSelect.HasValue
                ? ((SelectManyData)textDecorator.Data).MinToSelect.ToString()
                : string.Empty;

            _maxToSelectTxt.Text = ((SelectManyData)textDecorator.Data).MaxToSelect.HasValue
                ? ((SelectManyData)textDecorator.Data).MaxToSelect.ToString()
                : string.Empty;
            
            _randomizeOptionsPlace.Visible = showRandomize;
            _allowOtherPlace.Visible = showAllOther;

            //Initialize other option editor
            if (Page != null && !IsPostBack)
            {
                if (fromHtmlRedactor && !string.IsNullOrEmpty(HtmlEditedOtherOptionValue(itemId, column)))
                {
                    DefaultOtherText = HtmlEditedOtherOptionValue(itemId, column);
                }
                else //init other option editor with textDecorator data or defaults
                {
                    DefaultOtherText = Utilities.IsNullOrEmpty(textDecorator.OtherText)
                                           ? TextManager.GetText("/common/otherTextDefault", textDecorator.Language)
                                           : Utilities.IsHtmlFormattedText(textDecorator.OtherText) ? textDecorator.OtherText : Utilities.AdvancedHtmlEncode(textDecorator.OtherText);
                }
                _otherOptionEditor.Initialize(DefaultOtherText, templateId, pagePosition, editMode,
                                              textDecorator.Language, textDecorator.Data.AllowOther, false);
            }

            if (initNoneOfAbove || (Page != null && !IsPostBack))
            {
                if (fromHtmlRedactor && !string.IsNullOrEmpty(HtmlEditedNoneOfAboveOptionValue(itemId, column)))
                {
                    DefaultNoneOfAboveText = HtmlEditedNoneOfAboveOptionValue(itemId, column);
                }
                else //init other option editor with textDecorator data or defaults
                {
                    DefaultNoneOfAboveText = Utilities.IsNullOrEmpty(textDecorator.NoneOfAboveText)
                                           ? TextManager.GetText("/common/noneOfAboveTextDefault", textDecorator.Language)
                                           : Utilities.IsHtmlFormattedText(textDecorator.NoneOfAboveText) ? textDecorator.NoneOfAboveText : Utilities.AdvancedHtmlEncode(textDecorator.NoneOfAboveText);
                }
                _nonOfAboveOptionEditor.Initialize(DefaultNoneOfAboveText, templateId, pagePosition, editMode,
                                              textDecorator.Language, ((SelectManyData)textDecorator.Data).AllowNoneOfAbove, false,
                                              TextManager.GetText("/controlText/nonOfAboveEntry/defaultNonOfAboveText", textDecorator.Language), _allowNonOfAbove.ClientID, isMatrix);                
            }

            _otherOptionsPanel.Enabled = _allowOtherCheck.Checked;
        }

        private string HtmlEditedOtherOptionValue(string itemId, string column)
        {
            return Session["temporary_otherOption_" + itemId + "_c=" + column] as string;
        }

        private string HtmlEditedNoneOfAboveOptionValue(string itemId, string column)
        {
            return Session["temporary_noneOfAbove_" + itemId + "_c=" + column] as string;
        }

        /// <summary>
        /// Initialize the editor item id
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(SelectItemTextDecorator textDecorator)
        {
            if (!(textDecorator.Data is SelectManyData))
            {
                return;
            }

			textDecorator.Data.Alias = _aliasText.Text;
            textDecorator.Data.AllowOther = _allowOtherCheck.Checked;
            textDecorator.Data.Randomize = _randomizeOptionsCheck.Checked;
            DefaultOtherText = textDecorator.OtherText = Utilities.AdvancedHtmlDecode(_otherOptionEditor.OtherLabelText);
            DefaultNoneOfAboveText = textDecorator.NoneOfAboveText = Utilities.AdvancedHtmlDecode(_nonOfAboveOptionEditor.OptionLabelText);

            ((SelectManyData)textDecorator.Data).MinToSelect = Utilities.AsInt(_minToSelectTxt.Text);
            ((SelectManyData)textDecorator.Data).MaxToSelect = Utilities.AsInt(_maxToSelectTxt.Text);
            ((SelectManyData)textDecorator.Data).AllowNoneOfAbove = _allowNonOfAbove.Checked;

            //Initialize other option editor after text changing
            _otherOptionEditor.Initialize(DefaultOtherText, TemplateId, PagePosition, EditMode, textDecorator.Language, textDecorator.Data.AllowOther, false);
            _nonOfAboveOptionEditor.Initialize(DefaultNoneOfAboveText, TemplateId, PagePosition, EditMode, textDecorator.Language, ((SelectManyData)textDecorator.Data).AllowNoneOfAbove, false, TextManager.GetText("/controlText/nonOfAboveEntry/defaultNonOfAboveText", textDecorator.Language), _allowNonOfAbove.ClientID, IsMatrix);
        }
    }
}