using System;
using System.Web;
using Checkbox.Panels;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Web.Security;

namespace CheckboxWeb.Users.EmailLists
{
    public class EmailListEditorPage : SecuredPage
    {
        private EmailListPanel _panel;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Int32 panelID = Convert.ToInt32(GetQueryStringValue("p", "-1"));

            if (panelID > 0)
            {
                _panel = PanelManager.GetPanel(panelID) as EmailListPanel;
            }
        }

        #region Properties

        protected EmailListPanel EmailList
        {
            get { return _panel; }
        }

        #endregion

        protected override Prezza.Framework.Security.IAccessControllable GetControllableEntity()
        {
            return _panel;
        }

        protected string GetRequiredPermissionForAclEdit()
        {
            return "EmailList.Edit";
        }

        /// <summary>
        /// Handles the click event of the edit permissions action link
        /// - Redirects to the security editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EditPermissionsLink_Click(object sender, EventArgs e)
        {
            RedirectToAclEditor();
        }

        /// <summary>
        /// Get CSV list of permissions to grant on ACL
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAclPermissionsToGrant()
        {
            return string.Empty;
        }

        /// <summary>
        /// Redirect to the acl editor
        /// </summary>
        protected virtual void RedirectToAclEditor()
        {
            StoreSecurityEditorData();

            Response.Redirect(ResolveUrl("~/Security/SecurityEditor.aspx"), false);
        }


        /// <summary>
        /// Store security editor data in session.
        /// </summary>
        protected virtual void StoreSecurityEditorData()
        {
            var editorData = new SecurityEditorData
            {
                SecuredResourceType = SecuredResourceType.EmailList,
                SecuredResourceId = EmailList.ID.Value,
                AclPermissionsToGrant = GetAclPermissionsToGrant(),
                Context = SiteMap.CurrentNode != null ? SiteMap.CurrentNode.Url : string.Empty,
                RequiredPermission = GetRequiredPermissionForAclEdit(),
                SecuredResourceDefaultPolicyId = EmailList.DefaultPolicyID.Value,
                SecuredResourceName = EmailList.Name
            };

            Session["SecurityContextData"] = editorData;
        }
    }
}
