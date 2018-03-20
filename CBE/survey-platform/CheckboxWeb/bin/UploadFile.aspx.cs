using System;
using System.IO;
using System.Text;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web.Page;
using Checkbox.Web;
using Prezza.Framework.Data;

namespace CheckboxWeb
{
    /// <summary>
    /// Handle uploading file.  Currently works just for images, but could be made more generic.
    /// </summary>
    public partial class UploadFile : ApplicationPage
    {
        /// <summary>
        /// 
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        /// Callback to run on client.  Assume running in iFrame so callback will be run as 
        /// parent.[CALLBACK]([NEWIMAGEID]);
        /// </summary>
        [QueryParameter("cc")]
        public string ClientCallback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var imageIdParam = Utilities.AsInt(Request["i"]);

            FileId = imageIdParam.HasValue ? imageIdParam.Value : -1;

            //Ensure user is logged-in
            //if (UserManager.GetCurrentPrincipal() == null)
            //{
              //  throw new Exception("User not logged in.");
            //}

            _uploadBtn.Click += _uploadBtn_Click;
            _uploadNewImageBtn.Click += _uploadNewImageBtn_Click;

            //Load the path to jQuery
            var jsInclude = new StringBuilder();

            //jQuery
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/jquery-latest.min.js\"></script>"));

            //jSON for stringify for service posts
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/json2.min.js\"></script>"));

            //jQuery UI
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/jquery-ui-1.10.2.custom.min.js\"></script>"));

            //dialogHandler
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/DialogHandler.js\"></script>"));

            //jqueryTemplates
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/jquery.tmpl.min.js\"></script>"));

            //textHelper
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/textHelper.js\"></script>"));

            //fileupload
            jsInclude.Append("<script language=\"javascript\" type=\"text/javascript\" src=\"");
            jsInclude.Append(ResolveUrl("~/Resources/jquery.fileupload.js\"></script>"));

            //Emit script
            _jsInclude.Text = jsInclude.ToString();

            Page.ClientScript.RegisterStartupScript(
                GetType(),
                "appRoot",
                "var _appRoot = " + ResolveUrl("~"),
                true);

            EventHandlerWrapper(InitView);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _uploadNewImageBtn_Click(object sender, EventArgs e)
        {
            EventHandlerWrapper(OnUploadNewImageClick);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _uploadBtn_Click(object sender, EventArgs e)
        {
            EventHandlerWrapper(OnUploadClick);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitView()
        {
            if (FileId <= 0)
            {
                _selectFilePanel.Visible = true;
                _uploadedPanel.Visible = false;
                return;
            }

            _selectFilePanel.Visible = false;
            _uploadedPanel.Visible = true;

            try
            {
                using (var reader = DbUtility.GetImage(FileId))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            _fileNameLbl.Text = DbUtility.GetValueFromDataReader(reader, "ImageName", "Image " + FileId);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch
            {
                _selectFilePanel.Visible = true;
                _uploadedPanel.Visible = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnUploadClick()
        {
            if (!_fileUpload.HasFile)
            {
                return;
            }

            _fileNameLbl.Text = _fileUpload.FileName;
            
            _selectFilePanel.Visible = false;
            _uploadedPanel.Visible = true;

            //Get uploaded file content as a byte array
            byte[] byteArray = _fileUpload.FileBytes;
            Guid imageGuid = Guid.NewGuid();

            int imageId;

            //Now, save the file to disk or to memory and update the preview image.
            if (ApplicationManager.AppSettings.StoreImagesInDatabase)
            {
                //Save image to database and update the ImageUrl property
                imageId = DbUtility.SaveImage(byteArray, _fileUpload.PostedFile.ContentType, null, Path.GetFileName(_fileUpload.FileName), imageGuid.ToString());
            }
            else
            {
                //Save image to upload file location & register in db
                string fileName = imageGuid + Path.GetExtension(_fileUpload.FileName);
                string filePath = ApplicationManager.AppSettings.UploadedImagesFolder + "/" + fileName;

                FileStream stream = File.Exists(filePath) ? File.OpenWrite(filePath) : File.Create(filePath);

                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();

                //Register
                imageId = DbUtility.SaveImage(
                    null,
                     _fileUpload.PostedFile.ContentType,
                    ApplicationManager.AppSettings.UploadedImagesUrl + "/" + fileName,
                    Path.GetFileName(fileName),
                    imageGuid.ToString());
            }

            //
            FileId = imageId;

            //Restore view
            InitView();


            //
            if (Utilities.IsNotNullOrEmpty(ClientCallback))
            {
                Page.ClientScript.RegisterStartupScript(
                    GetType(),
                    "ClientImageUploadedCallback",
                    string.Format("parent.{0}({1});", ClientCallback, FileId),
                    true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnUploadNewImageClick()
        {
            _selectFilePanel.Visible = true;
            _uploadedPanel.Visible = false;
        }
    }
}