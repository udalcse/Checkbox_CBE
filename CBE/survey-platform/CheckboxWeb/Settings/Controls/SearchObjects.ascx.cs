using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Security;

namespace CheckboxWeb.Settings.Controls
{
    public partial class SearchObjects : Checkbox.Web.Common.UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            _rolesRepeater.DataSource = RoleManager.ListRoles();
            _rolesRepeater.DataBind();
        }
    }
}