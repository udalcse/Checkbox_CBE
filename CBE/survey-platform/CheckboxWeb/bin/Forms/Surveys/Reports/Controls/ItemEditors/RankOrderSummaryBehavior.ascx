<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderSummaryBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.RankOrderSummaryBehavior" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/chartEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <br class="clear" />   
    
    <asp:Panel ID="_multiOptionsPlace" runat="server" Visible="false">
        <p><label for="<%=_singleChartChk.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/useSingleChart") %></label></p>
        <asp:CheckBox ID="_singleChartChk" runat="server" />
        <br class="clear" />
    </asp:Panel>
</div>