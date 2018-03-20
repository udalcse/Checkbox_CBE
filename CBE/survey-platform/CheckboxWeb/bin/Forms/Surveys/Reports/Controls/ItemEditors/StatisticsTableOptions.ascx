<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StatisticsTableOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.StatisticsTableOptions" %>
<%@ Import Namespace="Checkbox.Web" %>

<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/useAliasEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>

    <br class="clear"/>

    <p><label><%=WebTextManager.GetText("/controlText/statisticsItemEditor/reportOption")%></label></p>
    <ckbx:MultiLanguageDropDownList ID="_calculationOption" runat="server">
        <asp:ListItem Value="Responses" TextId="/controlText/statisticsItemEditor/responses" />
        <asp:ListItem Value="Mean" TextId="/controlText/statisticsItemEditor/mean" />
        <asp:ListItem Value="Median" TextId="/controlText/statisticsItemEditor/median" />
        <asp:ListItem Value="Mode" TextId="/controlText/statisticsItemEditor/mode" />
        <asp:ListItem Value="StdDeviation" TextId="/controlText/statisticsItemEditor/stdDeviation" />
        <asp:ListItem Value="All" TextId="/controlText/statisticsItemEditor/all" />
    </ckbx:MultiLanguageDropDownList>
</div>