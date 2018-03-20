using System;
using System.Web.UI;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Invitations.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AddRecipients : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected int InvitationId { get; set; }

        protected int SurveyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PendingInvitationMode { get; set; }

        public bool UsersOnly { get; set; }

        public bool IsPrepMode => ApplicationManager.AppSettings.IsPrepMode;

        //
        public string ViewRecipientListCallback { get; set; }

        //
        public string GridCssClass
        {
            get { return _addEmailLists.GridCssClass; }
            set { 
                _addEmailLists.GridCssClass = value;
                _addGroupsGrid.GridCssClass = value;
                _addUsersGrid.GridCssClass = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invitation"></param>
        public void Initialize(Invitation invitation, int surveyId = 0)
        {
            if (invitation == null)
            {
                InvitationId = 0;
                UsersOnly = true;
                SurveyId = surveyId;

                _addUsersGrid.ListTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addOnlyUsersListTemplate.html");
                _addUsersGrid.ListItemTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addOnlyUsersListItemTemplate.html");
            }
            else
            {
                InvitationId = invitation.ID.Value;
            }
        }

        /// <summary>
        /// Get/set callback for handling recipients added event
        /// </summary>
        public String OnRecipientsAdded { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _addUsersGrid.InitialSortField = "UniqueIdentifier";
            _addUsersGrid.ListTemplatePath =
                ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsUsersListTemplate.html");
            _addUsersGrid.ListItemTemplatePath =
                ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsUsersListItemTemplate.html");

            _addUsersGrid.LoadDataCallback = "loadAddRecipientUsersList";
            _addUsersGrid.EmptyGridText =
                WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/noUsers");
            _addUsersGrid.FilterItemType = "searchTerm1";
            
            if (!IsPrepMode)
            {

                _addGroupsGrid.InitialSortField = "GroupName";
                _addGroupsGrid.ListTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsGroupsListTemplate.html");
                _addGroupsGrid.ListItemTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsGroupsListItemTemplate.html");
                _addGroupsGrid.LoadDataCallback = "loadAddRecipientGroupsList";
                _addGroupsGrid.EmptyGridText =
                    WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/noGroups");
                _addGroupsGrid.FilterItemType = "searchTerm2";

                _addEmailLists.InitialSortField = "Name";
                _addEmailLists.ListTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsEmailListTemplate.html");
                _addEmailLists.ListItemTemplatePath =
                    ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/addRecipientsEmailListItemTemplate.html");
                _addEmailLists.LoadDataCallback = "loadAddRecipientEmailLists";
                _addEmailLists.EmptyGridText =
                    WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/noEmailLists");
                _addEmailLists.FilterItemType = "searchTerm3";
            }
            else
            {
                //_addUsersGrid.Visible = false;
                _addGroupsGrid.Visible = false;
                _addEmailLists.Visible = false;
            }
        }


    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RegisterClientScriptInclude(
              "serviceHelper.js",
              ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcInvitationManagement.js",
                ResolveUrl("~/Services/js/svcInvitationManagement.js"));

            RegisterClientScriptInclude(
              "svcUserManagement.js",
              ResolveUrl("~/Services/js/svcUserManagement.js"));

            RegisterClientScriptInclude(
             "statusControl.js",
             ResolveUrl("~/Resources/StatusControl.js"));

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));

            RegisterClientScriptInclude(
                "gridLiveSearch.js",
                ResolveUrl("~/Resources/gridLiveSearch.js"));
        }
    }
}