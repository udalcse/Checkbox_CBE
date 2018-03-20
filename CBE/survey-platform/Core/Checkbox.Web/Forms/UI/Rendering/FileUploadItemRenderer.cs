using System.Web;
using Checkbox.Forms;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms.UI.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class FileUploadItemRenderer : UserControlHostItemRenderer
    {
        protected override string GetControlPath(IItemProxyObject model, RenderMode renderMode)
        {
            if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
                ControlNameOverride = "FileUploadAjaxifyed";

            return base.GetControlPath(model, renderMode);
        }
    }
}
