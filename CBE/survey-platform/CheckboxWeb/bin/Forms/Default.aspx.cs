using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms
{
    public partial class Default : Checkbox.Web.Page.BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            //TODO: * Ensure autoevent wireup is off on all pages
            //TODO: * Redirects have false argument to end response
            Response.Redirect("Manage.aspx", false);
        }
    }
}
