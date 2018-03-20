using System;
using System.IO;
using System.Net;
using System.Web;
using Checkbox.Content;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Styles;
using Checkbox.Web;
using Page = System.Web.UI.Page;

namespace CheckboxWeb
{
    /// <summary>
    /// Generic page for writing out content to stream
    /// </summary>
    public partial class ViewContent : Page
    {
        /// <summary>
        /// Id of generic content item to show.  This is is used for getting data for images,
        /// files, movies, etc.
        /// </summary>
        [QueryParameter("ContentId")]
        public int? ContentId { get; set; }

        /// <summary>
        /// Id of image to show.  This is used mostly for image survey items and legacy cases where
        /// images were stored separately from other user uploaded content.
        /// </summary>
        [QueryParameter("ImageId")]
        public int? ImageId { get; set; }

        /// <summary>
        /// Id of style template to write out.
        /// </summary>
        [QueryParameter("st")]
        public int? StyleTemplateId { get; set; }

        /// <summary>
        /// Survey render mode
        /// </summary>
        [QueryParameter("mode")]
        public RenderMode? RenderMode { get; set; }

        /// <summary>
        /// Override init to read parameter values
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WebParameterAttribute.SetValues(this, HttpContext.Current);
        }

        /// <summary>
        /// Write out requested data.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (ContentId.HasValue)
                {
                    WriteContent(ContentId.Value);
                    return;
                }

                if (ImageId.HasValue)
                {
                    WriteImage(ImageId.Value);
                    return;
                }

                if (StyleTemplateId.HasValue)
                {
                    WriteStyleTemplate(StyleTemplateId.Value);
                    return;
                }
            }
            catch
            { /*Ignore to prevent errors*/ }
            finally
            {
                Response.End();
            }
        }

        /// <summary>
        /// Write data for a content item to response stream.
        /// </summary>
        /// <param name="contentItemId"></param>
        private void WriteContent(int contentItemId)
        {
            var item = DBContentManager.GetItem(contentItemId);
            var itemData = item.Data;

            if (itemData == null)
            {                
                try
                {            
                    if (String.IsNullOrEmpty(item.ItemUrl))
                    {
                        //Attempt to read the file
                        var filePath = Request.PhysicalApplicationPath + ApplicationManager.AppSettings.UploadedImagesFolder + "\\" + item.ItemName;
                        var file = File.OpenRead(filePath);

                        itemData = new byte[file.Length];

                        file.Read(itemData, 0, itemData.Length);
                        file.Close();
                    }
                    else
                    {
                        //Attempt to download the data
                        var webRequest = (HttpWebRequest)WebRequest.Create(item.ItemUrl);
                        var webResponse = (HttpWebResponse)webRequest.GetResponse();

                        var stream = webResponse.GetResponseStream();

                        itemData = new byte[webResponse.ContentLength];
                        stream.Read(itemData, 0, itemData.Length);
                        stream.Close();
                        webResponse.Close();   
                    }                    
                }
                catch (Exception)
                {
                    itemData = new byte[0];
                }
            }

            Response.ContentType = item.ContentType;
            Response.AddHeader("content-disposition", "inline;filename=" + item.ItemName);
            Response.AddHeader("content-length", itemData.Length.ToString());

            Response.BinaryWrite(itemData);
        }

        /// <summary>
        /// Write data for an image to the response stream
        /// </summary>
        /// <param name="imageId"></param>
        private void WriteImage(int imageId)
        {
            string contentType;
            Response.BinaryWrite(ImageHelper.GetImageData(imageId, out contentType));
            Response.ContentType = contentType;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool isMobileBrowser
        {
            get { return RenderMode.HasValue && RenderMode.Value == Checkbox.Forms.RenderMode.SurveyMobilePreview; }
        }

        /// <summary>
        /// Write data for a style template
        /// </summary>
        /// <param name="styleTemplateId"></param>
        private void WriteStyleTemplate(int styleTemplateId)
        {
            StyleTemplate st = StyleTemplateManager.GetStyleTemplate(styleTemplateId);

            if (st != null)
            {
                string css = isMobileBrowser ? st.GetCssForMobile() : st.GetCss();

                Response.ContentType = "text/css";
                Response.Write(css);
            }
        }
    }
}
