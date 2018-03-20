<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSummaryTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.MatrixSummaryTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<table class="matrix Matrix" cellpadding="2">
    <caption><%=GetTitle() %></caption>

    <colgroup span="1"></colgroup>
    <thead>
        <tr class="columnHeader header">
            <th class="BorderRight BorderTop" >&nbsp;</th>

            <asp:Repeater ID="_columnRepeater" runat="server">
                <ItemTemplate>
                    <th class="BorderRight BorderTop" colspan="<%# GetColumnHeaderSpan((int)Container.DataItem) %>"><%# GetColumnHeaderTitle((int)Container.DataItem)%></th>
                </ItemTemplate>
            </asp:Repeater>
        </tr>

        <tr>
            <th class="BorderRight BorderTop" style="width:<%=ColumnWidth%>%">&nbsp;</th>
            <asp:Repeater ID="_columnOptionsRepeater" runat="server">
                <ItemTemplate>
                    <asp:Repeater ID="_optionsRepeater" runat="server" DataSource="<%# GetColumnOptionTexts((int)Container.DataItem) %>">
                        <ItemTemplate>
                            <th style="width:<%=ColumnWidth%>%;font-weight:normal;vertical-align:bottom;" class="BorderTop BorderRight"><div style="padding:2px;"><%# DataBinder.Eval(Container.DataItem, "Text") %></div></th>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="_rowsRepeater" runat="server">
            <ItemTemplate>
                <tr class="Item">
                    <td align="left" colspan="<%# GetColumnSpanForHeaderRow((int)Container.DataItem) %>" style="font-weight:bold;" class="BorderTop BorderRight">
                        <%# GetRowText((int)Container.DataItem) %>
                    </td>
                    <asp:Repeater ID="_innerColumnRepeater" runat="server" DataSource="<%# GetRowResults(Container.ItemIndex + 1) %>">
                        <ItemTemplate>
                            <%# GetColumnMarkup((RowBindingObject)Container.DataItem, DataBinder.Eval(Container.DataItem, "Text") as string)%>
                        </ItemTemplate>
                    </asp:Repeater>            
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="AlternatingItem">
                    <td align="left" colspan="<%# GetColumnSpanForHeaderRow((int)Container.DataItem) %>" style="font-weight:bold;" class="BorderTop BorderRight">
                        <%# GetRowText((int)Container.DataItem) %>
                    </td>
                    <asp:Repeater ID="_innerColumnRepeater" runat="server" DataSource="<%# GetRowResults(Container.ItemIndex + 1) %>">
                        <ItemTemplate>
                            <%# GetColumnMarkup((RowBindingObject)Container.DataItem, DataBinder.Eval(Container.DataItem, "Text") as string)%>
                        </ItemTemplate>
                    </asp:Repeater>            
                </tr>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </tbody>
    
</table>


