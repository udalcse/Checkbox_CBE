using System.Web;
using System.Web.UI;
using Checkbox.Security.Principal;

namespace CheckboxWeb.Controls
{
    public partial class Footer : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected virtual CheckboxPrincipal CurrentPrincipal
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return null;
                }

                return HttpContext.Current.User as CheckboxPrincipal;
            }
        }
    }
}