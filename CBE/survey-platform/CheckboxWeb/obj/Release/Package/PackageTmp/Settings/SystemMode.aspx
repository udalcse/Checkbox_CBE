<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="true" CodeBehind="SystemMode.aspx.cs" Inherits="CheckboxWeb.Settings.SystemMode" %>
<%@ Register TagPrefix="ckbx" TagName="Timeline" Src="~/Forms/Controls/Timeline.ascx" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
   <script type="text/javascript">
       function timelineLoaded() {
           $('.introPage').hide();
       }
   </script>
    <ckbx:ResolvingScriptElement ID="_templateHelper" runat="server" Source="~/Resources/templateHelper.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.localize.js" />
</asp:Content>
<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <h3>System mode</h3>

    <!-- Site wide options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats">Mode</span>
        </div>
        <div class="dashStatsContent">
            <asp:RadioButtonList runat="server" ID="_systemModeOptions" RepeatLayout="Flow" RepeatDirection="Vertical" OnSelectedIndexChanged="SystemModeOption_SelectedIndexChanged">
                <asp:ListItem Text="Prep Mode" Value="PrepMode"></asp:ListItem>
                <asp:ListItem Text="Production Mode" Value="ProdMode"></asp:ListItem>
            </asp:RadioButtonList>
            <br class="clear"/>
            <asp:Label runat="server" ID="_prepModeWarningMsg" ForeColor="red" Visible="False">You cannot put Engauge into Prep Mode before clearing response data and deactivating all active surveys</asp:Label>
        </div>
    </div>

    <!-- Report options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">Prep mode options</span>
        </div>
        <div class="dashStatsContent">
            <p>Select the users who can receive Prep Mode survey invitations</p>
            <p>Available users:</p>
            <hr class="dotted-line"/>
            <asp:CheckBoxList runat="server" ID="_availableUsers" />
            <br class="clear"/>
        </div>
    </div>

    <!-- user options -->
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">System mode history</span>
        </div>
        <div class="dashStatsContent">
           <ckbx:Timeline ID="_timeline" runat="server" Manager="UserManager" EnableTheming="False" OnClientLoad="timelineLoaded" ShowGraph="False"/>
        </div>
    </div>
    <br class="clear"/>
</asp:Content>