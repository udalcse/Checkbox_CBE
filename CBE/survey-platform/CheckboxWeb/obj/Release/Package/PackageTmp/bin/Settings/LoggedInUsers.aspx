<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="LoggedInUsers.aspx.cs" Inherits="CheckboxWeb.Settings.LoggedInUsers" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="grid" Namespace="Checkbox.Web.UI.Controls.GridTemplates" Assembly="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/currentUsers")%></h3>
        
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsContent">
            <div class="left">
                <div class="left input fixed_100"><ckbx:MultiLanguageLabel id="serverNameLbl" runat="server" CssClass="PrezzaBold" TextId="/pageText/settings/loggedInUsers.aspx/serverName">Server Name:</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:label id="serverName" runat="server"  />
                </div>
                <br class="clear" />
                <div class="left input fixed_100"><ckbx:MultiLanguageLabel id="Label2" runat="server" CssClass="PrezzaBold" TextId="/pageText/settings/loggedInUsers.aspx/numberOfUsers">Number of Users:</ckbx:MultiLanguageLabel></div>
                <div class="left input">
                    <asp:label id="numberOfUsers" runat="server" CssClass="ErrorMessage"></asp:label>
                </div>
                <br class="clear" />
            </div>
            <div class="left fixed_25">&nbsp;</div>
            <div class="left spacing">
                <ckbx:MultiLanguageLinkButton runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" id="refreshBtn" style="font-size:11px;" TextId="/pageText/settings/loggedInUsers.aspx/refresh" />
            </div>
            <br class="clear" />
            <div class="padding10">
                <asp:GridView ID="_users" runat="server" OnPageIndexChanging="OnUserGridPageIndexChanging" AutoGenerateColumns="false" AllowPaging="True" Width="100%"  HeaderStyle-HorizontalAlign="Left" HeaderStyle-VerticalAlign="Middle">
                    <EmptyDataTemplate>
                        <ckbx:MultiLanguageLabel id="_noExceptions" runat="server" CssClass="GridNoRecords" TextId="/pageText/Users/Manage.aspx/noUsersFound">No users...</ckbx:MultiLanguageLabel>
                    </EmptyDataTemplate>
                    <Columns>
                        <asp:BoundField DataField="UniqueIdentifier" Visible="true"/>
                        <asp:BoundField DataField="UserHostName" Visible="true"/>
                        <asp:BoundField DataField="UserHostIP" Visible="true"/>
                        <asp:BoundField DataField="LoginTime" Visible="true"/>
                        <asp:BoundField DataField="CurrentUrl"  Visible="true"/>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="dialogInstructions">
            <ckbx:MultiLanguageLabel ID="noteLbl" runat="server" TextId="/pageText/settings/loggedInUsers.aspx/noteText">* Only users logged-in to the web server identified above are shown on this screen.</ckbx:MultiLanguageLabel>        
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
