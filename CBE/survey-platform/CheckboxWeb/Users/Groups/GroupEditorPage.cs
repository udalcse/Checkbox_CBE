﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web.Page;
using Checkbox.Users;
using Checkbox.Web.Security;

namespace CheckboxWeb.Users.Groups
{
    public class GroupEditorPage : SecuredPage
    {
        private Group _group;

        /// <summary>
        /// 
        /// </summary>
        protected override string ControllableEntityRequiredPermission { get { return "Group.Edit"; } }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Int32 groupID = Convert.ToInt32(GetQueryStringValue("g", "-1"));

            if (groupID > 0)
            {
                _group = GroupManager.GetGroup(groupID);
            }
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected Group Group
        {
            get { return _group; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Prezza.Framework.Security.IAccessControllable GetControllableEntity()
        {
            return _group;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetRequiredPermissionForAclEdit()
        {
            return "Group.Edit";
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
                SecuredResourceType = SecuredResourceType.UserGroup,
                SecuredResourceId = Group.ID.Value,
                AclPermissionsToGrant = GetAclPermissionsToGrant(),
                Context = SiteMap.CurrentNode != null ? SiteMap.CurrentNode.Url : string.Empty,
                RequiredPermission = GetRequiredPermissionForAclEdit(),
                SecuredResourceDefaultPolicyId = Group.DefaultPolicyID.Value,
                SecuredResourceName = Group.Name
            };

            Session["SecurityContextData"] = editorData;
        }

    }
}
