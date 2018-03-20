<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryTableOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SummaryTableOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left"><p><label for="<%=_aliasesChk.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>

    <br class="clear" />
        
    <p><label for="<%=_otherOptionList.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/otherOption") %></label></p>
    <ckbx:MultiLanguageDropDownList ID="_otherOptionList" runat="server">
            <asp:ListItem Value="Aggregate" TextId="/enum/analysisOtherBehavior/aggregate" />
            <asp:ListItem Value="Display" TextId="/enum/analysisOtherBehavior/displayAll" />
            <asp:ListItem Value="AggregateAndDisplay" TextId="/enum/analysisOtherBehavior/aggregateAndDisplay" />
    </ckbx:MultiLanguageDropDownList>
    
    <asp:Panel ID="_multiOptionsPlace" runat="server" Visible="false">
        <p><label for="<%=_singleChartChk.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/useSingleChart") %></label></p>
        <asp:CheckBox ID="_singleChartChk" runat="server" />
    </asp:Panel>
</div>