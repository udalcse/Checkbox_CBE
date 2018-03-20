<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="UsersSecurity.aspx.cs" Inherits="CheckboxWeb.Settings.UsersSecurity" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=_minPasswordLength.ClientID%>').numeric();
            $('#<%=_minPasswordNonAlphaNumeric.ClientID%>').numeric();
            $('#<%=_maxFailedLoginAttempts.ClientID%>').numeric();
        });
    </script>
</asp:Content>


<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <!-- Controls whether or not Checkbox uses cookies to track anonymous respondents. When cookies are disabled it is not possible for anonymous respondents to resume incomplete responses or for Checkbox to enforce the maximum number of responses per user. -->
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/userssecurity")%></h3>

    <div class="dashStatsWrapper  border999 shadow999" id="_securityMode" runat="server">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/userssecurity.aspx/passwordRestrictions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left input fixed_300">
                <ckbx:MultiLanguageLabel ID="_lblMinPasswordLength" runat="server"  AssociatedControlID="_minPasswordLength" TextId="/pageText/settings/userssecurity.aspx/minPasswordLength">Min Password Length</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- Minimum length allowed for user passwords. -->            
                <asp:TextBox id="_minPasswordLength" runat="server" MaxLength="3" />
            </div>
            <div class="clear"></div>
            <div class="left input fixed_300">
                <ckbx:MultiLanguageLabel ID="_lblMinPasswordNonAlphaNumeric" runat="server"  AssociatedControlID="_minPasswordLength" TextId="/pageText/settings/userssecurity.aspx/minPasswordNonAlphaNumeric">Min Password Non Alpha Numeric</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- Minimum number of non-alphanumeric characters required for a password.  -->            
                <asp:TextBox id="_minPasswordNonAlphaNumeric" runat="server" MaxLength="2" />
            </div>
            <div class="clear"></div>
         </div>

    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/usersecurity.aspx/lockoutOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="left input fixed_300">
                <ckbx:MultiLanguageLabel ID="_lblMaxFailedLoginAttempts" runat="server"  AssociatedControlID="_maxFailedLoginAttempts" TextId="/pageText/settings/userssecurity.aspx/maxFailedLoginAttempts">Max Failed Login Attempts</ckbx:MultiLanguageLabel>
            </div>
            <div class="left input">
                <!-- Maximum number of consecutive failed login attempts allowed for an account until it is “locked”. -->            
                <asp:TextBox id="_maxFailedLoginAttempts" runat="server" MaxLength="3"/>
            </div>            
        </div>

        <div class="clear"></div>
    </div>

    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
