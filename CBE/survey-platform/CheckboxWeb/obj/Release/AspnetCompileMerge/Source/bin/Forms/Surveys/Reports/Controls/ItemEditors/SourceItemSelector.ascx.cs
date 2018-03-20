using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Web.Analytics.UI.Editing;
using CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors
{
    /// <summary>
    /// User control to allow users to select source items for reports.
    /// </summary>
    public partial class SourceItemSelector : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get list of allowed item types.  An empty list means all answerable items are allowed.
        /// </summary>
        public List<string> AllowedItemTypes { get; private set; }

        /// <summary>
        /// Get list of not allowed item types.
        /// </summary>
        public List<string> NotAllowedItemTypes { get; private set; }

        /// <summary>
        /// Allow to select items which include other items (for instance, matrixes)
        /// </summary>
        public bool AllowSelectCompositeItems { get; set; }

        /// <summary>
        /// Get number of item times that are allowed to be selected.  Null value means no limit.
        /// </summary>
        public int? AllowedItemCount { get; private set; }

        /// <summary>
        /// Determine if the control's layout is vertical or horizontal.
        /// </summary>
        public bool VerticalLayout { get; set; }

        /// <summary>
        /// Control's height in px.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Get language code for selector
        /// </summary>
        public string LanguageCode { get; private set; }

        /// <summary>
        /// Get/set text id of title.
        /// </summary>
        public int SourceItemsLimit { get; set; }

        /// <summary>
        /// Get response template id
        /// </summary>
        public int ResponseTemplateId { get; private set; }

        /// <summary>
        /// Get/set text id of title.
        /// </summary>
        public string TitleTextId { get; set; }

        /// <summary>
        /// Get list of selected items
        /// </summary>
        private List<int> SelectedItems { get; set; }

        /// <summary>
        /// Available items to add
        /// </summary>
        protected List<LightweightItemMetaData> AvailableItemMetaData { get; set; }

        /// <summary>
        /// Selected items to add
        /// </summary>
        protected List<LightweightItemMetaData> SelectedItemMetaData { get; set; }

        private SourceListBuilderFormat Builder { get; set; }

        /// <summary>
        /// Calculate the items container height.
        /// </summary>
        protected int ItemsContainerHeight
        {
            get
            {
                if (Height.HasValue)
                {
                    int buttonHeight = _addBtn.Height.Value > 0 ? (int) _addBtn.Height.Value : 23;
                    int height = Height.Value - buttonHeight;

                    if (VerticalLayout)
                        height = (height - buttonHeight) / 2;

                    return height;
                }
                
                return 250;
            }
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="sourceResponseTemplateId">The source response template identifier.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="allowedItemTypes">The allowed item types.</param>
        /// <param name="notAllowedItemTypes">The not allowed item types.</param>
        /// <param name="allowedItemCount">The allowed item count.</param>
        /// <param name="selectedItems">The selected items.</param>
        /// <param name="pageIsPostback">if set to <c>true</c> [page is postback].</param>
        /// <param name="builder">The builder.</param>
        public void Initialize(int sourceResponseTemplateId, string languageCode, List<string> allowedItemTypes, List<string> notAllowedItemTypes, int? allowedItemCount, List<int> selectedItems, bool pageIsPostback, SourceListBuilderFormat builder = SourceListBuilderFormat.Default)
        {
            AllowedItemTypes = allowedItemTypes;
            AllowedItemCount = allowedItemCount;
            LanguageCode = languageCode;
            ResponseTemplateId = sourceResponseTemplateId;
            SelectedItems = selectedItems;
            NotAllowedItemTypes = notAllowedItemTypes;
            Builder = builder;

            SourceItemsLimit = builder == SourceListBuilderFormat.GradientColorDirectorMatrix ? 1 : int.MaxValue;
            
            //Build lists of available items which will be filtered based on currently selected item list passed
            // in. Further adjustment to list based on user updates will be processed in OnLoad
            BuildItemLists();


            if (Page != null)
            {
                Page.Load += Page_Load;
            }

            BindLists();

        }

        protected int? _PrimarySourceItemID;
        /// <summary>
        /// ID of the primary source item
        /// </summary>
        public int? PrimarySourceItemID
        {
            get
            {
                if (!string.IsNullOrEmpty(_primaryItemHiddenID.Value))
                {
                    int pid = int.Parse(_primaryItemHiddenID.Value);
                    if (!SelectedItems.Contains(pid))
                        return null;
                    return pid;
                }
                return null;
            }
            set
            {
                _primaryItemHiddenID.Value = value == null ? "" : value.Value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindLists()
        {
            _availableList.DataSource = ListAvailableItems();
            _availableList.DataBind();

            var selectedItems = ListSelectedItems();
            _selectedList.DataSource = selectedItems;
            _selectedList.DataBind();

            _primaryItem.Items.Clear();
            if (selectedItems.Count > 0)
            {
                _primaryItem.Items.Add(new ListItem(WebTextManager.GetText("/controlText/chartEditor/primarySourceItem/na"), "0"));
                foreach (var i in selectedItems)
                {
                    _primaryItem.Items.Add(new ListItem(
                        string.Format("{0} - {1}", GetItemNumber(i, false), GetItemText(i)),
                        i.ItemId.ToString()));
                }
                _primarySelectorDiv.Visible = true;
            }
            else
            {
                if (_availableList.Items.Count == 0 && PrimarySourceItemID == null) 
                    _primarySelectorDiv.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemMetaData"></param>
        /// <returns></returns>
        protected string GetItemNumber(LightweightItemMetaData itemMetaData, bool useHtml = true)
        {
            //If item is contained in another item, such as a matrix
            // question.
            if (itemMetaData.Coordinate != null)
            {
                return string.Format(
                    useHtml ? "&nbsp;&nbsp;<b>{0}.{1}</b>.{2}.{3}" : "  {0}.{1}.{2}.{3}",
                    itemMetaData.PagePosition - 1,
                    itemMetaData.ItemPosition,
                    itemMetaData.Coordinate.Y,      //Row
                    itemMetaData.Coordinate.X);     //Column
            }

            return string.Format(
                useHtml ? "<b>{0}</b>" : "{0}",
                itemMetaData.PositionText);
        }

        /// <summary>
        /// Get text of item
        /// </summary>
        /// <param name="itemMetaData"></param>
        /// <returns></returns>
        protected string GetItemText(LightweightItemMetaData itemMetaData)
        {
            return Utilities.EncodeTagsInHtmlContent( Utilities.DecodeAndStripHtml(itemMetaData.GetText(false, LanguageCode), 55));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<LightweightItemMetaData> ListAvailableItems()
        {
            return AvailableItemMetaData
                    .OrderBy(data => data.PagePosition)
                    .ThenBy(data => data.ItemPosition)
                    .ThenBy(data => data.Coordinate != null ? data.Coordinate.Y : 0)  //Child row
                    .ThenBy(data => data.Coordinate != null ? data.Coordinate.X : 0) //Child column
                    .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected List<LightweightItemMetaData> ListSelectedItems()
        {
            return SelectedItemMetaData
                    .OrderBy(data => data.PagePosition)
                    .ThenBy(data => data.ItemPosition)
                    .ThenBy(data => data.Coordinate != null ? data.Coordinate.Y : 0)  //Child row
                    .ThenBy(data => data.Coordinate != null ? data.Coordinate.X : 0) //Child column
                    .ToList();
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Load(object sender, EventArgs e)
        {
            //Get list of available items to add to selected list
            var itemIdsToAdd = _itemsToAddTxt.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(val => Utilities.AsInt(val).HasValue)
                .Select(val => Utilities.AsInt(val).Value)
                .Distinct();

            //Get list of available items to remove from selected list
            var itemIdsToRemove = _itemsToRemoveTxt.Text
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(val => Utilities.AsInt(val).HasValue)
                .Select(val => Utilities.AsInt(val).Value)
                .Distinct();

            //Move items from selected to available
            AvailableItemMetaData.AddRange(SelectedItemMetaData.Where(val => itemIdsToRemove.Contains(val.ItemId)));

            //Move items from available to selected
            SelectedItemMetaData.AddRange(AvailableItemMetaData.Where(val => itemIdsToAdd.Contains(val.ItemId)));

            //Remove items
            AvailableItemMetaData.RemoveAll(val => itemIdsToAdd.Contains(val.ItemId));
            SelectedItemMetaData.RemoveAll(val => itemIdsToRemove.Contains(val.ItemId));

            //Update selected items lists
            SelectedItems = SelectedItemMetaData
                .Select(val => val.ItemId)
                .Distinct()
                .ToList();

            _primaryItem.Attributes.Add("title", WebTextManager.GetText("/controlText/chartEditor/primarySourceItem/title"));
        }



        /// <summary>
        /// Bind list of items in survey.
        /// </summary>
        public void BuildItemLists()
        {
            List<LightweightItemMetaData> itemList = ItemConfigurationManager.ListResponseTemplateItems(
                ResponseTemplateId, 
                null,
                false,
                true,
                true, 
                WebTextManager.GetUserLanguage());

            var builder = SourceItemBuilderFactory.CreateBuilder(this.Builder);

            //Build list of non-selected items
            AvailableItemMetaData = builder.Build(ResponseTemplateId, itemList, SelectedItems, AllowedItemTypes, NotAllowedItemTypes);
                //.OrderBy(data => data.PagePosition)
                //.ThenBy(data => data.ItemPosition)
                //.ThenBy(data => data.Coordinate != null ? data.Coordinate.Y : 0) //Child row
                //.ThenBy(data => data.Coordinate != null ? data.Coordinate.X : 0) //Child column
                //.ToList();

            //Build list of selected items
            SelectedItemMetaData = itemList
                .Where(data => SelectedItems.Contains(data.ItemId))
                .ToList();
        }

        /// <summary>
        /// Get list of selected items
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedItems()
        {
            return SelectedItems;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void  UpdateSelectedItems(List<int> items)
        {
            SelectedItems = items ?? new List<int>();

            BuildItemLists();

            BindLists();
        }
    }
}