using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Behavior editor for rating scale
    /// </summary>
    public partial class RatingScaleBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string DefaultNaText { get; private set; }

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
        /// Initialize editor
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="editMode"></param>
        public void Initialize(RatingScaleItemTextDecorator textDecorator, int templateId, int? pagePosition, EditMode editMode, string languageCode)
        {
			_aliasText.Text = textDecorator.Data.Alias;
            _minValue.Text = textDecorator.Data.StartValue.ToString();
            _maxValue.Text = textDecorator.Data.EndValue.ToString();

            LanguageCode = languageCode;

            _requiredCheck.Checked = textDecorator.Data.IsRequired;
            _allowNaCheck.Checked = textDecorator.Data.DisplayNotApplicable;

            _otherLabelTxt.Text = textDecorator.NotApplicableText;

            DefaultNaText = textDecorator.NotApplicableText;
            TemplateId = templateId;
            PagePosition = pagePosition;
            EditMode = editMode;            

            _startTextTxt.Text = textDecorator.StartText;
            _midTextTxt.Text = textDecorator.MidText;
            _endTextTxt.Text = textDecorator.EndText;
            
            //Initialize pipeSelector
            switch (EditMode)
            {
                case EditMode.Survey:
                    _pipeSelectorForOther.Initialize(TemplateId, PagePosition, LanguageCode, _otherLabelTxt.ClientID);
                    break;
                case EditMode.Library:
                    _pipeSelectorForOther.Initialize(null, null, LanguageCode, _otherLabelTxt.ClientID);
                    break;
                case EditMode.Report:
                    _pipeSelectorForOther.Visible = false;
                    break;
            }

            _otherOptionsPanel.Enabled = _allowNaCheck.Checked;
        }

        /// <summary>
        /// Update decorator with updated values
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(RatingScaleItemTextDecorator textDecorator)
        {
			textDecorator.Data.Alias = _aliasText.Text;

            textDecorator.Data.StartValue = Utilities.AsInt(_minValue.Text) ?? 0;
            textDecorator.Data.EndValue = Utilities.AsInt(_maxValue.Text) ?? 0;

            textDecorator.Data.IsRequired = _requiredCheck.Checked;
            textDecorator.Data.DisplayNotApplicable = _allowNaCheck.Checked;

            textDecorator.NotApplicableText = _otherLabelTxt.Text.Trim();

            textDecorator.StartText = _startTextTxt.Text.Trim();
            textDecorator.MidText = _midTextTxt.Text.Trim();
            textDecorator.EndText = _endTextTxt.Text.Trim();
        }

        public bool Validate(out string message)
        {
            if (string.IsNullOrEmpty(_minValue.Text))
            {
                message = TextManager.GetText("/controlText/ratingScaleItemEditor/startValueRequired");
                return false;
            }

            if (string.IsNullOrEmpty(_maxValue.Text))
            {
                message = TextManager.GetText("/controlText/ratingScaleItemEditor/endValueRequired");
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}