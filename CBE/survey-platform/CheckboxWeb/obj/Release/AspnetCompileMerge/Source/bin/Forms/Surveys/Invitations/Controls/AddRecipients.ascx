<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddRecipients.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.AddRecipients" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserStoreSelect" Src="~/Users/Controls/UserStoreSelect.ascx" %>

<script type="text/javascript">
    var _emailAddressList = new Array();
    var _invalidEmailAddressList = new Array();
    var _currentAddressIndex = -1;
    var _addRecipientSearchTerm = '';
    var _selectedUsers = new Array();
    var _liveSearchAddRecipients;
    var <%= ClientID %>_provider = '<%= MembershipProviderManager.FirstAvailableProvider.Name %>';
    var <%= ClientID %>_providerType = '<%= MembershipProviderManager.FirstAvailableProvider.GetType().Name %>';

    //
    $(document).ready(function () {        
        <% if(!PendingInvitationMode) { %>
            _liveSearchAddRecipients = new LiveSearchObj('innerSearchDiv');
            
            $('#addRecipientTabs').ckbxTabs({ 
                tabName: 'addRecipientTabs',
                initialTabIndex: <%= UsersOnly ? '1' : '0' %>,
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();},
                onTabClick:function(idx){
                    if (idx == 0) {
                        $("#searchDiv").hide();
                    } else {
                        $("#searchDiv").show();
                        _liveSearchAddRecipients.changeFilterName('searchTerm' + idx);
                        _liveSearchAddRecipients.doSearch();
                    }
                }
            });
        <% } %>

        $('.dialogFormContainer').on('membershipProviderChanged', function(e, eventId, membershipName, type) {
            if(eventId == 'recipientsList') {
                <%= ClientID %>_provider = membershipName;
                <%= ClientID %>_providerType = type;
                reloadGrids();
            }
        });
        
        <% if(!string.IsNullOrEmpty(ViewRecipientListCallback)){ %>
            $('#pendingRecipientsLink').unbind('click').bind('click', <%=ViewRecipientListCallback %>);
        <%} %>
        
        $('#addRecipientsLink').unbind('click').bind('click', onAddRecipientsClick);

        $('#genereateUsersLinks').unbind('click').bind('click', onGenerateLinks);

        //implementation all/none selection
        $(document).on('click', '#selectAllUsers', function() {
            if ($(this).prop('checked')) {
                $.each($('.addUser'), function() {
                    userSelected($(this).val(), true);
                    $(this).prop('checked', true);
                    $.uniform.update($(this));
                });
            } else {
                $.each($('.addUser'), function() {
                    userSelected($(this).val(), false);
                    $(this).prop('checked', false);
                    $.uniform.update($(this));
                });
            }
        });
        
        $(document).on('click', '#selectAllGroups', function() {
            if ($(this).prop('checked')) {
                $.each($('.addGroup'), function() {
                    $(this).prop('checked', true);
                    $.uniform.update($(this));
                });
            } else {
                $.each($('.addGroup'), function() {
                    $(this).prop('checked', false);
                    $.uniform.update($(this));
                });
            }
        });
        
        $(document).on('click', '#selectAllEmailLists', function() {
            if ($(this).prop('checked')) {
                $.each($('.addEmailList'), function() {
                    $(this).prop('checked', true);
                    $.uniform.update($(this));
                });
            } else {
                $.each($('.addEmailList'), function() {
                    $(this).prop('checked', false);
                    $.uniform.update($(this));
                });
            }
        });
        
        //Select all recipients
        $(document).on('click', '#_selectAllRecipients', function(){
            if ($(this).prop('checked'))
                $('.selectRecipient').prop('checked', true);
            else
                $('.selectRecipient').prop('checked', false);
            $.uniform.update('.selectRecipient');
        });

        $('.custom-select-container').css('margin', '5px');
    });

    //
    function userSelected(identifier, add) {
        var index = $.inArray(identifier, _selectedUsers);
        if(add) {
            if(index < 0)
                _selectedUsers.push(identifier.replace("'", "&#39;"));
        } else if(index >= 0) {
            _selectedUsers.splice(index, 1);
        }
    }

    //
    function isUserAlreadySelected(identifier) {
        return $.inArray(identifier, _selectedUsers) > -1;
    }
    
    //
    function reloadGrids() {
        <%=_addUsersGrid.ReloadGridHandler%>();
        <%=_addGroupsGrid.ReloadGridHandler%>();
        <%=_addEmailLists.ReloadGridHandler%>();
    }

    //
    function loadAddRecipientUsersList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterValue ){
        <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
            //if provider type is AD and user preloading is disabled, do nothing
            if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType && !filterValue) {
                    <%= _addUsersGrid.ClearGridHandler %>();
                    return;
                }
        <% } %> 
        
        <% if (UsersOnly) { %>
            svcUserManagement.getTenantUsers(
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
        return;
        <% } %>
           
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

            svcInvitationManagement.listAvailablePageItemUserDataForInvitation(
                _at, 
                <%= ClientID %>_provider,
                <%=InvitationId %>,
                {
                    pageNumber: currentPage,
                    pageSize: <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    sortField: sortField,
                    sortAscending: sortAscending,
                    filterField:'UniqueIdentifier',
                    filterValue: filterValue
                },
                loadCompleteCallback,
                loadCompleteArgs
           );
    }

    //
    function loadAddRecipientGroupsList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterValue ){

        <% if (UsersOnly) { %>
        return;
        <% } %>

        svcInvitationManagement.listAvailableUserGroupsForInvitation(
            _at, 
            <%=InvitationId %>,
            {
                pageNumber: currentPage,
                pageSize: <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                sortField: sortField,
                sortAscending: sortAscending,
                filterField:'GroupName',
                filterValue: filterValue
            },
            loadCompleteCallback,
            loadCompleteArgs
        ); 
       
    }

     //
    function loadAddRecipientEmailLists(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterValue  ){

        <% if (UsersOnly) { %>
        return;
        <% } %>

        svcInvitationManagement.listAvailableEmailListsForInvitation(
            _at, 
            <%=InvitationId %>,
            {
                pageNumber: currentPage,
                pageSize: <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                sortField: sortField,
                sortAscending: sortAscending,
                filterField:'Name',
                filterValue:filterValue
            },
            loadCompleteCallback,
            loadCompleteArgs
        ); 

  
    }

    //
    function onAddRecipientsClick(){
        var isPrepMode = "<%=IsPrepMode%>" ;

        if (isPrepMode !== "True") {
            addSelectedUsers();
            addSelectedGroups();
            addSelectedEmailLists();
            addSelectedEmailAddresses(); //add free form email addresses last in order to display invalid warning
        } else {
            addSelectedUsers();
        }

    }

    function onGenerateLinks(){
        var selectedUsersArray = _selectedUsers;
        if (selectedUsersArray.length === 0) {
            return;
        }

        var userList = [];

        for (var i = 0; i < selectedUsersArray.length; i++) {
            var values = [];

            userList.push(JSON.stringify(
            {
                'UserName': selectedUsersArray[i]
            }));
        }

        if (userList.length > 0) {
            svcInvitationManagement.generateUsersLinks(
                _at,
                userList,
                <%=SurveyId %>,
                function() {
                    statusControl.initialize('addRecipientsStatus');
                    statusControl.showStatusMessage(
                        '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addUserSuccess") %>'.replace('{0}', selectedUsersArray.length),
                        StatusMessageType.success);
                    _selectedUsers.length = 0;

                    <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
                    //if provider type is AD and user preloading is disabled, do nothing
                    if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType) {
                        <%= _addUsersGrid.ClearGridHandler %>();
                    } else {
                        <%=_addUsersGrid.ReloadGridHandler %>();
                    }
                    <% } else { %>
                    <%=_addUsersGrid.ReloadGridHandler %>();
                    <% } %>

                    <% if (!string.IsNullOrEmpty(OnRecipientsAdded))
                       {%>
                    <%=OnRecipientsAdded %>();
                    <%
                       }%>
                }
            );
        }
    }

    Array.prototype.removeEmptyAddresses = function() {
        for (var i = 0; i < this.length; i++) {
            if (this[i] == "") {         
                this.splice(i, 1);
                i--;
            }
        }
        return this;
    };

    //
    function addSelectedEmailAddresses(){
        //_emailAddressList = addresses.split(';');
        _emailAddressList = $('#addEmailAddressesList').val().split( /\r\n|\r|\n|,|;/ ).removeEmptyAddresses();

        if(_emailAddressList.length == 0)
            return;

        _invalidEmailAddressList = new Array();
        _currentAddressIndex = 0;

        var baseProgressMessage = '<%= WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addEmailAddressProgress") %>';
        $('#addEmailAddressesProgressMessage').html(baseProgressMessage.replace('{0}', '0').replace('{1}', _emailAddressList.length));

        $('#addAddressesProgressContainer').show();
        $('#addAddressesProgress').progressbar({value:0});
        
        //Start process
        addNextEmailAddressBatch();
    }
    
    //
    function addNextEmailAddressBatch(){
        var completeCallback;

        //If completing on this batch, call onComplete handler when done, otherwise
        // call next batch add
        if(_emailAddressList.length - _currentAddressIndex > 20) {
            completeCallback = addNextEmailAddressBatch;
        }
        else{
            completeCallback = onAddEmailAddressesComplete;
        }

        var maxIndex = Math.min(_currentAddressIndex + 19, _emailAddressList.length - 1);

        var addressList = new Array();

        while(_currentAddressIndex <= maxIndex){
            
            if(validateEmailAddress(_emailAddressList[_currentAddressIndex])){
                addressList.push(_emailAddressList[_currentAddressIndex]);
            }
            else{
                _invalidEmailAddressList.push(_emailAddressList[_currentAddressIndex]);
            }

            _currentAddressIndex++;
        }

        //Add to invitation
        if(addressList.length > 0){
             svcInvitationManagement.addEmailAddressesToInvitation(
                _at, 
                <%=InvitationId %>, 
                addressList,
                completeCallback);
        }else {
            if(completeCallback != null) {
                completeCallback();
            }
        }

        //Update message & progress
        var baseProgressMessage = '<%= WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addEmailAddressProgress") %>';
        $('#addEmailAddressesProgressMessage').html(baseProgressMessage.replace('{0}', _currentAddressIndex).replace('{1}', _emailAddressList.length));

        var percent = (((_currentAddressIndex + 1) / _emailAddressList.length) * 100) | 0;
        $('#addAddressesProgress').progressbar({value:percent});
    }

    //
    function onAddEmailAddressesComplete(){
        //Hide progress
        $('#addAddressesProgressContainer').hide();

        //Show status 
        statusControl.initialize('addRecipientsStatus');
        
        if(_invalidEmailAddressList.length == 0){
            //clear addresses
            $('#addEmailAddressesList').val('');

            statusControl.showStatusMessage(
                '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addAddressSuccess") %>'.replace('{0}', _emailAddressList.length),
                StatusMessageType.success);
        }
        else{
            //replace addresses with invalid addresses
            $('#addEmailAddressesList').val(_invalidEmailAddressList.join('\n'));

            statusControl.showStatusMessage(
                '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addAddressSuccessWithInvalid") %>'.replace('{0}', _emailAddressList.length - _invalidEmailAddressList.length).replace('{1}', _invalidEmailAddressList.length),
                StatusMessageType.warning);
        }

        <% if (!String.IsNullOrEmpty(OnRecipientsAdded))
            {%>
            <%=OnRecipientsAdded %>();
        <%
            }%>
    }
    
    //
    function validateEmailAddress(address){
        //Perform validation to ensure address has basic form of foo@bar.xyz
        return ((address.indexOf('@') > 0) 
                &&  (address.lastIndexOf('.') > address.indexOf('@'))
                &&  (address.length - address.lastIndexOf('.') >= 2));
    }

   
    //
    function addSelectedEmailLists(){
        var selectedEmailListsArray = getSelectedArray('addEmailList');
        
        if(selectedEmailListsArray.length == 0){
            return;
        }

        svcInvitationManagement.addEmailListsToInvitation(
            _at, 
            <%=InvitationId %>, 
            selectedEmailListsArray,
            function(){
                statusControl.initialize('addRecipientsStatus');
                statusControl.showStatusMessage(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addListSuccess") %>'.replace('{0}', selectedEmailListsArray.length),
                    StatusMessageType.success);
                    <%=_addEmailLists.ReloadGridHandler %>();

                <% if (!String.IsNullOrEmpty(OnRecipientsAdded))
                   {%>
                   <%=OnRecipientsAdded %>();
                <%
                   }%>
            }
        );
    }


      //
    function addSelectedGroups(){
        var selectedGroupsArray = getSelectedArray('addGroup');

        if(selectedGroupsArray.length == 0){
            return;
        }

        svcInvitationManagement.addGroupsToInvitation(
            _at, 
            <%=InvitationId %>, 
            selectedGroupsArray,
            function(){
                statusControl.initialize('addRecipientsStatus');
                statusControl.showStatusMessage(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addGroupSuccess") %>'.replace('{0}', selectedGroupsArray.length),
                    StatusMessageType.success);
                    <%=_addGroupsGrid.ReloadGridHandler %>();

                <% if (!String.IsNullOrEmpty(OnRecipientsAdded))
                   {%>
                   <%=OnRecipientsAdded %>();
                <%
                   }%>
            }
        );
    }

    //
    function addSelectedUsers() {
        var selectedUsersArray = _selectedUsers;
     
        if (selectedUsersArray.length === 0) {
            return;
        }

        var userList = [];
      

        for (var i = 0; i < selectedUsersArray.length; i++) {
            var values = [];

            var emailSourceElement;

            //if it is drop down list , take values from it otherwise use primary email 
            if ($("[name='initation-dd-" + selectedUsersArray[i] + "']").length) {
                emailSourceElement = $("[name='initation-dd-" + selectedUsersArray[i] + "']");
                values= $(emailSourceElement).val();
            } else {
                emailSourceElement = $('*[data-primary-email-username="' + selectedUsersArray[i] + '"]');
                values.push($(emailSourceElement).text());
            }

            if (values == null) {
                alert("Please select any email for " + selectedUsersArray[i]);
                continue;;;
            }

            userList.push(JSON.stringify(
            {
                'UserName': selectedUsersArray[i],
                'Emails': values
            }));
        }

        if (userList.length > 0) {
            
            svcInvitationManagement.addUsersToInvitation(
                _at,
                <%=InvitationId %>,
                userList,
                function() {
                    statusControl.initialize('addRecipientsStatus');
                    statusControl.showStatusMessage(
                        '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addUserSuccess") %>'.replace('{0}', selectedUsersArray.length),
                        StatusMessageType.success);
                    _selectedUsers.length = 0;

                    <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
                    //if provider type is AD and user preloading is disabled, do nothing
                    if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType) {
                        <%= _addUsersGrid.ClearGridHandler %>();
                    } else {
                        <%=_addUsersGrid.ReloadGridHandler %>();
                    }
                    <% } else { %>
                    <%=_addUsersGrid.ReloadGridHandler %>();
                    <% } %>

                    <% if (!string.IsNullOrEmpty(OnRecipientsAdded))
                       {%>
                    <%=OnRecipientsAdded %>();
                    <%
                       }%>
                }
            );
        }
    }

    //
    function getSelectedArray(className){
        var selectedArray = new Array();

        $('.' + className + ':checked').each(function(index){
            selectedArray.push($(this).attr('value'));
        });

        return selectedArray;
    }
