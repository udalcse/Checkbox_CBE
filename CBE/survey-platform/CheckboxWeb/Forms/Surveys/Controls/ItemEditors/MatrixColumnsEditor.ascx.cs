using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class MatrixColumnsEditor : UserControlItemEditorBase
    {
        private MatrixItemData _matrixItemData;
        private IItemProxyObject _model;
        private List<UserControlItemRendererBase> _childUserControlRenderers;
        private Dictionary<int, AppearanceData> _appearanceCache;


        /// <summary>
        /// Get collection of child renderers
        /// </summary>
        public List<UserControlItemRendererBase> ChildUserControls
        {
            get
            {
                if (_childUserControlRenderers == null)
                {
                    _childUserControlRenderers = new List<UserControlItemRendererBase>();
                }

                return _childUserControlRenderers;
            }
        }

        /// <summary>
        /// Prevent multiple loading of appearance data for each row/column of a matrix
        /// </summary>
        public Dictionary<int, AppearanceData> AppearanceCache
        {
            get
            {
                if (_appearanceCache == null)
                {
                    _appearanceCache = new Dictionary<int, AppearanceData>();
                }

                return _appearanceCache;
            }
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
        /// Get matrix item cast to proper type
        /// </summary>
        public MatrixAdditionalData MatrixItem
        {
            get
            {
                return Model.AdditionalData as MatrixAdditionalData;
            }
        }


        /// <summary>
        /// Determine if matrix consist ratingScale column
        /// </summary>
        public bool DoesConsistRatingScaleColumn
        {
            get
            {
                foreach (MatrixItemColumn column in MatrixItem.Columns)
                {
                    if (column.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Determine if matrix consist radioButtons column
        /// </summary>
        public bool DoesConsistRadioButtonsColumn
        {
            get
            {
                foreach (MatrixItemColumn column in MatrixItem.Columns)
                {
                    if (column.ColumnType.Equals("RadioButtons", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Determine if matrix consist checkboxes column
        /// </summary>
        public bool DoesConsistCheckBoxesColumn
        {
            get
            {
                foreach (MatrixItemColumn column in MatrixItem.Columns)
                {
                    if (column.ColumnType.Equals("CheckBoxes", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                return false;
            }
        }


        /// <summary>
        /// Get all columns count except primaryKeyColumn and some additional columns (e.g. "AddColumn")
        /// </summary>
        public int AllColumnsCount
        {
            get
            {
                int result = 0;
                foreach (MatrixItemColumn column in MatrixItem.Columns)
                {
                    if (column.ColumnNumber != MatrixItem.LabelColumnIndex)
                    {
                        if (column.ColumnType.Equals("DropDownList", StringComparison.InvariantCultureIgnoreCase))
                            result++;
                        else
                            result += Math.Max(column.OptionTexts.Length, 1);
                    }
                }
                return result;
            }
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

        public AppearanceData MatrixAppearance
        {
            get { return GetItemAppearance(Model.ItemId); }
        }


        /// <summary>
        /// CSS name for header columns of the matrix
        /// </summary>
        public string GridLineClassForHeader
        {
            get
            {
                if (MatrixAppearance["GridLines"] == "Both" || MatrixAppearance["GridLines"] == "Vertical")
                    return "BorderRight";
                return String.Empty;
            }
        }


        /// <summary>
        /// Get matrix to edit columns of
        /// </summary>
        private MatrixItemData MatrixItemData
        {
            get
            {
                if (_matrixItemData == null)
                {
                    _matrixItemData = ItemData as MatrixItemData;

                    if (_matrixItemData == null)
                    {
                        throw new Exception("Unable to load matrix item data for item with id: " + Model.ItemId);
                    }
                }

                return _matrixItemData;
            }
        }




        public SurveyResponseItem Model
        {
            get
            {
                if (_model == null || IsModelOld)
                {
                    _model = ItemData.CreateItem(CurrentLanguage).GetDataTransferObject();
                    IsModelOld = false;
                }
                return _model as SurveyResponseItem;
            }
        }


        /// <summary>
        /// Determine if Model should be updated
        /// </summary>
        public bool IsModelOld { get; set; }



        /// <summary>
        /// Create an empty row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EmptryRowRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                //Clear child user control renderer collection
                ChildUserControls.Clear();

                //First, figure out coordinate of item in matrix by getting column info object
                MatrixItemColumn columnInfo = GetColumnInfo(e);

                //Make sure column prototype information is available
                if (columnInfo == null)
                {
                    throw new Exception("Unable to get column information for child item.");
                }

                const int rowIndex = 0;
                int columnIndex = columnInfo.ColumnNumber - 1;


                AppearanceData itemAppearance = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? AppearanceDataManager.GetAppearanceDataForCode("MATRIX_MESSAGE")
                    : GetItemAppearance(columnInfo.PrototypeItemId);

                if (itemAppearance == null)
                {
                    throw new Exception("Unable to get appearance information for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
                }

                //Get the renderer for the column prototype
                Control itemRenderer = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? (Control)ItemRendererFactory.Create("MATRIX_MESSAGE")
                    : WebItemRendererManager.GetItemRenderer(columnInfo.PrototypeItemId, itemAppearance, RenderMode.Survey);
                if (itemRenderer == null)
                {
                    throw new Exception("Unable to get renderer for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
                }


                //Override control name if renderer supports            
                itemRenderer.ID = string.Format("Matrix_1_{0}", columnInfo.ColumnNumber);


                //Set the additional properties of renderer
                if (itemRenderer is MatrixChildrensItemRenderer)
                {
                    MatrixChildrensItemRenderer matrixRenderer = itemRenderer as MatrixChildrensItemRenderer;
                    matrixRenderer.GridLineMode = MatrixAppearance["GridLines"];
                    matrixRenderer.ColumnWidth = columnInfo.Width;
                }


                //Initialize the renderer.  If renderer is user control based,
                // override to use matrix-specific controls)
                if (itemRenderer is IItemRenderer)
                {
                    ItemData itemData = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                                            ? ItemConfigurationManager.CreateConfigurationData("Message")
                                            : ItemConfigurationManager.GetConfigurationData(columnInfo.PrototypeItemId);
                    ((UserControlHostItemRenderer)itemRenderer).ControlNameOverride = "Matrix_" + itemData.ItemTypeName;

                    ((IItemRenderer)itemRenderer).Initialize(itemData.CreateItem(Model.LanguageCode).GetDataTransferObject(), null, RenderMode.Survey);
                    ((IItemRenderer)itemRenderer).BindModel();
                }

                //Add renderer to page, surrounded by table cell tag
                PlaceHolder _controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

                if (_controlsPlace != null)
                {
                    _controlsPlace.Controls.Clear();
                    _controlsPlace.Controls.Add(itemRenderer);
                }
            }
            catch (Exception ex)
            {
                //Ensure that exception gets logged
                ExceptionPolicy.HandleException(ex, "UIProcess");

                //Show error message
                PlaceHolder _controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;

                if (_controlsPlace != null)
                {
                    TableCell cell = new TableCell();
                    cell.Controls.Add(CreateErrorPanel(ex.Message));
                    _controlsPlace.Controls.Add(cell);
                }
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

                AppearanceData itemAppearance = (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                    ? GetItemAppearance(childItemDto.ItemId)
                    : GetItemAppearance(columnInfo.PrototypeItemId);

                if (itemAppearance == null)
                {
                    throw new Exception("Unable to get appearance information for child item.  Row Index = " + rowIndex + " and column index = " + columnIndex);
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
                    MatrixChildrensItemRenderer matrixRenderer = itemRenderer as MatrixChildrensItemRenderer;
                    matrixRenderer.GridLineMode = MatrixAppearance["GridLines"];
                    matrixRenderer.ColumnWidth = columnInfo.Width;
                }


                //Initialize the renderer.  If renderer is user control based,
                // override to use matrix-specific controls
                if (itemRenderer is IItemRenderer)
                {
                    ((UserControlHostItemRenderer)itemRenderer).ControlNameOverride = "Matrix_" + childItemDto.TypeName;
                    ((IItemRenderer)itemRenderer).Initialize(childItemDto, null, RenderMode.Survey);
                    ((IItemRenderer)itemRenderer).BindModel();
                }

                //Add renderer to page, surrounded by table cell tag
                PlaceHolder childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;

                ChildUserControls.Add(((UserControlHostItemRenderer)itemRenderer).InternalItemRenderer);

                if (childControlsPlace != null)
                {
                    childControlsPlace.Controls.Clear();
                    if (rowInfo.RowType == RowType.Subheading.ToString())
                    {
                        if (columnInfo.ColumnNumber == MatrixItem.LabelColumnIndex)
                        {
                            TableCell cell = new TableCell();
                            cell.Text = rowInfo.Text;
                            cell.ColumnSpan = AllColumnsCount + 1;
                            cell.VerticalAlign = VerticalAlign.Middle;
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
                PlaceHolder childControlsPlace = e.Item.FindControl("_childControlsPlace") as PlaceHolder;

                if (childControlsPlace != null)
                {
                    TableCell cell = new TableCell();
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
            Panel p = new Panel { CssClass = "error" };
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

        protected void ColumnOptionsRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MatrixItemColumn matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
            PlaceHolder controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;
            if (matrixItemColumn.ColumnNumber != MatrixItem.LabelColumnIndex)
            {
                if (matrixItemColumn.OptionTexts.Length > 0 && !matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase)) // if the column has optionText and isn't a DropdownList
                {
                    for (int i = 0; i < matrixItemColumn.OptionTexts.Length; i++)
                    {
                        string optionText = matrixItemColumn.OptionTexts[i];
                        TableCell cell = new TableCell();

                        cell.CssClass = "columnHeader " + ((i < (matrixItemColumn.OptionTexts.Length - 1))
                                            ? String.Empty
                                            : GridLineClassForHeader);

                        cell.HorizontalAlign = HorizontalAlign.Center;
                        cell.Text = optionText;
                        controlsPlace.Controls.Add(cell);
                    }
                }
            }
        }




        protected void ColumnHeaderRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MatrixItemColumn matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
            PlaceHolder controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;
            int colspan = 1;
            TableCell cell = new TableCell();

            int rowspan = DoesConsistRatingScaleColumn ? 3 : (DoesConsistRadioButtonsColumn || DoesConsistCheckBoxesColumn) ? 2 : 1;
            cell.CssClass = "columnHeader " + GridLineClassForHeader;
            cell.RowSpan = rowspan;

            if (matrixItemColumn.ColumnNumber != MatrixItem.LabelColumnIndex)
            {
                if (!matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
                    colspan = Math.Max(matrixItemColumn.OptionTexts.Length, 1);

                if (matrixItemColumn.OptionTexts.Length > 0 && !matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
                    rowspan = matrixItemColumn.ColumnType.Equals("RadioButtonScale", StringComparison.InvariantCultureIgnoreCase) ? 1 : DoesConsistRatingScaleColumn ? 2 : 1;

                cell.Text = matrixItemColumn.HeaderText;
                cell.ColumnSpan = colspan;
                cell.RowSpan = rowspan;
                if (matrixItemColumn.Width.HasValue)
                    cell.Width = new Unit(matrixItemColumn.Width.Value);

            }
            controlsPlace.Controls.Add(cell);
        }




        protected void EditColumnRepeater_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MatrixItemColumn matrixItemColumn = e.Item.DataItem as MatrixItemColumn;
            PlaceHolder controlsPlace = e.Item.FindControl("_controlsPlace") as PlaceHolder;
            int colspan = 1;

            if (!matrixItemColumn.ColumnType.Equals("DropdownList", StringComparison.InvariantCultureIgnoreCase))
                colspan = Math.Max(matrixItemColumn.OptionTexts.Length, 1);

            TableCell cell = new TableCell();
            cell.CssClass = "columnHeader " + GridLineClassForHeader;
            cell.ColumnSpan = colspan;


            if (matrixItemColumn.ColumnNumber != 1)
            {
                ImageButton imageButton = new ImageButton();
                imageButton.Attributes.Add("style", "margin-left:5px; margin-right:5px;");
                imageButton.ID = "_leftButton";
                imageButton.ImageUrl = HttpContext.Current.Request.ApplicationPath +
                                       "/App_Themes/CheckboxTheme/Images/left16.gif";
                imageButton.CommandName = "ToLeft";
                imageButton.CommandArgument = matrixItemColumn.ColumnNumber.ToString();
                cell.Controls.Add(imageButton);
            }

            if (matrixItemColumn.ColumnNumber != MatrixItem.LabelColumnIndex)
            {
                ImageButton imageButton = new ImageButton();
                ImageButton editColumnButton = new ImageButton();

                imageButton.Attributes.Add("style", "margin-left:5px; margin-right:5px;");
                imageButton.ID = "_deleteButton";
                imageButton.ImageUrl = HttpContext.Current.Request.ApplicationPath +
                                       "/App_Themes/CheckboxTheme/Images/delete16.gif";
                imageButton.CommandName = "Delete";
                imageButton.CommandArgument = matrixItemColumn.ColumnNumber.ToString();



                editColumnButton.Attributes.Add("style", "margin-left:5px; margin-right:5px;");
                editColumnButton.ID = "_editColumnButton";
                editColumnButton.ImageUrl = HttpContext.Current.Request.ApplicationPath +
                                       "/App_Themes/CheckboxTheme/Images/edit16.gif";
                editColumnButton.CommandName = "Edit";
                editColumnButton.CommandArgument = matrixItemColumn.PrototypeItemId.ToString();

                cell.Controls.Add(editColumnButton);
                cell.Controls.Add(imageButton);

            }

            if (matrixItemColumn.ColumnNumber != MatrixItem.Columns.Length)
            {
                ImageButton imageButton = new ImageButton();
                imageButton.Attributes.Add("style", "margin-left:5px; margin-right:5px;");
                imageButton.ID = "_rightButton";
                imageButton.ImageUrl = HttpContext.Current.Request.ApplicationPath +
                                       "/App_Themes/CheckboxTheme/Images/right16.gif";
                imageButton.CommandName = "ToRight";
                imageButton.CommandArgument = matrixItemColumn.ColumnNumber.ToString();
                cell.Controls.Add(imageButton);

            }

            controlsPlace.Controls.Add(cell);
        }




        protected void EditColumnRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int commandArgument = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "Delete":
                    MatrixItemTextDecorator decorator = MatrixItemData.CreateTextDecorator("en-US") as MatrixItemTextDecorator;
                    decorator.Data.RemoveColumn(commandArgument);
                    break;
                case "ToLeft":
                    MatrixItemData.MoveColumn(commandArgument, commandArgument - 1);
                    break;
                case "ToRight":
                    MatrixItemData.MoveColumn(commandArgument, commandArgument + 1);
                    break;
                case "Edit":
                    TextDecorator.Save();
                    Response.Redirect(string.Format("EditItem.aspx?i={0}&p=-1&s={1}", commandArgument, TemplateId));
                    break;

            }
            IsModelOld = true;
            DataBind();
        }


    }
}