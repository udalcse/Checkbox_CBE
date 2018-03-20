<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SummaryTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.SummaryTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Analytics.Configuration" %>

<div style="text-align:center;margin:5px;" runat="server" id="_labelContainer">
    <asp:Label ID="_lblTitle" runat="server" CssClass="Question"></asp:Label>
</div>

<div style="text-align:center;">
    <asp:GridView EnableViewState="false" ID="_gridView" runat="server" CellPadding="2" AutoGenerateColumns="false" CssClass="matrix Matrix" GridLines="Both" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem" style="margin-left:auto;margin-right:auto;width:600px;">
        <Columns>
            <grd:LocalizedHeaderTemplateField HeaderTextID="/controlText/summaryTable/answer" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# Eval("ResultText") %>'></asp:Label>
                </ItemTemplate>
            </grd:LocalizedHeaderTemplateField>
            <grd:LocalizedHeaderBoundField DataField="AnswerCount" HeaderTextID="/controlText/summaryTable/count" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Width="100" />
            <grd:LocalizedHeaderBoundField DataField="AnswerPercent" DataFormatString="{0:f2}%" HeaderTextID="/controlText/summaryTable/percent" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" ItemStyle-Width="100" />
        </Columns>
    </asp:GridView>
    <ckbx:MultiLanguageLabel runat="server" ID="_tableRowLimitForPdfPrintWarning" Visible="False" ></ckbx:MultiLanguageLabel>
</div>

<script type="text/C#" runat="server">

    public bool HasTitle
    {
        get;
        set;
    }
    /// <summary>
    /// Initialize grid/text
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        if (HasTitle)
        {
            //Set title
            _lblTitle.Text = GetTitle();
        }
        else
        {
            _labelContainer.Visible = false;
        }

        var width = Utilities.AsInt(Appearance["Width"]);

        if (width.HasValue)
        {
            _gridView.Width = Unit.Pixel(width.Value);
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
                ? Model.AggregateResults.Where(r => r.ResultText != "Other").ToArray()
                : Model.AggregateResults;


        if (ExportMode == ExportMode.Pdf)
        {
            var max = ((ReportPerformanceConfiguration)
                Prezza.Framework.Configuration.ConfigurationManager.
                    GetConfiguration("checkboxReportPerformanceConfiguration")).
                        MaxResponseDetailsItemRowsForPdfExport;

            if (dataSource.Length > max)
            {
                dataSource = dataSource.Take(max).ToArray();
                var warningText = TextManager.GetText("/itemText/tableRowLimitForPdfExport");
                _tableRowLimitForPdfPrintWarning.Text = string.Format(warningText, max);
                _tableRowLimitForPdfPrintWarning.Visible = true;
            }
        }

        foreach (var result in dataSource)
        {
            result.ResultText = Utilities.RemoveScript(Utilities.AdvancedHtmlDecode(result.ResultText));
        }

        _gridView.DataSource = "Survey".Equals(Appearance["OptionsOrder"])
                             ? dataSource
                             : dataSource.OrderBy(r => Utilities.AdvancedHtmlEncode(Utilities.DecodeAndStripHtml(r.ResultText))).ToArray();
        _gridView.DataBind();
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
            sb.Append(Utilities.AdvancedHtmlEncode((Utilities.DecodeAndStripHtml(sourceItem.ReportingText))));

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