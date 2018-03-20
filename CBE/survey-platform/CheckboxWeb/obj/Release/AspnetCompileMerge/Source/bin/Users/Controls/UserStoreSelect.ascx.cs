using System;
using System.Web.UI;

namespace CheckboxWeb.Users.Controls 
{
    public partial class UserStoreSelect : Checkbox.Web.Common.UserControlBase 
    {
        public bool ShowLabel { get; set; }

        public string EventID { set; get; }

        public override string ClientID
        {
            get
            {
                return _userStoreSelectPanel.ClientID;
            }
        }

        protected void Page_Load(object sender, EventArgs e) 
        {
        }
    }
}