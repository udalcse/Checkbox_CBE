using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.ErrorPages
{
    /// <summary>
    /// Simple page to display a license limit error
    /// </summary>
    public partial class LimitError : SecuredPage
    {
        /// <summary>
        /// Set the message text
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _limitMessage.Text = Server.HtmlEncode(GetQueryStringValue("msg", WebTextManager.GetText("/pageText/limitError.aspx/noMessage")));
        }
    }
}
