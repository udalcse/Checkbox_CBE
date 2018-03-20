<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="MemcachedCache.aspx.cs" Inherits="CheckboxWeb.Settings.MemcachedCache" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <h3>Cache Management</h3>
        <btn:CheckboxButton runat="server" ID="_flushButton" CssClass="ckbxButton roundedCorners orangeButton border999 shadow999 left" Text="Flush" style="margin-left:0px;margin-top:20px;color:White;width:200px"></btn:CheckboxButton>
        <div class="clear"></div>
    </div>
</asp:Content>