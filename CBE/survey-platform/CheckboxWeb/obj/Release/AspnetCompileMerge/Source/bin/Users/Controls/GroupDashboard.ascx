<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GroupDashboard.ascx.cs"
    Inherits="CheckboxWeb.Users.Controls.GroupDashboard" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<script type="text/javascript">
	var _dashGroupId = null;

    function deleteAllGroupMembersFromCheckbox()
    {
        svcUserManagement.deleteAllGroupMembersFromCheckBox(
            _at,                                                
            _dashGroupId,
            function(result){
                loadGroupData(_dashGroupId, true);
                <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
                {%>
                var message = '';                            
                if(result.length>0)
                {
                    message='<%=WebTextManager.GetText("/users/groupDashboardTemplate/deletedPartially") %> '+result;                            
                }
                else
                {
                    message='<%=WebTextManager.GetText("/users/groupDashboardTemplate/deletedAllGroupMembersFromCheckbox") %>'
                }
                var messageColorValue = result.length > 0 ? null:true;
                <%=ShowStatusMessageHandler %>(message, messageColorValue);
            <%
                }%>

                <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
                    {%>
                    <%=GroupListUpdateHandler %>();
                <%
                    }%>
            }
        );
    }

    <%-- Ensure services initialized --%>
	$(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/groupDashboardTemplate.html") %>', 'groupDashboardTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/groupMemberListTemplate.html") %>', 'groupMemberListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/groupMemberListItemTemplate.html") %>', 'groupMemberListItemTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/groupListItemTemplate.html") %>', 'groupListItemTemplate.html');

        //Prevent parent handler from being notified of 'click' event when user click checkbox.
        $(document).on('click','.deleteMember',function(event){
            event.stopPropagation();
        });

        //Select all/none implementation
        $(document).on('click','#_deleteSelectedMembersLink',function(){
               var membersArray = new Array();

                $('.deleteMember:checked').each(function(){
                    membersArray.push($(this).attr('value').replace(/'/g, "&#39;"));
                });

                if (membersArray.length > 0){
                    svcUserManagement.removeUsersFromGroup(
                        _at,                        
                        membersArray,
                        _dashGroupId,
                        function(){
                            loadGroupData(_dashGroupId, true);
                            <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
                            {%>
                            var message = '<%=WebTextManager.GetText("/pageText/users/groups/members.aspx/usersDeleted") %>';
                            <%=ShowStatusMessageHandler %>(message.replace('{0}', membersArray.length), true);
                        <%
                            }%>

                            <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
                              {%>
                              <%=GroupListUpdateHandler %>();
                            <%
                              }%>
                        }
                    );
                }
        });

        $(document).on('click', '#_removeAllUsersFromGroup', function(){
            showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/users/groupDashboardTemplate/removeAllUsersFromGroupConfirmation") %>', 
                onRemoveAllUsersFromGroupConfirm,
                337,
                200,
                '<%=WebTextManager.GetText("/users/groupDashboardTemplate/removeAllUsersFromGroupConfirmTitle") %>'
            );
        });

        $(document).on('click', '#_deleteAllGroupMembersFromCheckBox', function() {
            showConfirmDialogWithCallback(
                '<%=WebTextManager.GetText("/users/groupDashboardTemplate/deleteAllGroupMembersFromCheckboxConfirmation") %>', 
                deleteAllGroupMembersFromCheckbox,
                337,
                250,
                '<%=WebTextManager.GetText("/users/groupDashboardTemplate/deleteAllGroupMembersFromCheckboxConfirmTitle") %>'
            );
        });

        $(document).on('click', '#_selectAllMembers', function(){
            if ($(this).prop('checked'))
                $('.deleteMember').prop('checked', true);
            else
                $('.deleteMember').prop('checked', false);
            $.uniform.update('.deleteMember');
        });
	});

    function onRemoveAllUsersFromGroupConfirm() {
        svcUserManagement.removeAllUsersFromGroup(
            _at,                                                
            _dashGroupId,
            function(){
                loadGroupData(_dashGroupId, true);
                <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
                {%>
                var message = '<%=WebTextManager.GetText("/users/groupDashboardTemplate/deletedAllGroupMembersFromCheckbox") %>';
                <%=ShowStatusMessageHandler %>(message, true);
            <%
                }%>

                <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
                    {%>
                    <%=GroupListUpdateHandler %>();
                <%
                    }%>
            }
        );
    }

    <%-- Expose Method to Load Group --%>
    function loadGroupData(groupId, reloadListData){
    
        if(groupId == null || groupId == ''){
            return;
        }
        
        $('#infoPlace').empty();
        $('#infoPlace').hide();
        $('#detailProgressContainer_<%=ID %>').show();

        _dashGroupId = groupId;

        //Load group data - Makes use of jQuery deferred objects so we can see entire chain of 
        // calls in one place.
        svcUserManagement.getUserGroupByIdD(_at, groupId, {reloadListData:reloadListData, groupId: groupId, at: _at})
            .then(
                function(userGroupResultData, userGroupResultArgs) {
                    //check Group.Edit permission
                    svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_USERGROUP, userGroupResultArgs.groupId, 'Group.Edit')
                        .then(function(result) {
                            if(result == null) {
                                return;
                            }
                            userGroupResultArgs.allowEdit = result;
                            //check Group.ManageUsers permission
                            svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_USERGROUP, userGroupResultArgs.groupId, 'Group.ManageUsers')
                                .then(function(resultManage) {
                                    userGroupResultArgs.allowManageUsers = resultManage;
                                })
                                .then(
                                    function(args) {
                                        onGroupMetaDataLoaded(userGroupResultData, userGroupResultArgs);
                                        applyGroupDashTemplate(userGroupResultData, userGroupResultArgs)
                                            .then(
                                                function() {
                                                    loadGroupMemberList(userGroupResultData, userGroupResultArgs);
                                                }
                                            );
                                    }
                                );
                        })
                }                
            );
    }

    <%-- Authorize Group --%>
    function authorizeGroup(resultData, args){
        if(resultData == null){
            return;
        }
        return svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_USERGROUP, args.groupId, 'Group.Edit')
            .then(function(result) {
                args.allowEdit = result;
            });
    }

    <%-- Apply Dash Template --%>
    function applyGroupDashTemplate(resultData, args){
        resultData.allowEdit = args.allowEdit;
        resultData.allowManageUsers = args.allowManageUsers;
        return templateHelper.loadAndApplyTemplateD(
            'groupDashboardTemplate.html',  
            '<%=ResolveUrl("~/Users/jqtmpl/groupDashboardTemplate.html") %>',
            resultData,
            null,
            'infoPlace',
            true);
    }

    <%-- Reload Group List Item, if necessary --%>
    function reloadGroupListItemIfNecessary(resultData, args)
    {
        if(args.reloadListData) {
             //Pulsate
             $('#group_' + resultData.DatabaseId).effect(
                'shake', 
                {
                    times:2,
                    distance:10,
                    duration:250
                },
                function(){
                    //Required to remove jagged text left behind in IE for some reason
                    // when pulsate is called.
                    if (typeof(this.style.removeAttribute) != 'undefined' && this.style.removeAttribute != null)
                        this.style.removeAttribute('filter');
                    
                    var index = $('#groupContainer_' + resultData.DatabaseId).attr('index');
                    if (typeof(index)=='undefined' || index == null)
                        return;

                    //Apply template
                    templateHelper.loadAndApplyTemplate(
                        'groupListItemTemplate.html',
                        '<%=ResolveUrl("~/Users/jqtmpl/groupListItemTemplate.html") %>',
                        resultData,
                        {index: index},
                        'groupContainer_' + resultData.DatabaseId,
                        true)
                });
        }
    }

    var gridPage=-1;

    <%-- Load Group Member List --%>
    function loadGroupMemberList(resultData, args){
    
        if(gridPage==-1)
        {
            gridPage=1;
        }

        $('#groupMembersPlace').html($('#detailProgressContainer_<%=ID %>').html());

      svcUserManagement.listUserGroupMembersD(
            _at,
            resultData.DatabaseId,
            {
                pageSize : <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageNumber : gridPage,
                sortField : '',
                sortAscending : true,
                filterField : '',
                filterValue : ''
            },
            args
        ).then(onGroupMemberListLoaded);
    }


	<%-- Apply template to loaded metadata and then apply child templates --%>
	function onGroupMetaDataLoaded(resultData, args) {
		if (resultData == null) {
			$('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load information about group with id: ' + args.groupId + '.</div>');
            return;
		}
	}

    <%-- Copy the group --%>
    function copyGroup(groupID)
    {
        if(groupID == null || groupID == ''){
            return;
        }
        <%-- Start copying the group --%>
        svcUserManagement.copyUserGroup(_at, groupID, '<%=WebTextManager.GetUserLanguage() %>', onGroupCopied, {at: _at});
     }
	
	<%-- refresh a group list --%>
	function onGroupCopied(resultData, args) {
        reloadGroupList();
    }

    function onGroupMemberListRendered()
    {
        $('select, input:checkbox, input:radio, input:text').filter(':not([uniformIgnore])').uniform();
    }


    //Member list loaded
    function onGroupMemberListLoaded(resultData, args){
        var itemCount = resultData.TotalItemCount;

        //The dashboard should be shown before rendering the grid, because 
        //at the end of the rendering 'resizePanels' function is invoked. An this function requires
        //the dashboard to be shown.
        $('#detailProgressContainer_<%=ID %>').hide();
        $('#infoPlace').show();

      //Render grid
        gridHelper.renderGrid(
            resultData.ResultPage, 
            'groupMembersPlace', 
            'groupMemberListTemplate.html',
            '<%=ResolveUrl("~/Users/jqtmpl/groupMemberListTemplate.html") %>',
             {itemClick: onGroupMemberClick, allowEdit: args.allowEdit, allowManageUsers: args.allowManageUsers},
             onGroupMemberListRendered
        );
        
        //Hide pager if not necessary
        if(itemCount == 0
            || itemCount <  <%= ApplicationManager.AppSettings.PagingResultsPerPage %>){
            $('#gridPaginationTopContainer').hide();
            $('#gridPaginationBottomContainer').hide();
        }
        else{
            $('#gridPaginationTopContainer').show();
            $('#gridPaginationBottomContainer').show();

            $('#paginationTop').pager({
                totalItems:itemCount,
                currentPage: gridPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageChanged: function(newPage){
                    
                    gridHelper.currentPage = newPage; 
                    gridPage = newPage;

                    loadGroupData(_dashGroupId, true);

                }
            });

             $('#paginationBottom').pager({
                totalItems:itemCount,
                currentPage: gridPage,
                pageSize:  <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageChanged: function(newPage){

                    gridHelper.currentPage = newPage; 
                    gridPage = newPage;

                    loadGroupData(_dashGroupId, true);

                }
            });
        }
    }

    //
    function onGroupMemberClick(groupMember){
        if(groupMember.AuthenticationType.toLowerCase() == 'checkboxpassworduser'){
            showDialog('Properties.aspx?u=' + groupMember.UserGuid + '&onClose=onDialogClosed', '_userPropertiesWindow');
        }
        else{
           showDialog('Properties.aspx?e=' + groupMember.UniqueIdentifier + '&onClose=onDialogClosed', '_userPropertiesWindow');
        }
    }

    //Determine if the window is a confirm window
    function checkConfirmDialog(window){
        var re = new RegExp("^confirm");
        return re.test(window.get_name());
    }

    //Handle dialog close and reload group dashboard
    function onDialogClosed(args){
        if(args == null || args == 'cancel'){
            return;
        }
        var reloadListData = true;

        if (args.page=='addGroup'){
            _dashGroupId = args.newGroupId;
            <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
              {%>
              <%=GroupListUpdateHandler %>();
            <%
              }%>
            reloadListData = false;
        }

        if (args.page=='properties'){
            _dashGroupId = args.groupId;
            <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
              {%>
              <%=GroupListUpdateHandler %>();
            <%
              }%>
        }

        if (args.page=='addUsers'){
            <%if(!String.IsNullOrEmpty(GroupListUpdateHandler))
              {%>
              <%=GroupListUpdateHandler %>();
            <%
              }%>
        }

        if(args == 'userUpdated'){
            statusControl.showStatusMessage(
                '<%=WebTextManager.GetText("/users/groupDashboardTemplate/userUpdated") %>',
                StatusMessageType.success);
        }

        //Reload dash
        loadGroupData(_dashGroupId, reloadListData);
    }

    //Clean group dashboard
    function cleanGroupDashboard(){
        $('#detailProgressContainer_<%=ID %>').hide();
        $('#infoPlace').empty();
    }
</script>
<%-- Container for Results --%>
<div id="groupDashStatus">
</div>
<div id="detailProgressContainer_<%=ID %>" style="display: none;">
    <div id="detailProgress_<%=ID %>" style="text-align: center;">
        <p>
            <%=WebTextManager.GetText("/common/loading")%></p>
        <p>
            <asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner" />
        </p>
    </div>
</div>
<div id="infoPlace">
</div>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Get/set handler for groupList updating.
    /// </summary>
    public string GroupListUpdateHandler { get; set; }

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

        RegisterClientScriptInclude(
        "svcAuthorization.js",
        ResolveUrl("~/Services/js/svcAuthorization.js"));

        RegisterClientScriptInclude(
            "templateHelper.js",
            ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
            "securityHelper.js",
            ResolveUrl("~/Resources/securityHelper.js"));

        RegisterClientScriptInclude(
          "jquery.ckbxprotect.js",
          ResolveUrl("~/Resources/jquery.ckbxprotect.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        //Status control
        RegisterClientScriptInclude(
            "statusControl.js",
            ResolveUrl("~/Resources/statusControl.js"));

        RegisterClientScriptInclude(
            "jquery.pager.js",
            ResolveUrl("~/Resources/jquery.pager.js"));

    }
</script>
