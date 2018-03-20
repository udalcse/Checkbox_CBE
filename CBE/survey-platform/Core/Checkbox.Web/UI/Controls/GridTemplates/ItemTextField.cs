using System;
using System.Web.UI.WebControls;

using Checkbox.Web;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Web.UI.Controls.GridTemplates
{
    /// <summary>
    /// Field for displaying an item's text
    /// </summary>
    public class ItemTextField : LocalizedHeaderBoundField
    {
        private string _languageCode;

        /// <summary>
        /// Get/set the language code
        /// </summary>
        public string LanguageCode
        {
            get
            {
                if (Utilities.IsNullOrEmpty(_languageCode))
                {
                    _languageCode = WebTextManager.GetUserLanguage();
                }

                return _languageCode;
            }
            set { _languageCode = value; }
        }

        /// <summary>
        /// Format the data
        /// </summary>
        /// <param name="dataValue"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        protected override string FormatDataValue(object dataValue, bool encode)
        {
            try
            {
                int itemID = Int32.Parse(dataValue.ToString());

                ItemData itemData = ItemConfigurationManager.GetConfigurationData(itemID);

                if (itemData != null)
                {
                    if (itemData is LabelledItemData)
                    {
                        return TextManager.GetText(
                            ((LabelledItemData)itemData).TextID,
                            LanguageCode,
                            WebTextManager.GetText("/itemType/" + itemData.ItemTypeName + "/description"),
                            TextManager.SurveyLanguages);
                    }
                    else if (itemData is MessageItemData)
                    {
                        return TextManager.GetText(
                            ((MessageItemData)itemData).TextID,
                            LanguageCode,
                            WebTextManager.GetText("/itemType/" + itemData.ItemTypeName + "/description"),
                            TextManager.SurveyLanguages);
                    }
                    else if (itemData is ImageItemData)
                    {
                        return "[IMAGE]";
                    }
                    else if (itemData is HtmlItemData)
                    {
                        return ((HtmlItemData)itemData).HTML;
                    }
                    else
                    {
                        return TextManager.GetText("/itemType/" + itemData.ItemTypeName + "/description", LanguageCode, itemData.ItemTypeName, TextManager.ApplicationLanguages);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }
    }
}
