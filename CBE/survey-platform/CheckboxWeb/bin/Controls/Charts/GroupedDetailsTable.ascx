<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GroupedDetailsTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.GroupedDetailsTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Checkbox.Analytics.Configuration" %>

<style type="text/css">
    #<%=_theGrid.ClientID%> table
    {
        margin-left:auto;
        margin-right:auto;
        border:1px solid #999999;
        width:<%=Appearance["Width"]%>px;
    }
    #<%=_theGrid.ClientID%> th {padding:3px;border-left:1px solid #999999;background-color:#DEDEDE;}
    #<%=_theGrid.ClientID%> tr {background-color:#fff;}
    #<%=_theGrid.ClientID%> tr:hover {background-color:#f3f3f3;}
    #<%=_theGrid.ClientID%> td {padding:3px;border-left:1px solid #999999;border-top:1px solid #999999;}
    #<%=_theGrid.ClientID%> td.detailLink{width:16px;}
</style>

<asp:GridView ID="_theGrid" runat="server" CssClass="matrix Matrix" HeaderStyle-CssClass="header" RowStyle-HorizontalAlign="Left" AlternatingRowStyle-CssClass="AlternatingItem" RowStyle-CssClass="Item" AutoGenerateColumns="false" ShowFooter="false" Style="margin:auto;">
    <Columns>
        <asp:TemplateField HeaderText="" ItemStyle-Width="25px" >             
            <ItemTemplate>
                <asp:Literal ID="_imageLink" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<ckbx:MultiLanguageLabel runat="server" ID="_tableRowLimitForPdfPrintWarning" Visible="False" ></ckbx:MultiLanguageLabel>


<script type="text/C#" runat="server">

    /// <summary>
    /// Get whether Include Response Details option is enabled or not
    /// </summary>
    public bool IncludeResponseDetails
    {
        get { return "true".Equals(Model.Metadata["IncludeResponseDetails"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Show Page Numbers option is enabled or not
    /// </summary>
    public bool ShowPageNumbers
    {
        get { return "true".Equals(Model.Metadata["ShowPageNumbers"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Include Message/HTML Items option is enabled or not
    /// </summary>
    public bool IncludeMessageItems
    {
        get { return "true".Equals(Model.Metadata["IncludeMessageItems"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _theGrid.RowDataBound += _theGrid_RowDataBound;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _theGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row == null || e.Row.RowType != DataControlRowType.DataRow)
        {
            return;
        }

        var literal = e.Row.FindControl("_imageLink") as Literal;

        if (literal == null)
        {
            return;
        }

        String responseGuid = (e.Row.DataItem as DataRowView)[0].ToString();

        if (!String.IsNullOrEmpty(responseGuid))
        {
            literal.Text = string.Format("<a style='vertical-align:middle;' href='" +
                    ResolveUrl("~/Forms/Surveys/Responses/View.aspx?responseGuid={0}&showMessages={1}&showPageNumbers={2}&includeDetails={3}") +
                    "' target='_blank'><img src='" + ResolveUrl("~/App_Themes/CheckboxTheme/Images/details16.gif") + "' /></a>",
                    responseGuid, IncludeMessageItems.ToString(), ShowPageNumbers.ToString(), IncludeResponseDetails.ToString());
        }
        else
        {
            literal.Text = "<a style='vertical-align:middle;' href='javascript:void(0);'><img src='" + ResolveUrl("~/App_Themes/CheckboxTheme/Images/details16.gif") + "' /></a>";
        }
    }

    /// <summary>
    /// Get whether answers are grouped by response or not
    /// </summary>
    public bool LinkToResponseDetails
    {
        get { return "true".Equals(Model.Metadata["LinkToResponseDetails"], StringComparison.InvariantCultureIgnoreCase); }
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
    /// Initialize grid with configuration data
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        AddColumns();
    }

    /// <summary>
    /// Bind model to data
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _theGrid.DataSource = GetDataSource();
        _theGrid.DataBind();

        SetWidth();
    }

    /// <summary>
    /// Get the proper data source for the grid.
    /// </summary>
    /// <returns></returns>
    protected DataTable GetDataSource()
    {
        DecodeOptions();

        DataTable result = new DataTable();
        result.Columns.Add("ResponseGuid");

        foreach (var t in Model.SourceItems)
        {
            result.Columns.Add(t.ItemId.ToString());
        }

        int maxRows = Model.GroupedDetailResults.Length;
        if (ExportMode == ExportMode.Pdf)
        {
            var max = ((ReportPerformanceConfiguration)
                Prezza.Framework.Configuration.ConfigurationManager.
                    GetConfiguration("checkboxReportPerformanceConfiguration")).
                        MaxResponseDetailsItemRowsForPdfExport;

            if (maxRows > max)
            {
                maxRows = max;
                var warningText = TextManager.GetText("/itemText/tableRowLimitForPdfExport");
                _tableRowLimitForPdfPrintWarning.Text = string.Format(warningText, max);
                _tableRowLimitForPdfPrintWarning.Visible = true;
            }
        }

        for (int i=0; i < maxRows; i++)
        {
            var aggregateResult = Model.GroupedDetailResults[i];

            DataRow row = result.NewRow();

            if (aggregateResult.GroupResults.Length > 0)
                row["ResponseGuid"] = aggregateResult.GroupResults[0].ResponseGuid;

            foreach (DetailResult detailResult in aggregateResult.GroupResults)
            {
                row[detailResult.ItemId.ToString()] = Utilities.RemoveScript(Utilities.AdvancedHtmlDecode(detailResult.ResultText));
            }

            result.Rows.Add(row);
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DecodeOptions()
    {
        foreach (var aggregateResult in Model.GroupedDetailResults)
        {
            foreach (var result in aggregateResult.GroupResults)
            {
                string text = result.ResultText;
                text = Utilities.AdvancedHtmlDecode(text);
                text = Utilities.EncodeTagsInHtmlContent(text);
                result.ResultText = text;
            }
        }
    }

    /// <summary>
    /// Add columns to grid
    /// </summary>
    private void AddColumns()
    {
        _theGrid.Columns[0].Visible = LinkToResponseDetails;

        foreach (var t in Model.SourceItems
            .OrderBy(si => si.PagePosition)
            .ThenBy(si => si.ItemPosition)
            .ThenBy(si => si.ParentColumnNumber)
            .ThenBy(si => si.ParentRowNumber))
        {
            _theGrid.Columns.Add(
                new BoundField
                {
                    DataField = t.ItemId.ToString(),
                    HeaderText = Utilities.DecodeAndStripHtml(t.ReportingText),
                    HtmlEncode = false
                }
                );
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected string GetItemText(int itemId)
    {
        var sourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

        return sourceItem != null ? sourceItem.ReportingText : string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected void SetWidth()
    {
        if (!string.IsNullOrEmpty(Appearance["Width"]))
        {
            int width;

            if (int.TryParse(Appearance["Width"], out width))
            {
                _theGrid.Width = Unit.Pixel(width);
            }
        }
    }

</script>