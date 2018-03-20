using System;
using System.IO;
using System.Web;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Content;
using Checkbox.Forms;
using Checkbox.Web.Page;

namespace CheckboxWeb
{
    public partial class PostFile : SecuredPage
    {
        /// <summary>
        ///
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Form.Edit"; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _fileUploadedBtn.Click +=_fileUploadedBtn_Click;

            Master.HideDialogButtons();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileUploadedBtn_Click(object sender, EventArgs e)
        { 
            //Save image data 
            var fileBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);
            var fileName = _uploadedFileNameTxt.Text;

            if (fileBytes == null || fileBytes.Length == 0 || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            UploadedFileType fileType;
            var mimeType = UploadItemManager.DetermineContentType(fileName, out fileType);

            string path = "/documents";

            if (fileType == UploadedFileType.Audio)
            {
                path = "/audio";
            }

            if (fileType == UploadedFileType.Video)
            {
                path = "/video";
            }

            if (fileType == UploadedFileType.Image)
            {
                path = "/images";
            }

            DBContentFolder folder = DBContentManager.GetFolder(path, UserManager.GetCurrentPrincipal());

            if (folder != null)
            {
                var item = new DBContentItem(null)
                               {
                                   ItemName = fileName, 
                                   Data = fileBytes, 
                                   CreatedBy = UserManager.GetCurrentPrincipal().Identity.Name,
                                   ContentType = mimeType
                               };

                folder.AddContentItem(item);

                item.ItemUrl = ApplicationManager.ApplicationHost + "/ViewContent.aspx?contentID=" + item.ItemID;
                item.Save();

                _previewPlace.Text = fileType == UploadedFileType.Image 
                    ? item.ItemUrl 
                    : string.Format("<a href=\"{0}\">{1}</a>", item.ItemUrl, item.ItemName);

                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "returnToEditor",
                    "returnToEditor('" + fileType + "', '" + item.ItemID + "');",
                    true);
            }
        }
    }
}