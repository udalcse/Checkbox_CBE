<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.Matrix" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyEditor/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Forms.Data" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Forms.Logic.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Prezza.Framework.ExceptionHandling" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Forms.Items.UI" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Prezza.Framework.Logging" %>

<script type="text/javascript">
    //Register callback
    $(document).ready(function(){
        templateEditor.registerCallback('dialogCallback_editItemConditions', onMatrixRowConditionsEdited);
    });

    var _rowID = 0;
    //Edit row condition click handler
    function onEditRowConditionsClick(id) {
        _rowID = id;
        templateEditor.openChildWindow(id, <%=GetPageNumber() %>, 'ItemConditions.aspx', null, 'xlargeDialog');
    }

    //Row condition edited handler
    function onMatrixRowConditionsEdited(args){
        //rerequest original page to load rules
        $.ajax({
            url: '<%=Request.Url.ToString()%>',
            context: document.body
        });

        $('[rowid=' + _rowID + ']').attr('src', '<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/")%>' + (args.hasConditions ? 'conditions_edit_16.gif' : 'conditions_add_16.gif'));

    }

</script>
<div style="display: none">
    <asp:Button ID="_submitButton" runat="server" Text="" />
</div>
<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>
        <div style="max-width: 800px; overflow: auto;">
            <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
                <asp:Panel ID="_inputPanel" runat="server">
                    <table id='<%=ClientID + "_matrix" %>' class="matrix" <%#String.IsNullOrEmpty(Appearance["Width"])? String.Empty : "style='width:" + Appearance["Width"]+"px'" %>>
                        <asp:Label ID="_MainHeadercontrolsPlace" runat="server" ForeColor="Black" Font-Bold="true" Font-Size="Large" />
                        <thead>
                            <%-- Column Text --%>
                            <tr class="header">
                                <td class="columnHeader" rowspan="<%=ContainsRatingScaleColumn ? 3 : (ContainsRadioButtonsColumn || ContainsCheckBoxesColumn || ContainsSliderColumn) ? 2 : 1 %>">
                                    <%-- Conditions Placeholder  --%>
                                &nbsp;
                            </td>
                                <%-- Input Columns --%>
                                <asp:Repeater ID="_columnHeaderRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound="ColumnHeaderRepeater_ItemDataBound">
                                    <ItemTemplate>
                                        <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                        <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                            <%-- Scale Texts --%>
                            <asp:PlaceHolder ID="_scalePlaceholder" runat="server" Visible="<%# ContainsRatingScaleColumn %>">
                                <tr valign="bottom" class="Answer scaleText">
                                    <asp:Repeater ID="_columnScaleTextsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemCreated="OnScaleTextItemCreated">
                                        <ItemTemplate>

                                            <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                            <asp:PlaceHolder ID="_scaleTextsPlace" runat="server" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                            </asp:PlaceHolder>
                            <%-- Column Options --%>
                            <asp:PlaceHolder ID="_columnOptionsPlaceholder" runat="server" Visible="<%#ContainsRatingScaleColumn || ContainsRadioButtonsColumn || ContainsCheckBoxesColumn || ContainsSliderColumn %>">
                                <tr class="Answer">
                                    <asp:Repeater ID="_columnOptionsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound="ColumnOptionsRepeater_OnItemDataBound">
                                        <ItemTemplate>
                                            <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                            <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                            </asp:PlaceHolder>
                        </thead>
                        <tbody class="matrixBody">
                            <%-- Empty row --%>
                            <asp:PlaceHolder ID="_emptyRow" runat="server" Visible="<%#MatrixItem.Rows.Length==0%>">
                                <tr>
                                    <td colspan="<%#MatrixItem.Columns.Length + 1%>">No Rows have been added to the matrix.
                                </td>
                                </tr>
                            </asp:PlaceHolder>
                            <%-- Rows --%>
                            <asp:Repeater ID="_rowRepeater" runat="server" DataSource='<%#MatrixItem.Rows %>'>
                                <ItemTemplate>
                                    <tr class='<%#GetRowClass(((MatrixItemRow) Container.DataItem).RowNumber)%>'>
                                        <td class=" <% if (_showConditions)
                                                   { %> rowConditions <% } %> <%=GetGridLinesClass("Conditions") %>">
                                            <% if (_showConditions)
                                           { %>
                                            <asp:ImageButton uframeignore="true" ID="_editConditionsBtn" runat="server" ImageUrl='<%#GetConditionImageUrl((MatrixItemRow)Container.DataItem) %>' ToolTip='<%#GetTitleTextForConditionImage((MatrixItemRow)Container.DataItem) %>' OnClientClick='<%#GetClickRowConditionsHandler((MatrixItemRow)Container.DataItem)%>' rowId="<%#GetChildItemId(((MatrixItemRow)Container.DataItem).RowNumber, MatrixItem.LabelColumnIndex)%>" />
                                            <% } %>
                                    </td>
                                        <asp:Repeater ID="_columnRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound="MatrixChildItemCreated">
                                            <ItemTemplate>
                                                <asp:PlaceHolder ID="_childControlsPlace" runat="server" />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </asp:Panel>
            </asp:Panel>
        </div>

    </asp:Panel>
