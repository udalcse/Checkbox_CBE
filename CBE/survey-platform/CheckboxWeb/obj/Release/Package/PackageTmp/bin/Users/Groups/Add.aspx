<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Add.aspx.cs" Inherits="CheckboxWeb.Users.Groups.Add" %>
<%@ MasterType VirtualPath="~/Dialog.Master"%>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>
<%@ Register src="../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserStoreSelect" Src="~/Users/Controls/UserStoreSelect.ascx" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/gridLiveSearch.js" />
    
    <script language="javascript" type="text/javascript">
        var <%= ClientID %>_provider = '<%= MembershipProviderManager.FirstAvailableProvider.Name %>';
        var <%= ClientID %>_providerType = '<%= MembershipProviderManager.FirstAvailableProvider.GetType().Name %>';

        var currentGroupUsers = new Array();
        var _availableLiveSearch;
        var _currentLiveSearch;

        $(document).ready(function() {
            $('#<%=_groupName.ClientID%>').focus();

            _currentLiveSearch = new LiveSearchObj('currentMembers', 'current');
            _availableLiveSearch = new LiveSearchObj('availableMembers', 'available');

            $('#<%= _userStoreSelect.ClientID %>').on('membershipProviderChanged', function(e, eventId, membershipName, type) {
                if(eventId === 'gac') {
                    <%= ClientID %>_provider = membershipName;
                    <%= ClientID %>_providerType = type;
                    //reset grid page
                    gridHelper_<%=_availableGrid.ClientID %>.currentPage = 1;
                    reloadAvailableList();
                }
            });
        });
        
    //Reload user list
    function reloadAvailableList(){
        <%=_availableGrid.ReloadGridHandler %>(true);
    }

    //Load grid of available users
    function <%=ID %>loadAvailableGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
            //if provider type is AD and user preloading is disabled, do nothing
            if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType && !filterValue) {
                    <%= _availableGrid.ClearGridHandler %>();
                    return;
                }
        <% } %>
        svcUserManagement.getPotentialUsersForNewGroup(
            _at,
            <%= ClientID %>_provider,
            {
                pageNumber: currentPage,
                pageSize:<%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                sortField: sortField,
                sortAscending: sortAscending,
                filterField: filterField,
                filterValue: filterValue
            },
            loadCompleteCallback,
            loadCompleteArgs);
    }
    
    //Load grid of current group users
    function <%=ID %>loadCurrentGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        svcUserManagement.getCurrentUsersForNewGroup(
            _at,
            {
                    pageNumber:currentPage,
                    pageSize:<%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                    sortField: sortField,
                    sortAscending: sortAscending,
                    filterField: filterField,
                    filterValue: filterValue
                },
                loadCompleteCallback,
                loadCompleteArgs);
    }
    
    var gridsLoading = false;
    //
    function onAvailableItemSelect(availableUser) {
        //if user is already in _currentGrid
        if(availableUser.IsInList) {
            return;
        }
        if (gridsLoading)
        {
            setTimeout(function(){onAvailableItemSelect(availableUser);}, 500);
            return;
        }
        gridsLoading = true;
        setTimeout(function(){gridsLoading = false;}, 1500);
        svcUserManagement.AddToUsersForNewGroup(
            _at,
            availableUser.UniqueIdentifier,
            function() {
                <%=_currentGrid.ReloadGridHandler %>(true);
                <%=_availableGrid.ReloadGridHandler %>(true);
            },
            "");
    }
    
    //
    function onCurrentItemSelect(currentUser) {
        if (gridsLoading)
        {
            setTimeout(function(){onCurrentItemSelect(currentUser);}, 500);
            return;
        }
        gridsLoading = true;
        setTimeout(function(){gridsLoading = false;}, 1500);

        svcUserManagement.RemoveFromUsersForNewGroup(
            _at,
            currentUser.UniqueIdentifier,
            function() {
                <%=_currentGrid.ReloadGridHandler %>(true);
                <%=_availableGrid.ReloadGridHandler %>(true);
            },
            '');
    }

    //
    function <%=ID %>showGrids(){
        <%=_availableGrid.ReloadGridHandler %>(true);
        <%=_currentGrid.ReloadGridHandler %>(true);
    }
    
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_addGroupWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true" OnNextButtonClick="AddGroupWizard_NextButtonClick" OnFinishButtonClick="AddGroupWizard_FinishButtonClick" OnCancelButtonClick="AddGroupWizard_CancelButtonClick">
        <WizardSteps>
            <asp:WizardStep ID="GroupNameStep" runat="server">
                <asp:Panel ID="Panel1" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" TextId="/pageText/users/groups/add.aspx/nameTitle" Text="Group name"/>
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/groups/add.aspx/nameInstructions" Text="Enter a name and optional description for this group"/>
                    </div>
                        
                    <div>
                        <asp:ValidationSummary ID="_validationSummary" runat="server" CssClass="error message" ValidationGroup="groupValidationGroup" />
                    </div>   

                    <div class="formInput">
                        <p>
                            <ckbx:MultiLanguageLabel ID="_groupNameLabel" runat="server" AssociatedControlID="_groupName" TextId="/pageText/users/groups/add.aspx/groupName" CssClass="loginInfoLabel" />
                        </p>
                        <asp:TextBox ID="_groupName" runat="server" MaxLength="32" Width="250" />
                        
                        <div>
                            <asp:RequiredFieldValidator ID="_groupNameRequired" runat="server" ControlToValidate="_groupName" Display="Dynamic" CssClass="error message" ValidationGroup="groupValidationGroup">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="_groupNameLength" runat="server" Display="Dynamic" ControlToValidate="_groupName" CssClass="error message" ValidationExpression="[\w\s-.]{1,255}" ValidationGroup="groupValidationGroup">*</asp:RegularExpressionValidator>
                            <asp:CustomValidator ID="_groupExistValidator" runat="server" ControlToValidate="_groupName" OnServerValidate="_groupExistValidator_ServerValidate" CssClass="error message" ValidationGroup="groupValidationGroup">*</asp:CustomValidator>
                        </div>
                        <br class="clear" />
                    </div>

                    <div class="formInput">
                        <p><ckbx:MultiLanguageLabel ID="_groupDescriptionLabel" runat="server" AssociatedControlID="_groupDescription" TextId="/pageText/users/groups/add.aspx/groupDescription" /></p>
                        <asp:TextBox ID="_groupDescription" runat="server" TextMode="MultiLine" Rows="5" Columns="60" />
                    </div>
                    <br class="clear" />
                </asp:Panel>
            </asp:WizardStep>

            <asp:WizardStep ID="AddMembersStep" runat="server">
                <asp:Panel runat="server">
                    <div class="dialogInstructions">
                        <%=WebTextManager.GetText("/pageText/users/groups/add.aspx/instructions")%>
                    </div>
                   <div id="groupMembers" class="groupMembersContainer">
                   
                        <div class="groupMembersAccumulatorWrapper border999 shadow999 left">
                            <!-- Filter -->
                            <div class="blueBackground">
                                <asp:Panel runat="server" ID="_userStoreWrapper" CssClass="padding5">
                                    <ckbx:UserStoreSelect EventID="gac" ShowLabel="True" ID="_userStoreSelect" runat="server"  />                                
                                </asp:Panel>
                                <div class="right groupMembersFilter" id="availableMembers">
                                    <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
                                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip" />
                                </div>
                                <br style="clear:both;" />
                            </div>

                            <ckbx:Grid ID="_availableGrid" runat="server" GridCssClass="ckbxNewGroupUsersGrid overflow-y"/>
        
                        </div>
        
                        <asp:Label ID="_currentLbl" runat="server" CssClass="PrezzaLabel" />

                        <div class="groupMembersAccumulatorWrapper border999 shadow999 right">
                            <!-- Filter -->
                            <div class="blueBackground">
                                <div class="right groupMembersFilter" id="currentMembers">
                                    <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
                                    <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip"/>
                                </div>
                                <br class="clear" />
                            </div>
        
                            <ckbx:Grid ID="_currentGrid" runat="server" GridCssClass="ckbxNewGroupUsersGridRight overflow-y"/>

                        </div>
                    </div>

                    <div style="display:none;">
                        <asp:TextBox ID="_delayLoad" runat="server" Text="True" />
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                <asp:Panel ID="Panel2" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle" style="width: 220px">
                        <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/groups/add.aspx/reviewTitle" Text="Ready to create group"/>
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" CssClass="" TextId="/pageText/users/groups/add.aspx/reviewInstructions" Text="Please review the information below to ensure it is correct"/>
                    </div>

                    <div class="dashStatsWrapper shadow999 border999">
                        <div class="dashStatsHeader"><span class="left mainStats"><%=WebTextManager.GetText("/pageText/users/groups/add.aspx/groupProperties") %></span></div>
                        <div class="dashStatsContent zebra">
                            <ckbx:MultiLanguageLabel ID="_groupNameReviewLabel" runat="server" TextId="/pageText/users/groups/add.aspx/groupName" CssClass="left fixed_125" />
                            <b><ckbx:MultiLanguageLabel ID="_groupNameReview" runat="server" CssClass="left"/></b>
                            <br class="clear" />
                        </div>
                        <div class="dashStatsContent detailZebra">
                            <ckbx:MultiLanguageLabel ID="_groupDescriptionReviewLabel" runat="server" TextId="/pageText/users/groups/add.aspx/groupDescription" CssClass="left fixed_125" />
                            <b><ckbx:MultiLanguageLabel ID="_groupDescriptionReview" runat="server" CssClass="left"/></b>
                            <br class="clear" />
                        </div>
                    </div>
                    <div class="groupMembersAccumulatorWrapper border999 shadow999">
                        <ckbx:Grid ID="_reviewGroupUsersGrid" runat="server" GridCssClass="ckbxNewGroupUserGridReview overflow-y"/>
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="FinishStep" runat="server" StepType="Complete">
                <asp:Panel ID="Panel3" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_completionTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/groups/add.aspx/completionTitle" Text="Group created"/>
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/users/groups/add.aspx/completionInstructions" Text="You may create another user group or return to the group manager"/>
                    </div>

                    <ckbx:MultiLanguageLabel ID="_createGroupError" runat="server" CssClass="error message" Visible="false" />
                </asp:Panel>
                <div class="WizardNavContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeButton" runat="server" CssClass="cancelButton left" OnClick="_close_OnClick" TextID="/pageText/users/groups/add.aspx/closeButton" />
                    <btn:CheckboxButton ID="_editUserGroupButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton right" OnClick="_editUserGroupButton_OnClick" TextID="/pageText/users/groups/add.aspx/exitButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
        <StartNavigationTemplate>
            <btn:WizardButtons ID="_startNavigationButtons" runat="server" NextButtonValidationGroup="groupValidationGroup" CloseDialogOnCancel="false" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons ID="_finishNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </FinishNavigationTemplate>
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
    </asp:Wizard>                
</asp:Content>
