using System;

namespace CheckboxWeb.Styles
{
    public partial class Default : Checkbox.Web.Page.BasePage
    {
        protected override void  OnLoad(EventArgs e)
        {
            Response.Redirect(ResolveUrl("~/Styles/Manage.aspx"));
        }
    }
}
