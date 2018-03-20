<%@ Page Language="C#" AutoEventWireup="false" Inherits="CheckboxWeb.ErrorPages.LimitError" MasterPageFile="~/Admin.master" Theme="CheckboxTheme" Codebehind="LimitError.aspx.cs" %>
<%@ MasterType VirtualPath="~/Admin.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="_headContent"></asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
<div class="padding10 introPage">
    <p>
        <ckbx:MultiLanguageLabel id="_limitDesc" runat="server" TextId="/pageText/limitError.aspx/description">
            The page you tried to access is not available because a software licensing limit was exceeded.  The message below will provide more information about the specific
            limit that was exceeded and what corrective action to take.
        </ckbx:MultiLanguageLabel>
    </p>
    <div class="spacing">&nbsp;</div>
    <asp:Label ID="_limitMessage" runat="server" CssClass="ErrorMessage" />
</div>
</asp:Content>