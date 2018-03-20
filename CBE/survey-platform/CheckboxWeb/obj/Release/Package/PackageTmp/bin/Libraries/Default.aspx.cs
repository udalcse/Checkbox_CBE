using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;

namespace CheckboxWeb.Libraries
{
    public partial class Default : SecuredPage
    {
        protected override void OnPageLoad()
        {
            Response.Redirect("~/Libraries/Manage.aspx");
        }
    }
}