</asp:Panel>
<script type="text/C#" runat="server">

    private Dictionary<int, AppearanceData> _appearanceCache;
    private ResponseTemplate _responseTemplate;
    private RuleDataService _ruleDataService;
    private bool _showAsterisks;
    private bool _showConditions = true;

    /// <summary>
    /// Get response template which contains this item
    /// </summary>
    public ResponseTemplate ResponseTemplate
    {
        get
        {
            if (_responseTemplate == null)
            {
                _responseTemplate = ResponseTemplateManager.GetResponseTemplate(Model.ParentTemplateId);

                if (_responseTemplate == null)
                {
                    throw new Exception(string.Format("Unable to load survey with id {0}.", Model.ParentTemplateId));
                }
            }
            return _responseTemplate;
        }
    }

    /// <summary>
    /// Get rule data service.
    /// </summary>
    public RuleDataService RuleDataService
    {
        get { return _ruleDataService ?? (_ruleDataService = ResponseTemplate.CreateRuleDataService()); }
    }


    /// <summary>
    /// Prevent multiple loading of appearance data for each row/column of a matrix
    /// </summary>
    public Dictionary<int, AppearanceData> AppearanceCache
    {
        get { return _appearanceCache ?? (_appearanceCache = new Dictionary<int, AppearanceData>()); }
    }

    /// <summary>
    /// Get appearance of item
    /// </summary>
    /// <param name="itemId"></param>
    public AppearanceData GetItemAppearance(int itemId)
    {
        if (AppearanceCache.ContainsKey(itemId))
        {
            return AppearanceCache[itemId];
        }

        AppearanceData appearanceData = AppearanceDataManager.GetAppearanceDataForItem(itemId);

        if (appearanceData != null)
        {
            AppearanceCache[itemId] = appearanceData;
        }

        return appearanceData;
    }


    /// <summary>
    /// 
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var childControls = base.ChildUserControls;
            childControls.Add(_questionText);
            return childControls;
        }
    }


    /// <summary>
    /// Add necessary javascript library reference
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DataBind();
    }

    /// <summary>
    /// Get id of matrix child at row/column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    protected int GetChildItemId(int row, int column)
    {
        if (MatrixItem.ChildItems.Length >= row
            && MatrixItem.ChildItems[row - 1].Length >= column)
        {
            return MatrixItem.ChildItems[row - 1][column - 1].ItemId;
        }

        return -1;
    }


    /// <summary>
    /// Initialize child user controls to set label and item positions
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        SetLabelPosition();
        SetItemPosition();
    }

    /// <summary>
    /// Get page number of page containing this item
    /// </summary>
    /// <returns></returns>
    protected int GetPageNumber()
    {
        return ResponseTemplate.GetPagePositionForItem(Model.ItemId) ?? 0;
    }


    /// <summary>
    /// CSS name for row header of the matrix
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <returns></returns>
    protected string GetRowClass(int rowNumber)
    {
        if (MatrixItem.Rows[rowNumber - 1].RowType == RowType.Subheading.ToString())
        {
            return "subheader";
        }

        if (rowNumber % 2 == 0)
            return "Item";

        return "AlternatingItem";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetGridLinesClass(string columnType)
    {
        //Set border for conditions cells
        if ("Conditions".Equals(columnType, StringComparison.InvariantCultureIgnoreCase))
        {
            return ("Both".Equals(Appearance["GridLines"], StringComparison.InvariantCultureIgnoreCase) ||
                    "Horizontal".Equals(Appearance["GridLines"], StringComparison.InvariantCultureIgnoreCase))
                       ? "BorderTop"
                       : string.Empty;
        }

        if ("Vertical".Equals(Appearance["GridLines"], StringComparison.InvariantCultureIgnoreCase))
            return "BorderRight";

        if ("Horizontal".Equals(Appearance["GridLines"], StringComparison.InvariantCultureIgnoreCase))
            return "BorderTop";

        return "Both".Equals(Appearance["GridLines"], StringComparison.InvariantCultureIgnoreCase)
            ? "BorderBoth"
            : String.Empty;
    }

    /// <summary>
    /// Get matrix item
    /// </summary>
    public MatrixAdditionalData MatrixItem
    {
        get { return Model.AdditionalData as MatrixAdditionalData; }
    }


    /// <summary>
    /// Determine if matrix consist ratingScale column
    /// </summary>
    public bool ContainsRatingScaleColumn
    {
        get
        {
            return
                MatrixItem.Columns.Any(
                    column =>
                    column.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <summary>
    /// Determine if matrix consist radioButtons column
    /// </summary>
    public bool ContainsRadioButtonsColumn
    {
        get
        {
            return
                MatrixItem.Columns.Any(
                    column => column.ColumnType.Equals("RadioButtons", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <summary>
    /// Determine if matrix consist slider column
    /// </summary>
    public bool ContainsSliderColumn
    {
        get
        {
            return MatrixItem.Columns.Any(column => column.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <summary>
    /// Determine if matrix consist checkboxes column
    /// </summary>
    public bool ContainsCheckBoxesColumn
    {
        get
        {
            return
                MatrixItem.Columns.Any(
                    column => column.ColumnType.Equals("CheckBoxes", StringComparison.InvariantCultureIgnoreCase));
        }
    }


    /// <summary>
    /// Get all columns count except primaryKeyColumn and some additional columns (e.g. "AddColumn")
    /// </summary>
    public int AllColumnsCount
    {
        get
        {
            //Add 1 for row texts.

            int result = 1;
            foreach (MatrixItemColumn column in MatrixItem.Columns)
            {
                if (column.ColumnNumber != MatrixItem.LabelColumnIndex)
                {
                    if (column.ColumnType.Equals("DropDownList", StringComparison.InvariantCultureIgnoreCase) || column.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
                        result++;
                    else
                        result += Math.Max(column.OptionTexts.Length, 1);
                }
            }

            return result;
        }
    }


    /// <summary>
    /// CSS name for header columns of the matrix
    /// </summary>
    protected string GridLineClassForHeader
    {
        get
        {
            if (Appearance["GridLines"] == "Both" || Appearance["GridLines"] == "Vertical")
                return "BorderRight";
            return String.Empty;
        }
    }

    /// <summary>
    /// Determine if row has conditions or not.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    private bool HasRowConditions(MatrixItemRow row)
    {
        int itemId = GetChildItemId(row.RowNumber, MatrixItem.LabelColumnIndex);
        return RuleDataService.ItemHasCondition(itemId);
    }


    /// <summary>
    /// Get conditionImageUrl
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    protected String GetConditionImageUrl(MatrixItemRow row)
    {
        string url = "~/App_Themes/CheckboxTheme/Images/" + (HasRowConditions(row)
                         ? "conditions_edit_16.gif"
                         : "conditions_add_16.gif");
        return url;
    }

    /// <summary>
    /// Get alternative text for the specified row
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    protected String GetTitleTextForConditionImage(MatrixItemRow row)
    {
        if (HasRowConditions(row))
            return WebTextManager.GetText("/controlText/matrixItemEditor/editRowConditions");

        return WebTextManager.GetText("/controlText/matrixItemEditor/addRowConditions");
    }

    /// <summary>
    /// Get handler for row condition click event. Handler is jScript code.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    protected String GetClickRowConditionsHandler(MatrixItemRow row)
    {
        return String.Format("onEditRowConditionsClick({0});return false;",
                             GetChildItemId(row.RowNumber, MatrixItem.LabelColumnIndex));
    }

    /// <summary>
    /// Handle creation of matrix scale texts
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnScaleTextItemCreated(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            var columnInfo = GetColumnInfo(e);

            var scaleTextsPlace = e.Item.FindControl("_scaleTextsPlace") as PlaceHolder;

            if (scaleTextsPlace == null)
            {
                throw new Exception("Unable to locate scale texts placeholder.");
            }

            //Make sure we have row info, and that column prototype information is available
            if (columnInfo == null)
            {
                throw new Exception("Unable to get column information.");
            }

            //Do nothing if not a radio button scale
            if (!"RadioButtonScale".Equals(columnInfo.ColumnType, StringComparison.InvariantCultureIgnoreCase))
            {
                scaleTextsPlace.Visible = false;
                return;
            }

            var prototypeItemData = SurveyMetaDataProxy.GetItemData(columnInfo.PrototypeItemId, true);

            if (prototypeItemData == null)
            {
                throw new Exception("Unable to get column prototype.");
            }

            var optionCount = prototypeItemData.AllowOther
                                 ? prototypeItemData.Options.Count - 1
                                 : prototypeItemData.Options.Count;

            //Find na text, which is last option text for column, if it exists
            var naOptionText = prototypeItemData.AllowOther
                               ? columnInfo.OptionTexts.LastOrDefault()
                               : null;

            Unit optionWidth;
            Unit naOptionWidth;

            //Calculate widths
            int startSpan;
            int endSpan;
            int midSpan;
            CalculateScaleContainerWidths(optionCount, columnInfo.ScaleStartText, columnInfo.ScaleMidText, columnInfo.ScaleEndText, naOptionText,
                                          columnInfo.Width, out optionWidth, out naOptionWidth, out startSpan, out midSpan, out endSpan);
            //Start index is cell containing scale start text.  Always item at index 0
            var startIndex = 0;

            //Middle index and span depends on whether there are odd-or even number of options.
            var midIndex = midSpan > 0 ? (optionCount - midSpan) / 2 : -1;

            //End index is cell containing end text.  Text does not go over NA option, so
            // adjust index accordingly.
            var endIndex = optionCount - endSpan;

            //Now add cells
            for (int index = 0; index < optionCount; index++)
            {
                var cell = new TableCell { CssClass = "columnHeader", HorizontalAlign = HorizontalAlign.Center };
                var panel = new Panel();
                var literal = new LiteralControl();

                if (index == startIndex)
                {
                    literal.Text = columnInfo.ScaleStartText;
                    cell.ColumnSpan = startSpan;
                    panel.HorizontalAlign = (cell.ColumnSpan > 1) ? HorizontalAlign.Left : HorizontalAlign.Center;
                    index += startSpan - 1;
                }

                if (index == endIndex)
                {
                    literal.Text = columnInfo.ScaleEndText;
                    cell.CssClass += " " + GridLineClassForHeader;
                    cell.ColumnSpan = endSpan;
                    panel.HorizontalAlign = (cell.ColumnSpan > 1) ? HorizontalAlign.Right : HorizontalAlign.Center;
                    index += endSpan - 1;
                }

                if (index == midIndex)
                {
                    literal.Text = columnInfo.ScaleMidText;
                    cell.ColumnSpan = midSpan;
                    index += midSpan - 1;
                }

                panel.Width = (int)optionWidth.Value * cell.ColumnSpan;

                cell.Controls.Add(panel);
                panel.Controls.Add(literal);

                scaleTextsPlace.Controls.Add(cell);
            }

            //Add cell for n/a option, if necessary
            if (prototypeItemData.AllowOther)
            {

                var cell = new TableCell { CssClass = "columnHeader BorderRight" };
                var panel = new Panel { Width = naOptionWidth };
                var literal = new LiteralControl { Text = "&nbsp;" };

                cell.Controls.Add(panel);
                panel.Controls.Add(literal);

                scaleTextsPlace.Controls.Add(cell);
            }
        }
        catch (Exception ex)
        {
            //Ensure that exception gets logged
            ExceptionPolicy.HandleException(ex, "UIProcess");

            //Show error message
            var childControlsPlace = e.Item.FindControl("_scaleTextsPlace") as PlaceHolder;

            if (childControlsPlace != null)
            {
                var cell = new TableCell();
                cell.Controls.Add(CreateErrorPanel(ex.Message));
                childControlsPlace.Controls.Add(cell);
            }
        }
    }

    /// <summary>
    /// Calculate width of option header text
    /// </summary>
    /// <returns></returns>
    protected void CalculateScaleContainerWidths(int optionCount, string startText, string midText, string endText, string naText,
                                                 int? columnWidth, out Unit optionWidth, out Unit naOptionWidth, out int startSpan, out int midSpan, out int endSpan)
    {
        //Header text container width needs to be set so that it is large enough
        // to contain all text and span all options except the N/A option (if any).
        //So, the width will be the greater of a fixed value * (number of options) or 
        // another fixed value * (number of characters in largest word in scale text).

        const int pixelsPerOption = 30;
        const int pixelsPerLetter = 8;

        //Get max of lengths of longest words for start/mid/end text so that we can
        // ensure word will fit w/out need for wrapping in middle of word.
        int[] maxWordLength = new[]
            {
                Utilities.IsNotNullOrEmpty(startText) ? startText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(midText) ? midText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(endText) ? endText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1
            };

        var startWidth = maxWordLength[0] * pixelsPerLetter;
        var midWidth = maxWordLength[1] * pixelsPerLetter;
        var endWidth = maxWordLength[2] * pixelsPerLetter;

        // Divide columnSpans between headers so that midHeader will be at the center
        // and make the whole scale to have minimal possible width
        var midHalfWidth = midWidth / 2.0;
        var boundaryWidth = Math.Max(startWidth, endWidth);

        if (optionCount <= 2)
            midSpan = 0;
        else
            if (optionCount == 3)
            midSpan = 1;
        else
        {
            double optionHalfCount = optionCount / 2.0;
            midSpan = (int)Math.Ceiling(midHalfWidth * optionHalfCount / (midHalfWidth + boundaryWidth) * 2 + 0.5);
            if (midSpan % 2 != optionCount % 2)
                midSpan++;
            if (midSpan == 0)
                midSpan = (optionCount % 2 == 1) ? 1 : 2;
        }
        startSpan = (optionCount - midSpan) / 2;
        endSpan = startSpan;
        if (startSpan == 0)
            startSpan = 1;

        //Min header text size (in pixels)
        int minHeaderTextSize = new[]
            {
                (startSpan > 0) ? startWidth / startSpan : 0,
                (midSpan > 0) ? midWidth / midSpan : 0,
                (endSpan > 0) ? endWidth / endSpan : 0
            }.Max() * optionCount;
        int minOptionSize = optionCount * pixelsPerOption;
        int naOptionSize = (Utilities.IsNotNullOrEmpty(naText) ? naText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1) * pixelsPerLetter;

        int containerWidth = Math.Max(minOptionSize, minHeaderTextSize);

        //Divide container width by number of non-other options to set option width,
        // which is necessary in cases where start/mid/end texts require more room
        // than options.
        optionWidth = Unit.Pixel(containerWidth / Math.Max(optionCount, 1));
        naOptionWidth = Unit.Pixel(Math.Max(naOptionSize, Math.Max((int)optionWidth.Value, 1)));

        //Finally, check to see if option width specified in appearance
        if (columnWidth.HasValue && columnWidth.Value > 0)
        {
            //Null indicates NO na option, vs empty text which could indicate option exists, but has no text.
            optionWidth = naText == null
                              ? Unit.Pixel(columnWidth.Value / optionCount)
                              : Unit.Pixel(columnWidth.Value / (optionCount + 1));

            naOptionWidth = optionWidth;
        }
        if ((int)(startWidth / optionWidth.Value) == 0)
            startSpan = 1;
        if ((optionCount > 1) && ((int)(endWidth / optionWidth.Value) == 0))
            endSpan = 1;
    }


    /// <summary>
    /// Handle matrix child created to instantiate and insert matrix item renderer controls
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void MatrixChildItemCreated(object sender, RepeaterItemEventArgs e)
    {
        try
        {
            //First, figure out coordinate of item in matrix by getting row and column info objects
            MatrixItemRow rowInfo = GetRowInfo(e);
            MatrixItemColumn columnInfo = GetColumnInfo(e);

            //Do nothing if this is a subheading row and we are not in the label column
            if (rowInfo.RowType == RowType.Subheading.ToString() && columnInfo.ColumnNumber != MatrixItem.LabelColumnIndex)
            {
                return;
            }


            int rowIndex = rowInfo.RowNumber - 1;
            int columnIndex = columnInfo.ColumnNumber - 1;

            //Make sure valid row/column
            if (rowIndex < 0
                || columnIndex < 0
                || rowIndex > MatrixItem.ChildItems.Length - 1
                || MatrixItem.ChildItems[rowIndex] == null
                || columnIndex > MatrixItem.ChildItems[rowIndex].Length - 1)
            {
                throw new Exception("Row or column position is not valid.  Row Index = " + rowIndex + " and column index = " + columnIndex);
            }

            //Get child item
            IItemProxyObject childItemDto = MatrixItem.ChildItems[rowIndex][columnIndex];


            //Make sure child is not null
            if (childItemDto == null
                || !(childItemDto is SurveyResponseItem))
            {
                return;
            }

            if ((childItemDto as SurveyResponseItem).Metadata != null)
            {
                bool.TryParse((childItemDto as SurveyResponseItem).Metadata["showAsterisks"], out _showAsterisks);
            }

            AppearanceData itemAppearance = null;

            if (childItemDto.TypeName == "Message" && childItemDto.AdditionalData != null)
            {
                itemAppearance = childItemDto.AdditionalData as AppearanceData;
            }

            if (itemAppearance == null)
            {
                itemAppearance = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? GetItemAppearance(childItemDto.ItemId)
                    : GetItemAppearance(columnInfo.PrototypeItemId);

                if (itemAppearance == null)
                {
                    //If no appearance use default for type
                    var itemTypeId = ItemConfigurationManager.GetTypeIdFromName(childItemDto.TypeName);

                    if (!itemTypeId.HasValue)
                    {
                        throw new Exception("Unable to type information for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex + ". Type = [" + childItemDto.TypeName + "].");
                    }

                    itemAppearance = AppearanceDataManager.GetDefaultAppearanceDataForType(itemTypeId.Value);

                    if (itemAppearance == null)
                    {
                        throw new Exception("Unable to get appearance information for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
                    }
                }
            }


            //Get the renderer for the column prototype
            Control itemRenderer = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                ? WebItemRendererManager.GetItemRenderer(childItemDto.ItemId, itemAppearance, RenderMode.Survey)
                : WebItemRendererManager.GetItemRenderer(columnInfo.PrototypeItemId, itemAppearance, RenderMode.Survey);

            if (itemRenderer == null && RenderMode == RenderMode.SurveyEditor)
            {
                itemRenderer = WebItemRendererManager.GetItemRenderer(childItemDto.ItemId, itemAppearance, RenderMode.Survey, true);
                _showConditions = false;
            }

            if (itemRenderer == null)
            {
                throw new Exception("Unable to get renderer for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
            }

            //Override control name if renderer supports            
            itemRenderer.ID = string.Format("Matrix_{0}_{1}", rowInfo.RowNumber, columnInfo.ColumnNumber);

            //Set the additional properties of renderer
            if (itemRenderer is MatrixChildrensItemRenderer)
            {
                var matrixRenderer = itemRenderer as MatrixChildrensItemRenderer;
                matrixRenderer.ChildType = columnIndex == 0 ? MatrixChildType.RowText : MatrixChildType.OrdinaryChild;
                matrixRenderer.ColumnWidth = columnInfo.Width;
                matrixRenderer.GridLineMode = Appearance["GridLines"];
                matrixRenderer.RowTextPosition = Appearance["RowTextPosition"];
            }


            //Initialize the renderer.  If renderer is user control based,
            // override to use matrix-specific controls
            if (itemRenderer is UserControlHostItemRenderer)
            {
                ((UserControlHostItemRenderer)itemRenderer).ControlNameOverride = "Matrix_" + childItemDto.TypeName;

                ((IItemRenderer)itemRenderer).Initialize(childItemDto, null, RenderMode.Survey);

                //copy appearance from the prototype item
                if (childItemDto.AppearanceData == null && itemAppearance != null)
                {
                    foreach (string key in itemAppearance.Properties.Keys)
                    {
                        ((UserControlHostItemRenderer)itemRenderer).Appearance[key] = itemAppearance[key];
                    }
                }

                if (columnInfo.Width.HasValue)
                    ((UserControlHostItemRenderer)itemRenderer).Width = Unit.Pixel(columnInfo.Width.Value);
                else if (!columnInfo.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
                    ((UserControlHostItemRenderer)itemRenderer).Width = Unit.Percentage(Math.Truncate(100.00 / AllColumnsCount));

                ((IItemRenderer)itemRenderer).BindModel();
            }

            //Add renderer to page, surrounded by table cell tag
            var childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;

            // Modified On 02-21-2018; Modified By Udal Bharti

            if (rowInfo.RowType == RowType.Subheading.ToString())
            {
                _MainHeadercontrolsPlace.Text = "";
                //for subheadings Text value should be taken from MatrixItem.ChildItems because they are already with merged @@[properties]
                if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                {


                    _MainHeadercontrolsPlace.Text = Utilities.IsHtmlFormattedText(rowInfo.Text) ? Utilities.EncodeTagsInHtmlContent(Utilities.AdvancedHtmlDecode(rowInfo.Text)) : Utilities.AdvancedHtmlEncode(rowInfo.Text);


                }
            }


            if (childControlsPlace != null)
            {
                childControlsPlace.Controls.Clear();
                if (rowInfo.RowType != RowType.Subheading.ToString())
                {

                    childControlsPlace.Controls.Add(itemRenderer);
                }
            }
            
            ImageButton _editConditionsBtn = (ImageButton)e.Item.NamingContainer.NamingContainer.FindControl("_editConditionsBtn");
            if (_editConditionsBtn != null)
            {
                if (rowInfo.RowType != RowType.Subheading.ToString())
                {
                    _editConditionsBtn.Visible = true;


                }
                else
                {
                    _editConditionsBtn.Visible = false;
                }
            }










            //Modification End

            #region Old Code
            //if (childControlsPlace != null)
            //{
            //    childControlsPlace.Controls.Clear();
            //    if (rowInfo.RowType == RowType.Subheading.ToString())
            //    {
            //        if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
            //        {
            //            var cell = new TableCell
            //            {
            //                Text = Utilities.IsHtmlFormattedText(rowInfo.Text) ? Utilities.EncodeTagsInHtmlContent(Utilities.AdvancedHtmlDecode(rowInfo.Text)) : Utilities.AdvancedHtmlEncode(rowInfo.Text),
            //                ColumnSpan = AllColumnsCount,
            //                VerticalAlign = VerticalAlign.Middle
            //            };
            //            childControlsPlace.Controls.Add(cell);
            //        }
            //    }
            //    else
            //    {
            //        childControlsPlace.Controls.Add(itemRenderer);
            //    }
            //}
            #endregion
        }
        catch (Exception ex)
        {
            //Ensure that exception gets logged
            ExceptionPolicy.HandleException(ex, "UIProcess");

            //Show error message
            var childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;

            if (childControlsPlace != null)
            {
                var cell = new TableCell();
                cell.Controls.Add(CreateErrorPanel(ex.Message));
                childControlsPlace.Controls.Add(cell);
            }
        }
    }


    /// <summary>
    /// Create an error panel
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static Panel CreateErrorPanel(string message)
    {
        var p = new Panel { CssClass = "error" };
        p.Controls.Add(new LiteralControl(message));

        return p;
    }

    /// <summary>
    /// Get a matrix row info from a matrix child repeater
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private static MatrixItemRow GetRowInfo(RepeaterItemEventArgs e)
    {
        //First, make sure items are not null and that we can navigate
        // back to the row repeater's RepeaterItem.
        if (e.Item == null
            || e.Item.Parent == null
            || e.Item.Parent.Parent == null
            || !(e.Item.Parent.Parent is RepeaterItem))
        {
            return null;
        }

        //Now make sure data items are correct type
        if (!(((RepeaterItem)e.Item.Parent.Parent).DataItem is MatrixItemRow))
        {
            return null;
        }

        //Otherwise, return coordinate
        return (MatrixItemRow)((RepeaterItem)e.Item.Parent.Parent).DataItem;
    }

    /// <summary>
    /// Get a matrix coordinate from a matrix child repeater
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private static MatrixItemColumn GetColumnInfo(RepeaterItemEventArgs e)
    {
        //First, make sure items are not null and that we can navigate
        // back to the row repeater's RepeaterItem.
        if (e.Item == null
            || !(e.Item.DataItem is MatrixItemColumn))
        {
            return null;
        }

        //Otherwise, return coordinate
        return (MatrixItemColumn)e.Item.DataItem;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ColumnOptionsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
        var controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

        if (matrixItemColumn == null
            || controlsPlace == null
            || matrixItemColumn.ColumnNumber == MatrixItem.LabelColumnIndex)
        {
            return;
        }

        //Handle slider item in the specific way
        if (matrixItemColumn.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
        {
            var sliderItem =
                ItemConfigurationManager.GetConfigurationData(
                    MatrixItem.Columns[matrixItemColumn.ColumnNumber - 1].PrototypeItemId) as SliderItemData;

            if (sliderItem.ValueType == SliderValueType.NumberRange)
            {
                var cell = new TableCell
                {
                    CssClass = "columnHeader " + GridLineClassForHeader,
                    Text =
                        String.Format(
                            "<span style='float:left;'>{0}</span><span style='float:right;'>{1}</span>",
                            sliderItem.MinValue, sliderItem.MaxValue)
                };
                controlsPlace.Controls.Add(cell);
            }

            if (sliderItem.ValueType != SliderValueType.NumberRange)
            {
                if (matrixItemColumn.OptionTexts.Length > 1)
                {
                    //Insert a table in the current TD. Table is needed here to make TDs with equal 'width' property.
                    TableCell cell = new TableCell
                    {
                        CssClass = "columnHeader " + GridLineClassForHeader,
                        ColumnSpan = matrixItemColumn.OptionTexts.Length - 1
                    };

                    Table table = new Table
                    {
                        Width = new Unit("100%")
                    };

                    table.Rows.Add(new TableRow());

                    cell.Controls.Add(table);
                    controlsPlace.Controls.Add(cell);

                    double width = 100.0 / (matrixItemColumn.OptionTexts.Length - 1);

                    TableCell tableCell = null;
                    for (int i = 0; i < matrixItemColumn.OptionTexts.Length - 1; i++)
                    {
                        string optionText = matrixItemColumn.OptionTexts[i];
                        tableCell = new TableCell
                        {
                            CssClass = "columnHeader",
                            HorizontalAlign = HorizontalAlign.Left,
                            Text = GetOptionText(optionText),
                            Width = new Unit(width, UnitType.Percentage)
                        };

                        table.Rows[0].Cells.Add(tableCell);
                    }

                    //Modify the last cell. The last cell consists of 2 text values: 1 value - on the left side, 2-th value - on the right side.
                    String theLastOptionText = matrixItemColumn.OptionTexts[matrixItemColumn.OptionTexts.Length - 1];
                    String theLastButOneOptionText =
                        matrixItemColumn.OptionTexts[matrixItemColumn.OptionTexts.Length - 2];
                    tableCell.Text = String.Format(
                        "<span style='float:left;'>{0}</span><span style='float:right;'>{1}</span>",
                        theLastButOneOptionText, theLastOptionText);
                }
                else
                {
                    var cell = new TableCell
                    {
                        CssClass = "columnHeader " + GridLineClassForHeader,

                        Text =
                            matrixItemColumn.OptionTexts.Length == 1
                                ? matrixItemColumn.OptionTexts[0]
                                : ""
                    };

                    controlsPlace.Controls.Add(cell);
                }
            }
        }

        if (matrixItemColumn.OptionTexts.Length > 0
            && !matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase) &&
            !matrixItemColumn.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
        // if the column has optionText and isn't a DropdownList
        {
            var prototypeItemData = SurveyMetaDataProxy.GetItemData(matrixItemColumn.PrototypeItemId, true);
            for (int i = 0; i < matrixItemColumn.OptionTexts.Length; i++)
            {
                string optionText = GetOptionText(matrixItemColumn.OptionTexts[i]);
                var cell = new TableCell
                {
                    CssClass = "columnHeader " + ((i < (matrixItemColumn.OptionTexts.Length - (prototypeItemData.AllowOther ? 2 : 1)))
                                                                     ? String.Empty
                                                                     : GridLineClassForHeader),
                    HorizontalAlign = HorizontalAlign.Center,
                    Text = optionText
                };

                controlsPlace.Controls.Add(cell);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ColumnHeaderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
        var controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

        if (matrixItemColumn == null || controlsPlace == null)
        {
            return;
        }

        int colspan = 1;
        var cell = new TableCell();

        int rowspan = ContainsRatingScaleColumn ? 3 : (ContainsRadioButtonsColumn || ContainsCheckBoxesColumn || ContainsSliderColumn) ? 2 : 1;
        cell.CssClass = "columnHeader " + GridLineClassForHeader;
        cell.RowSpan = rowspan;

        bool naOptionExists = false;

        if (matrixItemColumn.ColumnNumber != MatrixItem.LabelColumnIndex)
        {
            naOptionExists = ContainsRatingScaleColumn &&
                      SurveyMetaDataProxy.GetItemData(matrixItemColumn.PrototypeItemId, true).AllowOther;

            if (!matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
                colspan = Math.Max(matrixItemColumn.OptionTexts.Length, 1);

            if (matrixItemColumn.OptionTexts.Length > 0 && !matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
                rowspan = matrixItemColumn.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase) ? 1 : ContainsRatingScaleColumn ? 2 : 1;

            if (naOptionExists)
                colspan--;

            if (matrixItemColumn.ColumnType.Equals("Slider"))
            {
                if (matrixItemColumn.OptionTexts.Length == 0)
                    rowspan = ContainsRatingScaleColumn ? 2 : 1;
                else
                    colspan = Math.Max(matrixItemColumn.OptionTexts.Length - 1, 1);
            }

            cell.ColumnSpan = colspan;
            cell.RowSpan = rowspan;
            cell.Text = GetColumnText(matrixItemColumn);
        }

        controlsPlace.Controls.Add(cell);

        if (naOptionExists)
            controlsPlace.Controls.Add(new TableCell { CssClass = "columnHeader BorderRight" });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnInfo"></param>
    /// <returns></returns>
    private string GetColumnText(MatrixItemColumn columnInfo)
    {
        var columnText = string.Format("&nbsp;{0}&nbsp;", columnInfo.HeaderText);

        //Return text only if not requiring answers
        if (!columnInfo.AnswerRequired || !_showAsterisks)
        {
            return columnText;
        }

        var modifiedString = string.Format("<span class=\"requiredField\">{0}</span>", WebTextManager.GetText("/common/surveyItemRequiredIndicator", Model.LanguageCode, string.Empty));

        if (columnText.StartsWith("<p>", StringComparison.InvariantCultureIgnoreCase) && columnText.Length > 3)
        {
            return columnText.Insert(3, modifiedString);
        }

        return modifiedString + columnText;
    }



    /// <summary>
    /// Reorganize controls and/or apply specific styles depending
    /// on item's label position setting.
    /// </summary>
    protected void SetLabelPosition()
    {
        //When label is set to bottom, we need to move controls from the top panel
        // to the bottom panel.  Otherwise, position changes are managed by setting
        // CSS class.
        if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            //Move text controls to bottom
            _bottomAndOrRightPanel.Controls.Add(_textContainer);

            //Move input to top
            _topAndOrLeftPanel.Controls.Add(_inputPanel);
        }

        //Set css classes
        _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Top");
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

        if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        }
    }



</script>
