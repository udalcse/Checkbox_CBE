<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTabTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.CrossTabTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<table cellspacing="1" cellpadding="2" class="matrix Matrix" style="<%=GetHeightAndWidth() %>;margin-left:auto;margin-right:auto;">
    <caption><%=GetTableCaption() %></caption>

    <colgroup span="1"></colgroup>

    <asp:Repeater ID="_colGroupRepeater" runat="server">
        <ItemTemplate>
            <colgroup span="<%#GetItemHeaderCellSpan((int)Container.DataItem) %>"></colgroup>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <colgroup span="<%#GetItemHeaderCellSpan((int)Container.DataItem) %>"></colgroup>
        </AlternatingItemTemplate>
    </asp:Repeater>

    <thead>
        <tr class="header">
            <th>&nbsp;</th>

            <asp:Repeater ID="_xItemRepeater" runat="server">
                <ItemTemplate>
                    <th colspan="<%# GetItemHeaderCellSpan((int)Container.DataItem) %>"><%# GetItemText((int)Container.DataItem) %></th>
                </ItemTemplate>
            </asp:Repeater>
        </tr>

        <tr class="header">
            <th>&nbsp;</th>

            <asp:Repeater ID="_xItemOptionRepeater" runat="server">
                <ItemTemplate>
                    <th style="font-weight:normal;"><%# Utilities.StripHtmlAndEncode(Container.DataItem.ToString()) %></th>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
    </thead>

    <tbody>    
        <asp:Repeater ID="_yItemRepeater" runat="server">
            <ItemTemplate>
                    <tr class="subheader">
                        <td><%#  DataBinder.Eval(Container.DataItem, "ItemText") %></td>
                        <td colspan="<%# GetSubHeaderColSpan() %>">&nbsp;</td>
                    </tr>

                    <asp:Repeater ID="_yOptionRepeater" runat="server" DataSource="<%#((YAxisItemBindingObject)Container.DataItem).Options %>">
                        <ItemTemplate>
                            <tr class="Item">
                                <td><%# Utilities.StripHtmlAndEncode(((YAxisOptionBindingObject)Container.DataItem).OptionText)%></td>

                                <asp:Repeater ID="_yResultRepeater" runat="server" DataSource="<%#((YAxisOptionBindingObject)Container.DataItem).Results %>">
                                    <ItemTemplate>
                                        <td>
                                            <div class="crossTabCell">
                                                <%# FormatResult((AggregateResult)Container.DataItem) %>
                                            </div>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="AlternatingItem">
                                <td><%# Utilities.StripHtmlAndEncode(((YAxisOptionBindingObject)Container.DataItem).OptionText) %></td>

                                <asp:Repeater ID="_yResultRepeater" runat="server" DataSource="<%#((YAxisOptionBindingObject)Container.DataItem).Results %>">
                                    <ItemTemplate>
                                        <td>
                                            <div class="crossTabCell">
                                                <%# FormatResult((AggregateResult)Container.DataItem) %>
                                            </div>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
    
</table>