</script>

<%if(!PendingInvitationMode){ %>
    <ul id="addRecipientTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/addEmail")%></li>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/addUsers")%></li>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/addGroups")%></li>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/addEmailLists")%></li>
    </ul>
<%} %>

<div class="tabContentContainer<%= (!PendingInvitationMode) ? " recipient-list-wrapper" : "" %>">
    <%if (!PendingInvitationMode) { %>
        <div class="dialogInstructions">
            <%  if (!UsersOnly)
              { %>
                <%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/filterTitle")%>
            <% } else { %>
              <%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/userOnlyFilterTitle") %>
            <% } %>
        </div>
    <% } %>

    <div id="searchDiv" class="gridMenu clearfix" style="display:none;">
        <div class="groupMembersFilter right" id="innerSearchDiv">
            <asp:TextBox runat="server" Width="150px" autocomplete="off" />
            <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
            <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip" />
        </div>
    </div>

    <div class="">
        <div id="addRecipientsStatus"></div>
        
        <%if(!IsPrepMode){ %>
        <div id="addRecipientTabs-0-tabContent">
            <div id="addAddressesProgressContainer" class="padding10 border999 shadow999" style="position:absolute;text-align:center;height:180px;width:350px;display:none;background-color:#e8e2dc;">
                <div id="addAddressesProgress"></div>
                <div style="font-size:10px;" id="addEmailAddressesProgressMessage"></div>
                <div style="margin-top:15px;font-weight:bold;font-size:12px;">
                    <%= WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addingEmailAddresses") %>
                </div>
            </div>
            <div class="padding10">
                <div class="dialogInstructions">
                    <%= WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/emailCoachingText") %>
                </div>
                <div>
                    <textarea rows="15" cols="60" id="addEmailAddressesList"></textarea>
                </div>
            </div>
        </div>
        <%} %>
        <div id="addRecipientTabs-1-tabContent">
            <ckbx:UserStoreSelect EventID="recipientsList" ID="UserStoreSelect" runat="server" />
            <div style="min-width: 850px; display:inline-block;">
                <ckbx:Grid ID="_addUsersGrid" runat="server" GridCssClass="ckbxAddInvitationRecipientsGrid" />
            </div>
        </div>
        <div id="addRecipientTabs-2-tabContent">
            <ckbx:Grid ID="_addGroupsGrid" runat="server" GridCssClass="ckbxAddInvitationRecipientsGrid" />
        </div>
        <div id="addRecipientTabs-3-tabContent">
            <ckbx:Grid ID="_addEmailLists" runat="server" GridCssClass="ckbxAddInvitationRecipientsGrid" />
        </div>
    </div>
</div>

<%if (PendingInvitationMode)
  { %>
    <div class="left" style="margin-left:10px;">
        <a class="ckbxButton silverButton inactive" id="pendingRecipientsLink" href="javascript:void(0);"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/backToRecipientList")%></a>
    </div>
<% } %>
<% if (UsersOnly)
    { %>
<div class="left" style="margin-left:10px;">
    <a class="ckbxButton silverButton" id="genereateUsersLinks" href="javascript:void(0);"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/generateLinks")%></a>
</div>
<% } else
    { %>
    <div class="left" style="margin-left:10px;">
        <a class="ckbxButton silverButton" id="addRecipientsLink" href="javascript:void(0);"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/addSelected")%></a>
    </div>
<% } %>

<br class="clear" />
