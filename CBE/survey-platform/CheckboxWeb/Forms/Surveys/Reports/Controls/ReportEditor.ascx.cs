using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using Checkbox.Web.UI.Controls;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportEditor : UserControl
    {
        /// <summary>
        /// Binding object
        /// </summary>
        protected class ItemBindingObject
        {
            public int ItemId { get; set; }
            public int PageNumber { get; set; }
            public string ItemTypeName { get; set; }
            public string FilterText { get; set; }
        }

        /// <summary>
        /// Get/set data source for editor
        /// </summary>
        public List<TemplatePage> DataSource
        {
            get { return (List<TemplatePage>)_pageRepeater.DataSource; }
            set
            {
                _pageRepeater.DataSource = value;
            }
        }

        /// <summary>
        /// Get/set response template associated with this editor instance
        /// </summary>
        public AnalysisTemplate ReportTemplate { get; set; }

        public int? ReportId { get; set; }
        public int? ItemId { get; set; }
        public int? PageNumber { get; set; }

        /// <summary>
        /// Bind to repeater events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReportId = Utilities.AsInt(Request.QueryString["r"]);
            ItemId = Utilities.AsInt(Request.QueryString["i"]);
            PageNumber = Utilities.AsInt(Request.QueryString["p"]);

            //Bind item data bound events so nested item repeater can be bound to data
            _pageRepeater.ItemDataBound += _pageRepeater_ItemDataBound;
        }

        /// <summary>
        /// Override load to bind javascript for loading item renderers via AJAX
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (RepeaterItem pageRepeaterItem in _pageRepeater.Items)
            {
                var itemRepeater = pageRepeaterItem.FindControl("_itemRepeater") as Repeater;

                if (itemRepeater == null)
                {
                    continue;
                }

                foreach (RepeaterItem itemRepeaterItem in itemRepeater.Items)
                {
                    //Ensure renderer place exists
                    var rendererPanel = itemRepeaterItem.FindControl("_itemRendererPlace") as Panel;
                    var loadingPanel = itemRepeaterItem.FindControl("_itemLoadingPanel") as Panel;

                    if (rendererPanel == null
                        || loadingPanel == null
                        || Utilities.IsNullOrEmpty(rendererPanel.Attributes["ItemId"]))
                    {
                        continue;
                    }

                    //Emit Javascript for Ajax loading of renderer HTML.  Requires appropriate javascript on containing page: public static string GetItemRendererHtml(int itemId)
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "queueItemRendererLoad_" + rendererPanel.Attributes["ItemId"],
                        "queueLoadItemHtml(" + rendererPanel.Attributes["ItemId"] + ", '" + rendererPanel.ClientID +
                        "', '" + loadingPanel.ClientID + "');",
                        true);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public override void DataBind()
        {
            base.DataBind();

            List<FilterData> filterObjects = ReportTemplate.GetFilterDataObjects();

            _noFiltersLbl.Visible = filterObjects.Count == 0;
            _filterTxtLbl.Visible = !_noFiltersLbl.Visible;

            if (_filterTxtLbl.Visible)
            {
                var textBuilder = new StringBuilder();

                foreach (FilterData filterDataObject in filterObjects)
                {
                    //TODO: Language code
                    textBuilder.Append(filterDataObject.ToString("en-US"));
                    textBuilder.AppendLine("<br />");
                }

                _filterTxtLbl.Text = textBuilder.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem
               || e.Item.ItemType == ListItemType.Item)
            {

                //Get page to show in repeater
                var tp = e.Item.DataItem as TemplatePage;

                if (tp == null)
                {
                    return;
                }

                BindPagePositionLabel(tp, e);
                BindItemRepeater(tp, e);
            }

            //if (e.Item.ItemType == ListItemType.AlternatingItem
            //    || e.Item.ItemType == ListItemType.Item)
            //{

            //    //Get page to show in repeater
            //    TemplatePage tp = e.Item.DataItem as TemplatePage;

            //    if (tp == null)
            //    {
            //        return;
            //    }

            //    //Set page position title
            //    Label pagePositionLabel = e.Item.FindControl("_pagePositionLbl") as Label;

            //    if (pagePositionLabel != null)
            //    {
            //        pagePositionLabel.Text = tp.Position.ToString();
            //    }

            //    //Populate items
            //    Repeater itemRepeater = e.Item.FindControl("_itemRepeater") as Repeater;

            //    if (itemRepeater != null)
            //    {
            //        //Bind item created to insert item renderer
            //        itemRepeater.ItemCreated += itemRepeater_ItemCreated;

            //        int[] itemIds = tp.ListItemIds();

            //        List<ItemBindingObject> itemBindingObjects = new List<ItemBindingObject>();

            //        foreach (int itemId in itemIds)
            //        {
            //            ItemData itemData = ItemConfigurationManager.GetConfigurationData(itemId);

            //            itemBindingObjects.Add(new ItemBindingObject
            //            {
            //                ItemId = itemId,
            //                PageNumber = tp.Position,
            //                ItemTypeName = itemData.ItemTypeName,
            //                FilterText = GetItemFilterText(itemData)
            //            });


            //        }

            //        itemRepeater.DataSource = itemBindingObjects;
            //        itemRepeater.DataBind();
            //    }
            //}
        }

        /// <summary>
        /// Bind page position label of page repeater
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageRepeaterEventArgs"></param>
        private static void BindPagePositionLabel(TemplatePage page, RepeaterItemEventArgs pageRepeaterEventArgs)
        {
            //Set page position title
            var pagePositionLabel = pageRepeaterEventArgs.Item.FindControl("_pagePositionLbl") as Label;

            if (pagePositionLabel == null)
            {
                return;
            }
            if (page.PageType == TemplatePageType.HiddenItems)
            {
                pagePositionLabel.Text = WebTextManager.GetText("/enum/templatePageType/hiddenItems");
            }

            if (page.PageType == TemplatePageType.Completion)
            {
                pagePositionLabel.Text = WebTextManager.GetText("/enum/templatePageType/completion");
            }

            if (page.PageType == TemplatePageType.ContentPage)
            {
                //Hidden items has position 1, but is not "first page" of survey, so account
                // for that by subtracting 1.
                pagePositionLabel.Text = (page.Position - 1).ToString();
            }
        }

        /// <summary>
        /// Bind item repeater within a page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageRepeaterEventArgs"></param>
        private void BindItemRepeater(TemplatePage page, RepeaterItemEventArgs pageRepeaterEventArgs)
        {
            //Populate items
            var itemRepeater = pageRepeaterEventArgs.Item.FindControl("_itemRepeater") as Repeater;

            if (itemRepeater == null)
            {
                return;
            }

            //Bind item created to insert item renderer
            itemRepeater.ItemCreated += itemRepeater_ItemCreated;

            int[] itemIds = page.ListItemIds();

            var itemBindingObjects = itemIds.Select(itemId => new ItemBindingObject
            {
                ItemId = itemId,
                PageNumber = page.Position,
                ItemTypeName = ItemConfigurationManager.GetItemTypeName(itemId),
                FilterText =  GetItemFilterText(itemId)
            }).ToList();

            itemRepeater.DataSource = itemBindingObjects;
            itemRepeater.DataBind();
        }
        
        /// <summary>
        /// Get filter text for an item
        /// </summary>
        protected string GetItemFilterText(int itemId)
        {
            var itemData = ItemConfigurationManager.GetConfigurationData(itemId);

            if(itemData is AnalysisItemData)
            {
                var textBuilder = new StringBuilder();

                foreach (FilterData filterDataObject in ((AnalysisItemData)itemData).Filters)
                {
                    //TODO: Language code
                    textBuilder.Append(filterDataObject.ToString("en-US"));
                    textBuilder.AppendLine("<br />");
                }

                return textBuilder.ToString();
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Override item created to add item renderer details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            Panel rendererPlace = null;

            try
            {
                if (e.Item.ItemType == ListItemType.Item
                    || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    //Ensure renderer place exists
                    rendererPlace = e.Item.FindControl("_itemRendererPlace") as Panel;

                    if (rendererPlace == null)
                    {
                        return;
                    }

                    int itemId = ((ItemBindingObject)e.Item.DataItem).ItemId;

                    rendererPlace.Attributes["ItemId"] = itemId.ToString();
                }
            }
            catch (Exception ex)
            {
                if (rendererPlace != null)
                {
                    rendererPlace.Controls.Add(new ErrorMessage
                    {
                        Message = ex.Message,
                        Exception = ex,
                        Visible = true
                    });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void itemRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                if ("DeleteItem".Equals(e.CommandName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ReportId.HasValue && PageNumber.HasValue)
                    {
                        string[] values = e.CommandArgument.ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        int itemId = Convert.ToInt32(values[0]);
                        int pageNumber = Convert.ToInt32(values[1]);
                        AnalysisTemplate template = AnalysisTemplateManager.GetAnalysisTemplate(ReportId.Value);
                        TemplatePage page = template.GetPageAtPosition(pageNumber);
                        template.DeleteItemFromPage(page.ID.Value, itemId);
                        template.Save();
                    }
                }
            }
            Response.Redirect(Request.Url.AbsoluteUri);
        }
    }
}