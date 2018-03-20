<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="UserList.ascx.cs" Inherits="CheckboxWeb.Users.Controls.UserList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserStoreSelect" Src="~/Users/Controls/UserStoreSelect.ascx" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" Assembly="CheckboxWeb"%>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Prezza.Framework.Security" %>
<script type="text/javascript">
  <%-- Ensure service initialized --%>
    var <%= ClientID %>_provider = '<%= MembershipProviderManager.FirstAvailableProvider.Name %>';
    var <%= ClientID %>_providerType = '<%= MembershipProviderManager.FirstAvailableProvider.GetType().Name %>';

    $(document).ready(function() {   
        //implementation all/none selection
        $(document).on('change', 'input.deleteUser', function() {
            var deleteResponsesOnly = false;
            var totalUsers = $('.deleteUser:checked').length;
            
           //do not display the delete button if the only user selected is the logged in user
            if ($('#deleteUser_<%=WebUtilities.GetCurrentUserEncodedName() %>').attr('checked') && totalUsers == 1) {
                deleteResponsesOnly = true;
            }

           toggleGridActionButtons('user', this, totalUsers > 0, deleteResponsesOnly && <%= ClientID %>_provider == '<%= MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME %>');
        });
        $(document).on('click', '#_selectAllUsers', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteUser').prop('checked', true);
            } else {
                $('.deleteUser').prop('checked', false);
            }
            $.uniform.update('.deleteUser');
            toggleGridActionButtons('user', this, actionsAvailable, !<%= ClientID %>_provider == '<%= MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME %>');
        });

        $('#<%= UserStoreSelect.ClientID %>').on('membershipProviderChanged', function(e, eventId, membershipName, type) {
            if(eventId === 'userManager') {
                <%= ClientID %>_provider = membershipName;
                <%= ClientID %>_providerType = type;
                checkSortVisibility();
                resetGridMenu();
                //reset grid page
                gridHelper_<%=_userGrid.ClientID %>.currentPage = 1;
                reloadUserList();
            }
        });

        //Bind delete selected users click
        $('#_deleteSelectedUsersLink').click(function(){
            if($('.deleteUser:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsersConfirm") %>', 
                    onDeleteSelectedUsersConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsers") %>'
                );
            }
        });

        $('#_downloadUsersCSV').click(function(){
            if($('.deleteUser:checked').length > 0){
                var idArray = new Array();
                $('.deleteUser:checked').each(function(index){
                    idArray.push($(this).attr('value').replace(/'/g, "&#39;"));
                });

                $.ajax({
                    url: "Manage.aspx/SaveUserIds",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify({userIds : JSON.stringify(idArray)}),
                    datatype: "json",
                    success: function (result) {
                        showDialog("/Users/CsvExport/ExportProgress.aspx?UserList=" + result.d + "&loc=EN-US','properties', '', null, '#_surveyForm')");
                    }
                });

              
                console.log(idArray);
            }
        });

        //Bind delete responses of selected users click
        $('#_deleteSeletedUserResponsesLink').click(function(){
            if($('.deleteUser:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsersResponsesConfirm") %>', 
                    onDeleteSelectedUsersResponsesConfirm,
                    337,
                    200, 
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsersResponses") %>'
                );
            }
        });

        checkSortVisibility();
    });
     
    function checkSortVisibility() {
        if (<%= ClientID %>_provider != '<%= MembershipProviderManager.CHECKBOX_MEMBERSHIP_PROVIDER_NAME %>') {
            $('.gridSorter').hide();
        } else {
            $('.gridSorter').show();
        }
    }

    <%-- Load user list --%>
    function loadUserList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
        svcUserManagement.getUsers(
            _at,              
            <%= ClientID %>_provider,
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: '',
                filterValue: '',
                sortField: sortField,
                sortAscending: sortAscending
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }

    <%-- calculates the field name date for the given period --%>
    function getDateFieldName(eventName)
    {
        if (typeof(eventName) != 'undefined')
        {
            if (eventName.indexOf('CREATE') > 0)
                return 'Created';
            if (eventName.indexOf('EDIT') > 0)
                return 'ModifiedDate';
        }
        return null;
    }

    <%-- Load user list --%>
    function loadUserListAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        filterValue = (typeof(_lastSearchTerm) == 'undefined' || _lastSearchTerm == null || _lastSearchTerm.length == 0) ? filterValue : _lastSearchTerm;
        
        <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
            //if provider type is AD and user preloading is disabled, do nothing
            if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType && !filterValue) {
                    <%= _userGrid.ClearGridHandler %>();
                    return;
                }
        <% } %>

        svcUserManagement.getPageItemUsersData(
            _at, 
            <%= ClientID %>_provider,
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: filterField,
                filterValue: filterValue,
                sortField: sortField,
                sortAscending: sortAscending
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }

    //Confirm handler for deleting users
    function onDeleteSelectedUsersConfirm(){
        var idArray = new Array();

        $('.deleteUser:checked').each(function(index, u){
            if("<%=WebUtilities.GetCurrentUserEncodedName() %>" != $(this).attr('value')) {
                idArray.push($(this).attr('value').replace(/'/g, "&#39;"));
            }
        });

        if (idArray.length > 0){
            svcUserManagement.deleteUsers(
                _at,
                idArray,
                true,
                onUsersDeleted,
                idArray.length
            );
        }
    }

    function resetGridMenu() {
        $('.gridMenu').removeClass('actions-available');
    }

    function deleteUser(uniqueIdentifier) {
        showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUserConfirm") %>', 
                    onDeleteUserConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUser") %>',
                    uniqueIdentifier
                );
    }

    function onDeleteUserConfirm(uniqueIdentifier) {
        svcUserManagement
            .deleteUser(_at, uniqueIdentifier, true, onUsersDeleted, 1);
    }

    function deleteUserResponses(uniqueIdentifier) {
        showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUserResponsesConfirm") %>', 
                    onDeleteUserResponses,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUserResponses") %>',
                    uniqueIdentifier
                );
    }
    
    function onDeleteUserResponses(uniqueIdentifier) {
        //todo : write a DeleteResponsesOfUser() method in the usermanagement service
        var idArray = new Array();
        idArray.push(uniqueIdentifier.replace("_BACKSLASH_", "\\"));
        
        svcUserManagement
            .deleteResponsesOfUsers(_at, idArray, onDeleteUserResponses, idArray.length);
    }
    
    //User deleted handler
    function onUsersDeleted(resultData, count){
        reloadUserList(); 
        resetGridMenu();
        
        $('.userStoreSelect').parent().find('.gridMenu').removeClass('actions-available');

        <%if (!String.IsNullOrEmpty(OnUserDeleted))
          {%>
            <%=OnUserDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          
            var message = '';

            if(resultData.length > 0)
            {
               message='<%=WebTextManager.GetText("/users/groupDashboardTemplate/deletedPartially") %> '+resultData;                            
            }
            else
            {
               message = '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/usersDeleted") %>';
               message = message.replace('{0}', count);
            }
            var messageColorValue = resultData.length > 0 ? null:true;                           
          
          <%=ShowStatusMessageHandler %>(message, messageColorValue);
        <%
          }%> 
    }

    //Confirm handler for deleting responses
    function onDeleteSelectedUsersResponsesConfirm(){
        var idArray = new Array();

        $('.deleteUser:checked').each(function(index){
            idArray.push($(this).attr('value').replace(/'/g, "&#39;"));
        });

        if (idArray.length > 0){
            svcUserManagement.deleteResponsesOfUsers(
                _at,
                idArray,
                onUsersResponsesDeleted,
                idArray.length
            );
        }
    }

    //User responses deleted handler
    function onUsersResponsesDeleted(resultData, count){
       reloadUserList();
        resetGridMenu();
        <%if (!String.IsNullOrEmpty(OnUserDeleted))
          {%>
            <%=OnUserDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/userResponsesDeleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), true);
        <%
          }%> 
    }

    //Reload user list
    function reloadUserList(){
        <%=_userGrid.ReloadGridHandler %>(true);
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_userGrid.ShowSorter %>();
    }
