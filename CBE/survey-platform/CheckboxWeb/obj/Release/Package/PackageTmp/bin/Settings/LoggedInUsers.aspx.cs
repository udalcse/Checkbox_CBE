using System.Data;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings
{
    public partial class LoggedInUsers : SettingsPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            Master.IsDialog = false;
            ConfigureGridColumns();
            //Master.DisableSave();

            //Set up the page title with link back to mananger
            PlaceHolder titleControl = new PlaceHolder();
            HyperLink managerLink = new HyperLink();
            managerLink.NavigateUrl = "~/Settings/Manage.aspx";
            managerLink.Text = WebTextManager.GetText("/pageText/settings/manage.aspx/title");
            Label pageTitleLabel = new Label();
            pageTitleLabel.Text = " - ";
            pageTitleLabel.Text += WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/title");
            titleControl.Controls.Add(managerLink);
            titleControl.Controls.Add(pageTitleLabel);
            Master.SetTitleControl(titleControl);

            Master.HideDialogButtons();
            serverName.Text = Request.ServerVariables["SERVER_NAME"];

            _users.DataSource = GetUsers();
            _users.PageSize = 10;
            _users.DataBind();
        }

        /// <summary>
        /// Grid Page Index Changing handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnUserGridPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _users.PageIndex = e.NewPageIndex;
            _users.DataBind();
        }

        private DataTable GetUsers()
        {
            DataTable table = new DataTable("UsersList");

            table.Columns.Add("UniqueIdentifier");
            table.Columns.Add("UserHostName");
            table.Columns.Add("UserHostIP");
            table.Columns.Add("UserAgent");
            table.Columns.Add("LoginTime");
            table.Columns.Add("CurrentUrl");

            var infoList = UserManager.LoggedInUsers();

            foreach (UserLoginInfo info in infoList)
            {
                object[] values = { info.UserName, info.UserHostName, info.UserHostIp, info.UserAgent, info.LoginTime, info.CurrentUrl };
                table.Rows.Add(values);
            }

            numberOfUsers.Text = infoList.Length.ToString();

            return table;
        }

        /// <summary>
        /// Sets up the grid columns that will be displayed
        /// </summary>
        private void ConfigureGridColumns()
        {
            _users.Columns[0].HeaderText = WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/user");
            _users.Columns[1].HeaderText = WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/userHost");
            _users.Columns[2].HeaderText = WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/userIPAddress");
            _users.Columns[3].HeaderText = WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/loggedInSince");
            _users.Columns[4].HeaderText = WebTextManager.GetText("/pageText/settings/loggedInUsers.aspx/currentlyViewing");
        }
    }
}
