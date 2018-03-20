using System;
using System.Web;
using System.Web.UI;

using Checkbox.Web;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Base class for user controls that support mementos
    /// </summary>
    public class MementoEnabledUserControl : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WebParameterAttribute.SetValues(this, HttpContext.Current);
        }
        /// <summary>
        /// Save view state
        /// </summary>
        /// <returns></returns>
        protected override object SaveViewState()
        {
            Memento.PersistMementos(this, HttpContext.Current);

            return base.SaveViewState();
        }
    }
}
