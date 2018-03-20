using System;
using System.Web.UI;
using Checkbox.Web;
using Checkbox.Forms;
using Checkbox.Web.Page;
using Checkbox.Forms.Items;
using Checkbox.Web.UI.Controls;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Libraries
{
    /// <summary>
    /// Summary description for ViewItem.
    /// </summary>
    public partial class ViewItem : SecuredPage
    {
        /// <summary>
        /// Require view library permission
        /// </summary>
        protected override string PageRequiredRolePermission { get { return "Library.View"; } }

        [QueryParameter("i", "-1")]
        public int ItemId { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Bind & hide ui elements
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            if (ItemId < 0)
            {
                var msg = new ErrorMessage
                              {
                                  Message = WebTextManager.GetText("/pageText/viewItem.aspx/errorOccurred"),
                                  SubMessage = WebTextManager.GetText("/pageText/viewItem.aspx/noItemID")
                              };

                _itemPreviewPlace.Controls.Add(msg);

                return;
            }

            Master.SetTitle(WebTextManager.GetText("/pageText/Libraries/viewItem.aspx/title"));
            Master.HideDialogButtons();
            try
            {
                ItemData data = ItemConfigurationManager.GetConfigurationData(ItemId);

                if (data == null)
                {
                    var msg = new ErrorMessage
                                  {
                                      Message = WebTextManager.GetText("/pageText/viewItem.aspx/errorOccurred"),
                                      SubMessage = WebTextManager.GetText("/pageText/viewItem.aspx/unableToGetItemData")
                                  };

                    _itemPreviewPlace.Controls.Add(msg);

                    return;
                }

                Item item = data.CreateItem(ViewLanguage, null);
                var renderer = 
                    WebItemRendererManager.GetItemRenderer(
                        item.GetDataTransferObject(),
                        RenderMode.LibraryPreview,
                        null) 
                    as UserControlHostItemRenderer;


                if (renderer == null)
                {
                    _itemPreviewPlace.Controls.Add(new LiteralControl
                                                       {Text = "Renderer was null or not UserControlHostItemRenderer"});
                    return;
                }

                _itemPreviewPlace.Controls.Add(renderer);

                renderer.BindModel();
            }
            catch (Exception ex)
            {
                var msg = new ErrorMessage
                              {
                                  Message = WebTextManager.GetText("/pageText/viewItem.aspx/errorOccurred"),
                                  SubMessage = WebTextManager.GetText(ex.Message)
                              };

                _itemPreviewPlace.Controls.Add(msg);
            }
        }

        /// <summary>
        /// Get the language to use for editing
        /// </summary>
        /// <returns></returns>
        private string ViewLanguage
        {
            get
            {
                if (!string.IsNullOrEmpty(LanguageCode))
                {
                    return LanguageCode;
                }

                return WebTextManager.GetUserLanguage();
            }
        }
    }
}
