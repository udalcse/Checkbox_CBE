using System;
using System.Web;
using Checkbox.Web.Security;
using System.Text;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Base class for pages that are used to edit access controllable resources.  This class handles redirecting
    /// to the ACL editor page by populating proper session variables, etc.
    /// </summary>
    public abstract class AccessControllableEditor : SecuredPage
    {
        /// <summary>
        /// Redirect to the acl editor
        /// </summary>
        protected virtual void RedirectToAclEditor()
        {
            StoreSecurityEditorData();

            Response.Redirect(GetAccessListEditorUrl(), false);
        }

        /// <summary>
        /// Store security editor data in session.
        /// </summary>
        protected void StoreSecurityEditorData()
        {
            GetAclEditorReferrerUrl();

            var editorData = new SecurityEditorData(
                GetControllableEntity(),
                GetAclEditorReferrerUrl(),
                SiteMap.CurrentNode != null ? SiteMap.CurrentNode.Url : string.Empty,
                GetRequiredPermissionForAclEdit());

            Session["SecurityContextData"] = editorData;
        }

        /// <summary>
        /// Get the required permission for editing acls
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRequiredPermissionForAclEdit()
        {
            return string.Empty;
        }

        /// <summary>
        /// The title of the item being edited
        /// </summary>
        protected abstract string EntityTitle { get; }

        /// <summary>
        /// The section of the nav menu to be highlighted
        /// </summary>
        protected abstract string ActiveNavSection { get; }

        /// <summary>
        /// Get the URL for the access list editor
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAccessListEditorUrl()
        {
            var url = new StringBuilder(ResolveUrl("~/Security/SecurityEditor.aspx"));
            if (!String.IsNullOrEmpty(EntityTitle))
            {
                url.Append("?t=");
                url.Append(Server.UrlEncode(EntityTitle));
            }
            if (!String.IsNullOrEmpty(ActiveNavSection))
            {
                url.Append(!String.IsNullOrEmpty(EntityTitle) ? "&an=" : "?an=");
                url.Append(Server.UrlEncode(ActiveNavSection));
            }

            return url.ToString();
        }

        /// <summary>
        /// Get the referrer url to use when redirecting to the acl editor
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAclEditorReferrerUrl()
        {
            return Request.Url.PathAndQuery;
        }

        /// <summary>
        /// Put the SecurityEditorData into the Session for pages that need to access more than one ACL Editor
        /// </summary>
        protected virtual void SetSecuritySessionData()
        {
            var editorData = new SecurityEditorData(
                GetControllableEntity(),
                GetAclEditorReferrerUrl(),
                SiteMap.CurrentNode != null ? SiteMap.CurrentNode.Url : string.Empty,
                GetRequiredPermissionForAclEdit());

            Session["SecurityContextData"] = editorData;
        }
    }
}
