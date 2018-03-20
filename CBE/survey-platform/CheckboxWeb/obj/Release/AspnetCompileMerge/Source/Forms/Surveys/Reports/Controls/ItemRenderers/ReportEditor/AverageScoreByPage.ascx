<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScoreByPage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.AverageScoreByPage" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/AverageScoreByPageItemConfig.ascx" TagPrefix="ckbx" TagName="AverageScoreByPageConfig" %>
<%@ Register Src="~/Controls/Charts/AverageScoreByPageGraph.ascx" TagPrefix="ckbx" TagName="AverageScoreByPageGraph" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/averageScoreByPageItemRenderer/pageSource") %></li>
    <li>
        <ol style="list-style-type:square;">
            <asp:Repeater ID="_sourcePageRepeater" runat="server">
                <ItemTemplate>
                    <li><span><asp:Label ID="_itemTxtLbl" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ReportingText")%>' /></span></li>
                    <div class="clear"></div>
                </ItemTemplate>
                </asp:Repeater>
        </ol>
        <div class="clear"></div>
    </li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ckbx:AverageScoreByPageConfig ID="_scoreConfig" runat="server" />

<div style="margin-top:15px;">
    <ckbx:AverageScoreByPageGraph ID="_scoreGraph" runat="server" />

     <asp:Panel ID="_filterPanel" runat="server" style="padding:10px;border:1px solid #999999;margin:10px 0;text-align:left;"> 
        <div>
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize renderer and children
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _scoreConfig.Initialize(Model);
        _sourcePageRepeater.DataSource = Model.SourcePages;
    }

    /// <summary>
    /// Bind score graph
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        _scoreGraph.InitializeAndBind(Model,  Appearance);
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }

</script>

