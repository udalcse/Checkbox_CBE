<%@ Page Title="" Language="C#" MasterPageFile="~/DetailList.master" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Users.Manage" IncludeJsLocalization="true" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Register TagPrefix="ckbx" TagName="UserList" Src="~/Users/Controls/UserList.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="GroupList" Src="~/Users/Controls/GroupList.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="EmailListList" Src="~/Users/Controls/EmailListList.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="GroupDashboard" Src="~/Users/Controls/GroupDashboard.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserDashboard" Src="~/Users/Controls/UserDashboard.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="EmailListDashboard" Src="~/Users/Controls/EmailListDashboard.ascx" %>
<%@ Register Src="~/Forms/Controls/Timeline.ascx" TagName="Timeline" TagPrefix="ckbx" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="_head" ContentPlaceHolderID="_head" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/users/manage.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../Resources/StatusControl.js" />
    
    <script type="text/javascript">
        var _lastSearchTerm = '<%=Term%>';
        var _period = 0;
        var _eventName = '';
        var _selected = '<%=Selected%>';

        <%-- Ensure statusControl initialized--%>
        $(document).ready(function(){
            statusControl.initialize('_statusPanel');
            $('#<%=_userTabMenu.ClientID %>').val('<%=VisibleUserGrid %>');
            
            <%if (!string.IsNullOrEmpty(Term)) {%>
            //hide the search field
            //$('#leftPanel_search').hide();
            <%} %>

            $(this).on('searchExecuted', function(e, searchTerm, filterItemType, period, eventName) {
                if(filterItemType == 'user') {                    
                    if (typeof(eventName) != 'undefined' && typeof(period) != 'undefined') {
                        _lastSearchTerm = escapeInjections(searchTerm);
                        _period = period;
                        _eventName = eventName;
                        clearAndReload();
                    } else if (_lastSearchTerm != searchTerm) {
                        _period = 0;
                        _eventName = '';
                        _lastSearchTerm = escapeInjections(searchTerm);
                    } 
                }
            });

            if (_selected)
            {
                if (currentType == 'u')
                {
                    loadUserData(_selected);
                    $('#user_' + escString(_selected)).addClass('gridActive');
                }                    
                else if (currentType == 'g')
                {
                    loadGroupData(_selected);
                    $('#group_' + _selected).addClass('gridActive');
                }
                else if (currentType == 'e')
                {
                    $('#emailList_' + _selected).addClass('gridActive');
                    loadEmailListPanelData(_selected);
                }
            }
        });

        //Load user dashboard
        function onUserSelected(user) {
            loadUserData(user.UniqueIdentifier);
            $('.gridContent').removeClass('gridActive');
            $('#user_' + escString(user.UniqueIdentifier)).addClass('gridActive');
        }

        //Load group dashboard
        function onGroupSelected(group) {
            //Load group
            if (group)
            {
                loadGroupData(group.DatabaseId);
                $('.gridContent').removeClass('gridActive');
                $('#group_' + group.DatabaseId).addClass('gridActive');
            }
       }

        //Load email list dashboard
        function onEmailListSelected(emailList) {
            loadEmailListPanelData(emailList.DatabaseId);
            $('.gridContent').removeClass('gridActive');
            $('#emailList_' + emailList.DatabaseId).addClass('gridActive');
        }

        <%-- Show status message --%>
        function showStatusMessage(message, isSucceeded){
            if (isSucceeded)
                statusControl.showStatusMessage(message, StatusMessageType.success);
            else if (isSucceeded == false)
                statusControl.showStatusMessage(message, StatusMessageType.error);
            else
                statusControl.showStatusMessage(message, StatusMessageType.warning);
        }

        //Handler for OnUserDeleted event
        function userDeletedHandler(){
            //call the methods exposed by UserDashboard control
            cleanUserDashboard();
        }

        //Handler for OnGroupDeleted event
        function groupDeletedHandler(){
            //call the methods exposed by GroupDashboard control
            cleanGroupDashboard();
        }

        //Handler for EmailListDeleted event
        function emailListDeletedHandler(){
            //call the methods exposed by EmailListDashboard control
            cleanEmailListDashboard();
        }

        //Handler for updating userList
        function userListUpdateHandler(){
            //call the methods exposed by UserList control
            reloadUserList();
        }

        //Handler for updating groupList
        function groupListUpdateHandler(){
            //call the methods exposed by GroupList control
            reloadGroupList();
        }

        //Handler for updating emailList
        function emailListUpdateHandler(){
            //call the methods exposed by EmailList control
            reloadEmailListList();
        }

        //reload all
        function clearAndReload()
        {
            if (typeof(reloadUserList) == 'function')
            {
                reloadUserList();
            }
            if (typeof(reloadGroupList) == 'function')
            {
                reloadGroupList();
            }
            if (typeof(reloadEmailListList) == 'function')
            {
                reloadEmailListList();
            }
        }

        var currentType = '<%=VisibleUserGrid.ToLower()%>';

        function loadGrid(gridType) {
            if (currentType == gridType && gridType == 'u')
            {
                changeUserMembershipProvider();
                return;
            }
            document.location = "Manage.aspx?m=" + gridType;
        }

        function escString(string) {
            return string.replace(/([ #;&,.+*~\':"!^$[\]()=>|\/@])/g,'\\$1');
        }

        function timelineLoaded(){
            $('.introPage').hide();
        }
    </script>
</asp:Content>

<asp:Content ID="_title" runat="server" ContentPlaceHolderID="_titlePlace">
    
</asp:Content>

<asp:Content ID="_titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <ul id="_userTabMenu" runat="server" class="users-manage-tabs tabsMenu"></ul>
    <div class="users-manage-buttons">
        <a id="_buttonAddUser" runat="server" class="header-button ckbxButton blueButton" href="javascript:showDialog('Add.aspx', 'wizard');"><%= WebTextManager.GetText("/pageText/Users/Manage.aspx/addUsers+")%></a>
        <div id="usermanager_adduser_menu" class="groupMenu" style="display: none;">              
            <ul class="allMenu">
                <%--<li><a class="ckbxButton orangeButton" runat="server" id="_buttonAddUser" href=""><%= WebTextManager.GetText("/pageText/Users/Manage.aspx/addNewUser")%></a></li>--%>
                <li><a class="ckbxButton orangeButton" runat="server" id="_buttonImportUser" href="javascript:showDialog('Import.aspx', 'wizard');"><%=WebTextManager.GetText("/pageText/Users/Manage.aspx/importUsers")%></a></li>
            </ul>
        </div>
        <a class="header-button ckbxButton blueButton" runat="server" id="_buttonAddGroup" href="javascript:showDialog('Groups/Add.aspx', 'largeProperties');"><%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/addNewgroup")%></a>
        <a class="header-button ckbxButton blueButton" runat="server" id="_buttonAddMailList" href="javascript:showDialog('EmailLists/Add.aspx', 'largeProperties');"><%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/newEmailList")%></a>
    </div>
</asp:Content>

<asp:Content ID="_left" ContentPlaceHolderID="_leftContent" runat="server">
    <ckbx:UserList ID="_userList" runat="server" UserSelectedClientCallback="onUserSelected" OnUserDeleted="userDeletedHandler" ShowStatusMessageHandler="showStatusMessage"/>
    <ckbx:GroupList ID="_groupList" runat="server" GroupSelectedClientCallback="onGroupSelected" OnGroupDeleted="groupDeletedHandler" ShowStatusMessageHandler="showStatusMessage"/>
    <ckbx:EmailListList ID="_emailListList" runat="server" EmailListSelectedClientCallback="onEmailListSelected" OnEmailListDeleted="emailListDeletedHandler" ShowStatusMessageHandler="showStatusMessage"/>
</asp:Content>

<asp:Content ID="_right" ContentPlaceHolderID="_rightContent" runat="server">
  <div id="detailProgressContainer" style="display:none;">
        <div id="detailProgress" style="text-align:center;">
            <p><%=WebTextManager.GetText("/common/loading")%></p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
            </p>
        </div>
    </div>
    <div id="itemContainer">
        <ckbx:GroupDashboard ID="_groupDashboard" runat="server" ShowStatusMessageHandler="showStatusMessage" GroupListUpdateHandler="groupListUpdateHandler"/>
        <ckbx:UserDashboard ID="_userDashboard" runat="server" ShowStatusMessageHandler="showStatusMessage" UserListUpdateHandler="userListUpdateHandler"/>
        <ckbx:EmailListDashboard ID="_emailListDashboard" runat="server" ShowStatusMessageHandler="showStatusMessage" EmailListUpdateHandler="emailListUpdateHandler"/>
        <ckbx:Timeline ID="_timeline" runat="server" Manager="UserManager" OnClientLoad="timelineLoaded" />
    </div>
</asp:Content>