using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Controls.ControlsForRepeater
{
    /// <summary>
    /// This radioButton can be used in repeater control. When it is used in group, only one can be selected.
    /// </summary>
    public class RadioButtonForRepeater : RadioButton
    {
        /// <summary>
        /// This value is used to identify each radioButton
        /// </summary>
        protected string Value
        {
            get { return ClientID; }
        }

        /// <summary>
        /// Override render method to assign "name" attribute explicitly.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, GroupName);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Value, Value);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");

            if (Checked)
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");

            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        /// <summary>
        /// Override LoadPostData to process postback data.
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        protected override bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            bool isChanged;
            String selectedItemValue = postCollection[GroupName];

            if (selectedItemValue == Value)
            {
                isChanged = !Checked;
                Checked = true;
            }
            else
            {
                isChanged = Checked;
                Checked = false;
            }

            return isChanged;
        }
    }
}