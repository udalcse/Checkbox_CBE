<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AccessListEditor.ascx.cs" Inherits="CheckboxWeb.Controls.Security.AccessListEditor" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript">
    var _currentPolicyId = -1;
    var _liveSearch;

    //Reload the access list
    function reloadAccessList() {
        <%=_aclGrid.ReloadGridHandler %> (true);
        toggleGrids();
    }

    $(document).ready(function() {
        _liveSearch = new LiveSearchObj('aclSearchField', 'acl');

        toggleGrids();

        $('#<%=_applyPolicyBtn.ClientID %>').click(function(e) {
            e.preventDefault();
            submitAclPolicyChanges();
        });
                
        $('#aclPermissionToggle').change(function() {
            toggleGrids();
        });
    });
    
    //
    function toggleGrids() {
        if (_currentPolicyId <= 0) {
            $('#aclPermissionMaskGridContainer').hide();
            $('#aclPermissionGridContainer').hide();
            $('#saveAclPermissionsContainer').hide();
            return;
        }
        
        $('#saveAclPermissionsContainer').show();
        
        if ($('#aclPermissionToggle').is(':checked')) {
            $('#aclPermissionMaskGridContainer').hide();
            $('#aclPermissionGridContainer').show();

            <%=_permissionsGrid.ReloadGridHandler %> (true);
        } else {
             $('#aclPermissionMaskGridContainer').show();
            $('#aclPermissionGridContainer').hide();
            
            <%=_permissionMasksGrid.ReloadGridHandler %> (true);
        }
    }
    
    ///Load grid
    function loadAclGridAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs) {
        svcSecurityManagement.getAclEntries(
            '',
            '<%=SecuredResourceType %>',
            <%=SecuredResourceId %>,
            currentPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %> ,
            '',
            false,
            filterValue,
            loadCompleteCallback,
            loadCompleteArgs);
    }

    //
    function loadPermissions(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs) {
        if(_currentPolicyId  != null && _currentPolicyId > 0) {
            if ($('#aclPermissionToggle').is(':checked')) {
                svcSecurityManagement.getPolicyPermissions(
                    '',
                    '<%=SecuredResourceType %>',
                    <%=SecuredResourceId %> ,
                    _currentPolicyId,
                    loadCompleteCallback,
                    loadCompleteArgs);
            } else {
                svcSecurityManagement.getMaskedPolicyPermissions(
                    '',
                    '<%=SecuredResourceType %>',
                    <%=SecuredResourceId %> ,
                    _currentPolicyId,
                    loadCompleteCallback,
                    loadCompleteArgs);
            } 
        } else {
            //Do nothing if no entry selected
            loadCompleteCallback(null, loadCompleteArgs);
        }
    }
    
    function onAclItemSelect(aclEntry) {
        _currentPolicyId = aclEntry.AclPolicyId;
        toggleGrids();
    }
    
     ///
    function submitAclPolicyChanges() {
       if ($('#aclPermissionToggle').is(':checked')) {
            //Working with permissions
            var selectedPermissions = new Array();
            $('#aclPermissionGridContainer input:checked').each(function() {
                selectedPermissions.push($(this).attr('permissionName'));              
            });
           
            svcSecurityManagement.updatePolicyPermissions(
                '',
                <%=(int)SecuredResourceType %> ,
                <%=SecuredResourceId %> ,
                _currentPolicyId,
                selectedPermissions,
                function (args) {
                    statusControl.initialize('aclPolicyStatus');
                    statusControl.showStatusMessage('<%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/policySuccess") %>', StatusMessageType.success);
                }
            );
       } else {
           //Working with masks
            var selectedMasks = new Array();
            $('#aclPermissionMaskGridContainer input:checked').each(function() {
                selectedMasks.push($(this).attr('maskName'));              
            });
           
            svcSecurityManagement.updatePolicyMaskedPermissions(
                '',
                <%=(int)SecuredResourceType %> ,
                <%=SecuredResourceId %> ,
                _currentPolicyId,
                selectedMasks,
                function (args) {
                    statusControl.initialize('aclPolicyStatus');
                    statusControl.showStatusMessage('<%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/policySuccess") %>', StatusMessageType.success);
                }
            );
       }

        reloadAccessList();
    }
    
    
</script>


<asp:Panel ID="_instructionsPanel" runat="server" Visible='<%# ShowInstructions %>'>
    <div class="dialogInstructions">
        <%=WebTextManager.GetText("/controlText/AccessListEditor.ascx/instructions") %>
    </div>
</asp:Panel>

<div class="aclContainer">
    <asp:Panel ID="_accessListWrapper" runat="server" CssClass="aclAccumulatorWrapper left shadow999 border999">
        <div class="blueBackground">
            <div class="right aclFilter" id="aclSearchField">
                <asp:TextBox runat="server" Width="150px" autocomplete="off" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/AccessListEditor.ascx/filterOnTip" />
                <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/AccessListEditor.ascx/filterOffTip" />
            </div>
            <br class="clear" />
        </div>

        <ckbx:Grid ID="_aclGrid" runat="server" GridCssClass="ckbxAclGrid overflow-y"  />

    </asp:Panel>

    <asp:Panel ID="_permissionsWrapper" runat="server" CssClass="aclAccumulatorWrapper right border999 shadow999">
        <div id="aclPolicyStatus" class="StatusPanel"></div>

        <div class="blueBackground">
            <div class="left aclFilter">
                <input type="checkbox" id="aclPermissionToggle" />
                <label for="aclPermissionToggle"><%=WebTextManager.GetText("/controlText/AccessListEditor.ascx/toggleGroup") %></label>
            </div>
            <br class="clear" />
        </div>
        
        <asp:Panel ID="_permissionsGridWrapper" runat="server">

            <div id="aclPermissionGridContainer" style="display:none;">
                <ckbx:Grid ID="_permissionsGrid" runat="server" GridCssClass="ckbxAclPermissionsGrid" />
            </div>
            <div id="aclPermissionMaskGridContainer" style="display:none;">
                <ckbx:Grid ID="_permissionMasksGrid" runat="server" GridCssClass="ckbxAclPermissionsGrid" />    
            </div>           

             <div style="position:relative;top:5px;" id="saveAclPermissionsContainer">
                <div class="saveChangesWrapper">
                    <btn:CheckboxButton ID="_applyPolicyBtn" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton" runat="server" TextID="/controlText/AccessListEditor.ascx/applyPolicy" />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</div>