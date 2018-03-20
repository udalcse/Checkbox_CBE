﻿<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Create.aspx.cs" Inherits="CheckboxWeb.Styles.Charts.Create" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="StyleProperties" Src="~/Styles/Charts/Controls/Properties.ascx" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:StyleProperties ID="_styleProperties" runat="server"></ckbx:StyleProperties>
</asp:Content>