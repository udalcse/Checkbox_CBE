using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Checkbox.Globalization.Text;

namespace Checkbox.Web.UI.Controls
{
    /// <summary>
    /// Get 
    /// </summary>
    public class LocalizedLabelledEnumDropDownList : LocalizedLabelledDropDownList
    {
        private Type _enumType;

        /// <summary>
        /// Get/set the enum type this list represents
        /// </summary>
        public Type EnumType
        {
            get { return _enumType; }
            set 
            { 
                _enumType = value;

                //Load the list items
                AddListItems();
            }
        }

        /// <summary>
        /// Get list items
        /// </summary>
        /// <returns></returns>
        protected override List<ListItem> GetListItems()
        {
            if (EnumType != null)
            {
                Dictionary<string, string> localizedValues = TextManager.GetEnumLocalizedValues(EnumType, LanguageCode);

                List<ListItem> items = new List<ListItem>();

                foreach (string key in localizedValues.Keys)
                {
                    ListItem li = new ListItem(localizedValues[key], key);

                    if (key.Equals(SelectedValue, StringComparison.InvariantCultureIgnoreCase))
                    {
                        li.Selected = true;
                    }

                    items.Add(li);
                }

                return items;
            }
            
            return base.GetListItems();
        }
    }
}
