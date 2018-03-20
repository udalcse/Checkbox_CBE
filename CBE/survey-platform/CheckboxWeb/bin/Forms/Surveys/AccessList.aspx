<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AccessList.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.AccessList" MasterPageFile="~/Dialog.Master"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <style>body{background-color:#DEDEDE;}</style>

    <script type="text/javascript">
        $(document).ready(function () {
            
            $('#securityPermissionsTabs').ckbxTabs({ 
                tabName: 'securityPermissionsTabs',
                initialTabIndex:0,
                onTabsLoaded: function(){$('#securityPermissionsTabs').show();$('.tabContentContainer').show();}
            });

            $('#aclEditorTabs').ckbxTabs({
                tabName: 'aclEditorTabs',
                initialTabIndex: 0,
                tabStyle: 'inverted',
                onTabsLoaded: function () { $('#aclEditorTabs').show(); $('.aclEditorTabContentContainer').show(); }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <ul id="securityPermissionsTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/advancedSettings")%></li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>
    <div id="securityPermissionsTabs-2-tabContent">
        <ul id="aclEditorTabs" class="tabContainer">
            <li><%=WebTextManager.GetText("/pageText/aclAddEntries.aspx/addEntriesToAcl")%></li>
            <div class="clear"></div>
        </ul>
        <div class="clear"></div>

        <div id="aclEditorTabContentContainer">
            <div class="padding10" id="aclEditorTabs-2-tabContent">
                <security:GrantAccess ID="_grantAccess" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>