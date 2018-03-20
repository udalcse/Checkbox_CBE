using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Reports.Filters.Controls
{
    public partial class FilterList : Checkbox.Web.Common.UserControlBase
    {
        public int ResponseTemplateId { set; get; }

        public string FilterSelectedClientCallback { get; set; }
    }
}