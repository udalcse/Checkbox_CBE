<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Message.aspx.cs" EnableEventValidation="false" ValidateRequest="false" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Message" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>
<%@ Register src="../../../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" TagPrefix="ckbx" TagName="EditMessage" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <ckbx:EditMessage ID="_editMessage" runat="server" />
    </div>
</asp:Content>
