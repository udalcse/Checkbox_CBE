<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TotalScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.TotalScore" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/TotalScoreConfig.ascx" TagName="ChartConfig" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/PieGraph.ascx" TagName="PieGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/DoughnutGraph.ascx" TagName="DonutGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/BarGraph.ascx" TagName="BarGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/LineGraph.ascx" TagName="LineGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/ColumnGraph.ascx" TagName="ColumnGraph" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagName="SummaryTable" TagPrefix="ckbx" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>

<ckbx:ChartConfig ID="_summaryConfig" runat="server" />
<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<div style="margin-top:15px;">
    <ckbx:PieGraph ID="_pie" runat="server" Visible="false" />
    <ckbx:DonutGraph ID="_donut" runat="server" Visible="false" />
    <ckbx:BarGraph ID="_bar" runat="server" Visible="false" />
    <ckbx:LineGraph ID="_line" runat="server" Visible="false" />
    <ckbx:ColumnGraph ID="_column" runat="server" Visible="false" />
    <ckbx:SummaryGrid ID="_table" runat="server" Visible="false" />
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
        _column.Visible = "columngraph".Equals(Appearance["GraphType"], StringComparison.InvariantCultureIgnoreCase);

        //If no graph type, default to table
        _table.Visible = !_line.Visible && !_bar.Visible && !_donut.Visible && !_pie.Visible && !_column.Visible;

        if (String.IsNullOrEmpty(Model.Metadata["GraphType"]))  // set default graphType if it's empty
        {
            Model.Metadata["GraphType"] = "summaryTable";
        }

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

        if (_table.Visible)
        {
            _table.SummaryTitle = GetTitle();


            _table.ClearColumns();
            _table.ShowFooter = false;
            _table.AddBoundColumn("ResultKey", WebTextManager.GetText("/controlText/totalScoreItemRenderer/score"), null, HorizontalAlign.Right, HorizontalAlign.Right);
            _table.AddBoundColumn("AnswerCount", WebTextManager.GetText("/controlText/totalScoreItemRenderer/scoreCount"), null, HorizontalAlign.Right, HorizontalAlign.Right);
            _table.DataSource = Model.AggregateResults;
            _table.DataBind();
        }
    }

    /// <summary>
    /// Get language code for text
    /// </summary>
    public string LanguageCode
    {
        get
        {
            return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                ? TextManager.DefaultLanguage
                : Model.InstanceData["LanguageCode"];
        }
    }

    /// <summary>
    /// Get response count for item
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public int GetItemResponseCount(int itemId)
    {
        var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

        return itemData != null ? itemData.ResponseCount : 0;
    }
    
    
    /// <summary>
    /// Get the chart title
    /// </summary>
    protected string GetTitle()
    {
        var sb = new StringBuilder();

        bool showResponseCount = "true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase);
        bool multiSource = Model.SourceItems.Length > 1;

        foreach (var sourceItem in Model.SourceItems)
        {
            sb.Append(sourceItem.ReportingText);

            if (showResponseCount)
            {
                if (multiSource)
                {
                    sb.Append("  ");
                    sb.Append(GetItemResponseCount(sourceItem.ItemId));
                    sb.Append(" ");
                    sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                }
                else
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(GetItemResponseCount(sourceItem.ItemId));
                    sb.Append(" ");
                    sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                }
            }

            if (multiSource)
            {
                sb.Append(Environment.NewLine);
            }
        }

        return sb.ToString();
    }

</script>
