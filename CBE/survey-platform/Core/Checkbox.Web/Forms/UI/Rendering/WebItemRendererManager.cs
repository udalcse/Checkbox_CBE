using System;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// Simple exception for renderer load error.
    /// </summary>
    public class RendererLoadException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerException"></param>
        public RendererLoadException(Exception innerException)
            : base("An error occurred while loading an item renderer.", innerException)
        {
        }
    }

    /// <summary>
    /// Manager for accessing item renderers in a web environment.
    /// </summary>
    public static class WebItemRendererManager
    {
        /// <summary>
        /// Get the renderer for an item with the specific id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="templateId"></param>
        /// <param name="renderMode"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static Control GetItemRenderer(int itemId, int? templateId, RenderMode renderMode, string languageCode)
        {
            //Do nothing if no http context
            if (HttpContext.Current == null
                || HttpContext.Current.Server == null)
            {
                return null;
            }

            //Create a control simply to use its load method
            var loaderControl = new UserControl();

            //Get item and find appearance/renderer, etc.
            ItemData itemData = ItemConfigurationManager.GetConfigurationData(itemId);

            if (itemData == null)
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/NoItemData.ascx")
                    : null;
            }

            Item theItem = itemData.CreateItem(languageCode, templateId);

            if (theItem == null)
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                   ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/NoItem.ascx")
                   : null;
            }

            return GetItemRenderer(theItem.GetDataTransferObject(), renderMode, null, ExportMode.Default);
        }

        /// <summary>
        /// Get item renderer
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="renderMode"></param>
        /// <param name="itemPosition"></param>
        /// <param name="exportMode"></param>
        /// <returns></returns>
        public static Control GetItemRenderer(IItemProxyObject dataTransferObject, RenderMode renderMode, int? itemPosition, ExportMode exportMode)
        {
            //TODO: Get appearances through service layer

            //Create a control simply to use its load method
            var loaderControl = new UserControl();

            //Get appearance for item.  Some items have no visual manifestation, so it's not necessarily an 
            // error condition if there is no appearance.
            AppearanceData itemAppearanceData = AppearanceDataManager.GetAppearanceDataForItem(dataTransferObject.ItemId);

            if (itemAppearanceData == null)
            {
                //Return a message in some cases where there is no appearance.  Otherwise return null.
                return renderMode == RenderMode.ReportEditor || renderMode == RenderMode.SurveyEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/NoAppearance.ascx")
                    : null;
            }

            //Load renderer based on appearance.

            IItemRenderer renderer = null;

            try
            {
                renderer = ItemRendererFactory.Create(itemAppearanceData.AppearanceCode);
            }
            catch (Exception ex)
            {
                throw new RendererLoadException(ex);
            }

            if (renderer == null)
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/NoRenderer.ascx")
                    : null;
            }

            //Make sure renderer is a control of some sort
            if (!(renderer is Control))
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/RendererIsNotControl.ascx")
                    : null;
            }

            //TODO: 
            //Add appearance data to item dto. This step should go away once it is possible to get appearance
            // data through service layer
            if(dataTransferObject is ItemProxyObject)
            {
                ((ItemProxyObject)dataTransferObject).AppearanceData = itemAppearanceData.GetPropertiesAsNameValueCollection();
            }

            //Initialize
            renderer.Initialize(dataTransferObject, itemPosition, renderMode, exportMode);

            return (Control)renderer;
        }

        /// <summary>
        /// Get item renderer
        /// </summary>
        /// <param name="dataTransferObject"></param>
        /// <param name="renderMode"></param>
        /// <param name="itemPosition"></param>
        /// <returns></returns>
        public static Control GetItemRenderer(IItemProxyObject dataTransferObject, RenderMode renderMode, int? itemPosition)
        {
            return GetItemRenderer(dataTransferObject, renderMode, itemPosition, ExportMode.None);
        }

        /// <summary>
        /// Get item renderer
        /// </summary>
        /// <param name="itemAppearanceData"></param>
        /// <param name="renderMode"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Control GetItemRenderer(int itemId, AppearanceData itemAppearanceData, RenderMode renderMode, bool ignoreItemId = false)
        {
            if (!ignoreItemId)
            {


                if (itemId <= 0
                    || itemAppearanceData == null)
                {
                    return null;
                }
            }

            //Create a control simply to use its load method
            var loaderControl = new UserControl();

            //Load renderer based on appearance.
            IItemRenderer renderer = null;

            try
            {
                renderer = ItemRendererFactory.Create(itemAppearanceData.AppearanceCode);
            }
            catch (Exception ex)
            {
                throw new RendererLoadException(ex);
            }

            if (renderer == null)
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/NoRenderer.ascx")
                    : null;
            }

            //Make sure renderer is a control of some sort
            if (!(renderer is Control))
            {
                return renderMode == RenderMode.SurveyEditor || renderMode == RenderMode.ReportEditor
                    ? loaderControl.LoadControl(ApplicationManager.ApplicationRoot + "/forms/surveys/controls/itemRenderers/RendererIsNotControl.ascx")
                    : null;
            }

            return (Control)renderer;
        }
    }
}
