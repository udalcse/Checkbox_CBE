<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="AddMembers.aspx.cs" Inherits="CheckboxWeb.Users.Groups.AddMembers" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>
<%@ Register Src="~/Controls/Grid.ascx" TagPrefix="ckbx" TagName="Grid" %>
<%@ Register TagPrefix="ckbx" TagName="UserStoreSelect" Src="~/Users/Controls/UserStoreSelect.ascx" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <script language="javascript" type="text/javascript">
        var _liveSearch;
        var <%= ClientID %>_provider = '<%= MembershipProviderManager.FirstAvailableProvider.Name %>';
        var <%= ClientID %>_providerType = '<%= MembershipProviderManager.FirstAvailableProvider.GetType().Name %>';

        function ShowAlert(text) {
            return confirm(text); 
        }

        $(document).ready(function() {
            //Init status
            statusControl.initialize('statusDiv');

            $('#<%=_okBtn.ClientID %>').attr('href', '#');
            $('#<%=_cancelBtn.ClientID %>').attr('href', '#');

            //Add users
            $('#<%=_okBtn.ClientID %>').click(function(){
                addSelectedUsersToGroup();
            });

            //Close users
            $('#<%=_cancelBtn.ClientID %>').click(function(){
                closeWindow(window.top.onDialogClosed);
            });

            $(document).on('click', '#_selectAllUsers', function(){
                if ($(this).prop('checked')) {
                    $('.addUser').prop('checked', true);
                } else {
                    $('.addUser').prop('checked', false);
                }
                $.uniform.update('.addUser');
            });

            _liveSearch = new LiveSearchObj('notInGroupMembers', 'usersNotInGroup');

            $('#<%= _userStoreSelect.ClientID %>').on('membershipProviderChanged', function(e, eventId, membershipName, type) {
                if(eventId === 'addGroupMembers') {
                    <%= ClientID %>_provider = membershipName;
                    <%= ClientID %>_providerType = type;
                    //reset grid page
                    gridHelper_<%=_availableUsersGrid.ClientID %>.currentPage = 1;
                    reloadAvailableList();
                }
            });

        });
        
        //Reload user list
        function reloadAvailableList(){
            <%=_availableUsersGrid.ReloadGridHandler %>(true);
        }

        //Load users
        function <%=ClientID %>loadAvailableGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
            <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
                //if provider type is AD and user preloading is disabled, do nothing
                if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType && !filterValue) {
                        <%= _availableUsersGrid.ClearGridHandler %>();
                        return;
                    }
            <% } %>
            svcUserManagement.listPageItemUserDataForGroup(
                _at,
                <%= ClientID %>_provider,
                <%=Group.ID %>,
                {
                    pageNumber:currentPage,
                    pageSize:<%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    sortField: sortField,
                    sortAscending: sortAscending,
                    filterField: '',
                    filterValue: filterValue
                },
                loadCompleteCallback,
                loadCompleteArgs);
        }

        //Add users
        function addSelectedUsersToGroup(){
            var userArray = new Array();

            $('.addUser:checked').each(function(index){
                userArray.push($(this).attr('value').replace(/'/g, "&#39;"));
            });

            if (userArray.length > 0){
                 svcUserManagement.addUsersToGroup(
                    _at,
                    <%=Group.ID %>,
                    userArray,
                    function(){
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/pageText/Users/groups/addMembers.aspx/usersAdded") %>',
                            StatusMessageType.success);

                        //Reload grid
                        <%=_availableUsersGrid.ReloadGridHandler %>(false);
                    }
                );
            }
        }

    </script>
</asp:Content>
<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div id="statusDiv" style="display:none;"></div>
        
        <div class="centerContent" style="width:510px;">
            <div class="groupMembersFilter margin10">
                <div class="left">
                    <ckbx:UserStoreSelect EventID="addGroupMembers" ShowLabel="True" ID="_userStoreSelect" runat="server"  />                                
                </div>
                <div class="right" id="notInGroupMembers">
                    <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip" />
                </div>
                <br class="clear" />
            </div>

            <ckbx:Grid ID="_availableUsersGrid" runat="server" GridCssClass="ckbxAddGroupMembersGrid overflow-y" />
        </div>
    </div>

    <div class="WizardNavContainer">
        <hr />
        <btn:CheckboxButton runat="server" ID="_cancelBtn" TextId="/pageText/users/groups/addMembers.aspx/closeWindow" CausesValidation="false" CssClass="cancelButton left" />
        <btn:CheckboxButton runat="server" ID="_okBtn" TextId="/pageText/users/groups/addMembers.aspx/addSelected" CssClass="right ckbxButton roundedCorners border999 shadow999 silverButton" />
    </div>
</asp:Content> 


