<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.RankOrderSummary" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/PieGraph.ascx" TagName="PieGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/DoughnutGraph.ascx" TagName="DonutGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/BarGraph.ascx" TagName="BarGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/LineGraph.ascx" TagName="LineGraph" TagPrefix="ckbx" %> 
<%@ Register Src="~/Controls/Charts/ColumnGraph.ascx" TagName="ColumnGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:PieGraph ID="_pie" runat="server" UsePointsAsY="true" Visible="false" />
    <ckbx:DonutGraph ID="_donut" runat="server" UsePointsAsY="true" Visible="false" />
    <ckbx:BarGraph ID="_bar" runat="server" UsePointsAsY="true" Visible="false" />
    <ckbx:LineGraph ID="_line" runat="server" UsePointsAsY="true" Visible="false" />
    <ckbx:ColumnGraph ID="_column" runat="server" UsePointsAsY="true" Visible="false" />
        
    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>


<script type="text/C#" runat="server">
    
    /// <summary>
    /// Initialize proper chart according to graph type
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _pie.Visible = "piegraph".Equals(Appearance["GraphType"], StringComparison.InvariantCultureIgnoreCase);
        _donut.Visible = "doughnut".Equals(Appearance["GraphType"], StringComparison.InvariantCultureIgnoreCase);
        _bar.Visible = "bargraph".Equals(Appearance["GraphType"], StringComparison.InvariantCultureIgnoreCase);
        _line.Visible = "linegraph".Equals(Appearance["GraphType"], StringComparison.InvariantCultureIgnoreCase);

        //If no graph type, default to column
        _column.Visible = !_line.Visible && !_bar.Visible && !_donut.Visible && !_pie.Visible;

        if (_column.Visible)
        {
            Appearance["GraphType"] = "ColumnGraph";
        }

        //Store graph type on model
        Model.Metadata["GraphType"] = Appearance["GraphType"];

        _filterPanel.Width = Unit.Pixel(Utilities.AsInt(Appearance["Width"], 600));
    }

    /// <summary>
    /// Bind model to specific data
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        if (_pie.Visible)
        {
            _pie.InitializeAndBind(Model, Appearance);
        }

        if (_donut.Visible)
        {
            _donut.InitializeAndBind(Model, Appearance);
        }
        
        if(_column.Visible)
        {
            _column.InitializeAndBind(Model, Appearance);
        }
        
        if(_line.Visible)
        {
            _line.InitializeAndBind(Model, Appearance);
        }
        
        if(_bar.Visible)
        {
            _bar.InitializeAndBind(Model, Appearance);
        }
        
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }


</script>