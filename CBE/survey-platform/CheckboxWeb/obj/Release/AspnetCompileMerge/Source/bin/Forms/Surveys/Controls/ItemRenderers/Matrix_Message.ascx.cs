
using System.Web;
using Checkbox.Common;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Message renderer
    /// </summary>
    public partial class Matrix_Message : UserControlSurveyItemRendererBase
    {
        /// <summary>
        /// Bind control with the model
        /// </summary>
        protected override void InlineBindModel()
        {
            base.InlineBindModel();

            var isHtmlWrapped = false;

            if (!string.IsNullOrWhiteSpace(Model.Text))
            {
                var htmlEncode = HttpUtility.HtmlDecode(Model.Text);

                if (htmlEncode != null)
                    isHtmlWrapped = htmlEncode.Contains(@"class=""html-wrapper""");
            }

            _messageLiteral.Text = GetMatrixRowText(Model.Text , isHtmlWrapped);
        }
    }
}