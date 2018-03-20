using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Common;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Editor for multi line text input behavior options
    /// </summary>
    public partial class MultiLineTextBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        public bool PageIsPostback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int TemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected int? PagePosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected EditMode EditMode { get; set; }

        /// <summary>
        /// Initialize control with data to edit.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(TextItemDecorator textItemDecorator, bool isPagePostBack, string currentLanguage, int templateId, int? pagePosition, EditMode editMode)
        {
            if (textItemDecorator.Data.IsHtmlFormattedData)
                _defaultHtml.Text = textItemDecorator.DefaultText;
            else
                _defaultText.Text = textItemDecorator.DefaultText;

			_aliasText.Text = textItemDecorator.Data.Alias;
            _requiredChk.Checked = textItemDecorator.Data.IsRequired;
            _isHtmlFormattedDataChk.Checked = textItemDecorator.Data.IsHtmlFormattedData;
            _minCharLimit.Text = textItemDecorator.Data.MinLength.HasValue
                ? textItemDecorator.Data.MinLength.ToString()
                : string.Empty;
            _maxCharLimit.Text = textItemDecorator.Data.MaxLength.HasValue
                ? textItemDecorator.Data.MaxLength.ToString()
                : string.Empty;

            PageIsPostback = isPagePostBack;
            LanguageCode = currentLanguage;
            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;

            //Initialize pipeSelector
            switch (EditMode)
            {
                case EditMode.Survey:
                    _pipeSelectorHtml.Initialize(TemplateId, PagePosition, LanguageCode, _defaultHtml.ClientID);
                    _pipeSelectorText.Initialize(TemplateId, PagePosition, LanguageCode, _defaultText.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelectorHtml.Initialize(null, null, LanguageCode, _defaultHtml.ClientID);
                    _pipeSelectorText.Initialize(null, null, LanguageCode, _defaultText.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelectorHtml.Visible = false;
                    _pipeSelectorText.Visible = false;
                    break;
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            _pipeSelectorHtml.ID = ID + "_" + _pipeSelectorHtml.ID;
            _pipeSelectorText.ID = ID + "_" + _pipeSelectorText.ID;

            base.OnInit(e);

        }

        /// <summary>
        /// Update data with user inputs.
        /// </summary>
        /// <param name="textItemDecorator"></param>
        public void UpdateData(TextItemDecorator textItemDecorator)
        {
			textItemDecorator.Data.Alias = _aliasText.Text;
            textItemDecorator.Data.IsRequired = _requiredChk.Checked;
            textItemDecorator.Data.IsHtmlFormattedData = _isHtmlFormattedDataChk.Checked;
            textItemDecorator.DefaultText = _isHtmlFormattedDataChk.Checked? _defaultHtml.Text : _defaultText.Text;
            textItemDecorator.Data.MinLength = Utilities.AsInt(_minCharLimit.Text);
            textItemDecorator.Data.MaxLength = Utilities.AsInt(_maxCharLimit.Text);
        }
    }
}