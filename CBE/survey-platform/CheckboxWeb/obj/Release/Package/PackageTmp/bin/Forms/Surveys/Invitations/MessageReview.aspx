<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="MessageReview.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.MessageReview" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel ID="_messageSelectPanel" runat="server">
        <ckbx:MultiLanguageLabel ID="_messageSelectLbl" runat="server" TextId="/pageText/viewInvitation.aspx/selectMessage" />
        <asp:DropDownList ID="_messageList" runat="server" AutoPostBack="true" />
        <br />
        <asp:Panel ID="_errorMsgPlace" runat="server" Style="padding-top:5px;">
            <ckbx:MultiLanguageLabel ID="_successLbl" runat="server" TextId="/pageText/viewInvitation.aspx/successful" />
            &nbsp;&nbsp;
            <asp:Label ID="_yesNoLbl" runat="server" />
            &nbsp;
            <asp:Label ID="_errorMessageLbl" runat="server" />
        </asp:Panel>
    </asp:Panel>
    <div class="invitationPreview invitationBody">
        <asp:Panel ID="_subjectPreviewPanel" runat="server" style="border-bottom:1px solid gray;margin:15px" />
        <asp:Panel ID="_bodyPreviewPanel" runat="server" style="margin:15px"/>
    </div>
    <hr /> 
    <div style="margin-right: auto; margin-left: auto; width: 300px; margin-top:5px;">   
        <btn:CheckboxButton ID="_bottomCloseButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextID="/pageText/forms/surveys/invitations/messageReview.aspx/closeButton" OnClick="CloseBtn_Click" />
        <div class="clear"></div>
    </div>
</asp:Content>
