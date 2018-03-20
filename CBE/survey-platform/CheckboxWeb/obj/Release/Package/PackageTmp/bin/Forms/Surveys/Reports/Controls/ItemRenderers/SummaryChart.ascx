<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryChart.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SummaryChart" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Register Src="~/Controls/Charts/PieGraph.ascx" TagName="PieGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/DoughnutGraph.ascx" TagName="DonutGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/BarGraph.ascx" TagName="BarGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/LineGraph.ascx" TagName="LineGraph" TagPrefix="ckbx" %> 
<%@ Register Src="~/Controls/Charts/ColumnGraph.ascx" TagName="ColumnGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagName="SummaryTable" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/ResponseCountTable.ascx" TagName="ResponseCountTable" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>
<%@ Register Src="~/Controls/Charts/OthersTable.ascx" TagPrefix="ckbx" TagName="OthersTable" %>

<div class="itemContent pageBreak" style="border-width: <%=BorderLineWidth%>px; border-color: <%=BorderLineColor%>; border-style:solid; margin:30px auto;">
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
    <div style="display:inline-block;margin-bottom:-1px;" runat="server" id="_statisticsPanel">
        <ckbx:ResponseCountTable ID="_responseCountTable" runat="server" />
    </div>
    <div class="clear" />
    <div style="display:inline-block;" runat="server" id="_answersPanel">
        <ckbx:SummaryTable ID="_summaryTable" runat="server" hasTitle="false"/>
    </div>
    <asp:Panel ID="_otherPanel" runat="server"> 
        <div style="padding:10px;">
            <ckbx:OthersTable ID="_othersTable" runat="server" />
        </div>
    </asp:Panel>
    <div class="clear" style="padding-bottom:5px;"/>
</div>


<script type="text/C#" runat="server">
    
    /// <summary>
    /// Width for the border
    /// </summary>
    protected int BorderLineWidth
    {
        get
        {
            return Utilities.AsInt(Appearance["BorderLineWidth"], 0);
        }
    }

    /// <summary>
    /// Color of the border
    /// </summary>
    protected string BorderLineColor
    {
        get
        {
            return ColorTranslator.ToHtml(Utilities.GetColor(Appearance["BorderLineColor"], false));
        }
    }


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
                
        //other options summary
        _otherPanel.Visible = "AggregateAndDisplay".Equals(Model.Metadata["OtherOption"], StringComparison.InvariantCultureIgnoreCase)
            || "Display".Equals(Model.Metadata["OtherOption"], StringComparison.InvariantCultureIgnoreCase);
        if (_otherPanel.Visible)
        {
            Model.Metadata["DataSource"] = "DetailedResults";
        }

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
            if (_otherPanel.Visible)
                children.Add(_othersTable);
            children.Add(_summaryTable);
            children.Add(_responseCountTable);
            
            return children;
        }
    }


</script>