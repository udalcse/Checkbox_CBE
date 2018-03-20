using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Security;

namespace CheckboxWeb.Controls.Piping
{
    public partial class QuestionBindingControl : UserControl
    {
        public static int ItemId { get; set; }

        public void Initialize(Dictionary<int, string> profileKeys)
        {
            _questionBinding.Items.Add(new ListItem("None", "0"));
            foreach (var key in profileKeys)
            {
                if (string.IsNullOrEmpty(key.Value))
                {
                    continue;
                }
                _questionBinding.Items.Add(new ListItem(key.Value, key.Key.ToString()));
            }

            if (CanGetIdFromRequest())
            {
                var profileProperty = ProfileManager.GetPropertiesList().FirstOrDefault(x => x.BindedItemId.Any(i => i == ItemId));
                var fieldName = profileProperty != null ? profileProperty.Name : "None";

                int index = 0;
                foreach (var item in profileKeys)
                {
                    index++;
                    if (item.Value == fieldName)
                        _questionBinding.SelectedIndex = index;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private static bool CanGetIdFromRequest()
        {
            var context = HttpContext.Current;

            if (context == null)
                return false;

            ItemId = Convert.ToInt32(context.Request.QueryString.Get("i") ?? "0");

            return ItemId != 0;
        }
    }
}