<script type="text/C#" runat="server">
    
    /// <summary>
    /// Databinding object
    /// </summary>
    protected class RowBindingObject
    {
        public string Text { get; set; }
        public bool IsLastOption { get; set; }
        public bool IsHeader { get; set; }
        public int RowNumber { get; set; }
    }

    /// <summary>
    /// Transform data and bind model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        if (Model.AdditionalData is MatrixSummaryAdditionalData)
        {

            _columnRepeater.DataSource = ((MatrixSummaryAdditionalData) Model.AdditionalData).ColumnSourceItems;
            _columnRepeater.DataBind();

            _columnOptionsRepeater.DataSource = ((MatrixSummaryAdditionalData) Model.AdditionalData).ColumnSourceItems;
            _columnOptionsRepeater.DataBind();

            _rowsRepeater.DataSource = ((MatrixSummaryAdditionalData) Model.AdditionalData).RowSourceItems.Select(d => d.Key);
            _rowsRepeater.DataBind();
        }
    }

    /// <summary>
    /// Get the chart title
    /// </summary>
    protected virtual string GetTitle()
    {
        var sb = new StringBuilder();

        bool showResponseCount = "true".Equals(Appearance["ShowResponseCountInTitle"],
                                               StringComparison.InvariantCultureIgnoreCase);
        
        int? sourceItemId = Utilities.AsInt(Model.Metadata["MatrixSourceItem"]);

        if (!sourceItemId.HasValue)
        {
            return string.Empty;
        }

        var sourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == sourceItemId.Value);

        if (sourceItem == null)
        {
            return string.Empty;
        }

        sb.AppendFormat("<b>{0}</b>", sourceItem.ReportingText);
        sb.Append("<br>");

        if (showResponseCount)
        {
            sb.Append(Environment.NewLine);
            sb.Append(GetItemResponseCount(sourceItem.ItemId));
            sb.Append(" ");
            sb.Append(TextManager.GetText("/controlText/analysisItemRenderer/responses", LanguageCode));
        }
        
        return sb.ToString();
    }

    /// <summary>
    /// Get language code for text
    /// </summary>
    protected string LanguageCode
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
    protected int GetItemResponseCount(int itemId)
    {
        var itemData = Model.SourceItems.FirstOrDefault(item => item.ItemId == itemId);

        return itemData != null ? itemData.ResponseCount : 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetColumnMarkup(RowBindingObject rbo, string text)
    {
        if (rbo.IsHeader)
            return string.Empty;
        
        string cellStyle = GetCellStyle(rbo);

        const string markup = "<td align=\"center\" class=\"{0}\">{1}</td>";

        return string.Format(markup, cellStyle, text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnItemId"></param>
    /// <returns></returns>
    protected int GetColumnHeaderSpan(int columnItemId)
    {
        //If item can't be found, don't how
        var columnSourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == columnItemId);
        
        if(columnSourceItem == null)
        {
            return 0;
        }
        
        //If item has no options, assume sum total and return 1
        if (columnSourceItem.Options.Length == 0)
        {
            return 1;
        }
        
        //Otherwise, check type since we need to add an extra column for radio button scales
        if ("RadioButtonScale".Equals(columnSourceItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
        {
            return columnSourceItem.Options.Length + 1;
        }
        
        return columnSourceItem.Options.Length;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowItemId"></param>
    /// <returns></returns>
    protected int GetColumnSpanForHeaderRow(int rowItemId)
    {
        var additionalData = Model.AdditionalData as MatrixSummaryAdditionalData;

        //if there is header row, return full length
        if (additionalData != null && additionalData.RowSourceItems.ContainsKey(rowItemId) &&
            additionalData.RowSourceItems[rowItemId])
        {
            //each rating scale has additional result column, lets count them
            int ratingScales = Model.SourceItems.Count(si => "RadioButtonScale".Equals(si.ItemType, StringComparison.InvariantCultureIgnoreCase));
            
            return Model.SourceItems.Where(si =>
                additionalData.ColumnSourceItems.Contains(si.ItemId)).SelectMany(si => si.Options).Count() + ratingScales + 1;
        }
        
        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnItemId"></param>
    /// <returns></returns>
    protected string GetColumnHeaderTitle(int columnItemId)
    {
        //If item can't be found, don't how
        var columnSourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == columnItemId);

        if (columnSourceItem == null)
        {
            return string.Empty;
        }

        return columnSourceItem.ReportingText;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnItemId"></param>
    /// <returns></returns>
    protected List<RowBindingObject> GetColumnOptionTexts(int columnItemId)
    {
        var optionTexts = new List<RowBindingObject>();
        
        //If item can't be found, don't how
        var columnSourceItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == columnItemId);

        if (columnSourceItem == null)
        {
            return optionTexts;
        }

        //If item has no options, assume sum total and return "Average"
        if (columnSourceItem.Options.Length == 0)
        {
            optionTexts.Add(new RowBindingObject { IsLastOption = true, Text = TextManager.GetText("/controlText/matrixSummaryItem/sumTotalAverage", LanguageCode, "Average Value") });
            return optionTexts;
        }

        //Add options
        optionTexts.AddRange(
            columnSourceItem
                .Options
                .Select(option => new RowBindingObject { Text = Utilities.DecodeAndStripHtml(option.ReportingText) }));
        
        //Ensure last option marked
        if (optionTexts.Count > 0)
        {
            optionTexts[optionTexts.Count - 1].IsLastOption = true;
        }
        
        //Add average for rating scale
        if ("RadioButtonScale".Equals(columnSourceItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
        {
            optionTexts.Add(new RowBindingObject { IsLastOption = true, Text = TextManager.GetText("/controlText/matrixSummaryItem/ratingScaleAverage", LanguageCode, "Average Rating") });
        }

        return optionTexts;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rbo"></param>
    /// <returns></returns>
    protected string GetCellStyle(RowBindingObject rbo)
    {
        var style = string.Empty;

        //if (rbo.RowNumber == 1)
        //{
            style = "BorderTop ";
        //}

        //if (rbo.IsLastOption)
        //{
            style += "BorderRight";
        //}

        return style;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    protected List<RowBindingObject> GetRowResults(int rowIndex)
    {
        var results = new List<RowBindingObject>();
        var additionalData = Model.AdditionalData as MatrixSummaryAdditionalData;

        if (additionalData == null)
        {
            return results;
        }

        //if there is header row, return the result
        if (additionalData.RowSourceItems.Count > rowIndex - 1 &&
            additionalData.RowSourceItems.ElementAt(rowIndex - 1).Value)
        {
            results.Add(new RowBindingObject{ IsHeader = true });
            return results;
        }

        var matrixItems = additionalData.MatrixChildren;

        if (matrixItems == null)
        {
            return results;
        }
        
        //
        for (int columnIndex = 1; columnIndex <= additionalData.ColumnSourceItems.Length; columnIndex++)
        {
            //Column source items contains only columns shown in summary item, so item index may not match up with actual
            // index in the matrix.  Look this up so we can get ids of child items based on position.
            int? childActualColumn = Utilities.AsInt(additionalData.ColumnItemPositions[additionalData.ColumnSourceItems[columnIndex - 1].ToString()]);

            if (!childActualColumn.HasValue)
            {
                continue;
            }

            int? childId = Utilities.AsInt(matrixItems[rowIndex + "_" + childActualColumn]);

            if (!childId.HasValue)
            {
                results.Add(new RowBindingObject{IsLastOption = true, Text = string.Empty, RowNumber =  rowIndex});
                continue;
            }

            var childItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == childId.Value);

            if (childItem == null || "Message".Equals(childItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            //Add average for rating scale
            if ("Slider".Equals(childItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
            {
                results.Add(new RowBindingObject { IsLastOption = true, Text = additionalData.SliderAverages[childId.ToString()], RowNumber = rowIndex });
                continue;
            }
            
            //If item has no options, assume sum total and return "Average"
            if (childItem.Options.Length == 0)
            {
                results.Add(new RowBindingObject { IsLastOption = true, Text = additionalData.SumTotalAverages[childId.ToString()], RowNumber =  rowIndex });
                continue;
            }

            //Otherwise, handle option
            var optionResults = 
                childItem
                    .Options
                    .Select(sourceOptionData => Model.AggregateResults.FirstOrDefault(result => result.ResultKey == string.Format("{0}_{1}", childId, sourceOptionData.OptionId)))
                    .Select(optionResult => new RowBindingObject{Text = optionResult != null ? FormatResult(optionResult) : string.Empty, RowNumber =  rowIndex})
                    .ToList();

            //Ensure last option is marked
            if (optionResults.Count > 0)
            {
                optionResults[optionResults.Count - 1].IsLastOption = true;
            }
            
            //Add average for rating scale
            if ("RadioButtonScale".Equals(childItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
            {
                optionResults.Add(new RowBindingObject { IsLastOption = true, Text = additionalData.RatingScaleAverages[childId.ToString()], RowNumber =  rowIndex });
            }

            if (optionResults.Count > 0)
            {
                results.AddRange(optionResults);
            }
        }

        return results;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected string FormatResult(AggregateResult result)
    {
        var showZeroValues = Utilities.AsBool(Appearance["ShowDataLabelZeroValues"], false);
        bool showAnswerCount = Utilities.AsBool(Appearance["ShowAnswerCount"], true);
        bool showPercent = Utilities.AsBool(Appearance["ShowPercent"], true);

        var resultString = new StringBuilder();

        if (!showZeroValues && result.AnswerCount == 0)
        {                                
            return string.Format("<span class=\"crossTabAnswerCount\">0</span><i><br>({0:f2}%)</i>", 0.0);
        }

        if (showAnswerCount)
        {
            resultString.Append("<span class=\"crossTabAnswerCount\">");
            resultString.Append(result.AnswerCount);
            resultString.Append("</span>");

            if (showPercent)
            {
                resultString.Append("<i><br />(");
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
    /// Get text of row.
    /// </summary>
    /// <param name="rowItemId"></param>
    /// <returns></returns>
    protected string GetRowText(int rowItemId)
    {
        //TODO: Handle "Subheader" rows
        var rowItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == rowItemId);

        return rowItem != null
                   ? Utilities.DecodeAndStripHtml(rowItem.ReportingText)
                   : string.Empty;
    }

    int _ColumnWidth;
    /// <summary>
    /// Returns a column width
    /// </summary>
    protected int ColumnWidth
    {
        get
        {
            if (_ColumnWidth == 0)
            {
                _ColumnWidth = calculateColumnWidth();
            }
            return _ColumnWidth;
        }
    }

    /// <summary>
    /// Calculates column width
    /// </summary>
    /// <returns></returns>
    private int calculateColumnWidth()
    {
        int ColCount = 1;
        if ("true".Equals(Appearance["ShowResponseCountInTitle"], StringComparison.InvariantCultureIgnoreCase))
            ColCount++;

        var results = new List<RowBindingObject>();

        if (Model.AdditionalData == null || !(Model.AdditionalData is MatrixSummaryAdditionalData))
        {
            return 50;
        }

        var additionalData = (MatrixSummaryAdditionalData)Model.AdditionalData;

        var matrixItems = additionalData.MatrixChildren;

        if (matrixItems == null)
        {
            return 50;
        }

        //
        for (int columnIndex = 1; columnIndex <= additionalData.ColumnSourceItems.Length; columnIndex++)
        {
            //Column source items contains only columns shown in summary item, so item index may not match up with actual
            // index in the matrix.  Look this up so we can get ids of child items based on position.
            int? childActualColumn = Utilities.AsInt(additionalData.ColumnItemPositions[additionalData.ColumnSourceItems[columnIndex - 1].ToString()]);

            if (!childActualColumn.HasValue)
            {
                continue;
            }

            int? childId = Utilities.AsInt(matrixItems[1 + "_" + childActualColumn]);

            if (!childId.HasValue)
            {
                continue;
            }

            var childItem = Model.SourceItems.FirstOrDefault(item => item.ItemId == childId.Value);

            if (childItem == null || "Message".Equals(childItem.ItemType, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            //If item has no options, assume sum total and return "Average"
            if (childItem.Options.Length == 0)
            {
                ColCount++;
                continue;
            }


            //Ensure last option is marked
            ColCount += childItem.Options.Length;
        }

        return ColCount > 100 ? 1 : 100 / ColCount;
    }
    
    /*

    int _rowCount = -1;
    protected int RowCount
    {
        get
        {
            if (_rowCount == -1)
                _rowCount = Model.InstanceData["RowCount"] == null ? 0 : int.Parse(Model.InstanceData["RowCount"]);
            return _rowCount;
        }
    }


    int _colCount = -1;
    protected int ColCount
    {
        get
        {
            if (_colCount == -1)
                _colCount = Model.InstanceData["ColCount"] == null ? 0 : int.Parse(Model.InstanceData["ColCount"]);
            return _colCount;
        }
    }

    List<ColumnBindingObject> _columnsDataSource;
    
    /// <summary>
    /// Builds a columns info list
    /// </summary>
    protected List<ColumnBindingObject> ColumnsDataSource
    {
        get
        {
            if (_columnsDataSource != null)
                return _columnsDataSource;

            _columnsDataSource = new List<ColumnBindingObject>(ColCount);
            
            string[] columns = Model.InstanceData["ColumnItems"].Split('|');

            foreach (string column in columns)
            {
                if (string.IsNullOrEmpty(column))
                    continue;
                string[] columnParams = column.Split(';');
                var obj = new ColumnBindingObject
                              {
                                  ID = int.Parse(columnParams[1]),
                                  ItemText = MatrixSummaryItem.DecodeString(columnParams[2])
                              };

                string[] options = columnParams[3].Split(',');
                foreach (string option in options)
                {
                    if (string.IsNullOrEmpty(option))
                        continue;
                    string[] optionParams = option.Split('^');
                    var optionObj = new OptionObject { ID = int.Parse(optionParams[0]), Text = MatrixSummaryItem.DecodeString(optionParams[1]) };
                    obj.Options.Add(optionObj);
                }
                _columnsDataSource.Add(obj);
            }

            return _columnsDataSource;
        }
    }

    List<RowBindingObject> _rowsDataSource;
    /// <summary>
    /// Builds a rows info list
    /// </summary>
    protected List<RowBindingObject> RowsDataSource
    {
        get
        {
            if (_rowsDataSource != null)
                return _rowsDataSource;

            _rowsDataSource = new List<RowBindingObject>(RowCount);
            string[] rows = Model.InstanceData["RowTexts"].Split('|');
            foreach (string row in rows)
            {
                if (string.IsNullOrEmpty(row))
                    continue;

                string[] rowParams = row.Split(',');

                //parse items
                string[] rowCells = rowParams[2].Split(';');
                var cells = new List<string>();
                int i = 0;
                foreach (string cell in rowCells)
                {
                    if (string.IsNullOrEmpty(cell))
                        continue;

                    if (i > 0 && i < ColumnsDataSource.Count && !cell.Equals(rowParams[0])) //bypass table row names
                    {
                        cells.AddRange(ColumnsDataSource[i - 1].Options.Select(option => cell + "_" + option.ID));
                    }
                    i++;
                }

                var rowObject = new RowBindingObject { RowText = MatrixSummaryItem.DecodeString(rowParams[1]), RowID = int.Parse(rowParams[0]), Cells = cells };

                _rowsDataSource.Add(rowObject);
            }

            return _rowsDataSource;
        }
    }

    protected AggregateResult GetAnalysisResult(string key)
    {
        return
            Model.AggregateResults.FirstOrDefault(
                result => result.ResultKey == key)
            ?? new AggregateResult();
    }    */
</script>