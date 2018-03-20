<%@ Control Language="C#" AutoEventWireup="false" Inherits="CheckboxWeb.Controls.Security.GrantAccessControl" Codebehind="GrantAccessControl.ascx.cs" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Providers" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserStoreSelect" Src="~/Users/Controls/UserStoreSelect.ascx" %>

<script type="text/javascript">
    var <%= ClientID %>_provider = '<%= MembershipProviderManager.FirstAvailableProvider.Name %>';
    var <%= ClientID %>_providerType = '<%= MembershipProviderManager.FirstAvailableProvider.GetType().Name %>';
    var _availableLiveSearch;
    var _currentLiveSearch;

    $(document).ready(function (){
        $('#grantAccessControl').on('membershipProviderChanged', function(e, eventId, membershipName, type) {
            if (eventId == 'gac') {
                <%= ClientID %>_provider = membershipName;
                <%= ClientID %>_providerType = type;
                <%=_availableGrid.ReloadGridHandler %> (true);
            }
        });

        _availableLiveSearch = new LiveSearchObj('availableMembers', 'available');
        _currentLiveSearch = new LiveSearchObj('currentMembers', 'current');
    });
        
    //Load grid of current entries
    function <%=ID %>loadAvailableGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){ 
        <% if(ApplicationManager.AppSettings.DisableUserListForAD) { %>
            //if provider type is AD and user preloading is disabled, do nothing
            if ('<%= typeof(ActiveDirectoryMembershipProvider).Name %>' === <%= ClientID %>_providerType && !filterValue) {
                    <%= _availableGrid.ClearGridHandler %>();
                    return;
                }
        <% } %>
        svcSecurityManagement.getAvailableAclEntries(
            '',
            <%= ClientID %>_provider,
            '<%=SecuredResourceType %>',
            <%=SecuredResourceId %> , 
            '<%=PermissionToGrant %>',
            currentPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %> ,
            '',
            false,
            filterValue,
            loadCompleteCallback,
            loadCompleteArgs);
    }
        
    //Load grid of current entries
    function <%=ID %>loadCurrentGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){ 
        svcSecurityManagement.getCurrentAclEntries(
            '',
            '<%=SecuredResourceType %>',
            <%=SecuredResourceId %> , 
            '<%=PermissionToGrant %>',
            currentPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %> ,
            '',
            false,
            filterValue,
            loadCompleteCallback,
            loadCompleteArgs);
    }
    
    //
    function <%=ID %>onAvailableItemSelect(aclEntry) {
        if (!aclEntry.IsInList) {
            svcSecurityManagement.addEntryToAcl(
                '',
                <%=(int)SecuredResourceType %>,
                <%=SecuredResourceId %> ,
                aclEntry.EntryType,
                aclEntry.EntryIdentifier,
                '<%=PermissionToGrant %>',
                function() {
                    <%=_currentGrid.ReloadGridHandler %> (true);
                    <%=_availableGrid.ReloadGridHandler %> (true);
                }
            );
        }
    }
    
    //
    function <%=ID %>onCurrentItemSelect(aclEntry) {
        svcSecurityManagement
            .removeEntryFromAcl(
            '',
            <%=(int)SecuredResourceType %> ,
            <%=SecuredResourceId %> ,
            aclEntry.EntryType,
            aclEntry.EntryIdentifier,
            function() {
                if(typeof (_currentPolicyId) != 'undefined') {
                    _currentPolicyId = -1;
                }
                <%=_currentGrid.ReloadGridHandler %> (true);
                <%=_availableGrid.ReloadGridHandler %> (true);
            }
        );
    }

    //
    function <%=ID %>showGrids(){
        <%=_availableGrid.ReloadGridHandler %>(true);
//        <%=_currentGrid.ReloadGridHandler %>(true);
    }
</script>

<div class="dialogInstructions">
    <%=WebTextManager.GetText("/pageText/security/grantAccessDialog.aspx/instructions")%>
</div>

<asp:Label ID="_availableLbl" runat="server" />
<div id="grantAccessControl" class="aclContainer">
    <div class="aclAccumulatorWrapper border999 shadow999 left">
        <!-- Filter -->
        <div class="blueBackground">
            <ckbx:UserStoreSelect EventID="gac" ID="UserStoreSelect" runat="server" />
            <div class="right aclFilter" id="availableMembers">
                <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip" />
            </div>
            <br style="clear:both;" />
        </div>

        <ckbx:Grid ID="_availableGrid" runat="server" GridCssClass="ckbxAclGrid overflow-y"  />
        
    </div>
        
    <asp:Label ID="_currentLbl" runat="server" CssClass="PrezzaLabel" />

    <div class="aclAccumulatorWrapper border999 shadow999 right">
        <!-- Filter -->
        <div class="blueBackground">
            <div class="right aclFilter" id="currentMembers">
                <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip"/>
            </div>
            <br class="clear" />
        </div>
        
        <ckbx:Grid ID="_currentGrid" runat="server" GridCssClass="ckbxAclGridRight overflow-y"  />

    </div>
</div>

<div style="display:none;">
    <asp:TextBox ID="_delayLoad" runat="server" Text="True" />
</div>
