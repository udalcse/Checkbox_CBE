using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ResponseProperties : Checkbox.Web.Common.UserControlBase
    {
        public bool IncludeIncompleteResponses
        {
            get
            {
                return _includeIncomplete.Checked;
            }
            set
            {
                _includeIncomplete.Checked = value;
            }
        }
        
        public bool IncludeTestResponses
        {
            get
            {
                return _includeTest.Checked;
            }
            set
            {
                _includeTest.Checked = value;
            }
        }
    }
}