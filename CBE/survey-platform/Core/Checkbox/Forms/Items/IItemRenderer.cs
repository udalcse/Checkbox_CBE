using Checkbox.Common;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Base interface for item renderers
    /// </summary>
    public interface IItemRenderer
    {
        /// <summary>
        /// Get reference to item to render.
        /// </summary>
        IItemProxyObject DataTransferObject { get; }

        /// <summary>
        /// Appearance data for the renderer.
        /// </summary>
        SimpleNameValueCollection Appearance { get; }

        /// <summary>
        /// Bind renderer to item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemPosition"></param>
        /// <param name="renderMode"></param>
        /// <param name="exportMode"></param>
        void Initialize(IItemProxyObject model, int? itemPosition, RenderMode renderMode, ExportMode exportMode);

        /// <summary>
        /// Bind renderer to item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemPosition"></param>
        /// <param name="renderMode"></param>
        void Initialize(IItemProxyObject model, int? itemPosition, RenderMode renderMode);

        /// <summary>
        /// Bind renderer to the item.
        /// </summary>
        void BindModel();

        /// <summary>
        /// Update the model with an user interface or other data.
        /// </summary>
        void UpdateModel();

        /// <summary>
        /// Get whether renderer is visible.
        /// </summary>
        /// <returns></returns>
        bool Visible { get; }
    }
}
