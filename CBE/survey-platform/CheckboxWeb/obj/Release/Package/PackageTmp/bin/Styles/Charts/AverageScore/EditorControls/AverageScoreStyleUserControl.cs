using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Web;

namespace CheckboxWeb.Styles.Charts.AverageScore.EditorControls
{
    public class AverageScoreStyleUserControl: Checkbox.Web.Common.UserControlBase
    {
        public AverageScoreItemAppearanceData Appearance { get; private set; }


        /// <summary>
        /// Initialize with appearance to edit
        /// </summary>
        /// <param name="appearance"></param>
        public virtual void Initialize(AverageScoreItemAppearanceData appearance)
        {
            Appearance = appearance;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadStyleValues();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateAppearanceValues();
        }

        /// <summary>
        /// Apply style values to inputs.
        /// </summary>
        protected virtual void LoadStyleValues() { }

        /// <summary>
        /// Update appearance with input values
        /// </summary>
        protected virtual void UpdateAppearanceValues() { }

        /// <summary>
        /// Bind a particular list to items, also specify the selected value
        /// </summary>
        /// <param name="list"></param>
        /// <param name="listItems"></param>
        /// <param name="selectedValue"></param>
        protected void BindList(ListControl list, IEnumerable<ListItem> listItems, string selectedValue)
        {
            foreach (ListItem item in listItems)
            {
                list.Items.Add(item);
            }

            if (selectedValue != null && list.Items.FindByValue(selectedValue) != null)
            {
                list.SelectedValue = selectedValue;
            }
        }

        /// <summary>
        /// Get enum items
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        protected List<ListItem> GetEnumListItems(Type enumType)
        {
            List<ListItem> items = new List<ListItem>();

            string[] enumNames = Enum.GetNames(enumType);

            foreach (string enumName in enumNames)
            {
                string localizedName = WebTextManager.GetText("/enum/" + enumType.Name + "/" + enumName);

                if (Utilities.IsNullOrEmpty(localizedName))
                {
                    localizedName = enumName;
                }

                items.Add(new ListItem(localizedName, enumName));
            }

            return items;
        }

        /// <summary>
        /// Get an array of Colors from a csv list
        /// </summary>
        /// <param name="csvColorList"></param>
        /// <returns></returns>
        protected Color[] GetColorArray(string csvColorList)
        {
            List<Color> colors = new List<Color>();

            foreach (string color in csvColorList.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                colors.Add(Utilities.GetColor(color, false));

            return colors.ToArray();
        }

        /// <summary>
        /// Get a list of font family names that support all specified styles
        /// </summary>
        /// <param name="requiredStyles"></param>
        /// <returns></returns>
        protected List<ListItem> GetFontFamilyNameListItems(params FontStyle[] requiredStyles)
        {
            List<ListItem> familyNames = new List<ListItem>();

            foreach (FontFamily family in FontFamily.Families)
            {
                bool supported = true;

                foreach (FontStyle style in requiredStyles)
                {
                    if (!family.IsStyleAvailable(style))
                    {
                        supported = false;
                        break;
                    }
                }

                if (supported)
                {
                    familyNames.Add(new ListItem(family.Name, family.Name));
                }
            }

            return familyNames;
        }

        /// <summary>
        /// Get list items for font sizes
        /// </summary>
        /// <param name="smallest"></param>
        /// <param name="largest"></param>
        /// <param name="step"></param>
        /// <param name="sizeSuffix"></param>
        /// <returns></returns>
        protected List<ListItem> GetFontSizeListItems(int smallest, int largest, int step, string sizeSuffix)
        {
            List<ListItem> items = new List<ListItem>();

            for (int i = smallest; i <= largest; i = i + step)
            {
                items.Add(new ListItem(i + " " + sizeSuffix, i.ToString()));
            }

            return items;
        }
    }
}