using System.Web;
using Checkbox.Management;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckboxServerProtectedPage : BasePage
    {
        /// <summary>
        /// Retrieve and Validate the license.
        /// </summary>
        public CheckboxServerProtectedPage()
        { 
            //If the multidatabase is allowed, redirect the error page
            if (ApplicationManager.AppSettings.EnableMultiDatabase)
            {
                HttpContext.Current.Server.Transfer(ResolveUrl("~/ErrorPages/NotAvailableError.aspx"));
            }
        }
    }
}
