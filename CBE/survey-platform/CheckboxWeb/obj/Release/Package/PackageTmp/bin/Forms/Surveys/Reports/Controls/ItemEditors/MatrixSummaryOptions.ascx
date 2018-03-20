<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSummaryOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.MatrixSummaryOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/useAliasEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <br class="clear"/>

    <asp:Panel ID="_multiOptionsPlace" runat="server" Visible="false">
        <div class="left"><p><label><%=WebTextManager.GetText("/controlText/chartEditor/useSingleChart") %></label></p></div>
        <div class="left"><asp:CheckBox ID="_singleChartChk" runat="server" /></div>
        <br class="clear"/>
    </asp:Panel>
</div>
