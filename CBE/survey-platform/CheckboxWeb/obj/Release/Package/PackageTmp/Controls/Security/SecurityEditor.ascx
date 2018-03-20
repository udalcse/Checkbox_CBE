<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SecurityEditor.ascx.cs" Inherits="CheckboxWeb.Controls.Security.SecurityEditor" %>
<%@ Register Src="~/Controls/Security/AccessListEditor.ascx" TagPrefix="ckbx" TagName="AclEditor" %>

<div class="fixed_200 left" id="MenuChannel">
    <div class="floatMenu"></div>
</div>

<asp:Panel id="_aclWrapperPanel" runat="server">
    <ckbx:AclEditor ID="_aclEditor" runat="server" />
</asp:Panel>