﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Invitations
{
    public partial class Default : Checkbox.Web.Page.BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Response.Redirect("Manage.aspx");
        }
    }
}