<script type="text/C#" runat="server">
    /// <summary>
    /// Simple object to use for binding data to items on y axis
    /// </summary>
    protected class YAxisItemBindingObject
    {
        public string ItemText { get; set; }
        public List<YAxisOptionBindingObject> Options { get; set; }
    }
    
    /// <summary>
    /// Simple object for binding data to options on y axis
    /// </summary>
    protected class YAxisOptionBindingObject
    {
        public string OptionText { get; set; }
        public List<AggregateResult> Results { get; set; }
    }
    
    /// <summary>
    /// Transform data and bind model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _colGroupRepeater.DataSource = XAxisItemIds;
        _colGroupRepeater.DataBind();

        _xItemRepeater.DataSource = XAxisItemIds;
        _xItemRepeater.DataBind();

        _xItemOptionRepeater.DataSource = GetXAxisOptionTexts();
        _xItemOptionRepeater.DataBind();

        _yItemRepeater.DataSource = GetYAxisItemDataSource();
        _yItemRepeater.DataBind();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public string FormatResult(AggregateResult result)
    {
        var showZeroValues = Utilities.AsBool(Appearance["ShowDataLabelZeroValues"], false);
        bool showAnswerCount = Utilities.AsBool(Appearance["ShowAnswerCount"], true);
        bool showPercent = Utilities.AsBool(Appearance["ShowPercent"], true);

        var resultString = new StringBuilder();

        if (!showZeroValues && result.AnswerCount == 0)
        {
            return "-";
        }

        if (showAnswerCount)
        {
            resultString.Append("<span class=\"crossTabAnswerCount\">");
            resultString.Append(result.AnswerCount);
            resultString.Append("</span>");

            if (showPercent)
            {
                resultString.Append("<i>(");
            }
        }

        if (showPercent)
        {
            resultString.Append(string.Format("{0:f2}%", result.AnswerPercent));

            if (showAnswerCount)
            {
                resultString.Append(")</i>");
            }
            
            
        }

        return resultString.ToString();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected int GetSubHeaderColSpan()
    {
        return GetXAxisOptionTexts().Count;
    }
    
    /// <summary>
    /// Get data source for YAxisItems
    /// </summary>
    /// <returns></returns>
    protected List<YAxisItemBindingObject> GetYAxisItemDataSource()
    {
        return YAxisItemIds.Select(
            yAxisItemId => 
                new YAxisItemBindingObject
                {
                    ItemText = GetItemText(yAxisItemId), 
                    Options = GetYAxisItemOptionsDataSource(yAxisItemId)
                }
        ).ToList();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected List<YAxisOptionBindingObject> GetYAxisItemOptionsDataSource(int yAxisItemId)
    {
        return ListItemOptions(yAxisItemId).Select(
            option => 
                new YAxisOptionBindingObject
                {
                    OptionText = option.ReportingText,
                    Results = GetOptionResultsRowDataSource(option.OptionId)
                }
        ).ToList();
    }
    
    /// <summary>
    /// Get result row data source for options
    /// </summary>
    /// <param name="optionId"></param>
    /// <returns></returns>
    protected List<AggregateResult> GetOptionResultsRowDataSource(int optionId)
    {
        return GetXAxisOptions().Select(xAxisOption => GetResult(xAxisOption.OptionId, optionId)).ToList();
    }
    
    /// <summary>
    /// Get results
    /// </summary>
    /// <param name="xAxisOptionId"></param>
    /// <param name="yAxisOptionId"></param>
    /// <returns></returns>
    protected AggregateResult GetResult(int xAxisOptionId, int yAxisOptionId)
    {
        return
            Model.AggregateResults.FirstOrDefault(
                result => result.ResultKey == string.Format("{0}_{1}", xAxisOptionId, yAxisOptionId))
            ?? new AggregateResult();
    }
    
    /// <summary>
    /// Get a list of X-Axis option ids
    /// </summary>
    /// <returns></returns>
    protected List<ReportItemSourceOptionData> GetXAxisOptions()
    {
        var options = new List<ReportItemSourceOptionData>();
        
        //Loop through items
        foreach (var itemOptions in XAxisItemIds.Select(ListItemOptions))
        {
            if (itemOptions.Length == 0)
            {
              // it will throw an exception if 'ReportingText' is null
              //TODO : need to understand how cross tab should work with non-options items
             //   options.Add(new ReportItemSourceOptionData { OptionId = -1 });
            }
            else
            {
                options.AddRange(itemOptions);
            }
        }

        return options;
    }
    
    /// <summary>
    /// Get texts for x-axis items
    /// </summary>
    /// <returns></returns>
    protected List<string> GetXAxisOptionTexts()
    {
        return GetXAxisOptions().Select(option => Utilities.DecodeAndStripHtml(option.ReportingText)).ToList();
    }
    
    /// <summary>
    /// Get Ids of X-Axis items
    /// </summary>
    protected int[] XAxisItemIds
    {
        get { return ConvertCsvToIntArray(Model.InstanceData["XAxisItemIds"]); }
    }

    /// <summary>
    /// Get Ids of Y-Axis items
    /// </summary>
    protected int[] YAxisItemIds
    {
        get { return ConvertCsvToIntArray(Model.InstanceData["YAxisItemIds"]); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    protected int[] ConvertCsvToIntArray(string csv)
    {
        if (string.IsNullOrEmpty(csv))
        {
            return new int[] { };
        }

        var stringItemIds = csv.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

        var itemIds = new List<int>();

        foreach (var stringItemId in stringItemIds)
        {
            int itemId;

            if (int.TryParse(stringItemId, out itemId))
            {
                itemIds.Add(itemId);
            }
        }

        return itemIds.ToArray();
    }

    /// <summary>
    /// Attempt to get text of an item from model
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected string GetItemText(int itemId)
    {
        var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);
        return itemData != null ? Utilities.StripHtmlAndEncode((itemData.ReportingText)) : string.Empty;
    }

    /// <summary>
    /// Attempt to get text of an item from model
    /// </summary>
    /// <returns></returns>
    protected string GetTableCaption()
    {
        return string.Empty;
    }

    /// <summary>
    /// Get count of number of options for an item
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected int GetItemHeaderCellSpan(int itemId)
    {
        var itemOptionList = ListItemOptions(itemId);

        return itemOptionList.Length > 0
            ? itemOptionList.Length
            : 1;
    }
    
    /// <summary>
    /// List options for an item.
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected ReportItemSourceOptionData[] ListItemOptions(int itemId)
    {
        var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

        return itemData != null && itemData.Options != null && itemData.Options.Any() ? itemData.Options : new ReportItemSourceOptionData[] { };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetHeightAndWidth()
    {
        var theValue = string.Empty;

        if (!string.IsNullOrEmpty(Appearance["Height"]))
        {
            theValue = string.Format("height:{0}px;", Appearance["Height"]);
        }

        if (!string.IsNullOrEmpty(Appearance["Width"]))
        {
            theValue += string.Format("width:{0}px;", Appearance["Width"]);
        }

        return theValue;
    }
    
</script>