using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Behavior editor for file upload item
    /// </summary>
    public partial class FileUploadBehavior : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Initialize the editor control
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="isPostBack"></param>
        public void Initialize(UploadItemData itemData, bool isPostBack)
        {
			_aliasText.Text = itemData.Alias;
            //Set required
            _requiredChk.Checked = itemData.IsRequired;

            //Bind list of selected/allowed file types.  Sort lists to have selected items first
            // and then in alphabetical order.
            var allFileTypes = new List<string>(UploadItemManager.AllAllowedFileTypes);
            allFileTypes.Sort();

            var allowedFileTypes = new List<string>(itemData.AllowedFileTypes);
            allowedFileTypes.Sort();

            foreach (string allowedType in allowedFileTypes.Where(allFileTypes.Contains))
            {
                allFileTypes.Remove(allowedType);
            }

            var sourceList = new List<string>(allowedFileTypes);
            sourceList.AddRange(allFileTypes);

            _fileTypesList.DataSource = sourceList;
            _fileTypesList.DataBind();

            for (int i = 0; i < allowedFileTypes.Count; i++)
            {
                if (i >= _fileTypesList.Items.Count)
                {
                    break;
                }

                _fileTypesList.Items[i].Selected = true;
            }
        }


        /// <summary>
        /// Update the file upload item data
        /// </summary>
        /// <param name="itemData"></param>
        public void UpdateData(UploadItemData itemData)
        {
			itemData.Alias = _aliasText.Text;
            itemData.AllowedFileTypes.Clear();

            foreach (ListItem item in _fileTypesList.Items)
            {
                if (item.Selected)
                {
                    itemData.AllowedFileTypes.Add(item.Value);
                }
            }

            itemData.IsRequired = _requiredChk.Checked;
        }
    }
}