using System;
using System.Web.UI;

namespace CheckboxWeb.ErrorPages
{
    public partial class ApplicationError : Page
    {
        /// <summary>
        /// Handle errors and redirect to application error page plain html page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnError(EventArgs e)
        {
            Response.Redirect("ApplicationError.html", true);
        }
    }
}