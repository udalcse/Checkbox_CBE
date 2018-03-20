<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryChart.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.SummaryChart" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/SummaryChartConfig.ascx" TagName="ChartConfig" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/PieGraph.ascx" TagName="PieGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/DoughnutGraph.ascx" TagName="DonutGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/BarGraph.ascx" TagName="BarGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/LineGraph.ascx" TagName="LineGraph" TagPrefix="ckbx" %> 
<%@ Register Src="~/Controls/Charts/ColumnGraph.ascx" TagName="ColumnGraph" TagPrefix="ckbx" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagName="SummaryTable" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/ResponseCountTable.ascx" TagName="ResponseCountTable" TagPrefix="ckbx" %>

<div class="">
    <ckbx:ChartConfig id="_summaryConfig" runat="server" />
    <ckbx:BasicConfig id="_basicConfig" runat="server" />
</div>

<div style="margin-top:15px;">
    <ckbx:PieGraph ID="_pie" runat="server" Visible="false" />
    <ckbx:DonutGraph ID="_donut" runat="server" Visible="false" />
    <ckbx:BarGraph ID="_bar" runat="server" Visible="false" />
    <ckbx:LineGraph ID="_line" runat="server" Visible="false" />
    <ckbx:ColumnGraph ID="_column" runat="server" Visible="false" />
        
    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
    <div style="margin: 0 auto;width:600px">
        <div style="display:inline-block;margin-bottom:-1px;" runat="server" id="_statisticsPanel">
            <ckbx:ResponseCountTable ID="_responseCountTable" runat="server" />
        </div>
        <div class="clear" />
        <div style="display:inline-block;" runat="server" id="_answersPanel">
            <ckbx:SummaryTable ID="_summaryTable" runat="server" hasTitle="false"/>
        </div>
        <div class="clear" style="padding-bottom:5px;"/>
    </div>
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
       
        //Bind configs
        _basicConfig.Initialize(Model);
        _summaryConfig.Initialize(Model);

        _statisticsPanel.Visible = "True".Equals(Model.Metadata["DisplayStatistics"]);
        _answersPanel.Visible = "True".Equals(Model.Metadata["DisplayAnswers"]);
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

    public override List<Checkbox.Web.Forms.UI.Rendering.UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var children = base.ChildUserControls;
            children.Add(_summaryTable);
            children.Add(_responseCountTable);

            return children;
        }
    }    
</script>