using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Content;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Image selector control
    /// </summary>
    public partial class ImageSelector : Checkbox.Web.Common.UserControlBase
    {
        ///NOTE: ImageId vs ContentId
        ///  Previous versions of Checkbox uploaded images to ckbx_Image table, whereas new version
        ///   usings ckbx_Content_Items table.   Image id lets us know that this is likely an item from
        ///   a previous version so we deal with it for backwards compatibility.

        /// <summary>
        /// Javascript to call when image uploaded
        /// </summary>
        public string OnClientImageUploaded { get; set; }


        /// <summary>
        /// Get/set whether page postback or not.
        /// </summary>
        private bool IsPagePostBack { get; set; }

        /// <summary>
        /// Get image url
        /// </summary>
        public string ImageUrl { get { return _imageUrlTxt.Text.Trim(); } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int? GetImageId()
        {
            int? result = null;

            if (_currentEditMode.Text == "1")
            {
                //Parse image id, if any
                var startIndex = ImageUrl.IndexOf("imageId=", StringComparison.InvariantCultureIgnoreCase);

                if (startIndex > 0)
                {
                    result = Utilities.AsInt(ImageUrl.Substring(startIndex + "imageId=".Length));
                }

                startIndex = ImageUrl.IndexOf("contentId=", StringComparison.InvariantCultureIgnoreCase);

                if (startIndex > 0)
                {
                    result = Utilities.AsInt(ImageUrl.Substring(startIndex + "contentId=".Length));
                }
            }

            if (_currentEditMode.Text == "2")
            {
                int temp;
                result = int.TryParse(_imageList.SelectedValue, out temp) ? (int?)temp : null;
            }

            return result;
        }

        /// <summary>
        /// Bind event handlers and show any messages related to configuration issues.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _updatePreviewBtn.Click += _updatePreviewBtn_Click;
            _fileUploadedBtn.Click += _fileUploadedBtn_Click;


            PopulateImagesList();
        }


        /// <summary>
        /// Update preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _updatePreviewBtn_Click(object sender, EventArgs e)
        {
            UpdateUrlAndPreview();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _fileUploadedBtn_Click(object sender, EventArgs e)
        {
            //Save image data 
            var imageBytes = Upload.GetDataForFile(HttpContext.Current, _uploadedFilePathTxt.Text);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                return;
            }

            var currentPrincipal = UserManager.GetCurrentPrincipal();
            var dbContentFolder = DBContentManager.GetFolder("/Images", currentPrincipal);
            DBContentItem newItem = null;
            if (ApplicationManager.AppSettings.StoreImagesInDatabase)
            {
                newItem = new DBContentItem(null)
                                  {
                                      ContentType = string.Empty,
                                      CreatedBy = currentPrincipal.Identity.Name,
                                      ItemName = _uploadedFileNameTxt.Text,
                                      Data = imageBytes
                                  };

                dbContentFolder.AddContentItem(newItem);

                newItem.ItemUrl = string.Format("/ViewContent.aspx?contentID={0}", 
                    newItem.ItemID);

                newItem.CreatedBy = ((CheckboxPrincipal)HttpContext.Current.User).Identity.Name;
                newItem.Save();
            }
            else
            {
                //Save image to upload file location & register in db
                Guid imageGuid = Guid.NewGuid();

                string fileName = imageGuid + Path.GetExtension(_uploadedFileNameTxt.Text);
                string filePath = Request.PhysicalApplicationPath + ApplicationManager.AppSettings.UploadedImagesFolder + "\\" + fileName;                

                FileStream stream = File.Exists(filePath) ? File.OpenWrite(filePath) : File.Create(filePath);

                stream.Write(imageBytes, 0, imageBytes.Length);
                stream.Close();

                newItem = new DBContentItem(null)
                {
                    ContentType = string.Empty,
                    CreatedBy = currentPrincipal.Identity.Name,                    
                    ItemName = fileName
                };

                dbContentFolder.AddContentItem(newItem);                
            }

            _imageUrlTxt.Text = string.Format("/ViewContent.aspx?contentID={0}",
                    newItem.ItemID);
            _fileUploadedPanel.Visible = true;

            //Update image preview
            UpdateUrlAndPreview();
        }


        /// <summary>
        /// Set initial tab onload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _fileUploadedPanel.Visible = false;

            if (!IsPagePostBack)
            {
                //Set initial tab depending on whether upload is availalbe
                _currentEditMode.Text = Utilities.IsNullOrEmpty(ImageUrl) ? "0" : "1";
            }

            UpdateUrlAndPreview();


            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// 
        /// </summary>
        private void PopulateImagesList()
        {
            _imageList.Items.Clear();

            _imageList.Items.Add(new ListItem(WebTextManager.GetText("/pageText/styles/forms/backgroundImage.aspx/noneSelected"), "NONE_SELECTED"));

            //Get the images
            Dictionary<string, ContentItem> contentItems = DBContentManager.ListItems("/Images", HttpContext.Current.User as CheckboxPrincipal);

            foreach (ContentItem item in contentItems.Values.OrderBy(ci => ci.ItemName))
            {
                if (item is DBContentItem)
                {
                    _imageList.Items.Add(new ListItem(
                        item.ItemName,
                        ((DBContentItem)item).ItemID.ToString()));
                }
            }
        }
        /// <summary>
        /// Update image preview
        /// </summary>
        private void UpdateUrlAndPreview()
        {
            if (_currentEditMode.Text == "2")
            {
                var imageSelected = _imageList.SelectedValue != "NONE_SELECTED";
                _noPreviewMsgPanel.Visible = !imageSelected;
                _previewPanel.Visible = imageSelected;

                if (imageSelected)
                {
                    _imageUrlTxt.Text =  "/ViewContent.aspx?contentId=" + _imageList.SelectedValue;
                    _previewImage.ImageUrl =  "/ViewContent.aspx?contentId=" + _imageList.SelectedValue;
                }
            }
            else
            {
                if (Utilities.IsNotNullOrEmpty(ImageUrl))
                {
                    _previewImage.ImageUrl = ImageUrl;
                    _previewPanel.Visible = true;
                    _noPreviewMsgPanel.Visible = false;
                    _currentEditMode.Text = "1";
                }
                else
                {
                    _previewPanel.Visible = false;
                    _noPreviewMsgPanel.Visible = true;
                    _currentEditMode.Text = "0";
                }
            }
        }


        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="isPagePostBack">if set to <c>true</c> [is page post back].</param>
        public void Initialize(string imageUrl, bool isPagePostBack)
        {
            _imageUrlTxt.Text = imageUrl != "none" ? imageUrl : string.Empty;
            IsPagePostBack = isPagePostBack;
        }
    }
}