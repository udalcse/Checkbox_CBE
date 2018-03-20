<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CategorizedMatrix.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.CategorizedMatrix" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Forms.Data" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Prezza.Framework.ExceptionHandling" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Forms.Items.UI" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Common" %>
<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <table id='<%=ClientID + "_matrix" %>' class="Matrix" <%#String.IsNullOrEmpty(Appearance["Width"])? String.Empty : "style='width:" + Appearance["Width"]+"px'" %>>
                    <%-- Column Text --%>
                    <tr class="header">
                        <asp:Repeater ID="_columnHeaderRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound='ColumnHeaderRepeater_ItemDataBound'>
                            <ItemTemplate>
                                <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                            </ItemTemplate>
                        </asp:Repeater>
                    </tr>
                    <%-- Scale Texts --%>
                    <asp:PlaceHolder ID="_scalePlaceholder" runat="server" Visible="<%# ContainsRatingScaleColumn %>">
                        <tr valign="bottom" class="header Answer scaleText">
                            <asp:Repeater ID="_columnScaleTextsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemCreated="OnScaleTextItemCreated">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_scaleTextsPlace" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Column Options --%>
                    <asp:PlaceHolder ID="_columnOptionsPlaceholder" runat="server" Visible="<%#ContainsRatingScaleColumn || ContainsCategorizedRadioButtonsColumn || ContainsCategorizedCheckBoxesColumn %>">
                        <tr class="header Answer">
                            <asp:Repeater ID="_columnOptionsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound="ColumnOptionsRepeater_OnItemDataBound">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Column Validation Messages --%>
                    <asp:PlaceHolder ID="_columnValidationMessagesPlaceholder" runat="server" Visible="<%#IsMatrixResponseValid %>">
                        <tr>
                            <asp:Repeater ID="_columnValidationMessagesRepeater" runat="server" DataSource='<%#MatrixItem.Columns %>'
                                OnItemDataBound="ColumnValidationMessages_OnItemDataBound">
                                <ItemTemplate>
                                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </asp:PlaceHolder>
                    <%-- Rows --%>
                    <asp:Repeater ID="_rowRepeater" runat="server"  DataSource='<%#MatrixItem.Rows %>'>
                        <ItemTemplate>
                            <tr class='<%#GetRowClass(((MatrixItemRow) Container.DataItem).RowNumber)%>'>
                                <asp:Repeater ID="_columnRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>' OnItemDataBound="MatrixChildItemCreated">
                                    <ItemTemplate>
                                        <asp:PlaceHolder ID="_childControlsPlace" runat="server"  />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <!--</tbody> -->
                </table>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<script type="text/C#" runat="server">

    private Dictionary<int, UserControlItemRendererBase> _childRenderers;
    private Dictionary<int, AppearanceData> _appearanceCache;
    private MatrixAdditionalData _matrixData;
    private bool _showAsterisks;

    /// <summary>
    /// 
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var controls = base.ChildUserControls;
            controls.AddRange(_childRenderers.Values);
            controls.Add(_questionText);

            return controls;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataTransferObject"></param>
    /// <param name="itemNumber"></param>
    protected override void InitializeChildUserControls(IItemProxyObject dataTransferObject, int? itemNumber)
    {
        //Bind row/column repeaters so that child user controls are populated before children initialized
        // by base class.
        if (dataTransferObject == null)
        {
            throw new Exception("NULL DTO provided to matrix renderer.");
        }
        
        _matrixData = dataTransferObject.AdditionalData as MatrixAdditionalData;

        if (_matrixData == null)
        {
            throw new Exception("Matrix additional data parameter was null.");
        }

        _childRenderers = new Dictionary<int, UserControlItemRendererBase>();

        _rowRepeater.DataBind();

        base.InitializeChildUserControls(dataTransferObject, itemNumber);
    }
    
    
    /// <summary>
    /// Initialize child user controls to set label and item positions
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        //Cause rows/columns to be bound and child renderers created
        DataBind();

        SetLabelPosition();
        SetItemPosition();
    }


    ///// <summary>
    ///// Bind the model
    ///// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        DataBind();
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
    /// CSS name for row header of the matrix
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <returns></returns>
    public string GetRowClass(int rowNumber)
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
    /// CSS name for header columns of the matrix
    /// </summary>
    public string GridLineClassForHeader
    {
        get
        {
            if (Appearance["GridLines"] == "Both" || Appearance["GridLines"] == "Vertical")
                return "BorderRight";
            return String.Empty;
        }
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
            return MatrixItem.Columns.Any(column => column.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <summary>
    /// Determine if matrix consist CategorizedRadioButtons column
    /// </summary>
    public bool ContainsCategorizedRadioButtonsColumn
    {
        get
        {
            return MatrixItem.Columns.Any(column => column.ColumnType.Equals("CategorizedRadioButtons", StringComparison.InvariantCultureIgnoreCase));
        }
    }

    /// <summary>
    /// Determine if matrix consist CategorizedCheckBoxes column
    /// </summary>
    public bool ContainsCategorizedCheckBoxesColumn
    {
        get
        {
            return MatrixItem.Columns.Any(column => column.ColumnType.Equals("CategorizedCheckBoxes", StringComparison.InvariantCultureIgnoreCase));
        }
    }


    /// <summary>
    /// Get all columns count except primaryKeyColumn and some additional columns (e.g. "AddColumn")
    /// </summary>
    public int AllColumnsCount
    {
        get
        {
            int result = 1;
            foreach (MatrixItemColumn column in MatrixItem.Columns)
            {
                if (column.ColumnNumber != MatrixItem.LabelColumnIndex)
                {
                    if (column.ColumnType.Equals("CategorizedDropDownList", StringComparison.InvariantCultureIgnoreCase))
                        result++;
                    else
                        result += Math.Max(column.OptionTexts.Length, 1);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Determine if matrix response is valid or not.
    /// </summary>
    protected bool IsMatrixResponseValid
    {
        get
        {
            bool result = false;
            foreach (MatrixItemColumn column in MatrixItem.Columns)
            {
                result |= column.ValidationErrors.Count() > 0;
            }

            return result;
        }
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

            //Start index is cell containing scale start text.  Always item at index 0
            var startIndex = 0;
            var optionCount = prototypeItemData.AllowOther
                                  ? prototypeItemData.Options.Count - 1
                                  : prototypeItemData.Options.Count;


            //End index is cell containing end text.  Text does not go over NA option, so
            // adjust index accordingly.
            var endIndex = optionCount - 1;

            //Middle index and span depends on whether there are odd-or even number of options.
            var midIndex = (optionCount % 2 == 1)
                               ? ((optionCount + 1) / 2) - 1
                               : (optionCount / 2) - 1;

            var midSpan = optionCount % 2 == 1 ? 1 : 2;

            //Find na text, which is last option text for column, if it exists
            var naOptionText = prototypeItemData.AllowOther
                               ? columnInfo.OptionTexts.LastOrDefault()
                               : null;

            Unit optionWidth;
            Unit naOptionWidth;

            //Calculate widths
            CalculateScaleContainerWidths(optionCount, columnInfo.ScaleStartText, columnInfo.ScaleMidText, columnInfo.ScaleEndText, naOptionText, columnInfo.Width, out optionWidth, out naOptionWidth);


            //Now add cells
            for (int index = 0; index < optionCount; index++)
            {
                var cell = new TableCell { HorizontalAlign = HorizontalAlign.Center };
                var panel = new Panel { Width = optionWidth };
                var literal = new LiteralControl();

                if (index == startIndex)
                {
                    literal.Text = columnInfo.ScaleStartText;
                }

                if (index == endIndex)
                {
                    literal.Text = columnInfo.ScaleEndText;
                }


                if (index == midIndex)
                {
                    literal.Text = columnInfo.ScaleMidText;
                    cell.ColumnSpan = midSpan;

                    //If spanning multiple columns, increment index to ensure we don't
                    // add too many.
                    if (midSpan > 1)
                    {
                        index++;
                    }
                }

                cell.Controls.Add(panel);
                panel.Controls.Add(literal);

                scaleTextsPlace.Controls.Add(cell);
            }


            //Add cell for n/a option, if necessary
            if (prototypeItemData.AllowOther)
            {

                var cell = new TableCell();
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
    protected void CalculateScaleContainerWidths(int optionCount, string startText, string midText, string endText, string naText, int? columnWidth, out Unit optionWidth, out Unit naOptionWidth)
    {
        //Header text container width needs to be set so that it is large enough
        // to contain all text and span all options except the N/A option (if any).
        //So, the width will be the greater of a fixed value * (number of options) or 
        // another fixed value * (number of characters in largest word in scale text).

        const int pixelsPerOption = 30;
        const int pixelsPerLetter = 15;

        //Get sum of lengths of longest words for start/mid/end text so that we can
        // ensure word will fit w/out need for wrapping in middle of word.
        int maxWordLengthSum = new[]
            {
                Utilities.IsNotNullOrEmpty(startText) ? startText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(midText) ? midText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1,
                Utilities.IsNotNullOrEmpty(endText) ? endText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Max(word => word.Length) : 1
            }.Sum();

        //Sum lengths

        //Min header text size (in pixels)
        int minHeaderTextSize = maxWordLengthSum * pixelsPerLetter;
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

            //Make sure we have row info, and that column prototype information is available
            if (rowInfo == null || columnInfo == null)
            {
                throw new Exception("Unable to get position information for child item.");
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

            bool.TryParse((childItemDto as SurveyResponseItem).Metadata["showAsterisks"], out _showAsterisks);

            AppearanceData itemAppearance = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                ? GetItemAppearance(childItemDto.ItemId)
                : GetItemAppearance(columnInfo.PrototypeItemId);

            if (itemAppearance == null)
            {
                //If no appearance use default for type
                var itemTypeId = ItemConfigurationManager.GetTypeIdFromName(childItemDto.TypeName);
                
                if(!itemTypeId.HasValue)
                {
                    throw new Exception("Unable to type information for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex + ". Type = [" + childItemDto.TypeName + "].");
                }

                itemAppearance = AppearanceDataManager.GetDefaultAppearanceDataForType(itemTypeId.Value);

                if (itemAppearance == null)
                {
                    throw new Exception("Unable to get appearance information for child item.  Row Index = " + rowIndex +" and column index = " + columnIndex);
                }
            }

            //Get the renderer for the column prototype
            Control itemRenderer = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                ? WebItemRendererManager.GetItemRenderer(childItemDto.ItemId, itemAppearance, RenderMode.Survey)
                : WebItemRendererManager.GetItemRenderer(columnInfo.PrototypeItemId, itemAppearance, RenderMode.Survey);

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
                matrixRenderer.GridLineMode = Appearance["GridLines"];
                matrixRenderer.ColumnWidth = columnInfo.Width;
            }
            
            
            //Initialize the renderer.  If renderer is user control based,
            // override to use matrix-specific controls
            if (itemRenderer is UserControlHostItemRenderer)
            {
                ((UserControlHostItemRenderer)itemRenderer).ControlNameOverride = "Matrix_" + childItemDto.TypeName;
                ((IItemRenderer)itemRenderer).Initialize(childItemDto, null, RenderMode.Survey);

                ((UserControlHostItemRenderer)itemRenderer).Width = Unit.Percentage(Math.Truncate(100.00 / AllColumnsCount));
                
                ((IItemRenderer)itemRenderer).BindModel();

                _childRenderers[childItemDto.ItemId] = ((UserControlHostItemRenderer)itemRenderer).InternalItemRenderer;
            }

            //Add renderer to page, surrounded by table cell tag
            var childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;
            
            if (childControlsPlace != null)
            {
                childControlsPlace.Controls.Clear();
                if (rowInfo.RowType == RowType.Subheading.ToString())
                {
                    if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    {
                        var cell = new TableCell
                                       {
                                           Text =  Utilities.SimpleHtmlEncode(rowInfo.Text),
                                           ColumnSpan = AllColumnsCount,
                                           VerticalAlign = VerticalAlign.Middle
                                       };
                        childControlsPlace.Controls.Add(cell);
                    }
                }
                else
                {
                    childControlsPlace.Controls.Add(itemRenderer);
                }
            }
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

        if (matrixItemColumn.OptionTexts.Length > 0 && !IsColumnDropDown(matrixItemColumn.ColumnType) && !IsColumnRowSelector(matrixItemColumn.ColumnType)) // if the column has optionText and isn't a DropdownList and isn't a RowSelector            
        {
            for (int i = 0; i < matrixItemColumn.OptionTexts.Length; i++)
            {
                string optionText = matrixItemColumn.OptionTexts[i];
                var cell = new TableCell
                               {
                                   CssClass = ((i < (matrixItemColumn.OptionTexts.Length - 1))
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

        int rowspan = ContainsRatingScaleColumn ? 3 : (ContainsCategorizedRadioButtonsColumn || ContainsCategorizedCheckBoxesColumn) ? 2 : 1;
        cell.CssClass = GridLineClassForHeader;
        cell.RowSpan = rowspan;

        if (matrixItemColumn.ColumnNumber != MatrixItem.LabelColumnIndex)
        {
            if (!IsColumnDropDown(matrixItemColumn.ColumnType))
                colspan = Math.Max(matrixItemColumn.OptionTexts.Length, 1);

            if (matrixItemColumn.OptionTexts.Length > 0 && !IsColumnDropDown(matrixItemColumn.ColumnType) && !IsColumnRowSelector(matrixItemColumn.ColumnType))
                rowspan = matrixItemColumn.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase) ? 1 : ContainsRatingScaleColumn ? 2 : 1;

            cell.Text = GetColumnText(matrixItemColumn);
            cell.ColumnSpan = colspan;
            cell.RowSpan = rowspan;

            if (matrixItemColumn.Width.HasValue)
                cell.Width = new Unit(matrixItemColumn.Width.Value);

        }
        controlsPlace.Controls.Add(cell);
    }

    protected void ColumnValidationMessages_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        MatrixItemColumn matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
        PlaceHolder controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

        TableCell cell = new TableCell();

        Panel errorPanel = new Panel { CssClass = "Error" };

        foreach (string validationError in matrixItemColumn.ValidationErrors)
        {
            Label errorLabel = new Label();
            errorLabel.Text = validationError;
            errorPanel.Controls.Add(errorLabel);
        }

        cell.Controls.Add(errorPanel);

        controlsPlace.Controls.Add(cell);
    }

    /// <summary>
    /// Determine if the item with specified column type should be displayed as dropDown or not.
    /// </summary>
    /// <param name="columnType"></param>
    /// <returns></returns>
    private bool IsColumnDropDown(String columnType)
    {
        return columnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase) ||
               columnType.Equals("CategorizedDropdownList", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Determine if the item is rowSelector or not.
    /// </summary>
    /// <param name="columnType"></param>
    /// <returns></returns>
    private bool IsColumnRowSelector(String columnType)
    {
        return columnType.Equals("RowSelector", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnInfo"></param>
    /// <returns></returns>
    private string GetColumnText(MatrixItemColumn columnInfo)
    {
        var columnText = columnInfo.HeaderText;

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
