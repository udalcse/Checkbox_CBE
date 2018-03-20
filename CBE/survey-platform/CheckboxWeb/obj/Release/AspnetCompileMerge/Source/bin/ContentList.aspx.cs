using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Checkbox.Content;
using Checkbox.Users;

namespace CheckboxWeb
{
    public partial class ContentList : Page
    {
        /// <summary>
        /// 
        /// </summary>
        private string ContentType
        {
            get { return Request.QueryString["contentType"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                Response.ContentType = "text/javascript";

                var listName = "images".Equals(ContentType, StringComparison.InvariantCultureIgnoreCase)
                                   ? "tinyMCEImageList"
                                   : "video".Equals(ContentType, StringComparison.InvariantCultureIgnoreCase)
                                         ? "tinyMCEMediaList"
                                         : "tinyMCELinkList";

                var contentItems = ListContentItems(ContentType);
                int counter = 0;

                //Write items
                Response.Write("var " + listName + " = new Array(");
                Response.Write(Environment.NewLine);

                foreach (var contentItem in contentItems)
                {
                    if (counter > 0)
                    {
                        Response.Write(",");
                        Response.Write(Environment.NewLine);
                    }
                    
                    Response.Write(string.Format("['{0}', '{1}']", contentItem.ItemName, contentItem.ItemUrl));
                    counter++;
                }

                //Close array
                Response.Write(Environment.NewLine);
                Response.Write(");");
            }
            catch
            {
                Response.ClearContent();
                Response.Write("var tinyMCEImageList = new Array();");
            }
            finally
            {
                Response.End();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ContentItem> ListContentItems(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return new List<ContentItem>();
            }

            var userPrincipal = UserManager.GetCurrentPrincipal();

            if ("images".Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
            {
                return DBContentManager
                    .ListItems("/images", userPrincipal)
                    .Values
                    .OrderBy(item => item.ItemName)
                    .ToList();
            }
            
            if ("video".Equals(contentType, StringComparison.InvariantCultureIgnoreCase))
            {
                //For upgrades, include no-longer-used flash and media folders
                var flashList = DBContentManager.ListItems("/flash", userPrincipal);
                var mediaList = DBContentManager.ListItems("/media", userPrincipal);
                var videoList = DBContentManager.ListItems("/video", userPrincipal);

                foreach (var flashItem in flashList)
                {
                    videoList[flashItem.Key] = flashItem.Value;
                }

                foreach (var mediaItem in mediaList)
                {
                    videoList[mediaItem.Key] = mediaItem.Value;
                }

                return videoList
                    .Values
                    .OrderBy(item => item.ItemName)
                    .ToList();
            }

            //Otherwise assume simply listing links and include all other stuff.
            var documentList = DBContentManager.ListItems("/documents", UserManager.GetCurrentPrincipal());
            var audioList = DBContentManager.ListItems("/audio", UserManager.GetCurrentPrincipal());

            foreach (var audioItem in audioList)
            {
                documentList[audioItem.Key] = audioItem.Value;
            }

            return documentList
                .Values
                .OrderBy(item => item.ItemName)
                .ToList();
        }
    }
}