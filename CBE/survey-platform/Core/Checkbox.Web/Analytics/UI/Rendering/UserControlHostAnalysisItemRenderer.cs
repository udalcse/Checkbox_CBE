using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Web.Forms.UI.Rendering;

namespace Checkbox.Web.Analytics.UI.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class UserControlHostAnalysisItemRenderer : UserControlHostItemRenderer
    {
        /// <summary>
        /// Get control base path as absolute path from application root. e.g. /Forms/Surveys/Controls/ItemEditors
        /// </summary>
        protected override string ControlBasePath { get { return "/Forms/Surveys/Reports/Controls/ItemRenderers"; } }
    }
}
