using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Rendering;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Renderer for matrix items
    /// </summary>
    public partial class Matrix : UserControlSurveyItemRendererBase
    {

        private Dictionary<int, UserControlItemRendererBase> _childRenderers;
        private Dictionary<int, AppearanceData> _appearanceCache;
        private MatrixAdditionalData _matrixData;
        private bool _showAsterisks = false;
        private int _matrixId;
        public BindedMatrixInfo BindedMatrixInfo;
        public bool IsBinded { get; set; }
        private const int DefaultColumnWidth = 150;

        private string StoredResponseSessionDataKey
        {
            get { return "SurveySession_" + Guid.Parse(Request.QueryString["s"]); }
        }

        #region ----- Binded matrix request variables -----

        private string MatrixControlId => ClientID + "_matrix";

        private List<string> ColumnHeaders
        {
            get
            {
                var headers = WebUtilities.GetRequestFormValuseBySubstring(MatrixControlId + "-binded-matrix-column-header-");

                if (ShouldAddNewColumnValues)
                {
                    headers.Add(NewRowHeader);
                }

                return headers;
            }
        }

        private List<string> RowHeaders
        {
            get
            {
                var headers = WebUtilities.GetRequestFormValuseBySubstring(MatrixControlId + "-binded-matrix-row-header-");

                if (ShouldAddNewRowValues)
                {
                    headers.Add(NewRowHeader);
                }

                return headers;

            }
        }

        private List<string> CustomCellValues
        {
            get
            {
                var values = WebUtilities.GetRequestFormValuseBySubstring(MatrixControlId + "-binded-matrix-input");
                if (ShouldAddNewRowValues || ShouldAddNewColumnValues)
                {
                    values.AddRange(NewRowInputs);
                }

                return values;
            }
        }


        private List<string> NewRowInputs => WebUtilities.GetRequestFormValuseBySubstring(MatrixControlId + "-binded-matrix-new-row-input");

        private string NewRowHeader => WebUtilities.GetRequestFormValueBySubstring(MatrixControlId + "-binded-matrix-new-row-header");

        //private bool ShouldAddNewRowCells {
        //    get
        //    {
        //        if ((BindedMatrixInfo.HasRowHeaders || BindedMatrixInfo.HasColumnHeaders) && !string.IsNullOrWhiteSpace(NewRowHeader))
        //        {
        //            return true;
        //        }
        //        else if (!(BindedMatrixInfo.HasRowHeaders && BindedMatrixInfo.HasColumnHeaders) && NewRowInputs.Any(item => !string.IsNullOrWhiteSpace(item)))
        //        {
        //            return true;
        //        }

        //        return false;
        //    }
        //}

        private bool ShouldAddNewRowValues
        {
            get
            {
                if (BindedMatrixInfo.IsColumnsStatic)
                {
                    if (BindedMatrixInfo.HasRowHeaders && !string.IsNullOrWhiteSpace(NewRowHeader))
                    {
                        return true;
                    }
                    else if (!BindedMatrixInfo.HasRowHeaders && string.IsNullOrWhiteSpace(NewRowHeader) && NewRowInputs.Any(item => !string.IsNullOrWhiteSpace(item)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private bool ShouldAddNewColumnValues
        {
            get
            {
                if (BindedMatrixInfo.IsRowsStatic)
                {
                    if (BindedMatrixInfo.HasColumnHeaders && !string.IsNullOrWhiteSpace(NewRowHeader))
                    {
                        return true;
                    }
                    else if (!BindedMatrixInfo.HasColumnHeaders && string.IsNullOrWhiteSpace(NewRowHeader) && NewRowInputs.Any(item => !string.IsNullOrWhiteSpace(item)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private int ColumnCount => int.Parse(WebUtilities.GetRequestFormValueBySubstring(MatrixControlId + "-binded-matrix-columns-count"));

        private int RowCount => int.Parse(WebUtilities.GetRequestFormValueBySubstring(MatrixControlId + "-binded-matrix-rows-count"));


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="itemNumber"></param>
        public override void Initialize(IItemProxyObject dataTransferObject, int? itemNumber)
        {
            base.Initialize(dataTransferObject, itemNumber);


            //initialize with item appearance data
            if (!string.IsNullOrEmpty(Appearance["Width"]))
                _inputPanel.Attributes["style"] = "width: " + Appearance["Width"] + "px; overflow: auto;";
        }

        /// <summary>
        /// 
        /// </summary>
        protected string TableWidth
        {
            get { return string.IsNullOrEmpty(Appearance["Width"]) ? "100%" : (Appearance["Width"] + "px"); }
        }

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
        /// <param name="exportMode"></param>
        protected override void InitializeChildUserControls(IItemProxyObject dataTransferObject, int? itemNumber, ExportMode exportMode)
        {
            //Bind row/column repeaters so that child user controls are populated before children initialized
            // by base class.
            if (dataTransferObject == null)
            {
                throw new Exception("NULL DTO provided to matrix renderer.");
            }

            _matrixData = dataTransferObject.AdditionalData as MatrixAdditionalData;
            _matrixId = dataTransferObject.ItemId;

            if (_matrixData?.BindedMatrixInfo != null)
            {
                BindedMatrixInfo = _matrixData.BindedMatrixInfo;
                BindedMatrixInfo.GridLines = Appearance["GridLines"];
                IsBinded = BindedMatrixInfo != null;
            }

            if (_matrixData == null)
            {
                throw new Exception("Matrix additional data parameter was null.");
            }

            _childRenderers = new Dictionary<int, UserControlItemRendererBase>();

            _rowRepeater.DataBind();

            base.InitializeChildUserControls(dataTransferObject, itemNumber, exportMode);
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

        protected override void InlineUpdateModel()
        {
            base.InlineUpdateModel();

            var bindedField = PropertyBindingManager.GetBindedPropertyByItemId(_matrixId);

            if (bindedField != null && MatrixItem != null)
            {
                var userGuid = PropertyBindingManager.GetCurrentUserGuid();

                var matrix = ProfileManager.GetMatrixField(bindedField.Name,
                    userGuid);

                if (matrix.HasFixedStructure)
                {
                    //clear current matrix cells and add new ones from dto
                    matrix.RemoveDataCells();
                    var matrixValues = GetBaseMatrixValues(MatrixItem.Rows.Length, MatrixItem.Columns.Length);

                    matrix.CreateDataCells(MatrixItem.Rows.Length, MatrixItem.Columns.Length - 1, matrixValues);
                }
                else
                {
                    PopulateCustomMatrixStructure(matrix);
                }

                if (userGuid.Equals(Guid.Empty))
                {
                    var sessionState = ((ResponseSessionData)Session[StoredResponseSessionDataKey]);
                    if (sessionState.AnonymousBindedFields == null)
                        sessionState.AnonymousBindedFields = new Dictionary<string, string>();

                    sessionState.AnonymousBindedFields[matrix.FieldName] = MatrixField.MatrixToJson(matrix);
                    Session[StoredResponseSessionDataKey] = sessionState;
                }
                else
                {
                    ProfileManager.AddCustomMatrixCells(matrix, userGuid);
                }
            }
        }

        /// <summary>
        /// Dinamycally store custom matrix fields
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        private void PopulateCustomMatrixStructure(MatrixField matrix)
        {
            matrix.RemoveDataCells();

            if (!matrix.IsRowsFixed)
            {
                matrix.RemoveCustomStructure();
                matrix.CreateCustomRowHeaders(RowHeaders);
            }
            else
            {
                matrix.RemoveCustomStructure();
                matrix.CreateCustomColumnHeaders(ColumnHeaders);
            }

            var matrixValues = GetBaseMatrixValues(matrix.BaseRowsCount, matrix.BaseColumnCount + 1);

            //create basic structure 
            matrix.CreateDataCells(matrix.BaseRowsCount, matrix.BaseColumnCount, matrixValues, true);

            //create custom structure if there is any value
            if (CustomCellValues.Any())
            {
                if (!matrix.IsRowsFixed)
                {

                    var customRowsCount = matrix.IsColumnsFixed ? RowCount - matrix.BaseRowsCount : RowCount;


                    if (ShouldAddNewRowValues)
                    {
                        customRowsCount++;
                    }

                    matrix.CreateCustomRowDataCells(customRowsCount, ColumnCount,
                      CustomCellValues);
                }
                else
                {
                    var customColumnCount = matrix.IsRowsFixed
                 ? ColumnCount - matrix.BaseColumnCount
                 : ColumnCount;

                    if (ShouldAddNewColumnValues)
                    {
                        customColumnCount++;
                    }

                    matrix.CreateCustomColumnDataCells(RowCount, customColumnCount,
                         CustomCellValues);
                }
            }
        }

        /// <summary>
        /// Gets the base structure matrix values.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        private List<string> GetBaseMatrixValues(int rows, int columns)
        {
            List<string> matrixValues = new List<string>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 1; j < columns; j++)
                {
                    var answer = ((SurveyResponseItem)MatrixItem.ChildItems[i][j]).Answers;
                    if (answer != null && answer.Any())
                        matrixValues.Add(answer[0].AnswerText);
                }
            }

            return matrixValues;
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
                return "AlternatingItem";

            return "Item";
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
        /// Determine if matrix consist radioButtons column
        /// </summary>
        public bool ContainsRadioButtonsColumn
        {
            get
            {
                return MatrixItem.Columns.Any(column => column.ColumnType.Equals("RadioButtons", StringComparison.InvariantCultureIgnoreCase));
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
                return MatrixItem.Columns.Any(column => column.ColumnType.Equals("CheckBoxes", StringComparison.InvariantCultureIgnoreCase));
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
                    var cell = new TableCell { HorizontalAlign = HorizontalAlign.Center };
                    var panel = new Panel();
                    panel.Style[HtmlTextWriterStyle.MarginLeft] = "auto";
                    panel.Style[HtmlTextWriterStyle.MarginRight] = "auto";

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
                    var cell = new TableCell { CssClass = GridLineClassForHeader };
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
            const int pixelsPerLetter = 15;

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
                {
                    if (midSpan + 1 == optionCount)
                        midSpan--;
                    else
                        midSpan++;
                }
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

                //Make sure we have row info, and that column prototype information is available
                if (rowInfo == null || columnInfo == null)
                {
                    throw new Exception("Unable to get position information for child item.");
                }

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
                    bool.TryParse((childItemDto as SurveyResponseItem).Metadata["showAsterisks"], out _showAsterisks);


                AppearanceData itemAppearance = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? GetItemAppearance(childItemDto.ItemId)
                    : GetItemAppearance(columnInfo.PrototypeItemId);

                //If no appearance use default for type
                if (itemAppearance == null)
                {
                    var appearenceType = childItemDto.TypeName;

                    //use matrix message type instead of message
                    if (childItemDto.TypeName.Equals("Message"))
                        appearenceType = "MatrixMessage";

                    var itemTypeId = ItemConfigurationManager.GetTypeIdFromName(appearenceType);

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

                if (string.IsNullOrEmpty(Appearance["GridLines"]) && !string.IsNullOrEmpty(MatrixItem.GridLines))
                {
                    Appearance["GridLines"] = MatrixItem.GridLines;
                }

                ((ItemProxyObject)childItemDto).AppearanceData = itemAppearance.GetPropertiesAsNameValueCollection();
                if (childItemDto != null)
                {
                    if (rowInfo.Text.ToString().ToLower().Contains("date") && columnInfo.ColumnType == "SingleLineText")
                    {
                        // MatrixItem.ChildItems.SetValue(DateTime.Now,0);



                    }
                }


                var renderMode = IsMobileSurvey ? RenderMode.SurveyMobile : RenderMode.Survey;

                //Get the renderer for the column prototype
                Control itemRenderer = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? WebItemRendererManager.GetItemRenderer(childItemDto.ItemId, itemAppearance, renderMode, true)
                    : WebItemRendererManager.GetItemRenderer(columnInfo.PrototypeItemId, itemAppearance, renderMode, true);

                if (itemRenderer == null)
                {
                    throw new Exception("Unable to get renderer for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
                }

                //Override control name if renderer supports            
                itemRenderer.ID = string.Format("Matrix_{0}_{1}", rowInfo.RowNumber, columnInfo.ColumnNumber);
               
                bool addTopBorderForRowHeaders = false;
                bool addRightBorderForRowHeaders = false;
                //Set the additional properties of renderer
                if (itemRenderer is MatrixChildrensItemRenderer)
                {
                    var matrixRenderer = itemRenderer as MatrixChildrensItemRenderer;
                    matrixRenderer.ChildType = columnIndex == 0 ? MatrixChildType.RowText : MatrixChildType.OrdinaryChild;
                    matrixRenderer.ColumnWidth = columnInfo.Width;
                    matrixRenderer.GridLineMode = Appearance["GridLines"];
                    matrixRenderer.RowTextPosition = Appearance["RowTextPosition"];
                    
                  


                    addRightBorderForRowHeaders = "Vertical".Equals(matrixRenderer.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                        || "Both".Equals(matrixRenderer.GridLineMode, StringComparison.InvariantCultureIgnoreCase);
                    addTopBorderForRowHeaders = "Horizontal".Equals(matrixRenderer.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                        || "Both".Equals(matrixRenderer.GridLineMode, StringComparison.InvariantCultureIgnoreCase);
                }

                //Initialize the renderer.  If renderer is user control based,
                // override to use matrix-specific controls
                if (itemRenderer is UserControlHostItemRenderer)
                {
                    ((UserControlHostItemRenderer)itemRenderer).ControlNameOverride = "Matrix_" + childItemDto.TypeName;
                    ((IItemRenderer)itemRenderer).Initialize(childItemDto, null, renderMode);

                    if (columnInfo.Width.HasValue)
                    {
                        if (columnInfo.Width.Value == 0)
                            columnInfo.Width = DefaultColumnWidth;

                        ((UserControlHostItemRenderer)itemRenderer).Width = Unit.Pixel(columnInfo.Width.Value);
                        ((UserControlHostItemRenderer)itemRenderer).Appearance.Add("Width",
                            columnInfo.Width.Value.ToString());
                    }

                    else if (!columnInfo.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
                        ((UserControlHostItemRenderer)itemRenderer).Width = Unit.Percentage(Math.Truncate(100.00 / AllColumnsCount));

                    ((IItemRenderer)itemRenderer).BindModel();

                    _childRenderers[childItemDto.ItemId] = ((UserControlHostItemRenderer)itemRenderer).InternalItemRenderer;
                }

                //Add renderer to page, surrounded by table cell tag
                var childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;

                if (rowInfo.RowType == RowType.Subheading.ToString())
                {
                    _MainHeadercontrolsPlace.Text = "";
                    //for subheadings Text value should be taken from MatrixItem.ChildItems because they are already with merged @@[properties]
                    var childItem = MatrixItem.ChildItems[rowInfo.RowNumber - 1][0].InstanceData.NameValueList.FirstOrDefault(x => x.Name == "Text");
                    if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    {
                        var cellText = childItem != null ? childItem.Value : rowInfo.Text;

                        _MainHeadercontrolsPlace.Text = Utilities.IsHtmlFormattedText(cellText) ? Utilities.EncodeTagsInHtmlContent(Utilities.AdvancedHtmlDecode(cellText)) : Utilities.AdvancedHtmlEncode(cellText);


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

                #region Old Code
                //if (childControlsPlace != null)
                //{
                //    childControlsPlace.Controls.Clear();
                //    if (rowInfo.RowType == RowType.Subheading.ToString())
                //    {
                //        //for subheadings Text value should be taken from MatrixItem.ChildItems because they are already with merged @@[properties]
                //        var childItem = MatrixItem.ChildItems[rowInfo.RowNumber - 1][0].InstanceData.NameValueList.FirstOrDefault(x => x.Name == "Text");
                //        if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                //        {
                //            var cellText = childItem != null ? childItem.Value : rowInfo.Text;
                //            var cell = new TableCell
                //            {
                //                Text = Utilities.IsHtmlFormattedText(cellText) ? Utilities.EncodeTagsInHtmlContent(Utilities.AdvancedHtmlDecode(cellText)) : Utilities.AdvancedHtmlEncode(cellText),
                //                ColumnSpan = AllColumnsCount,
                //                VerticalAlign = VerticalAlign.Middle,
                //                CssClass = (addTopBorderForRowHeaders ? "BorderTop " : "") + (addRightBorderForRowHeaders ? "BorderRight" : "")
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
            var p = new Panel { CssClass = "Error" };
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
                        CssClass = "Answer " + GridLineClassForHeader,
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
                            CssClass = GridLineClassForHeader,
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
                                CssClass = "Answer",
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
                            CssClass = "Answer " + GridLineClassForHeader,

                            Text =
                                matrixItemColumn.OptionTexts.Length == 1
                                    ? matrixItemColumn.OptionTexts[0]
                                    : ""
                        };

                        controlsPlace.Controls.Add(cell);
                    }
                }
            }

            if (matrixItemColumn.OptionTexts.Length > 0 &&
                !matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase) &&
                !matrixItemColumn.ColumnType.Equals("Slider", StringComparison.InvariantCultureIgnoreCase))
            // if the column has optionText and isn't a DropdownList
            {
                var prototypeItemData = SurveyMetaDataProxy.GetItemData(matrixItemColumn.PrototypeItemId, true);
                for (int i = 0; i < matrixItemColumn.OptionTexts.Length; i++)
                {
                    string optionText = GetOptionText(matrixItemColumn.OptionTexts[i]);
                    var cell = new TableCell
                    {
                        CssClass = "Answer " + ((i < (matrixItemColumn.OptionTexts.Length - (prototypeItemData.AllowOther ? 2 : 1)))
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
            cell.CssClass = GridLineClassForHeader;
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

                cell.Text = GetColumnText(matrixItemColumn);
                cell.ColumnSpan = colspan;
                cell.RowSpan = rowspan;
                if (matrixItemColumn.Width.HasValue && matrixItemColumn.Width != 0)
                    cell.Width = new Unit(matrixItemColumn.Width.Value);

            }
            controlsPlace.Controls.Add(cell);

            if (naOptionExists)
                controlsPlace.Controls.Add(new TableCell { CssClass = "BorderRight" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ColumnValidationMessages_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
            var controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

            if (controlsPlace == null || matrixItemColumn == null)
            {
                return;
            }

            var cell = new TableCell();

            int colspan = 1;
            if (!matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
            {
                colspan = Math.Max(matrixItemColumn.OptionTexts.Length, 1);
            }

            cell.ColumnSpan = colspan;

            foreach (string validationError in matrixItemColumn.ValidationErrors)
            {
                var errorLabel = new LiteralControl { Text = validationError };
                cell.Controls.Add(errorLabel);
            }

            controlsPlace.Controls.Add(cell);
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

        /// <summary>
        /// 
        /// </summary>
        protected void SetRowTextPosition()
        {
            /*
            _rowRepeater.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["RowTextPosition"]) ? Appearance["RowTextPosition"] : "Center");

            if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
            {
                _rowRepeater.Style[HtmlTextWriterStyle.Display] = "inline-block";
            }
             */
        }




    }
}