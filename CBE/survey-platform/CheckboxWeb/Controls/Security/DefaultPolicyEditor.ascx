<%@ Control Language="C#" Inherits="CheckboxWeb.Controls.Security.DefaultPolicyEditor" Codebehind="DefaultPolicyEditor.ascx.cs" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript">
    $(document).ready(function() {
        //toggleDefaultPolicyGrids();

        $('#<%=_applyButton.ClientID %>').click(function(e) {
            e.preventDefault();

            submitDefaultPolicyChanges();
        });

        $('#defaultPolicyPermissionToggle').change(function() {
            toggleDefaultPolicyGrids();
        });
    });
    
    
    function toggleDefaultPolicyGrids() {
        if($('#defaultPolicyPermissionToggle').is(':checked')) {
            $('#defaultPolicyPermissionMaskGridContainer').hide();
            $('#defaultPolicyPermissionGridContainer').show();

            <%=_defaultPolicyPermissionsGrid.ReloadGridHandler %> (true);
        }
        else {
             $('#defaultPolicyPermissionMaskGridContainer').show();
            $('#defaultPolicyPermissionGridContainer').hide();
            
            <%=_defaultPolicyPermissionMasksGrid.ReloadGridHandler %> (true);
        }
    }
    
    ///
    function submitDefaultPolicyChanges() {
       if($('#defaultPolicyPermissionToggle').is(':checked')) {
            //Working with permissions
            var selectedPermissions = new Array();
            $('#defaultPolicyPermissionGridContainer input:checked').each(function() {
                selectedPermissions.push($(this).attr('permissionName'));              
            });
           
            svcSecurityManagement.updatePolicyPermissions(
                '',
                <%=(int)SecuredResourceType %> ,
                <%=SecuredResourceId %> ,
                <%=DefaultPolicyId %> ,
                selectedPermissions,
                function (args) {
                    statusControl.initialize('defaultPolicyStatus');
                    statusControl.showStatusMessage('<%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/policySuccess") %>', StatusMessageType.success);
                }
            );
       }
        else {
           //Working with masks
            var selectedMasks = new Array();
            $('#defaultPolicyPermissionMaskGridContainer input:checked').each(function() {
                selectedMasks.push($(this).attr('maskName'));              
            });
           
            svcSecurityManagement.updatePolicyMaskedPermissions(
                '',
                <%=(int)SecuredResourceType %> ,
                <%=SecuredResourceId %> ,
                <%=DefaultPolicyId %> ,
                selectedMasks,
                function (args) {
                    statusControl.initialize('defaultPolicyStatus');
                    statusControl.showStatusMessage('<%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/policySuccess") %>', StatusMessageType.success);
                }
            );
       }
    }
    

//
    function loadDefaultPolicyPermissions(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs) {
        if($('#defaultPolicyPermissionToggle').is(':checked')) {
            svcSecurityManagement.getPolicyPermissions(
                '',
                '<%=SecuredResourceType %>',
                <%=SecuredResourceId %>,
                <%=DefaultPolicyId %>,
                loadCompleteCallback,
                loadCompleteArgs);
        }
        else {
            svcSecurityManagement.getMaskedPolicyPermissions(
                '',
                '<%=SecuredResourceType %>',
                <%=SecuredResourceId %> ,
                <%=DefaultPolicyId %>,
                loadCompleteCallback,
                loadCompleteArgs);
        }
    }
</script>

<div class="dialogInstructions">
    <%=WebTextManager.GetText("/pageText/security/defaultPolicyDialog.aspx/instructions")%>
</div>

<!-- Policy Grid -->
<div class="aclContainer">
    <div class="aclAccumulatorWrapper border999 shadow999">
        <div id="defaultPolicyStatus" class="StatusPanel"></div>

        <div class="blueBackground">
            <div class="left aclFilter">
                <input type="checkbox" id="defaultPolicyPermissionToggle"/>
                <label for="defaultPolicyPermissionToggle"><%=WebTextManager.GetText("/controlText/AccessListEditor.ascx/toggleGroup") %></label>
            </div>
            <br class="clear" />
        </div>

      
        <div id="defaultPolicyPermissionGridContainer" style="display:none;">
            <ckbx:Grid ID="_defaultPolicyPermissionsGrid" runat="server" GridCssClass="ckbxAclGrid" ArrowStyle="None" />
        </div>
        <div id="defaultPolicyPermissionMaskGridContainer">
            <ckbx:Grid ID="_defaultPolicyPermissionMasksGrid" runat="server" GridCssClass="ckbxAclGrid" ArrowStyle="None" />    
        </div>           

        <div style="position:relative;top:5px;">
            <div class="saveChangesWrapper">
                <btn:CheckboxButton ID="_applyButton" runat="server" TextId="/controlText/defaultPolicyEditor.ascx/applyPolicy" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton" />
            </div>
        </div>
    </div>
</div>