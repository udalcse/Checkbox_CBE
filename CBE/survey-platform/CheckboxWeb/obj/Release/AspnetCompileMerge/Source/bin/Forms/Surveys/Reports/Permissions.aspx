<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Permissions.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Permissions" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register TagPrefix="security" TagName="GrantAccess" Src="~/Controls/Security/GrantAccessControl.ascx" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <style>body{background-color:#DEDEDE;}</style>

     <script type="text/javascript">
        $(document).ready(function () {
            
            $('#securityPermissionsTabs').ckbxTabs({ 
                tabName: 'securityPermissionsTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('#securityPermissionsTabs').show();$('.tabContentContainer').show();}
            });

        });
    </script>

</asp:Content>
<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
 
    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
        <asp:TextBox ID="_currentAdvancedTabIndex" runat="server" Text="0" />
    </div>
    <ul id="securityPermissionsTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/basicSettings")%></li>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/permissions.aspx/advancedSettings")%></li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>
    
    <div class="tabContentContainer">
        <div class="padding10" id="securityPermissionsTabs-0-tabContent">
            <ckbx:MultiLanguageRadioButton TextId='/pageText/settings/security.aspx/private' ID="_privateReport" runat="server" GroupName="ReportType"/>
            <div class="clear"></div>
            <ckbx:MultiLanguageRadioButton TextId='/pageText/settings/security.aspx/public' ID="_publicReport" runat="server" GroupName="ReportType"/>
        </div>

        <div id="securityPermissionsTabs-1-tabContent">
            <security:GrantAccess ID="_grantAccess" runat="server" />
        </div>
    </div>

</asp:Content>
