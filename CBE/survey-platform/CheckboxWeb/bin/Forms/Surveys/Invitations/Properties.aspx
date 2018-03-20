<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Properties" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register TagName="InvitationProperties" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Properties.ascx" %>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <ckbx:InvitationProperties ID="_properties" runat="server" />
    </div>
</asp:Content>