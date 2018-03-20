<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Analysis_Total_Score.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Total_Score" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Panel ID="_chartTypePanel" runat="server" CssClass="styleSectionContainer">
    <div class="field_100">
        <%= WebTextManager.GetText("/pageText/styles/charts/graphOptions.ascx/chartType")%></div>
    <div class="input">
        <ckbx:MultiLanguageDropDownList ID="_graphTypeList" runat="server">
            <asp:ListItem TextId="/enum/graphType/summaryTable" Value="SummaryTable" />
            <asp:ListItem TextId="/enum/graphType/BarGraph" Value="BarGraph" />
            <asp:ListItem TextId="/enum/graphType/ColumnGraph" Value="ColumnGraph" />
            <asp:ListItem TextId="/enum/graphType/Doughnut" Value="Doughnut" />
            <asp:ListItem TextId="/enum/graphType/LineGraph" Value="LineGraph" />
            <asp:ListItem TextId="/enum/graphType/PieGraph" Value="PieGraph" />
        </ckbx:MultiLanguageDropDownList>
    </div>
    <div class="clear">
    </div>
</asp:Panel>
