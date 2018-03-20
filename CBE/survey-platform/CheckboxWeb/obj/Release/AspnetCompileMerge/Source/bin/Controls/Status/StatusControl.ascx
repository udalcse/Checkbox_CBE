<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StatusControl.ascx.cs" Inherits="CheckboxWeb.Controls.Status.StatusPanel" %>
<asp:Panel ID="_statusPanel" runat="server" CssClass="StatusPanel" Visible="false" Height="0" Width="0">
    <asp:Label ID="_messageLabel" runat="server" /><%--<asp:LinkButton ID="_messageAction" runat="server" CssClass="StatusAction right" OnClick="MessageAction_Click"/>--%>
    <div class="clear"></div>
</asp:Panel>