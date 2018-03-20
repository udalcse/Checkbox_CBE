using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items;
using CheckboxWeb.Forms.Surveys.Controls.ItemRenderers;
using Checkbox.Management;
using Checkbox.Web.UI.Controls;
using Checkbox.Security.Principal;
using System.Web;

namespace CheckboxWeb.Libraries
{
    public partial class LibraryEditor : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Binding object
        /// </summary>
        protected class ItemBindingObject
        {
            public int ItemId { get; set; }
            public int PageNumber { get; set; }
            public string ItemTypeName { get; set; }
        }

        /// <summary>
        /// Get/set data source for editor
        /// </summary>
        public List<TemplatePage> DataSource
        {
            get { return (List<TemplatePage>)_pageRepeater.DataSource; }
            set { _pageRepeater.DataSource = value; }
        }

        /// <summary>
        /// Get/set survey id
        /// </summary>
        public int? LibraryId { get; set; }

        /// <summary>
        /// Get/set response template associated with this editor instance
        /// </summary>
        public LibraryTemplate LibraryTemplate { get; set; }

        /// <summary>
        /// Bind to repeater events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LibraryId = Convert.ToInt32(Request.QueryString["id"]);

            //Bind item data bound events so nested item repeater can be bound to data
            _pageRepeater.ItemDataBound += _pageRepeater_ItemDataBound;
        }

        /// <summary>
        /// Repeater Item Commands
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void _itemRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                switch (e.CommandName)
                {
                    case "DeleteItem":
                        if (LibraryId.HasValue)
                        {
                            int itemId = Convert.ToInt32(e.CommandArgument);
                            LibraryTemplate template = LibraryTemplateManager.GetLibraryTemplate(LibraryId.Value);
                            TemplatePage page = template.GetPageAtPosition(1);
                            template.DeleteItemFromPage(page.ID.Value, itemId);
                            template.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                            template.Save();
                        }
                        break;
                    case "CopyItem":
                        if (LibraryId.HasValue)
                        {
                            int itemId = Convert.ToInt32(e.CommandArgument);
                            LibraryTemplate template = LibraryTemplateManager.GetLibraryTemplate(LibraryId.Value);
                            template.CopyItem(itemId, (CheckboxPrincipal)HttpContext.Current.User);
                            template.ModifiedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                            template.Save();
                        }
                        break;
                    default:
                        break;
                }
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

                //Set page position title
                var pagePositionLabel = e.Item.FindControl("_pagePositionLbl") as Label;

                if (pagePositionLabel != null)
                {
                    if (tp.PageType == TemplatePageType.ContentPage)
                    {
                        // Libraries only contain one page so we don't do fancy math to the label
                        pagePositionLabel.Text = (tp.Position + 1).ToString();
                    }
                }

                //Populate items
                var itemRepeater = e.Item.FindControl("_itemRepeater") as Repeater;

                if (itemRepeater != null)
                {
                    //Bind item created to insert item renderer
                    itemRepeater.ItemCreated += itemRepeater_ItemCreated;

                    int[] itemIds = tp.ListItemIds();

                    var itemBindingObjects = itemIds.Select(itemId =>
                            new ItemBindingObject
                            {
                                ItemId = itemId,
                                PageNumber = tp.Position,
                                ItemTypeName = ItemConfigurationManager.GetItemTypeName(itemId)
                            }
                        ).ToList();

                    itemRepeater.DataSource = itemBindingObjects;
                    itemRepeater.DataBind();
                }
            }
        }

        /// <summary>
        /// Override item created to add item renderer details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            Control rendererPlace = null;

            try
            {
                if (e.Item.ItemType == ListItemType.Item
                    || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    //Ensure renderer place exists
                    rendererPlace = e.Item.FindControl("_itemRendererPlace");

                    if (rendererPlace == null)
                    {
                        return;
                    }

                    int itemId = ((ItemBindingObject)e.Item.DataItem).ItemId;

                    //TODO: Language code
                    Control renderer = WebItemRendererManager.GetItemRenderer(itemId, LibraryId, RenderMode.SurveyEditor, "en-US");

                    if (renderer == null)
                    {
                        return;
                    }

                    //Add renderer to control hierarchy
                    rendererPlace.Controls.Add(renderer);

                    //If renderer is an item renderer, and not an error control or such,
                    // initialize it
                    if (renderer is IItemRenderer)
                    {
                        ((IItemRenderer)renderer).BindModel();
                    }
                }
            }
            catch (RendererLoadException ex)
            {
                if (rendererPlace != null)
                {
                    var errorCtrl = Page.LoadControl(ApplicationManager.ApplicationRoot + "/Forms/Surveys/Controls/ItemRenderers/RendererLoadError.ascx") as RendererLoadError;

                    if (errorCtrl != null)
                    {
                        errorCtrl.Initialize(ex.InnerException);
                    }

                    rendererPlace.Controls.Add(errorCtrl);
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
    }
}