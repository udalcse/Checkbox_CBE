using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Prezza.Framework.Data;
using Checkbox.Content;
using CheckboxWeb.Settings;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class UploadImage : SecuredPage
    {
        [QueryParameter("optionId")]
        public int? OptionId { get; set; }

        [QueryParameter("imageId")]
        public int? ImageId { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }

        [QueryParameter("redirect")]
        public string RedirectTo { get; set; }

        /// <summary>
        /// Initialize page's components
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Master.OkTextId = "/pageText/uploadImage.aspx/setTheImage";
            Master.SetTitle(WebTextManager.GetText("/pageText/uploadImage.aspx/uploadImage"));
            Master.OkClick += Master_OkClick;
            Master.CancelClick += Master_CancelClick;

            _imageSelector.Initialize(ImageId.HasValue ? ResolveUrl("~/ViewContent.aspx?ContentId=" + ImageId.Value) : String.Empty, IsPostBack);
        }

        void Master_CancelClick(object sender, EventArgs e)
        {
            Dictionary<String, String> args = new Dictionary<string, string>();
            args.Add("op", "cancel");

            if (IsNew.HasValue && IsNew.Value)
                ReturnToEditItemPage(args);
            else
                Master.CloseDialog(null);
        }

        /// <summary>
        /// Override OnLoad to add jScript
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Return to edit item page. This method is used when an image is added just after item creation.
        /// </summary>
        private void ReturnToEditItemPage(Dictionary<string, string> args)
        {
            String url;
            if (!String.IsNullOrEmpty(RedirectTo) && RedirectTo == "library")
            {
                 url = ResolveUrl("~/Libraries/EditItem.aspx?");
            }
            else
                 url = "EditItem.aspx?";
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "c" &&
                    key != "showOptionsTab" &&
                    key != "op" &&
                    key != "optionId" &&
                    key != "imageId" &&
                    key != "imageUrl" &&
                    key != "imageName")
                    url += key + "=" + Request.QueryString[key] + "&";
            }
            foreach (var arg in args)
            {
                url += arg.Key + "=" + arg.Value + "&";
            }
            url += "showOptionsTab=true";

            Response.Redirect(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_OkClick(object sender, EventArgs e)
        {
            int? imageId = _imageSelector.GetImageId();
            string imageUrl = string.Empty;
            string imageName = string.Empty;

            if (!imageId.HasValue && !String.IsNullOrEmpty(_imageSelector.ImageUrl))
            {                
                var imageBytes = Upload.GetBytesFromUrl(_imageSelector.ImageUrl);

                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return;
                }

                var currentPrincipal = UserManager.GetCurrentPrincipal();
                var dbContentFolder = DBContentManager.GetFolder("/Images", currentPrincipal);
                DBContentItem newItem = new DBContentItem(null)
                {
                    ContentType = string.Empty,
                    CreatedBy = currentPrincipal.Identity.Name,
                    ItemName = Path.GetFileName(_imageSelector.ImageUrl),
                    Data = imageBytes
                };

                dbContentFolder.AddContentItem(newItem);

                newItem.ItemUrl = string.Format("{0}/ViewContent.aspx?contentID={1}", ApplicationManager.ApplicationRoot,
                    newItem.ItemID);

                newItem.CreatedBy = User.Identity.Name;
                newItem.Save();

                imageUrl = newItem.ItemUrl;
                imageName = newItem.ItemName;
                imageId = newItem.ItemID;
            }

            Dictionary<String, String> args = new Dictionary<string, string>();

            //If there is no image id, close dialog and do nothing.
            
            if (!imageId.HasValue)
            {
                args.Add("op", "cancel");

                if (IsNew.HasValue && IsNew.Value)
                    ReturnToEditItemPage(args);

                Master.CloseDialog("imageUploaded", args);
                return;
            }
            
            args.Add("op", "imageUploaded");
            args.Add("optionId", OptionId.ToString());
            args.Add("imageId", imageId.ToString());
            args.Add("imageUrl", imageUrl);
            args.Add("imageName", imageName);

            if (IsNew.HasValue && IsNew.Value)
                ReturnToEditItemPage(args);

            Master.CloseDialog("imageUploaded", args);
        }

    }
}