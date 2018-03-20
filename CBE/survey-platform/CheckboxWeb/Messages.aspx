<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Messages.aspx.cs" Inherits="CheckboxWeb.Messages" Theme="CheckboxTheme" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ContentPlaceHolderID="_pageContent" ID="page" runat="server">
    <div>
        <asp:Literal ID="_pageText" runat="server"></asp:Literal>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
    <div class="DialogButtonsContainer">
        <hr size="1" />
        <div class="buttonWrapper">
            <ckbx:MultiLanguageButton ID="_doNotShowAgainButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton ckbx-closedialog left" TextId="/pageText/message.aspx/doNotShowAgain" />
            <ckbx:MultiLanguageButton ID="_closeButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton ckbx-closedialog right" TextId="/pageText/message.aspx/close" />
            <br class="clear" />
        </div>
    </div>
</asp:Content>