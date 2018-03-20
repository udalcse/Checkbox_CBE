using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CheckboxWeb.Controls.Navigation
{
    public partial class NavMenu : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Determines if a given menu item should be highlighted
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        protected Boolean IsSectionActive(SiteMapNode currentNode)
        {
            //First check if a menu item has been manually specified, if so, highlight it
            if ((!String.IsNullOrEmpty(ActiveMenuItem)) && String.Compare(ActiveMenuItem, currentNode.Title, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return true;
            }

            //Next check if current page is explicitly in the sitemap file
            if (SiteMap.CurrentNode != null)
            {
                return SiteMap.CurrentNode.Equals(currentNode) || SiteMap.CurrentNode.IsDescendantOf(currentNode);
            }

            //Finally, try to use the path of the current page to highlight to corresponding menu item
            //We'll consider a page to be a child of the current sitemap node if it lives in the same directory or a child directory
            string currentPath = Request.Path;
            string siteMapPath = currentNode.Url.Substring(0, currentNode.Url.LastIndexOf('/'));
            if (currentPath.Contains(siteMapPath))
            {
                return true;
            }

            return false;
        }

        public string ActiveMenuItem { get; set; }
    }
}