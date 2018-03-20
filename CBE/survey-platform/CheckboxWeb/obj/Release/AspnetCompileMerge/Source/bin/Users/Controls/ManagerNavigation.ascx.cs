using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Security.Principal;
using Prezza.Framework.Security;

namespace CheckboxWeb.Users.Controls
{
    public partial class ManagerNavigation : Checkbox.Web.Common.UserControlBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Determine which page is loaded and highlight the correct tab
            switch (Page.AppRelativeVirtualPath.ToLowerInvariant())
            {
                case "~/users/manage.aspx":
                    _userManagerTab.Attributes["class"] = "first selected";
                    _groupManagerTab.Attributes["class"] = "";
                    _emailListManagerTab.Attributes["class"] = "";
                    break;
                case "~/users/groups/manage.aspx":
                    _userManagerTab.Attributes["class"] = "first";
                    _groupManagerTab.Attributes["class"] = "selected";
                    _emailListManagerTab.Attributes["class"] = "";
                    break;
                case "~/users/emaillists/manage.aspx":
                    _userManagerTab.Attributes["class"] = "first";
                    _groupManagerTab.Attributes["class"] = "";
                    _emailListManagerTab.Attributes["class"] = "selected";
                    break;
            }


            //Hide tabs that the current user cannot access
            _userManagerTab.Visible = AuthorizationFactory.GetAuthorizationProvider().Authorize(HttpContext.Current.User as CheckboxPrincipal, "Group.ManageUsers");
            _emailListManagerTab.Visible = AuthorizationFactory.GetAuthorizationProvider().Authorize(HttpContext.Current.User as CheckboxPrincipal, "EmailList.View");
            _groupManagerTab.Visible = AuthorizationFactory.GetAuthorizationProvider().Authorize(HttpContext.Current.User as CheckboxPrincipal, "Group.View");
            
        }
    }
}