using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Controls.Charts
{
    public partial class CrossTabItemTemplate : Checkbox.Web.Common.UserControlBase, ITemplate
    {
        public void InstantiateIn(Control container)
        {
            container.Controls.Add(_templatePlace);
        }
    }
}