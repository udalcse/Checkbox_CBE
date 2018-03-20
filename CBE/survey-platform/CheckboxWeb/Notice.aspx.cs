using Checkbox.Users;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Notice : ApplicationPage
    {
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (UserManager.GetCurrentPrincipal() == null)
                Response.Redirect("Login.aspx");
        }
    }
}
