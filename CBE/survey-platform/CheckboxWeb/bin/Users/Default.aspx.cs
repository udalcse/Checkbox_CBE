using Checkbox.Web.Page;

namespace CheckboxWeb.Users
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Default : ApplicationPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Response.Redirect(ResolveUrl("~/Users/Manage.aspx"), false);
        }
    }
}
