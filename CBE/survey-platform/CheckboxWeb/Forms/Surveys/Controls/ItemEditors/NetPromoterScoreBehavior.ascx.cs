using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Common;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class NetPromoterScoreBehavior : UserControlBase
    {
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
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Initialize editor
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="languageCode"></param>
        public void Initialize(RatingScaleItemTextDecorator textDecorator, int templateId, int? pagePosition, string languageCode)
        {
            _aliasText.Text = textDecorator.Data.Alias;
            _requiredCheck.Checked = textDecorator.Data.IsRequired;

            LanguageCode = languageCode;
            TemplateId = templateId;
            PagePosition = pagePosition;
        }

        /// <summary>
        /// Update decorator with updated values
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(RatingScaleItemTextDecorator textDecorator)
        {
            textDecorator.Data.Alias = _aliasText.Text;
            textDecorator.Data.IsRequired = _requiredCheck.Checked;
        }
    }
}