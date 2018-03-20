<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Permissions.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Permissions" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register Src="~/Controls/Security/AccessListEditor.ascx" TagPrefix="security" TagName="AclEditor" %>
<%@ Register TagPrefix="security" TagName="PolicyEditor" Src="~/Controls/Security/DefaultPolicyEditor.ascx" %>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Common" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <style>body{background-color:#DEDEDE;}</style>

     <script type="text/javascript">
        var _grantAccessControlLoaded = false;

        $(document).ready(function () {
            <% if (IsDialog) { %>
                $('.dialogFormContainer').css('background-color', 'white');
            <% } %>

            $('#aclEditorTabs').ckbxTabs({
                tabName: 'aclEditorTabs',
                initialTabIndex: 0,
                tabStyle: 'normal',
                onTabClick: function(index) {
                <% if (!ApplicationManager.UseSimpleSecurity) {%>
                    if (index == 0) {
                        reloadAccessList();
                    }
                    
                    if(index == 1) {
                        //Not ideal, but call method in default policy editor
                        toggleDefaultPolicyGrids();
                    }
                    
                    if (index == 2 && !_grantAccessControlLoaded) {
                        //This method is provided by GrantAccessControl
                        _grantAccessshowGrids();
                        _grantAccessControlLoaded = true;
                    }

               <%}%>
                },
                onTabsLoaded: function () { $('#aclEditorTabs').show(); $('#aclEditorTabContentContainer').show();
                 }
            });
        });
    </script>

</asp:Content>
<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
  
    <% if(IsDialog) { %>
        <ul id="aclEditorTabs" class="tabContainer" style="display:block;">
    <% } else { %>
        <ul id="aclEditorTabs" class="tabContainer">
    <% } %>
    <% if (!ApplicationManager.UseSimpleSecurity)
        { %>
        <li><%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/accessList")%></li>
        <li><%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/defaultPolicy")%></li>
    <%} %>
        <li><%=WebTextManager.GetText("/pageText/aclAddEntries.aspx/addEntriesToAcl")%></li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>

    <div id="aclEditorTabContentContainer">
    <% if (!ApplicationManager.UseSimpleSecurity)
        { %>
        <div class="padding10" id="aclEditorTabs-0-tabContent">
            <security:AclEditor 
                ID="_aclEditor" 
                AccessListHeight="300"
                PermissionsListHeight="305"
                ShowTopButtons="false"
                ShowBottomButtons="false"
                runat="server" />
        </div>
        <div class="padding10" id="aclEditorTabs-1-tabContent">
            <security:PolicyEditor ID="_defaultPolicyEditor" runat="server" ShowCloseButton="false" ShowApplyButton="false" />
        </div>
    <%}; %>
        <div class="padding10" id="aclEditorTabs-2-tabContent">
            <security:GrantAccess ID="_grantAccess" runat="server" />
        </div>
    </div>
</asp:Content>
