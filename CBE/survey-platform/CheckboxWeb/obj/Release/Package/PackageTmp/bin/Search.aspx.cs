using System;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Search : SecuredPage
    {
        /// <summary>
        /// 
        /// </summary>
        [QueryParameter("term")]
        public string SearchTerm { get; set; }

        /// <summary>
        /// Get/set whether user can list surveys
        /// </summary>
        public bool CanListSurveys { get; set; }

        /// <summary>
        /// Get/set whether user can view responses
        /// </summary>
        public bool CanViewResponses { get; set; }

        /// <summary>
        /// Get/set whether user can view users
        /// </summary>
        public bool CanViewUsers { get; set; }

        /// <summary>
        /// Get/set whether user can view reports
        /// </summary>
        public bool CanViewReports { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanViewInvitations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Master.SetTitle(string.Empty);
            Master.LeftPanelWidth = Unit.Pixel(570);
            Master.HideSearch();

            _searchText.Text = Server.HtmlDecode(SearchTerm);

            if (Utilities.IsNotNullOrEmpty(SearchTerm)
                && _searchAreaList.Items.FindByValue("everywhere") != null)
            {
                _searchAreaList.SelectedValue = "everywhere";

                if (!Page.IsPostBack)
                {
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "startSearch",
                        "$(document).ready(function(){startSearch('" + SearchTerm.Replace("'", "\\'") + "', 'everywhere');});",
                        true);
                }
            }

            //Perform auth to remove inappropriate choices
            CanListSurveys = AuthorizationProvider.Authorize(CurrentPrincipal, "FormFolder.Read") || AuthorizationProvider.Authorize(CurrentPrincipal, "Form.Fill");
            CanViewInvitations = AuthorizationProvider.Authorize(CurrentPrincipal, "Form.Administer");
            CanViewResponses = AuthorizationProvider.Authorize(CurrentPrincipal, "Analysis.Responses.View");
            CanViewUsers = AuthorizationProvider.Authorize(CurrentPrincipal, "Group.View");
            CanViewReports = AuthorizationProvider.Authorize(CurrentPrincipal, "Analysis.Run")
                             || AuthorizationProvider.Authorize(CurrentPrincipal, "Analysis.Edit")
                             || AuthorizationProvider.Authorize(CurrentPrincipal, "Analysis.Administer");


            if (!CanListSurveys && _searchAreaList.Items.FindByValue("surveys") != null)
            {
                _searchAreaList.Items.Remove("surveys");
            }

            if (!CanViewResponses && _searchAreaList.Items.FindByValue("responses") != null)
            {
                _searchAreaList.Items.Remove("responses");
            }

            if (!CanViewUsers && _searchAreaList.Items.FindByValue("users") != null)
            {
                _searchAreaList.Items.Remove("users");
            }

            if (!CanViewUsers && _searchAreaList.Items.FindByValue("usergroups") != null)
            {
                _searchAreaList.Items.Remove("usergroups");
            }

            if (!CanViewReports && _searchAreaList.Items.FindByValue("reports") != null)
            {
                _searchAreaList.Items.Remove("reports");
            }

            if (!CanViewInvitations && _searchAreaList.Items.FindByValue("invitations") != null)
            {
                _searchAreaList.Items.Remove("invitations");
            }
        }
    }
}