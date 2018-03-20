using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemRenderers
{
    /// <summary>
    /// Simple control to display a rendering error
    /// </summary>
    public partial class RendererLoadError : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="ex"></param>
        public void Initialize(Exception ex)
        {
            _errorMsg.Exception = ex;
        }
    }
}