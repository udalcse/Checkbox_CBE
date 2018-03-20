using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Settings.Controls
{
    public partial class TimelineSettings : Checkbox.Web.Common.UserControlBase
    {
        public string Manager
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}