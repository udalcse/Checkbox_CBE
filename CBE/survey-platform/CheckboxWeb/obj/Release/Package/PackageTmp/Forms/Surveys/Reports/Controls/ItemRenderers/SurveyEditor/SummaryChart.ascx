<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryChart.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SurveyEditor.SummaryChart" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/BasicConfiguration.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/SummaryChartConfig.ascx" TagName="ChartConfig" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/PieGraph.ascx" TagName="PieGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/DoughnutGraph.ascx" TagName="DonutGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/BarGraph.ascx" TagName="BarGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/LineGraph.ascx" TagName="LineGraph" TagPrefix="ckbx" %> 
<%@ Register Src="~/Controls/Charts/ColumnGraph.ascx" TagName="ColumnGraph" TagPrefix="ckbx" %>

<ckbx:ChartConfig id="_summaryConfig" runat="server" />

<ckbx:BasicConfig id="_basicConfig" runat="server" />

<ckbx:PieGraph ID="_pie" runat="server" Visible="false" />
<ckbx:DonutGraph ID="_donut" runat="server" Visible="false" />
<ckbx:BarGraph ID="_bar" runat="server" Visible="false" />
<ckbx:LineGraph ID="_line" runat="server" Visible="false" />
<ckbx:ColumnGraph ID="_column" runat="server" Visible="false" />


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

        if (_column.Visible)
        {
            _column.InitializeAndBind(Model, Appearance);
        }

        if (_line.Visible)
        {
            _line.InitializeAndBind(Model, Appearance);
        }

        if (_bar.Visible)
        {
            _bar.InitializeAndBind(Model, Appearance);
        }
    }

</script>