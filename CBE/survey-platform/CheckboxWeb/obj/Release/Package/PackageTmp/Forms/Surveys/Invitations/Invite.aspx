<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Invite.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Invite" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register TagName="InviteAdditional" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/AddRecipients.ascx" %>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <ckbx:InviteAdditional ID="_inviteAdditional" runat="server" PendingInvitationMode="false" />
</asp:Content>
