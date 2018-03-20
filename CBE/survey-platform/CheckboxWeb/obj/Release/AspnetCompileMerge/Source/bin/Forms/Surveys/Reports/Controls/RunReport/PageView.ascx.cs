using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.PageLayout;
using Checkbox.Forms.PageLayout.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Progress.DatabaseProvider;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.Forms.UI.Templates;
using Page = System.Web.UI.Page;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport
{
    /// <summary>
    /// Get run time data for an item in a report
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public delegate IItemProxyObject GetReportItemCallback(int itemId);

    /// <summary>
    /// 
    /// </summary>
    public partial class PageView : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/set layout template
        /// </summary>
        public UserControlLayoutTemplate LayoutTemplate { get; set; }

        /// <summary>
        /// Callback for loading items, if necessary
        /// </summary>
        private GetReportItemCallback GetItemCallback { get; set; }

        /// <summary>
        /// List of item renderers
        /// </summary>
        private List<IItemRenderer> ItemRenderers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int? LayoutTemplateId { get; set; }

        /// <summary>
        /// Initialize page view
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="layoutTemplateId"></param>
        /// <param name="itemCallback"></param>
        /// <param name="languageCode"></param>
        /// <param name="progressKey"> </param>
        /// <param name="exportMode"> </param>
        /// <param name="itemIds"></param>
        public void Initialize(int reportId, int? layoutTemplateId, GetReportItemCallback itemCallback, string languageCode, string progressKey, ExportMode exportMode, IList<int> itemIds)
        {
            GetItemCallback = itemCallback;

            LayoutTemplateId = LayoutTemplateId;

            //Add layout template to control hierarchy
            AddLayoutTemplate(layoutTemplateId, languageCode);
            AddItems(itemIds, progressKey, exportMode);
        }

        
        /// <summary>
        /// Bind renderers
        /// </summary>
        public void BindRenderers()
        {
            if (ItemRenderers != null)
            {
                foreach (IItemRenderer itemRenderer in ItemRenderers)
                {
                    itemRenderer.BindModel();
                }
            }
        }

        /// <summary>
        /// Add items with the specified ids to the page
        /// </summary>
        /// <param name="itemIds"></param>
        /// <param name="progressKey"> </param>
        /// <param name="exportMode"> </param>
        private void AddItems(IEnumerable<int> itemIds, string progressKey, ExportMode exportMode)
        {
            //Build list of items
            List<IItemProxyObject> items = new List<IItemProxyObject>();

            if (itemIds != null && GetItemCallback != null)
            {
                int index = 1;
                int step = 100 / (itemIds.Count() + 1);  
                /*
                //item types that should be skipped on pdf print
                List<string> restrictedTypes = new List<string>
                        {
                            "Details",
                            "Frequency"
                        };*/

                foreach (var itemId in itemIds)
                {
                    if (exportMode == ExportMode.Pdf)
                    {
                        ProgressProvider.SetProgress(progressKey, TextManager.GetText(
                            "/controlText/analysisData/pdfExport/getResponseData"), step * index++, null);

                        /*string type = ItemConfigurationManager.GetItemTypeName(itemId);

                        if (restrictedTypes.Contains(type))
                            continue;*/
                    }

                    var itemData = GetItemCallback(itemId);
                    if (itemData != null)
                        items.Add(itemData);
                }                
            }

            AddItems(items, exportMode);
        }

        /// <summary>
        /// Add list of items to page
        /// </summary>
        /// <param name="items"></param>
        /// <param name="exportMode"></param>
        private void AddItems(IEnumerable<IItemProxyObject> items, ExportMode exportMode)
        {
            ItemRenderers = new List<IItemRenderer>();

            bool isFirst = true;
            //Now, get a renderer for each item and add to layout, if any
            foreach (IItemProxyObject item in items)
            {
                //Get a renderer for the item
                Control renderer = WebItemRendererManager.GetItemRenderer(item, RenderMode.Report, null, exportMode);

                if (renderer != null)
                {
                    renderer.ID = "Renderer_" + item.ItemId;

                    //Add to layout or to page directly
                    if (LayoutTemplate != null)
                    {
                        ILayoutZone itemZone = LayoutTemplate.GetItemZone(item.ItemId);

                        if (itemZone != null)
                        {
                            //Add spacer & renderer
                            if (!isFirst)
                            {
                                LayoutTemplate.AddControlToZone(itemZone.ZoneName, new Panel { CssClass = "Page", ID = "Spacer" + item.ItemId });       //CSS class name is a legacy of pre-5.0
                            }
                            else
                            {
                                isFirst = false;
                            }
                            LayoutTemplate.AddControlToZone(itemZone.ZoneName, GetWrappedRenderer(renderer));
                        }
                    }

                    var itemRenderer = renderer as IItemRenderer;
                    if (itemRenderer != null)
                    {
                        ItemRenderers.Add(itemRenderer);
                    }
                }
            }
        }

        /// <summary>
        /// Wrap renderer in a panel
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        private static Control GetWrappedRenderer(Control renderer)
        {
            var container = new Panel { ID = renderer.ID + "_Wrapper" };

            container.Style["clear"] = "both";

            container.Controls.Add(renderer);

            return container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public void AddPageNumberControl(Control control)
        {
            if (LayoutTemplate == null || control == null)
            {
                return;
            }

            AddControlToLayout(control, LayoutTemplate.PageNumberZone, LayoutTemplate);
        }

        /// <summary>
        /// Add layout template
        /// </summary>
        /// <param name="layoutTemplateId"></param>
        /// <param name="languageCode"></param>
        private void AddLayoutTemplate(int? layoutTemplateId, string languageCode)
        {
            //Get template for the page
            LayoutTemplate = GetLayoutTemplate(layoutTemplateId, languageCode);

            if (LayoutTemplate == null)
            {
                return;
            }

            LayoutTemplate.ID = "Layout_" + LayoutTemplate.ID;

            //Clear layout place
            _pageLayoutPlace.Controls.Clear();

            //Add template to controls collection
            _pageLayoutPlace.Controls.Add(LayoutTemplate);

            //Add controls to layout
            AddControlToLayout(_titlePanel, LayoutTemplate.TitleZone, LayoutTemplate);
        }

        /// <summary>
        /// Add a control to the layout
        /// </summary>
        /// <param name="c"></param>
        /// <param name="zone"></param>
        /// <param name="layoutTemplate"></param>
        private static void AddControlToLayout(Control c, ILayoutZone zone, IWebLayoutTemplate layoutTemplate)
        {
            if (c != null && zone != null)
            {
                layoutTemplate.AddControlToZone(zone.ZoneName, c);
            }
        }

        /// <summary>
        /// Get layout template to use for the page.
        /// </summary>
        /// <param name="layoutTemplateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static UserControlLayoutTemplate GetLayoutTemplate(int? layoutTemplateId, string languageCode)
        {
            if (layoutTemplateId.HasValue)
            {
                PageLayoutTemplateData layoutTemplateData = PageLayoutTemplateManager.GetPageLayoutTemplateData(layoutTemplateId.Value);

                object layoutTemplate = layoutTemplateData.CreateTemplate(languageCode);

                if (layoutTemplate is UserControlLayoutTemplate)
                {
                    return (UserControlLayoutTemplate)layoutTemplate;
                }
            }
            else
            {
                if (HttpContext.Current != null
                    && HttpContext.Current.Handler is Page)
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath("~/Forms/Surveys/Reports/Controls/RunReport/Templates/DefaultTemplate.ascx")))
                    {
                        Control defaultTemplate = ((Page)HttpContext.Current.Handler).LoadControl("~/Forms/Surveys/Reports/Controls/RunReport/Templates/DefaultTemplate.ascx");

                        if (defaultTemplate is UserControlLayoutTemplate)
                        {
                            return (UserControlLayoutTemplate)defaultTemplate;
                        }
                    }
                }
            }

            return null;
        }
    }
}