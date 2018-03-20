<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="SecurityEditor.aspx.cs" Inherits="CheckboxWeb.Security.SecurityEditor" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Controls/Security/AccessListEditor.ascx" TagPrefix="ckbx" TagName="AclEditor" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="security" TagName="PolicyEditor" Src="~/Controls/Security/DefaultPolicyEditor.ascx" %>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript" language="javascript">
        var _grantAccessControlLoaded = false;
        
        $(document).ready(function () {
            $('#securityEditorTabs').ckbxTabs({
                tabName: 'securityEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){                    
                    if (index == 0) {
                        reloadAccessList();
                    }
                    
                    if (index == 2 && !_grantAccessControlLoaded) {
                        //This method is provided by GrantAccessControl
                        _grantAccessshowGrids();
                        _grantAccessControlLoaded = true;
                    }

                    $('#<%=_currentTabIndex.ClientID %>').val(index);
                },
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });
    </script>
</asp:Content>

<asp:Content ID="_content" runat="server" ContentPlaceHolderID="_pageContent">
    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

     <ul id="securityEditorTabs" class="tabContainer shadow999">
    <% if (!Checkbox.Management.ApplicationManager.UseSimpleSecurity)
       { %>
        <li><%=WebTextManager.GetText("/pageText/security/securityEditor.aspx/accessList")%></li>
        <li><%=WebTextManager.GetText("/pageText/defaultPolicyEditor.aspx/defaultPolicy")%></li>
    <%} %>
        <li><%=WebTextManager.GetText("/pageText/aclAddEntries.aspx/addEntriesToAcl")%></li>
    </ul>

    <div class="tabContentContainer padding10">
    <% if (!Checkbox.Management.ApplicationManager.UseSimpleSecurity)
       { %>
        <div id="securityEditorTabs-0-tabContent">
            <ckbx:AclEditor ID="_aclEditor" runat="server" ShowTopButtons="False" ShowBottomButtons="False" AccessListHeight="340" PermissionsListHeight="340"/> 
        </div>

        <div id="securityEditorTabs-1-tabContent">
            <asp:Panel ID="_policyWrapper" runat="server">
                <security:PolicyEditor ID="_defaultPolicyEditor" runat="server" ShowCloseButton="false" ShowApplyButton="false" />
            </asp:Panel>
        </div>
    <%} %>

        <div id="securityEditorTabs-2-tabContent">
            <asp:Panel ID="_grantAccessWrapper" runat="server">
                <security:GrantAccess ID="_grantAccess" runat="server" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>
