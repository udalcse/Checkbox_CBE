using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Checkbox.Users;
using Checkbox.Web.Page;
using Checkbox.Web;
using System.Web.UI;
using System.Web.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Web;

namespace CheckboxWeb.Users
{
    public partial class Manage : SecuredPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Group.View"; }
        }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "m")]
        public string VisibleUserGrid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "selected")]
        public string Selected{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryParameter(ParameterName = "term")]
        public string Term { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanManageUsers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanViewUsers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanManageEveryone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanViewEmailLists { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanViewGroups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool CanManageGroups { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //string title = WebTextManager.GetText("/pageText/Users/Manage.aspx/title");

            //Prune choices based on user permissions
            var userPrincipal = UserManager.GetCurrentPrincipal();
            CanViewUsers = AuthorizationProvider.Authorize(userPrincipal, "Group.View");
            CanManageEveryone = AuthorizationProvider.Authorize(userPrincipal, GroupManager.GetEveryoneGroup(), "Group.ManageUsers");
            CanViewEmailLists = AuthorizationProvider.Authorize(userPrincipal, "EmailList.View");
            CanViewGroups = AuthorizationProvider.Authorize(userPrincipal, "Group.View");
            CanManageGroups = AuthorizationProvider.Authorize(userPrincipal, "Group.Edit");
            CanManageUsers = AuthorizationProvider.Authorize(userPrincipal, "Group.ManageUsers");

            HtmlGenericControl listItem;
            HtmlAnchor anchorItem;

            //Populate dropdowns and set default view
            if (CanManageUsers || CanViewUsers)
            {
                listItem = new HtmlGenericControl("li");
                anchorItem = new HtmlAnchor();
                
                anchorItem.Attributes["href"] = "javascript:loadGrid('u');";

                if (string.IsNullOrEmpty(VisibleUserGrid) || (!string.IsNullOrEmpty(VisibleUserGrid) && VisibleUserGrid == "u"))
                {
                    VisibleUserGrid = "u";
                    listItem.Attributes["class"] = "active";
                }

                anchorItem.InnerText = WebTextManager.GetText("/pageText/users/manage.aspx/users");

                listItem.Controls.Add(anchorItem);
                _userTabMenu.Controls.Add(listItem);
            }

            if(CanViewGroups)
            {
                listItem = new HtmlGenericControl("li");
                anchorItem = new HtmlAnchor();
                anchorItem.InnerText = WebTextManager.GetText("/pageText/users/manage.aspx/userGroups");
                anchorItem.Attributes["href"] = "javascript:loadGrid('g');";

                if (!string.IsNullOrEmpty(VisibleUserGrid) && VisibleUserGrid == "g")
                {
                    VisibleUserGrid = "g";
                    listItem.Attributes["class"] = "active";
                }

                listItem.Controls.Add(anchorItem);
                _userTabMenu.Controls.Add(listItem);
            }

            if(CanViewEmailLists)
            {
                listItem = new HtmlGenericControl("li");
                anchorItem = new HtmlAnchor();
                anchorItem.InnerText = WebTextManager.GetText("/pageText/users/manage.aspx/emailLists");
                anchorItem.Attributes["href"] = "javascript:loadGrid('e');";

                if (!string.IsNullOrEmpty(VisibleUserGrid) && VisibleUserGrid == "e")
                {
                    VisibleUserGrid = "e";
                    listItem.Attributes["class"] = "active";
                }

                listItem.Controls.Add(anchorItem);
                _userTabMenu.Controls.Add(listItem);
            }

            if (VisibleUserGrid == "g" && CanViewGroups)
            {
				_buttonAddGroup.Visible = CanManageGroups;
                _buttonAddMailList.Visible = false;
				_buttonAddUser.Visible = false;
				_buttonImportUser.Visible = false;

                _userList.Visible = false;
                _groupList.Visible = true;
                _emailListList.Visible = false;
                _timeline.VisibleEvents = "GROUP_CREATED,GROUP_EDITED";
            }
            else if (VisibleUserGrid == "e" && CanViewEmailLists)
            {
				_buttonAddGroup.Visible = false;
                _buttonAddMailList.Visible = true;
				_buttonAddUser.Visible = false;
				_buttonImportUser.Visible = false;

                _userList.Visible = false;
                _groupList.Visible = false;
                _emailListList.Visible = true;
                _timeline.VisibleEvents = "EMAILLIST_CREATED,EMAILLIST_EDITED";
            }
            else if (VisibleUserGrid == "u" && CanViewUsers)
            {
                _buttonAddGroup.Visible = false;
                _buttonAddMailList.Visible = false;
                _buttonAddUser.Visible = CanManageUsers;
                _buttonImportUser.Visible = CanManageUsers;

                _userList.Visible = true;
                _groupList.Visible = false;
                _emailListList.Visible = false;
                _timeline.VisibleEvents = "USER_CREATED,USER_EDITED";
            }
            else
            {
                Response.Redirect(UserDefaultRedirectUrl, false);
                return;
            }

            _userDashboard.Visible = _userList.Visible;
            _groupDashboard.Visible = _groupList.Visible;
            _emailListDashboard.Visible = _emailListList.Visible;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            LoadDatePickerLocalized();
        }

        [WebMethod]
        public static string SaveUserIds(string userIds)
        {
            var newKey = Guid.NewGuid();
            var items = JsonConvert.DeserializeObject<List<string>>(userIds);

            HttpContext.Current.Session.Add(newKey.ToString(), items);

            return newKey.ToString();
        }

    }
}
