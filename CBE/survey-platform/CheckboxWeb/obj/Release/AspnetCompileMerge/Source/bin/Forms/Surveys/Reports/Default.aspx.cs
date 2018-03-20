using System;
using System.Web.UI;


namespace CheckboxWeb.Forms.Surveys.Reports
{
    public partial class Default : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            Response.Redirect("Manage.aspx", false);
        }
    }
}
