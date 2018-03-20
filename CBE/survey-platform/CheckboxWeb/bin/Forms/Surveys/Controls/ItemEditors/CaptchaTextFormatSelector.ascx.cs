using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Common.Captcha;
using Checkbox.Common.Captcha.Image;
using Checkbox.Management;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Text format selector for captcha item
    /// </summary>
    public partial class CaptchaTextFormatSelector : Checkbox.Web.Common.UserControlBase
    {
      
        /// <summary>
        /// Initialize with list of selected formats.
        /// </summary>
        /// <param name="selectedFormats"></param>
        public void Initialize(List<TextStyleEnum> selectedFormats)
        {
            List<string> availableFormatTypes = new List<string>(Enum.GetNames(typeof(TextStyleEnum)));
            availableFormatTypes.Sort();

            List<string> selectedFormatTypes = new List<string>();

            foreach (TextStyleEnum textStyle in selectedFormats)
            {
                selectedFormatTypes.Add(textStyle.ToString());

                if (availableFormatTypes.Contains(textStyle.ToString()))
                {
                    availableFormatTypes.Remove(textStyle.ToString());
                }
            }

            selectedFormatTypes.Sort();

            //All formats
            List<string> allFormatTypes = new List<string>(selectedFormatTypes);
            allFormatTypes.AddRange(availableFormatTypes);

            //Data bind
            _formatList.DataSource = allFormatTypes;
            _formatList.DataBind();

            //Set selected
            for (int i = 0; i < selectedFormatTypes.Count; i++)
            {
                if (i >= _formatList.Items.Count)
                {
                    break;
                }

                _formatList.Items[i].Selected = true;
            }
        }

        /// <summary>
        /// Get list of selected formats
        /// </summary>
        /// <returns></returns>
        public List<TextStyleEnum> GetSelectedFormats()
        {
            List<TextStyleEnum> formats = new List<TextStyleEnum>();

            foreach (ListItem item in _formatList.Items)
            {
                if (item.Selected)
                {
                    formats.Add((TextStyleEnum)Enum.Parse(typeof(TextStyleEnum), item.Value));
                }
            }

            return formats;
        }
    }
}