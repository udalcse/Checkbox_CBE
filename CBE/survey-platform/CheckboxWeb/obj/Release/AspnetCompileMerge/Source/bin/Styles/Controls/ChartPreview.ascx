<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ChartPreview.ascx.cs" Inherits="CheckboxWeb.Styles.Controls.ChartPreview" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<div>
    <ckbx:MultiLanguageLabel ID="_graphTypeLbl" runat="server" TextId="/pageText/styles/charts/chartPreview.ascx/graphType" />
    <ckbx:MultiLanguageDropDownList ID="_graphType" runat="server" AutoPostBack="false" />
</div>
<asp:Panel ID="_chartPlace" runat="server" Width="600px" />