</script>
<ckbx:UserStoreSelect EventID="userManager" ID="UserStoreSelect" ShowLabel="true" runat="server" />

<div class="gridMenu">
    <div class="userGridMenu">
            <a class="header-button ckbxButton silverButton" runat="server" style="font-size:0.9em;font-weight:normal;" id="_everyoneDefaultSecurityBtn" Visible="False"><%=WebTextManager.GetText("/pageText/users/Manage.aspx/manageEveryone")%></a>
        <div class="gridButtons">
            <div class="itemActionMenu">
                <a class="ckbxButton redButton" href="javascript:void(0);" id="_deleteSelectedUsersLink"><%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsers")%></a>
                <a class="ckbxButton silverButton right" href="javascript:void(0);" id="_downloadUsersCSV">Export Selected to CSV</a>
                <a class="ckbxButton redButton" href="javascript:void(0);" id="_deleteSeletedUserResponsesLink"><%=WebTextManager.GetText("/pageText/Users/Manage.aspx/deleteSelectedUsersResponses")%></a>
            </div>
        </div>
        <br class="clear" />
    </div>
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_userGrid" runat="server" GridCssClass="ckbxGrid" />

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
    /// <summary>
    /// Get/set callback for handling user delete event
    /// </summary>
    public string OnUserDeleted { get; set; }

    /// <summary>
    /// Callback for invitation selected
    /// </summary>
    public string UserSelectedClientCallback { get; set; }

    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _userGrid.InitialSortField = "UniqueIdentifier";
        _userGrid.ItemClickCallback = UserSelectedClientCallback;
        _userGrid.ListTemplatePath = ResolveUrl("~/Users/jqtmpl/userListTemplate.html");
        _userGrid.ListItemTemplatePath = ResolveUrl("~/Users/jqtmpl/userListItemTemplate.html");
        _userGrid.LoadDataCallback = "loadUserListAjax";
        _userGrid.EmptyGridText = WebTextManager.GetText("/pageText/Users/Manage.aspx/noUsers");
        _userGrid.IsAjaxScrollModeEnabled = true;
        _userGrid.InitialFilterField = string.IsNullOrEmpty(Request.Params["term"]) ? "UniqueIdentifier" : "";
        _userGrid.FilterItemType = "user";
        _userGrid.RenderCompleteCallback = "gridRenderComplete";
        
        UserStoreSelect.Visible = UserManager.HasEveryoneGroupAccess(UserManager.GetCurrentPrincipal());

        if (AuthorizationFactory.GetAuthorizationProvider().Authorize(UserManager.GetCurrentPrincipal(), GroupManager.GetEveryoneGroup(), "Group.ManageUsers"))
        {
            // we only initialize the button with appropriate values if the current user
            // is authorized to edit the everyone group's security
            _everyoneDefaultSecurityBtn.Visible = true;
            _everyoneDefaultSecurityBtn.HRef = "javascript:showDialog('Groups/Everyone.aspx?g=1', 'security');";
            _everyoneDefaultSecurityBtn.Title =
                WebTextManager.GetText("/pageText/users/manage.aspx/manageEveryoneTooltip");
        }
    }


    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
           "svcUserManagement.js",
           ResolveUrl("~/Services/js/svcUserManagement.js"));
    }
</script>
