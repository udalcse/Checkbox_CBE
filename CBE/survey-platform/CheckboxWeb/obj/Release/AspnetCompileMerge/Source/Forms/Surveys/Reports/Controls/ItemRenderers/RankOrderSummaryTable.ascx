<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderSummaryTable.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.RankOrderSummaryTable" %>
<%@Import Namespace="Checkbox.Common" %>
<%@Import Namespace="Checkbox.Globalization.Text" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">

    <div style="text-align:center;margin:5px;">
        <asp:Label ID="_lblTitle" runat="server" CssClass="Question"></asp:Label>
    </div>

    <div style="text-align:center;">
        <asp:GridView EnableViewState="false" ID="_gridView" runat="server" CellPadding="2" AutoGenerateColumns="false" CssClass="matrix Matrix" GridLines="Both" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem" style="margin-left:auto;margin-right:auto;">
            <Columns>
                <grd:LocalizedHeaderBoundField DataField="ResultText" HeaderTextID="/controlText/summaryTable/answer" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
                <grd:LocalizedHeaderBoundField DataField="Points" DataFormatString="{0:f0}" HeaderTextID="/controlText/rankSummaryTable/totalScore" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Width="100" />
                <grd:LocalizedHeaderBoundField DataField="Rank" HeaderTextID="/controlText/rankSummaryTable/overallRank" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Width="100" />
            </Columns>
        </asp:GridView>
        <div>
            <table runat="server" id="_hintContainer" style="margin-left:auto;margin-right:auto;">
                <tr><td align="left"><ckbx:MultiLanguageLabel TextId="/controlText/rankSummaryTable/hint" runat="server" ID="_hint" style="text-align:left"/></td></tr>
            </table>
        </div>
    </div>

    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>


<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize grid/text
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        //Set title
        _lblTitle.Text = GetTitle();

        var width = Utilities.AsInt(Appearance["Width"]);

        if (width.HasValue)
        {
            _gridView.Width = Unit.Pixel(width.Value);
            _hintContainer.Width = _gridView.Width.ToString();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetGridViewWidth()
    {
        if (string.IsNullOrEmpty(Appearance["Width"]))
        {
            return "width:600px;";
        }

        var width = Utilities.AsInt(Appearance["Width"]);

        if (!width.HasValue)
        {
            return "width:600px;";
        }
        
        return string.Format("width:{0};", Unit.Pixel(Math.Max(width.Value, 200)));
        
    }

    class RankedResult
    {
        public string ResultText { get; set; }
        public double Points { get; set; }
        public int Rank { get; set; }        
    }
    
    /// <summary>
    /// Bind grid to model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        /*
        _gridView.DataSource = "Survey".Equals(Appearance["OptionsOrder"]) ? Model.AggregateResults : 
            Model.AggregateResults.OrderBy(r => r.ResultText).ToArray();
         */

        // Hide "Other" row in the table
        var dataSource =
            ("Display".Equals(Model.Metadata["OtherOption"], StringComparison.InvariantCultureIgnoreCase))
                ? Model.AggregateResults.Where(r => r.ResultText != "Other")
                : Model.AggregateResults;

        int rank = 0;
        var rankedDataSource = dataSource.OrderByDescending(r => r.Points).Select(r => new RankedResult() { Points = r.Points, ResultText = Utilities.DecodeAndStripHtml(r.ResultText), Rank = ++rank });
        
        _gridView.DataSource = !"Survey".Equals(Appearance["OptionsOrder"])
                             ? rankedDataSource.OrderBy(r => r.ResultText).ToArray()
                             : rankedDataSource;
        _gridView.DataBind();
        
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
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
                    sb.Append("<br />");
                    sb.Append(GetItemResponseCount(sourceItem.ItemId));
                    sb.Append(" ");
                    sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
                }
            }

            if (multiSource)
            {
                sb.Append("<br />");
            }
        }

        return sb.ToString();
    }
    
</script>
