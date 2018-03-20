<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Legend.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.AverageScore.EditorControls.Legend" %>

<div class="formInput">

    <div class="left checkBox"><asp:CheckBox ID="_showLegend" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_showLegend" ID="showLegendLbl" runat="server" TextId="/pageText/styles/charts/legend.ascx/showLegend" /></p></div>
    <br class="clear" />

    <p><ckbx:MultiLanguageLabel ID="legendLayoutLbl" AssociatedControlID="_legendLayout" runat="server" TextId="/pageText/styles/charts/legend.ascx/layout" /></p>
    <ckbx:MultiLanguageDropDownList ID="_legendLayout" runat="server" >
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/vertical" Value="vertical" />
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/horizontal" Value="horizontal" />
    </ckbx:MultiLanguageDropDownList>
    <br class="clear" />

    <p><ckbx:MultiLanguageLabel ID="legendAlignLbl" AssociatedControlID="_legendAlign" runat="server" TextId="/pageText/styles/charts/legend.ascx/align" /></p>
    <ckbx:MultiLanguageDropDownList ID="_legendAlign" runat="server">
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/left" Value="left" />
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/center" Value="center" />
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/right" Value="right" />
    </ckbx:MultiLanguageDropDownList>
    <br class="clear" />

    <p><ckbx:MultiLanguageLabel ID="legendVerticalAlignLbl" AssociatedControlID="_legendVerticalAlign" runat="server" TextId="/pageText/styles/charts/legend.ascx/verticalAlign" /></p>
    <ckbx:MultiLanguageDropDownList ID="_legendVerticalAlign" runat="server">
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/top" Value="top" />
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/center" Value="middle" />
        <asp:ListItem TextId="/pageText/styles/charts/legend.ascx/bottom" Value="bottom" />
    </ckbx:MultiLanguageDropDownList>
    <br class="clear" />
</div>