using System.Web.UI;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Configuration editor for image item
    /// </summary>
    public partial class ImageOptions : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize the image options editor
        /// </summary>
        /// <param name="textDecorator"></param>
        public void Initialize(ImageItemTextDecorator textDecorator)
        {
            _altTextTxt.Text = textDecorator.AlternateText;
        }

        /// <summary>
        /// Update item data
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(ImageItemTextDecorator textDecorator)
        {
            textDecorator.AlternateText = _altTextTxt.Text;
        }
    }
}