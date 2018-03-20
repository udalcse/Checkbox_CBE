<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryChartBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SummaryChartBehavior" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/chartEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <br class="clear" />
    
    <p><label for="<%=_otherOptionList.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/otherOption") %></label></p>
    <ckbx:MultiLanguageDropDownList ID="_otherOptionList" runat="server">
        <asp:ListItem Value="Aggregate" TextId="/enum/analysisOtherBehavior/aggregate" />
        <asp:ListItem Value="Display" TextId="/enum/analysisOtherBehavior/displayAll" />
        <asp:ListItem Value="AggregateAndDisplay" TextId="/enum/analysisOtherBehavior/aggregateAndDisplay" />
    </ckbx:MultiLanguageDropDownList>
    <br class="clear" />
    
    <asp:Panel ID="_multiOptionsPlace" runat="server" Visible="false">
        <p><label for="<%=_singleChartChk.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/useSingleChart") %></label></p>
        <asp:CheckBox ID="_singleChartChk" runat="server" />
        <br class="clear" />
    </asp:Panel>

    <asp:Panel ID="_displayStatisticsPlace" runat="server" Visible="true">
        <p><label for="<%=_displayStatistics.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/displayStatisticsForCharts") %></label></p>
        <asp:CheckBox ID="_displayStatistics" runat="server" />
        <br class="clear" />
    </asp:Panel>

    <asp:Panel ID="_displayAnswersPlace" runat="server" Visible="true">
        <p><label for="<%=_displayAnswers.ClientID %>"><%=WebTextManager.GetText("/controlText/chartEditor/displayAnswersForCharts")%></label></p>
        <asp:CheckBox ID="_displayAnswers" runat="server" />
        <br class="clear" />
    </asp:Panel>
</div>