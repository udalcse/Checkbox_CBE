using Checkbox.Web.Page;
using Checkbox.Web;

namespace CheckboxWeb.Settings
{
    public partial class Manage : SecuredPage
    {
        /// <summary>
        /// Get the required page role
        /// </summary>
        protected override string PageRequiredRole { get { return "System Administrator"; } }

        protected override void OnPageInit()
        {
            base.OnPageInit();

           
        }
    }
}
